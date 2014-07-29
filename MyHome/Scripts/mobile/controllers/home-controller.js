
classy.controller('HomeController', function ($scope, $http, AppSettings, ClassyUtilities, Localizer) {

    ClassyUtilities.Screen.StaticViewport();
    AppSettings.then(function (appSettings) {

        $scope.listingId = '5314ecfca3a75d1aec7b0de4';

    });

});