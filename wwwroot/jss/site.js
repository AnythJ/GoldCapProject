
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

    

jQueryAjaxDeleteDashboard = form => { //SEEMS LIKE WORKING FOR NOW
    try {
        $.confirm({
            title: 'Delete Expense',
            content: 'Are you sure you want to delete this expense?',
            type: 'red',
            icon: 'fa fa-warning',
            animateFromElement: false,
            offsetBottom: 600,
            typeAnimated: true,
            buttons: {
                confirm: {
                    text: 'Confirm',
                    btnClass: 'btn-red',
                    action: function () {
                        $.ajax({
                            type: 'POST',
                            url: form.action,
                            data: new FormData(form),
                            contentType: false,
                            processData: false,
                            success: function (res) {
                                $('#tableInModal').html(res.html);
                            },
                            error: function (err) {
                                console.log(err)
                            }
                        })
                    }
                },
                confirmAll: {
                    text: 'Delete old too',
                    btnClass: 'btn-red',
                    action: function () {
                        $.ajax({
                            type: 'POST',
                            url: form.action,
                            data: new FormData(form),
                            contentType: false,
                            processData: false,
                            success: function (res) {
                                $('#tableInModal').html(res.html);
                            },
                            error: function (err) {
                                console.log(err)
                            }
                        })
                    }
                },
                close: function () {
                }
            }
        });
        
    } catch (ex) {
        console.log(ex)
    }

    //prevent default form submit event
    return false;
    
}

//jQueryAjaxDeleteDashboard = form => {
//    if (confirm('Are you sure to delete this record ?')) {
//        try {
//            $.ajax({
//                type: 'POST',
//                url: form.action,
//                data: new FormData(form),
//                contentType: false,
//                processData: false,
//                success: function (res) {
//                    $('#tableInModal').html(res.html);
//                },
//                error: function (err) {
//                    console.log(err)
//                }
//            })
//        } catch (ex) {
//            console.log(ex)
//        }
//    }

//    //prevent default form submit event
//    return false;
//}



jQueryAjaxPostDashboard = form => {
    try {
        $.ajax({
            type: 'POST',
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (res) {
                if (res.isValid) {
                    $('#tableInModal').html(res.html)
                    $('#form-modal .modal-body').html('');
                    $('#form-modal .modal-title').html('');
                    $('#form-modal').modal('hide');
                    showInPopup("/Dashboard/RecurringPayments", "Recurring payments");
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

JqueryAjaxSort = (url, title) => {
    var x = $('#idForSort').val();
    $.ajax({
        type: "GET",
        data: {'id': $('#idForSort').val()},
        url: url,
        success: function (res) {
            $('#view-all').html(res.html);
            $('#idForSort').val(x);
        }
    })
};


sendToList = (id) => {
    $.ajax({
        type: "GET",
        data: { 'id': id },
        url: '/Dashboard/TooltipSort',
        success: function (res) {
            $('#view-all').html(res.html);
            $('#idForSort').val(id);
        }
    })
};




