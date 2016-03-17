(() => {
    "use strict";

    angular.module("app", [DEPS]) // TODO: specify dependencies !!! 

        .factory("globalErrorHandler", globalErrorHandler)
        .factory("notifyAppError", notifyAppError)
        .factory("bootstrapApp", bootstrapApp)
        .config()
        .run(run);

    function run(globalErrorHandler, bootstrapApp, $rootScope, notifyError) {

        globalErrorHandler();

        bootstrapApp().then(() => {
            $rootScope.appStateValid = true;
        }, (error) => {
            $rootScope.appStateValid = false;
            notifyAppError(error);
        });
    }

    function bootstrapApp($q, globals) {

        return $q.all([
            globals.load()
            // do all that is needed to bootstrap the application
        ]);
    }

    function notifyAppError(toastr, logger)
    {
        toastr.error(error);
        logger.error(error);
    }

    function globalErrorHandler($rootScope, notifyError, constants) {

        $rootScope.$on(constants.appEvents.APPERROREVENT, (event, error) => {
            notifyError(error);
        });

    }
})();