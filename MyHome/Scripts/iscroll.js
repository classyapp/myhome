function loadMorePhotos() {
    var iscroll = $(".iscroll");
    if (iscroll.data("hasmore")) {
        if ($(document).height() - ($("body").scrollTop()) - $(window).height() < 250) {
            if (iscroll.data("loading") != true) {
                iscroll.data("loading", true);
                var page = iscroll.data("page") || 2;
                $.get("/profile/1/all/photos", { page: page }, function (response) {
                    iscroll.data("loading", false);
                    var html = $(response);
                    if (html.find(".thumbnail").length < 9) { // less then page size
                        iscroll.data("hasmore", false);
                    } else {
                        iscroll.data("page", page + 1);
                    }
                    iscroll.append(response);
                });
            }
        }
    }
}

$(document).on("scroll", loadMorePhotos);
