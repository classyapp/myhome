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

function ChangePassword(e) {
    $('#change-password-modal')
        .modal('show')
        .on("loaded.bs.modal", function () { attachValidation(frmChangePassword); });
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
        bootbox.confirm({
            title: "HomeLab", message: msgConfirm, callback: function (result) {
                if (result) {
                    $.post("/" + listingType + "/" + listingId + "/delete", function (data) { if ("error" in data) { } else { thumb.prepend("<div class='deleted'></div>"); } });
                }
            }
        });
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
        bootbox.confirm({
            title: "HomeLab", message: msgConfirm, callback: function (result) {
                if (result) {
                    $.post("/collection/" + collectionId + "/remove/" + listingId, function (data) { if ("error" in data) { } else { thumb.closest(".row").remove(); } });
                }
            }
        });
    });

    $('[trigger-collection-action="delete"]', context).click(function (e) {
        bootbox.confirm({
            title: "HomeLab", message: msgConfirm, callback: function (result) {
                if (result) {
                    $.post($(e.target).data("href"), {}, function (response) {
                        if ("error" in response) {
                            $("#pageAlert").attr("class", "alert alert-danger alert-dismissable").find("span").html(response.error);
                        } else {
                            document.location.href = response.url;
                        }
                    });
                }
            }
        });
    });
}
