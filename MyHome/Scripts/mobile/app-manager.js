
var appManagerService = angular.module('AppManagerService', []);

appManagerService.factory('AppSettings', ['$http', '$q', 'CacheProvider', function ($http, $q, CacheProvider) {
    var appSettingsKey = "__AppSettings__";
    var appSettings = CacheProvider.get(appSettingsKey);
    if (!appSettings) {
        var d = $q.defer();
        $http.get('config.js').then(function (response) {

            // add browser culture to the app settings object
            var browserLanguage = window.navigator.language;
            var browserCulture = browserLanguage.split('-')[0];
            response.data.Culture = browserCulture;

            // TODO: I think we should cache this in localStorage
            CacheProvider.put(appSettingsKey, response.data);
            d.resolve(response.data);
        });
        return d.promise;
    } else {
        return $q.defer().resolve(appSettings);
    }
}]);