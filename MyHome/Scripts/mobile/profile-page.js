
var profilePage = angular.module('profilePage', ['ngRoute', 'ngSanitize', 'ngAnimate', 'ngTouch', 'AppManagerService', 'ClassyUtilitiesService', 'LocalizerService']);

profilePage.directive('classyScrollable', function(ClassyUtilities) {
    var offset = 0;

    function handleDrag(ev) {
        if (ev.type == 'dragstart') {
            $(ev.currentTarget).css('transition', 'none');
            var transform = window.getComputedStyle(ev.currentTarget).webkitTransform;
            offset = !transform || transform == 'none' ? 0 : parseInt(transform.split(',')[4]);
            return;
        }
        if (ev.type == 'release') {
            var elem = $(ev.currentTarget);
            var transform = window.getComputedStyle(ev.currentTarget).webkitTransform;
            var currentOffset = !transform || transform == 'none' ? 0 : parseInt(transform.split(',')[4]);
            var maxOffset = ev.currentTarget.scrollWidth - ClassyUtilities.Screen.GetWidth();
            if (currentOffset >= 0) {
                requestAnimationFrame(function() {
                    elem.css('transition', '-webkit-transform 0.5s ease');
                    elem.css('-webkit-transform', 'translate3d(0,0,0)');
                });
            } else if (Math.abs(currentOffset) >= maxOffset) {
                requestAnimationFrame(function() {
                    elem.css('transition', '-webkit-transform 0.5s ease');
                    elem.css('-webkit-transform', 'translate3d(-' + maxOffset + 'px,0,0)');
                });
            }
            return;
        }
        
        ev.gesture.preventDefault();
        
        var drag = ev.gesture.deltaX + offset;

        $(ev.currentTarget).css('-webkit-transform', 'translate3d(' + drag + 'px, 0, 0)');
    }

    return function (scope, element) {
        Hammer(element[0], { dragLockToAxis: true })
            .on("dragstart release dragleft dragright swipeleft swiperight", handleDrag);
    };
});

profilePage.factory('CacheProvider', function ($cacheFactory) {
    // we can add a cache limit here if we'll need to
    return $cacheFactory('HomeLab_Mobile_Cache');
});

profilePage.config(['$routeProvider', function($routeProvider) {
    $routeProvider
        .when('/Profile/:profileId', {
            templateUrl: 'profile-page.html',
            controller: 'ProfileController'
        }).when('/Collection/:collectionId', {
            templateUrl: 'collection.html',
            controller: 'CollectionController'
        }).when('/Collection/SlideShow/:collectionId/:photoId', {
            templateUrl: 'collection-slideshow.html',
            controller: 'PhotoController'
        });
}]);

profilePage.filter('unsafe', function ($sce) {
    return function (val) {
        return $sce.trustAsHtml(val);
    };
});

