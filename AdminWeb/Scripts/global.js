//juicer组件注册
if (typeof (juicer) != 'undefined') {
    $(function () {
        //自定义juicer的操作符避免与Razor语法的冲突
        juicer.set({
            'tag::operationOpen': '{$',
            'tag::operationClose': '}',
            'tag::interpolateOpen': '${',
            'tag::interpolateClose': '}',
            'tag::noneencodeOpen': '$${',
            'tag::noneencodeClose': '}',
            'tag::commentOpen': '{#',
            'tag::commentClose': '}'
        });
    });

    juicer.register('JsonToString', JSON.stringify);
    juicer.register('StringToJson', $.parseJSON);
    juicer.register('TagKeywords', lib51.tagKeywords);
    juicer.register('GetDateTime', lib51.getDateTime);
    juicer.register('GetDateTimeString', lib51.getDateTimeString);
    juicer.register('IsWebImage', IsWebImage);
}
/*
确认操作框
*/
function ConfirmAction(message, action, okStr, cancelStr) {
    okStr = okStr || '确定';
    cancelStr = cancelStr || '取消';

    art.dialog({
        id: 'CinfirmActionDialog',
        title: '确认操作',
        content: message,
        okVal: okStr,
        cancelVal: cancelStr,
        ok: function () {
            action();
            return true;
        },
        cancel: function () {
            return true;
        }
    });
}

/*
显示加载条
*/
function ShowLoading() {
    var loading = $('#loading');
    loading.show();

    lib51.fixObjectMiddlePosition('loading');
}
/*
隐藏加载条
*/
function CloseLoading() {
    var loading = $('#loading');
    loading.hide();
}

/*
显示成功提示
*/
function ShowSuccess(message, callback) {
    if (lib51.isSet(art.dialog.list["ShowMessageDialog"])) {
        art.dialog.list["ShowMessageDialog"].close();
    }
    $('#SuccessMessageBox .content').html(message);
    if (typeof (callback) == 'function') {
        art.dialog({
            id: 'ShowMessageDialog',
            title: '提示消息',
            content: document.getElementById('SuccessMessageBox'),
            fixed: true,
            lock: true,
            background: '#CCC', // 背景色
            opacity: 0.77,	// 透明度
            okVal: '确定',
            ok: function(){
                callback();
                return true;
            },
            close: function () {
                $('#SuccessMessageBox .content').html('');
            }
        });
    }
    else {
        art.dialog({
            id: 'ShowMessageDialog',
            title: '提示消息',
            content: document.getElementById('SuccessMessageBox'),
            fixed: true,
            time: 3,
            close: function () {
                $('#SuccessMessageBox .content').html('');
            }
        });
    }
}
/*
显示失败提示
*/
function ShowAlert(message) {
    if (lib51.isSet(art.dialog.list["ShowMessageDialog"])) {
        art.dialog.list["ShowMessageDialog"].close();
    }
    $('#AlertMessageBox .content').html(message);
    art.dialog({
        id: 'ShowMessageDialog',
        title: '提示消息',
        fixed: true,
        time: 5,
        content: document.getElementById('AlertMessageBox'),
        close: function () {
            $('#AlertMessageBox .content').html('');
        }
    });
}


