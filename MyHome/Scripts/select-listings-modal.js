
Classy.Polls = Classy.Polls || {};

Classy.Polls.SelectListingsModal = function() {

    var modalContainer = $('#select-listings-modal');
    var modalBody = modalContainer.find('.modal-body');
    var photosContainer = modalBody.find('.photos-container .photos');
    var collectionsContainer = modalBody.find('.collections-container')
    var loader = modalBody.find('.loader');

    var photosCache = {};

    var init = function () {
        modalBody.find('.collection').click(function () {
            var collectionId = $(this).data('collection-id');
            loadCollectionListings(collectionId);
        });
        modalBody.find('.back').click(function() {
            modalBody.find('.photos-container').addClass('hidden');
            modalBody.find('.collections-container').removeClass('hidden');
        });
        modalContainer.find('.save-btn').click(saveAndClose);
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
        photosContainer.html('');
        collectionsContainer.addClass('hidden');
        $.each(photos, function () {
            var photoDiv = $('<div style=\"padding:0 14px;\"/>').addClass('col-md-4 thumbnail');
            photoDiv.append($('<img src=\"' + Classy.Images.Thumbnail(this.Image, 166, 166) + '\"/>')
                .data('photo-id', this.Image)
                .data('listing-id', this.Id)
                .addClass('selectable'));

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
        photosContainer.find('.selectable').click(function () {
            if ($(this).hasClass('selected')) {
                $(this).removeClass('selected');
                $(this).siblings('.select').removeClass('selected');
            } else {
                if (photosContainer.find('.select.selected').length >= 4)
                    return;

                $(this).addClass('selected');
                $(this).siblings('.select').addClass('selected');
            }
        });
    };

    var saveAndClose = function () {
        var previewContainer = $('#listings-preview');
        previewContainer.html('');
        modalBody.find('.thumbnail img.selected').each(function() {
            $('<div/>').addClass('col-md-4')
                .append($('<img src=\"' + $(this).attr('src') + '\"/>').data('listing-id', $(this).data('listing-id')).data('photo-id', $(this).data('photo-id')))
                .appendTo(previewContainer);
        });
        modalContainer.modal('hide');
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