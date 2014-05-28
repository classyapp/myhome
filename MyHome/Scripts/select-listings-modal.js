
Classy.Polls = Classy.Polls || {};

Classy.Polls.SelectListingsModal = function() {

    var modalContainer = $('#select-listings-modal');
    var modalBody = modalContainer.find('.modal-body');
    var photosContainer = modalBody.find('.photos-container .photos');
    var loader = modalBody.find('.loader');

    var photosCache = {};
    var selectedPhotos = [];

    var init = function () {
        modalBody.find('.collection').click(function () {
            var collectionId = $(this).data('collection-id');
            loadCollectionListings(collectionId);
        });
    };

    var loadCollectionListings = function (collectionId) {

        if (photosCache.hasOwnProperty(collectionId)) {
            renderPhotos(photosCache[collectionId]);
            return;
        }

        showLoader();
        $.ajax({
            type: 'post',
            url: '/polls/create/select-photos-modal',
            data: { collectionId: collectionId },
            dataType: 'json'
        }).success(function(data) {
            photosCache[collectionId] = data;
            renderPhotos(data);
        }).error(function() {
            alert('error while fetching photos');
        }).done(hideLoader);
    };

    var renderPhotos = function (photos) {
        debugger;
        photosContainer.html('');
        $.each(photos, function () {
            var photoDiv = $('<div style=\"padding:0 14px;\"/>').addClass('col-md-4 thumbnail');
            photoDiv.append($('<img src=\"' + Classy.Images.Thumbnail(this.Image, 166, 166) + '\"/>').data('photo-id', this.Image).addClass('selectable'));

            $('<span style=\"margin-right:14px;\"/>')
                .addClass('select')
                .append($('<i class="glyphicon glyphicon-ok"></i>'))
                .appendTo(photoDiv);

            photosContainer.append(photoDiv);
        });
        $('.photos-container').removeClass('hidden');

        bindPhotosSelectionEvents();
    };

    var bindPhotosSelectionEvents = function() {
        selectedPhotos = [];
        photosContainer.find('.selectable').click(function () {
            if ($(this).hasClass('selected')) {
                selectedPhotos = selectedPhotos.splice(selectedPhotos.indexOf($(this).data('photo-id')), 1);
                $(this).removeClass('selected');
                $(this).siblings('.select').removeClass('selected');
            } else {
                selectedPhotos.push($(this).data('photo-id'));
                $(this).addClass('selected');
                $(this).siblings('.select').addClass('selected');
            }
        });
    };

    var showLoader = function() {
        modalBody.find('.hide-when-loading').addClass('hidden');
        loader.removeClass('hidden');
    };
    var hideLoader = function() {
        loader.addClass('hidden');
    };

    return {
        Init: init
    };

};