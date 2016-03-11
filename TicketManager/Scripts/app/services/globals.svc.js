
(function () {
    "use strict";

    angular
      .module("services")
      .factory("globals", [ globals ]);

    function globals($http, reportHttpError) {

        var data = null;
        var globalsUrl = "/api/globals";
        
        return {
            load: load,
            get: get
        };

        function load() {
            //TODO: make this a one-off operation, i.e. load globals only once. They should not change too often.
            var deferred = $.defer();
            $http.get(globalsUrl)
                .then((result) => {
                    data = result;
                    deferred.resolve(data);
                })
                .catch((err) => {
                    reportHttpError("Невозможно загрузить глобальные данные. Работа приложения невозможна.")(err);
                    deferred.reject(err);
                });
            return deferred.promise;
        }

        function get(listName) {
            return new LookupList(data[listName]);
        }
    }
    
    class LookupList {

        constructor(arr) {
            this.data = arr;
        }
        getById(id) {
            return _.find(this.data, { id: id });
        }
        getNameById(id) {
            return getById(id).name;
        }
    }

})();
