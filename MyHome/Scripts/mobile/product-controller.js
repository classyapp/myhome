
classy.controller('ProductController', function ($scope, $http, AppSettings, ClassyUtilities, Localizer, $routeParams) {
    ClassyUtilities.Screen.StaticViewport();

    AppSettings.then(function (appSettings) {

        var utilities = ClassyUtilities;
        var w = utilities.Screen.GetWidth();
        var h = utilities.Screen.GetHeight();

        $http.get(appSettings.ApiUrl + '/listing/' + $routeParams.productId, config).success(function(data) {

            $scope.Id = data.Id;
            $scope.Title = data.Title;
            $scope.Content = data.Content;
            $scope.Categories = data.Categories;
            $scope.PricingInfo = data.PricingInfo.BaseOption;

            $scope.ImageUrl = utilities.Images.Thumbnail(appSettings, data.ExternalMedia[0].Key, w - 16);

        }).error(function () {
            // TODO: display some error message
        });

        // get localized resources
        $scope.Resources = {};
        Localizer.Get('Mobile_ProductPage_PriceLabel', AppSettings.Culture).then(function (resource) { $scope.Resources.PriceLabel = resource; });
        
    });
});
