classy.directive('classyCollection', function ($http, AppSettings, ClassyUtilities) {

    var getCopyrightMessage = function (listing) {
        if (listing.Metadata.IsWebPhoto && listing.Metadata.IsWebPhoto == 'True')
            return listing.Metadata.CopyrightMessage.extractHost();
        if (listing.Metadata.CopyrightMessage != '')
            return listing.Metadata.CopyrightMessage;
        return getProfileName(listing.Profile);
    };

    var getProfileName = function (profile) {
        if (!profile) return 'unknown';
        if (!profile.IsProfessional && !profile.ContactInfo) return 'unknown';
        var name;
        if (profile.IsProxy) name = profile.ProfessionalInfo.CompanyName;
        else if (profile.IsProfessional) name = profile.ProfessionalInfo.CompanyName;
        else name = profile.ContactInfo.Name ? profile.ContactInfo.Name : profile.UserName;
        if (name) return name;
        return 'unknown';
    };

    return {
        restrict: 'E',
        templateUrl: 'Home/classy-collection.html',
        link: function (scope) {
            var w = ClassyUtilities.Screen.GetWidth();
            AppSettings.then(function(appSettings) {
                $http.get(appSettings.ApiUrl + '/collection/' + scope.collectionId + '?includeListings=true', config).success(function (data) {

                    var imageWidth = Math.floor((w - 20) / 3);
                    
                    var listings = [];
                    data.Listings.forEach(function (listing) {
                        var l = {
                            Id: listing.Id,
                            Title: listing.Title,
                            ImageUrl: ClassyUtilities.Images.Thumbnail(appSettings, listing.ExternalMedia[0].Key, imageWidth, imageWidth),
                            ArticleImageUrl: ClassyUtilities.Images.Thumbnail(appSettings, listing.ExternalMedia[0].Key, w),
                            CopyrightMessage: getCopyrightMessage(listing)
                        };
                        data.IncludedListings.forEach(function (includedListing) {
                            if (includedListing.Id != listing.Id) return;
                            l.Comments = includedListing.Comments;
                        });
                        listings.push(l);
                    });
                    scope.Listings = listings;

                });
            });
        }
    };
});