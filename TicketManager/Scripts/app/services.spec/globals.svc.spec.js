/// <reference path="../../jasmine/jasmine.js" />
/// <reference path="../../angular.min.js" />


/// <reference path="../blocks/error-handling/error-handling.module.js" />
/// <reference path="../blocks/error-handling/reportHTTPError.js" />
/// <reference path="../third-party/third-party.module.js" />

/// <reference path="../services/services.module.js" />
/// <reference path="../services/globals.svc.js" />

describe("globals", () => {
    var $httpBackend, globalsObject;

    var testData = {
        profile: {},
        lookups: {
            "testList": [
                { id: 0, name: "object0" },
                { id: 1, name: "object1" },
                { id: 2, name: "object2" }
            ]
        }
    };

    beforeEach(() => {
        module("services");
    });

    beforeEach((done) => {
        inject((globals, $injector) => {

            $httpBackend = $injector.get("$httpBackend");
    
            globalsObject = $httpBackend.when("GET", "/api/globals")
                                        .respond(testData);

            globals.load().then(done);

        });
    });

    it("loads global data context for the application", () => {
        $httpBackend.expectGET('/api/globals');

    });

    it("returns data list by item name", () => {
        // do smth
    });

    it("returns config", () => {
        // do smth
    });

});
