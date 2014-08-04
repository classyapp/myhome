classy.directive('classyListing', function ($http, $q, AppSettings, ClassyUtilities) {
    return {
        restrict: 'E',
        templateUrl: 'Home/classy-listing.html',
        scope: {
            listingIds: '@listingIds'
        },
        link: function (scope) {
            var w = ClassyUtilities.Screen.GetWidth();
            AppSettings.then(function (appSettings) {

                var listings = scope.listingIds.split(',');
                var featuredListings = [];

                var promises = [];

                listings.forEach(function(listingId) {
                    var q = $http.get(appSettings.ApiUrl + '/listing/' + listingId + '?includeProfile=true', config).success(function (data) {
                        featuredListings.push({
                            ImageUrl: ClassyUtilities.Images.Thumbnail(appSettings, data.ExternalMedia[0].Key, w, 300),
                            CopyrightMessage: ClassyUtilities.Listing.GetCopyrightMessage(data)
                        });
                    });
                    promises.push(q);
                });
                scope.FeaturedListings = featuredListings;

                $q.all(promises).then(function() {
                    $('.swiper-container').swiper({
                        mode: 'horizontal',
                        loop: true
                    });
                });

            });
        }
    };
});