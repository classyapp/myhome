
var profilePage = angular.module('profilePage', ['ngRoute', 'ngSanitize', 'ngAnimate', 'ngTouch', 'AppManagerService', 'ClassyUtilitiesService', 'LocalizerService']);

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
        });
}]);

//profilePage.value('appSettingsPromise', 'http://www.thisisclassy.com:8008'); // way to inject objects into module controllers
profilePage.filter('unsafe', function ($sce) {
    return function (val) {
        return $sce.trustAsHtml(val);
    };
});

profilePage.controller('ProfileController', function ($scope, $http, AppSettings, ClassyUtilities, Localizer, $routeParams) {
    AppSettings.then(function (appSettings) {

        var utilities = ClassyUtilities;
        var profileId = parseInt($routeParams.profileId);

        $http.get(appSettings.ApiUrl + '/profile/' + profileId + '?includeCollections=true&includeReviews=true', config).success(function(data) {
            $scope.profileDetails = data;

            // organize collections
            var collections = [];
            $scope.profileDetails.Collections.forEach(function(collection) {
                if (collection.CoverPhotos && collection.CoverPhotos.length > 0 && collection.CoverPhotos[0].trim() != '')
                    collections.push(utilities.Images.Thumbnails(appSettings, collection.CoverPhotos, 200, 200));
            });
            $scope.Collections = collections;
            
            $scope.Avatar = utilities.Images.Thumbnail(appSettings, data.Avatar.Key, 80, 80);
            $scope.Location = getProfileLocation(data);
            $scope.Rating = getRatingAsArray(data.ReviewAverageScore);

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
        Localizer.Get('Mobile_ProfilePage_ViewAllProjects').then(function (resource) {
            $scope.Resources.ViewAllProjects = resource;
        });
        Localizer.Get('Mobile_ProfilePage_ViewAllReviews').then(function(resource) {
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

profilePage.controller('CollectionController', function ($scope, $http, AppSettings, ClassyUtilities, Localizer, $routeParams) {
    $scope.direction = 'left';
    $scope.currentIndex = 0;

    $scope.setCurrentListingIndex = function (index) {
        $scope.direction = (index > $scope.currentIndex) ? 'left' : 'right';
        $scope.currentIndex = index;
    };

    $scope.isCurrentListingIndex = function (index) {
        return $scope.currentIndex === index;
    };

    $scope.prevListing = function () {
        $scope.direction = 'left';
        $scope.currentIndex = ($scope.currentIndex < $scope.Listings.length - 1) ? ++$scope.currentIndex : 0;
    };

    $scope.nextListing = function () {
        $scope.direction = 'right';
        $scope.currentIndex = ($scope.currentIndex > 0) ? --$scope.currentIndex : $scope.Listings.length - 1;
    };

    AppSettings.then(function (appSettings) {

        $http.get(appSettings.ApiUrl + '/collection/' + $routeParams.collectionId + '?includeListings=true&increaseViewCounter=true', config).success(function (data) {

            $scope.Listings = data.Listings;

        }).error(function () {
            // TODO: display some error message
        });

        // get localized resources
        $scope.Resources = {};
        //Localizer.Get('Mobile_ProfilePage_ViewAllProjects').then(function (resource) {
        //    $scope.Resources.ViewAllProjects = resource;
        //});

    });
}).animation('.slide-animation', function () {
    return {
        addClass: function (element, className, done) {
            var scope = element.scope();

            if (className == 'ng-hide') {
                var finishPoint = element.parent().width();
                if (scope.direction !== 'right') {
                    finishPoint = -finishPoint;
                }
                TweenMax.to(element, 0.5, { left: finishPoint, onComplete: done });
            }
            else {
                done();
            }
        },
        removeClass: function (element, className, done) {
            var scope = element.scope();

            if (className == 'ng-hide') {
                element.removeClass('ng-hide');

                var startPoint = element.parent().width();
                if (scope.direction === 'right') {
                    startPoint = -startPoint;
                }

                TweenMax.set(element, { left: startPoint });
                TweenMax.to(element, 0.5, { left: 0, onComplete: done });
            }
            else {
                done();
            }
        }
    };
});