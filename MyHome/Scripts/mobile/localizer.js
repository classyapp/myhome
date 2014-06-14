
var localizerService = angular.module('LocalizerService', []);

localizerService.factory('Localizer', [ '$http', '$q', function($http, $q) {
    
    function get(key, culture) {
        var d = $q.defer();
        $http.get('')
    }

    return {
        Get: get
    };


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