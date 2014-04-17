﻿$.validator.addMethod('accept', function () { return true; });
$("#file").on("change", function () {
    $("#filesPreview").html("");
    $("#DummyFile").val("");

    var totalSize = 0;
    $.each(this.files, function (idx, item) {
        $("#DummyFile").val("fs");
        if (!!window.FileReader && totalSize < 4096000) {
            totalSize += item.size;
            var reader = new FileReader();
            reader.onload = function (e) {
                var img = $("<div class=\"col-lg-4 col-md-4 col-sm-6 col-xs-6\"><div class=\"img-preview\" style=\"background-image: url(" + e.target.result + ")\"></div><div class=\"filename\">" + item.name + "</div><div class=\"progress\"><div class=\"progress-bar\" role=\"progressbar\" aria-valuenow=\"60\" aria-valuemin=\"0\" aria-valuemax=\"100\" style=\"width: 0%;\"></div></div></div>");
                $("#filesPreview").prepend(img); // use prepend to add to the beginning as it will take time to actually load the content
            };

            reader.readAsDataURL(item);
        }
        else {
            var img = $("<div class=\"col-lg-4 col-md-4 col-sm-6 col-xs-6\"><div class=\"img-preview\" style=\"background-image: url(/img/homelab-logo.png)\"></div><div class=\"filename\">" + item.name + "</div><div class=\"progress\"><div class=\"progress-bar\" role=\"progressbar\" aria-valuenow=\"60\" aria-valuemin=\"0\" aria-valuemax=\"100\" style=\"width: 0%;\"></div></div></div>");
            $("#filesPreview").append(img);
        }
    });
});

function showFBPhotoChooser() {
    if ($("#filesPreview .img-preview").length > 0) {
        bootbox.dialog({
            title: Classy.Messages.CreatePhoto_ChangeSelectionTitle,
            message: Classy.Messages.CreatePhoto_ChangeSelectionText,
            onEscape: function () { },
            show: true,
            buttons: {
                cancel: {
                    label: Classy.Messages.Confirm_Cancel, className: "btn-default", callback: function () { }
                },
                success: {
                    label: Classy.Messages.Confirm_Yes, className: "btn-danger", callback: function () {
                        $("#DummyFile").val("");
                        $("#filesPreview").html("");
                        $("#load-fb-modal").modal('show');
                    }
                }
            }
        });
    } else {
        $("#load-fb-modal").modal('show');
    }
}

function previewSelectedFBPhotos(urls) {
    $("#DummyFile").val("");
    for (var i = 0; i < urls.length; i++) {
        $("#DummyFile").val("url");
        var img = $("<div class=\"col-lg-4 col-md-4 col-sm-6 col-xs-6\"><div class=\"img-preview\" data-url=\"" + urls[i] + "\" style=\"background-image: url(" + urls[i] + ")\"></div><div class=\"progress\"><div class=\"progress-bar\" role=\"progressbar\" aria-valuenow=\"60\" aria-valuemin=\"0\" aria-valuemax=\"100\" style=\"width: 0%;\"></div></div></div>");
        $("#filesPreview").prepend(img); // use prepend to add to the beginning as it will take time to actually load the content
    }
}

function uploadPhotos() {
    var frm = $("#uploadBtn").closest("form");
    if (frm.valid()) {
        // upload files
        if (!!window.FileReader) {
            var idx = 0;
            $("#uploadBtn").data("file-id", idx);
            var extra = {};
            frm.find(".form-control,input[type=checkbox],select,input[type=hidden]").each(function (idx, item) { if($(item).attr("id") != null) extra[$(item).attr("name")] = $(item).val() });
            $("#file, #uploadBtn").prop("disabled", "disabled");
            doUpload(idx, extra);
        }
        else {
            frm[0].submit();
        }
    }
}

function doUpload(idx, extra) {
    var file = $("#file")[0];
    if ($("#DummyFile").val() == "fs" && file.files.length > idx) {
        $("#uploadBtn").data("file-id", idx);
        var upload = new XHRFileUpload(uploadPhotoUrl, file.files[idx], extra);
        upload.upload(function (e) {
            if (e.lengthComputable) {
                var percentComplete = (e.loaded / e.total) * 100;
                $("#filesPreview > div:nth-child(" + (idx + 1) + ") .progress-bar").css("width", percentComplete + '%');
            }
        }, function (e) {
            if (extra.CollectionId == "") { extra.CollectionId = e.collectionId }
            setTimeout(function () { doUpload(idx + 1, extra); }, 100);
        }, function (e) {
            alert("Something went wrong!!!");
            setTimeout(function () { doUpload(idx + 1, extra); }, 100);
        });
    } else if ($("#DummyFile").val() == "url" && $("#filesPreview .img-preview").length > idx) {
        extra.OriginUrl = "http://www.facebook.com";
        extra.ExternalMediaUrl = $($("#filesPreview .img-preview")[idx]).data("url");
        $("#filesPreview > div:nth-child(" + (idx + 1) + ") .progress-bar").css("width", '20%');
        $.post(uploadPhotoFromUrlUrl, extra, function (response) {
            $("#filesPreview > div:nth-child(" + (idx + 1) + ") .progress-bar").css("width", '100%');
            if (extra.CollectionId == "") { extra.CollectionId = response.collectionId }
            setTimeout(function () { doUpload(idx + 1, extra); }, 100);
        });
    }
    else {
        document.location.href = redirectUrl;
    }
}

// check file reader support
if (!!window.FileReader && !!window.FormData) {
} else {
    $("#file").removeProp("multiple");
}

jQuery.validator.unobtrusive.adapters.add("brequired", function (options) {
    //b-required for checkboxes
    if (options.element.tagName.toUpperCase() == "INPUT" && options.element.type.toUpperCase() == "CHECKBOX") {
        //setValidationValues(options, "required", true);
        options.rules["required"] = true;
        if (options.message) {
            options.messages["required"] = options.message;
        }
    }
});