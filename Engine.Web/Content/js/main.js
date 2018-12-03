define(function(require, exports, module) {

	setTimeout(function() {
		require.async('./module/m_global/opt', function(e) {
			e.doOpt();
		});
		if ($('.index-login').length > 0) {
			require.async('./module/m_loginBox/opt', function(e) {
				e.doOpt();
			});
		}
		if($("div[id*='m_']").length > 0){
			$("div[id*='m_']").each(function(index, el) {
				require.async('./module/'+$(this).attr('id')+'/opt', function(e) {
					e.doOpt();
				});
			});
		}
	}, 300);
});