var user = {
    init: function () {
        user.resisterEvent();
    },
    resisterEvent: function () {
        $('.trigger').off('click').on('click', function (e) {
            e.preventDefault();
            var btn = $(this);
            var id = btn.data('id');
            $.ajax({
                url: "/Admin/User/ChangeStatus",
                data: { id: id },
                dataType: "Json",
                type: "POST",

                success: function (response) {
                    if (response.status == true) {
                        btn.text('');
                    }
                    else {
                        btn.text('');
                    }
                }
            })
        });
    }

}
user.init();