classy.directive('classyHtml', function ($http) {
    return {
        restrict: 'E',
        templateUrl: 'Home/classy-html.html',
        scope: {
            htmlId: '=htmlId'
        },
        link: function (scope, elem) {
            // get the 
            elem.find('.content').html(elem.find('.custom-html').val());
        }
    };
});