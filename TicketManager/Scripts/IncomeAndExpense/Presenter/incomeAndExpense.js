
function doPageSettings() {
    

    
        $('document').ready(function () {

            var ajaxService = new AjaxService();
            var myUtils = {};
            var viewModel = {};
            var data = {};
            moment.lang("ru");

            $.ajaxSetup({
                // Disable caching of AJAX responses
                cache: false
            });


            ajaxService.load(getContextURL, data, {}, contextLoadSuccess);

            function contextLoadSuccess(result) {
                console.log('got context as ');
                console.dir(result);
                var context = ko.mapping.fromJS(result);
                myUtils = new Utils(context);
                viewModel = new ViewModel(result, myUtils);
                ko.applyBindings(viewModel);
                viewModel.loadData();
            }
        });
    

}