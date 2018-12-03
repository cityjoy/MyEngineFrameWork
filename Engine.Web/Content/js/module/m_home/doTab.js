define(function (require, exports, module) {

    var doTab = {};

    doTab.doOpt = function () {
        $(".index_v2-news .title span a").click(function () {
            if ($(this).hasClass("on")) {
                var hUrl = $(this).data("href");
                window.open(hUrl);
            } else {
                var i = $(this).index();
                $(this).addClass("on").siblings("a").removeClass("on");
                $(".index_v2-news").find(".index-news-con").hide();
                $(".index_v2-news").find(".index-news-con").eq(i).show();
            }
            
        });

    };

    module.exports = doTab;
});