(() => {
    "use strict";

    angular.module("error-reporting")
        .factory("reportHttpError", reportHttpError);

    function reportHttpError(reportError, appError, constants) {

        return (msg) => ((err) => {
            const error = appError(msg, makeErrorText(err), err, constants.appErrorTypes.HTTP);
            reportError(error);
        });
    }

    function makeErrorText(httpErr) {
        return `${httpErr.status}: ${httpErr.data.error}`;
    }

})();