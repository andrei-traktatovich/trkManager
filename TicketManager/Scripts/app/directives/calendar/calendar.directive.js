(() => {

    "use strict";

    angular.module("calendar-control")
        .directive("calendar", calendarDir);

    function calendarDir() {

        return {
            transclude: true,
            templateUrl: "calendar.template.html",
            scope: {
                data: "=",
                selection: "=",
                calendarWantsData: "="
            },
            controller: "calendarCtrl"
        };
    }

})();
