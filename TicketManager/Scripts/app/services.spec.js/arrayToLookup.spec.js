/// <reference path="../../jasmine/jasmine-test.js" />

/// <reference path="../../lodash.min.js" />

/// <reference path="../third-party/third-party.module.js" />

/// <reference path="../services/services.module.js" />
/// <reference path="../services/arrayToLookup.js" />


describe("arrayToLookup", () => {

    beforeEach(() => {
        module("services");
    });

    it("supplies an array with additional functions", () => {
        var testArray = [];
        
        inject((arrayToLookup) => {
            arrayToLookup(testArray);
            expect(testArray.lookup).toBeDefined();
            expect(testArray.idByName).toBeDefined();
            expect(testArray.nameById).toBeDefined();
        });
    });

    it("finds display member by key member", () => {
        var testArray = [
            { id: 1, name: "name" },
            { id: 2, name: "another name" }
        ];

        inject((arrayToLookup) => {
            arrayToLookup(testArray);
            expect(testArray.nameById(1)).toEqual("name");
        });
    });

    it("finds key member by display member", () => {
        var testArray = [
            { id: 1, name: "name" },
            { id: 2, name: "another name" }
        ];

        inject((arrayToLookup) => {
            arrayToLookup(testArray);
            expect(testArray.idByName("another name")).toEqual(2);
        });
    });

    it("finds any member by any member", () => {
        var testArray = [
            { id: 1, name: "name" },
            { id: 2, name: "another name" }
        ];

        inject((arrayToLookup) => {
            arrayToLookup(testArray);
            expect(testArray.lookup("name", "id", "another name")).toEqual(testArray[1]);
        });
    });

    it("if an object is supplied, each property that is an array is made a lookup list", () => {
        inject((arrayToLookup) => {
            var testObject = {
                testArray: [
                    { id: 1, name: "name" },
                    { id: 2, name: "another name" }
                ],
                testArray2: [
                    { id: 1, name: "name" },
                    { id: 2, name: "another name" }
                ]
            };
            arrayToLookup(testObject);
            console.log(testObject);

            expect(testObject.testArray.idByName).toBeDefined();
        });
    });
});

 