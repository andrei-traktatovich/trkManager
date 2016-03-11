/// <reference path="C:\Users\nikolaev\Documents\Visual Studio 2013\Projects\ticketManager 1702\TicketManager\Scripts/jasmine/jasmine.js" />
/// <reference path="C:\Users\nikolaev\Documents\Visual Studio 2013\Projects\ticketManager 1702\TicketManager\Scripts/angular.min.js" />
/// <reference path="C:\Users\nikolaev\Documents\Visual Studio 2013\Projects\ticketManager 1702\TicketManager\Scripts/angular-mocks.js" />

/// <reference path="../../blocks/error-handling/error-handling.module.js" />
/// <reference path="../../blocks/error-handling/reportHTTPError.js" />

describe("appErrorFactory", () => {
    beforeEach(function () {
        module("error-reporting");
    });

    it("creates a correct AppErrorInfo object from http error", inject(function (appErrorFactory) {
        var httpError = {
            status: 404,
            statusText: "some text",
            data: {
                error: "Not found",
                statusCode: 404
            }
        };

        var result = appErrorFactory.fromHttpError(httpError, "some text");
        expect(result).toBeDefined();
        //expect(result.header).toEqual("some text");
        expect(result.message).toEqual("404 Not found");
    }));
});

describe("reportHTTPError", function() {
    "use strict";

    beforeEach(function() {
        module("error-reporting");
    });
    it("fires an app-error event on rootscope", inject(function(reportHttpError, $rootScope) {
        var error = {
            status: 404,
            statusText: "some text",
            data: {
                error: "Not found",
                statusCode: 404
            }
        };

        var spy = jasmine.createSpy();

        $rootScope.$on("app-error", spy);

        reportHttpError("some message")(error);
        expect(spy).toHaveBeenCalled();

    }));
});

