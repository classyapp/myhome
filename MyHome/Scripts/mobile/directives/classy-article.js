classy.directive('classyArticle', function ($http, $location, AppSettings, ClassyUtilities) {
    return {
        restrict: 'E',
        templateUrl: 'Home/classy-article.html',
        scope: {
            articleId: '@articleId'
        },
        link: function (scope) {
            var w = ClassyUtilities.Screen.GetWidth();
            AppSettings.then(function(appSettings) {
                $http.get(appSettings.ApiUrl + '/collection/' + scope.articleId + '?includeListings=true', config).success(function (data) {

                    scope.Id = data.Id;
                    scope.Title = data.Title;
                    scope.Content = data.Content;
                    scope.CoverPhoto = ClassyUtilities.Images.Thumbnail(appSettings, data.CoverPhotos[0], w, 300);

                    var listings = [];
                    data.IncludedListings.forEach(function(listing) {
                        listings.push({
                            Id: listing.Id,
                            Content: listing.Comments
                        });
                    });
                    scope.Listings = listings;

                    scope.articlePage = function() {
                        $location.url('/Collection/' + data.Id + '/article');
                    };

                });
            });
        }
    };
});