
var Classy = Classy || {};

Classy.OpenListing = function (elem) {
    var collectionId = $(elem).data('collection-id');
    var listingId = $(elem).data('listing-id');
    window.location.href = '//' + window.location.host + window.location.pathname + '#/Collection/SlideShow/' + collectionId + '/' + listingId;
};

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

String.prototype.format = function() {
    var s = this;
    for (var i = 0; i < arguments.length; i++) {
        var reg = new RegExp("\\{" + i + "\\}", "gm");
        s = s.replace(reg, arguments[i]);
    }
    return s;
};

String.prototype.extractHost = function(url) {
    var a = document.createElement('a');
    a.href = url;
    return a.hostname;
};

$.fn.css3 = function(name, value) {
    $(this).css(name, value);
    $(this).css('-webkit-' + name, value);
    $(this).css('-moz-' + name, value);
    $(this).css('-ms-' + name, value);
    $(this).css('-o-' + name, value);
};