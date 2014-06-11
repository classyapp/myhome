
var profileApp = angular.module('profileApp', []);

profileApp.value('apiUrl', 'http://www.thisisclassy.com:8008'); // way to inject objects into module controllers

var config = {
    headers: {
        'X-Classy-Env': '{ "AppId": "v1.0" }',
        'Content-Type': 'application/json',
        'Accept': 'application/json'
    }
};

profileApp.controller('ProfileController', function ($scope, $http, apiUrl) {
    $scope.profileDetails = { 'UserName': 'Gilly' };
    $http.get(apiUrl + '/profile/246', config).success(function(data) {
        $scope.profileDetails = data;
    }).error(function() {
        // TODO: display some error message
    });
});