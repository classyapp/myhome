
classy.controller('CollectionController', function ($scope, $http, AppSettings, ClassyUtilities, Localizer, $routeParams, $location, AuthProvider, $timeout, $route) {
    ClassyUtilities.Screen.StaticViewport();
    
    $scope.showAllComments = function () {
        $('.profile-comments .comment-container').removeClass('hidden');
        $('.profile-comments .comment-container:nth-child(2)').removeClass('last');
        $('.profile-comments .panel-footer').addClass('hidden');
    };

    AppSettings.then(function (appSettings) {

        $scope.CollectionId = $routeParams.collectionId;
        $http.get(appSettings.ApiUrl + '/collection/' + $routeParams.collectionId + '?includeComments=true&includeCommenterProfiles=true&includeListings=true&increaseViewCounter=true&includeProfile=true', config).success(function (data) {

            AuthProvider.getUser().then(function(data) {
                $scope.IsAuthenticated = data.IsAuthenticated;
                $scope.User = data.Profile;
            });

            $scope.Content = data.Content;
            $scope.Avatar = data.Profile.Avatar.Url;
            $scope.CollectionName = data.Title;
            $scope.ProfileName = data.Profile.UserName;
            $scope.ProfileId = data.Profile.Id;

            $scope.ViewCount = data.ViewCount;
            $scope.FavoriteCount = data.FavoriteCount;
            $scope.CommentsCount = data.CommentCount;

            var w = ClassyUtilities.Screen.GetWidth();
            var h = ClassyUtilities.Screen.GetHeight();
            $scope.ScreenWidth = w;
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
                var l = {
                    Id: listing.Id,
                    Title: listing.Title,
                    ImageUrl: ClassyUtilities.Images.Thumbnail(appSettings, listing.ExternalMedia[0].Key, imageWidth, imageWidth),
                    ArticleImageUrl: ClassyUtilities.Images.Thumbnail(appSettings, listing.ExternalMedia[0].Key, w),
                    CopyrightMessage: ClassyUtilities.Listing.GetCopyrightMessage(listing)
                };
                data.IncludedListings.forEach(function(includedListing) {
                    if (includedListing.Id != listing.Id) return;
                    l.Comments = includedListing.Comments;
                });
                listings.push(l);
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
                Localizer.Get('Mobile_CollectionPage_MoreProjectsFromPro', appSettings.Culture).then(function (resource) { $scope.Resources.MoreProjectsTitle = resource; });
            else 
                Localizer.Get('Mobile_CollectionPage_MoreCollectionsFromUser', appSettings.Culture).then(function(resource) { $scope.Resources.MoreProjectsTitle = resource; });

            Localizer.Get('Mobile_ProfilePage_Views', appSettings.Culture).then(function (resource) { $scope.Resources.Views = resource; });
            Localizer.Get('Mobile_CollectionPage_Favorites', appSettings.Culture).then(function (resource) { $scope.Resources.Favorites = resource; });
            Localizer.Get('Mobile_CollectionPage_Comments', appSettings.Culture).then(function (resource) { $scope.Resources.Comments = resource; });
            Localizer.Get('Mobile_CollectionPage_CommentsSectionTitle', appSettings.Culture).then(function (resource) { $scope.Resources.CommentsSectionTitle = resource; });
            Localizer.Get('Mobile_CollectionPage_CommentsFirstTitle', appSettings.Culture).then(function (resource) { $scope.Resources.CommentsFirstTitle = resource; });
            Localizer.Get('Mobile_CollectionPage_NewCommentPlaceholder', appSettings.Culture).then(function (resource) { $scope.Resources.NewCommentPlaceholder = resource; });

            $scope.submitComment = function () {
                if (!$scope.IsAuthenticated) {
                    $location.url('/Login');
                    return;
                }

                var comment = $('#new-comment').val();
                var data = {
                    CollectionId: $routeParams.collectionId,
                    Content: comment
                };
                $http.post(appSettings.Host + '/collection/' + $routeParams.collectionId + '/comments/new', data, config).success(function () {
                    $route.reload();
                });
            };

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
        Localizer.Get('Mobile_CollectionPage_ViewAllComments', appSettings.Culture).then(function (resource) {
            $scope.Resources.ViewAllComments = resource;
        });

        $scope.share = function (network) {
            var url = window.location.protocol + appSettings.Host + '/collection/' + $scope.CollectionId + '/grid/public';
            Classy.Share(network, url);
        };

        $timeout(function () {
            $('#new-comment').keydown(function () {
                var newValue = $(this).val();
                if (newValue.trim().length >= 2)
                    $('.btn.post-comment').removeAttr('disabled');
                else
                    $('.btn.post-comment').attr('disabled', 'disabled');
            });
        });
    });
});