(() => {
    "use strict";

    angular.module("app")
    .controller("searchFormCtrl", searchFormCtrl);

    function searchFormCtrl($scope, globals, reportHttpError) {

        initData();

        $scope.mayBeClearOfficeSelection = mayBeClearOfficeSelection;
        $scope.doSearch = doSearch;

        // TODO: this will actually belong to the employees-list controller!!!
        $scope.toggleSelectEmployee = toggleSelectEmployee;
        $scope.selectAll = selectAll;
        $scope.unselectAll = unselectAll;
        $scope.confirmAll = confirmAll;

        function mayBeClearOfficeSelection() {
            if ($scope.search.text.trim()) {
                $scope.search.officeId = -1;
                $scope.officeSelectionDisabled = true;
            }
        }

        function initData() {

            $scope.offices = globals.get("offices");

            $scope.officeSelectionEnabled = true;

            $scope.search = {
                officeId: profile.officeId || -1,
                text: "",
                reserve: false,
                blackList: false
            };
        }

        function doSearch(search) {
            employees.find(search)
                .then(showEmployeeList)
                .catch(reportHttpError("Не удается отобразить список сотрудников"));
        }

        function showEmployeeList(employees) {
            $scope.employees = employees;
        }
    }

})();