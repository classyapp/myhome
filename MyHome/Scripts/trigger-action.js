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
    $('.thumbnail').mouseover(function () {
        $(this).find('.actions').removeClass('hidden');
    });
    $('.thumbnail').mouseout(function () {
        $(this).find('.actions').addClass('hidden');
    });

    $('[authorize]').click(function (e) {
        if (!Classy.IsAuthenticated) {
            $('#login-modal').modal('show');
            e.stopImmediatePropagation(); e.preventDefault();
        }
    });

    bindTriggerActions($(document));
});

function bindTriggerActions(context) {
    $('[trigger-listing-action="favorite"]', context).click(FavoriteListing);

    $('[trigger-listing-action="unfavorite"]', context).click(UnfavoriteListing);

    $('[trigger-listing-action="collect"]').click(function (e) {
        var listingId = $(this).attr('listing-id');
        var listingType = $(this).attr('listing-type');
        $('#collect-modal')
            .modal('show')
            .on('loaded.bs.modal', function () {
                $('#listingId').val(listingId);
            });
    });

    $('[trigger-listing-action="edit"]', context).click(function (e) {
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

    $('[trigger-listing-action="delete"]', context).click(function (e) {
        var listingId = $(this).attr('listing-id');
        var listingType = $(this).attr('listing-type');
        var thumb = $(this).closest(".thumbnail");
        if (confirm(msgConfirm)) {
            $.post("/" + listingType + "/" + listingId, function (data) { if ("error" in data) { } else { thumb.prepend("<div class='deleted'></div>"); } });
        }
    });

    $('[trigger-profile-action="follow"]', context).click(function (e) {
        var profileId = $(this).attr('profile-id');
        var url = "/profile/" + profileId + "/follow";
        $.post(url, null, function (data) { console.log(data); })
    });

    $('[trigger-listing-action="remove"]', context).click(function (e) {
        var listingId = $(this).attr('listing-id');
        var collectionId = $(this).closest(".collection-items").attr('collection-id');
        var thumb = $(this).closest(".thumbnail");
        if (confirm(msgConfirm)) {
            $.post("/collection/" + collectionId + "/remove/" + listingId, function (data) { if ("error" in data) { } else { thumb.closest(".row").remove(); } });
        }
    });
}
