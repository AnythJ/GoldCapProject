ListSort = (url, refresh) => {
    try {
        $.ajax({
            type: 'POST',
            url: url,
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

jQueryAjaxDeleteSort = (url) => {
    if (confirm('Are you sure to delete this record ?')) {
        var sortForm = document.getElementById('sortMenuForm');
        try {
            $.ajax({
                type: 'POST',
                url: url,
                data: new FormData(sortForm),
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