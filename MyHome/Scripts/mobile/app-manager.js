
var appManagerService = angular.module('AppManagerService', []);

appManagerService.factory('AppSettings', [ '$http', '$q', function($http, $q) {
    var appSettingsKey = "__AppSettings__";
    // TODO: see if i can use angularjs built in cache provider
    var appSettings = Classy.CacheProvider.Get(appSettingsKey);
    if (!appSettings) {
        var d = $q.defer();
        $http.get('config.js').then(function (response) {
            Classy.CacheProvider.Add(appSettingsKey, response.data);
            d.resolve(response.data);
        });
        return d.promise;
    } else {
        return $q.defer().resolve(appSettings);
    }
}]);