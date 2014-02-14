function loadMorePhotos() {
    var iscroll = $(".iscroll");
    var pageSize = parseInt($(".iscroll").data("page-size"));
    if (iscroll.data("hasmore")) {
        if ($(document).height() - ($("body").scrollTop()) - $(window).height() < 250) {
            if (iscroll.data("loading") != true) {
                iscroll.data("loading", true);
                var page = iscroll.data("page") + 1;
                var url = iscroll.data("url");
                $.get(url, { page: page, iscroll: 1 }, function (response) {
                    iscroll.data("loading", false);
                    var html = $(response);
                    var count = html.find(".thumbnail").length;
                    if (count == 0) return;
                    html.find(".thumbnail:nth-child(1)").attr("id", "page" + page);
                    if (html.find(".thumbnail").length < pageSize) { // less then page size
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
    var maxPages = $("ul.pagination").data("pages");
    var url = $(".iscroll").data("url");
    var cp = 0;
    var first = Math.max(page - Math.floor(maxPages / 2), 1);
    var last = 0;
    for (var i = 2, j = 0; i <= (maxPages + 1); i++, j++) {
        cp = first + j;
        if (cp <= total) {
            $("ul.pagination li:nth-child(" + i + ") a").html(cp).attr("href", url + "?Page=" + cp).parent().toggleClass("active", cp == page).show();
            last = cp;
        }
        else
            $("ul.pagination li:nth-child(" + i + ") a").html(cp).parent().toggleClass("active", false).hide();
    }
    $("ul.pagination li:nth-child(1) a").attr("href", (page > 1 ? url + "?Page=" + (page - 1) : ""));
    $("ul.pagination li:nth-child(7)").toggleClass("hide", cp > last).find("a").attr("href", (last < total ? url + "?Page=" + (last + 1) : ""));
    $("ul.pagination li:nth-child(1)").toggleClass("hide", page == 1);
}

$(document).on("scroll", loadMorePhotos);
