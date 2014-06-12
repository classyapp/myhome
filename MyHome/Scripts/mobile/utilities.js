
var classyUtilitiesService = angular.module('ClassyUtilitiesService', []);

classyUtilitiesService.factory('ClassyUtilities', [function() {
    return {
        Images: {
            Thumbnail: function(appSettings, imageKey, width, height) {
                var cdnUrl = appSettings.CdnUrl;
                return cdnUrl + '/thumbnail/' + imageKey + '?Width=' + width + '&Height=' + height + '&format=json';
            }
        }
    };
}]);
