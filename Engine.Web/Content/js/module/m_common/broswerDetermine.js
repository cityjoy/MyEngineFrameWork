define(function(require, exports, module) {
	var browserDetermine = {},
		userAgent = navigator.userAgent;

	browserDetermine.getIE = function() {
		var isOpera = userAgent.indexOf("OPR") > -1,
			isIE = userAgent.indexOf("compatible") > -1 && userAgent.indexOf("MSIE") > -1 && !isOpera,
			isIE11 = userAgent.toLowerCase().indexOf("trident") > -1 && userAgent.indexOf("rv") > -1,
			isEdge = userAgent.indexOf("Edge") > -1;
		if (isIE) {
			var reIE = new RegExp("MSIE (\\d+\\.\\d+);");
			reIE.test(userAgent);
			var fIEVersion = parseFloat(RegExp.$1);
			if (fIEVersion === 6) {
				return "IE6";
			} else if (fIEVersion === 7) {
				return "IE7";
			} else if (fIEVersion === 8) {
				return "IE8";
			} else if (fIEVersion === 9) {
				return "IE9";
			} else if (fIEVersion === 10) {
				return "IE10";
			} else {
				return "0";
			} //IE版本过低
		}
		if (isIE11) {
			return "IE11";
		}
		if (isEdge) {
			return "Microsoft Edge";
		}
		return null;
	};

	browserDetermine.getChrome = function() {
		var isChrome = userAgent.indexOf("Chrome") > -1 && parseFloat(userAgent.indexOf("Safari")) > -1; //判断Chrome浏览器
		if (isChrome) {
			return "Chrome";
		}
		return null;
	};

	browserDetermine.getSafari = function() {
		var isSafari = userAgent.indexOf("Safari") > -1 && userAgent.indexOf("Chrome") === -1; //判断是否Safari浏览器
		if (isSafari) {
			return "Safari";
		}
		return null;
	};

	browserDetermine.getOpera = function() {
		var isOpera = userAgent.indexOf("OPR") > -1; //判断是否Opera浏览器
		if (isOpera) {
			return "Opera";
		}
		return null;
	};

	browserDetermine.getFF = function() {
		var isFF = userAgent.indexOf("Firefox") > -1; //判断是否Firefox浏览器
		if (isFF) {
			return "Firefox";
		}
		return null;
	};

	browserDetermine.getQQ = function() {
		var isQQ = userAgent.indexOf("QQBrowser") > -1; //判断是否Firefox浏览器
		if (isQQ) {
			return "QQBrowser";
		}
		return null;
	};

	browserDetermine.getLiebao = function() {
		var isLiebao = userAgent.indexOf("LBBROWSER") > -1; //判断是否Firefox浏览器
		if (isLiebao) {
			return "LiebaoBrowser";
		}
		return null;
	};

	browserDetermine.getSougou = function() {
		var isSougou = userAgent.indexOf("MetaSr") > -1; //判断是否Firefox浏览器
		if (isSougou) {
			return "SougouBrowser";
		}
		return null;
	};

	browserDetermine.doOpt = function() {
		var getIE = this.getIE(),
			getChrome = this.getChrome(),
			getSafari = this.getSafari(),
			getOpera = this.getOpera(),
			getFF = this.getFF(),
			getQQ = this.getQQ(),
			getLiebao = this.getLiebao(),
			getSougou = this.getSougou();
		switch (true) {
			case typeof(getIE) !== "object":
				return getIE;
			case typeof(getQQ) !== "object":
				return getQQ;
			case typeof(getLiebao) !== "object":
				return getLiebao;
			case typeof(getSougou) !== "object":
				return getSougou;
			case typeof(getFF) !== "object":
				return getFF;
			case typeof(getOpera) !== "object":
				return getOpera;
			case typeof(getSafari) !== "object":
				return getSafari;
			case typeof(getChrome) !== "object":
				return getChrome;
			default:
				return null;

		}

	};

	module.exports = browserDetermine;
});