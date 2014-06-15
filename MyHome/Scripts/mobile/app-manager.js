
var appManagerService = angular.module('AppManagerService', []);

appManagerService.factory('AppSettings', ['$http', '$q', 'CacheProvider', function ($http, $q, CacheProvider) {
    var appSettingsKey = "__AppSettings__";
    var appSettings = CacheProvider.get(appSettingsKey);
    if (!appSettings) {
        var d = $q.defer();
        $http.get('config.js').then(function (response) {
            CacheProvider.put(appSettingsKey, response.data);
            d.resolve(response.data);
        });
        return d.promise;
    } else {
        return $q.defer().resolve(appSettings);
    }
}]);