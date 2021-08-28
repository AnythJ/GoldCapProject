
showInPopup = (url, title) => {
    $.ajax({
        type: "GET",
        url: url,
        success: function (res) {
            $("#form-modal .modal-body").html(res);
            $("#form-modal .modal-title").html(title);
            $("#form-modal").modal('show');
        }
    })
};


jQueryAjaxPost = form => {
    try {
        $.ajax({
            type: 'POST',
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (res) {
                if (res.isValid) {
                    $('#view-all').html(res.html)
                    $('#form-modal .modal-body').html('');
                    $('#form-modal .modal-title').html('');
                    $('#form-modal').modal('hide');
                }
                else
                    $('#form-modal .modal-body').html(res.html);
            },
            error: function (err) {
                console.log(err)
            }
        })
        //to prevent default form submit event
        return false;
    } catch (ex) {
        console.log(ex)
    }
}


jQueryAjaxDelete = form => {
    if (confirm('Are you sure to delete this record ?')) {
        try {
            $.ajax({
                type: 'POST',
                url: form.action,
                data: new FormData(form),
                contentType: false,
                processData: false,
                success: function (res) {
                    $('#view-all').html(res.html);
                },
                error: function (err) {
                    console.log(err)
                }
            })
        } catch (ex) {
            console.log(ex)
        }
    }

    //prevent default form submit event
    return false;
}

// HERE TO CHANGE, IT CAN BE IN ONE FUNCTION

    

jQueryAjaxDeleteDashboard = (form, tableId) => { //SEEMS LIKE WORKING FOR NOW
    try {
        $.confirm({
            title: 'Delete Expense',
            content: 'Are you sure you want to delete this expense?',
            type: 'light',
            theme: 'dark',
            icon: 'fa fa-warning',
            animateFromElement: false,
            offsetBottom: 600,
            typeAnimated: true,
            draggable: true,
            buttons: {
                confirm: {
                    text: 'Confirm',
                    btnClass: 'btn-danger',
                    action: function () {
                        $.ajax({
                            type: 'POST',
                            url: form.action,
                            data: new FormData(form),
                            contentType: false,
                            processData: false,
                            success: function (res) {
                                showLineChart();
                                showPieChart();
                                $(tableId).html(res.html);
                            },
                            error: function (err) {
                                console.log(err)
                            }
                        })
                    }
                },
                confirmAll: {
                    text: 'Delete old too',
                    btnClass: 'btn-danger',
                    action: function () {
                        var formData = new FormData(form);
                        formData.append('oneOrAll', true);
                        $.ajax({
                            type: 'POST',
                            url: form.action,
                            data: formData,
                            contentType: false,
                            processData: false,
                            success: function (res) {
                                showLineChart();
                                showPieChart();
                                $(tableId).html(res.html);
                            },
                            error: function (err) {
                                console.log(err)
                            }
                        })
                    }
                },
                close: function () {
                    btnClass: 'btn'
                }
            }
        });
        
    } catch (ex) {
        console.log(ex)
    }

    //prevent default form submit event
    return false;
    
}




jQueryAjaxPostDashboard = (form, tableId) => {
    try {
        $.ajax({
            type: 'POST',
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (res) {
                if (res.isValid) {
                    $(tableId).html(res.html)
                    $('#form-modal .modal-body').html('');
                    $('#form-modal .modal-title').html('');
                    $('#form-modal').modal('hide');
                    showLineChart();
                    showPieChart();
                    if (tableId == '#tableInModalIncome') {
                        showInPopup("/Dashboard/IncomeList", "Income list");
                    }
                    else {
                        showInPopup("/Dashboard/RecurringPayments", "Recurring payments");
                    }
                }
                else
                    $('#form-modal .modal-body').html(res.html);
                if (document.getElementById('recurringSelect').value == 4) {
                    document.getElementById('customStatus').style.visibility = "visible";
                }
                
            },
            error: function (err) {
                console.log(err)
            }
        })
        //to prevent default form submit event
        return false;
    } catch (ex) {
        console.log(ex)
    }
}

JqueryAjaxSort = (url, title, refresh) => {
    var activeElement = document.getElementById('active');
    if (activeElement != null && refresh != true)
        activeElement = activeElement.textContent.replace(/[\n\r]+|[\s]{2,}/g, ' ').trim();
    else activeElement = null;
    var x = $('#idForSort').val();
    $.ajax({
        type: "GET",
        data: {
            'id': $('#idForSort').val(),
            'categoryName': activeElement,
            'period': getParameterByName("period")
        },
        url: url,
        success: function (res) {
            $('#view-all').html(res.html);
            $('#idForSort').val(x);
        }
    })
};


sendToList = (id, categoryName, period) => {
    $.ajax({
        type: "GET",
        data: {
            'id': id,
            'categoryName': categoryName,
            'period': period
        },
        url: '/Dashboard/TooltipSort',
        success: function (res) {
            $('#view-all').html(res.html);
            $('#idForSort').val(id);
        }
    })
};


function getParameterByName(name, url = window.location.href) {
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
}



