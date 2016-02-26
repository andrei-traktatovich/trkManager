/// <reference path="C:\Users\nikolaev\Documents\Visual Studio 2013\Projects\ticketManager 1702\TicketManager\Scripts/jasmine/jasmine.js" />
/// <reference path="C:\Users\nikolaev\Documents\Visual Studio 2013\Projects\ticketManager 1702\TicketManager\Scripts/angular.min.js" />
/// <reference path="C:\Users\nikolaev\Documents\Visual Studio 2013\Projects\ticketManager 1702\TicketManager\Scripts/angular-mocks.js" />

var app = angular.module('testModule', []);
app.factory('dataService', function () {
    return {
        isGood: true
    }
});

describe('testing', function () {
    beforeEach(function () {
        module('testModule');
    });

    describe('just', function () {
        it('works', inject(function (dataService) {
            expect(dataService.isGood).toEqual(true);
        }));
    })
    
})