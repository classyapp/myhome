classy.directive('classyCollection', function ($http, AppSettings, ClassyUtilities) {

    return {
        restrict: 'E',
        templateUrl: 'Home/classy-collection.html',
        link: function (scope) {
            var w = ClassyUtilities.Screen.GetWidth();
            AppSettings.then(function(appSettings) {
                $http.get(appSettings.ApiUrl + '/collection/' + scope.collectionId + '?includeListings=true', config).success(function (data) {

                    var imageWidth = Math.floor((w - 20) / 3);
                    
                    var listings = [];
                    data.Listings.forEach(function (listing) {
                        var l = {
                            Id: listing.Id,
                            Title: listing.Title,
                            ImageUrl: ClassyUtilities.Images.Thumbnail(appSettings, listing.ExternalMedia[0].Key, imageWidth, imageWidth),
                            ArticleImageUrl: ClassyUtilities.Images.Thumbnail(appSettings, listing.ExternalMedia[0].Key, w),
                            CopyrightMessage: ClassyUtilities.GetCopyrightMessage(listing)
                        };
                        data.IncludedListings.forEach(function (includedListing) {
                            if (includedListing.Id != listing.Id) return;
                            l.Comments = includedListing.Comments;
                        });
                        listings.push(l);
                    });
                    scope.Listings = listings;

                });
            });
        }
    };
});