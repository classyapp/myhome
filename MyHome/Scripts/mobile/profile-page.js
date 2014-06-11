
var profilePage = angular.module('profilePage', []);

var appSettings;
profilePage.run(function ($q, $http) {
    var appSettingsPromise = Classy.AppManager.GetAppSettings($q, $http);
    appSettings = appSettingsPromise;
});

//profilePage.value('appSettingsPromise', 'http://www.thisisclassy.com:8008'); // way to inject objects into module controllers

var config = {
    headers: {
        'X-Classy-Env': '{ "AppId": "v1.0" }',
        'Content-Type': 'application/json',
        'Accept': 'application/json'
    }
};

profilePage.controller('ProfileController', function ($scope, $http) {
    debugger;
    $http.get(apiUrl + '/profile/246?includeCollections=true', config).success(function(data) {
        $scope.profileDetails = data;
    }).error(function() {
        // TODO: display some error message
    });
});