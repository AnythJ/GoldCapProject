AjaxPostRecurringOrIncome = (form, tableId) => {
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
                    if (tableId == '#tableInModalIncome') {
                        showInPopup("/Dashboard/IncomeList", "Income list");
                    }
                    else {
                        showInPopup("/Dashboard/RecurringPayments", "Recurring payments");
                    }

                    $.notify(
                        "Refresh to see all changes",
                        { globalPosition: "top left", clickToHide: true, autoHide: false, className: 'info' }
                    );
                }
                else
                    $('#form-modal .modal-body').html(res.html);
                if (document.getElementById('recurringSelect') != null && document.getElementById('recurringSelect').value == 4) {
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

AjaxDeleteIncome = form => {
    try {
        $.confirm({
            title: 'Delete Income',
            content: 'Are you sure you want to delete this income?',
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
                                $('#tableInModalIncome').html(res.html);
                                $.notify(
                                    "Refresh to see all changes",
                                    { globalPosition: "top left", clickToHide: true, autoHide: false, className: 'info' }
                                );
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



AjaxDeleteRecurring = form => {
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
                                $('#tableInModal').html(res.html);
                                $.notify(
                                    "Refresh to see all changes",
                                    { globalPosition: "top left", clickToHide: true, autoHide: false, className: 'info' }
                                );
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
                                $('#tableInModal').html(res.html);
                                $.notify(
                                    "Refresh to see all changes",
                                    { globalPosition: "top left", clickToHide: true, autoHide: false, className: 'info' }
                                );
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