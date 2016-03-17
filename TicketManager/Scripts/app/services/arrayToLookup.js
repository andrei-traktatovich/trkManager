(() => {

    angular.module("services")
    .factory("arrayToLookup", arrayToLookup);

    function arrayToLookup(_) {

        return (input) => {
            if (Array.isArray(input)) {
                extendWithLookup(input);
            } else {
                for (var propname in input) {
                    if (input.hasOwnProperty(propname) && Array.isArray(input[propname])) {
                        extendWithLookup(input[propname]);
                        
                    }
                }
            }
        }
    }

    function extendWithLookup(array) {
        array.lookup = lookup.bind(array);
        array.nameById = nameById.bind(array);
        array.idByName = idByName;
        return array;
    }

    function lookup(key, display, value) {
        return _.find(this, (item) => item[key] === value);
    }

    function nameById(id) {
        var item = this.lookup("id", "name", id);
        return item ? item.name : null;
    }
    function idByName(name) {
        var item = this.lookup("name", "id", name);
        return item ? item.id : null;
    }

})();