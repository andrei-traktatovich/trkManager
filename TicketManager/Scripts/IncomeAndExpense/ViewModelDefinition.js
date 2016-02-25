function IncomeItem(data, mappingOptions) {

    console.log('income item - mapping hit');
    var result = ko.mapping.fromJS(data, mappingOptions);

    console.log('adding hit');
    result.isDeleted = ko.observable(false);
    result.isDirty = ko.observable(false);
    result.isEditing = ko.observable(false);
    result.officeName = ko.computed(function () { return mappingOptions.utils.getOfficeName(this.officeId()); }, result);
    //this.typeName = ko.computed(function () { return myUtils.getTypeName(this.typeId); });

    var editFunc = function () {
        if (!this.isEditable() || (this.isEditing() || mappingOptions.vm.isItemEditing()))
            return;
        this.isEditing(true);
       
        mappingOptions.vm.isItemEditing(true);
        mappingOptions.vm.isViewDirty(false);
    }

    var checkValidFunc = function () {
        console.log('income item validation hit');

        return true; //this && !isNaN(this.amount()) && this.amount() > 0 && this.payerName().length > 0;
    };

    result.isValid = ko.computed(checkValidFunc, result);

    var removeFunc = function () {
        var self = this;
        if (this.isEditable()) {
            bootbox.confirm("Вы уверены, что хотите удалить эту запись?", function () {
                self.isDeleted(true);
                mappingOptions.vm.deleteItem(self.id());
                mappingOptions.vm.isViewDirty(true);
                mappingOptions.vm.isItemEditing(false);
                mappingOptions.vm.reduce(mappingOptions.vm.view().totalIncomes, self.amount());
                
            });
        }
    };
    

    var commitFunc = function () {
        console.log('commit hit');
        if (this.isValid()) {
            console.log('commit: valid');
            var initialValue = ko.utils.unwrapObservable(this.amount);
            console.log('initial value = ' + initialValue.toString());
            var value = parseFloat(ko.utils.unwrapObservable(this.amount).toString().replace(",", "."));
            this.amount(value);
            
            this.isEditing(false);
            this.isDirty(true);
            mappingOptions.vm.saveIncome(this);
            this.isDirty(false);
            mappingOptions.vm.isItemEditing(false);
            mappingOptions.vm.isViewDirty(true);
            mappingOptions.vm.add(mappingOptions.vm.view().totalIncomes, this.amount());
        }
    };

    result.commit = commitFunc;

    var tryCommitFunc = function (data, event) {
        if (event.ctrlKey && event.keyCode == 13)
            this.commit();
    };

    result.tryCommit = tryCommitFunc;
    result.edit = editFunc;
    result.tryEdit = editFunc; // do I really need this one? 
    result.remove = removeFunc;
    console.log('income item - mapping ended');
    return result;
}

