/*******************************************************************************
* KindEditor - WYSIWYG HTML Editor for Internet
* Copyright (C) 2006-2011 kindsoft.net
*
* @author Roddy <luolonghao@gmail.com>
* @site http://www.kindsoft.net/
* @licence http://www.kindsoft.net/license.php
*******************************************************************************/

// google code prettify: http://google-code-prettify.googlecode.com/
// http://google-code-prettify.googlecode.com/

KindEditor.plugin('code', function(K) {
	var self = this, name = 'code';
	self.clickToolbar(name, function() {
		var lang = self.lang(name + '.'),
			html = ['<div style="padding:10px 20px;">',
				'<div class="ke-dialog-row">',
				'<select class="ke-code-type">',
                '<option value="csharp">C#</option>',
                '<option value="javascript">JavaScript</option>',
                '<option value="javascript_dom">JavaScript with DOM</option>',
                '<option value="css">CSS</option>',
                '<option value="php">PHP</option>',
                '<option value="sql">SQL</option>',
                '<option value="html">HTML</option>',
                '<option value="xml">XML</option>',
                '<option value="java">Java</option>',
                '<option value="c">C</option>',
                '<option value="cpp">C++</option>',
                '<option value="flex">Flex</option>',
                '<option value="perl">Perl</option>',
                '<option value="python">Python</option>',
                '<option value="ruby">Ruby</option>',

                '<option value="vb">VB.Net</option>',
                '<option value="erlang">Erlang</option>',
                '<option value="as3">ActionScript3</option>',
                '<option value="bash">Bash/Shell</option>',
                '<option value="cf">ColdFusion</option>',
                '<option value="delphi">Delphi</option>',
                '<option value="diff">Diff</option>',
                '<option value="groovy">Groovy</option>',
                '<option value="java">JavaFx</option>',
                '<option value="scala">Scala</option>',
                '<option value="powershell">PowerShell</option>',
                '<option value="html">Other</option>',
				'</select>',
                '&nbsp;<select class="ke-code-collapse">',
                '<option value="1">折叠</option>',
                '<option value="0">展开</option>',
                '</select>',
                '&nbsp;样式:<select class="ke-code-style">',
                '<option value="random">随机</option>',
                '<option value="whatis">whatis</option>',
                '<option value="kwrite">kwrite</option>',
                '</select>',
				'</div>',
				'<textarea class="ke-textarea" style="width:408px;height:260px;"></textarea>',
				'</div>'].join(''),
			dialog = self.createDialog({
				name : name,
				width : 450,
				title : self.lang(name),
				body : html,
				yesBtn : {
					name : self.lang('yes'),
					click : function(e) {
					    var type = K('.ke-code-type', dialog.div).val();
					    var collapse = parseInt(K('.ke-code-collapse', dialog.div).val());
					    var cStyle = parseInt(K('.ke-code-style', dialog.div).val());
					    var code = textarea.val();
					    var cls = type === '' ? '' : ' lang-' + type;
					    var brush = type === '' ? '' : ' brush:' + type;
					    var collapseCls = collapse > 0 ? " collapse" : "";
					    var html = '<pre class="prettyprint' + cls + brush + collapseCls + '" style-data="' + cStyle + '">\n' + K.escape(code) + '</pre> ';
						if (K.trim(code) === '') {
							alert(lang.pleaseInput);
							textarea[0].focus();
							return;
						}
						self.insertHtml(html).hideDialog().focus();
					}
				}
			}),
			textarea = K('textarea', dialog.div);
		textarea[0].focus();
	});
});
