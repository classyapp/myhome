
angular.module('profile-module', ['ngRoute'])

//.value('varName', { someVar: 'someValue' }) // way to inject objects into module controllers

.config(function($routeProvider) {

})
.controller('ProfileController', function ($scope, $http) {
    debugger;
    $http.get('someUrl/profile/data').success(function(data) {
        $scope.profileDetails = data;
    }).error(function() {
        // TODO: display some error message
    });
});