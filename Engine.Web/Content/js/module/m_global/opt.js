define(function(require, exports, module) {
	var headerOpt = require("module/m_global/header"),
		sidebarOpt = require("module/m_global/sidebar"),
		loginMsgOpt = require("module/m_global/loginMsg"),
        parentNotice = require("module/m_global/parentNotice"),
        isTrial = require("module/m_global/isTrial"),
		globalOpt = {};
	globalOpt.doOpt = function() {
		headerOpt.doOpt();
		sidebarOpt.doOpt();
		loginMsgOpt.doOpt();
		parentNotice.doOpt();
		isTrial.doOpt();
	};
	module.exports = globalOpt;
});