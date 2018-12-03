define(function(require, exports, module) {

	var sidebar = {};
	sidebar.sidebarPanel = function() {
		var winWidth = $(window).width();
		if (winWidth >= 1280) {
			$(".sidebar").fadeIn();
			$(".minSidebar").remove();
			$(".sidebar").hover(function() {
				$(this).addClass("show");
			}, function() {
				$(this).removeClass("show");
			});
		} else {
			$(".minSidebar").fadeIn();
			$(".sidebar").addClass("show");
			$(".minSidebar").mouseover(function() {
				$(".sidebar").fadeIn();
			});
			$(".sidebar").mouseleave(function() {
				$(".sidebar").hide();
				$(".minSidebar").show();
			});
		}

	};
	sidebar.returnTop = function() {
		lib51.fixObjectPosition("returnTop", {
			"bottom": 34,
			"right": 10,
			"showPosition": {
				"top": $(window).height() / 3,
				"left": 0
			}
		});
		$("#returnTop").click(function(event) {
			lib51.scrollToPosition("stage");
		});
	};
	sidebar.doOpt = function() {
		this.sidebarPanel();
		//this.returnTop();
	};
	module.exports = sidebar;
});