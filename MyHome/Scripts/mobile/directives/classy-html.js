classy.directive('classyHtml', function ($http) {
    return {
        restrict: 'E',
        templateUrl: 'Home/classy-html.html',
        scope: {
            htmlId: '@htmlId'
        },
        link: function (scope, elem) {
            // get the html from the controller and insert into element
            // elem.html();
        }
    };
});