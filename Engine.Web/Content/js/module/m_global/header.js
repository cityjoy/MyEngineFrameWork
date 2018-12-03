define(function(require, exports, module) {
	var header = {};
	header.navPanel = function() {
		$(".header-nav li").hover(function() {
			$(this).find("dl").show();
		}, function() {
			$(this).find("dl").hide();
		});
	};

	header.userPanel = function() {
		$(".header-user").hover(function() {
			$(this).find("ul").show();
		}, function() {
			$(this).find("ul").hide();
		});
	};
	header.narrowNav = function(){
		
			$("#headerMenu").click(function(event) {
				if($(window).width()<=1200){
					if($(this).hasClass('on')){
						$(this).siblings('.header-nav,.header-user,.header-login').removeClass("headerShow");
						$(this).removeClass('on');
					}else{
						$(this).siblings('.header-nav,.header-user,.header-login').addClass('headerShow');
						$(this).addClass('on');
					}
				}
			});
		
	};
	header.doOpt = function() {
		this.userPanel();
		this.navPanel();
		this.narrowNav();
	};
	module.exports = header;

});