
classy.controller('ProfileController', function ($scope, $http, AppSettings, ClassyUtilities, Localizer, $routeParams) {
    ClassyUtilities.Screen.StaticViewport();

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

            $scope.ViewCount = data.ViewCount;
            $scope.CommentCount = data.CommentCount;
            $scope.ReviewCount = data.ReviewCount;

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
