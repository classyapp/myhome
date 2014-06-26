
classy.controller('CollectionSlideShowController', function($scope, $http, AppSettings, ClassyUtilities, Localizer, $routeParams, $timeout, $location) {

    ClassyUtilities.Screen.ZoomableViewport();

    AppSettings.then(function(appSettings) {

        $scope.ScreenHeight = ClassyUtilities.Screen.GetHeight();
        $scope.ScreenWidth = ClassyUtilities.Screen.GetWidth();

        $http.get(appSettings.ApiUrl + '/collection/' + $routeParams.collectionId + '?includeListings=true&increaseViewCounter=true&includeProfile=true', config).success(function(data) {

            var listings = [];
            data.Listings.forEach(function(listing) {
                listings.push({
                    Id: listing.Id,
                    Title: listing.Title,
                    Description: listing.Content,
                    ViewCount: listing.ViewCount,
                    FavoriteCount: listing.FavoriteCount,
                    CommentCount: listing.CommentCount,
                    ImageUrl: ClassyUtilities.Images.Thumbnail(appSettings, listing.ExternalMedia[0].Key, $scope.ScreenWidth),
                    CopyrightMessage: (listing.Metadata.IsWebPhoto && listing.Metadata.IsWebPhoto == "True") ?
                        extractHostFromUrl(listing.Metadata.CopyrightMessage) :
                        listing.Metadata.CopyrightMessage ? listing.Metadata.CopyrightMessage : getProfileName(listing.Profile)
                });
            });
            $scope.Listings = listings;

            $timeout(loadImages);

            $scope.loadComments($routeParams.photoId);

        }).error(function() {
            // TODO: display some error message
        });

        Localizer.Get('Mobile_CollectionSlideShow_ReadMore', AppSettings.Culture, function(resource) { $scope.Resources.ReadMore = resource; });

        function extractHostFromUrl(url) {
            var a = window.createElement('a');
            a.href = url;
            return a.hostname;
        }

        function getProfileName(profile) {
            if (!profile || profile == '') return '';
            if (!profile.ContactInfo && !profile.IsProfessional) return 'unknown';
            var name;
            if (profile.IsProxy) return profile.ProfessionalInfo.CompanyName;
            else if (profile.IsProfessional) name = profile.ProfessionalInfo.CompanyName;
            else name = (!profile.ContactInfo.Name || profile.ContactInfo.Name == '') ? profile.UserName : profile.ContactInfo.Name;
            if (name) return name;
            return 'unknown';
        }

        $scope.share = function(network) {
            var selectedListing = $('.slideshow .listing.selected').data('listing-id');
            var url = window.location.protocol + appSettings.Host + '/photo/' + selectedListing + '--show';
            Classy.Share(network, url);
        };

        $scope.loadComments = function(listingId) {
            $http.get(appSettings.ApiUrl + '/listing/' + listingId + '?includeComments=true&includeCommenterProfiles=true', config).success(function (data) {
                var comments = [];
                data.Comments.forEach(function(comment) {
                    comments.push({
                        ProfileId: comment.ProfileId,
                        Content: comment.Content,
                        ProfileName: comment.Profile.UserName
                    });
                });
                $scope.Comments = comments;
            });
        };
        $scope.closeComments = function () {
            $('.comments-container').css('opacity', '0').css('display', 'none');
        };
        $scope.showComments = function() {
            $('.comments-container').css('display', 'inline-block').css('opacity', '1');
        };
    });

    function loadImages() {
        var selectedImage = $('.slideshow .selected .photo');
        var details = $('.slideshow .photo-details');

        var description = details.find('.description');
        details.find('.title').html(selectedImage.data('title'));
        description.html(selectedImage.data('description'));
        details.find('.photo-stats .views .value').html(selectedImage.data('view-count'));
        details.find('.photo-stats .favorites .value').html(selectedImage.data('favorite-count'));
        details.find('.photo-stats .comments .value').html(selectedImage.data('comment-count'));

        // check for description overflow
        if (description[0].scrollHeight > description[0].clientHeight)
            details.find('.read-more').css('display', 'inline-block');
        else
            details.find('.read-more').css('display', 'none');
        details.find('.read-more').click(function() {
            description.css('height', description[0].scrollHeight);
            details.find('.read-more').html('Hide');
            description.one('click', function() {
                description.css('height', '30px');
                details.find('.read-more').html('Read more');
            });
        });

        $scope.loadComments(selectedImage.data('listing-id'));

        if (selectedImage.hasClass('loaded')) return;
        selectedImage.css('width', '100%');
        selectedImage
            .attr('src', selectedImage.data('orig-src'))
            .load(function () {
                $(this).addClass('loaded');

                // preload 2 more photos
                var nextImage = $(this).parents('.listing').next().find('.photo');
                if (nextImage.length == 0) return;
                nextImage.attr('src', nextImage.data('orig-src')).load(function() {
                    $(this).addClass('loaded');
                    var secondImage = $(this).parents('.listing').next().find('.photo');
                    if (secondImage.length == 0) return;
                    secondImage.attr('src', secondImage.data('orig-src')).load(function() {
                        $(this).addClass('loaded');
                    });
                });
        });
    }

    $scope.nextSlide = function () {
        var selectedImage = $('.slideshow .selected');
        var nextImage = selectedImage.next();
        if (!nextImage.hasClass('listing')) return;

        selectedImage.addClass('hidden');
        selectedImage.removeClass('selected');

        nextImage.removeClass('hidden');
        nextImage.addClass('selected');

        loadImages();
    };
    $scope.prevSlide = function () {
        var selectedImage = $('.slideshow .selected');
        var prevImage = selectedImage.prev();
        if (!prevImage.hasClass('listing')) return;

        selectedImage.addClass('hidden');
        selectedImage.removeClass('selected');

        prevImage.removeClass('hidden');
        prevImage.addClass('selected');

        loadImages();
    };

    $scope.backToCollection = function() {
        $location.url('/Collection/' + $routeParams.collectionId);
    };

    $scope.openShareMenu = function() {
        $('.slideshow .share-menu').css('opacity', '1');
    };
    $scope.closeShareMenu = function() {
        $('.slideshow .share-menu').css('opacity', '0');
    };

});