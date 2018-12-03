define(function (require, exports, module) {

	var loginMsg = {};

	loginMsg.optionHtml = function (i, val, txt) {
        switch (i) {
            case 0:
                return "<option selected='selected' value=''>请选择学校</option>";
            case 1:
                return "<option value='" + val + "'>" + txt + "</option>";
            default:
                break;
        }
    };
    loginMsg.getCity = function (obj) {
        return $(obj).val();
    };
    loginMsg.getJson = function (obj) {
        var _this = this,
            html = _this.optionHtml(0);
        if (this.getCity(obj) > 0) {
            $.getJSON("/Ajax/QueryGetSchoolList?cityId=" + this.getCity(obj), function (data) {
                var schoolList = data.Data;
                $.each(schoolList, function (i) {
                    var name = schoolList[i].Name;
                    var id = schoolList[i].SchoolId;
                    html += _this.optionHtml(1, id, name);

                });
                $("select[name='SchoolId']").html(html);
            });
        } else {
            $("select[name='SchoolId']").html(html);
        }
	};
	
	loginMsg.selectSchool = function (obj) {
        var _this = this;
        $(document).on("change", "select[name='CityId']", function () {
            $("select[name='School']").html("");
            _this.getJson(this);
        });
    };


	loginMsg.msgPanel = function () {
		if (IsLogin === false && $(".jNoLogin").length > 0) {
			$(".jNoLogin").click(function (e) {
				$(".msg-login").show();
				e.preventDefault();
			});
			$(".close").click(function () {
				$(".msg").hide();
			});
		}
	};
	loginMsg.doLogin = function () {
		$("#btnLogin").click(function (event) {
			event.preventDefault();
			PostFormData('LoginForm', 'btnLogin', function (jsonData) {
			    var result = parseInt(jsonData["Result"]),
					message = jsonData["Message"],
					errorCode = parseInt(jsonData["ErrorCode"]);
                 
				if (result > 0) {
				    var returnUrl = jsonData.Data.ReturnUrl;
				    ShowSuccess(message, function () {
				        var homeUrl = $('#hdHomeUrl').val();
				        //var returnUrl = $('#hdReturnUrl').val();
				        if (returnUrl && returnUrl.length > 0) {
				            location.href = returnUrl;
				        } else {
				            if (homeUrl && homeUrl.length > 0) {
				                location.href = homeUrl;
				            } else {
				                location.href = '/';
				            }
				        }
				    });
					//if (errorCode == -100) {
					//	//隐藏登录时的蒙版
					//	$(".msg-login").hide();
					//} else {
					//	ShowSuccess(message, function () {
					//		var homeUrl = $('#hdHomeUrl').val();
					//		//var returnUrl = $('#hdReturnUrl').val();
					//		if (returnUrl && returnUrl.length > 0) {
					//			location.href = returnUrl;
					//		} else {
					//			if (homeUrl && homeUrl.length > 0) {
					//				location.href = homeUrl;
					//			} else {
					//				location.href = '/';
					//			}
					//		}
					//	});
					//}
				} else {
					ShowAlert(message);
				}
			});
		});

	};
	loginMsg.doOpt = function () {
		this.selectSchool($("#LoginForm"));
		this.msgPanel();
		this.doLogin();
	};
	module.exports = loginMsg;
});