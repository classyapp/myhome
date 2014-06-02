
$('.poll .thumbnail.vote').click(function () {

    if (!Classy.SiteMetadata.LoggedIn)
        return;

    var imageClicked = $(this);
    var listings = $('.poll .thumbnail.vote');
    var votedListing;

    var hasVoted = function () {
        var voted = false;
        listings.each(function() {
            if ($(this).hasClass('voted')) {
                voted = true;
                votedListing = $(this);
            }
        });
        return voted;
    };

    if (hasVoted() && $(this).hasClass('voted')) {
        return;
    } else {
        if (window.confirm('You already voted on this poll.\nThis will change your current vote. Are you sure ?')) {
            $.ajax({
                type: 'post',
                url: '/polls/vote',
                data: {
                    pollId: $('.poll-details .poll').data('poll-id'),
                    listingId: $(this).data('listing-id')
                }
            }).success(function () {

                var votedButton = votedListing.find('.vote-button');
                votedButton.find('i').addClass('hidden');
                votedButton[0].style.width = "66px";

                var button = imageClicked.find('.vote-button');
                button[0].style.width = button[0].offsetWidth + 'px';
                var a = button[0].offsetWidth + button[0].clientWidth;
                button[0].style.width = "86px";
                button.one('transitionend', function () {
                    button.find('i').removeClass('hidden');
                });

                $('.poll .thumbnail.vote').removeClass('voted');
                imageClicked.addClass('voted');

            }).fail(function () {
                alert('failed');
            });
        }
    }
});
