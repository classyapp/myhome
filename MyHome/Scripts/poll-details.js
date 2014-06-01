
$('.poll .thumbnail.vote').click(function () {
    // TODO: check if the user voted already...
    $.ajax({
        type: 'post',
        url: '/polls/vote',
        data: {
            pollId: $('.poll-details .poll').data('poll-id'),
            listingId: $(this).data('listing-id')
        }
    }).success(function() {
        alert('voted');
    }).fail(function() {
        alert('failed');
    });
});
