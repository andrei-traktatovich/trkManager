google.load("visualization", "1", { packages: ["corechart"] });
$(document).ready(function () {
   
    setToolTip('give-me-help');
    setToolTip('my-requests');
    setToolTip('who-where');
    setToolTip('my-schedule');
    setToolTip('my-analytics');

    toastr.options = {
        "closeButton": false,
        "debug": false,
        "positionClass": "toast-bottom-right",
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };

    // retrieving search settings from local storage
    $('#StrOfficeID').val(localStorage.StrOfficeID);
    $('#StrOfficeID').change(function () { localStorage.StrOfficeID = $(this).val(); });
    $('#StrAuthorName').val(localStorage.StrAuthorName);
    $('#StrAuthorName').change(function () { localStorage.StrAuthorName = $(this).val(); });
    $('#DrDnShowMode').val(localStorage.DrDnShowMode);
    $('#DrDnShowMode').change(function () { localStorage.DrDnShowMode = $(this).val(); });
    $('#ChkOpen').attr('checked', localStorage.ChkOpen);
    $('#ChkOpen').change(function () { localStorage.ChkOpen = $(this).attr('checked'); });
    $('#ChkInProgress').attr('checked', localStorage.ChkInProgress);
    $('#ChkInProgress').change(function () { localStorage.ChkInProgress = $(this).attr('checked'); });
    $('#ChkClosed').attr('checked', localStorage.ChkClosed);
    $('#ChkClosed').change(function () { localStorage.ChkClosed = $(this).attr('checked'); });
    $('#ChkRejected').attr('checked', localStorage.ChkRejected);
    $('#ChkRejected').change(function () { localStorage.ChkRejected = $(this).attr('checked'); });
    $('#TxtStartDate').val(localStorage.TxtStartDate);
    $('#TxtStartDate').change(function () { localStorage.TxtStartDate = $(this).val(); });
    $('#TxtEndDate').val(localStorage.TxtEndDate);
    $('#TxtEndDate').change(function () { localStorage.TxtEndDate = $(this).val(); });
    $('#ChkConfirmed').attr('checked', localStorage.ChkConfirmed);
    $('#ChkConfirmed').change(function () { localStorage.ChkConfirmed = $(this).attr('checked'); });
    
    // if there is no local storage, fall back on defaults
    if (!($('#ChkOpen').attr('checked') || $('#ChkInProgress').attr('checked') || $('#ChkClosed').attr('checked') || $('#ChkRejected').attr('checked')
        || $('#ChkConfirmed').attr('checked'))) {
        $('#ChkOpen').attr('checked', true);
        $('#ChkInProgress').attr('checked', true);
    }

    // setting calendars
    $('.date-picker input').each(function () {
        var c = $(this);
        
            c.datebox('calendar').calendar({ firstDay: 1 });
    });
     
    $('#post-submit').click(function () {
            $(document.forms[0]).validate();
            if ($(document.forms[0]).valid()) {
                showProgressAnimation();
            }
        });

    if (typeof doPageSettings != 'undefined')
        doPageSettings();
    
    if (typeof pageLocalSettings != 'undefined')
        pageLocalSettings();

    updateTicketSearchResults();

});

function showProgressAnimation() {
    $('#progress').show();
}

function toggleVisible(callingEl) {
    var tr = $(callingEl).closest('tr');
    $(tr).toggleClass('big');
    var e = $(tr.next());

    if (e.is(':hidden'))
        e.fadeIn();
    else
        e.fadeOut();
}

function toggleVisibility(el) {

    var e = $('#' + el);
    if (e.is(':hidden'))
        e.fadeIn();
    else
        e.fadeOut();
}

function hideProgressAnimation() {
    $('#progress').hide();
}

function wireUpTicketsTable() {
    $('#popup').dialog({ closed: true });
    

    $("#RequestsGrid tr:odd").addClass("alternating");


    //setToolTip('RequestsGrid th', 'Кликните на заголовок столбца,<br/>чтобы сортировать таблицу по этому столбцу - <br/> по возрастанию или убыванию.');

    /*$('#RequestsGrid').dataTable({
            "bPaginate": false,
            "bLengthChange": false,
            "bFilter": false,
            "bSort": true,
            "bInfo": false,
            "bAutoWidth": false
    }); */
}

function updateTicketSearchResults() {
    
    var data = {
        StrOfficeID: sanitized($("#StrOfficeID").val(), "-1"),
        StrAuthorName: sanitized($("#StrAuthorName").val(), ""),
        DrDnShowMode: sanitized($('#DrDnShowMode').val(),"0"),
        ChkOpen: $('#ChkOpen').attr('checked') == 'checked' ? true : false,
        ChkInProgress: $('#ChkInProgress').attr('checked') == 'checked' ? true : false,
        ChkClosed: $('#ChkClosed').attr('checked') == 'checked' ? true : false,
        ChkRejected: $('#ChkRejected').attr('checked') == 'checked' ? true : false,
        ChkConfirmed: $('#ChkConfirmed').attr('checked') == 'checked' ? true : false,
        TxtStartDate: sanitized($('#TxtStartDate').val(),""),
        TxtEndDate: sanitized($('#TxtEndDate').val(),"")
    };
    
    // this update will run only if ticketsearchresultsrurl, defined in the index view, is defined. Otherwise it will degrade gracefully.
    if (typeof ticketSearchResultsURL != 'undefined') {
        $('#TicketSearchResults').load(ticketSearchResultsURL, data, function (response, status, xhr) {
            if (status == "error") {
                showMessage("Ошибка: " + xhr.status + " " + xhr.statusText + ". Для отображения списка нажмите кнопку ФИЛЬТРОВАТЬ");
            } else {
                wireUpTicketsTable();
            }
        });
    }

    function sanitized(e, defaultvalue) {
        if (!defaultvalue)
            defaultvalue = null;
        return e;
        return e == null ? defaultvalue : e;
    }
}

