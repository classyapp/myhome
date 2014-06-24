
var Classy = Classy || {};

Classy.Utilities = {
    GetUrlParam: function(paramName) {
        paramName = paramName.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + paramName + "=([^&#]*)"),
            results = regex.exec(location.search);
        return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    }
};

Classy.Share = function (network, url) {
    if (!url) url = window.location.href;
    var protocol = window.location.protocol;
    switch (network) {
        case 'facebook':
            window.location.href = protocol + '//www.facebook.com/sharer/sharer.php?u=' + encodeURIComponent(url) + '&utm_source=share_image&utm_medium=facebook';
            break;
        case 'twitter':
            window.location.href = protocol + '//twitter.com/home?status=' + encodeURIComponent(url) + '&utm_source=share_image&utm_medium=twitter';
            break;
        case 'google':
            window.location.href = protocol + '//plus.google.com/share?url=' + encodeURIComponent(url) + '?utm_source=share_image&utm_medium=gplus';
            break;
    }
};

String.prototype.format = function () {
    var s = this;
    for (var i = 0; i < arguments.length; i++) {
        var reg = new RegExp("\\{" + i + "\\}", "gm");
        s = s.replace(reg, arguments[i]);
    }
    return s;
}