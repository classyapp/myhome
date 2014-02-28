var Classy = {};

Classy.AjaxReconnect = function () {
    $(document).trigger("classy.ajax.reconnect");
}

Classy.ChangeProfilePhoto = function (file) {
    if (file.value != "") {
        var upload = new XHRFileUpload($(file).data("url"), file.files[0], { Fields: 16, ProfileId: 2});
        upload.upload(function (e) { }, function (e) {
            $(file).closest(".image").find("img").attr("src", e.url);
            alert("GREAT SUCCESS!!!");
        }, function (e) {
            alert("Something went wrong!!!");
        });
    }
}