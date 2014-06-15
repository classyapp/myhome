
var localizerService = angular.module('LocalizerService', []);

localizerService.factory('Localizer', [ '$http', '$q', 'CacheProvider', function($http, $q, CacheProvider) {
    
    function get(key, culture) {

        var d = $q.defer();
        var resource = CacheProvider.get(key);
        if (resource) {
            return d.resolve(resource[culture.toLowerCase()]);
        }

        $http.get('/resource/' + key).then(function(response) {
            CacheProvider.put(key, response.Values);
            d.resolve(response.values[culture.toLowerCase()]);
        });

        return d.promise();
    }

    return {
        Get: get
    };
}]);