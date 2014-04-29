function FavoriteListing(e)
{
    e.preventDefault();
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

function UnfavoriteListing(e) {
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
    bindTriggerActions($(document));
});

function bindTriggerActions(context) {
    $('[authorize]').click(function (e) {
        if (!Classy.IsAuthenticated) {
            $('#login-modal').modal('show');
            e.stopImmediatePropagation(); e.preventDefault();
        }
    });

    $('[trigger-listing-action="favorite"]', context).click(FavoriteListing);

    $('[trigger-listing-action="unfavorite"]', context).click(UnfavoriteListing);

    $('[trigger-listing-action="collect"]').click(function (e) {
        e.preventDefault();
        var listingId = $(this).attr('listing-id');
        var listingType = $(this).attr('listing-type');
        $('#collect-modal')
            .modal('show')
            .on('loaded.bs.modal', function () {
                $('#listingId').val(listingId);
            });
    });

    $('[trigger-listing-action="edit"]', context).click(function (e) {
        e.preventDefault();
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
        e.preventDefault();
        var listingId = $(this).attr('listing-id');
        var listingType = $(this).attr('listing-type');
        var thumb = $(this).closest(".thumbnail");
        bootbox.dialog({
            title: Classy.Messages["Delete" + listingType + "_ConfirmTitle"],
            message: Classy.Messages["Delete" + listingType + "_ConfirmText"],
            onEscape: function () { },
            show: true,
            buttons: {
                cancel: {
                    label: Classy.Messages.Confirm_Cancel, className: "btn-default", callback: function () { }
                },
                success: {
                    label: Classy.Messages.Confirm_Yes, className: "btn-danger", callback: function () {
                        $.post("/" + listingType + "/" + listingId + "/delete", function (response) {
                            if ("error" in response) {
                                $("#pageAlert").attr("class", "alert alert-danger alert-dismissable").find("span").html(response.error);
                            } else {
                                thumb.prepend("<div class='deleted'></div>");
                                // check last image
                                if ($(".photo.thumbnail").length == $(".photo.thumbnail > div.deleted").length) {
                                    bootbox.dialog({
                                        title: Classy.Messages["DeleteCollection_ConfirmTitle"],
                                        message: Classy.Messages["DeleteCollection_EmptyConfirmText"],
                                        onEscape: function () { },
                                        show: true,
                                        buttons: {
                                            cancel: {
                                                label: Classy.Messages.Confirm_Cancel, className: "btn-default", callback: function () { }
                                            },
                                            success: {
                                                label: Classy.Messages.Confirm_Yes, className: "btn-danger", callback: function () {
                                                    $.post("/collection/" + $(".collection.row").data("id") + "/delete", function (response) {
                                                        if ("error" in response) {
                                                            $("#pageAlert").attr("class", "alert alert-danger alert-dismissable").find("span").html(response.error);
                                                        } else {
                                                            document.location.href = response.url;
                                                        }
                                                    });
                                                }
                                            }
                                        }
                                    });
                                }
                            }
                        });
                    }
                }
            }
        });
    });

    $('[trigger-profile-action="follow"]', context).click(function (e) {
        e.preventDefault();
        var profileId = $(this).attr('profile-id');
        var url = "/profile/" + profileId + "/follow";
        var el = $(this);
        $.post(url, null, function (data) {
            el.addClass('hidden');
            $('[trigger-profile-action="unfollow"]', context).removeClass('hidden');
        })
    });

    $('[trigger-profile-action="unfollow"]', context).click(function (e) {
        e.preventDefault();
        var profileId = $(this).attr('profile-id');
        var url = "/profile/" + profileId + "/unfollow";
        var el = $(this);
        $.post(url, null, function (data) {
            el.addClass('hidden');
            $('[trigger-profile-action="follow"]', context).removeClass('hidden');
        })
    });

    $('[trigger-listing-action="remove"]', context).click(function (e) {
        e.preventDefault();
        var listingId = $(this).attr('listing-id');
        var listingType = $(this).attr('listing-type');
        var collectionId = $(this).closest(".collection-items").attr('collection-id');
        var thumb = $(this).closest(".thumbnail");
        bootbox.dialog({
            title: Classy.Messages["CollectionRemove" + listingType + "_ConfirmTitle"],
            message: Classy.Messages["CollectionRemove" + listingType + "_ConfirmText"],
            onEscape: function () { },
            show: true,
            buttons: {
                cancel: {
                    label: Classy.Messages.Confirm_Cancel, className: "btn-default", callback: function () { }
                },
                success: {
                    label: Classy.Messages.Confirm_Yes, className: "btn-danger", callback: function () {
                        $.post("/collection/" + collectionId + "/remove/" + listingId, function (response) {
                            if ("error" in response) {
                                $("#pageAlert").attr("class", "alert alert-danger alert-dismissable").find("span").html(response.error);
                            } else {
                                thumb.closest(".row").remove();
                            }
                        });
                    }
                }
            }
        });
    });

    $('[trigger-collection-action="delete"]', context).click(function (e) {
        e.preventDefault();
        bootbox.dialog({
            title: Classy.Messages.DeleteCollection_ConfirmTitle,
            message: Classy.Messages.DeleteCollection_ConfirmText,
            onEscape: function () { },
            show: true,
            buttons: {
                cancel: {
                    label: Classy.Messages.Confirm_Cancel, className: "btn-default", callback: function () { }
                },
                success: {
                    label: Classy.Messages.Confirm_Yes, className: "btn-danger", callback: function () {
                        $.post($(e.target).data("href"), {}, function (response) {
                            if ("error" in response) {
                                $("#pageAlert").attr("class", "alert alert-danger alert-dismissable").find("span").html(response.error);
                            } else {
                                document.location.href = response.url;
                            }
                        });
                    }
                }
            }
        });
    });

    $('[trigger-project-action="delete"]', context).click(function (e) {
        e.preventDefault();
        bootbox.dialog({
            title: Classy.Messages.DeleteProject_ConfirmTitle,
            message: Classy.Messages.DeleteProject_ConfirmText,
            onEscape: function () {}, 
            show: true,
            buttons: {
                cancel: {
                    label: Classy.Messages.Confirm_Cancel, className: "btn-default", callback: function () { }
                },
                success: {
                    label: Classy.Messages.Confirm_Yes, className: "btn-danger", callback: function () {
                        $.post($(e.target).data("href"), {}, function (response) {
                            if ("error" in response) {
                                $("#pageAlert").attr("class", "alert alert-danger alert-dismissable").find("span").html(response.error);
                            } else {
                                document.location.href = response.url;
                            }
                        });
                    }
                }
            }
        });
    });

    $('[trigger-listing-action="translate"], [trigger-collection-action="translate"]', context).click(function (e) {
        var objectId = $(this).attr('object-id') || $(this).attr('listing-id');
        var objectType = $(this).attr('object-type') || $(this).attr('listing-type');
        $('#translate-modal')
            .data("remote", "/" + objectType + "/" + objectId + "/translate")
            .modal('show');
    });
}
