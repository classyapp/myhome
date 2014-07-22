classy.directive('classyHeader', function (ClassyUtilities, AppSettings, $location, $http) {
    return {
        restrict: 'E',
        templateUrl: 'classy-header.html',
        transclude: true,
        link: function (scope) {
            var w = ClassyUtilities.Screen.GetWidth();
            var searchInput = $('#header-search');
            searchInput.css('width', parseInt(w - 70 - 70 - 10).toString() + 'px');
            scope.openSearch = function () {
                $('.main-navigation').addClass('visible');
                searchInput.focus();
            };

            $('body').delegate('.auto-suggestions .suggestion', 'click', function () {
                var query = $(this).find('.content').data('suggestion-value');
                var section = $(this).closest('.suggestion-section').find('.title').html();
                switch (section) {
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
                AppSettings.then(function (appSettings) {
                    if (query.length >= 2) {
                        $http.get(window.location.protocol + appSettings.Host + '/mobile/search/suggest?q=' + query, config).success(function(data) {
                            scope.AutoSuggestions = data;
                            scope.$apply();
                        });
                    } else {
                        scope.AutoSuggestions = [];
                        scope.$apply();
                    }
                });
            });
        }
    };
});