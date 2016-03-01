/// <reference path="C:\Users\nikolaev\Documents\Visual Studio 2013\Projects\ticketManager 1702\TicketManager\Scripts/jasmine/jasmine.js" />
/// <reference path="C:\Users\nikolaev\Documents\Visual Studio 2013\Projects\ticketManager 1702\TicketManager\Scripts/angular.min.js" />
/// <reference path="C:\Users\nikolaev\Documents\Visual Studio 2013\Projects\ticketManager 1702\TicketManager\Scripts/angular-mocks.js" />

'use strict';

var globalsUrl = '/api/employeeCalendar/globals';

(function () {
     
    angular.module('employeeCalendar.globals', [])
    .service('globals', function ($http, $q) {
        return {
            get: function (name) {
                var d = $q.defer();
                d.resolve(1);
                return d.promise;
            }
        }
    });

})();


describe('employeeCalendar.globals', function () {

    var $httpBackEnd;
    
    beforeEach(function () {
        module('employeeCalendar.globals');
    })
    
    beforeEach(inject(function ($injector) {
       
        $httpBackEnd = $injector.get('$httpBackend');
        $httpBackEnd.when('GET', globalsUrl).respond({
            offices: [{ id: 1, name: 'office1' }]
        });
    }));

    describe('when asked for something', function () {

        var _globalValues;

        beforeEach(function(done) {
            inject(function (globals) {
                globals.get('offices').then(function (data) {

                    _globalValues = data;
                    done();
                }, function () { done(); });
            });
        });

        it('retrieves globals', function () {
            expect(_globalValues).toBeDefined();
        });

        xit('retrieves globals only once', function () {
            // expect $http call to be made only once 

        });

    });
     
   
});
 