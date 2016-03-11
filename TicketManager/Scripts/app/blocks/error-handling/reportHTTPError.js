(function () {

    "use strict";

    angular.module("blocks.error-handling")
    .factory("appErrorFactory", appErrorFactory)
    .factory("reportHttpError", reportHttpErrorFactory);

    
    function reportHttpErrorFactory($rootScope, appErrorFactory) {

        return function (errorMessage) {
            return function (err) {
                $rootScope.$emit("app-error", appErrorFactory.fromHttpError(err, errorMessage));
            };
        };
    }

    function AppErrorInfo(header, message, errorType, err) {
        this.header = header || "Ошибка";
        this.message = message || "Неизвестная ошибка";
        this.kind = errorType;
        this.err = err;
    }

    function appErrorFactory() {
        return {
            fromHttpError: function (err, errorMessage) { return new AppErrorInfo((errorMessage || "Ошибка"), err.status + " " + err.data.error, "Ошибка сети", err) }
        };
    }

})();
