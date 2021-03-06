var Classy = window.Classy || {};
Classy.CurrentAjaxCalls = [];
$.ajax({
    beforeSend: function(xhr, settings) {
        var url = settings.url;
        var request = Classy.CurrentAjaxCalls[url];
        if (!request || request.readyState == 4) {
            Classy.CurrentAjaxCalls[url] = xhr;
            return true;
        }
        return false;
    }
});
$(document).bind('ajaxComplete', function(e, xhr, settings) {
    var url = settings.url;
    if (Classy.CurrentAjaxCalls[url] == xhr)
        delete Classy.CurrentAjaxCalls[url];
});