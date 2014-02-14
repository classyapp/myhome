function loadMorePhotos() {
    var iscroll = $(".iscroll");
    if (iscroll.data("hasmore")) {
        if ($(document).height() - ($("body").scrollTop()) - $(window).height() < 250) {
            if (iscroll.data("loading") != true) {
                iscroll.data("loading", true);
                var page = iscroll.data("page") || 2;
                var url = iscroll.data("url");
                $.get(url, { page: page, iscroll: 1 }, function (response) {
                    iscroll.data("loading", false);
                    var html = $(response);
                    var count = html.find(".thumbnail").length;
                    if (count == 0) return;
                    html.find(".thumbnail:nth-child(1)").attr("id", "page2");
                    if (html.find(".thumbnail").length < 9) { // less then page size
                        iscroll.data("hasmore", false);
                    } else {
                        iscroll.data("page", page + 1);
                    }
                    html.find('.thumbnail').mouseover(function () {
                        $(this).find('.actions').removeClass('hide');
                    });
                    html.find('.thumbnail').mouseout(function () {
                        $(this).find('.actions').addClass('hide');
                    });
                    iscroll.append(html);
                    bindTriggerActions(html);
                    resetPagination(page);
                });
            }
        }
    }
}

function resetPagination(page) {
    var total = $("ul.pagination").data("total");
    var cp = 0;
    var first = page < 4 ? 1 : page - 2;
    var last = 0;
    for (var i = 2; i <= 6; i++) {
        cp = page < 4 ? i - 1 : page - 4 + i;
        if (cp <= total) {
            $("ul.pagination li:nth-child(" + i + ") a").html(cp).attr("href", "#page" + cp).parent().toggleClass("active", cp == page).show();
            last = cp;
        }
        else
            $("ul.pagination li:nth-child(" + i + ") a").html(cp).parent().toggleClass("active", false).hide();
    }
    $("ul.pagination li:nth-child(7)").toggleClass("hide", cp < last);
    $("ul.pagination li:nth-child(1)").toggleClass("hide", page == 1);
}

$(document).on("scroll", loadMorePhotos);
