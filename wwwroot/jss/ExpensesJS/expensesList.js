ExpenseListSort = (url, title, refresh) => {
    var form = document.getElementById('sortMenuForm');
    if (sessionStorage.getItem("filtered") == "true") {
        url = url + "&filtered=True";
    }

    var sortOrder;
    if (url.indexOf("sortOrder=Date") != -1) sortOrder = 'Date';
    if (url.indexOf("sortOrder=date_desc") != -1) sortOrder = 'date_desc';
    if (url.indexOf("sortOrder=Amount") != -1) sortOrder = 'Amount';
    if (url.indexOf("sortOrder=amount_desc") != -1) sortOrder = 'amount_desc';
    if (url.indexOf("sortOrder=Category") != -1) sortOrder = 'Category';
    if (url.indexOf("sortOrder=category_desc") != -1) sortOrder = 'category_desc';
    sessionStorage.setItem("sortOrder", sortOrder);
    try {
        $.ajax({
            type: 'POST',
            url: url,
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
        return false;
    } catch (ex) {
        console.log(ex)
    }
};
PostSortMenu = form => {
    let url = form.action;
    if (sessionStorage.getItem("sortOrder") != null) url = url + "?&sortOrder=" + sessionStorage.getItem("sortOrder");

    console.log(url);
    try {
        $.ajax({
            type: 'POST',
            url: url + "&filtered=True",
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (res) {
                $('#view-all').html(res.html);
                sessionStorage.setItem("filtered", "true");
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
AjaxDeleteSort = (url) => {
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
                        if (sessionStorage.getItem("sortOrder") != null) url = url + "?&sortOrder=" + sessionStorage.getItem("sortOrder");
                        var sortForm = document.getElementById('sortMenuForm');
                        $.ajax({
                            type: 'POST',
                            url: url,
                            data: new FormData(sortForm),
                            contentType: false,
                            processData: false,
                            success: function (res) {
                                $('#view-all').html(res.html);
                                $.notify(
                                    "Expense deleted",
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


function sortCollapseFunction() {
    var sortParagraphs = document.getElementsByClassName("sort-paragraph");
    var sortHeader = document.getElementsByClassName("sort-header")[0];
    var currencyIcons = document.getElementsByClassName("currency");
    document.getElementById("sortCollapseButton").style.transformOrigin = "25% 50%";
    document.getElementById("sortCollapseButton").style.left = "right";
    var sortMenuClass = document.getElementsByClassName("sortMenu")[0];
    var sortMenu = document.getElementById("sortMenu");
    if (sessionStorage.getItem("sortMenuActive") == "false") {
        sortMenu.classList.add("sortMenuCollapsed");
        document.getElementById("listCol").style.display = "none";
        document.getElementById("sortCollapseButtonInside").classList.remove("sortCollapseNotActive");
        document.getElementById("sortCollapseButtonInside").classList.add("sortCollapseActive");
        sessionStorage.setItem("sortMenuActive", "true");
    }
    else {
        document.getElementById("sortCollapseButtonInside").classList.add("sortCollapseNotActive");
        document.getElementById("sortCollapseButtonInside").classList.remove("sortCollapseActive");
        sortMenu.classList.remove("sortMenuCollapsed");
        document.getElementById("listCol").style.display = "block";
        sessionStorage.setItem("sortMenuActive", "false");
    }
};


function showMore() {
    var x = $('#showMore').text();
    var listLen = document.getElementById('categoryList').getElementsByTagName("li").length;
    if (x == 'Show More') {
        $('#categoryList li:hidden').slice(0, listLen).show();
        $('#showMore').innerHtml == 'Show Less';
        document.getElementById('showMore').innerHTML = 'Show Less';
        if (document.getElementById('showArrow')) {
            document.getElementById('showArrow').classList.remove('fa-chevron-down');
            document.getElementById('showArrow').classList.add('fa-chevron-up');
        }

        sessionStorage.setItem('showButton', 'Show More');
    }
    else if (x == 'Show Less') {
        $('#categoryList li').slice(7, listLen).hide();
        $('#showMore').innerHtml == 'Show More';
        if (document.getElementById('showArrow')) {
            document.getElementById('showArrow').classList.add('fa-chevron-down');
            document.getElementById('showArrow').classList.remove('fa-chevron-up');
        }
        document.getElementById('showMore').innerHTML = 'Show More';

        sessionStorage.setItem('showButton', 'Show Less');
    }
};

AjaxPostExpense = (form, url) => {

    if (sessionStorage.getItem("filtered") == "true") {
        url = url + "&filtered=True";
    }

    if (sessionStorage.getItem("sortOrder") != null) url = url + "?&sortOrder=" + sessionStorage.getItem("sortOrder");


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
                                $('#form-modal .modal-body').html('');
                                $('#form-modal .modal-title').html('');
                                $('#form-modal').modal('hide');
                                $('#sortMenuForm').submit();
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