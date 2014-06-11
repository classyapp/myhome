
var Classy = Classy || {};

Classy.AppManager = {
    AppSettingsKey: "__AppSettings__",
    GetAppSettings: function($q, $http) {
        var appSettings = Classy.CacheProvider.Get(Classy.AppManager.AppSettingsKey);
        if (!appSettings) {
            var d = $q.defer();
            $http.get('config.js').then(function(data) {
                Classy.CacheProvider.Add(Classy.AppManager.AppSettingsKey, data);
                d.resolve(data);
            });
            return d.promise;
        } else {
            return $q.defer().resolve(appSettings);
        }
    }
};