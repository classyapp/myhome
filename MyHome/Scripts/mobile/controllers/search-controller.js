
classy.controller('SearchController', function ($scope, $http, $routeParams, $location, AppSettings, ClassyUtilities, Localizer) {
    ClassyUtilities.Screen.StaticViewport();
    
    AppSettings.then(function (appSettings) {

        var q = $routeParams.q;
        var style = $routeParams.style;
        var room = $routeParams.room;
        var category = $routeParams.category;

        var priceMin = $routeParams.priceMin;
        var priceMax = $routeParams.priceMax;

        var queryString = q ? '?q=' + q : '?';
        queryString += priceMin ? '&priceMin=' + priceMin : '';
        queryString += priceMax ? '&priceMin=' + priceMax : '';

        var postData = {};
        if (style) postData["Metadata"] = { 'Style': style };
        if (room) postData["Metadata"] = { 'Room': room };
        if (category) postData.Categories = [category];

        $scope.filterSearch = function() {
            var minPrice = $('.search-filters-container').find('#min-price').val();
            var maxPrice = $('.search-filters-container').find('#max-price').val();
            if (minPrice.trim() == '') $.query.REMOVE('priceMin'); else $.query.SET('priceMin', minPrice);
            if (maxPrice.trim() == '') $.query.REMOVE('priceMax'); else $.query.SET('priceMax', maxPrice);
            window.location.href = window.location.protocol + '//' + window.location.host + window.location.pathname + '#' + decodeURIComponent($.query.toString().substring(1));
        };

        $http.post(appSettings.ApiUrl + '/listing/search' + queryString, postData, config).success(function(data) {

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
            Localizer.Get('Mobile_SearchPage_MinPrice', appSettings.Culture).then(function (resource) { $scope.Resources.MinPrice = resource; });
            Localizer.Get('Mobile_SearchPage_MaxPrice', appSettings.Culture).then(function (resource) { $scope.Resources.MaxPrice = resource; });
            Localizer.Get('Mobile_SearchPage_Search', appSettings.Culture).then(function (resource) { $scope.Resources.Search = resource; });
            Localizer.Get('Mobile_SearchPage_Filter', appSettings.Culture).then(function (resource) { $scope.Resources.Filter = resource; });

        }).error(function(ex) {
            // TODO: display some error message
        });

    });

    $scope.productPage = function(productId) {
        $location.url('/Product/' + productId);
    };

});