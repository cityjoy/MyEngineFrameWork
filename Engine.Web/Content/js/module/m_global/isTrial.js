define(function (require, exports, module) {
    var isTrial = {};

    isTrial.doShow = function () {
        var obj = ".col-l-list-ul a,.teaching-catalog a,.col-r-list-1 a,.xx-datum-list a,.xx-datum-tab a,.teaching-detail-other a";
        $(document).on("click",obj,function(e){
            layer.open({
                type: 1,
                title:false,
                skin: 'layui-layer-rim', //加上边框
                area: ['1000px', '340px'], //宽高
                content: $("#tpl_isTrial").html()
            });
            e.preventDefault();
        });
        
        $(document).on("click", ".datum-list-1 table a", function (e) {
            if ($("#tpl_isTrial1").length > 0) {
                layer.open({
                    type: 1,
                    title: false,
                    skin: 'layui-layer-rim', //加上边框
                    area: ['1000px', '340px'], //宽高
                    content: $("#tpl_isTrial1").html()
                });
            }
                e.preventDefault();
            });
       
        layer.open({
            type: 1,
            title: false,
            skin: 'layui-layer-rim', //加上边框
            area: ['1000px', '340px'], //宽高
            content: $("#tpl_isTrial").html()
        });
    };

    isTrial.doOpt = function () {
        if ($("#tpl_isTrial").length > 0) {
            this.doShow();
        }
        
    };
    module.exports = isTrial;
});