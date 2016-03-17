(() => {
    "use strict";

    var owner = angular.module("glyph-buttons", []);

    createButtons([

        "fast-forward",
        "step-forward",
        "fast-backward",
        "step-backward",
        "refresh",
        "floppy-disk" // TODO: add search !!! 

    ], owner);

    function createButtons(glyphicons, ownerMod) {

        glyphicons.forEach((style) => {

            ownerMod.directive(camelCase(`btn-${style}`), function () {
                return makeButton(style);
            });
        });


        function camelCase(s) {
            // https://coderwall.com/p/iprsng/convert-snake-case-to-camelcase
            return s.replace(/(\-\w)/g, function (m) { return m[1].toUpperCase(); });
        }

        function makeButton(glyph) {
            return {
                restrict: "E",
                replace: true,
                template: `
                                <button class="btn btn-default btn-sm">
                                    <span class="glyphicon glyphicon-${glyph}"></span>
                                </button>`
            };
        }
    } // end createButtons()

})();
