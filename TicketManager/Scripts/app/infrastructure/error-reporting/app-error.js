(() => {

    angular.module("error-reporting")
        .factory("appError", appError);

    function appError() {
        return (title, text, error, type) => {

            return {
                title,
                text,
                error,
                type
            };

        };
    }

})();