function AjaxService() {
    
    $.ajaxSetup({ "cache" : false });

    return { load: load, post : post };

    function post(url, data, success, fail) {
        showProgressAnimation();
        //console.log('post hit ' + url);
        //console.log('data = ');
        //console.dir(data);
        //console.log(JSON.stringify(data));

        $.ajax({
            type: "POST",
            
            url: url,
            contentType: 'application/json; charset=utf-8',
            //dataType: 'json',
             
            data: JSON.stringify(data),
            traditional: true,
            success: function(d) {
                if (d.success === undefined || d.success == true)
                    ok(d);
                else { if (fail) fail(); }
            },
            error: function (xhr, textStatus, errorThrown) {
                errorMessage(textStatus, errorThrown);
            }
        });

        function ok(d) {
            console.log('post successful');
            hideProgressAnimation();
            success(d);
        }

        function errorMessage(msg, errorThrown) {
            if (msg)
                console.log(msg);
            if (errorThrown)
                console.log(errorThrown);
            toastr.error('Ошибка: ' + msg ? msg : '' + '. ' + errorThrown ? errorThrown : '');
            hideProgressAnimation();
            if (fail)
                fail();
        }
    }


    function load(url, resultingData, data, success, fail) {
        showProgressAnimation();

        $.ajax({
            url: url,
            data: data,
            success: ok,
            fail : fail,
            cache: false
        });
        //$.get(url, data, ok).fail(fail);

        function ok(result) {
            hideProgressAnimation();
            success(result);
        }

        function fail(result) {
            hideProgressAnimation();
            toastr.error('Произошла ошибка: ' + result.message);
        }
    }
}