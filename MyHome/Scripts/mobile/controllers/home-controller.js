
classy.controller('HomeController', function ($scope, $http, $compile, AppSettings, ClassyUtilities, Localizer) {

    ClassyUtilities.Screen.StaticViewport();
    AppSettings.then(function (appSettings) {

        var mainContainer = $('.main-container');
        var homePageSettings = appSettings.HomePage;
        for (var elem in homePageSettings) {
            if (homePageSettings.hasOwnProperty(elem)) {
                var newElement;
                switch (elem) {
                    case "HomeHeader":
                        newElement = $compile("<classy-home-header></classy-home-header>")($scope);
                        break;
                    case "Listing":
                        newElement = $compile("<classy-listing listing-id=\"" + homePageSettings[elem] + "\"></classy-listing>")($scope);
                        break;
                    case "Collection":
                        newElement = $compile("<classy-collection collection-id=\"" + homePageSettings[elem] + "\"></classy-collection>")($scope);
                        break;
                    case "Article":
                        newElement = $compile("<classy-article article-id=\"" + homePageSettings[elem] + "\"></classy-article>")($scope);
                        break;
                    case "Html":
                        newElement = $compile("<classy-html html-content=\"" + $('<div/>').text(homePageSettings[elem]).html() + "\"></classy-html>")($scope);
                        break;
                }
                if (newElement)
                    mainContainer.append(newElement);
            }
        }

        //$scope.listingId = '5314ecfca3a75d1aec7b0de4';
        //$scope.collectionId = '5314ecfca3a75d1aec7b0de3';
        //$scope.articleId = '5352d6dbaa05a623b456d2b8';

        //var newElement = $compile("<classy-listing listing-id=\"listingId\"></classy-listing>")($scope);
        //$('.main-container').append(newElement);

    });

});