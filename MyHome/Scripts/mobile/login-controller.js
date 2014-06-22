
classy.controller('LoginController', function ($scope, $http, AppSettings, ClassyUtilities, Localizer, $routeParams, $location) {
    ClassyUtilities.Screen.StaticViewport();
    AppSettings.then(function (appSettings) {

        $scope.submitForm = function () {
            var data = {
                Email: $('#Email').val(),
                Password: $('#Password').val()
            };
            $http.post('/mobile-login', JSON.stringify(data))
                .success(function(data) {
                    // TODO: redirect user to next url
                }).error(function(ex) {
                    alert(ex);
                });
        };

        $scope.Resources = {};
        Localizer.Get('Login_Facebook', AppSettings.Culture).then(function (resource) {
            $scope.Resources.LoginFacebook = resource;
        });
        Localizer.Get('Login', AppSettings.Culture).then(function (resource) {
            $scope.Resources.Login = resource;
        });
        Localizer.Get('Login_Email', AppSettings.Culture).then(function (resource) {
            $scope.Resources.LoginEmail = resource;
        });
        Localizer.Get('Login_Password', AppSettings.Culture).then(function (resource) {
            $scope.Resources.LoginPassword = resource;
        });
        Localizer.Get('Login_RememberMe', AppSettings.Culture).then(function (resource) {
            $scope.Resources.LoginRememberMe = resource;
        });
        Localizer.Get('Login_Submit', AppSettings.Culture).then(function (resource) {
            $scope.Resources.LoginSubmit = resource;
        });
        Localizer.Get('Login_Register', AppSettings.Culture).then(function (resource) {
            $scope.Resources.LoginRegister = resource;
        });
        Localizer.Get('Login_ForgotPassword', AppSettings.Culture).then(function (resource) {
            $scope.Resources.LoginForgotPassword = resource;
        });

    });
});