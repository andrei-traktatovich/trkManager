


//function doPageSettings() {
//    $('#txtName').keyup(function () {
//        $('#drdnOfficeID').attr("disabled", $('#txtName').val() != '');
//    });
//    $('#tabel').window({ closed: true, width: 1000, height: 800 });
//    $('#filter-div').mouseleave(function () { $(this).fadeOut(); });
    
//    var browserWidth = $(window).width();
//    $('#calendar-container').width(browserWidth);
//    $('#calendar-container').height(700);
    
//    if (calendarServiceURL)
//        window.calendar = new Calendar(calendarServiceURL, updateCalendarURL, getGlobalsURL, new HTTPService());
//}
//var calendar = new Calendar(calendarServiceURL, updateCalendarURL, getGlobalsURL, new HTTPService());
//var calendar = new Calendar(calendarServiceURL, updateCalendarURL, getCalendarGlobalsURL, new HTTPService());

function showCalendarByID(id, month, year) {
    var date = new Date();
    var currentMonth = date.getMonth();
    var currentYear = date.getFullYear();
    calendar.getCalendar(id, (month || currentMonth), (year || currentYear), updateEmployeeList);
    //$('#calendar-popup')
    //  .dialog({ closed: false, width: 700, height: 600 });

    function updateEmployeeList() {
        refreshWhoWhereSearchResults();
    }
}
//var service = new HTTPService();
//service.getData(calendarServiceURL, { id: 407, month: 11, year: 2013 }, function callBack(data) { alert('dd'); });



function HTTPService() {
    var self = this;
    this.callBack = {};
    this.showMessage = false;

    function success(data, textStatus, jqXHR) {
        hideProgressAnimation();
        console.log('success');

        /*if (self.showMessage) {
            $.messager.show({
                title: 'Сохранение данных',
                msg: 'Данные успешно сохранены',
                timeout: 5000,
                showType: 'slide'
            });
        }*/
        if (self.showMessage) {
            toastr.success("Сохранение данных", "Данные успешно сохранены");
        }

        if (typeof self.callBack == "function") {
            console.log('calling callback');
            self.callBack(data);
        }
    }

    function failure(data,textStatus,jqXHR) {
        console.log('error');
        hideProgressAnimation();
        showMessage(jqXHR);
    }
    
    this.getJSONData = function (url, params, callback) {
        
        self.showMessage = false;
        self.callBack = callback;
        showProgressAnimation();
        $.getJSON(url, params)
        .done(success)
        .fail(failure);
    }
    this.getData = function (url, params, callback, usePost) {
        self.showMessage = false;
        self.callBack = callback;
        showProgressAnimation();
        //$.getJSON(url, params)
        //.done(success)
        //.fail(failure);
        $.ajax({
            type: usePost ? "POST" : "GET",
            url: url,
            data: JSON.stringify(params),
            dataType: 'json',
            contentType: 'application/json',
            traditional: true,
            success: success,
            error: failure

        });

    };
    
    this.post = function post(url, data, callBack, showMessage) {
        self.callBack = callBack;
        self.showMessage = showMessage;
        showProgressAnimation();
        console.log("post data= url:" + url);
        console.dir(data);
        console.log(JSON.stringify(data));
        $.ajax({
            type: "POST",
            url: url,
            data: JSON.stringify(data),
            dataType: 'json',
            contentType: 'application/json',
            
            success: success,
            error: failure
            
        });
        //$.post(url, data)
        //.done(success)
        //.fail(failure);
    };

    function showMessage(jqXHR) {
        $.messager.show({
            title: 'Ошибка',
            msg: 'Произошла досадная ошибка: ' + jqXHR,
            timeout: 50000,
            showType: 'slide',
            style: {
                background: "red"
            }
        });
    }
}