profilePage.controller('ProfileController', function ($scope, $http, AppSettings, ClassyUtilities, Localizer, $routeParams) {

    $scope.currentSlide = 0;
    $scope.nextSlide = function() {
        if ($scope.currentSlide == 1) return;
        $('.cover-slider').find('.pane2').show();
        $('.cover-slider').css('left', '0');
        $('.cover-slider').css('left', '-' + $('.cover-slider').width() + 'px');
        $scope.currentSlide++;
    };
    $scope.prevSlide = function() {
        if ($scope.currentSlide == 0) return;
        $('.cover-slider')
            .css('left', '0')
            .one('webkitTransitionEnd transitionend', function() {
                $('.cover-slider').find('.pane2').css('display', 'none');
            });
        $scope.currentSlide--;
    };

    AppSettings.then(function (appSettings) {

        var utilities = ClassyUtilities;
        var w = utilities.Screen.GetWidth();
        var h = utilities.Screen.GetHeight();

        var profileId = parseInt($routeParams.profileId);
        
        $http.get(appSettings.ApiUrl + '/profile/' + profileId + '?includeCollections=true&includeReviews=true', config).success(function(data) {
            $scope.profileDetails = data;

            // organize collections
            var collections = [];
            $scope.profileDetails.Collections.forEach(function(collection) {
                if (collection.CoverPhotos && collection.CoverPhotos.length > 0 && collection.CoverPhotos[0].trim() != '' && collection.Type == 'PhotoBook')
                    collections.push({
                        Id: collection.Id,
                        ImageUrl: utilities.Images.Thumbnail(appSettings, collection.CoverPhotos[0], 160, 160)
                    });
            });
            $scope.Collections = collections;
            // organize projects
            var projects = [];
            $scope.profileDetails.Collections.forEach(function(collection) {
                if (collection.CoverPhotos && collection.CoverPhotos.length > 0 && collection.CoverPhotos[0].trim() != '' && collection.Type == 'Project')
                    projects.push({
                        Id: collection.Id,
                        ImageUrl: utilities.Images.Thumbnail(appSettings, collection.CoverPhotos[0], 160, 160)
                    });
            });
            $scope.Projects = projects;
            
            $scope.Avatar = utilities.Images.Thumbnail(appSettings, data.Avatar.Key, 80, 80);
            $scope.Location = getProfileLocation(data);
            $scope.Rating = getRatingAsArray(data.ReviewAverageScore);
            $scope.BusinessDescription = data.Metadata.BusinessDescription;
            if (data.CoverPhotos && data.CoverPhotos.length > 0) {
                $scope.CoverPhotos = [];
                data.CoverPhotos.forEach(function(imageKey) {
                    $scope.CoverPhotos.push(utilities.Images.Thumbnail(appSettings, imageKey, w, h));
                });
            } else {
                $scope.CoverPhotos = [ appSettings.Host + '/img/blueprint.jpg' ];
            }

            // reviews
            var reviews = [];
            data.Reviews.forEach(function(review) {
                reviews.push({
                    ReviewerId: review.ProfileId,
                    Name: review.ReviewerUsername,
                    Avatar: review.ReviewerThumbnailUrl,
                    Content: review.Content,
                    Rating: getRatingAsArray(review.Score)
                });
            });
            $scope.Reviews = reviews;

        }).error(function () {
            // TODO: display some error message
        });

        // get localized resources
        $scope.Resources = {};
        Localizer.Get('Mobile_ProfilePage_ViewAllProjects', AppSettings.Culture).then(function (resource) {
            $scope.Resources.ViewAllProjects = resource;
        });
        Localizer.Get('Mobile_ProfilePage_ViewAllCollections', AppSettings.Culture).then(function (resource) {
            $scope.Resources.ViewAllCollections = resource;
        });
        Localizer.Get('Mobile_ProfilePage_ViewAllReviews', AppSettings.Culture).then(function(resource) {
            $scope.Resources.ViewAllReviews = resource;
        });

        function getProfileLocation(profileDetails) {
            var professionalInfo = profileDetails.professionalInfo;
            if (!professionalInfo) return '';
            var contactInfo = professionalInfo.CompanyContactInfo;
            if (!contactInfo) return '';
            var location = contactInfo.Location;
            if (!location) return '';
            var address = location.Address;
            if (!address) return '';
            return address.City + ', ' + address.Country;
        }

        function getRatingAsArray(rating) {
            var ratings = [];
            for (var i = 0; i < 5; i++)
                ratings.push({ id: i, star: (rating > i ? true : false) });
            return ratings;
        }
    });
});

