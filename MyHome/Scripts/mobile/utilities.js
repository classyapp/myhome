
var classyUtilitiesService = angular.module('ClassyUtilitiesService', []);

classyUtilitiesService.factory('ClassyUtilities', [function() {
    return {
        Screen: {
            StaticViewport: function() {
                document.getElementById('viewport').setAttribute('content', 'width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no');
            },
            ZoomableViewport: function() {
                document.getElementById('viewport').setAttribute('content', 'width=device-width, initial-scale=1, maximum-scale=3');
            },
            GetWidth: function() {
                return Math.max(document.documentElement.clientWidth, window.innerWidth || 0);
            },
            GetHeight: function() {
                return Math.max(document.documentElement.clientHeight, window.innerHeight || 0);
            }
        },
        Images: {
            Thumbnail: function(appSettings, imageKey, width, height) {
                var cdnUrl = appSettings.CdnUrl;
                var imageUrl = cdnUrl + '/thumbnail/' + imageKey + '?Width=' + width;
                if (height) imageUrl += '&Height=' + height;
                imageUrl += '&format=json';
                return imageUrl;
            },
            Thumbnails: function (appSettings, imageKeys, collectionId, width, height) {
                // TODO: take width/height into consideration!
                var sizes = [
                    [{ x: 270, y: 220 }],
                    [{ x: 135, y: 220 }, { x: 135, y: 220 }],
                    [{ x: 200, y: 220 }, { x: 70, y: 110 }, { x: 70, y: 110 }],
                    [{ x: 200, y: 220 }, { x: 70, y: 70 }, { x: 70, y: 70 }, { x: 70, y: 70 }]
                ];
                var count = imageKeys.length;
                var container = "<div class=\"thumbs" + count + "\" data-collection-id=\"" + collectionId + "\">";
                for (var i = 0; i < count; i++) {
                    var imageUrl = appSettings.CdnUrl + "/thumbnail/" + imageKeys[i] + "?Width=" + sizes[count-1][i].x + "&Height=" + sizes[count-1][i].y + "&format=json";
                    container += "<div class=\"thumb\" style=\"background-image:url(" + imageUrl + ");\"></div>";
                }
                container += "</div>";
                return container;
            }
        },
        Listing: {
            GetCopyrightMessage: function (listing) {
                if (listing.Metadata.IsWebPhoto && listing.Metadata.IsWebPhoto == 'True')
                    return listing.Metadata.CopyrightMessage.extractHost();
                if (listing.Metadata.CopyrightMessage && listing.Metadata.CopyrightMessage != '')
                    return listing.Metadata.CopyrightMessage;
                return this.GetProfileName(listing.Profile);
            },
            GetProfileName: function (profile) {
                if (!profile) return 'unknown';
                if (!profile.IsProfessional && !profile.ContactInfo) return 'unknown';
                var name;
                if (profile.IsProxy) name = profile.ProfessionalInfo.CompanyName;
                else if (profile.IsProfessional) name = profile.ProfessionalInfo.CompanyName;
                else name = profile.ContactInfo.Name ? profile.ContactInfo.Name : profile.UserName;
                if (name) return name;
                return 'unknown';
            }
        }
    };
}]);
