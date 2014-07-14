
classy.controller('ProductController', function ($scope, $http, AppSettings, ClassyUtilities, Localizer, $routeParams) {
    ClassyUtilities.Screen.StaticViewport();

    AppSettings.then(function (appSettings) {

        var utilities = ClassyUtilities;
        var w = utilities.Screen.GetWidth();
        var h = utilities.Screen.GetHeight();

        $http.get(appSettings.ApiUrl + '/listing/' + $routeParams.productId + '?includeComments=true&includeCommenterProfiles=true', config).success(function(data) {

            $scope.Id = data.Id;
            $scope.Title = data.Title;
            $scope.Content = data.Content;
            $scope.Categories = data.Categories;
            $scope.PricingInfo = data.PricingInfo.BaseOption;

            $scope.ImageUrl = utilities.Images.Thumbnail(appSettings, data.ExternalMedia[0].Key, w - 16);

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

        }).error(function () {
            // TODO: display some error message
        });

        // get localized resources
        $scope.Resources = {};
        Localizer.Get('Mobile_ProductPage_PriceLabel', AppSettings.Culture).then(function (resource) { $scope.Resources.PriceLabel = resource; });
        Localizer.Get('Mobile_ProductPage_DescriptionTitle', AppSettings.Culture).then(function (resource) { $scope.Resources.Description = resource; });
        Localizer.Get('Mobile_ProductPage_CommentsTitle', AppSettings.Culture).then(function (resource) { $scope.Resources.Comments = resource; });
        
    });

    $scope.showAllComments = function () {
        $('.product-comments .comment-container').removeClass('hidden');
        $('.product-comments .comment-container:nth-child(2)').removeClass('last');
        $('.product-comments .panel-footer').addClass('hidden');
    };

});
