
var classy = angular.module('classy-mobile-app', ['ngRoute', 'ngSanitize', 'ngAnimate', 'ngTouch', 'AppManagerService', 'ClassyUtilitiesService', 'LocalizerService']);

classy.directive('classyScrollable', function(ClassyUtilities) {
    var offset = 0;

    function handleDrag(ev) {
        if (ev.type == 'dragstart') {
            $(ev.currentTarget).css('transition', 'none');
            var transform = window.getComputedStyle(ev.currentTarget).webkitTransform;
            offset = !transform || transform == 'none' ? 0 : parseInt(transform.split(',')[4]);
            return;
        }
        if (ev.type == 'release') {
            var elem = $(ev.currentTarget);
            var transform = window.getComputedStyle(ev.currentTarget).webkitTransform;
            var currentOffset = !transform || transform == 'none' ? 0 : parseInt(transform.split(',')[4]);
            var maxOffset = ev.currentTarget.scrollWidth - ClassyUtilities.Screen.GetWidth();
            if (currentOffset >= 0) {
                requestAnimationFrame(function() {
                    elem.css('transition', '-webkit-transform 0.5s ease');
                    elem.css('-webkit-transform', 'translate3d(0,0,0)');
                });
            } else if (Math.abs(currentOffset) >= maxOffset) {
                requestAnimationFrame(function() {
                    elem.css('transition', '-webkit-transform 0.5s ease');
                    elem.css('-webkit-transform', 'translate3d(-' + maxOffset + 'px,0,0)');
                });
            }
            return;
        }
        
        ev.gesture.preventDefault();
        
        var drag = ev.gesture.deltaX + offset;

        $(ev.currentTarget).css('-webkit-transform', 'translate3d(' + drag + 'px, 0, 0)');
    }

    return function (scope, element) {
        Hammer(element[0], { dragLockToAxis: true })
            .on("dragstart release dragleft dragright swipeleft swiperight", handleDrag);
    };
});

classy.directive('classyHeader', function(ClassyUtilities, AppSettings, $location, $http) {
    return {
        restrict: 'E',
        templateUrl: 'classy-header.html',
        transclude: true,
        link: function(scope, element, attrs) {
            var w = ClassyUtilities.Screen.GetWidth();
            var searchInput = $('#header-search');
            searchInput.css('width', parseInt(w - 70 - 70 - 10).toString() + 'px');
            scope.openSearch = function() {
                $('.main-navigation').addClass('visible');
                searchInput.focus();
            };

            $('body').delegate('.auto-suggestions .suggestion', 'click', function () {
                var query = $(this).find('.content').data('suggestion-value');
                var section = $(this).closest('.suggestion-section').find('.title').html();
                switch (section)
                {
                    case 'Rooms':
                        window.location.href = '//' + $location.host() + ':' + $location.port() + window.location.pathname + '#/Search?room=' + query;
                        break;
                    case 'Styles':
                        window.location.href = '//' + $location.host() + ':' + $location.port() + window.location.pathname + '#/Search?style=' + query;
                        break;
                    case 'Professionals':
                        window.location.href = '//' + $location.host() + ':' + $location.port() + window.location.pathname + '#/Profile/' + query;
                        break;
                }
            });

            searchInput.keyup(function (e) {
                var query = $(this).val().trim();
                if (e.keyCode == 13 && query != '') {
                    window.location.href = '//' + $location.host() + ':' + $location.port() + '/' + $location.path() + '#/Search?q=' + $(this).val().trim();
                }
                AppSettings.then(function(appSettings) {
                    if (query.length >= 2) {
                        $http.get(appSettings.Host + '/mobile/search/suggest?q=' + query, config).success(function(data) {
                            scope.AutoSuggestions = data;
                        });
                    }
                });
            });
        }
    };
});

classy.factory('CacheProvider', function ($cacheFactory) {
    // we can add a cache limit here if we'll need to
    return $cacheFactory('HomeLab_Mobile_Cache');
});

classy.config([
    '$routeProvider', function($routeProvider) {
        $routeProvider
            .when('/Profile/:profileId', {
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
