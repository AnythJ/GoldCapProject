ListSortCategories = (url, refresh) => {
    try {
        var request = new XMLHttpRequest();
        request.open('GET', url, true);

        request.onload = function () {
            if (this.status >= 200 && this.status < 400) {
                var resp = JSON.parse(this.response);
                document.getElementById("view-all").innerHTML = resp.html;
            }
        };

        request.onerror = function (err) {
            console.log(err)
        };

        request.send();
    } catch (ex) {
        console.log(ex)
    }
};