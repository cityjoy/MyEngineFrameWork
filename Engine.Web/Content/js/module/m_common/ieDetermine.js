define(function(require, exports, module) {
	var ieDetermine = {};

	ieDetermine.browserIE = function(Num) { //判断ie版本
		if ($.browser.msie && $.browser.version < Num) {
			return true;
		} else {
			return false;
		}
	};

	module.exports = ieDetermine;
});