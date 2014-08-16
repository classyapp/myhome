classy.directive('classyArticle', function ($http, $location, $timeout, AppSettings, ClassyUtilities, Localizer) {
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

                    var covers = [];
                    data.CoverPhotos.forEach(function(cover) {
                        covers.push(ClassyUtilities.Images.Thumbnail(appSettings, cover, w, 300));
                    });
                    scope.CoverPhotos = covers;

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

                    scope.Resources = {};
                    Localizer.Get('Mobile_Home_ArticleMorePhotos', appSettings.Culture).then(function (resource) { scope.Resources.MorePhotosLink = resource; });

                    $timeout(function() {
                        $('.swiper-container.article-swiper').swiper({
                            mode: 'horizontal',
                            loop: true,
                            preventLinks: true,
                            preventLinksPropagation: false,
                            shortSwipes: false
                        });
                    });

                });
            });
        }
    };
});
