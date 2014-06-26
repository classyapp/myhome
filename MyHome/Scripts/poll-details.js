਍ഀ
$('.poll .thumbnail.vote').click(function () {਍ഀ
਍ഀ
    if (!Classy.SiteMetadata.LoggedIn)਍ഀ
        return;਍ഀ
਍ഀ
    var pollEnded = $('.poll-details').data('poll-ended');਍ഀ
    if (pollEnded) {਍ഀ
        bootbox.dialog({਍ഀ
            title: Classy.Messages.PollDetails_PollEndedDialogTitle,਍ഀ
            message: Classy.Messages.PollDetails_PollEndedDialogContent,਍ഀ
            onEscape: $.noop(),਍ഀ
            show: true,਍ഀ
            buttons: {਍ഀ
                success: {਍ഀ
                    label: Classy.Messages.Confirm_Ok,਍ഀ
                    className: "btn-success"਍ഀ
                }਍ഀ
            }਍ഀ
        });਍ഀ
        return;਍ഀ
    }਍ഀ
਍ഀ
    var imageClicked = $(this);਍ഀ
    var listings = $('.poll .thumbnail.vote');਍ഀ
਍ഀ
    var hasVoted = function () {਍ഀ
        var voted = false;਍ഀ
        listings.each(function() {਍ഀ
            if ($(this).hasClass('voted')) {਍ഀ
                voted = true;਍ഀ
            }਍ഀ
        });਍ഀ
        return voted;਍ഀ
    };਍ഀ
਍ഀ
    var submitVote = function (pollId, listingId) {਍ഀ
        Classy.ReportEvent('button', 'vote-on-poll');਍ഀ
        $.ajax({਍ഀ
            type: 'post',਍ഀ
            url: '/polls/vote',਍ഀ
            data: {਍ഀ
                pollId: pollId,਍ഀ
                listingId: listingId਍ഀ
            }਍ഀ
        }).success(function () {਍ഀ
਍ഀ
            var votedButton = $('.poll .thumbnail.vote.voted .vote-button');਍ഀ
            votedButton.find('i').addClass('hidden');਍ഀ
            votedButton.find('span').html(Classy.Messages.PollDetails_VoteButton);਍ഀ
਍ഀ
            var button = imageClicked.find('.vote-button');਍ഀ
            button.find('span').html(Classy.Messages.PollDetails_MyVote);਍ഀ
            button.find('i').removeClass('hidden');਍ഀ
਍ഀ
            $('.poll .thumbnail.vote').removeClass('voted');਍ഀ
            imageClicked.addClass('voted');਍ഀ
਍ഀ
        }).fail(function () {਍ഀ
            alert('failed');਍ഀ
        });਍ഀ
    };਍ഀ
਍ഀ
    if (!hasVoted()) {਍ഀ
        $(this).addClass('voted');਍ഀ
        submitVote($('.poll-details').data('poll-id'), imageClicked.data('listing-id'));਍ഀ
        return;਍ഀ
    }਍ഀ
਍ഀ
    if ($(this).hasClass('voted')) {਍ഀ
        // the user voted on this listing already਍ഀ
        return;਍ഀ
    } else {਍ഀ
        // the user is changing their vote਍ഀ
        bootbox.dialog({਍ഀ
            title: Classy.Messages.PollDetails_ChangeVotePopupTitle,਍ഀ
            message: Classy.Messages.PollDetails_ChangeVotePopupContent,਍ഀ
            onEscape: $.noop(),਍ഀ
            show: true,਍ഀ
            buttons: {਍ഀ
                cancel: {਍ഀ
                    label: Classy.Messages.Confirm_Cancel,਍ഀ
                    className: "btn-default",਍ഀ
                    callback: $.noop()਍ഀ
                },਍ഀ
                success: {਍ഀ
                    label: Classy.Messages.Confirm_Ok,਍ഀ
                    className: "btn-danger",਍ഀ
                    callback: function() {਍ഀ
                        submitVote($('.poll-details').data('poll-id'), imageClicked.data('listing-id'));਍ഀ
                    }਍ഀ
                }਍ഀ
            }਍ഀ
        });਍ഀ
    }਍ഀ
});਍ഀ