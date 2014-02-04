$.validator.addMethod('accept', function () { return true; });
$("#uploadBtn").closest("form").bind('invalid-form.validate', function (form, validator) {
    var $list = $('.validation-summary-valid ul:first')
    if ($list.length && validator.errorList.length) {
        $list.empty();
        $.each(validator.errorList, function () {
            $("<li />").html(this.message).appendTo($list);
        });
    }
});
$("#file").on("change", function () {
    $("#filesPreview").html("");

    $.each(this.files, function (idx, item) {
        var reader = new FileReader();
        reader.onload = function (e) {
            var img = $("<div class=\"col-lg-4 col-md-4 col-sm-6 col-xs-6\"><div class=\"img-preview\" style=\"background-image: url(" + e.target.result + ")\"></div><div class=\"progress\"><div class=\"progress-bar\" role=\"progressbar\" aria-valuenow=\"60\" aria-valuemin=\"0\" aria-valuemax=\"100\" style=\"width: 0%;\"></div></div></div>");
            $("#filesPreview").append(img);
        };

        reader.readAsDataURL(item);
    });
});

function uploadPhotos() {
    var frm = $("#uploadBtn").closest("form");
    if (frm.valid()) {
        // upload files
        var idx = 0;
        $("#uploadBtn").data("file-id", idx);
        var extra = {};
        frm.find(".form-control").each(function (idx, item) { extra[$(item).attr("name")] = $(item).val() });
        doUpload(idx, extra);
    }
}

function doUpload(idx, extra) {
    if (file.files.length > idx) {
        $("#uploadBtn").data("file-id", idx);
        var upload = new XHRFileUpload(uploadPhotoUrl, file.files[idx], extra);
        upload.upload(function (e) {
            if (e.lengthComputable) {
                var percentComplete = (e.loaded / e.total) * 100;
                $("#filesPreview > div:nth-child(" + (idx + 1) + ") .progress-bar").css("width", percentComplete + '%');
            }
        }, function (e) {
            setTimeout(function () { doUpload(idx + 1, extra); }, 100);
        }, function (e) {
            alert("Something went wrong!!!");
            setTimeout(function () { doUpload(idx + 1, extra); }, 100);
        });
    } else {
        document.location.href = redirectUrl;
    }
}