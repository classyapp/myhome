
classy.controller('HomeController', function ($scope, $http, $compile, AppSettings, ClassyUtilities, Localizer) {

    ClassyUtilities.Screen.StaticViewport();
    AppSettings.then(function (appSettings) {

        var mainContainer = $('.main-container');
        var homePageSettings = appSettings.HomePage;
        for (var elem in homePageSettings) {
            if (homePageSettings.hasOwnProperty(elem)) {
                var newElement;
                    if (elem.indexOf('HomeHeader') == 0)
                        newElement = $compile("<classy-home-header></classy-home-header>")($scope);
                    if (elem.indexOf('Listing') == 0)
                        newElement = $compile("<classy-listing listing-ids=\"" + homePageSettings[elem] + "\"></classy-listing>")($scope);
                    if (elem.indexOf('Collection') == 0)
                        newElement = $compile("<classy-collection collection-id=\"" + homePageSettings[elem] + "\"></classy-collection>")($scope);
                    if (elem.indexOf('Article') == 0)
                        newElement = $compile("<classy-article article-id=\"" + homePageSettings[elem] + "\"></classy-article>")($scope);
                    if (elem.indexOf('Html') == 0)
                        newElement = $compile("<classy-html html-content=\"" + homePageSettings[elem] + "\"></classy-html>")($scope);
                if (newElement)
                    mainContainer.append(newElement);
            }
        }
        
    });

});