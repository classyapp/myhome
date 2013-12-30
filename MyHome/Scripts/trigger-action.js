$(function () {
    $('[authorize]').click(function (e) {
        if (!Classy.IsAuthenticated) {
            $('#login-modal').modal('show');
            e.stopImmediatePropagation(); e.preventDefault();
        }
    });

    $('[trigger-listing-action="favorite"]').click(function (e) {
        var listingId = $(this).attr('listing-id');
        var listingType = $(this).attr('listing-type');
        var url = "/" + listingType + "/" + listingId + "/favorite";
        $.post(url, null, function (data) { console.log(data); })
    });

    $('[trigger-listing-action="collect"]').click(function (e) {
        var listingId = $(this).attr('listing-id');
        var listingType = $(this).attr('listing-type');
        $('#collect-modal')
            .modal('show')
            .on('shown.bs.modal', function () {
                $('#listingId').val(listingId);
        });
    });

    $('[trigger-profile-action="follow"]').click(function (e) {
        var profileId = $(this).attr('profile-id');
        var url = "/profile/" + profileId + "/follow";
        $.post(url, null, function (data) { console.log(data); })
    });
});