function ExpenseItem(data, mappingOptions) {

    console.log('expense item - mapping hit');
    var result = ko.mapping.fromJS(data, mappingOptions);

    result.isDeleted = ko.observable(false);
    result.isDirty = ko.observable(false);

    result.isEditing = ko.observable(false);
    result.officeName = ko.computed(function () {
        console.log('expense item - retrieving office name ');
        var r = mappingOptions.utils.getOfficeName(this.officeId());
        console.log('expense item - retrieving office name successful');
        return r;
    }, result);

    result.typeName = ko.computed(function () {
        console.log('expense item - retrieving type name for type : ' + this.typeId);
        return mappingOptions.utils.getTypeName(this.typeId());
        console.log('expense item - retrieving type name successful');
    }, result);

    var checkValidFunc = function () {
        console.log('expense item validation hit');
        return true;//!isNaN(this.amount()) && this.amount() > 0 && this.text().length > 0;
    };
    result.isValid = ko.computed(checkValidFunc, result);

    var editFunc = function () {
        if (!this.isEditable() || (this.isEditing() || mappingOptions.vm.isItemEditing()))
            return;
        this.isEditing(true);
        mappingOptions.vm.isItemEditing(true);
        mappingOptions.vm.isViewDirty(true);
        
    }

    var removeFunc = function () {
        var self = this;
        if (this.isEditable()) {
            bootbox.confirm("Вы уверены, что хотите удалить эту запись?",
                function () {
                     
                    self.isDirty(false);
                    self.isDeleted(true);
                    mappingOptions.vm.deleteItem(self.id());
                    mappingOptions.vm.isViewDirty(true);
                    mappingOptions.vm.isItemEditing(false);
                    mappingOptions.vm.reduce(mappingOptions.vm.view().totalExpenses, self.amount());
                });
        }
    };

    var commitFunc = function () {
        console.log('commit hit');
        if (this.isValid()) {
            console.log('commit: valid');
            var initialValue = ko.utils.unwrapObservable(this.amount);
            console.log('initial value = ' + initialValue.toString());
            var value = Math.round(parseFloat(ko.utils.unwrapObservable(this.amount).toString().replace(",", ".")) * 100) / 100;
            this.amount(value);
            this.isEditing(false);
            this.isDirty(true);
            mappingOptions.vm.saveExpense(this);
            this.isDirty(false);
            mappingOptions.vm.isViewDirty(true);
            mappingOptions.vm.isItemEditing(false);
            mappingOptions.vm.add(mappingOptions.vm.view().totalExpenses, this.amount());
        }
    };

    result.commit = commitFunc;

    var tryCommitFunc = function (data, event) {
        if (event.ctrlKey && event.keyCode == 13)
            this.commit();
    };


    result.tryCommit = tryCommitFunc;
    result.edit = editFunc;
    result.tryEdit = editFunc; // do I really need this one? 
    result.remove = removeFunc;
    console.log('expense item - mapping ended');
    return result;
}

function Day(data, mappingOptions) {

    var result = ko.mapping.fromJS(data, mappingOptions);
    
    result.totalIncomes = ko.computed(function () {
        var sum = 0;
        var incomes = ko.mapping.toJS(result.incomes);
        
        ko.utils.arrayForEach(incomes, function (item) { if(!item.isDeleted)
            sum = sum + item.amount;
        });
        return sum;
    });
    
    result.totalExpenses = ko.computed(function () {
        var sum = 0;
        var expenses = ko.mapping.toJS(result.expenses);
        ko.utils.arrayForEach(expenses, function (item) { if(!item.isDeleted) sum = sum + item.amount; });
        return sum;
    });
    
    result.openState = ko.observable({ focussed: false, shouldOpen: false });

    result.toggle = function (group, event) {
        var shouldOpen = group.openState().shouldOpen;
        result.openState({ focussed: true, shouldOpen: !shouldOpen });
    };

    var pushExpenseItem = function (day) {
        if (mappingOptions.vm.isItemEditing())
            return;
        var newItem = new ExpenseItem(mappingOptions.utils.createEmptyExpenseItem(this.date()), mappingOptions);
        newItem.officeId(mappingOptions.vm.context.myOfficeId());
        newItem.isEditing(true);
        day.expenses.push(newItem);
        mappingOptions.vm.isItemEditing(true);
        
    };
    var pushIncomeItem = function (day) {
        if (mappingOptions.vm.isItemEditing())
            return;
        var newItem = new IncomeItem(mappingOptions.utils.createEmptyIncomeItem(this.date()), mappingOptions);
        console.log('created new item at office= ' + newItem.officeId().toString());
        newItem.officeId(mappingOptions.vm.context.myOfficeId());
        newItem.isEditing(true);
        day.incomes.push(newItem);
        mappingOptions.vm.isItemEditing(true);
        
    };

    result.addExpenseItem = pushExpenseItem;
    result.addIncomeItem = pushIncomeItem;
    return result;
}

function ViewMappingOptions(viewModel, utils) {

    this.vm = viewModel;
    this.utils = utils;
    var self = this;
    this.mappingOptions = {
        'amount': {
            'create': function (options) {
                return ko.observable(options.data).extend({ numeric: 2 });
            }
        },
        'days': {
            'create': function (options) {
                return new Day(options.data, self.mappingOptions);
            }
        },
        'incomes': {
            'create': function (options) {
                return new IncomeItem(options.data, self.mappingOptions);
            }
        },
        'expenses': {
            'create': function (options) {
                return new ExpenseItem(options.data, self.mappingOptions);
            }
        },
        utils: self.utils,
        vm: self.vm
    };
    this.mappingOptions.vm = viewModel;

    return this.mappingOptions;

}

