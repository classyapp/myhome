classy.directive('classyScrollable', function (ClassyUtilities) {
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
                requestAnimationFrame(function () {
                    elem.css('transition', '-webkit-transform 0.5s ease');
                    elem.css('-webkit-transform', 'translate3d(0,0,0)');
                });
            } else if (Math.abs(currentOffset) >= maxOffset) {
                requestAnimationFrame(function () {
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