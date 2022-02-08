/*/*const { concat } = require("core-js/library/js/array");*/

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

jQueryAjaxDeleteIncome = form => { //SEEMS LIKE WORKING FOR NOW
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

JqueryAjaxSort = (url, title, refresh) => {
    if (refresh == true) sessionStorage.setItem("filtered", "false");

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



jQueryAjaxPostSort = (form, url) => {

    if (sessionStorage.getItem("filtered") == "true") {
        url = url + "&filtered=True";
    }


    try {
        $.ajax({
            type: 'POST',
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (res) {
                if (res.isValid) {
                    var formSort = document.getElementById('sortMenuForm');
                    try {
                        $.ajax({
                            type: 'POST',
                            url: url,
                            data: new FormData(formSort),
                            contentType: false,
                            processData: false,
                            success: function (res) {
                                $('#view-all').html(res.html);
                                $('#form-modal .modal-body').html('');
                                $('#form-modal .modal-title').html('');
                                $('#form-modal').modal('hide');
                            },
                            error: function (err) {
                                console.log(err)
                            }
                        })
                        return false;
                    } catch (ex) {
                        console.log(ex)
                    }
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




function showMore() {
    var x = $('#showMore').text();
    var listLen = document.getElementById('categoryList').getElementsByTagName("li").length;
    if (x == 'Show More') {
        $('#categoryList li:hidden').slice(0, listLen).show();
        $('#showMore').innerHtml == 'Show Less';
        document.getElementById('showMore').innerHTML = 'Show Less';
        document.getElementById('showArrow').classList.remove('fa-chevron-down');
        document.getElementById('showArrow').classList.add('fa-chevron-up');
        sessionStorage.setItem('showButton', 'Show More');
    }
    else if (x == 'Show Less') {
        $('#categoryList li').slice(7, listLen).hide();
        $('#showMore').innerHtml == 'Show More';
        document.getElementById('showMore').innerHTML = 'Show More';
        document.getElementById('showArrow').classList.add('fa-chevron-down');
        document.getElementById('showArrow').classList.remove('fa-chevron-up');
        sessionStorage.setItem('showButton', 'Show Less');
    }
};

function changeColorTheme(color) {
    if (color == 'first') {
        document.documentElement.style.setProperty('--bg-primary', '#212529');
        document.documentElement.style.setProperty('--bg-secondary', '#2D3339');
        document.documentElement.style.setProperty('--bg-third', '#0DCAF0');
        document.documentElement.style.setProperty('--text-primary', '#434D56');
        document.documentElement.style.setProperty('--text-secondary', '#9ba6b0');
        document.documentElement.style.setProperty('--text-third', 'white');
        document.documentElement.style.setProperty('--piechart-border-color', '#9ba6b0');
        document.documentElement.style.setProperty('--scrollbar-primary', '#0790ab');
        document.documentElement.style.setProperty('--btn-succes', '#1B998B');
        document.documentElement.style.setProperty('--btn-succes-hover', '#136c62');
        document.documentElement.style.setProperty('--btn-info', '#87BCDE');
        document.documentElement.style.setProperty('--btn-info-hover', '#5fa7d3');
        document.documentElement.style.setProperty('--btn-danger', '#DF2935');
        document.documentElement.style.setProperty('--btn-danger-hover', '#c81e29');
        sessionStorage.setItem("colorTheme", "first");
    }
    if (color == 'second') {
        document.documentElement.style.setProperty('--bg-primary', '#03071e');
        document.documentElement.style.setProperty('--bg-secondary', '#370617');
        document.documentElement.style.setProperty('--bg-third', '#dc2f02');
        document.documentElement.style.setProperty('--text-primary', '#6a040f');
        document.documentElement.style.setProperty('--text-secondary', '#faa307');
        document.documentElement.style.setProperty('--text-third', '#faa307');
        document.documentElement.style.setProperty('--piechart-border-color', '#faa307');
        document.documentElement.style.setProperty('--scrollbar-primary', '#dc2f02');
        document.documentElement.style.setProperty('--btn-succes', '#82D173');
        document.documentElement.style.setProperty('--btn-succes-hover', '#65c752');
        document.documentElement.style.setProperty('--btn-info', '#7EBDC2');
        document.documentElement.style.setProperty('--btn-info-hover', '#63b0b6');
        document.documentElement.style.setProperty('--btn-danger', '#EF6461');
        document.documentElement.style.setProperty('--btn-danger-hover', '#ed4845');
        sessionStorage.setItem("colorTheme", "second");
    }
    if (color == 'third') {
        document.documentElement.style.setProperty('--bg-primary', '#2F3C7E');
        document.documentElement.style.setProperty('--bg-secondary', '#3e50a8');
        document.documentElement.style.setProperty('--bg-third', '#CF4D6F');
        document.documentElement.style.setProperty('--text-primary', '#6290C8');
        document.documentElement.style.setProperty('--text-secondary', '#FBEAEB');
        document.documentElement.style.setProperty('--text-third', 'white');
        document.documentElement.style.setProperty('--piechart-border-color', '#2F3C7E');
        document.documentElement.style.setProperty('--scrollbar-primary', '#CF4D6F');
        document.documentElement.style.setProperty('--btn-succes', '#82D173');
        document.documentElement.style.setProperty('--btn-succes-hover', '#65c752');
        document.documentElement.style.setProperty('--btn-info', '#5998C5');
        document.documentElement.style.setProperty('--btn-info-hover', '#63b0b6');
        document.documentElement.style.setProperty('--btn-danger', '#CA3C25');
        document.documentElement.style.setProperty('--btn-danger-hover', '#ed4845');
        sessionStorage.setItem("colorTheme", "third");
    }
}

function collapseSideNavbar() {
    var sideNavbarSize = document.getElementsByClassName("navbar-side")[0].offsetWidth;
    if (sideNavbarSize > 80) {
        document.getElementById("logo-icon").style.display = "block";
        document.getElementsByClassName("navbar-side")[0].style.width = "5rem";
        document.getElementsByClassName("navbar-top")[0].style.marginLeft = "5rem";
        document.getElementsByTagName("main")[0].style.marginLeft = "5rem";
        document.getElementById("collapseSideNavbar").style.transform = "rotate(180deg)";
        document.getElementsByClassName("navbar-top")[0].style.width = "calc(100% - 5rem)";
        sessionStorage.setItem("sideNavbarCollapsed", true);
        var links = document.getElementsByClassName("link-text");
        for (var i = 0; i < links.length; i++) {
            links[i].style.display = "none";
        }
        if (window.location.href.toUpperCase().indexOf("/DASHBOARD") != -1) {
            showLineChart();
            showPieChart();
        }
    }
    else {
        document.getElementById("logo-icon").style.display = "none";
        document.getElementsByClassName("navbar-side")[0].style.width = "14rem";
        document.getElementsByClassName("navbar-top")[0].style.marginLeft = "14rem";
        document.getElementsByTagName("main")[0].style.marginLeft = "14rem";
        document.getElementById("collapseSideNavbar").style.transform = "rotate(0deg)";
        document.getElementsByClassName("navbar-top")[0].style.width = "calc(100% - 14rem)";
        var links = document.getElementsByClassName("link-text");
        for (var i = 0; i < links.length; i++) {
            links[i].style.display = "inline";
        }
        if (window.location.href.toUpperCase().indexOf("/DASHBOARD") != -1) {
            showLineChart();
            showPieChart();
        }
        sessionStorage.setItem("sideNavbarCollapsed", false);
    }
}

function showPage() {
    var el = document.getElementsByClassName("loadingBackground")[0];
    el.classList.add("hidden");
    el.addEventListener("animationend", (event) => {
        if (event.type === "animationend") {
            el.style.display = "none";
            document.getElementsByTagName("main")[0].style.display = "block";
        }
    }, false);
}