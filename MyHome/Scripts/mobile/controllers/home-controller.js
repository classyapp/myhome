
classy.controller('HomeController', function ($scope, $http, AppSettings, ClassyUtilities, Localizer) {

    ClassyUtilities.Screen.StaticViewport();
    AppSettings.then(function (appSettings) {

        $scope.listingId = '5314ecfca3a75d1aec7b0de4';
        $scope.collectionId = '5314ecfca3a75d1aec7b0de3';
        $scope.articleId = '5352d6dbaa05a623b456d2b8';

    });

});