var Classy = {};

Classy.AjaxReconnect = function () {
    $(document).trigger("classy.ajax.reconnect");
}

Classy.AcquireGPSCoordinates = function () {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function successFunction(position) {
            var lat = position.coords.latitude;
            var long = position.coords.longitude;
            Classy.SetCookie(Classy.Env.GPSCookieName, JSON.stringify({ latitude: lat, longitude: long }), 365);
            $(document).trigger("classy.gps.available", { Available: true, Latitude: lat, Longitude: long });
            Classy.Env.GPSEnabled = true;
        }, function () {
            Classy.SetCookie(Classy.Env.GPSCookieName, "", -365);
            $(document).trigger("classy.gps.available", { Available: false });
            Classy.Env.GPSEnabled = false;
        });
    } else {
        Classy.SetCookie(Classy.Env.GPSCookieName, "", -365);
        $(document).trigger("classy.gps.available", { Available: false });
        Classy.Env.GPSEnabled = false;
    }
};

Classy.SetCookie = function (cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toGMTString();
    document.cookie = cname + "=" + cvalue + "; path=/; " + expires;
}

Classy.GetCookie = function (cname)
{
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for(var i=0; i<ca.length; i++) 
    {
        var c = ca[i].trim();
        if (c.indexOf(name)==0) return c.substring(name.length,c.length);
    }
}

Classy.ParseQueryString = function () {
    var data = {};
    var query = window.location.search.substring(1);
    if (query != "") {
        var vars = query.split('&');
        for (var i = 0; i < vars.length; i++) {
            var pair = vars[i].split('=');
            data[decodeURIComponent(pair[0])] = decodeURIComponent(pair[1]);
        }
    }
    return data;
}