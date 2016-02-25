

function JobsCalendar(globalsUrl, calendarGetUrl, calendarUpdateUrl) {

    // loading the globals
    this.globalsUrl = globalsUrl;
    this.calendarGetUrl = calendarGetUrl;
    this.calendarUpdateUrl = calendarUpdateUrl;

    this.globals = getGlobals(globalsUrl);

    function getGlobals(url) {
        return $.get(url, function (data) { return data; }); // this assumes that the load is successful. 
    }

    this.getCalendar = function (filterParameters) {
    };

    this.selection = [];

    this.updateCalendar = function () {
    };

}