function Calendar(getCalendarURL, updateCalendarURL, getGlobalsURL, service) {
    
    var self = this, vm = {}, mapped = false;
    
    self.months = ['январь', 'февраль', 'март', 'апрель',
        'май', 'июнь', 'июль', 'август', 'сентябрь', 'октябрь', 'ноябрь', 'декабрь'];

    self.selection = [];

    this.service = service;

    if (!self.globals)
        setGlobals();


    this.getCalendar = function (id, month, year, callBackOnSave) {
        
        self.callBackOnSave = callBackOnSave;
        self.id = id;
        self.month = month;
        self.year = year;
        self.selection = [];
        
        // do parameter checks here

        // get raw data
        var params = { id: id, month: month, year: year };
        
        service.getJSONData(getCalendarURL, params, createCalendar);
    };

    function setGlobals() {

        /*self.globals = {
            offices: [{ id: 0, name: 'таганка' }, { id: 2, name: 'кремль' }, { id: 3, name: 'мурло' }],
            staffStatuses: [{ id: 0, name: 'хз' }, { id: 2, name: 'умер' }, { id: 3, name: 'болен' }],

        };*/
        service.getData(getGlobalsURL, {}, storeGlobals);

        function storeGlobals(data) {
            self.globals = data;
        }
    }

    function createCalendar(data) {
        console.log("calendar data = ");
        console.dir(data);

        var model = modelFromRawData(data);
        console.dir(model);

        model.month = self.month;
        model.year = self.year;



        model.globals = self.globals;

        if (mapped) {
            ko.mapping.fromJS(model, vm);
            mapped = true;
            return;
        }

        $('#calendar-area').show();
        
        mapped = true;
        
            
        vm = ko.mapping.fromJS(model);
        vm.daysSelected = [false, false, false, false, false, false, false];
        vm.isDirty = ko.observable(false);

        vm.moveToPreviousMonth = function () {
            console.log('currentmonth = ' + self.month);
            if (self.month == 0) {
                self.year--;
                self.month = 11;
            }
            else
                self.month--;

            self.getCalendar(self.id, self.month, self.year);
        }

        vm.moveToNextMonth = function () {
            console.log('currentmonth = ' + self.month);
            if (self.month == 11) {
                self.year++;
                self.month = 0;
            }
            else
                self.month++;

            self.getCalendar(self.id, self.month, self.year);
        }

        vm.timeRange = ko.computed(function () {
            return self.months[vm.month()] + ' ' + vm.year().toString();
        });

        vm.applyOffice = function (data) {
            vm.isDirty(true);
            console.log('this is data');
            console.dir(data);
            var newOfficeID = data.id();
            var newOfficeName = data.name();
            console.log(data.id() + data.name());
            self.selection.forEach(function (entry) {
                entry.OfficeID(newOfficeID);
                entry.Office(newOfficeName);
                entry.Author(vm.userName());
                entry.LastChanged(new Date());
            });
        };

        vm.applyStaffStatus = function (data) {
            vm.isDirty(true);
            var newStaffStatusID = data.id();
            var newStaffStatusName = data.name();
            console.log(data.id() + data.name());
            self.selection.forEach(function (entry) {
                entry.Status.ID(newStaffStatusID);
                entry.Status.Name(newStaffStatusName);
                entry.Author(vm.userName());
                entry.LastChanged(new Date());
            });
        };

        vm.clearAll = function () {
            vm.weeks()
                .forEach(function (week) { week.selected(true); vm.selectWeek(week); });
        }

        vm.selectAll = function () {
            vm.weeks()
                .forEach(function (week) { week.selected(false); vm.selectWeek(week); });
        }

        vm.addComment = function () {
            vm.isDirty(true);
            self.selection.forEach(function (item) {
                item.Comment($('#inp-comment').val()); item.Author(vm.userName()); item.LastChanged(new Date());
            });
        };

        vm.confirmSelection = function () {
            vm.isDirty(true);
            self.selection.forEach(function (item) {
                item.Confirmed(true); item.Author(vm.userName()); item.LastChanged(new Date());
            });
        };

        vm.disconfirmSelection = function () {
            vm.isDirty(true);
            self.selection.forEach(function (item) {
                item.Confirmed(false); item.Author(vm.userName()); item.LastChanged(new Date());
            });
        };

        vm.setStandBy = function () {
            vm.isDirty(true);
            self.selection.forEach(function (item) {
                item.StandBy(true); item.Author(vm.userName()); item.LastChanged(new Date());
            });
        };

        vm.removeStandBy = function () {
            vm.isDirty(true);
            self.selection.forEach(function (item) {
                item.StandBy(false); item.Author(vm.userName()); item.LastChanged(new Date());
            });
        };

        vm.selectWeekDay = function (dayNo) {
            var newValue = !vm.daysSelected[dayNo - 1];
            vm.daysSelected[dayNo - 1] = newValue;
            vm.weeks().forEach(function (item) {
                item.days().forEach(function (day) {
                    if (day.weekDay() == dayNo) {
                        if (day.selected()) {
                            var index = self.selection.indexOf(day);
                            self.selection.splice(index, 1);
                        }
                        day.selected(newValue);
                        if (newValue)
                            self.selection.push(day);
                    }
                });
            });
        };

        vm.selectWeek = function (week) {
            var newValue = !week.selected();
            week.selected(newValue);
            week.days().forEach(function (day) {
                if (day.weekDay() < 6) {
                    if (day.selected()) {
                        var index = self.selection.indexOf(day);
                        self.selection.splice(index, 1);
                    }
                    day.selected(newValue);
                    if (newValue)
                        self.selection.push(day);
                }
            });
        }

        vm.test = function () { alert('sss'); };
        
        vm.commit = function commit(vm) {
            var url = updateCalendarURL;
            var daysArray = weeksToDayInfoArray(vm.weeks);
            var data = {
                ID: vm.id(),
                Days: daysArray,
                month: self.month,
                year: self.year
            };

            self.service.post(url, data, success, true);
            console.dir(data);

            function success() {
                vm.isDirty(false);
                self.callBackOnSave();
                
                 
            }

            function weeksToDayInfoArray(weeks) {
                var result = [];
                weeks().forEach(function (item) {
                    item.days().forEach(function (day) {
                        result.push({
                            OfficeID: day.OfficeID(),
                            StatusID: day.Status.ID(),
                            Comment: day.Comment(),
                            Author: day.Author(),
                            LastChanged: day.LastChanged(),
                            Confirmed: day.Confirmed(),
                            StandBy: day.StandBy()
                        });
                    });
                });
                return result;
            }
        }

        ko.applyBindings(vm);
        
         
    }

    
    

    function selectDay(item) {
        console.log('selection');
        item.selected(!(item.selected()));
        modifySelection(item);
    }

    function modifySelection(item) {

        if (item.selected())
            self.selection.push(item);
        else {
            var index = self.selection.indexOf(item);
            self.selection.splice(index, 1);
        }
    }

    function modelFromRawData(data) {
        // provide numbers, divide into weeks ...
        var model = { weeks: [], name: data.Name, id: data.ID, userName: data.UserName, canEdit : data.CanEdit },
            counter = 0,
            day = {},
            week = {},
            weekDay = 0;
            
        while (counter < data.Weeks.length) {
            var date = new Date(self.year, self.month, counter + 1);
            weekDay = date.getDay();
            if (weekDay == 0)
                weekDay = 7;
            
            week = { selected: false, firstWeekDay: weekDay, offset: ((weekDay - 1) * 80).toString() + 'px', days: [] };

            while (weekDay <= 7 && counter < data.Weeks.length) {
                day = data.Weeks[counter++];
                day.date = counter;
                day.weekDay = weekDay;
                day.selected = false;
                day.selectDay = selectDay;
                week.days.push(day);
                weekDay++;
            }
            model.weeks.push(week);
        }
        return model;
    }

        

}


