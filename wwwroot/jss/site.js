showInPopup = (url, title) => {
    if (title == "Recurring payments") document.getElementById("modalDialog").classList.add("modal-lg")
    else document.getElementById("modalDialog").classList.remove("modal-lg");
    
    var el = document.getElementsByClassName("modalLoadingSpinner")[0];
    el.style.display = "block";
    var request = new XMLHttpRequest();
    request.open('GET', url, true);

    request.onload = function () {
        if (this.status >= 200 && this.status < 400) {
            var resp = this.response;
            document.getElementById("form-modal").getElementsByClassName("modal-body")[0].innerHTML = resp;
            document.getElementById("form-modal").getElementsByClassName("modal-title")[0].innerHTML = title;

            FormEventListener(title);

            el.classList.add("hidden");
            el.addEventListener("animationend", (event) => {
                if (event.type === "animationend") {
                    el.style.display = "none";
                    $("#form-modal").modal('show');
                }
            }, false);
        }
    };

    request.onerror = function (err) {
        console.log(err)
    };

    request.send();
};

FormEventListener = (title) => {
    console.log(title);
    switch (title) {
        case "New expense":
            document.getElementById("createExpenseForm").addEventListener("keypress", (event) => {
                if (event.keyCode === 13) {
                    document.getElementById("submitButton").click();
                }
            });
            break;
        case "Update expense":
            document.getElementById("createExpenseForm").addEventListener("keypress", (event) => {
                if (event.keyCode === 13) {
                    document.getElementById("submitButton").click();
                }
            });
            break;
        case "New recurring expense":
            document.getElementById("createRecurringExpenseForm").addEventListener("keypress", (event) => {
                if (event.keyCode === 13) {
                    document.getElementById("submitButton").click();
                }
            });
            break;
        case "New income":
            document.getElementById("createIncomeForm").addEventListener("keypress", (event) => {
                if (event.keyCode === 13) {
                    document.getElementById("submitButton").click();
                }
            });
            break;
        default:
            break;
    }
};

RequestSort = (url, title, refresh) => {
    if (refresh == true) sessionStorage.setItem("filtered", "false");

    var activeElement = document.getElementById('active');
    if (activeElement != null && refresh != true)
        activeElement = activeElement.textContent.replace(/[\n\r]+|[\s]{2,}/g, ' ').trim();
    else activeElement = null;

    var x = document.getElementById("idForSort").value;

    

    const params = new URLSearchParams({
        "id": x,
        "categoryName": activeElement,
        "period": getParameterByName("period")
    });

    if (refresh) {
        params.set("id", -1);
    }

    if (activeElement == null) params.delete("categoryName");

    var request = new XMLHttpRequest();
    request.open('POST', url, true);
    
    request.onload = function () {
        if (this.status >= 200 && this.status < 400) {
            var resp = JSON.parse(this.response);
            document.getElementById("view-all").innerHTML = resp.html;
            document.getElementById("idForSort").value = x;
        }
    };

    request.onerror = function (err) {
        console.log(err)
    };

    request.send(params);
};


sendToList = (id, categoryName, period) => {
    const params = new URLSearchParams({
        "id": id,
        "categoryName": categoryName,
        "period": period
    });

    if (categoryName == null) params.delete("categoryName");

    var request = new XMLHttpRequest();
    request.open('POST', '/Dashboard/TooltipSort', true);

    request.onload = function () {
        if (this.status >= 200 && this.status < 400) {
            var resp = JSON.parse(this.response);
            document.getElementById("view-all").innerHTML = resp.html;
            document.getElementById("idForSort").value = id;
        }
    };

    request.onerror = function (err) {
        console.log(err)
    };

    request.send(params);
};


function getParameterByName(name, url = window.location.href) {
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
}






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
            links[i].style.animation = "show-links 0.4s forwards";
            links[i].style.opacity = "0";
        }
        if (window.location.href.toUpperCase().indexOf("/DASHBOARD") != -1) {
            setTimeout(function () { reloadAreaChart(); }, 150);
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
            setTimeout(function () { reloadAreaChart(); }, 150);
        }
        sessionStorage.setItem("sideNavbarCollapsed", false);
    }
}

function showPage() {
    let period;
    var el = document.getElementsByClassName("loadingBackground")[0];
    el.classList.add("hidden");
    el.addEventListener("animationend", (event) => {
        if (event.type === "animationend") {
            el.style.display = "none";
            document.getElementsByTagName("main")[0].style.display = "block";
        }
    }, false);
}

function notificationBoxSlide() {
    var container = document.getElementById('notificationsListBox');
    var containerStyle = container.CurrentStyle || window.getComputedStyle(container);
    if (containerStyle.visibility == "hidden") {
        container.style.transform = "translateX(0)";
        container.style.visibility = "visible";
        container.style.opacity = "1";
        container.style.width = "25rem";
    }
    else {
        container.style.transform = "translateX(100vw)";
        container.style.visibility = "hidden";
        container.style.opacity = "0";
        container.style.width = "0rem";
    }
}





