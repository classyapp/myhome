
var profilePage = angular.module('profilePage', ['AppManagerService', 'ClassyUtilitiesService']);

//profilePage.value('appSettingsPromise', 'http://www.thisisclassy.com:8008'); // way to inject objects into module controllers

var config = {
    headers: {
        'X-Classy-Env': '{ "AppId": "v1.0" }',
        'Content-Type': 'application/json',
        'Accept': 'application/json'
    }
};

profilePage.controller('ProfileController', function ($scope, $http, AppSettings, ClassyUtilities) {
    AppSettings.then(function () {

        var appSettings = Classy.CacheProvider.Get("__AppSettings__");
        var utilities = ClassyUtilities;

        $http.get(appSettings.ApiUrl + '/profile/246?includeCollections=true', config).success(function (data) {
            $scope.profileDetails = data;
            
            // organize collections
            var collectionImages = [];
            $scope.profileDetails.Collections.forEach(function (collection) {
                if (collection.CoverPhotos && collection.CoverPhotos.length > 0 && collection.CoverPhotos[0].trim() != '')
                    collectionImages.push({ Image: utilities.Images.Thumbnail(appSettings, collection.CoverPhotos[0], 200, 200) });
            });
            $scope.Collections = collectionImages;

            $scope.Location = getProfileLocation(data);

        }).error(function () {
            // TODO: display some error message
        });

        function getProfileLocation(profileDetails) {
            var professionalInfo = profileDetails.professionalInfo;
            if (!professionalInfo) return '';
            var contactInfo = professionalInfo.CompanyContactInfo;
            if (!contactInfo) return '';
            var location = contactInfo.Location;
            if (!location) return '';
            var address = location.Address;
            if (!address) return '';
            return address.City + ', ' + address.Country;
        }

    });
});