function displayWorkSheetTable() {
    var officeID = parseInt($('#drdnOfficeID').val());
    if (officeID >= 0) {
        data = { officeID: officeID };
        $('#tabel-info').load(getWorkSheetTableURL, data);
         
    }
}

function downloadWorkSheetTable() {
    var officeID = parseInt($('#drdnOfficeID').val());
    if (officeID >= 0) {
        data = { officeID: officeID };
        $.post(downloadWorkSheetTableURL, data);
    }
}

function modifyPermanentOffice(name, id, permOffice) {
    // setting values 
    // name
    $('#popup-change-perm-office-trname').text(name);
    // office dropdown
    $('#popup-change-perm-office-officeid').val(permOffice);

    $('#popup-change-perm-office-submit').unbind();
    $('#popup-change-perm-office-submit').on('click', function () { submitPermOfficeChange(id); }); 
}

function modifyTempOffice(name, id, tempOffice) {
    // setting values 
    // name
    $('#popup-change-temp-office-trname').text(name);
    // office dropdown
    $('#popup-change-temp-office-officeid').val(tempOffice);

    $('#popup-change-temp-office-submit').unbind();
    $('#popup-change-temp-office-submit').on('click', function () { submitTempOfficeChange(id); });

}

function modifyTempStatus(name, id, statusID) {
    // setting values 
    // name
    console.log('modifyTempStatus');
    $('#popup-change-staff-status-trname').text(name);
    // office dropdown
    $('#popup-change-staff-statusId').val(statusID);

    $('#popup-change-staff-status-submit').unbind();
    $('#popup-change-staff-status-submit').on('click', function () { submitTempStatusChange(id); });

}

