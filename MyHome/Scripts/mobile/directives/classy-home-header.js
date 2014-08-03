classy.directive('classyHomeHeader', function($location, $http, AppSettings, AuthProvider, ClassyUtilities) {
    return {
        restrict: 'E',
        templateUrl: 'Home/classy-header.html',
        transclude: true,
        link: function(scope) {
            AppSettings.then(function(appSettings) {
                AuthProvider.getUser().then(function(data) {
                    scope.IsAuthenticated = data.IsAuthenticated;
                    scope.User = data.Profile;
                    scope.User.Avatar.ImageUrl = ClassyUtilities.Images.Thumbnail(appSettings, data.Profile.Avatar.Key, 45, 45);
                });

                scope.homePage = function() {
                    $location.url('/');
                };
                scope.loginPage = function() {
                    $location.url('/Login');
                };
                scope.profilePage = function(userId) {
                    $location.url('/Profile/' + userId);
                };

                var searchInput = $('#header-search');
                scope.displaySearch = function() {
                    searchInput.removeClass('hidden').focus().blur(scope.hideSearch);
                };
                scope.hideSearch = function() {
                    searchInput.addClass('hidden');
                };

                searchInput.keyup(function (e) {
                    var query = $(this).val().trim();
                    if (e.keyCode == 13 && query != '') {
                        window.location.href = '//' + $location.host() + ':' + $location.port() + '/' + $location.path() + '#/Search?q=' + $(this).val().trim();
                    }
                    if (query.length >= 2) {
                        $http.get(window.location.protocol + appSettings.Host + '/mobile/search/suggest?q=' + query, config).success(function (data) {
                            scope.AutoSuggestions = data;
                            scope.$apply();
                        });
                    } else {
                        scope.AutoSuggestions = [];
                        scope.$apply();
                    }
                });

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
                        case 'Product Categories':
                            window.location.href = '//' + $location.host() + ':' + $location.port() + window.location.pathname + '#/Search?category=' + query;
                    }
                });
            });
        }
    };
});