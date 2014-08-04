
classy.controller('ProfileController', function ($scope, $http, AppSettings, ClassyUtilities, Localizer, $routeParams, $timeout) {

    // enforce ssl
    if (window.location.protocol == 'http:') {
        window.location.href = window.location.href.replace('http://', 'https://');
    }

    ClassyUtilities.PageLoader.Show();
    ClassyUtilities.Screen.StaticViewport();

    $scope.currentSlide = 0;
    $scope.nextSlide = function() {
        if ($scope.currentSlide == 1) return;
        var slider = $('.cover-slider');
        var pane2 = slider.find('.pane2');
        pane2.css('display', 'inline-block');

        requestAnimationFrame(function () {
            slider.css('transition', '-webkit-transform 0.5s ease');
            slider.css('-webkit-transform', 'translate3d(-' + pane2.outerWidth(true) + 'px,0,0)');
            slider.one('webkitTransitionEnd', function() {
                slider.find('.pane1').css('display', 'none');
                slider.css('-webkit-transition', 'none');
                slider.css('transition', 'none');
                slider.css('-webkit-transform', 'none');
            });
        });
        $scope.currentSlide=1;
    };
    $scope.prevSlide = function() {
        if ($scope.currentSlide == 0) return;
        var slider = $('.cover-slider');
        var pane1 = slider.find('.pane1');
        var pane2 = slider.find('.pane2');
        requestAnimationFrame(function() {
            slider.css('-webkit-transform', 'translate3d(' + (pane2[0].getBoundingClientRect().left - pane1.outerWidth(true)) + 'px,0,0)');
            pane1.css('display', 'inline-block');
            requestAnimationFrame(function() {
                slider.css('transition', '-webkit-transform 0.5s ease');
                slider.css('-webkit-transform', 'translate3d(0,0,0)');
                slider.one('webkitTransitionEnd', function() {
                    pane2.css('display', 'none');
                });
            });
        });
        $scope.currentSlide=0;
    };

    $scope.showAllReviews = function() {
        $('.profile-reviews .review-container').removeClass('hidden');
        $('.profile-reviews .review-container:nth-child(2)').removeClass('last');
        $('.profile-reviews .panel-footer').addClass('hidden');
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
                        Name: collection.Title,
                        ImageUrl: utilities.Images.Thumbnail(appSettings, collection.CoverPhotos[0], 160, 160)
                    });
            });
            $scope.Collections = collections;
            // organize projects
            var projects = [];
            $scope.profileDetails.Collections.forEach(function(project) {
                if (project.CoverPhotos && project.CoverPhotos.length > 0 && project.CoverPhotos[0].trim() != '' && project.Type == 'Project')
                    projects.push({
                        Id: project.Id,
                        Name: project.Title,
                        ImageUrl: utilities.Images.Thumbnail(appSettings, project.CoverPhotos[0], 160, 160)
                    });
            });
            $scope.Projects = projects;

            $scope.ViewCount = data.ViewCount;
            $scope.FollowerCount = data.FollowerCount;
            $scope.FollowingCount = data.FollowingCount;

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

            $timeout(initProfileHeader, 0);
            $timeout(ClassyUtilities.PageLoader.Hide);

        }).error(function () {
            // TODO: display some error message
        });

        // get localized resources
        $scope.Resources = {};
        Localizer.Get('Mobile_ProfilePage_ViewAllProjects', AppSettings.Culture).then(function (resource) { $scope.Resources.ViewAllProjects = resource; });
        Localizer.Get('Mobile_ProfilePage_ViewAllCollections', AppSettings.Culture).then(function (resource) { $scope.Resources.ViewAllCollections = resource; });
        Localizer.Get('Mobile_ProfilePage_ViewAllReviews', AppSettings.Culture).then(function (resource) { $scope.Resources.ViewAllReviews = resource; });
        Localizer.Get('Mobile_ProfilePage_Views', AppSettings.Culture).then(function (resource) { $scope.Resources.Views = resource; });
        Localizer.Get('Mobile_ProfilePage_Followers', AppSettings.Culture).then(function (resource) { $scope.Resources.Followers = resource; });
        Localizer.Get('Mobile_ProfilePage_Following', AppSettings.Culture).then(function (resource) { $scope.Resources.Following = resource; });

        $scope.share = function (network) {
            var url = window.location.protocol + appSettings.Host + '/profile/' + $routeParams.profileId;
            Classy.Share(network, url);
        };

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

        function initProfileHeader() {
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

                    if (currentOffset >= 0 && $scope.currentSlide == 0) { // bounce back
                        requestAnimationFrame(function () {
                            elem.css('transition', '-webkit-transform 0.5s ease');
                            elem.css('-webkit-transform', 'translate3d(0,0,0)');
                        });
                    } else if (Math.abs(currentOffset) - maxOffset > 30 && $scope.currentSlide == 0) {
                        $scope.nextSlide();
                    } else if (currentOffset >= 30 && $scope.currentSlide == 1) {
                        $scope.prevSlide();
                    } else if (currentOffset <= -30 && $scope.currentSlide == 1) {
                        requestAnimationFrame(function () {
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

            Hammer($('.profile-header .cover-slider')[0], { dragLockToAxis: true }).on("dragstart release dragleft dragright swipeleft swiperight", handleDrag);
        }
    });
});
