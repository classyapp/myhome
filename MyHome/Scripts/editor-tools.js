
var Classy = Classy || {};

Classy.EditorTools = function() {

    var isSelecting = false;
    var selectedListings = [];

    var init = function() {
        $('.btn.select-tool').click(function() {
            if ($(this).hasClass('active')) {
                disableSelectMode();
                $(this).removeClass('active');
            } else {
                enableSelectMode();
                $(this).addClass('active');
            }
        });

        $('.editor-tools .btn.save').click(save);
    };

    var enableSelectMode = function() {
        isSelecting = true;
        $(document).delegate('.photo.thumbnail a img', 'click', function () {
            if (!isSelecting) return true; // so if we're not in select mode anymore, the link will redirect

            var img = $(this);
            var listingId = img.parents('.photo.thumbnail').data('listing-id');
            if (img.hasClass('selected')) {
                selectedListings.splice(selectedListings.indexOf(listingId), 1);
                img.removeClass('selected');
            } else {
                selectedListings.push(listingId);
                img.addClass('selected');
            }
            return false; // so the link won't redirect
        });
    };

    var disableSelectMode = function() {
        selectedListings = [];
        isSelecting = false;
        $('.photo.thumbnail img').removeClass('selected');
    };

    var save = function() {
        if (selectedListings.length == 0)
            return;

        var data = {
            listingIds: selectedListings,
            room: $('#room-dropdown').val(),
            style: $('#style-dropdown').val()
        };
        if ($('#editor-rank').val().trim() != '')
            data.editorsRank = $('#editor-rank').val().trim();

        $.ajax({
            type: 'post',
            url: '/listings/edit-multiple',
            data: data,
            dataType: 'json',
            traditional: true
        }).success(function() {
            alert('listings updated succesfully');
        }).error(function() {
            alert('error while saving changes');
        });
    };

    return {
        Init: init
    };

};

$(function() {
    Classy.EditorTools().Init();
});