function ViewModel(context, utils) {

    var self = this, today = new Date();

    var options = new ViewMappingOptions(this.viewModel, utils);

    

    var viewModel = {
        context: ko.mapping.fromJS(context),
        month: ko.observable(today.getMonth() + 1),
        year : ko.observable(today.getFullYear()),
        isViewEmpty: ko.observable(true),
        isViewDirty: ko.observable(false),
        isItemEditing: ko.observable(false),
        view: ko.observable(),
        loadData: getData,
        reloadData : reloadData, 
        saveIncome: saveIncome,
        saveExpense: saveExpense,
        deleteItem: deleteItem,
        add: add,
        reduce : reduce
    };

    var options = new ViewMappingOptions(viewModel, utils);
    viewModel.mappingOptions = options;
    
    

    return viewModel;
    
    function add(targetObservable, amount) {
        var value = targetObservable() + amount;
        targetObservable(value);
        recalculateClosingBalance();
    }

    function reduce(targetObservable, amount) {
        var value = targetObservable() - amount;
        targetObservable(value);
        recalculateClosingBalance();
    }

    function recalculateClosingBalance() {
        var openingBalance = viewModel.view().openingBalance();
        var totalIncomes = viewModel.view().totalIncomes();
        var totalExpenses = viewModel.view().totalExpenses();
        var result = openingBalance + totalIncomes - totalExpenses;
        viewModel.view().closingBalance(result);
    }

    function reloadData() {
        if (viewModel.isViewDirty()) {
            bootbox.confirm("Изменения не были сохранены. Вы уверены, что хотите обновить? Все несохраненные изменения будут потеряны", getData);
        }
        else {
            getData();
        }

    }

    function saveIncome(item) {
        
        var itemToSave = ko.mapping.toJS(item);
        var data = {
            id: itemToSave.id,
            noteNo: itemToSave.noteNo,
            amount: itemToSave.amount,
            text: itemToSave.payerName,
            payer: itemToSave.payer,
            officeID: itemToSave.officeId,
            date : moment(itemToSave.date)
        };

        var ajaxService = new AjaxService();
        ajaxService.post(incomeUpdateURL, data, success, failure);

        function success(result) {
            toastr.success('Данные успешно сохранены');
            item.id(result.id);
        }

        function failure(result) {
            toastr.error('Произошла ошибка');
        }
    }

    function deleteItem(id) {
        var data = { id: id };

        var ajaxService = new AjaxService();
        ajaxService.post(deleteURL, data, success, failure);

        function success(result) {
            toastr.success('Запись удалена');
        }

        function failure(result) {
            toastr.error('Произошла ошибка');
        }
    }

    function saveExpense(item) {
        var expense = ko.mapping.toJS(item);
        var data = {
            id: expense.id,
            amount: expense.amount,
            text: expense.text,
            typeId: expense.typeId,
            officeID: expense.officeId,
            date: moment(expense.date)
        };

        var ajaxService = new AjaxService();
        ajaxService.post(expenseUpdateURL, data, success, failure);

        function success(result) {
            toastr.success('Данные успешно сохранены');
            item.id(result.id);
        }

        function failure(result) {
            toastr.error('Произошла ошибка');
        }
    }


    function getData() {

        var ajaxService = new AjaxService();
        var data = {};
        var parameters = {
            office: viewModel.context.myOfficeId() || 0,
            month: viewModel.month() || 0,
            year: viewModel.year() || 0
        };

        ajaxService.load(IncomesAndExpensesURL, data, parameters, modelLoadSuccess);

        function modelLoadSuccess(result) {
            
            console.log('here is the data');
            console.dir(result);
            //console.dir(ko.mapping.fromJS(result, options));
            //console.log('here are the days');
            //console.dir(ko.mapping.fromJS(result, options).days()[0].incomes()[0]);
            var mappedModel = ko.mapping.fromJS(result, options);
            viewModel.view(mappedModel);
            //this.view(mappedModel);
            viewModel.isViewEmpty(false);
            viewModel.isItemEditing(false);
            viewModel.isViewDirty(false);
            
            //this.isViewEmpty(false);
        }

        
    }
}



