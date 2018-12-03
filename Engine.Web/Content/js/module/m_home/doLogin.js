define(function (require, exports, module) {

    var doLogin = {};

    doLogin.optionHtml = function (i, val, txt) {
        switch (i) {
            case 0:
                return "<option selected='selected' value=''>请选择学校</option>";
            case 1:
                return "<option value='" + val + "'>" + txt + "</option>";
            default:
                break;
        }
    };
    doLogin.getCity = function (obj) {
        return $(obj).val();
    };
    doLogin.getJson = function (obj) {
        var _this = this,
            html = _this.optionHtml(0);
        if (this.getCity(obj) > 0) {
            $.getJSON("/School/QueryGetSchoolByCityIdData?CityId=" + this.getCity(obj), function (data) {
                var schoolList = data.Data;
                $.each(schoolList, function (i) {
                    var name = schoolList[i].Name;
                    var id = schoolList[i].SchoolId;
                    html += _this.optionHtml(1, id, name);
                });
                $("select[name='Sschool']").html(html);
            });
        } else {
            $("select[name='Sschool']").html(html);
        }
    };

    doLogin.changeMsg = function () {
        $(document).on("click", ".index-login-opt .c-blue", function () {
            if ($(this).hasClass("cur")) {
                var txt1 = "用户名/手机号/邮箱";
                $(this).removeClass("cur");
                $(this).parents("form").find("p:first span").text(txt1);
                $(this).parent().siblings(".selectSchool").hide();
                $(this).text("使用学号 / 工号登录 »");
                $(this).parents("form").find("p [type=submit]").removeClass("cur");
            } else {
                var txt2 = $(this).parents("form").find("p:first span").data("title");
                $(this).addClass("cur");
                $(this).parents("form").find("p:first span").text(txt2);
                $(this).parent().siblings(".selectSchool").show();
                $(this).text("使用帐号登录 »");
                $(this).parents("form").find("p [type=submit]").addClass("cur");
            }
        });
    };

    doLogin.selectSchool = function (obj) {
        var _this = this;
        $(document).on("change", "select[name='CityId']", function () {
            $("select[name='Sschool']").html("");
            _this.getJson(this);
        });
        $("#selectSchool .btn").click(function () {
            if ($("select[name='CityId']").val() == "" || $("select[name='Sschool']").val() == "") {
                ShowAlert("请选择正确的选项");
            } else {
                $("[name=SchoolProvinceId]").val($("[name=ProvinceId]").val());
                $("[name=SchoolCityId]").val($("[name=CityId]").val());
                $("[name=SchoolId]").val($("[name=Sschool]").val());
                $(".selectSchool span").text($("select[name='Sschool'] option:selected").text());
                layer.close(obj);
            }
        });
    };

    doLogin.showSelectMsg = function () {
        $(".selectSchool").click(function () {
            layer.open({
                type: 1,
                content: $("#tpl_selectSchool").html(),
                success: function (layero, index) {
                    doLogin.selectSchool(index);
                }
            });
        });
    };

    doLogin.StudentLogin = function () {
        var appendData = {};
        appendData["Type"] = 1;

        PostFormData('StudentLoginForm', 'btnStudentLogin', function (jsonData) {
            var result = parseInt(jsonData["Result"]);
            var message = jsonData["Message"];
            var errorCode = parseInt(jsonData["ErrorCode"]);
            var returnUrl = jsonData.Data["ReturnUrl"];

            if (result > 0) {
                if (errorCode == -100) {
                    alertMsgBox(returnUrl);
                } else {
                    ShowSuccess(message, function () {
                        $('#LoginPanel').hide();
                        var eduUrl = $('#hdHomeUrl').val();
                        if (returnUrl.length > 0) {
                            location.href = returnUrl;
                        } else {
                            location.href = eduUrl;
                        }
                    });
                }
            } else {
                ShowAlert(message);
            }
        }, appendData);
    };
    doLogin.TeacherLogin = function () {
        var appendData = {};
        appendData["Type"] = 2;

        PostFormData('TearcherLoginForm', 'btnTeacherLogin', function (jsonData) {
            var result = parseInt(jsonData["Result"]);
            var message = jsonData["Message"];
            var errorCode = parseInt(jsonData["ErrorCode"]);
            var returnUrl = jsonData.Data["ReturnUrl"];

            if (result > 0) {
                ShowSuccess(message, function () {
                    $('#LoginPanel').hide();
                    var eduUrl = $('#hdHomeUrl').val();
                    if (returnUrl.length > 0) {
                        location.href = returnUrl;
                    } else {
                        location.href = eduUrl;
                    }
                });

            } else {
                ShowAlert(message);
            }
        }, appendData);
    };
    doLogin.ParentLogin = function () {
        var appendData = {};
        appendData["Type"] = 3;

        PostFormData('ParentLoginForm', 'btnParentLogin', function (jsonData) {
            var result = parseInt(jsonData["Result"]);
            var message = jsonData["Message"];
            var errorCode = parseInt(jsonData["ErrorCode"]);
            var returnUrl = jsonData.Data["ReturnUrl"];

            if (result > 0) {
                if (errorCode == -100) {
                    alertMsgBox(returnUrl);
                } else {
                    ShowSuccess(message, function () {
                        $('#LoginPanel').hide();
                        var eduUrl = $('#hdHomeUrl').val();
                        if (returnUrl.length > 0) {
                            location.href = returnUrl;
                        } else {
                            location.href = eduUrl;
                        }
                    });
                }
            } else {
                ShowAlert(message);
            }
        }, appendData);
    };

    doLogin.doSubmit = function () {
        $(".index-login [type=submit]").click(function () {
            if ($(this).hasClass("cur")) {
                if ($("[name=SchoolId]").val() == "" || $("[name=SchoolCityId]").val() == "" || $("[name=SchoolProvinceId]").val() == "") {
                    ShowAlert("请选择学校");
                    return false;
                } else {
                    var type = $(this).parent().siblings("[name=Type]").val();
                    switch (type) {
                        case "Student":
                            doLogin.StudentLogin();
                            break;
                        case "Teacher":
                            doLogin.TeacherLogin();
                            break;
                        case "Parent":
                            doLogin.ParentLogin();
                            break;
                    }
                }
            } else {
                var type = $(this).parent().siblings("[name=Type]").val();
                console.log(type);
                switch (type) {
                    case "Student":
                        doLogin.StudentLogin();
                        break;
                    case "Teacher":
                        doLogin.TeacherLogin();
                        break;
                    case "Parent":
                        doLogin.ParentLogin();
                        break;
                }
            }
        });
    };

    doLogin.doOpt = function () {
        this.changeMsg();
        this.showSelectMsg();
        this.doSubmit();
    };

    module.exports = doLogin;
});

function CheckRoleLink(roleType, messtype) {
    if (IsLogin) {
        if (CurrentUser["Type"] == roleType) {
            return true;
        } else {
            switch (roleType) {
                case 1:
                    ShowAlert('您不是学生，无法访问此页面');
                    break;
                case 2:
                    ShowAlert('您不是教师，无法访问此页面');
                    break;
                case 3:
                    ShowAlert('您不是学生家长，无法访问此页面');
                    break;
            }
            return false;
        }
    } else {
        if (messtype == 1) {
            ShowAlert('您未登录');
            return false;
        }
    }
    return false;
}

function CheckRolesLink(roleTypes, messtype) {
    if (IsLogin) {
        if (roleTypes.indexOf(CurrentUser["Type"]) >= 0)
            return true;
        else {
            ShowAlert('您无法访问此页面');
        }
    } else {
        if (messtype == 1) {
            ShowAlert('您未登录');
            return false;
        }
    }
    return false;
}