var review = {
    init: function () {
        review.resisterEvent();
    },
    resisterEvent: function () {
        $('.triggerReview').off('click').on('click', function (e) {
            e.preventDefault();
            var btn = $(this);
            var id = btn.data('id');
            $.ajax({
                url: "/Admin/FlimReviewComment/ChangeStatus",
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
review.init();