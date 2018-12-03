(function($) {
	var defaultSettings = {
		url: "", //上传地址    
		fileSuffixs: ["jpg", "png", "gif", "bmp"], //允许上传的文件后缀名列表
		errorText: "不能上传后缀为 {0} 的文件！", //错误提示文本，其中{0}将会被上传文件的后缀名替换    
		onCheckUpload: function(text) { //上传时检查文件后缀名不包含在fileSuffixs属性中时触发的回调函数，(text为错误提示文本)    

		},
		onComplete: function(msg) { //上传完成后的回调函数[不管成功或失败，它都将被触发](msg为服务端的返回字符串)
			alert(msg);
		},

		onChosen: function(file, obj, fileSize, errorTxt) {
			/*选择文件后的回调函数，(file为选中文件的本地路径;obj为当前的上传控件实例,  fileSize为上传文件大小，单位KB[只有在isFileSize为true时，此参数才有值], errorTxt为获取文件大小时的错误文本提示)  */
			//alert(file);
		},
		cropperParam: {}, //图片截取参数设置，此参数即为Jcrop插件参数  
		isFileSize: false, //是否获取文件大小  
		perviewImageElementId: "", //用于预览上传图片的元素id（请传入一个div元素的id）
		perviewImgStyle: null //用于设置图片预览时的样式（可不设置，在不设置的情况下多文件上传时只能显示一张图片），如{ width: '100px', height: '100px', border: '1px solid #ebebeb' }
	};


	$.fn.cropperUpload = function(settings) {

		settings = $.extend({}, defaultSettings, settings || {});

		return this.each(function() {
			var self = $(this);

			var upload = new UploadAssist(settings);

			upload.createIframe(this);

			//绑定当前按钮点击事件    
			self.bind("click", function(e) {
				upload.chooseFile();
			});

			//将上传辅助类的实例，存放到当前对象中，方便外部获取    
			self.data("uploadFileData", upload);

			//创建的iframe中的那个iframe，它的事件需要延迟绑定    
			window.setTimeout(function() {
				$(upload.getIframeContentDocument().body).find("input[type='file']").change(function() {
					if (!this.value) return;
					var fileSuf = this.value.substring(this.value.lastIndexOf(".") + 1);


					//检查是否为允许上传的文件    
					if (!upload.checkFileIsUpload(fileSuf, upload.settings.fileSuffixs)) {
						upload.settings.onCheckUpload(upload.settings.errorText.replace("{0}", fileSuf));
						return;
					}

					if (upload.settings.isFileSize) {
						var size = perviewImage.getFileSize(this, upload.getIframeContentDocument());
						var fileSize, errorTxt;
						if (size == "error") {
							errorTxt = upload.errorText;
						} else {
							fileSize = size;
						}
						//选中后的回调    
						upload.settings.onChosen(this.value, this, fileSize, errorTxt);
					} else {
						//选中后的回调    
						upload.settings.onChosen(this.value, this);
					}


					//是否开启了图片预览    
					if (upload.settings.perviewImageElementId) {
						var main = perviewImage.createPreviewElement("closeImg", this.value, upload.settings.perviewImgStyle);
						$("#" + upload.settings.perviewImageElementId).append(main);
						var div = $(main).children("div").get(0);

						perviewImage.beginPerview(this, div, upload.getIframeContentDocument(), upload);
					}
				});

				//为创建的iframe内部的iframe绑定load事件    
				$(upload.getIframeContentDocument().body.lastChild).on("load", function() {
					var dcmt = upload.getInsideIframeContentDocument();
					upload.submitStatus = true;
					if (dcmt.body.innerHTML) {
						if (settings.onComplete) {
							settings.onComplete(dcmt.body.innerHTML);
						}
						dcmt.body.innerHTML = "";
					}
				});
			}, 100);
		});
	};
})(jQuery);


