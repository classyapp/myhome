classy.directive('classyListing', function ($http, AppSettings, ClassyUtilities) {
    return {
        restrict: 'E',
        templateUrl: 'Home/classy-listing.html',
        scope: {
            listingId: '@listingId'
        },
        link: function (scope) {
            var w = ClassyUtilities.Screen.GetWidth();
            AppSettings.then(function(appSettings) {
                $http.get(appSettings.ApiUrl + '/listing/' + scope.listingId + '?includeProfile=true', config).success(function (data) {
                    scope.ImageUrl = ClassyUtilities.Images.Thumbnail(appSettings, data.ExternalMedia[0].Key, w, 300);
                    scope.CopyrightMessage = ClassyUtilities.Listing.GetCopyrightMessage(data);
                });
            });
        }
    };
});