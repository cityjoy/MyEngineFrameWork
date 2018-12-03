$(function() {

    var btn = $("#Button1");

    btn.cropperUpload({
        url: "WebForm1.aspx",
        fileSuffixs: ["jpg", "png", "bmp","gif"],
        errorText: "{0}",
        onComplete: function(msg) {
            $("#testimg").attr("src", msg);
        },
        cropperParam: { //Jcrop参数设置，除onChange和onSelect不要使用，其他属性都可用  
            maxSize: [350, 350], //不要小于50，如maxSize:[40,24]  
            minSize: [100, 100], //不要小于50，如minSize:[40,24]  
            bgColor: "#000000",
            bgOpacity: 0.4,
            allowResize: true,
            allowSelect: false,
            animationDelay: 50,
            aspectRatio : 1/1,
            bgFade :true

        },
        perviewImageElementId: "fileList", //设置预览图片的元素id    
        perviewImgStyle: {
            width: '350px',
            height: '350px'
        } //设置预览图片的样式
    });
    var upload = btn.data("uploadFileData");

    $("#files").click(function() {
        upload.submitUpload();
    });

});