//上传辅助类  
function UploadAssist(settings) {
	//保存设置  
	this.settings = settings;
	//创建的iframe唯一名称  
	this.iframeName = "upload" + this.getTimestamp();
	//提交状态  
	this.submitStatus = true;
	//针对IE上传获取文件大小时的错误提示文本  
	this.errorText = "请设置浏览器一些参数后再上传文件，方法如下（设置一次即可）：\n请依次点击浏览器菜单中的\n'工具->Internet选项->安全->可信站点->自定义级别'\n在弹出的自定义级别窗口中找到 'ActiveX控件和插件' 项，将下面的子项全部选为 '启用' 后，点击确定。\n此时不要关闭当前窗口，再点击 '站点' 按钮，在弹出的窗口中将下面复选框的 '√' 去掉，然后点击 '添加' 按钮并关闭当前窗口。\n最后一路 '确定' 完成并刷新当前页面。";
	this.jcropApi;
	return this;
}
UploadAssist.prototype = {
	//辅助类构造器  
	constructor: UploadAssist,

	//创建iframe  
	createIframe: function( /*插件中指定的dom对象*/ elem) {

		var html = "<html>" + "<head>" + "<title>upload</title>" + "<script>" + "function getDCMT(){return window.frames['dynamic_creation_upload_iframe'].document;}" + "</" + "script>" + "</head>" + "<body>" + "<form method='post' name='upload_img' target='dynamic_creation_upload_iframe' enctype='multipart/form-data' action='" + this.settings.url + "'>" + "<input type='text' id='x1' name='OffsetX' />" + "<input type='text' id='y1' name='OffsetY' />" + "<input type='text' id='x2' name='x2' />" + "<input type='text' id='y2' name='y2' />" + "<input type='text' id='w'  name='CropperWidth' />" + "<input type='text' id='h'  name='CropperHeight' />" + "<input type='text' id='maxW' name='ImageWidth' />" + "<input type='text' id='maxH' name='ImageHeight' />" + "<input type='file' name='AvatarImage' />" + "</form>" + "<iframe name='dynamic_creation_upload_iframe'></iframe>" + "</body>" + "</html>";


		this.iframe = $("<iframe name='" + this.iframeName + "'></iframe>")[0];
		this.iframe.style.width = "0px";
		this.iframe.style.height = "0px";
		this.iframe.style.border = "0px solid #fff";
		this.iframe.style.margin = "0px";
		elem.parentNode.insertBefore(this.iframe, elem);
		var iframeDocument = this.getIframeContentDocument();
		iframeDocument.write(html);
	},

	//获取时间戳  
	getTimestamp: function() {
		return (new Date()).valueOf();
	},
	//设置图片缩放的最大高宽  
	setMaxWidthAndHeight: function( /*最大宽*/ maxW, /*最大高*/ maxH) {
		this.getElement("maxW").value = maxW;
		this.getElement("maxH").value = maxH;
	},
	//设置图片截取参数  
	setImageCropperObj: function( /*图片截取对象*/ obj) {
		this.getElement("x1").value = obj.x;
		this.getElement("y1").value = obj.y;
		this.getElement("x2").value = obj.x2;
		this.getElement("y2").value = obj.y2;
		this.getElement("w").value = obj.w;
		this.getElement("h").value = obj.h;
	},
	//获取创建的iframe中的元素  
	getElement: function(id) {
		var dcmt = this.getIframeContentDocument();
		return dcmt.getElementById(id);
	},
	//获取创建的iframe中的document对象  
	getIframeContentDocument: function() {
		return this.iframe.contentDocument || this.iframe.contentWindow.document;
	},

	//获取创建的iframe所在的window对象  
	getIframeWindow: function() {
		return this.iframe.contentWindow || this.iframe.contentDocument.parentWindow;
	},

	//获取创建的iframe内部iframe的document对象  
	getInsideIframeContentDocument: function() {
		return this.getIframeWindow().getDCMT();
	},

	//获取上传input控件  
	getUploadInput: function() {
		var inputs = this.getIframeContentDocument().getElementsByTagName("input");
		var uploadControl;
		this.forEach(inputs, function() {
			if (this.type === "file") {
				uploadControl = this;
				return false;
			}
		});
		return uploadControl;
	},

	//forEach迭代函数
	forEach: function( /*数组*/ arr, /*代理函数*/ fn) {
		var len = arr.length;
		for (var i = 0; i < len; i++) {
			var tmp = arr[i];
			if (fn.call(tmp, i, tmp) == false) {
				break;
			}
		}
	},
	//提交上传
	submitUpload: function() {
		if (!this.submitStatus) return;
		this.submitStatus = false;
		this.getIframeContentDocument().forms['upload_img'].submit();
	},
	//检查文件是否可以上传  
	checkFileIsUpload: function(fileSuf, suffixs) {
		var status = false;
		this.forEach(suffixs, function(i, n) {
			if (fileSuf.toLowerCase() === n.toLowerCase()) {
				status = true;
				return false;
			}
		});
		return status;
	},
	//选择上传文件  
	chooseFile: function() {
		if (this.settings.perviewImageElementId) {
			$("#" + this.settings.perviewImageElementId).empty();
		}

		var uploadfile = this.getUploadInput();
		$(uploadfile).val("").click();
	}
};

