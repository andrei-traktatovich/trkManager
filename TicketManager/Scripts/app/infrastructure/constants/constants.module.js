(() => {
    angular.module("constants", [])
        .value("constants", {
            appEvents: {
                APPERROREVENT: "app-error"
            },
            appErrorTypes: {
                HTTP: "HTTP"
            }

        });

})();