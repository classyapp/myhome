
classy.factory('AuthProvider', ['CacheProvider', 'AppSettings', '$http', '$q', function(cacheProvider, AppSettings, $http, $q) {

    this._getUser = function () {
        var d = $q.defer();
        if (!cacheProvider.get('__User__')) {
            AppSettings.then(function(appSettings) {
                $http.get(appSettings.Host + '/mobile/authenticate', config).success(function(data) {
                    d.resolve(data);
                }).error(function(ex) {
                    // TODO: now what ?...
                });
            });
            return d.promise;
        } else {
            return d.resolve(cacheProvider.get('__User__'));
        }
    };

    this.getUserInfo = function () {
        var self = this;
        FB.api('/me', function(response) {
            $rootScope.$apply(function() {
                $rootScope.user = self.user = response;
            });
        });
    };

    var _watchAuthenticationStatusChange = function () {
        var self = this;
        FB.Event.subscribe('auth.authResponseChange', function (response) {
            if (response.status === 'connected') {
                if (self && self.getUserInfo)
                    self.getUserInfo();
            }
            else {
                // user is not logged in
            }
        });
    };

    var facebookLogout = function () {
        var self = this;
        FB.logout(function(response) {
            $rootScope.$apply(function() {
                $rootScope.user = self.user = {};
            });
        });
    };

    return {
        watchAuthenticationStatusChange: _watchAuthenticationStatusChange,
        getUser: this._getUser
    };

}]);

classy.directive('authenticatedRoute', ['$rootScope', '$location', function ($root, $location) {
    return {
        link: function () {
            $root.$on('$routeChangeStart', function (event, currRoute, prevRoute) {
                if (prevRoute.authenticate && !authProvider.User.isAuthenticated) {
                    // reload the login route - $location
                }
            });
        }
    };
}]);

classy.run(['$rootScope', '$window', 'AuthProvider', function ($scope, $window, AuthProvider) {

    $window.fbAsyncInit = function () {
        FB.init({
            appId: '281478942020037',
            channelUrl: '/channel.html',
            status: true,
            cookie: true,
            xfbml: true
        });

        AuthProvider.watchAuthenticationStatusChange();
    };

    (function (d) {
        var js, id = 'facebook-jssdk', ref = d.getElementsByTagName('script')[0];

        if (d.getElementById(id)) return;

        js = d.createElement('script');
        js.id = id;
        js.async = true;
        js.src = "//connect.facebook.net/en_US/all.js";

        ref.parentNode.insertBefore(js, ref);
    }(document));

}]);