classy.directive('classyHtml', function ($http) {
    return {
        restrict: 'E',
        templateUrl: 'Home/classy-html.html',
        scope: {
            htmlContent: '@htmlContent'
        },
        link: function (scope) {
            var html = scope.htmlContent;
            $('.html-block').html(html);
        }
    };
});