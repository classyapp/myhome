
classy.controller('LoginController', function ($scope, $http, $location, AppSettings, ClassyUtilities, Localizer) {

    ClassyUtilities.PageLoader.Show();

    var promises = [];

    // enforce ssl
    if (window.location.protocol == 'http:') {
        window.location.href = window.location.href.replace('http://', 'https://');
    }

    ClassyUtilities.Screen.StaticViewport();
    AppSettings.then(function (appSettings) {

        $scope.submitForm = function () {

            var btn = $('#login-submit');
            var spinner = $('<span/>').addClass('fa fa-spinner fa-spin');
            btn.prepend(spinner);
            btn.attr('disabled', 'disabled');

            var data = {
                Email: $('#Email').val(),
                Password: $('#Password').val()
            };
            $http.post('/mobile-login', JSON.stringify(data))
                .success(function(profileId) {
                    window.location.href = window.location.protocol + '//' + window.location.host + '/Mobile/App.html#/Profile/' + profileId;
                }).error(function() {
                    OnLoginError();
                });
        };

        $scope.Resources = {};
        promises.push(Localizer.Get('Login_Facebook', appSettings.Culture).then(function (resource) { $scope.Resources.LoginFacebook = resource; }));
        promises.push(Localizer.Get('Login', appSettings.Culture).then(function (resource) { $scope.Resources.Login = resource; }));
        promises.push(Localizer.Get('Login_Email', appSettings.Culture).then(function (resource) { $scope.Resources.LoginEmail = resource; }));
        promises.push(Localizer.Get('Login_Password', appSettings.Culture).then(function (resource) { $scope.Resources.LoginPassword = resource; }));
        promises.push(Localizer.Get('Login_RememberMe', appSettings.Culture).then(function (resource) { $scope.Resources.LoginRememberMe = resource; }));
        promises.push(Localizer.Get('Login_Submit', appSettings.Culture).then(function (resource) { $scope.Resources.LoginSubmit = resource; }));
        promises.push(Localizer.Get('Login_Register', appSettings.Culture).then(function (resource) { $scope.Resources.LoginRegister = resource; }));
        promises.push(Localizer.Get('Login_ForgotPassword', appSettings.Culture).then(function (resource) { $scope.Resources.LoginForgotPassword = resource; }));
        promises.push(Localizer.Get('Mobile_Login_LoginErrorMessage', appSettings.Culture).then(function(resource) { $scope.Resources.LoginErrorMessage = resource; }));

        $q.all(promises).then(ClassyUtilities.PageLoader.Hide);

    });

    $scope.facebookLogin = function() {
        FB.login(function (response) {
            if (response.authResponse) {
                var access_token = FB.getAuthResponse()['accessToken'];
                $.ajax('/login/fb', {
                    type: 'POST',
                    dataType: 'json',
                    contentType: 'application/json',
                    data: JSON.stringify({ 'token': access_token }),
                    success: function (data) {
                        if (data.IsValid) {
                            OnFacebookLogin(data.Profile);
                        } else {
                            OnLoginError();
                        }
                    }
                });
            } else {
                console.log('User cancelled login or did not fully authorize.');
            }
        }, { scope: 'basic_info,email,user_friends,user_photos,user_website,publish_actions' });
    };

    var OnFacebookLogin = function () {
        $location.url('/');
    };

    var OnLoginError = function() {
        $('.alert-danger').removeClass('hidden');
    };

});