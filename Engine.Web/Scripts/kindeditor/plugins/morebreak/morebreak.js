KindEditor.plugin('morebreak', function (K) {
	var self = this;
	var name = 'morebreak';
	var morebreakHtml = K.undef(self.morebreakHtml, '<div class="xt-more">&nbsp;</div>');

	self.clickToolbar(name, function() {
		var cmd = self.cmd, range = cmd.range;
		self.focus();
		self.insertHtml(morebreakHtml);
	});
});
