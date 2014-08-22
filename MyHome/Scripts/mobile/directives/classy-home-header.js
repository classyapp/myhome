classy.directive('classyHomeHeader', function($location, $http, $timeout, AppSettings, AuthProvider, ClassyUtilities, Localizer) {
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
                scope.signOut = function() {
                    window.location.href = '/logout';
                };

                scope.openPresenceMenu = function () {
                    var presenceMenu = $('.home-header .presence-menu');
                    presenceMenu.removeClass('hidden');
                    $timeout(function() {
                        $('body').one('click', function() { presenceMenu.addClass('hidden'); });
                    }, 500);

                };

                var searchInput = $('#header-search');
                scope.displaySearch = function() {
                    $('#header-search').removeClass('hidden').focus().blur(scope.hideSearch);
                };
                scope.hideSearch = function() {
                    $('#header-search').addClass('hidden');
                };

                scope.Resources = {};
                Localizer.Get('Mobile_Header_ProfileLink', appSettings.Culture).then(function (resource) { scope.Resources.ProfileLink = resource; });
                Localizer.Get('Mobile_Header_SignOutLink', appSettings.Culture).then(function (resource) { scope.Resources.SignOutLink = resource; });
                Localizer.Get('Mobile_Header_LoginLink', appSettings.Culture).then(function (resource) { scope.Resources.LoginLink = resource; });
                Localizer.Get('Mobile_Header_SearchPlaceholder', appSettings.Culture).then(function (resource) { scope.Resources.SearchPlaceholder = resource; });
                Localizer.Get('Mobile_General_Loading', appSettings.Culture).then(function (resource) { scope.Resources.Loading = resource; });

                scope.LoadingPosition = (ClassyUtilities.Screen.GetWidth() / 2) - 97;

                $('#header-search').keyup(function (e) {
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
                    var section = $(this).closest('.suggestion-section').find('.title').data('section-key');
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