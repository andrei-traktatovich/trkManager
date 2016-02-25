function Utils(context) {

    console.log('got context from root');
    console.dir(context);
    var justExpenseTypes = [];
    ko.utils.arrayForEach(context.expenseGroups(), function (group) {
        ko.utils.arrayForEach(group.items(), function (item) {
            justExpenseTypes.push(item);
        });
    });

    var getOfficeName = function (officeId) {
        return ko.utils.arrayFirst(context.offices(), function (item) { return item.id() == officeId; }).name();
    };
    var getTypeName = function (typeId) {
        var result = ko.utils.arrayFirst(justExpenseTypes, function (item) { return item.id() == typeId; });
        if (result && result.name) return result.name();
        else return '';
    };

    var createEmptyExpenseItem = function (createdDate) {
        
        return {
            id: 0,
            isDeleted: false,
            isDirty: false,
            date: createdDate,
            
            isEditable: true,
            noteNo: 0,
            typeId: 34,
            officeId: context.myOfficeId(),
            text: '',
            amount: 0.0,
            author: context.author(),
            authorId: context.authorId()
        };
    };

    var createEmptyIncomeItem = function (createdDate) {
        return {
            id: 0,
            isDeleted: false,
            isDirty: false,
            date: createdDate,
            isEditable: true,
            typeId: 2,
            noteNo: 0,
            payerName: '',
            officeId: context.myOfficeId(),
            text: '',
            amount: 0.0,
            author: context.author(),
            authorId: context.authorId()
        };
    }
    return {
        getOfficeName: getOfficeName,
        getTypeName: getTypeName,
        createEmptyExpenseItem: createEmptyExpenseItem,
        createEmptyIncomeItem: createEmptyIncomeItem
    };
}