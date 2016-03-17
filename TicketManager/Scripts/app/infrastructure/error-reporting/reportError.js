(() => {
    "use strict";

    angular.module("error-reporting")
        .factory("reportError", reportError);

    function reportError($rootScope, log, constants, config) {

        return (error) => {
            
            $rootScope.$emit(constants.appEvents.APPERROREVENT, error);

            if (config.DEBUG)
                log.error(error);
        }
    }

})();