$(function () {
    $('[authorize]').click(function (e) {
        if (!Classy.IsAuthenticated) {
            $('#login-modal-form').submit(function (e) {

            });
            $('#login-modal').modal('show');
            e.stopImmediatePropagation(); e.preventDefault();
        }
    });

    $('[trigger-listing-action="favorite"]').click(function (e) {
        var listingId = $(this).attr('trigger-action-id');
        var listingType = $(this).attr('trigger-action-type');
        var url = "/" + listingType + "/" + listingId + "/favorite";
        $.post(url, null, function (data) { console.log(data); })
    });

    $('[trigger-profile-action="follow"]').click(function (e) {
        var profileId = $(this).attr('trigger-action-id');
        var url = "/profile/" + profileId + "/follow";
        $.post(url, null, function (data) { console.log(data); })
    });
});