profilePage.controller('CollectionController', function($scope, $http, AppSettings, ClassyUtilities, Localizer, $routeParams, $location) {
    AppSettings.then(function(appSettings) {

        $scope.CollectionId = $routeParams.collectionId;
        $http.get(appSettings.ApiUrl + '/collection/' + $routeParams.collectionId + '?includeComments=true&includeCommenterProfiles=true&includeListings=true&increaseViewCounter=true&includeProfile=true', config).success(function (data) {

            $scope.Avatar = data.Profile.Avatar.Url;
            $scope.CollectionName = data.Title;
            $scope.ProfileName = data.Profile.UserName;

            $scope.ViewCount = data.ViewCount;
            $scope.FavoriteCount = data.FavoriteCount;
            $scope.CommentCount = data.CommentCount;

            var w = ClassyUtilities.Screen.GetWidth();
            var h = ClassyUtilities.Screen.GetHeight();
            if (data.CoverPhotos && data.CoverPhotos.length > 0) {
                $scope.CoverPhotos = [];
                data.CoverPhotos.forEach(function (imageKey) {
                    $scope.CoverPhotos.push(ClassyUtilities.Images.Thumbnail(appSettings, imageKey, w, h));
                });
            } else {
                $scope.CoverPhotos = [appSettings.Host + '/img/blueprint.jpg'];
            }

            var imageWidth = parseInt((ClassyUtilities.Screen.GetWidth() - (16 * 4)) / 3);
            var listings = [];
            data.Listings.forEach(function(listing) {
                listings.push({
                    Id: listing.Id,
                    Title: listing.Title,
                    ImageUrl: ClassyUtilities.Images.Thumbnail(appSettings, listing.ExternalMedia[0].Key, imageWidth, imageWidth)
                });
            });
            $scope.Listings = listings;

            var comments = [];
            data.Comments.forEach(function(comment) {
                comments.push({
                    Commenter: comment.Profile.UserName,
                    CommenterId: comment.ProfileId,
                    CommenterAvatarUrl: comment.Profile.Avatar.Url,
                    Content: comment.Content
                });
            });
            $scope.Comments = comments;

        }).error(function() {
            // TODO: display some error message
        });

        $scope.openSlideShow = function(collectionId, photoId) {
            $location.url('/Collection/SlideShow/' + collectionId + '/' + photoId);
        };

        // get localized resources
        $scope.Resources = {};
        Localizer.Get('Mobile_CollectionPage_ViewAllComments').then(function (resource) {
            $scope.Resources.ViewAllComments = resource;
        });

    });
});

profilePage.controller('PhotoController', function ($scope, $http, AppSettings, ClassyUtilities, Localizer, $routeParams) {
    AppSettings.then(function(appSettings) {

        $scope.ScreenHeight = ClassyUtilities.Screen.GetHeight();

        $http.get(appSettings.ApiUrl + '/collection/' + $routeParams.collectionId + '?includeListings=true&increaseViewCounter=true&includeProfile=true', config).success(function (data) {

            var listings = [];
            data.Listings.forEach(function(listing) {
                listings.push({
                    Title: listing.Title,
                    Description: listing.Content,
                    ImageUrl: listing.ExternalMedia[0].Url,
                    CopyrightMessage: (listing.Metadata.IsWebPhoto && listing.Metadata.IsWebPhoto == "True") ?
                        extractHostFromUrl(listing.Metadata.CopyrightMessage) :
                        listing.Metadata.CopyrightMessage ? listing.Metadata.CopyrightMessage : getProfileName(listing.Profile)
                });
            });
            $scope.Listings = listings;

            // TODO: check if we can get this from metadata instead!
            $scope.CopyrightMessage = data.CopyrightMessage;

        }).error(function() {
            // TODO: display some error message
        });

        function extractHostFromUrl(url) {
            var a = window.createElement('a');
            a.href = url;
            return a.hostname;
        }

        function getProfileName(profile) {
            if (!profile || profile == '') return '';
            if (!profile.ContactInfo && !profile.IsProfessional) return 'unknown';
            var name;
            if (profile.IsProxy) return profile.ProfessionalInfo.CompanyName;
            else if (profile.IsProfessional) name = profile.ProfessionalInfo.CompanyName;
            else name = (!profile.ContactInfo.Name || profile.ContactInfo.Name == '') ? profile.UserName : profile.ContactInfo.Name;
            if (name) return name;
            return 'unknown';
        }
    });
});