function modifyTitle(name, id, titleID) {
    // setting values 
    // name
    
    $('#popup-change-title-trname').text(name);
    // office dropdown
    $('#popup-change-titleid').val(titleID);

    $('#popup-change-title-submit').unbind();
    $('#popup-change-title-submit').on('click', function () { submitTitleChange(id); });

}

function submitTitleChange(id) {
    var data = { id: id, titleID: $('#popup-change-titleid').val() };
    showProgressAnimation();
    $.post(updateTitleURL, data, onComplete);
}

function submitTempStatusChange(id) {
    var data = { id: id, newStatus: $('#popup-change-staff-statusId').val() };
    showProgressAnimation();
    $.post(updateTempStatusURL, data, onComplete);
}

function modifyActiveStatus(name, id, active) {
    // setting values 
    // name

    $('#popup-change-active-status-trname').text(name);
    // office dropdown
    $('#popup-change-active-status-statusid').val(active);

    $('#popup-change-active-status-submit').unbind();
    $('#popup-change-active-status-submit').on('click', function () { submitNewActiveStatus(id); });
     
}

function submitNewActiveStatus(id) {
    var data = { id: id, newStatus: $('#popup-change-active-status-statusid').val() };
    showProgressAnimation();
    $.post(updateActiveStatusURL, data, onComplete);
}

function onComplete(response, status, xhr) {

    if (status == "error") {

        alert('status == error');
        ajaxErrorHandler(response, status, xhr);

    }
    else refreshWhoWhereSearchResults();
}

function submitPermOfficeChange(id) {
    var data = { id: id, officeID: $('#popup-change-perm-office-officeid').val() };

    showProgressAnimation();
    $.post(updatePermOfficeURL, data, onComplete);

}

function submitTempOfficeChange(id) {
    var data = { id: id, officeID: $('#popup-change-temp-office-officeid').val() };

    showProgressAnimation();
    $.post(updateTempOfficeURL, data, onComplete);

}

