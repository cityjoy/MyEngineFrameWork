define(function(require, exports, module) {
	var citySelect = {};

	citySelect.optionHtml = function(i, val, txt) {
		switch (i) {
			case 0:
				return "<option selected='selected' value=''>请选择城市</option>";
			case 1:
				return "<option value='" + val + "'>" + txt + "</option>";
			default:
				break;
		}
	};
	citySelect.getProvince = function(obj) {
		return $(obj).val();
	};
	citySelect.getJson = function(obj) {
		var _this = this,
			html = _this.optionHtml(0);
		if (this.getProvince(obj) > 0) {
			$.getJSON("/Ajax/QueryGetCityList?ProvinceId=" + this.getProvince(obj), function(data) {
				var cityList = data.Data;
				$.each(cityList, function(i) {
					var name = cityList[i].Name;
					var id = cityList[i].CityId;
					html += _this.optionHtml(1, id, name);
				});
				$("select[name='CityId']").append(html);
			});
		} else {
			$("select[name='CityId']").append(html);
		}

	};

	citySelect.doOpt = function() {
		var _this = this;
		$(document).on("change","select[name='ProvinceId']",function () {
			$("select[name='CityId']").html("");
			_this.getJson(this);
		});
	};

	module.exports = citySelect;
});