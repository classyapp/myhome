
var classyUtilitiesService = angular.module('ClassyUtilitiesService', []);

classyUtilitiesService.factory('ClassyUtilities', [function() {
    return {
        Images: {
            Thumbnail: function(appSettings, imageKey, width, height) {
                var cdnUrl = appSettings.CdnUrl;
                return cdnUrl + '/thumbnail/' + imageKey + '?Width=' + width + '&Height=' + height + '&format=json';
            },
            Thumbnails: function (appSettings, imageKeys, width, height) {
                // TODO: take width/height into consideration!
                var sizes = [
                    [{ x: 270, y: 220 }],
                    [{ x: 135, y: 220 }, { x: 135, y: 220 }],
                    [{ x: 200, y: 220 }, { x: 70, y: 110 }, { x: 70, y: 110 }],
                    [{ x: 200, y: 220 }, { x: 70, y: 70 }, { x: 70, y: 70 }, { x: 70, y: 70 }]
                ];
                var count = imageKeys.length;
                var container = "<div class=\"thumbs" + count + "\">";
                for (var i = 0; i < count; i++) {
                    var imageUrl = appSettings.CdnUrl + "/thumbnail/" + imageKeys[i] + "?Width=" + sizes[count-1][i].x + "&Height=" + sizes[count-1][i].y + "&format=json";
                    container += "<div class=\"thumb\" style=\"background-image:url(" + imageUrl + ");\"></div>";
                }
                container += "</div>";
                return container;
            }
        }
    };
}]);
