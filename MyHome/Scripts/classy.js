var Classy = {};

Classy.AjaxReconnect = function () {
    $(document).trigger("classy.ajax.reconnect");
}

Classy.AcquireGPSCoordinates = function () {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function successFunction(position) {
            var lat = position.coords.latitude;
            var long = position.coords.longitude;
            Classy.SetCookie("classy.env.gps_location", JSON.stringify({ latitude: lat, longitude: long }), 365);
        }, function () {
            Classy.SetCookie("classy.env.gps_location", "", -365);
        });
    } 
};

Classy.SetCookie = function (cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toGMTString();
    document.cookie = cname + "=" + cvalue + "; " + expires;
}
