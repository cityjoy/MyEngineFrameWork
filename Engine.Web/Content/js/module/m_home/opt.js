define(function (require, exports, module) {

    var indexV2 = {},
    citySelect = require("module/m_common/citySelect"),
		doSlide = require("module/m_home/doSlide"),
	    doTab = require("module/m_home/doTab"),
        doLogin = require("module/m_home/doLogin");

    indexV2.doOpt = function () {
        doSlide.doOpt();
        doTab.doOpt();
        doLogin.doOpt();
        citySelect.doOpt();
    };

    module.exports = indexV2;
});