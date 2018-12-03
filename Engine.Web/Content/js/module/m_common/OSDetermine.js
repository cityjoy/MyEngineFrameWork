define(function(require, exports, module) {
	var OSDetermine = {};

	OSDetermine.getWin = function() {
		var isWin = navigator.platform === "Win32" || navigator.platform === "Windows",
			userAgent = navigator.userAgent;
		if (isWin) {
			var isWin2K = userAgent.indexOf("Windows NT 5.0") > -1 || userAgent.indexOf("Windows 2000") > -1,
				isWinXP = userAgent.indexOf("Windows NT 5.1") > -1 || userAgent.indexOf("Windows XP") > -1,
				isWin2003 = userAgent.indexOf("Windows NT 5.2") > -1 || userAgent.indexOf("Windows 2003") > -1,
				isWinVista = userAgent.indexOf("Windows NT 6.0") > -1 || userAgent.indexOf("Windows Vista") > -1,
				isWin7 = userAgent.indexOf("Windows NT 6.1") > -1 || userAgent.indexOf("Windows 7") > -1,
				isWin8 = userAgent.indexOf("windows nt 6.2")>-1,
				isWin81 = userAgent.indexOf("windows nt 6.3")>-1,
				isWin10 = userAgent.indexOf("Windows NT 10.0");
			if (isWin2K) return "windows 2000";
			if (isWinXP) return "windows XP";
			if (isWin2003) return "windows 2003";
			if (isWinVista) return "windows Vista";
			if (isWin7) return "windows 7";
			if (isWin8) return "windows 8";
			if (isWin81) return "windows 8.1";
			if (isWin10) return "windows 10";
		}
		return null;
	};

	OSDetermine.getMac = function() {
		var isMac = navigator.platform === "Mac68K" || navigator.platform === "MacPPC" || navigator.platform === "Macintosh" || navigator.platform === "MacIntel";
		if (isMac) return "Mac";
		return null;
	};

	OSDetermine.getUnix = function() {
		var isUnix = navigator.platform === "X11";
		if (isUnix) return "Unix";
		return null;
	};

	OSDetermine.getLinux = function() {
		var isLinux = String(navigator.platform).indexOf("Linux") > -1;
		if (isLinux) return "Linux";
		return null;
	};

	OSDetermine.doOpt = function() {
		var getWin = this.getWin(),
			getMac = this.getMac(),
			getUnix = this.getUnix(),
			getLinux = this.getLinux();
		switch (true) {
			case typeof(getWin) !== "object":
				return getWin;
			case typeof(getMac) !== "object":
				return getMac;
			case typeof(getUnix) !== "object":
				return getUnix;
			case typeof(getLinux) !== "object":
				return getLinux;
			default:
				return null;
		}
		
	};

	module.exports = OSDetermine;
});