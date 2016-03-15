/// <reference path="../../jasmine/jasmine-test.js" />
/// <reference path="../../lodash.min.js" />

/// <reference path="../third-party/third-party.module.js" />
/// <reference path="../services/services.module.js" />
/// <reference path="../services/arrayToLookup.js" />

(() => {
    "use strict";

    angular.module("services")
        .factory("reportHttpError", function() { return () => {}; })
        .factory("globals", globals)
        .factory("toLookupList", () => { });
        
     

    function globals($http, reportHttpError, arrayToLookup) {
        
        const globalsUrl = "/api/globals";
        
        
        return {
            load() {
                return $http.get(globalsUrl).then((data) => {
                    
                    this.profile = data.data.profile;
                    this.lookups = data.data.lookups;
                    arrayToLookup(this.lookups);

                }, reportHttpError("Невозможно загрузить глобальные данные"));
            },
            get(name) {
                return this.lookups[name];
            }
        };
    }

})();

describe("globals", () => {
    let globalsUrl = "/api/globals";

    let httpBackend;
    beforeEach(() => {
        module("services");
    });

    beforeEach(inject(($httpBackend) => {
        $httpBackend.whenGET(globalsUrl).respond(200, { profile: {}, lookups: { "smth": [] } }, {});
        httpBackend = $httpBackend;
    }));

    describe("when loading globals object", () => {
        it("loads a globals object", (done) => {
            inject((globals) => {
                httpBackend.expectGET(globalsUrl);
                globals.load().then(() => {
                    expect(globals.profile).toBeDefined();
                    expect(globals.lookups).toBeDefined();
                    done();
                });

                httpBackend.flush();
            });
        });

        xit("reports error if request fails");

    });

    describe("when working with lookups", () => {
        
        beforeEach((proceed) => {
            inject((globals) => {
                globals.load().then(proceed).catch(proceed);
                httpBackend.flush();
            });
        });

        it("returns a lookup by name", () => {
            inject((globals) => {
                console.log(globals);
                expect(globals.get("smth")).toBeDefined();
            });
        });
            
    });
});