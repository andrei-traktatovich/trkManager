
function PaginatedSearch(url, searchParameters) {

	this.url = url;								// url of controller to be used to retrieve data
	this.searchParameters = searchParameters;	
	this.totalPages = 0;						// total number of pages returned after instantiation of paginated search
	this.currentPage = {};						// copy of current page to be used 
	this.currentPageNo = 0;						// current page number (starting with 0!)
	
	var service = new HTTPService(), self = this;
	self.pages = [];
	
	this.start = function(vm, id, mapping) {
	    console.log('trying to retrieve 1st page');
	    console.log('model = ');
	    console.dir(vm);
	    self.id = id;
	    self.vm = vm;
	    self.mapping = mapping;
	    _getPage(0, true, this.finalizeStart);
	};

	this.finalizeStart = function finalizeStart() {
	    console.log('finalize start');
	    console.log('initial viewmodel = '); console.dir(self.vm);
	    console.log('mapping = '); console.dir(self.mapping);
	    console.log('current page = '); console.dir(self.currentPage);
	    var r = { currentPage: self.currentPage };
	    //ko.mapping.fromJS(self.currentPage, self.mapping, self.vm);
	    //self.vm.currentPage(ko.mapping.fromJS(self.currentPage, self.mapping));
	    ko.mapping.fromJS(r, self.mapping, self.vm);
	    
	    console.log('applying bindings');
	    
	    console.log('final vm= '); console.dir(self.vm);
	    console.log('unwrapped'); console.dir(ko.mapping.toJS(self.vm.currentPage));

	    // self.vm.currentPage(self.currentPage);
	    console.log('current page as applied to model is ');
	    console.dir(self.currentPage);

	    console.log('total pages = ' + self.totalPages);

	    if (self.totalPages > 1) {
	        var options = {
	            currentPage: 1,
	            totalPages: self.totalPages,
	            bootstrapMajorVersion: 1,
	            onPageClicked: updatePage
	        };
	        console.log('initialising paginator');
	        $('#' + self.id).bootstrapPaginator(options);
	        $('#' + self.id).show();
	    }
	    else {
	        console.log('need not init paginator at ' + self.id);
	        $('#' + self.id).hide();
	    }

	    function updatePage(e, originalEvent, type, page) {
	        console.log('entering update page for page = ' + page);
	        _getPage(page - 1, false, finalize);

	        function finalize() {
	            console.log('finalizing get new page');
	            var r = { currentPage: self.currentPage };
	            ko.mapping.fromJS(r, self.mapping, self.vm);
	        }
	    }
	};
	
	
	/*
	function FHTTPService() {
		this.getData = function(url,data, callBack) {
			console.log('entering getdata');
			var result = {};
			var p = data.page;
			if(data.getCount)
			{
				result = {
					pageCount : 5,
					page: [{ name: 'ivanov', grade: p},{ name: 'ivanov', grade: p},{ name: 'ivanov', grade: p},{ name: 'ivanov', grade: p}]
				};
			}
			else
				result = {
					page: [{ name: 'ivanov', grade: p},{ name: 'ivanov', grade: p},{ name: 'ivanov', grade: p},{ name: 'ivanov', grade: p}]
				}
			console.dir(result);
			callBack(result);
		};
	}
	*/

	self.success = function success(data, callback) {
	    console.log('entering success');
	    console.dir(data);
	    if (data.totalPages)
	        self.totalPages = data.totalPages;

	    self.currentPage = data.page;

	    console.log('currentpage = ');
	    console.dir(self.currentPage);
	    self.pages[self.currentPageNo] = self.currentPage;
	    if (typeof callback == 'function')
	        callback();
	};
	
	function _getPage(pageNo, withCount, callback) {
		
		self.currentPageNo = pageNo || 0;
		console.log('enter _getPage ' + self.currentPageNo);
		if (self.pages[pageNo]) {
		    self.currentPage = self.pages[pageNo];
		    console.log('getting page ' + pageNo + ' from cache');
		    if (typeof callback == 'function')
		        callback();
		}
		else {
			console.log('getting page ' + pageNo + ' from service');
			console.log('search params = ');
			console.dir(searchParameters);
			var data = {
				getCount : withCount || false,
				searchParameters: self.searchParameters,
				page : pageNo || 0
			};
			console.log('parameters being passed = ');
			console.dir(data);
			service.getData(self.url, data, function (data) { self.success(data, callback); }, true);

			
		}
		
	}
	
	this.getPage = function(pageNo) {
		
		pageNo--;							// internally, we start pagination from 0, but paginator plugin starts with 1!
		_getPage(pageNo, false, finalize);

		function finalize() {
		    return self.currentPage;
		}
	}
}