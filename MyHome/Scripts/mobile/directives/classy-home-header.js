classy.directive('classyHomeHeader', function ($location, AuthProvider) {
    return {
        restrict: 'E',
        templateUrl: 'Home/classy-header.html',
        transclude: true,
        link: function (scope) {
            AuthProvider.getUser().then(function (data) {
                scope.IsAuthenticated = data.IsAuthenticated;
                scope.User = data.Profile;
            });

            scope.loginPage = function() {
                $location.url('/Login');
            };
        }
    };
});