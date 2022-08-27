function PostRecurringOrIncome(formId, tableId) {
    var form = document.getElementById(formId);
    if ((0.1).toLocaleString().indexOf(",") > 0)
        document.getElementById("amountinput").value = document.getElementById("amountinput").value.replace(/,/g, '.');

    var request = new XMLHttpRequest();

    request.open('POST', form.action, true);
    request.onload = function () {
        if (this.status >= 200 && this.status < 400) {
            var resp = JSON.parse(this.response);
            if (resp.isValid) {
                document.getElementById("form-modal").getElementsByClassName("modal-body")[0].innerHTML = '';
                document.getElementById("form-modal").getElementsByClassName("modal-title")[0].innerHTML = '';

                $('#form-modal').modal('hide');
                if (tableId == 'tableInModalIncome') {
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
            else {
                document.getElementById("form-modal").getElementsByClassName("modal-body")[0].innerHTML = resp.html;
                if (formId === "createRecurringExpenseForm")
                {
                    FormEventListener("New recurring expense");
                    customField();
                }
                else FormEventListener("New income");

            }
        } else {
            alert("Something went wrong, refresh and try again");
        }
    };

    request.onerror = function (err) {
        console.log(err)
    };

    request.send(new FormData(form));
}

DeleteIncome = form => {
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
                        var request = new XMLHttpRequest();
                        request.open('POST', form.action, true);
                        request.onload = function () {
                            if (this.status >= 200 && this.status < 400) {
                                var resp = JSON.parse(this.response);
                                document.getElementById("tableInModalIncome").innerHTML = resp.html;
                                $.notify(
                                    "Refresh to see all changes",
                                    { globalPosition: "top left", clickToHide: true, autoHide: false, className: 'info' }
                                );
                            } else {
                                alert("Something went wrong, refresh and try again");
                            }
                        };

                        request.onerror = function (err) {
                            console.log(err)
                        };

                        request.send(new FormData(form));
                    }
                },
                close: function () {
                    btnClass: 'btn'
                }
            }
        });

    //prevent default form submit event
    return false;
}



DeleteRecurring = form => {
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
                        var request = new XMLHttpRequest();
                        request.open('POST', form.action, true);
                        request.onload = function () {
                            if (this.status >= 200 && this.status < 400) {
                                var resp = JSON.parse(this.response);
                                document.getElementById("tableInModal").innerHTML = resp.html;
                                $.notify(
                                    "Refresh to see all changes",
                                    { globalPosition: "top left", clickToHide: true, autoHide: false, className: 'info' }
                                );
                            } else {
                                alert("Something went wrong, refresh and try again");
                            }
                        };

                        request.onerror = function (err) {
                            console.log(err)
                        };

                        request.send(new FormData(form));
                    }
                },
                confirmAll: {
                    text: 'Delete old too',
                    btnClass: 'btn-danger',
                    action: function () {
                        var formData = new FormData(form);
                        formData.append('oneOrAll', true);

                        var request = new XMLHttpRequest();
                        request.open('POST', form.action, true);
                        request.onload = function () {
                            if (this.status >= 200 && this.status < 400) {
                                var resp = JSON.parse(this.response);
                                document.getElementById("tableInModal").innerHTML = resp.html;
                                $.notify(
                                    "Refresh to see all changes",
                                    { globalPosition: "top left", clickToHide: true, autoHide: false, className: 'info' }
                                );
                            } else {
                                alert("Something went wrong, refresh and try again");
                            }
                        };

                        request.onerror = function (err) {
                            console.log(err)
                        };

                        request.send(formData);
                    }
                },
                close: function () {
                    btnClass: 'btn'
                }
            }
        });

    //prevent default form submit event
    return false;

}


function customField() {
    var customStatus = document.getElementById("customStatus");
    var focusableInput = document.getElementById("focusableInput");
    var statusDayCol = document.getElementById("statusDayCol");
    var howOftenValidationSpan = document.getElementById("howOftenValidationSpan");
    var weekdaysValidationSpan = document.getElementById("weekdaysValidationSpan");

    if (document.getElementById("recurringSelect").value == 4) {
        customStatus.style.visibility = "visible";
        statusDayCol.style.visibility = "visible";
        howOftenValidationSpan.style.visibility = "visible";
        weekdaysValidationSpan.style.visibility = "visible";
        focusableInput.required = true;
    }
    else {
        customStatus.style.visibility = "hidden";
        statusDayCol.style.visibility = "hidden";
        howOftenValidationSpan.style.visibility = "hidden";
        weekdaysValidationSpan.style.visibility = "hidden";
        focusableInput.required = false;
    }
}

function showStartDateTooltip() {
    $('#startDate').tooltip('show');
}