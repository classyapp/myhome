
var Classy = Classy || {};

Classy.Cache = {};
Classy.CacheProvider = {
    Add: function(key, value) {
        Classy.Cache[key] = value;
    },
    Get: function(key) {
        return Classy[key];
    },
    Remove: function(key) {
        delete Classy.Cache[key];
    }
};