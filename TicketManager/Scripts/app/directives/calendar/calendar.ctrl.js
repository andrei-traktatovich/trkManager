(function () {

    "use strict";

    angular.module("calendar")
      .controller("calendarCtrl", ctrl);


    var months = ["Янв", "Фев", "мар", "апр", "май", "июн", "июл", "авг", "сен", "окт", "ноя", "дек"];

    function ctrl($scope) {
        $scope.weeks = [];
        $scope.selection = [];
        $scope.toggleSelected = function (day) {
            function toggleMultiSelect(e) {
                if (e.shiftKey)
                    $scope.multiselect = !scope.multiselect;
            }
            //day.isSelected = !day.isSelected;
        }

        $scope.monthName = function (month) {
            return months[month];
        }
        $scope.beginSelect = function ($event, day) {
            if ($scope.selection.length > 1 && !$event.shiftKey) {
                clearSelection();
                return;
            }

            if ($event.shiftKey) {
                $scope.multiselect = true;
            }

            if (!$scope.multiselect || !$scope.selecting)
                clearSelection();

            day.isSelected = !day.isSelected;
            $scope.selection.push(day);
            $scope.selecting = true;
        }

        function clearSelection() {
            $scope.selection.forEach(function (day) {
                day.isSelected = false;
            });

            $scope.selection = [];
        }
        $scope.continueSelect = function (day) {
            if ($scope.selecting) {
                day.isSelected = true;
                $scope.selection.push(day);
            }
        }

        $scope.endSelect = function (day) {
            $scope.selecting = false;
        }
        var currentDate = moment(new Date($scope.data.year, $scope.data.month, 1));


        $scope.prevYear = function () {
            $scope.data.year--;
            feedData();
        }

        $scope.prevMonth = function () {
            if ($scope.data.month === 0) {
                $scope.data.month = 11;
                $scope.prevYear();
            } else {
                $scope.data.month--;
                feedData();
            }

        }

        $scope.nextYear = function () {
            $scope.data.year++;
            feedData();
        }

        $scope.nextMonth = function () {
            if ($scope.data.month === 11) {
                $scope.data.month = 0;
                $scope.nextYear();
            } else {
                $scope.data.month++;
                feedData();
            }
        }

        $scope.now = function () {
            var now = moment();

            $scope.data.month = now.month();
            $scope.data.year = now.year();
            feedData();
        }

        feedData();

        function feedData() {
            clearSelection();
            $scope.calendarWantsData($scope.data.month, $scope.data.year)
              .then((data) => {
                  $scope.data.days = data;
                  $scope.weeks = buildMonth($scope.data);
              });
        }


    }

    function buildMonth(data) {
        var result = [];

        var currentDate = moment(new Date(data.year, data.month, 1));
        var currentMonth = currentDate.month();
        var done = false,
          counter = 0;


        while (!done) {
            var delta = _buildWeek(currentDate);
            currentDate = currentDate.add(delta, "d");
            done = currentDate.month() != currentMonth;
        }

        return result;

        // maybe decorate moment's function?
        function russifyWeekDay(day) {
            if (day == 0)
                return 6;
            return day - 1;
        }

        function _buildWeek(date) {
            date = date.clone();
            var firstDay = russifyWeekDay(date.weekday());
            var week = {
                firstDay: firstDay,
                days: []
            };

            var delta = 7 - firstDay;
            for (var i = 0; i < delta; i++) {
                if (date.month() == currentMonth)
                    week.days.push({
                        date: date,
                        modified: Math.random() > .5,
                        isHoliday: [5, 6].indexOf(russifyWeekDay(date.weekday())) > -1,
                        data: data.days[counter++]
                    });
                date = date.clone().add(1, "d");
            }
            result.push(week);

            return delta;
        }
    }

})();