਍␀⠀✀⸀瀀漀氀氀 ⸀琀栀甀洀戀渀愀椀氀⸀瘀漀琀攀✀⤀⸀挀氀椀挀欀⠀昀甀渀挀琀椀漀渀 ⠀⤀ 笀ഀ
਍    椀昀 ⠀℀䌀氀愀猀猀礀⸀匀椀琀攀䴀攀琀愀搀愀琀愀⸀䰀漀最最攀搀䤀渀⤀ഀ
        return;਍ഀ
    var pollEnded = $('.poll-details').data('poll-ended');਍    椀昀 ⠀瀀漀氀氀䔀渀搀攀搀⤀ 笀ഀ
        bootbox.dialog({਍            琀椀琀氀攀㨀 䌀氀愀猀猀礀⸀䴀攀猀猀愀最攀猀⸀倀漀氀氀䐀攀琀愀椀氀猀开倀漀氀氀䔀渀搀攀搀䐀椀愀氀漀最吀椀琀氀攀Ⰰഀ
            message: Classy.Messages.PollDetails_PollEndedDialogContent,਍            漀渀䔀猀挀愀瀀攀㨀 ␀⸀渀漀漀瀀⠀⤀Ⰰഀ
            show: true,਍            戀甀琀琀漀渀猀㨀 笀ഀ
                success: {਍                    氀愀戀攀氀㨀 䌀氀愀猀猀礀⸀䴀攀猀猀愀最攀猀⸀䌀漀渀昀椀爀洀开伀欀Ⰰഀ
                    className: "btn-success"਍                紀ഀ
            }਍        紀⤀㬀ഀ
        return;਍    紀ഀ
਍    瘀愀爀 椀洀愀最攀䌀氀椀挀欀攀搀 㴀 ␀⠀琀栀椀猀⤀㬀ഀ
    var listings = $('.poll .thumbnail.vote');਍ഀ
    var hasVoted = function () {਍        瘀愀爀 瘀漀琀攀搀 㴀 昀愀氀猀攀㬀ഀ
        listings.each(function() {਍            椀昀 ⠀␀⠀琀栀椀猀⤀⸀栀愀猀䌀氀愀猀猀⠀✀瘀漀琀攀搀✀⤀⤀ 笀ഀ
                voted = true;਍            紀ഀ
        });਍        爀攀琀甀爀渀 瘀漀琀攀搀㬀ഀ
    };਍ഀ
    var submitVote = function (pollId, listingId) {਍        䌀氀愀猀猀礀⸀刀攀瀀漀爀琀䔀瘀攀渀琀⠀✀戀甀琀琀漀渀✀Ⰰ ✀瘀漀琀攀ⴀ漀渀ⴀ瀀漀氀氀✀⤀㬀ഀ
        $.ajax({਍            琀礀瀀攀㨀 ✀瀀漀猀琀✀Ⰰഀ
            url: '/polls/vote',਍            搀愀琀愀㨀 笀ഀ
                pollId: pollId,਍                氀椀猀琀椀渀最䤀搀㨀 氀椀猀琀椀渀最䤀搀ഀ
            }਍        紀⤀⸀猀甀挀挀攀猀猀⠀昀甀渀挀琀椀漀渀 ⠀⤀ 笀ഀ
਍            瘀愀爀 瘀漀琀攀搀䈀甀琀琀漀渀 㴀 ␀⠀✀⸀瀀漀氀氀 ⸀琀栀甀洀戀渀愀椀氀⸀瘀漀琀攀⸀瘀漀琀攀搀 ⸀瘀漀琀攀ⴀ戀甀琀琀漀渀✀⤀㬀ഀ
            votedButton.find('i').addClass('hidden');਍            瘀漀琀攀搀䈀甀琀琀漀渀⸀昀椀渀搀⠀✀猀瀀愀渀✀⤀⸀栀琀洀氀⠀䌀氀愀猀猀礀⸀䴀攀猀猀愀最攀猀⸀倀漀氀氀䐀攀琀愀椀氀猀开嘀漀琀攀䈀甀琀琀漀渀⤀㬀ഀ
਍            瘀愀爀 戀甀琀琀漀渀 㴀 椀洀愀最攀䌀氀椀挀欀攀搀⸀昀椀渀搀⠀✀⸀瘀漀琀攀ⴀ戀甀琀琀漀渀✀⤀㬀ഀ
            button.find('span').html(Classy.Messages.PollDetails_MyVote);਍            戀甀琀琀漀渀⸀昀椀渀搀⠀✀椀✀⤀⸀爀攀洀漀瘀攀䌀氀愀猀猀⠀✀栀椀搀搀攀渀✀⤀㬀ഀ
਍            ␀⠀✀⸀瀀漀氀氀 ⸀琀栀甀洀戀渀愀椀氀⸀瘀漀琀攀✀⤀⸀爀攀洀漀瘀攀䌀氀愀猀猀⠀✀瘀漀琀攀搀✀⤀㬀ഀ
            imageClicked.addClass('voted');਍ഀ
        }).fail(function () {਍            愀氀攀爀琀⠀✀昀愀椀氀攀搀✀⤀㬀ഀ
        });਍    紀㬀ഀ
਍    椀昀 ⠀℀栀愀猀嘀漀琀攀搀⠀⤀⤀ 笀ഀ
        $(this).addClass('voted');਍        猀甀戀洀椀琀嘀漀琀攀⠀␀⠀✀⸀瀀漀氀氀ⴀ搀攀琀愀椀氀猀✀⤀⸀搀愀琀愀⠀✀瀀漀氀氀ⴀ椀搀✀⤀Ⰰ 椀洀愀最攀䌀氀椀挀欀攀搀⸀搀愀琀愀⠀✀氀椀猀琀椀渀最ⴀ椀搀✀⤀⤀㬀ഀ
        return;਍    紀ഀ
਍    椀昀 ⠀␀⠀琀栀椀猀⤀⸀栀愀猀䌀氀愀猀猀⠀✀瘀漀琀攀搀✀⤀⤀ 笀ഀ
        // the user voted on this listing already਍ഀ
        return;਍    紀 攀氀猀攀 笀ഀ
        // the user is changing their vote਍ഀ
        bootbox.dialog({਍            琀椀琀氀攀㨀 䌀氀愀猀猀礀⸀䴀攀猀猀愀最攀猀⸀倀漀氀氀䐀攀琀愀椀氀猀开䌀栀愀渀最攀嘀漀琀攀倀漀瀀甀瀀吀椀琀氀攀Ⰰഀ
            message: Classy.Messages.PollDetails_ChangeVotePopupContent,਍            漀渀䔀猀挀愀瀀攀㨀 ␀⸀渀漀漀瀀⠀⤀Ⰰഀ
            show: true,਍            戀甀琀琀漀渀猀㨀 笀ഀ
                cancel: {਍                    氀愀戀攀氀㨀 䌀氀愀猀猀礀⸀䴀攀猀猀愀最攀猀⸀䌀漀渀昀椀爀洀开䌀愀渀挀攀氀Ⰰഀ
                    className: "btn-default",਍                    挀愀氀氀戀愀挀欀㨀 ␀⸀渀漀漀瀀⠀⤀ഀ
                },਍                猀甀挀挀攀猀猀㨀 笀ഀ
                    label: Classy.Messages.Confirm_Ok,਍                    挀氀愀猀猀一愀洀攀㨀 ∀戀琀渀ⴀ搀愀渀最攀爀∀Ⰰഀ
                    callback: function() {਍                        猀甀戀洀椀琀嘀漀琀攀⠀␀⠀✀⸀瀀漀氀氀ⴀ搀攀琀愀椀氀猀✀⤀⸀搀愀琀愀⠀✀瀀漀氀氀ⴀ椀搀✀⤀Ⰰ 椀洀愀最攀䌀氀椀挀欀攀搀⸀搀愀琀愀⠀✀氀椀猀琀椀渀最ⴀ椀搀✀⤀⤀㬀ഀ
                    }਍                紀ഀ
            }਍        紀⤀㬀ഀ
    }਍