/* tooltips */

/* this will need to be improved */
function setToolTip(el, text) {

    var tooltip = (!text) ? $('#tooltip-' + el).html() : text;

    $('#' + el).tooltip({
        position: 'bottom',
        content: tooltip,
        showDelay: 1000,
        hideDelay: 100,
        onShow: function () {
            $(this).tooltip('tip').addClass('custom-tooltip');
        },
        onPosition: function () {
            $(this).tooltip('tip').css('left', $(this).offset().left);
            $(this).tooltip('arrow').css('left', 20);

        }
    });
}

function showPopup() {
    $('#popup :submit').click(function () { showProgressAnimation(); });
    $('#popup').dialog({ closed: false, width: 780, height: 800 });
}
function errorMesage() { alert('Error'); }
function ajaxErrorHandler(response, status, xhr) {

    if (status == "error") {

        var msg = "Произошла ошибка: " + xhr.status + " " + xhr.statusText;
        toastr.error(msg);
    }
}

function ajaxErrorMessage() { $('#fulldescription').html('<p>Произошла ошибка</p>'); $('#popup').dialog("open"); }
function hideRow(el) { $(el).hide(); }


// * date-time parser and formatter
function russianFormatter(date) {  
    var y = date.getFullYear();  
    var m = date.getMonth()+1;  
    var d = date.getDate();  
    return (d<10?('0'+d):d) + '.' + (m<10?('0'+m):m) + '.' + y;  
}  

function datetimeParser(s) {
    if (!s) return new Date();
    var ss = (s.split('.'));
    var y = parseInt(ss[2], 10);
    var m = parseInt(ss[1], 10);
    var d = parseInt(ss[0], 10);
    if (!isNaN(y) && !isNaN(m) && !isNaN(d)) {
        return new Date(y, m - 1, d);
    } else {
        return new Date();
    }
}

/* infrastructure */

function ajaxErrorHandler(response, status, xhr) {

    if (status == 'error') {
        showMessage(xhr.status + " " + xhr.statusText);
    }
}

function showMessage(messageText) {
    $.messager.alert('Ошибка', messageText, 'error');

}

function LineChartArea(config) {

    var data = config.data;
    var url = config.sourceURL;
    var id = config.id;
    var chartData = {};
    console.log('input = ');
    console.dir(data);

    getData(url, data);

    function getData(url, data) {
        $.ajax({ url: url, data: data, type: "GET", success: success }).fail(fail);
    }

    function fail(data, textStatus, jqXHR) { console.log('fail'); console.dir(textStatus); }

    function success(data, textStatus, jqXHR) {
        console.log('result = ');
        console.dir(data);
        chartData = google.visualization.arrayToDataTable(data);
        draw(id);
    }

    function draw(id) {

        var lineChart = new google.visualization.LineChart(document.getElementById(id));

        lineChart.draw(chartData, {
             
            width: "100%", height: "100%", curveType: 'none', fontSize: 12, hAxis: { slantedText: true, slantedTextAngle: 90 },
            vAxis: { minValue: 10 }
        });
    };
}


function ChartArea(title, tableId, chartAreaId) {
    this.tableId = tableId;
    this.chartAreaId = chartAreaId;
    this.chartLocation = chartAreaId + '-chart';
    this.title = title;
    var self = this;

    this.createChartArea = function (configObject) { // configObject, is empty, causes default behaviour. Otherwise it allows to specify a different source for the chart. 
        // creating the dropdownlist
        tableId = this.tableId;
        chartAreaId = this.chartAreaId;

        configObject = configObject || { showDropdDown: true, source : []};

        var chartArea = $('#' + chartAreaId);

        chartArea.append('<h1>' + this.title + '</h1>');

        

        var els = $('#' + tableId + " thead tr th");
        this.source = configObject.source;

        if (this.source.length == 0) {
            var dropDown = $('<select/>');
            var counter = 0;
            for (var i = 0; i < els.length; i++) {
                if ($(els[i]).hasClass('charted')) {
                    var optionText = $(els[i]).text();
                    var option = $('<option/>').attr('value', counter++).text(optionText);



                    $(dropDown).append(option);
                    this.source.push({ title: optionText, source: getSourceDataFrom(tableId, i) });
                }
            }
            dropDown.on('change', function () { makeChart($(this).val()); });
            chartArea.append(dropDown);
        }

        function makeChart(option) {
            console.log('option = ' + option.toString());
            console.dir(self.source[option].source);
            drawChart(self.source[option].source, self.source[option].title, self.chartLocation);
        }

        var chartDiv = $('<div id="' + this.chartLocation + '" style="width:100%; height: 83%">'); // this is for all charts, but width and height will differ 
        chartArea.append(chartDiv);
        chartArea.addClass('chart-area');
        makeChart(0);

        function getSourceDataFrom(tableId, tableColNo) { // this is a very specific routine that only works for certain tables. 
            var result = [['1', '2']];
            var rows = $('#' + tableId + ' tbody tr.info');
            var length = rows.length;

            for (var i = 0; i < length; i++) {
                var name = $(rows[i]).children('td:first').text();

                var val = parseInt($(rows[i]).children('td:nth-child(' + (tableColNo + 1) + ')').text().replace(/\s/g, '').replace(',', '.'));
                result.push([name, val]);
            }
            console.dir(result);
            return result;
        }
        function drawChart(source, title, element) {

            var data = google.visualization.arrayToDataTable(source);

            var options = {
                title: title
            };

            var chart = new google.visualization.PieChart(document.getElementById(element));
            chart.draw(data, options);
        }
    }

}
