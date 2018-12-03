define(function (require, exports, module) {
	var validate = {};

	validate.empty = function (text) {
		if (typeof text == 'string') {
			text = text.replace(/(^\s+)|(\s+$)/g, "");
			if (text.length === 0) return false;
			else return true;
		} else {
			return false
		}
	};

	validate.len = function (text, min, max) {
		min = arguments[1] ? arguments[1] : 0;
		if (text.length < min || text.length > max) return false;
		else return true;
	};

	validate.exp = function (text, reg) {
		if (reg.test(text)) return true;
		else return false;
	};

	validate.exp_email = function (text) {
		var re = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
		return this.exp(text, re);
	};

	validate.exp_tel = function (text) {
		var re = /^1[3,4,5,7,8]\d{9}$/;
		return this.exp(text, re);
	};

	module.exports = validate;
});