
classy.controller('CollectionController', function ($scope, $http, AppSettings, ClassyUtilities, Localizer, $routeParams, $location) {
    ClassyUtilities.Screen.StaticViewport();

    $scope.showAllComments = function () {
        $('.profile-comments .comment-container').removeClass('hidden');
        $('.profile-comments .comment-container:nth-child(2)').removeClass('last');
        $('.profile-comments .panel-footer').addClass('hidden');
    };

    AppSettings.then(function (appSettings) {

        $scope.CollectionId = $routeParams.collectionId;
        $http.get(appSettings.ApiUrl + '/collection/' + $routeParams.collectionId + '?includeComments=true&includeCommenterProfiles=true&includeListings=true&increaseViewCounter=true&includeProfile=true', config).success(function (data) {

            $scope.Avatar = data.Profile.Avatar.Url;
            $scope.CollectionName = data.Title;
            $scope.ProfileName = data.Profile.UserName;
            $scope.ProfileId = data.Profile.Id;

            $scope.ViewCount = data.ViewCount;
            $scope.FavoriteCount = data.FavoriteCount;
            $scope.CommentsCount = data.CommentCount;

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
            $scope.ListingImageWidth = imageWidth;
            var listings = [];
            data.Listings.forEach(function (listing) {
                listings.push({
                    Id: listing.Id,
                    Title: listing.Title,
                    ImageUrl: ClassyUtilities.Images.Thumbnail(appSettings, listing.ExternalMedia[0].Key, imageWidth, imageWidth)
                });
            });
            $scope.Listings = listings;

            var comments = [];
            data.Comments.forEach(function (comment) {
                comments.push({
                    Commenter: comment.Profile.UserName,
                    CommenterId: comment.ProfileId,
                    CommenterAvatarUrl: comment.Profile.Avatar.Url,
                    Content: comment.Content
                });
            });
            $scope.Comments = comments;

            ClassyUtilities.OpenGraph.Title($scope.Title);
            if (data.CoverPhotos && data.CoverPhotos.length > 0)
                ClassyUtilities.OpenGraph.Image(ClassyUtilities.Images.Thumbnail(appSettings, data.CoverPhotos[0], 720));
            else if (data.Listings && data.Listings.length > 0 && data.Listings[0].ExternalMedia && data.Listings[0].ExternalMedia.length > 0)
                ClassyUtilities.OpenGraph.Image(ClassyUtilities.Images.Thumbnail(appSettings, data.Listings[0].ExternalMedia[0].Key, 720));
            if (data.Content)
                ClassyUtilities.OpenGraph.Description(data.Content);
            else
                Localizer.Get('Mobile_Collection_ShareDescription', appSettings.Culture).then(function(resource) {
                    ClassyUtilities.OpenGraph.Description(resource.format(getProfileName(data.Profile)));
                });


            // get more projects/collections from this professional/user
            var moreListingsType = data.Profile.IsProfessional ? 'Project' : 'Collection';
            $http.get(appSettings.ApiUrl + '/profile/' + data.Profile.Id + '/collection/list/' + moreListingsType, config).success(function(projects) {
                var p = [];
                projects.forEach(function(project) {
                    if (project.Id == $routeParams.collectionId) return;
                    p.push({
                        Id: project.Id,
                        Title: project.Title,
                        ImageUrl: project.CoverPhotos && project.CoverPhotos.length > 0 ?
                            ClassyUtilities.Images.Thumbnail(appSettings, project.CoverPhotos[0], 160, 160) :
                            'http://www.homelab.com/img/missing-thumb.png'
                    });
                });
                $scope.MoreProjects = p;
            });

            if (data.Profile.IsProfessional)
                Localizer.Get('Mobile_CollectionPage_MoreProjectsFromPro', function (resource) {
                    $scope.Resources.MoreProjectsTitle = resource;
                });
            else 
                Localizer.Get('Mobile_CollectionPage_MoreCollectionsFromUser', function(resource) {
                    $scope.Resources.MoreProjectsTitle = resource;
                });

            Localizer.Get('Mobile_ProfilePage_Views', AppSettings.Culture, function (resource) { $scope.Resources.Views = resource; });
            Localizer.Get('Mobile_CollectionPage_Favorites', AppSettings.Culture, function (resource) { $scope.Resources.Favorites = resource; });
            Localizer.Get('Mobile_CollectionPage_Comments', AppSettings.Culture, function (resource) { $scope.Resources.Comments = resource; });

        }).error(function () {
            // TODO: display some error message
        });

        $scope.openSlideShow = function (collectionId, photoId) {
            $location.url('/Collection/SlideShow/' + collectionId + '/' + photoId);
        };

        $scope.openProfile = function (profileId) {
            $location.url('/Profile/' + profileId);
        };

        // get localized resources
        $scope.Resources = {};
        Localizer.Get('Mobile_CollectionPage_ViewAllComments').then(function (resource) {
            $scope.Resources.ViewAllComments = resource;
        });

        $scope.share = function (network) {
            var url = window.location.protocol + appSettings.Host + '/collection/' + $scope.CollectionId + '/grid/public';
            Classy.Share(network, url);
        };

        

    });
});