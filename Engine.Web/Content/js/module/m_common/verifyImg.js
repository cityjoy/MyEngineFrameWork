define(function(require, exports, module) {
	var verifyImg = {};

	verifyImg.getUrl = function(obj) {
		return $(obj).attr("_src").split("rand=");
	};
	verifyImg.rand = function() {
		return new Date().getTime();
	};
	verifyImg.sendUrl = function(obj) {
		return this.getUrl(obj)[0] + "rand=" + this.rand();
	};

	verifyImg.exchangeImg = function(obj) {
		$(obj).attr("src", this.sendUrl(obj));
	};

	verifyImg.clickImg = function(obj) {
		var _this = this;
		$(obj).click(function() {
			_this.exchangeImg(this);
		});
	};
	verifyImg.doOpt = function() {
		this.clickImg(".verifyimg");
	};


	module.exports = verifyImg;
});