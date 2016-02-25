// used for select list with options
ko.bindingHandlers.option = {
    update: function (element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        ko.selectExtensions.writeValue(element, value);
    }
};

// hidden is reverse to visible 
ko.bindingHandlers.hidden = {
    update: function (element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        ko.bindingHandlers.visible.update(element, function () { return !value; });
    }
};

// money binding 

ko.bindingHandlers.money = {
    update: function (element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        $(element).text(accounting.formatMoney(value));
    }
    
    //read: function () {
    //    if (target) {
    //        var t = target();
    //        if (!t) t = 0;
    //        return parseFloat(t.toString().replace(",", ".")).toFixed(precision || 2);
    //    }
    //}
};

// date binding 

ko.bindingHandlers.date = {
    update: function (element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        $(element).text(moment(value).format('L'));
    }
};

// numeric input binding
ko.bindingHandlers.numeric = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        console.log('init fired');
        $(element).val(value.toString().replace('.', ','));
        //$(element).blur(function () {
        //    var value = valueAccessor();
        //    var stringValue = $(element).val().toString().replace(",", ".");
        //    var newVal = isNaN(stringValue) ? 0 : parseFloat(stringValue);
        //    value(newVal);
        //});
        $(element).keyup(function () {
            console.log('keyup  fired')
            var value = valueAccessor();
            var stringValue = $(this).val().toString().replace(",", ".");
            var newVal = isNaN(stringValue) ? 0 : parseFloat(stringValue);
            value(newVal);
        });
    },
    update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        console.log('update fired');
        var value = ko.utils.unwrapObservable(valueAccessor());
        console.log('value is ' + value.toString());
        $(element).val(value.toString().replace('.', ','));
    },
    write: function (newValue) {
        underlyingObservable(parseFloat(newValue.toString().replace(",", ".")));
    }
}

//ko.extenders.numeric = function (target, precision) {

//    var result = ko.computed({
//        read: function () {
//            if (target) {
//                var t = target();
//                if (!t) t = 0;
//                return parseFloat(t.toString().replace(",", ".")).toFixed(precision || 2);
//            }
//        },
//        write: target
//    });

//    result.raw = target;
//    return result;
//};

ko.bindingHandlers.accordion = {

    init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        $(element).next().hide();
    },
    update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {

        var slideUpTime = 200;
        var slideDownTime = 200;

        var openState = ko.utils.unwrapObservable(valueAccessor());
        var focussed = openState.focussed;
        var shouldOpen = openState.shouldOpen;

        if (focussed) {

            var clickedGroup = viewModel;

            $.each(bindingContext.$root.view().days(), function (idx, group) {
                if (clickedGroup != group) {
                    group.openState({ focussed: false, shouldOpen: false });
                }
            });
        }

        var dropDown = $(element).next();

        var header = $(element).find('.chevron');
        
        
        if (shouldOpen) {
            header.addClass('glyphicon-chevron-up');
            header.removeClass('glyphicon-chevron-right');
        }
        else {
            header.removeClass('glyphicon-chevron-up');
            header.addClass('glyphicon-chevron-right');
        }
        

        if (focussed && shouldOpen) {
            dropDown.slideDown(slideDownTime);
        } else if (focussed && !shouldOpen) {
            dropDown.slideUp(slideUpTime);
        } else if (!focussed && !shouldOpen) {
            dropDown.slideUp(slideUpTime);
        }

        
    }
};