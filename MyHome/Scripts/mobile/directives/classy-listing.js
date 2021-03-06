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

                var listingObjects = scope.listingIds.split(',');
                var featuredListings = [];

                var promises = [];

                listingObjects.forEach(function (listingObject) {
                    var collectionId = listingObject.split(':')[0];
                    var listingId = listingObject.split(':')[1];

                    var q = $http.get(appSettings.ApiUrl + '/listing/' + listingId + '?includeProfile=true', config).success(function (data) {
                        featuredListings.push({
                            Id: listingId,
                            CollectionId: collectionId,
                            ImageUrl: ClassyUtilities.Images.Thumbnail(appSettings, data.ExternalMedia[0].Key, w, 300),
                            CopyrightMessage: ClassyUtilities.Listing.GetCopyrightMessage(data),
                            ProfileImage: ClassyUtilities.Images.Thumbnail(appSettings, data.Profile.Avatar.Key, 35, 35),
                            ProfileName: data.Profile.UserName
                        });
                    });
                    promises.push(q);
                });
                scope.FeaturedListings = featuredListings;

                $q.all(promises).then(function() {
                    $('.swiper-container.listing-swiper').swiper({
                        mode: 'horizontal',
                        loop: true,
                        preventLinks: true,
                        preventLinksPropagation: false,
                        shortSwipes: false
                    });
                });

                scope.openListing = function(collectionId, listingId) {
                    $location.url('/Collection/SlideShow/' + collectionId + '/' + listingId);
                };

            });
        }
    };
});