/*
jQuery ajax GET请求封装，带加载提示等
@url[string] 请求地址
@data[json] 发送的数据
@callback[function] 请求成功后的回调方法
@errorCallback[function] 请求出错的回调方法。
*/
function AjaxQuery(url, type, data, callback, errorCallback) {
    if (!lib51.isFunc(callback)) {
        callback = function (jsonData) {
            var result = parseInt(jsonData['Result']);
            var message = jsonData['Message'];
            if (result > 0) {
                if (lib51.isSet(message)) {
                    ShowSuccess(message);
                }
            }
            else {
                if (lib51.isSet(message)) {
                    ShowAlert(message);
                }
            }
        }
    }
    if (!lib51.isFunc(errorCallback)) {
        errorCallback = function (jsonData) {
            var result = parseInt(jsonData['Result']);
            var message = jsonData['Message'];
            if (result > 0) {
                if (lib51.isSet(message)) {
                    ShowSuccess(message);
                }
            }
            else {
                if (lib51.isSet(message)) {
                    ShowAlert(message);
                }
            }
        }
    }

    var options = {};
    options["type"] = type;
    options["success"] = callback;
    options["error"] = errorCallback;
    options["begin"] = function () {
        ShowLoading();
    };
    options["complete"] = function () {
        CloseLoading();
    };

    lib51.ajaxQuery(url, data, options);
}
/*
jQuery ajax GET请求封装，带加载提示等
@url[string] 请求地址
@data[json] 发送的数据
@callback[function] 请求成功后的回调方法
@errorCallback[function] 请求出错的回调方法。[可选]
*/
function AjaxGet(url, data, callback, errorCallback) {
    AjaxQuery(url, "GET", data, callback, errorCallback);
}
/*
jQuery ajax POST请求封装，带加载提示等
@url[string] 请求地址
@data[json] 发送的数据
@callback[function] 请求成功后的回调方法
@errorCallback[function] 请求出错的回调方法。[可选]
*/
function AjaxPost(url, data, callback, errorCallback) {
    AjaxQuery(url, "POST", data, callback, errorCallback);
}

/*
POST提交表单数据
@formId[string] 表单id
@buttonId[string] 表单提交按钮id
@callback[function]
*/
function PostFormData(formId, buttonId, callback, extraData) {
    if (!lib51.isFunc(callback)) {
        callback = function (jsonData) {
            var result = parseInt(jsonData['Result']);
            var message = jsonData['Message'];
            if (result > 0) {
                if (lib51.isSet(message)) {
                    ShowSuccess(message);
                }
            }
            else {
                if (lib51.isSet(message)) {
                    ShowAlert(message);
                }
            }
        }
    }
    lib51.postForm(formId, buttonId, callback, function () {
        ShowLoading();
    }, function () {
        CloseLoading();
    }, extraData);
}

/*
是否图片
*/
function IsWebImage(fileName) {
    var dotPos = fileName.lastIndexOf('.');
    if (dotPos > 0) {
        var ext = fileName.substr(dotPos);
        ext = ext.toLowerCase();
        if (
            ext == '.jpg' ||
            ext == '.png' ||
            ext == '.gif' ||
            ext == '.bmp' ||
            ext == '.ico' ||
            ext == '.jpeg'
        ) {
            return true;
        }
        return false;
    }
    else {
        return false;
    }
}

/*
退出登陆
*/
function Logout() {
    var url = '/Home/QueryLogout';

    AjaxGet(url, null, function (jsonData) {
        var message = jsonData["Message"];
        ShowSuccess(message);
        setTimeout(function () {
            location.reload();
        }, 300);
    });
    return false;
}

/*
解析列表模板加载数据
*/
function ParseListTemplate(containerId, templateId, listData) {
    var tmpl = $('#' + templateId).html();
    var html = '';
    for (var i = 0; i < listData.length; i++) {
        var item = listData[i];
        var itemHtml = juicer(tmpl, item);
        html += itemHtml;
    }
    $('#' + containerId).html(html);
}
/*
全选框
*/
function SetCheckAllOrNot(chkId, containerId, inputName) {
    var chk = $('#' + chkId);
    if (!chk[0]) {
        return;
    }
    chk.bind('click', function () {
        if ($(this).attr("checked")) {
            $('#' + containerId + ' input[name="' + inputName + '"]').attr("checked", true);
        }
        else {
            $('#' + containerId + ' input[name="' + inputName + '"]').attr("checked", false);
        }
    });

    $('#' + containerId + ' input[name="' + inputName + '"]').bind('click', function () {
        var itemChk = $(this);
        
        var chkT = $('#' + chkId);
        var isAllCheck = true;
        $('#' + containerId + ' input[name="' + inputName + '"]').each(function () {
            if (!$(this).attr("checked")) {
                isAllCheck = false;
            }
        });
        chkT.attr("checked", isAllCheck);
    });
}