//图片预览操作  
var perviewImage = {
	timers: [],
	//获取预览元素  
	getElementObject: function(elem) {
		if (elem.nodeType && elem.nodeType === 1) {
			return elem;
		} else {
			return document.getElementById(elem);
		}
	},
	//开始图片预览  
	beginPerview: function( /*文件上传控件实例*/ file, /*需要显示的元素id或元素实例*/ perviewElemId, /*上传页面所在的document对象*/ dcmt, /*上传辅助类实例*/ upload) {
		this.imageOperation(file, perviewElemId, dcmt, upload);
	},
	//图片预览操作  
	imageOperation: function( /*文件上传控件实例*/ file, /*需要显示的元素id或元素实例*/ perviewElemId, /*上传页面所在的document对象*/ dcmt, /*上传辅助类实例*/ upload) {
		for (var t = 0; t < this.timers.length; t++) {
			window.clearInterval(this.timers[t]);
		}
		this.timers.length = 0;

		var tmpParams = {
			onChange: function(c) {
				upload.setImageCropperObj(c);
				var $preview = $('#preview-pane'),
					$pcnt = $('#preview-pane .preview-container'),
					$pimg = $('#preview-pane .preview-container img'),
					xsize = $pcnt.width(),
					ysize = $pcnt.height(),
					$preview1 = $('#preview-pane1'),
					$pcnt1 = $('#preview-pane1 .preview-container'),
					$pimg1 = $('#preview-pane1 .preview-container img'),
					xsize1 = $pcnt1.width(),
					ysize1 = $pcnt1.height();
				$('#preview-pane,#preview-pane1').css({
					marginLeft: '0',
					marginTop: '0'
				});
				$('#fileList').css('backgroundImage', 'none');
				if (parseInt(c.w) > 0) {
					var rx = xsize / c.w;
					var ry = ysize / c.h;
					var rx1 = xsize1 * 0.3 / c.w;
					var ry1 = ysize1 * 0.3 / c.h;
					$pimg.css({
						width: Math.round(rx * parseInt($('.jcrop-holder').width())) + 'px',
						height: Math.round(ry * parseInt($('.jcrop-holder').height())) + 'px',
						marginLeft: '-' + Math.round(rx * c.x) + 'px',
						marginTop: '-' + Math.round(ry * c.y) + 'px'
					});
					$pimg1.css({
						width: Math.round(rx * 0.3 * parseInt($('.jcrop-holder').width())) + 'px',
						height: Math.round(ry * 0.3 * parseInt($('.jcrop-holder').height())) + 'px',
						marginLeft: '-' + Math.round(rx * 0.3 * c.x) + 'px',
						marginTop: '-' + Math.round(ry * 0.3 * c.y) + 'px'
					});
				}
			},
			onSelect: function(c) {
				upload.setImageCropperObj(c);
				var $preview = $('#preview-pane'),
					$pcnt = $('#preview-pane .preview-container'),
					$pimg = $('#preview-pane .preview-container img'),
					xsize = $pcnt.width(),
					ysize = $pcnt.height(),
					$preview1 = $('#preview-pane1'),
					$pcnt1 = $('#preview-pane1 .preview-container'),
					$pimg1 = $('#preview-pane1 .preview-container img'),
					xsize1 = $pcnt1.width(),
					ysize1 = $pcnt1.height();
				$('#preview-pane,#preview-pane1').css({
					marginLeft: '0',
					marginTop: '0'
				});
				if (parseInt(c.w) > 0) {
					var rx = xsize / c.w;
					var ry = ysize / c.h;
					var rx1 = xsize1 * 0.3 / c.w;
					var ry1 = ysize1 * 0.3 / c.h;
					$pimg.css({
						width: Math.round(rx * parseInt($('.jcrop-holder').width())) + 'px',
						height: Math.round(ry * parseInt($('.jcrop-holder').height())) + 'px',
						marginLeft: '-' + Math.round(rx * c.x) + 'px',
						marginTop: '-' + Math.round(ry * c.y) + 'px'
					});
					$pimg1.css({
						width: Math.round(rx * 0.3 * parseInt($('.jcrop-holder').width())) + 'px',
						height: Math.round(ry * 0.3 * parseInt($('.jcrop-holder').height())) + 'px',
						marginLeft: '-' + Math.round(rx * 0.3 * c.x) + 'px',
						marginTop: '-' + Math.round(ry * 0.3 * c.y) + 'px'
					});
				}

			}
		};
		var sWidth = 50,
			sHeight = 50;
		if (upload.settings.cropperParam.minSize) {
			var size = upload.settings.cropperParam.minSize;
			sWidth = size[0] > sWidth ? size[0] : sWidth;
			sHeight = size[1] > sHeight ? size[1] : sHeight;
		}
		var params = $.extend({}, tmpParams, upload.settings.cropperParam || {});

		var preview_div = this.getElementObject(perviewElemId);

		var MAXWIDTH = preview_div.clientWidth;
		var MAXHEIGHT = preview_div.clientHeight;

		upload.setMaxWidthAndHeight(MAXWIDTH, MAXHEIGHT);

		if (file.files && file.files[0]) { //此处为Firefox，Chrome以及IE10的操作  
			preview_div.innerHTML = "";
			var img = document.createElement("img");
			preview_div.appendChild(img);
			img.style.visibility = "hidden";
			img.onload = function() {
				var rect = perviewImage.clacImgZoomParam(MAXWIDTH, MAXHEIGHT, img.offsetWidth, img.offsetHeight);
				img.style.width = rect.width + 'px';
				img.style.height = rect.height + 'px';
				img.style.visibility = "visible";
				var offsetWidth = (rect.width / 2) - (sWidth / 2);
				var offsetHeight = (rect.height / 2) - (sHeight / 2);
				var obj = {
					x: offsetWidth,
					y: offsetHeight,
					x2: offsetWidth + sWidth,
					y2: offsetHeight + sHeight,
					w: sWidth,
					h: sHeight
				};
				$('img.preview').attr('src', img.src);
				$(img).Jcrop(params, function() {
					upload.jcropApi = this;
					this.animateTo([obj.x, obj.y, obj.x2, obj.y2]);
					upload.setImageCropperObj(obj);
					var bounds = this.getBounds(),
						boundx = bounds[0],
						boundy = bounds[1],
						jcrop_api = this;
					//$('#preview-pane').appendTo(jcrop_api.ui.holder);
				});
			};

			var reader = new FileReader();
			reader.onload = function(evt) {
				img.src = evt.target.result;
			};
			reader.readAsDataURL(file.files[0]);
		} else { //此处为IE6，7，8，9的操作  
			file.select();
			var src = dcmt.selection.createRange().text;

			var div_sFilter = "progid:DXImageTransform.Microsoft.AlphaImageLoader(sizingMethod='scale',src='" + src + "')";
			var img_sFilter = "progid:DXImageTransform.Microsoft.AlphaImageLoader(sizingMethod='image',src='" + src + "')";
			preview_div.innerHTML = "";
			var img = document.createElement("div");
			preview_div.appendChild(img);
			img.style.filter = img_sFilter;
			img.style.visibility = "hidden";
			img.style.width = "100%";
			img.style.height = "100%";
			$('img.preview').css('filter', div_sFilter);
			//$('#preview-pane').css('right','0');
			function setImageDisplay() {
				var rect = perviewImage.clacImgZoomParam(MAXWIDTH, MAXHEIGHT, img.offsetWidth, img.offsetHeight);
				preview_div.innerHTML = "";
				var div = document.createElement("div");
				div.style.width = rect.width + 'px';
				div.style.height = rect.height + 'px';
				div.style.filter = div_sFilter;
				var offsetWidth = (rect.width / 2) - (sWidth / 2);
				var offsetHeight = (rect.height / 2) - (sHeight / 2);
				var obj = {
					x: offsetWidth,
					y: offsetHeight,
					x2: offsetWidth + sWidth,
					y2: offsetHeight + sHeight,
					w: sWidth,
					h: sHeight
				};
				preview_div.appendChild(div);

				$(div).Jcrop(params, function() {
					upload.jcropApi = this;
					this.animateTo([obj.x, obj.y, obj.x2, obj.y2]);
					upload.setImageCropperObj(obj);
				});
			}

			//图片加载计数  
			var tally = 0;

			var timer = window.setInterval(function() {
				if (img.offsetHeight != MAXHEIGHT) {
					window.clearInterval(timer);
					setImageDisplay();
				} else {
					tally++;
				}
				//如果超过两秒钟图片还不能加载，就停止当前的轮询  
				if (tally > 20) {
					window.clearInterval(timer);
					setImageDisplay();
				}
			}, 100);

			this.timers.push(timer);
		}
	},
	//按比例缩放图片  
	clacImgZoomParam: function(maxWidth, maxHeight, width, height) {
		var param = {
			width: width,
			height: height
		};
		if (width > maxWidth || height > maxHeight) {
			var rateWidth = width / maxWidth;
			var rateHeight = height / maxHeight;

			if (rateWidth > rateHeight) {
				param.width = maxWidth;
				param.height = Math.round(height / rateWidth);
			} else {
				param.width = Math.round(width / rateHeight);
				param.height = maxHeight;
			}
		}

		param.left = Math.round((maxWidth - param.width) / 2);
		param.top = Math.round((maxHeight - param.height) / 2);
		return param;
	},
	//创建图片预览元素  
	createPreviewElement: function( /*关闭图片名称*/ name, /*上传时的文件名*/ file, /*预览时的样式*/ style) {
		var img = document.createElement("div");
		//img.title = file;
		//img.style.overflow = "hidden";
		for (var s in style) {
			img.style[s] = style[s];
		}

		// var text = document.createElement("div");
		// text.style.width = style.width;
		// text.style.overflow = "hidden";
		// text.style.textOverflow = "ellipsis";
		// text.style.whiteSpace = "nowrap";
		// text.innerHTML = file;

		var main = document.createElement("div");
		main.appendChild(img);
		//main.appendChild(text);
		return main;
	},

	//获取上传文件大小  
	getFileSize: function( /*上传控件dom对象*/ file, /*上传控件所在的document对象*/ dcmt) {
		var fileSize;
		if (file.files && file.files[0]) {
			fileSize = file.files[0].size;
		} else {
			file.select();
			var src = dcmt.selection.createRange().text;
			try {
				var fso = new ActiveXObject("Scripting.FileSystemObject");
				var fileObj = fso.getFile(src);
				fileSize = fileObj.size;
			} catch (e) {
				return "error";
			}
		}
		fileSize = ((fileSize / 1024) + "").split(".")[0];
		return fileSize;
	}
};