define(function(require, exports, module) {

	var doSlide = {};

	doSlide.doOpt = function() {
		if ($(".index-section-con li").length > 5) {
			jQuery(".index-section-btn").show();
			jQuery(".index-section").slide({
				mainCell: ".index-section-con ul",
				autoPlay: true,
				effect:"leftLoop",
				vis: 5,
				delayTime: 500,
				interTime: 5000,
				trigger: "click"
			});
		}

	};

	module.exports = doSlide;
});