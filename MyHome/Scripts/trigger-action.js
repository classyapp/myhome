function FavoriteListing(e)
{
    var listingId = $(this).attr('listing-id');
    var listingType = $(this).attr('listing-type');
    var url = "/" + listingType + "/" + listingId + "/favorite";
    var button = $(this);
    $.post(url, null, function (data) {
        button.addClass('like-on');
        button.unbind(e);
        button.click(UnfavoriteListing);
    });
}

function UnfavoriteListing(e)
{

    e.preventDefault();
    var listingId = $(this).attr('listing-id');
    var listingType = $(this).attr('listing-type');
    var url = "/" + listingType + "/" + listingId + "/unfavorite";
    var button = $(this);
    $.post(url, null, function (data) {
        button.removeClass('like-on');
        button.unbind(e);
        button.click(FavoriteListing);
    });
}

$(function () {
    $('[authorize]').click(function (e) {
        if (!Classy.IsAuthenticated) {
            $('#login-modal').modal('show');
            e.stopImmediatePropagation(); e.preventDefault();
        }
    });

    bindTriggerActions($(document));
});


sting);
function bindTriggerActions(con    $('[trigger-listing-action="favorite"]').click(FavoriteListing);
    $('[trigger-listing-action="unfavorite"]').click(UnfavoriteListing);
    });

    $('[trigger-listing-action="collect"]').click(function (e) {
        var listingId = $(this).attr('listing-id');
        var listingType = $(this).attr('listing-type');
        $('#collect-modal')
            .modal('show')
            .on('loaded.bs.modal', function () {
                $('#listingId').val(listingId);
            });
    });

    $('[trigger-listing-action="edit"]').click(function (e) {
        var listingId = $(this).attr('listing-id');
        var listingType = $(this).attr('listing-type');
        var url = $('#photo-modal').data("url");
        $('#photo-modal').data("remote", url.replace("/ID/", "/" + listingId + "/"));
        $('#photo-modal')
            .modal('show')
            .on('loaded.bs.modal', function () {
                var that = this;
                $('#listingId').val(listingId);
                setTimeout(function () {
                    $("#editPhotoPreview").height($('#photo-modal form').height());
                    $("#editPhotoPreview").width($("#editPhotoPreview").parent().width());
                    $("#editPhotoPreview > img").css("display", "inline");
                    attachValidation($(that).find("form"));
                }, 200);
            });
    });

    $('[trigger-listing-action="delete"]').click(function (e) {
        var listingId = $(this).attr('listing-id');
        var listingType = $(this).attr('listing-type');
        var thumb = $(this).closest(".thumbnail");
        if (confirm(msgConfirm)) {
            $.post("/" + listingType + "/" + listingId, function (data) { if ("error" in data) { } else { thumb.prepend("<div class='deleted'></div>"); } });
        }
    });

    $('[trigger-profile-action="follow"]').click(function (e) {
        var profileId = $(this).attr('profile-id');
        var url = "/profile/" + profileId + "/follow";
        $.post(url, null, function (data) { console.log(data); })
    });
}