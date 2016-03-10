/// <reference path="C:\Users\nikolaev\Documents\Visual Studio 2013\Projects\ticketManager 1702\TicketManager\Scripts/jasmine/jasmine.js" />
/// <reference path="C:\Users\nikolaev\Documents\Visual Studio 2013\Projects\ticketManager 1702\TicketManager\Scripts/angular.min.js" />
/// <reference path="C:\Users\nikolaev\Documents\Visual Studio 2013\Projects\ticketManager 1702\TicketManager\Scripts/angular-mocks.js" />



// function that accepts an error response and fires a rootscope error event passing data to it.
// data include 
// - header 
// - message (error code + custom message)
// - additional info (error object)
'use strict';
angular.module('error-reporting', [])
.factory('appErrorFactory', appErrorFactory)
.factory('reportHTTPError', reportHTTPErrorFactory);

function AppErrorInfo(header, message, errorType, err) {
    this.header = header || 'Ошибка';
    this.message = message || 'Неизвестная ошибка';
    this.kind = errorType;
    this.err = err;
}
function appErrorFactory($rootScope) {
    return {
        fromHttpError: function (err, errorMessage) { return new AppErrorInfo((errorMessage || "Ошибка"), err.status + " " + err.data.error, 'Ошибка сети', err) }
    };
}
function reportHTTPErrorFactory($rootScope, appErrorFactory) {
    
    return function reportHTTPError(errorMessage) {
        return function (err) {
            $rootScope.$emit('app-error', appErrorFactory.fromHttpError(err, errorMessage));
        };
    }
}
/*
describe('reportHTTPError', function()  {
    it('s', function() {
        expect(2 * 2).toEqual(4);
    })
});
*/

describe('appErrorFactory', function () {
    beforeEach(function () {
        module('error-reporting');
    });

    it('creates a correct AppErrorInfo object from http error', inject(function (appErrorFactory) {
        var httpError = {
            status: 404,
            statusText: 'some text',
            data: {
                error: 'Not found',
                statusCode: 404
            }
        };

        var result = appErrorFactory.fromHttpError(httpError, 'some text');
        expect(result).toBeDefined();
        //expect(result.header).toEqual("some text");
        expect(result.message).toEqual("404 Not found");
    }));
});

describe('reportHTTPError', function() {
    'use strict';

    beforeEach(function() {
        module('error-reporting');
    })
    
    it('fires an app-error event on rootscope', inject(function(reportHTTPError, $rootScope) {
        var error = {
            status: 404,
            statusText: 'some text',
            data: {
                error: 'Not found',
                statusCode: 404
            }
        };
        var spy = jasmine.createSpy();
        
        $rootScope.$on('app-error', spy);
        
        reportHTTPError('some message')(error);
        expect(spy).toHaveBeenCalled();
         
    }))

});

