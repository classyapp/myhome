classy.directive('classyHomeHeader', function($location, AppSettings, AuthProvider, ClassyUtilities) {
    return {
        restrict: 'E',
        templateUrl: 'Home/classy-header.html',
        transclude: true,
        link: function(scope) {
            AppSettings.then(function(appSettings) {
                AuthProvider.getUser().then(function(data) {
                    scope.IsAuthenticated = data.IsAuthenticated;
                    scope.User = data.Profile;
                    scope.User.Avatar.ImageUrl = ClassyUtilities.Images.Thumbnail(appSettings, data.Profile.Avatar.Key, 45, 45);
                });

                scope.homePage = function() {
                    $location.url('/');
                };
                scope.loginPage = function() {
                    $location.url('/Login');
                };
                scope.profilePage = function(userId) {
                    $location.url('/Profile/' + userId);
                };
            });
        }
    };
});