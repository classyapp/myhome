
classy.controller('CollectionSlideShowController', function ($scope, $http, AppSettings, ClassyUtilities, Localizer, $routeParams, $timeout, $location) {

    ClassyUtilities.Screen.ZoomableViewport();

    AppSettings.then(function (appSettings) {

        $scope.ScreenHeight = ClassyUtilities.Screen.GetHeight();

        $http.get(appSettings.ApiUrl + '/collection/' + $routeParams.collectionId + '?includeListings=true&increaseViewCounter=true&includeProfile=true', config).success(function (data) {

            var listings = [];
            data.Listings.forEach(function (listing) {
                listings.push({
                    Title: listing.Title,
                    Description: listing.Content,
                    ImageUrl: listing.ExternalMedia[0].Url,
                    CopyrightMessage: (listing.Metadata.IsWebPhoto && listing.Metadata.IsWebPhoto == "True") ?
                        extractHostFromUrl(listing.Metadata.CopyrightMessage) :
                        listing.Metadata.CopyrightMessage ? listing.Metadata.CopyrightMessage : getProfileName(listing.Profile)
                });
            });
            $scope.Listings = listings;

            $timeout(loadImages);

        }).error(function () {
            // TODO: display some error message
        });

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
    });

    function loadImages() {
        var selectedImage = $('.slideshow .selected .photo');
        if (selectedImage.hasClass('loaded')) return;
        var details = $('.slideshow .photo-details');
        details.find('.title').html(selectedImage.data('title'));
        details.find('.description').html(selectedImage.data('description'));
        selectedImage.css('width', '100%');
        selectedImage
            .attr('src', selectedImage.data('orig-src'))
            .load(function () {
                $(this).addClass('loaded');
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

});