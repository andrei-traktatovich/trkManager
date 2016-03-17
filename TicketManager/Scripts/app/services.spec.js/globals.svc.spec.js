/// <reference path="../../jasmine/jasmine-test.js" />
/// <reference path="../../lodash.min.js" />

/// <reference path="../third-party/third-party.module.js" />
/// <reference path="../third-party/lodash.js" />


/// <reference path="../infrastructure/constants/constants.module.js" />
/// <reference path="../infrastructure/config/config.module.js" />
/// <reference path="../infrastructure/log/logging.module.js" />
/// <reference path="../infrastructure/log/console-logger.js" />

/// <reference path="../infrastructure/error-reporting/error-reporting.module.js" />
/// <reference path="../infrastructure/error-reporting/app-error.js" />
/// <reference path="../infrastructure/error-reporting/reportError.js" />
/// <reference path="../infrastructure/error-reporting/reportHttpError.js" />

/// <reference path="../infrastructure/infrastructure.module.js" />

/// <reference path="../services/services.module.js" />

/// <reference path="../services/arrayToLookup.js" />

(() => {
    "use strict";

    angular.module("services")
        .factory("globals", globals);

    function globals($http, reportHttpError, arrayToLookup, config) {

        const globalsUrl = config.urls.APPGLOBALS;
        
        
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
    var httpHandler;
    beforeEach(inject(($httpBackend) => {
        httpHandler = $httpBackend.whenGET(globalsUrl).respond(200, { profile: {}, lookups: { "smth": [] } }, {});
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

        it("reports error if request fails", (done) => {
            inject((globals, $rootScope, constants) => {

                var spy = jasmine.createSpy("root scope handler");
                
                $rootScope.$on(constants.appEvents.APPERROREVENT, spy);

                httpHandler.respond(404, "something went wrong");

                httpBackend.expectGET(globalsUrl);

                globals.load().finally(() => {
                    expect(spy).toHaveBeenCalled();
                    done();
                });

                httpBackend.flush();
            });
        });

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