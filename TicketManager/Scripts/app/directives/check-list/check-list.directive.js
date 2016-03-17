(() => {

    "use strict";

    angular.module("check-list")
      .directive("checkList", () => {
          return {
              scope: {
                  items: "=",
                  viewProp: "@",
                  keyProp: "@"
              },
              templateUrl: "template.html",
              require: "ngModel",
              link: linker
          }
      });

    function linker(scope, element, attrs, ngModelController) {

        scope.isItemSelected = (item) => scope.selectedItem === undefined ? false : item[scope.keyProp] === scope.selectedItem;

        scope.select = (item) => {
            if (scope.selectedItem === item[scope.keyProp])
                scope.selectedItem = undefined;
            else {
                scope.selectedItem = item[scope.keyProp];
                ngModelController.$setViewValue(item[scope.keyProp]);
            }
        }
    }

})();