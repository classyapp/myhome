
$('.poll .thumbnail.vote').click(function () {

    if (!Classy.SiteMetadata.LoggedIn)
        return;

    var imageClicked = $(this);
    var listings = $('.poll .thumbnail.vote');

    var hasVoted = function () {
        var voted = false;
        listings.each(function() {
            if ($(this).hasClass('voted')) {
                voted = true;
            }
        });
        return voted;
    };

    var submitVote = function (pollId, listingId) {
        $.ajax({
            type: 'post',
            url: '/polls/vote',
            data: {
                pollId: pollId,
                listingId: listingId
            }
        }).success(function () {

            var votedButton = $('.poll .thumbnail.vote.voted .vote-button');
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
    };

    if (!hasVoted()) {
        submitVote($('.poll-details .poll').data('poll-id'), imageClicked.data('listing-id'));
        return;
    }

    if ($(this).hasClass('voted')) {
        // the user voted on this listing already
        return;
    } else {
        // the user is changing their vote
        if (window.confirm('You already voted on this poll.\nThis will change your current vote. Are you sure ?')) {
            submitVote($('.poll-details .poll').data('poll-id'), imageClicked.data('listing-id'));
        }
    }
});
