
var localizerService = angular.module('LocalizerService', []);

localizerService.factory('Localizer', [ '$http', '$q', 'AppSettings', 'CacheProvider', function($http, $q, AppSettings, CacheProvider) {
    
    function get(key, culture) {

        // TODO: get the culture from somewhere!
        if (!culture) culture = "en";

        var d = $q.defer();
        AppSettings.then(function(appSettings) {
            var resource = CacheProvider.get(key);
            if (resource) {
                d.resolve(resource[culture.toLowerCase()]);
            }

            $http.get(appSettings.ApiUrl + '/resource/' + key, config).then(function (response) {
                if (response.data == '') {
                    d.resolve(key);
                    return;
                }
                CacheProvider.put(key, response.data.Values);
                d.resolve(response.data.Values[culture.toLowerCase()]);
            });
        });
        return d.promise;
    }

    return {
        Get: get
    };
}]);