
var classy = angular.module('classy-mobile-app', ['ngRoute', 'ngSanitize', 'ngAnimate', 'ngTouch', 'AppManagerService', 'ClassyUtilitiesService', 'LocalizerService']);

classy.factory('CacheProvider', function ($cacheFactory) {
    // we can add a cache limit here if we'll need to
    return $cacheFactory('HomeLab_Mobile_Cache');
});

classy.config([
    '$routeProvider', function($routeProvider) {
        $routeProvider
            .when('/', {
                templateUrl: 'classy-home.html',
                controller: 'HomeController'
            }).when('/Profile/:profileId', {
                templateUrl: 'profile-page.html',
                controller: 'ProfileController'
            }).when('/Collection/:collectionId', {
                templateUrl: 'collection.html',
                controller: 'CollectionController'
            }).when('/Collection/SlideShow/:collectionId/:photoId', {
                templateUrl: 'collection-slideshow.html',
                controller: 'CollectionSlideShowController'
            }).when('/Collection/:collectionId/article', {
                templateUrl: 'collection-article.html',
                controller: 'CollectionController'
            }).when('/Product/:productId', {
                templateUrl: 'product-page.html',
                controller: 'ProductController'
            }).when('/Search', {
                templateUrl: 'search-page.html',
                controller: 'SearchController'
            }).when('/Login', {
                templateUrl: 'login.html',
                controller: 'LoginController'
            });
    }
]);

classy.filter('unsafe', function ($sce) {
    return function (val) {
        return $sce.trustAsHtml(val);
    };
});
