var Classy = {};

Classy.AjaxReconnect = function () {
    $(document).trigger("classy.ajax.reconnect");
}

Classy.AcquireGPSCoordinates = function () {
    if (Classy.GetCookie(Classy.Env.GPSCookieName) == null || Classy.GetCookie(Classy.Env.GPSOriginCookieName) == "auto") {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(function successFunction(position) {
                var lat = position.coords.latitude;
                var long = position.coords.longitude;
                Classy.SetCookie(Classy.Env.GPSCookieName, JSON.stringify({ latitude: lat, longitude: long }), 365);
                $(document).trigger("classy.gps.available", { Available: true, Latitude: lat, Longitude: long });
                Classy.Env.GPSEnabled = true;
            }, function () {
                Classy.SetCookie(Classy.Env.GPSCookieName, "", 7);
                Classy.SetCookie(Classy.Env.GPSOriginCookieName, "browser", 7);
                $(document).trigger("classy.gps.available", { Available: false });
                Classy.Env.GPSEnabled = false;
            });
        } else {
            Classy.SetCookie(Classy.Env.GPSCookieName, "", 7);
            $(document).trigger("classy.gps.available", { Available: false });
            Classy.Env.GPSEnabled = false;
        }
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

    return null;
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

Classy.SendEmail = function (subject, body, title) {
    $("#send-email-modal")
        .on("loaded.bs.modal", function () {
            $(this).find("#Subject").val(subject.decodeHTML());
            $(this).find("#Body").val(body.decodeHTML());
            if (typeof title !== 'undefined') { $(this).find(".modal-title").html(title); }
            jQuery.validator.unobtrusive.parse($("#frmSendEmail"));
        })
        .on("hidden.bs.modal", function () {
            $(this).find("input[type=text], textarea").val("");
        })
        .modal("show");
}

Classy.ShareUI = function (socialUrl, url, winWidth, winHeight) {
    var winTop = (screen.height / 2) - (winHeight / 2);
    var winLeft = (screen.width / 2) - (winWidth / 2);
    window.open(socialUrl + url, 'sharer', 'top=' + winTop + ',left=' + winLeft + ',toolbar=0,status=0,width=' + winWidth + ',height=' + winHeight);
}

var font = 'font-family: "Helvetica Neue", Helvetica, Arial, sans-serif;font-size: 15px;font-weight: bold;';
if (console) console.log("%cjoinHomelab()", "color: #2b2;" + font)
function joinHomelab() { window.location.href = "/careers"; }

String.prototype.decodeHTML = function () {
    var map = { "gt": ">" /* , … */ };
    return this.replace(/&(#(?:x[0-9a-f]+|\d+)|[a-z]+);?/gi, function ($0, $1) {
        if ($1[0] === "#") {
            return String.fromCharCode($1[1].toLowerCase() === "x" ? parseInt($1.substr(2), 16) : parseInt($1.substr(1), 10));
        } else {
            return map.hasOwnProperty($1) ? map[$1] : $0;
        }
    });
};

$(function () {
    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry/i.test(navigator.userAgent)) {
        var ww = ($(window).width() < window.screen.width) ? $(window).width() : window.screen.width; //get proper width
        var mw = 1220; // min width of site
        var ratio = ww / mw; //calculate ratio
        alert(ww + " " + mw + " " + ratio);
        if (ww < mw) { //smaller than minimum size
            $('#viewport').attr('content', 'initial-scale=' + ratio + ', maximum-scale=' + ratio + ', minimum-scale=' + ratio + ', user-scalable=yes, width=' + ww);
        } else { //regular size
            $('#viewport').attr('content', 'initial-scale=1.0, maximum-scale=2, minimum-scale=1.0, user-scalable=yes, width=' + ww);
        }
        $('#viewport').attr('content');
    }
});