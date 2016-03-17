(() => {
    angular.module("config", [])
        .value("config", {
            DEBUG: true,
            LOGLEVEL: "silly",
            urls: {
                // put all app urls here !!! 
                APPGLOBALS: "/api/globals"
            }
        });
})();