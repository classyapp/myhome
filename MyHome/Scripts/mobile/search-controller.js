
classy.controller('SearchController', function ($scope, $http, AppSettings, ClassyUtilities, Localizer, $routeParams) {
    ClassyUtilities.Screen.StaticViewport();
    
    AppSettings.then(function (appSettings) {

        var q = $routeParams.q;
        var category = $routeParams.category;
        var room = $routeParams.room;

        var queryString = q ? 'q=' + q : category ? 'category=' + category : room ? 'room=' + room : '';

        $http.get(appSettings.ApiUrl + '/listing/search?' + queryString, config).success(function(data) {

            var imageWidth = parseInt((ClassyUtilities.Screen.GetWidth() - (16 * 4)) / 3);
            $scope.ListingImageWidth = imageWidth;

            var listings = [];
            data.Results.forEach(function (listing) {
                if (!listing.ExternalMedia || !listing.ExternalMedia.length)
                    return;

                var l = {
                    Id: listing.Id,
                    Title: listing.Title,
                    Content: listing.Content,
                    ImageUrl: ClassyUtilities.Images.Thumbnail(appSettings, listing.ExternalMedia[0].Key, imageWidth, imageWidth)
                };
                listings.push(l);
            });
            $scope.Results = listings;
            $scope.Total = data.Count;

            $scope.Resources = [];
            Localizer.Get('Mobile_SearchPage_SearchResultsTitle', appSettings.Culture).then(function (resource) { $scope.Resources.SearchResults = resource; });
            Localizer.Get('Mobile_SearchPage_EmptyResultsMessage', appSettings.Culture).then(function (resource) { $scope.Resources.EmptyResults = resource; });

        }).error(function(ex) {
            // TODO: display some error message
        });

    });
});