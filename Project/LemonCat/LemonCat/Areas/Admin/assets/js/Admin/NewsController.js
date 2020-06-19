var RootComment_News = {
    init: function () {
        RootComment_News.resisterEvent();
    },
    resisterEvent: function () {
        $('.triggerRootComment_News').off('click').on('click', function (e) {
            e.preventDefault();
            var btn = $(this);
            var id = btn.data('id');
            $.ajax({
                url: "/Admin/News/ChangeStatusRoot",
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
var ChildComment_News = {
    init: function () {
        ChildComment_News.resisterEvent();
    },
    resisterEvent: function () {
        $('.triggerChildComment_News').off('click').on('click', function (e) {
            e.preventDefault();
            var btn = $(this);
            var id = btn.data('id');
            $.ajax({
                url: "/Admin/News/ChangeStatusChild",
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
var LikeRootNews = {
    init: function () {
        LikeRootNews.resisterEvent();
    },
    resisterEvent: function () {
        $('.triggerLikeRootNews').off('click').on('click', function (e) {
            e.preventDefault();
            var btn = $(this);
            var id = btn.data('id');
            $.ajax({
                url: "/Admin/News/LikeRoot",
                data: { id: id },
                dataType: "Json",
                type: "POST",

                success: function (response) {
                    if (response.status == true) {
                        btn.html("<span class='btn btn-danger btn-round'><i class='zmdi zmdi - hc - fw'></i></span>");
                    }
                    else {
                        btn.html("<span class='btn btn-success btn-round'><i class='zmdi zmdi - hc - fw'></i></span>");
                    }
                    
                }
            })
        });
    }
}
var NewsStatus = {
    init: function () {
        NewsStatus.resisterEvent();
    },
    resisterEvent: function () {
        $('.triggerNewsStatus').off('click').on('click', function (e) {
            e.preventDefault();
            var btn = $(this);
            var id = btn.data('id');
            $.ajax({
                url: "/Admin/News/ChangeStatusNews",
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
var LikeNews = {
    init: function () {
        LikeNews.resisterEvent();
    },
    resisterEvent: function () {
        $('.triggerLikeNews').off('click').on('click', function (e) {
            e.preventDefault();
            var btn = $(this);
            var id = btn.data('id');
            $.ajax({
                url: "/Admin/News/LikeNews",
                data: { id: id },
                dataType: "Json",
                type: "POST",

                success: function (response) {
                    if (response.status == true) {
                        btn.html("<span class='btn btn-danger btn-round'><i class='zmdi zmdi - hc - fw'></i></span>");
                    }
                    else {
                        btn.html("<span class='btn btn-success btn-round'><i class='zmdi zmdi - hc - fw'></i></span>");
                    }
                    $("#Mark_" + id).html(response.mark);
                }
            })
        });
    }
}
LikeNews.init();
NewsStatus.init();
LikeRootNews.init();
ChildComment_News.init();
RootComment_News.init();