function submitToUpdateCalendar() {
    calendarSelection.Status = parseInt($('#drdnStatusSelector').val());
    calendarSelection.Office = $('#drdnOfficeSelector').val();
    calendarSelection.StandBy = $('#chkStandBy').attr('checked') == 'checked' ? true : false;
    calendarSelection.Confirmed = $('#chkConfirmed').attr('checked') == 'checked' ? true : false;
    calendarSelection.Comment = $('#txtTranslatorComment').val();

    var data = { Selection: calendarSelection };
    showProgressAnimation();
    $.post(updateCalendarURL, calendarSelection, function () { changeCurrentOffice(translatorID, currentMonth, currentYear); refreshWhoWhereSearchResults(); });
    hideProgressAnimation();
    $('#changeStatusPopup').dialog('close');
    var selectedDays = $('.day .selected');
    if (calendarSelection.Confirmed == true) {
        selectedDays.addClass('day-confirmed');
        selectedDays.removeClass('non-confirmed');

    }
    else {
        selectedDays.removeClass('day-confirmed');
        selectedDays.addClass('non-confirmed');
    }
    selectedDays.removeClass('selected');

    refreshWhoWhereSearchResults();
}

function whoWhereSearchComplete() {
    $('#progress').hide();
}
function whoWhereSearchFailure() {
    toastr.error("Произошла ошибка ...", "произошла нелепая ошибка");
}

function refreshWhoWhereSearchResults() {
    showProgressAnimation();
    $('.popupdialog').unbind('dialogclose');
    $('.popupdialog').dialog("close");

    var data = { txtName: $('#txtName').val(), drdnOfficeID: $('#drdnOfficeID').val() };

    $('#whoWhereSearchResultsTable').load(WhoWhereSearchResultsURL, data, ajaxErrorHandler);
    hideProgressAnimation();
}

function modifyCurrentOffice(name, id, month, year, permanentOffice) {
    //$('#popuptitle-translator-name').text(name);
    $('#drdnOfficeSelector').val(permanentOffice); // setting default value = PermanentOffice
    translatorID = id;
    currentTranslatorName = name;
    changeCurrentOffice(id, month, year)
}

var months = ['январь', 'февраль', 'март', 'апрель', 'май', 'июнь', 'июль', 'август', 'сентябрь', 'октябрь', 'ноябрь', 'декабрь'];
var currentTranslatorName = "";

function changeCurrentOffice(id, month, year) {
    //$('#popup').unbind('dialogclose');
    //$('#popup').dialog().onClosed { onClose: function () { refreshWhoWhereSearchResults(); } });


    var title = currentTranslatorName + " (" + months[month] + ' ' + year.toString() + ' г.' + ")";
    $('#txtTranslatorID').val(id);
    $('#popup').dialog({ title: title });
    //$('#popuptitle-date').text(title);
    $('#monthBack').unbind();
    $('#monthForward').unbind();
    $('#monthBack').click(function () { showPreviousMonth(id); });
    $('#monthForward').click(function () { showNextMonth(id); });
    $('#popup').dialog('open');
    $.getJSON(calendarServiceURL, { id: id, month: month, year: year }, function (data) {
        showCalendar(data);
    });
}

