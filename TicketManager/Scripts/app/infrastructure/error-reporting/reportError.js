(() => {
    "use strict";

    angular.module("error-reporting")
        .factory("reportError", reportError);

    // a more elegant solution to curry function? 
    var emitError = $rootScope.emit.bind(null, constants.appEvents.APPERROREVENT);

    function reportError($rootScope, log, constants, config) {

        return (error) => {
            if (config.DEBUG)
                log.error(error);

            emitError(error);
        }
    }

})();