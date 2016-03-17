(() => {
    angular.module("config", [])
        .value("config", {
            DEBUG: true,
            urls: {
                // put all app urls here !!! 
                APPGLOBALS: "/api/globals"
            }
        });
})();