function showPreviousMonth(id) {
    if (currentMonth > 0)
        currentMonth--;
    else {
        currentMonth = 11;
        currentYear--;
    }
    changeCurrentOffice(id, currentMonth, currentYear);
}
function showNextMonth(id) {
    if (currentMonth < 11)
        currentMonth++;
    else {
        currentMonth = 0;
        currentYear++;
    }
    changeCurrentOffice(id, currentMonth, currentYear);
}
function showCalendar(data) {
    // clearing calendar popup except header
         
    $('.week').remove();
    var firstDay = 1;
    var day = 1;
    var weekCounter = 1;
    var maxDays = getDaysInMonth(currentMonth, currentYear);
       

    // do until month change
    while (day <= maxDays) {
             
        var theDate = new Date(currentYear, currentMonth, day);
        var weekDay = theDate.getDay() == 0 ? 7 : theDate.getDay();
        var offset = (weekDay - 1) * 80;
             
        var el = '<div class="week" style="margin-left:' + offset.toString() + 'px;">';
        while (weekDay <= 7 && day <= maxDays) {
                
                
            var additionalClass = (data[day - 1].Status.ID >= 4) ? 'weekend' : (!data[day - 1].Status) ? 'no-data' : '';
            if (data[day - 1].Confirmed == false) {
                additionalClass = additionalClass + " non-confirmed ";
            }
            else {
                additionalClass = additionalClass + " day-confirmed ";
            }
            var standByclass = (data[day - 1].StandBy == true) ? 'employee-stand-by' : '';
                 
            el = el + '<div unselectable="on" id="day' + weekDay.toString() + '_' + weekCounter.toString()
                + '" class="day ' + additionalClass + ' ' + standByclass + ' ' +
                ' easyui-tooltip" title="' + (data[day - 1].Confirmed ? "Подтверждено: " + data[day - 1].Author + ". " : "Не подтверждено. Автор последнего изменения: ") + data[day - 1].Author + ". "
                + "Дата последнего изменения: " + (data[day - 1].LastChanged ? data[day - 1].LastChanged : "нет данных") + ". Комментарий: " +
                data[day - 1].Comment
                + '">'
                + day + '<div unselectable="on" class="day-info">' + data[day - 1].Status.Name
                + '<br/>' + data[day - 1].Office
                + '</div></div>';
            day = day + 1;
            weekDay = weekDay + 1;
        }
        weekCounter++;
        el = el + '</div><div class="clear-float"></div>';
        $('#calendar').append($(el));
    }

    $('#calendar .day')
        .on('mousedown', function () { startID = this.id; doSelection(startID, startID); })
        .on('mouseenter', function () { doSelection(startID, this.id); })
        .on('mouseup', function () {
            $('#changeStatusPopup').dialog('open');
            startID = null;
        });


    $('#popup').removeClass('invisible');
    $('.day').slideDown();

    function getDaysInMonth(m, y) {
        return /8|3|5|10/.test(m) ? 30 : m == 1 ? (!(y % 4) && y % 100) || !(y % 400) ? 29 : 28 : 31;
    }

    function doSelection(start, end) {
        if (startID == null)
            return;
        $('#calendar .day').removeClass('selected');
        var startX = parseInt(start.substr(3, 1));
        var startY = parseInt(start.substr(5.1));

        var endX = parseInt(end.substr(3, 1));
        var endY = parseInt(end.substr(5, 1));

        if (startX > endX) {
            var temp = endX;
            endX = startX;
            startX = temp;
        }

        if (startY > endY) {
            var temp = endY;
            endY = startY;
            startY = temp;
        }

        calendarSelection = { Month: currentMonth, Year: currentYear, Id: translatorID, StartX: startX, StartY: startY, EndX: endX, EndY: endY };
        var counter = 0;
        for (var x = startX; x <= endX; x++)
            for (var y = startY; y <= endY; y++) {
                var id = "day" + x.toString() + "_" + y.toString();
                $('#' + id).addClass('selected');
            }



    }
}
function paddedString(i) {
    var s = i.toString();
    return s.length == 2 ? s : "0" + s;
}


function confirmStatuses() {
    var translatorIDs = getCheckedTranslators();
    updateTranslators(translatorIDs);
    
}

function getCheckedTranslators() {
    var els = $('.confirm-checkbox:checkbox:checked');
    var ids = [];
    var curID = 0;
    for (var i = 0; i < els.length; i++) {
        curID = parseInt($(els[i]).attr('id').substr(10));
        ids.push(curID);
    }
    return ids;
}

function updateTranslators(translatorIDs) {
    var data = { 'ids': translatorIDs };
    $('#progress').show();
    $.ajax({
        type: "POST",
        url: confirmStatusURL,
        datatype: "json",
        traditional: true,
        data: data
    }).then(function () {
        $('#progress').hide();
        refreshWhoWhereSearchResults();
    }, function () {
        $('#progress').hide();
        refreshWhoWhereSearchResults();
        toastr.error("Ошибка", "Произошла ошибка");
    });
}

