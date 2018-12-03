/************************************************************************************************************************
文件说明：      51选校JS框架/类库
                注意：基于jQuery，必须先引入jQuery脚本才能使用
                集成了jQuery.cookie,juicer ，引用了本脚本，就不需要再引用jQuery.cookie.js,juicer.js
Author:         周少阳(Michael Zhou)
History:        2015/6/11 17:48:27 创建
                
*************************************************************************************************************************/



//-------------------------------------------------------------------------------------------
/*
jQuery.cookie
*/
(function (factory) {
    if (typeof define === 'function' && define.amd) {
        // AMD. Register as anonymous module.
        define(['jquery'], factory);
    } else {
        // Browser globals.
        factory(jQuery);
    }
}(function ($) {

    var pluses = /\+/g;

    function raw(s) {
        return s;
    }

    function decoded(s) {
        return decodeURIComponent(s.replace(pluses, ' '));
    }

    function converted(s) {
        if (s.indexOf('"') === 0) {
            // This is a quoted cookie as according to RFC2068, unescape
            s = s.slice(1, -1).replace(/\\"/g, '"').replace(/\\\\/g, '\\');
        }
        try {
            return config.json ? JSON.parse(s) : s;
        } catch (er) { }
    }

    var config = $.cookie = function (key, value, options) {

        // write
        if (value !== undefined) {
            options = $.extend({}, config.defaults, options);

            if (typeof options.expires === 'number') {
                var days = options.expires, t = options.expires = new Date();
                t.setDate(t.getDate() + days);
            }

            value = config.json ? JSON.stringify(value) : String(value);

            return (document.cookie = [
                config.raw ? key : encodeURIComponent(key),
                '=',
                config.raw ? value : encodeURIComponent(value),
                options.expires ? '; expires=' + options.expires.toUTCString() : '', // use expires attribute, max-age is not supported by IE
                options.path ? '; path=' + options.path : '',
                options.domain ? '; domain=' + options.domain : '',
                options.secure ? '; secure' : ''
            ].join(''));
        }

        // read
        var decode = config.raw ? raw : decoded;
        var cookies = document.cookie.split('; ');
        var result = key ? undefined : {};
        for (var i = 0, l = cookies.length; i < l; i++) {
            var parts = cookies[i].split('=');
            var name = decode(parts.shift());
            var cookie = decode(parts.join('='));

            if (key && key === name) {
                result = converted(cookie);
                break;
            }

            if (!key) {
                result[name] = converted(cookie);
            }
        }

        return result;
    };

    config.defaults = {};

    $.removeCookie = function (key, options) {
        if ($.cookie(key) !== undefined) {
            // Must not alter options, thus extending a fresh object...
            $.cookie(key, '', $.extend({}, options, { expires: -1 }));
            return true;
        }
        return false;
    };

}));
//-------------------------------------------------------------------------------------------

//-------------------------------------------------------------------------------------------
/*
juicer - 模板引擎
*/
(function () {

    // This is the main function for not only compiling but also rendering.
    // there's at least two parameters need to be provided, one is the tpl, 
    // another is the data, the tpl can either be a string, or an id like #id.
    // if only tpl was given, it'll return the compiled reusable function.
    // if tpl and data were given at the same time, it'll return the rendered 
    // result immediately.

    var juicer = function () {
        var args = [].slice.call(arguments);

        args.push(juicer.options);

        if (args[0].match(/^\s*#([\w:\-\.]+)\s*$/igm)) {
            args[0].replace(/^\s*#([\w:\-\.]+)\s*$/igm, function ($, $id) {
                var _document = document;
                var elem = _document && _document.getElementById($id);
                args[0] = elem ? (elem.value || elem.innerHTML) : $;
            });
        }

        if (typeof (document) !== 'undefined' && document.body) {
            juicer.compile.call(juicer, document.body.innerHTML);
        }

        if (arguments.length == 1) {
            return juicer.compile.apply(juicer, args);
        }

        if (arguments.length >= 2) {
            return juicer.to_html.apply(juicer, args);
        }
    };

    var __escapehtml = {
        escapehash: {
            '<': '&lt;',
            '>': '&gt;',
            '&': '&amp;',
            '"': '&quot;',
            "'": '&#x27;',
            '/': '&#x2f;'
        },
        escapereplace: function (k) {
            return __escapehtml.escapehash[k];
        },
        escaping: function (str) {
            return typeof (str) !== 'string' ? str : str.replace(/[&<>"]/igm, this.escapereplace);
        },
        detection: function (data) {
            return typeof (data) === 'undefined' ? '' : data;
        }
    };

    var __throw = function (error) {
        if (typeof (console) !== 'undefined') {
            if (console.warn) {
                console.warn(error);
                return;
            }

            if (console.log) {
                console.log(error);
                return;
            }
        }

        throw (error);
    };

    var __creator = function (o, proto) {
        o = o !== Object(o) ? {} : o;

        if (o.__proto__) {
            o.__proto__ = proto;
            return o;
        }

        var empty = function () { };
        var n = Object.create ?
            Object.create(proto) :
            new (empty.prototype = proto, empty);

        for (var i in o) {
            if (o.hasOwnProperty(i)) {
                n[i] = o[i];
            }
        }

        return n;
    };

    var annotate = function (fn) {
        var FN_ARGS = /^function\s*[^\(]*\(\s*([^\)]*)\)/m;
        var FN_ARG_SPLIT = /,/;
        var FN_ARG = /^\s*(_?)(\S+?)\1\s*$/;
        var FN_BODY = /^function[^{]+{([\s\S]*)}/m;
        var STRIP_COMMENTS = /((\/\/.*$)|(\/\*[\s\S]*?\*\/))/mg;
        var args = [],
            fnText,
            fnBody,
            argDecl;

        if (typeof fn === 'function') {
            if (fn.length) {
                fnText = fn.toString();
            }
        } else if (typeof fn === 'string') {
            fnText = fn;
        }

        fnText = fnText.replace(STRIP_COMMENTS, '');
        fnText = fnText.trim();
        argDecl = fnText.match(FN_ARGS);
        fnBody = fnText.match(FN_BODY)[1].trim();

        for (var i = 0; i < argDecl[1].split(FN_ARG_SPLIT).length; i++) {
            var arg = argDecl[1].split(FN_ARG_SPLIT)[i];
            arg.replace(FN_ARG, function (all, underscore, name) {
                args.push(name);
            });
        }

        return [args, fnBody];
    };

    juicer.__cache = {};
    juicer.version = '0.6.9-stable';
    juicer.settings = {};

    juicer.tags = {
        operationOpen: '{@',
        operationClose: '}',
        interpolateOpen: '\\${',
        interpolateClose: '}',
        noneencodeOpen: '\\$\\${',
        noneencodeClose: '}',
        commentOpen: '\\{#',
        commentClose: '\\}'
    };

    juicer.options = {
        cache: true,
        strip: true,
        errorhandling: true,
        detection: true,
        _method: __creator({
            __escapehtml: __escapehtml,
            __throw: __throw,
            __juicer: juicer
        }, {})
    };

    juicer.tagInit = function () {
        var forstart = juicer.tags.operationOpen + 'each\\s*([^}]*?)\\s*as\\s*(\\w*?)\\s*(,\\s*\\w*?)?' + juicer.tags.operationClose;
        var forend = juicer.tags.operationOpen + '\\/each' + juicer.tags.operationClose;
        var ifstart = juicer.tags.operationOpen + 'if\\s*([^}]*?)' + juicer.tags.operationClose;
        var ifend = juicer.tags.operationOpen + '\\/if' + juicer.tags.operationClose;
        var elsestart = juicer.tags.operationOpen + 'else' + juicer.tags.operationClose;
        var elseifstart = juicer.tags.operationOpen + 'else if\\s*([^}]*?)' + juicer.tags.operationClose;
        var interpolate = juicer.tags.interpolateOpen + '([\\s\\S]+?)' + juicer.tags.interpolateClose;
        var noneencode = juicer.tags.noneencodeOpen + '([\\s\\S]+?)' + juicer.tags.noneencodeClose;
        var inlinecomment = juicer.tags.commentOpen + '[^}]*?' + juicer.tags.commentClose;
        var rangestart = juicer.tags.operationOpen + 'each\\s*(\\w*?)\\s*in\\s*range\\(([^}]+?)\\s*,\\s*([^}]+?)\\)' + juicer.tags.operationClose;
        var include = juicer.tags.operationOpen + 'include\\s*([^}]*?)\\s*,\\s*([^}]*?)' + juicer.tags.operationClose;
        var helperRegisterStart = juicer.tags.operationOpen + 'helper\\s*([^}]*?)\\s*' + juicer.tags.operationClose;
        var helperRegisterBody = '([\\s\\S]*?)';
        var helperRegisterEnd = juicer.tags.operationOpen + '\\/helper' + juicer.tags.operationClose;

        juicer.settings.forstart = new RegExp(forstart, 'igm');
        juicer.settings.forend = new RegExp(forend, 'igm');
        juicer.settings.ifstart = new RegExp(ifstart, 'igm');
        juicer.settings.ifend = new RegExp(ifend, 'igm');
        juicer.settings.elsestart = new RegExp(elsestart, 'igm');
        juicer.settings.elseifstart = new RegExp(elseifstart, 'igm');
        juicer.settings.interpolate = new RegExp(interpolate, 'igm');
        juicer.settings.noneencode = new RegExp(noneencode, 'igm');
        juicer.settings.inlinecomment = new RegExp(inlinecomment, 'igm');
        juicer.settings.rangestart = new RegExp(rangestart, 'igm');
        juicer.settings.include = new RegExp(include, 'igm');
        juicer.settings.helperRegister = new RegExp(helperRegisterStart + helperRegisterBody + helperRegisterEnd, 'igm');
    };

    juicer.tagInit();

    // Using this method to set the options by given conf-name and conf-value,
    // you can also provide more than one key-value pair wrapped by an object.
    // this interface also used to custom the template tag delimater, for this
    // situation, the conf-name must begin with tag::, for example: juicer.set
    // ('tag::operationOpen', '{@').

    juicer.set = function (conf, value) {
        var that = this;

        var escapePattern = function (v) {
            return v.replace(/[\$\(\)\[\]\+\^\{\}\?\*\|\.]/igm, function ($) {
                return '\\' + $;
            });
        };

        var set = function (conf, value) {
            var tag = conf.match(/^tag::(.*)$/i);

            if (tag) {
                that.tags[tag[1]] = escapePattern(value);
                that.tagInit();
                return;
            }

            that.options[conf] = value;
        };

        if (arguments.length === 2) {
            set(conf, value);
            return;
        }

        if (conf === Object(conf)) {
            for (var i in conf) {
                if (conf.hasOwnProperty(i)) {
                    set(i, conf[i]);
                }
            }
        }
    };

    // Before you're using custom functions in your template like ${name | fnName},
    // you need to register this fn by juicer.register('fnName', fn).

    juicer.register = function (fname, fn) {
        var _method = this.options._method;

        if (_method.hasOwnProperty(fname)) {
            return false;
        }

        return _method[fname] = fn;
    };

    // remove the registered function in the memory by the provided function name.
    // for example: juicer.unregister('fnName').

    juicer.unregister = function (fname) {
        var _method = this.options._method;

        if (_method.hasOwnProperty(fname)) {
            return delete _method[fname];
        }
    };

    juicer.template = function (options) {
        var that = this;

        this.options = options;

        this.__interpolate = function (_name, _escape, options) {
            var _define = _name.split('|'), _fn = _define[0] || '', _cluster;

            if (_define.length > 1) {
                _name = _define.shift();
                _cluster = _define.shift().split(',');
                _fn = '_method.' + _cluster.shift() + '.call({}, ' + [_name].concat(_cluster) + ')';
            }

            return '<%= ' + (_escape ? '_method.__escapehtml.escaping' : '') + '(' +
                        (!options || options.detection !== false ? '_method.__escapehtml.detection' : '') + '(' +
                            _fn +
                        ')' +
                    ')' +
                ' %>';
        };

        this.__removeShell = function (tpl, options) {
            var _counter = 0;

            tpl = tpl
                // inline helper register
                .replace(juicer.settings.helperRegister, function ($, helperName, fnText) {
                    var anno = annotate(fnText);
                    var fnArgs = anno[0];
                    var fnBody = anno[1];
                    var fn = new Function(fnArgs.join(','), fnBody);

                    juicer.register(helperName, fn);
                    return $;
                })

                // for expression
                .replace(juicer.settings.forstart, function ($, _name, alias, key) {
                    var alias = alias || 'value', key = key && key.substr(1);
                    var _iterate = 'i' + _counter++;
                    return '<% ~function() {' +
                                'for(var ' + _iterate + ' in ' + _name + ') {' +
                                    'if(' + _name + '.hasOwnProperty(' + _iterate + ')) {' +
                                        'var ' + alias + '=' + _name + '[' + _iterate + '];' +
                                        (key ? ('var ' + key + '=' + _iterate + ';') : '') +
                            ' %>';
                })
                .replace(juicer.settings.forend, '<% }}}(); %>')

                // if expression
                .replace(juicer.settings.ifstart, function ($, condition) {
                    return '<% if(' + condition + ') { %>';
                })
                .replace(juicer.settings.ifend, '<% } %>')

                // else expression
                .replace(juicer.settings.elsestart, function ($) {
                    return '<% } else { %>';
                })

                // else if expression
                .replace(juicer.settings.elseifstart, function ($, condition) {
                    return '<% } else if(' + condition + ') { %>';
                })

                // interpolate without escape
                .replace(juicer.settings.noneencode, function ($, _name) {
                    return that.__interpolate(_name, false, options);
                })

                // interpolate with escape
                .replace(juicer.settings.interpolate, function ($, _name) {
                    return that.__interpolate(_name, true, options);
                })

                // clean up comments
                .replace(juicer.settings.inlinecomment, '')

                // range expression
                .replace(juicer.settings.rangestart, function ($, _name, start, end) {
                    var _iterate = 'j' + _counter++;
                    return '<% ~function() {' +
                                'for(var ' + _iterate + '=' + start + ';' + _iterate + '<' + end + ';' + _iterate + '++) {{' +
                                    'var ' + _name + '=' + _iterate + ';' +
                            ' %>';
                })

                // include sub-template
                .replace(juicer.settings.include, function ($, tpl, data) {
                    // compatible for node.js
                    if (tpl.match(/^file\:\/\//igm)) return $;
                    return '<%= _method.__juicer(' + tpl + ', ' + data + '); %>';
                });

            // exception handling
            if (!options || options.errorhandling !== false) {
                tpl = '<% try { %>' + tpl;
                tpl += '<% } catch(e) {_method.__throw("Juicer Render Exception: "+e.message);} %>';
            }

            return tpl;
        };

        this.__toNative = function (tpl, options) {
            return this.__convert(tpl, !options || options.strip);
        };

        this.__lexicalAnalyze = function (tpl) {
            var buffer = [];
            var method = [];
            var prefix = '';
            var reserved = [
                'if', 'each', '_', '_method', 'console',
                'break', 'case', 'catch', 'continue', 'debugger', 'default', 'delete', 'do',
                'finally', 'for', 'function', 'in', 'instanceof', 'new', 'return', 'switch',
                'this', 'throw', 'try', 'typeof', 'var', 'void', 'while', 'with', 'null', 'typeof',
                'class', 'enum', 'export', 'extends', 'import', 'super', 'implements', 'interface',
                'let', 'package', 'private', 'protected', 'public', 'static', 'yield', 'const', 'arguments',
                'true', 'false', 'undefined', 'NaN'
            ];

            var indexOf = function (array, item) {
                if (Array.prototype.indexOf && array.indexOf === Array.prototype.indexOf) {
                    return array.indexOf(item);
                }

                for (var i = 0; i < array.length; i++) {
                    if (array[i] === item) return i;
                }

                return -1;
            };

            var variableAnalyze = function ($, statement) {
                statement = statement.match(/\w+/igm)[0];

                if (indexOf(buffer, statement) === -1 && indexOf(reserved, statement) === -1 && indexOf(method, statement) === -1) {

                    // avoid re-declare native function, if not do this, template 
                    // `{@if encodeURIComponent(name)}` could be throw undefined.

                    if (typeof (window) !== 'undefined' && typeof (window[statement]) === 'function' && window[statement].toString().match(/^\s*?function \w+\(\) \{\s*?\[native code\]\s*?\}\s*?$/i)) {
                        return $;
                    }

                    // compatible for node.js
                    if (typeof (global) !== 'undefined' && typeof (global[statement]) === 'function' && global[statement].toString().match(/^\s*?function \w+\(\) \{\s*?\[native code\]\s*?\}\s*?$/i)) {
                        return $;
                    }

                    // avoid re-declare registered function, if not do this, template 
                    // `{@if registered_func(name)}` could be throw undefined.

                    if (typeof (juicer.options._method[statement]) === 'function' || juicer.options._method.hasOwnProperty(statement)) {
                        method.push(statement);
                        return $;
                    }

                    buffer.push(statement); // fuck ie
                }

                return $;
            };

            tpl.replace(juicer.settings.forstart, variableAnalyze).
                replace(juicer.settings.interpolate, variableAnalyze).
                replace(juicer.settings.ifstart, variableAnalyze).
                replace(juicer.settings.elseifstart, variableAnalyze).
                replace(juicer.settings.include, variableAnalyze).
                replace(/[\+\-\*\/%!\?\|\^&~<>=,\(\)\[\]]\s*([A-Za-z_]+)/igm, variableAnalyze);

            for (var i = 0; i < buffer.length; i++) {
                prefix += 'var ' + buffer[i] + '=_.' + buffer[i] + ';';
            }

            for (var i = 0; i < method.length; i++) {
                prefix += 'var ' + method[i] + '=_method.' + method[i] + ';';
            }

            return '<% ' + prefix + ' %>';
        };

        this.__convert = function (tpl, strip) {
            var buffer = [].join('');

            buffer += "'use strict';"; // use strict mode
            buffer += "var _=_||{};";
            buffer += "var _out='';_out+='";

            if (strip !== false) {
                buffer += tpl
                    .replace(/\\/g, "\\\\")
                    .replace(/[\r\t\n]/g, " ")
                    .replace(/'(?=[^%]*%>)/g, "\t")
                    .split("'").join("\\'")
                    .split("\t").join("'")
                    .replace(/<%=(.+?)%>/g, "';_out+=$1;_out+='")
                    .split("<%").join("';")
                    .split("%>").join("_out+='") +
                    "';return _out;";

                return buffer;
            }

            buffer += tpl
                    .replace(/\\/g, "\\\\")
                    .replace(/[\r]/g, "\\r")
                    .replace(/[\t]/g, "\\t")
                    .replace(/[\n]/g, "\\n")
                    .replace(/'(?=[^%]*%>)/g, "\t")
                    .split("'").join("\\'")
                    .split("\t").join("'")
                    .replace(/<%=(.+?)%>/g, "';_out+=$1;_out+='")
                    .split("<%").join("';")
                    .split("%>").join("_out+='") +
                    "';return _out.replace(/[\\r\\n]\\s+[\\r\\n]/g, '\\r\\n');";

            return buffer;
        };

        this.parse = function (tpl, options) {
            var _that = this;

            if (!options || options.loose !== false) {
                tpl = this.__lexicalAnalyze(tpl) + tpl;
            }

            tpl = this.__removeShell(tpl, options);
            tpl = this.__toNative(tpl, options);

            this._render = new Function('_, _method', tpl);

            this.render = function (_, _method) {
                if (!_method || _method !== that.options._method) {
                    _method = __creator(_method, that.options._method);
                }

                return _that._render.call(this, _, _method);
            };

            return this;
        };
    };

    juicer.compile = function (tpl, options) {
        if (!options || options !== this.options) {
            options = __creator(options, this.options);
        }

        try {
            var engine = this.__cache[tpl] ?
                this.__cache[tpl] :
                new this.template(this.options).parse(tpl, options);

            if (!options || options.cache !== false) {
                this.__cache[tpl] = engine;
            }

            return engine;

        } catch (e) {
            __throw('Juicer Compile Exception: ' + e.message);

            return {
                render: function () { } // noop
            };
        }
    };

    juicer.to_html = function (tpl, data, options) {
        if (!options || options !== this.options) {
            options = __creator(options, this.options);
        }

        return this.compile(tpl, options).render(data, options._method);
    };

    // avoid memory leak for node.js
    if (typeof (global) !== 'undefined' && typeof (window) === 'undefined') {
        juicer.set('cache', false);
    }

    typeof (module) !== 'undefined' && module.exports ? module.exports = juicer : this.juicer = juicer;

})();
//-------------------------------------------------------------------------------------------

//-------------------------------------------------------------------------------------------
/**
 * @classDescription 超级Marquee，可做图片导航，图片轮换
 * @author Aken Li(www.kxbd.com)
 * @date 2009-07-27
 * @dependence jQuery 1.3.2
 * @DOM
 *  	<div id="marquee">
 *  		<ul>
 *   			<li></li>
 *   			<li></li>
 *  		</ul>
 *  	</div>
 * @CSS
 *  	#marquee {width:200px;height:50px;overflow:hidden;}
 * @Usage
 *  	$('#marquee').kxbdSuperMarquee(options);
 * @options
 *		distance:200,//一次滚动的距离
 *		duration:20,//缓动效果，单次移动时间，越小速度越快，为0时无缓动效果
 *		time:5,//停顿时间，单位为秒
 *		direction: 'left',//滚动方向，'left','right','up','down'
 *		scrollAmount:1,//步长
 *		scrollDelay:20//时长，单位为毫秒
 *		isEqual:true,//所有滚动的元素长宽是否相等,true,false
 *		loop: 0,//循环滚动次数，0时无限
 *		btnGo:{left:'#goL',right:'#goR'},//控制方向的按钮ID，有四个属性left,right,up,down分别对应四个方向
 *		eventGo:'click',//鼠标事件
 *		controlBtn:{left:'#goL',right:'#goR'},//控制加速滚动的按钮ID，有四个属性left,right,up,down分别对应四个方向
 *		newAmount:4,//加速滚动的步长
 *		eventA:'mouseenter',//鼠标事件，加速
 *		eventB:'mouseleave',//鼠标事件，原速
 *		navId:'#marqueeNav', //导航容器ID，导航DOM:<ul><li>1</li><li>2</li><ul>,导航CSS:.navOn
 *		eventNav:'click' //导航事件
 */
(function ($) {

    $.fn.kxbdSuperMarquee = function (options) {
        var opts = $.extend({}, $.fn.kxbdSuperMarquee.defaults, options);

        return this.each(function () {
            var $marquee = $(this);//滚动元素容器
            var _scrollObj = $marquee.get(0);//滚动元素容器DOM
            var scrollW = $marquee.width();//滚动元素容器的宽度
            var scrollH = $marquee.height();//滚动元素容器的高度
            var $element = $marquee.children(); //滚动元素
            var $kids = $element.children();//滚动子元素
            var scrollSize = 0;//滚动元素尺寸
            var _type = (opts.direction == 'left' || opts.direction == 'right') ? 1 : 0;//滚动类型，1左右，0上下
            var scrollId, rollId, isMove, marqueeId;
            var t, b, c, d, e; //滚动动画的参数,t:当前时间，b:开始的位置，c:改变的位置，d:持续的时间，e:结束的位置
            var _size, _len; //子元素的尺寸与个数
            var $nav, $navBtns;
            var arrPos = [];
            var numView = 0; //当前所看子元素
            var numRoll = 0; //轮换的次数
            var numMoved = 0;//已经移动的距离

            //防止滚动子元素比滚动元素宽而取不到实际滚动子元素宽度
            $element.css(_type ? 'width' : 'height', 10000);
            //获取滚动元素的尺寸
            var navHtml = '<ul>';
            if (opts.isEqual) {
                _size = $kids[_type ? 'outerWidth' : 'outerHeight']();
                _len = $kids.length;
                scrollSize = _size * _len;
                for (var i = 0; i < _len; i++) {
                    arrPos.push(i * _size);
                    navHtml += '<li>' + (i + 1) + '</li>';
                }
            } else {
                $kids.each(function (i) {
                    arrPos.push(scrollSize);
                    scrollSize += $(this)[_type ? 'outerWidth' : 'outerHeight']();
                    navHtml += '<li>' + (i + 1) + '</li>';
                });
            }
            navHtml += '</ul>';

            //滚动元素总尺寸小于容器尺寸，不滚动
            if (scrollSize < (_type ? scrollW : scrollH)) return;
            //克隆滚动子元素将其插入到滚动元素后，并设定滚动元素宽度
            $element.append($kids.clone()).css(_type ? 'width' : 'height', scrollSize * 2);

            //轮换导航
            if (opts.navId) {
                $nav = $(opts.navId).append(navHtml).hover(stop, start);
                $navBtns = $('li', $nav);
                $navBtns.each(function (i) {
                    $(this).bind(opts.eventNav, function () {
                        if (isMove) return;
                        if (numView == i) return;
                        rollFunc(arrPos[i]);
                        $navBtns.eq(numView).removeClass('navOn');
                        numView = i;
                        $(this).addClass('navOn');
                    });
                });
                $navBtns.eq(numView).addClass('navOn');
            }

            //设定初始位置
            if (opts.direction == 'right' || opts.direction == 'down') {
                _scrollObj[_type ? 'scrollLeft' : 'scrollTop'] = scrollSize;
            } else {
                _scrollObj[_type ? 'scrollLeft' : 'scrollTop'] = 0;
            }

            if (opts.isMarquee) {
                //滚动开始
                //marqueeId = setInterval(scrollFunc, opts.scrollDelay);
                marqueeId = setTimeout(scrollFunc, opts.scrollDelay);
                //鼠标划过停止滚动
                $marquee.hover(
					function () {
					    clearInterval(marqueeId);
					},
					function () {
					    //marqueeId = setInterval(scrollFunc, opts.scrollDelay);
					    clearInterval(marqueeId);
					    marqueeId = setTimeout(scrollFunc, opts.scrollDelay);
					}
				);

                //控制加速运动
                if (opts.controlBtn) {
                    $.each(opts.controlBtn, function (i, val) {
                        $(val).bind(opts.eventA, function () {
                            opts.direction = i;
                            opts.oldAmount = opts.scrollAmount;
                            opts.scrollAmount = opts.newAmount;
                        }).bind(opts.eventB, function () {
                            opts.scrollAmount = opts.oldAmount;
                        });
                    });
                }
            } else {
                if (opts.isAuto) {
                    //轮换开始
                    start();

                    //鼠标划过停止轮换
                    $marquee.hover(stop, start);
                }

                //控制前后走
                if (opts.btnGo) {
                    $.each(opts.btnGo, function (i, val) {
                        $(val).bind(opts.eventGo, function () {
                            if (isMove == true) return;
                            opts.direction = i;
                            rollFunc();
                            if (opts.isAuto) {
                                stop();
                                start();
                            }
                        });
                    });
                }
            }

            function scrollFunc() {
                var _dir = (opts.direction == 'left' || opts.direction == 'right') ? 'scrollLeft' : 'scrollTop';

                if (opts.isMarquee) {
                    if (opts.loop > 0) {
                        numMoved += opts.scrollAmount;
                        if (numMoved > scrollSize * opts.loop) {
                            _scrollObj[_dir] = 0;
                            return clearInterval(marqueeId);
                        }
                    }
                    var newPos = _scrollObj[_dir] + (opts.direction == 'left' || opts.direction == 'up' ? 1 : -1) * opts.scrollAmount;
                } else {
                    if (opts.duration) {
                        if (t++ < d) {
                            isMove = true;
                            var newPos = Math.ceil(easeOutQuad(t, b, c, d));
                            if (t == d) {
                                newPos = e;
                            }
                        } else {
                            newPos = e;
                            clearInterval(scrollId);
                            isMove = false;
                            return;
                        }
                    } else {
                        var newPos = e;
                        clearInterval(scrollId);
                    }
                }

                if (opts.direction == 'left' || opts.direction == 'up') {
                    if (newPos >= scrollSize) {
                        newPos -= scrollSize;
                    }
                } else {
                    if (newPos <= 0) {
                        newPos += scrollSize;
                    }
                }
                _scrollObj[_dir] = newPos;

                if (opts.isMarquee) {
                    marqueeId = setTimeout(scrollFunc, opts.scrollDelay);
                } else if (t < d) {
                    if (scrollId) clearTimeout(scrollId);
                    scrollId = setTimeout(scrollFunc, opts.scrollDelay);
                } else {
                    isMove = false;
                }

            };

            function rollFunc(pPos) {
                isMove = true;
                var _dir = (opts.direction == 'left' || opts.direction == 'right') ? 'scrollLeft' : 'scrollTop';
                var _neg = opts.direction == 'left' || opts.direction == 'up' ? 1 : -1;

                numRoll = numRoll + _neg;
                //得到当前所看元素序号并改变导航CSS
                if (pPos == undefined && opts.navId) {
                    $navBtns.eq(numView).removeClass('navOn');
                    numView += _neg;
                    if (numView >= _len) {
                        numView = 0;
                    } else if (numView < 0) {
                        numView = _len - 1;
                    }
                    $navBtns.eq(numView).addClass('navOn');
                    numRoll = numView;
                }

                var _temp = numRoll < 0 ? scrollSize : 0;
                t = 0;
                b = _scrollObj[_dir];
                //c=(pPos != undefined)?pPos:_neg*opts.distance;
                e = (pPos != undefined) ? pPos : _temp + (opts.distance * numRoll) % scrollSize;
                if (_neg == 1) {
                    if (e > b) {
                        c = e - b;
                    } else {
                        c = e + scrollSize - b;
                    }
                } else {
                    if (e > b) {
                        c = e - scrollSize - b;
                    } else {
                        c = e - b;
                    }
                }
                d = opts.duration;

                //scrollId = setInterval(scrollFunc, opts.scrollDelay);
                if (scrollId) clearTimeout(scrollId);
                scrollId = setTimeout(scrollFunc, opts.scrollDelay);
            }

            function start() {
                rollId = setInterval(function () {
                    rollFunc();
                }, opts.time * 1000);
            }
            function stop() {
                clearInterval(rollId);
            }

            function easeOutQuad(t, b, c, d) {
                return -c * (t /= d) * (t - 2) + b;
            }

            function easeOutQuint(t, b, c, d) {
                return c * ((t = t / d - 1) * t * t * t * t + 1) + b;
            }

        });
    };
    $.fn.kxbdSuperMarquee.defaults = {
        isMarquee: false,//是否为Marquee
        isEqual: true,//所有滚动的元素长宽是否相等,true,false
        loop: 0,//循环滚动次数，0时无限
        newAmount: 3,//加速滚动的步长
        eventA: 'mousedown',//鼠标事件，加速
        eventB: 'mouseup',//鼠标事件，原速
        isAuto: true,//是否自动轮换
        time: 5,//停顿时间，单位为秒
        duration: 50,//缓动效果，单次移动时间，越小速度越快，为0时无缓动效果
        eventGo: 'click', //鼠标事件，向前向后走
        direction: 'left',//滚动方向，'left','right','up','down'
        scrollAmount: 1,//步长
        scrollDelay: 10,//时长
        eventNav: 'click'//导航事件
    };

    $.fn.kxbdSuperMarquee.setDefaults = function (settings) {
        $.extend($.fn.kxbdSuperMarquee.defaults, settings);
    };

})(jQuery);
//-------------------------------------------------------------------------------------------


(function (window, undefined) {
    //-------------------------------------------------------------------------------------------
    var document = window.document,
	navigator = window.navigator,
	location = window.location;

    //-------------------------------------------------------------------------------------------
    /*
    模拟c# String.Format函数
    */
    String.prototype.format = function (args) {
        var str = this;
        if (args && args.length > 0) {
            for (var i = 0; i < args.length; i++) {
                str = str.replace('{' + i + '}', args[i]);
            }
        }
        return str;
    };
    //-------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------
    // 对Date的扩展，将 Date 转化为指定格式的String
    Date.prototype.Format = function (formatStr) {
        var dt = this;
        var fyear = dt.getFullYear();
        var year = dt.getYear();
        var month = dt.getMonth() + 1;
        var wd = dt.getDay();
        var day = dt.getDate();
        var hour = dt.getHours();
        var minute = dt.getMinutes();
        var second = dt.getSeconds();

        var str = formatStr;
        var weeks = ['日', '一', '二', '三', '四', '五', '六'];

        str = str.replace(/yyyy|YYYY/, fyear);
        str = str.replace(/yy|YY/, (year % 100) > 9 ? (year % 100).toString() : '0' + (year % 100));

        str = str.replace(/MM/, month > 9 ? month.toString() : ('0' + month));
        str = str.replace(/M/g, month);

        str = str.replace(/w|W/g, weeks[wd]);

        str = str.replace(/dd|DD/, day > 9 ? day.toString() : ('0' + day));
        str = str.replace(/d|D/g, day);

        str = str.replace(/hh|HH/, hour > 9 ? hour.toString() : ('0' + hour));
        str = str.replace(/h|H/g, hour);
        str = str.replace(/mm/, minute > 9 ? minute.toString() : ('0' + minute));
        str = str.replace(/m/g, minute);

        str = str.replace(/ss|SS/, second > 9 ? second.toString() : ('0' + second));
        str = str.replace(/s|S/g, second);
        return str;
    }
    //-------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------
    /*
    获取事件源
    */
    function getEvent() {
        if (document.all) // IE 
        {
            return window.event;

        }
        var func = getEvent.caller; // 返回调用本函数的函数 

        while (func != null) {
            // Firefox 中一个隐含的对象 arguments，第一个参数为 event 对象  
            var arg0 = func.arguments[0];

            if (arg0) {
                if ((arg0.constructor == Event || arg0.constructor == MouseEvent) || (typeof (arg0) == "object" && arg0.preventDefault && arg0.stopPropagation)) {
                    return arg0;
                }
            }
            func = func.caller;
        }
        return null;
    }
    /*
    获取事件源对象
    @oEvent[Ojbect] 事件源对象
    */
    function getEventObj(oEvent) {
        var srcObj = ((oEvent.srcElement) ? oEvent.srcElement : oEvent.target);
        return srcObj;
    }
    //-------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------
    /*
    变量不为undefined和null
    */
    function isSet(v) {
        if (typeof (v) == 'undefined' || v == null) {
            return false;
        }
        return true;
    }
    /*
    变量为函数
    */
    function isFunc(v) {
        if (typeof (v) == 'function') {
            return true;
        }
        return false;
    }
    //-------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------
    /*
    合并json到目标json中
    */
    function mergeJson(newJson, targetJson) {
        var resultJson = {};
        for (var key in targetJson) {
            if (newJson[key]) {
                resultJson[key] = newJson[key];
            }
            else {
                resultJson[key] = targetJson[key];
            }
        }
        return resultJson;
    }
    //-------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------
    /*
    初始化互相对应的菜单与容器ID集合，点击菜单显示或隐藏容器
    @panels[json] key:menuId, value:panelId
    @options[json] key:配置名, value:配置值。[可选]
                   eventType:触发菜单事件的事件类型，默认为点击-"click"
                   activeMenuCss:菜单触发后的激活样式名
                   callback:切换菜单后的回调函数
    */
    function panelSwitch(panels, options) {
        var self = this;
        self.panels = panels || {};
        self.options = mergeJson(options, self.options);

        for (var menuId in panels) {
            $('#' + menuId).bind(self.options.eventType, function () {
                var showMenuId = $(this).attr('id');
                if (!showMenuId)
                    return;
                self.show(showMenuId);
            });
        }
    }
    panelSwitch.prototype.panels = null;
    panelSwitch.prototype.options = {
        mode: 0,
        animateTimespan: 0,
        eventType: 'click',
        activeMenuCss: '',
        callback: null
    };
    /*
    触发菜单显示相应的容器
    @showMenuId[string] 点击的菜单元素id
    */
    panelSwitch.prototype.show = function (showMenuId) {
        var self = this;

        for (var menuId in self.panels) {
            var panelId = self.panels[menuId];
            if (menuId == showMenuId) {
                if (self.options.activeMenuCss.length > 0) {
                    $('#' + menuId).addClass(self.options.activeMenuCss);
                }
                switch (self.options.mode) {
                    case 0:
                    default:
                        if (self.options.animateTimespan > 0) {
                            $('#' + panelId).show(self.options.animateTimespan);
                        }
                        else {
                            $('#' + panelId).show();
                        }
                        break;
                    case 1:
                        if (self.options.animateTimespan > 0) {
                            $('#' + panelId).fadeIn(self.options.animateTimespan);
                        }
                        else {
                            $('#' + panelId).fadeIn();
                        }
                        break;
                }
                if (self.options.callback != null) {
                    self.options.callback(showMenuId, panelId);
                }
            }
            else {
                if (self.options.activeMenuCss.length > 0) {
                    $('#' + menuId).removeClass(self.options.activeMenuCss);
                }
                $('#' + panelId).hide();
            }
        }
    };
    //-------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------
    /*
    移至移出指定菜单显示或隐藏容器块
    @menuList[json] key:menuId, value:panelId
    @options[json] key:配置名, value:配置值。[可选]
                   triggerMethod:触发显示容器块的事件类型，默认为鼠标移过-"mouseover"
                   cancelMethod:触发隐藏容器块的事件类型，默认为鼠标移出-"mouseout"
                   position:容器块相对菜单按钮的位置 - 1:菜单下方(默认),2:菜单上方,3:菜单左边,4:菜单右边,5:原始位置
                   isPosition:是否相对于父元素位置
                   isFixed:是否静止定位
                   fadeInMilSeconds:淡入毫秒数,0则不使用淡入效果
                   fadeOutMilSeconds:淡出毫秒数,0则不使用淡出效果
                   activeMenuCss:激活时的菜单的样式
                   currentMenuId:当前激活的菜单id
                   defaultShow:默认是否激活当前指定的菜单
                   fixTopPix:附加的修正top像素
                   fixLeftPix:附加的修正left像素
                   forceTopPix:强制设置top位置
                   forceLeftPix:强制设置left位置
    */
    function popupBox(menuList, options) {
        var self = this;

        self.menuList = menuList || {};
        self.options = mergeJson(options, self.options);

        for (var menuId in menuList) {
            var boxId = menuList[menuId];

            var menu = $("#" + menuId);
            var box = $("#" + boxId);

            menu.bind(self.options.triggerMethod, function () {
                self.show($(this).attr("id"));
            });
            menu.bind(self.options.cancelMethod, function () {
                self.hide($(this).attr("id"));
            });
            box.bind("mouseover", function () {
                self.show($(this).attr("id"));
            });
            box.bind("mouseout", function () {
                self.hide($(this).attr("id"));
            });
        }

        if (self.options.defaultShow == true) {
            self.show(self.options.currentMenuId);
        }
    }
    popupBox.prototype.hoverTimer = {};
    popupBox.prototype.menuList = null;
    popupBox.prototype.options = {
        triggerMethod: "mouseover",
        cancelMethod: "mouseout",
        position: 1,
        isPosition: false,
        isFixed: false,
        fadeInMilSeconds: 0,
        fadeOutMilSeconds: 0,
        activeMenuCss: '',
        currentMenuId: '',
        defaultShow: false,
        fixTopPix: 0,
        fixLeftPix: 0,
        forceTopPix: null,
        forceLeftPix: null
    };
    /*
    检测指定元素id是菜单id还是容器id
    返回值: { menu: object, box: object }
    */
    popupBox.prototype._getMenuAndBox = function (objId) {
        var self = this;
        var mid = null, bid = null;
        for (var menuId in self.menuList) {
            var boxId = self.menuList[menuId];
            if (menuId == objId) {
                mid = menuId;
                bid = boxId;
            }
            if (boxId == objId) {
                mid = menuId;
                bid = boxId;
            }
        }

        if (mid != null && bid != null) {
            var m = $('#' + mid);
            var b = $('#' + bid);
            return { "menu": m, "box": b };
        }
        else {
            return null;
        }
    };
    /*
    显示容器块
    */
    popupBox.prototype.show = function (objId) {
        var self = this;
        var menuAndBox = self._getMenuAndBox(objId);
        if (menuAndBox == null) {
            return;
        }

        var menu = menuAndBox["menu"];
        var box = menuAndBox["box"];
        var menuId = menu.attr('id');

        if (self.hoverTimer[menuId] && self.hoverTimer[menuId] != null) {
            clearTimeout(self.hoverTimer[menuId]);
            self.hoverTimer[menuId] = null;
        }

        var menuTop = self.options.isPosition ? menu.position().top : menu.offset().top;
        var menuLeft = self.options.isPosition ? menu.position().left : menu.offset().left;

        var menuHeight = menu.height();
        var menuWidth = menu.width();
        var boxHeight = box.height();
        var boxWidth = box.width();
        var documentHeight = $(document).height();
        var documentWidth = $(document).width();

        if (self.options.position > 0) {
            var fixTop = menuTop;
            var fixLeft = menuLeft;

            switch (self.options.position) {
                case 1: //菜单下方
                default:
                    fixTop = menuTop + menuHeight;
                    //if ((fixTop + boxHeight) > documentHeight) {
                    //    fixTop = menuTop - boxHeight;
                    //}
                    fixLeft = menuLeft;
                    break;
                case 2: //菜单上方
                    fixTop = menuTop - menuHeight - boxHeight;
                    fixLeft = menuLeft;
                    break;
                case 3: //菜单左方
                    fixTop = menuTop;
                    fixLeft = menuLeft - boxWidth;
                    break;
                case 4: //菜单右方
                    fixTop = menuTop;
                    //fixLeft = menuLeft + menuWidth;
                    //if ((fixLeft + boxWidth) > documentWidth) {
                    //    fixLeft = menuLeft - boxWidth;
                    //}
                    break;
                case 5: //原始位置

                    break;
            }

            if (self.options.position != 5) {
                if (self.options.forceTopPix != null) {
                    fixTop = self.options.forceTopPix;
                }
                if (self.options.forceLeftPix != null) {
                    fixLeft = self.options.forceLeftPix;
                }

                var useFixed = self.options.isFixed;
                if ($.browser.msie && parseFloat($.browser.version) <= 6) {
                    useFixed = false;
                }
                fixTop = fixTop + self.options.fixTopPix;
                if (useFixed) {
                    fixTop = fixTop - $(document).scrollTop();
                }
                fixLeft = fixLeft + self.options.fixLeftPix;
                if (useFixed) {
                    fixLeft = fixLeft - $(document).scrollLeft();
                }

                var fixedPosition = useFixed ? "fixed" : "absolute";
                box.css({ "position": fixedPosition, "top": fixTop, "left": fixLeft });
            }
        }

        box.css({ "z-index": 2015 });

        if (self.options.activeMenuCss.length > 0) {
            if (!menu.hasClass(self.options.activeMenuCss)) {
                menu.addClass(self.options.activeMenuCss);
            }
        }

        if (self.options.fadeInMilSeconds > 0) {
            box.fadeIn(self.options.fadeInMilSeconds);
        }
        else {
            box.show();
        }
    };
    /*
    隐藏容器块
    */
    popupBox.prototype.hide = function (objId) {
        var self = this;
        var menuAndBox = self._getMenuAndBox(objId);
        if (menuAndBox == null) {
            return;
        }

        if (self.hoverTimer != null) {
            clearTimeout(self.hoverTimer);
        }

        var menu = menuAndBox["menu"];
        var box = menuAndBox["box"];

        var menuId = menu.attr('id');

        self.hoverTimer[menuId] = setTimeout(function () {
            if (self.options.activeMenuCss.length > 0) {
                if (menu.hasClass(self.options.activeMenuCss)) {
                    menu.removeClass(self.options.activeMenuCss);
                }
            }
            if (self.options.fadeOutMilSeconds > 0) {
                box.fadeOut(self.options.fadeOutMilSeconds);
            }
            else {
                box.hide();
            }
            if (self.options.defaultShow == true) {
                self.show(self.options.currentMenuId, false);
            }
        }, 60);
    };

    //-------------------------------------------------------------------------------------------


    //-------------------------------------------------------------------------------------------
    /*
    幻灯片
    @id[string] 幻灯片唯一id
    @sliderContainerSelector[string] 幻灯片项目容器选择器
    @sliderItemSelector[string] 幻灯片项目选择器
    @sliderMenuSelector[string] 幻灯片按钮选择器
    @options[json] key:配置名, value:配置值。
                    mode:播放模式 0:无效果(直接显示隐藏), 1:水平, 2:垂直, 3:淡入淡出
                    direction:方向 1:左至右/上至下,-1:右至左/下至上
                    width:指定幻灯片容器的宽度(像素)
                    height:指定幻灯片容器的高度(像素)
                    left:幻灯片项相对幻灯片窗口的左位置(像素)
                    top:幻灯片项相对幻灯片窗口的上位置(像素)
                    menuActiveCss:播放指定幻灯片项时，对应的幻灯片触发按钮添加的样式名
                    menuEventType:幻灯片项的对应的按钮触发方式，默认click
                    auto:是否在网页加载时自动播放幻灯片
                    autoTimespan:自动播放幻灯片的时间间隔(毫秒)
                    animateTimespan:幻灯片切换动作效果时间(毫秒)
                    callback:每项的回调，回调参数(playIndex:播放的项, orgIndex:播放前的项)
    */
    function slider(id, sliderContainerSelector, sliderItemSelector, sliderMenuSelector, options) {
        var self = this;
        self.id = id;
        self.containerSelector = sliderContainerSelector;
        self.itemSelector = sliderItemSelector;
        self.menuSelector = sliderMenuSelector;
        self.options = mergeJson(options, self.options);
        if (self.options.direction > 1) {
            self.options.direction = 1;
        }
        if (self.options.direction <= 0) {
            self.options.direction = -1;
        }

        self.autoPlayTimer[self.id] = null;

        var total = 0;
        $(sliderItemSelector).each(function () {
            var item = $(this);
            if (self.options.mode != 0 && self.options.mode != 3) {
                item.css({ "position": "absolute", "width": self.options.width + "px", "height": self.options.height + "px" });
            }
            total++;
        });
        self._total = total;
        if (total <= 1) {
            $(sliderMenuSelector).hide();
            return;
        }

        $(sliderMenuSelector).each(function () {
            var menu = $(this);
            menu.bind(self.options.menuEventType, function () {
                self.play(parseInt($(this).attr('data-wrap')));
            });
        });

        var container = $(sliderContainerSelector);
        if (self.options.mode != 0 && self.options.mode != 3) {
            container.css({ "position": "relative", "overflow": "hidden" });
        }

        if (self.options.auto) {
            self.autoPlay();
        }
    }
    slider.prototype.containerSelector = '';
    slider.prototype.itemSelector = '';
    slider.prototype.menuSelector = '';
    slider.prototype.options = {
        mode: 0,
        direction: 1,
        width: 0,
        height: 0,
        left: 0,
        top: 0,
        menuActiveCss: '',
        menuEventType: 'click',
        auto: false,
        autoTimespan: 4000,
        animateTimespan: 500
    };
    slider.prototype.id = '';
    slider.prototype.autoPlayTimer = {};
    slider.prototype.currentIndex = 1;
    slider.prototype._total = 0;

    /*
    播放指定的幻灯片
    @itemIndex[int] 指定第几个幻灯片项,基数为1
    */
    slider.prototype.play = function (playIndex) {
        var self = this;

        if (self.currentIndex == playIndex) {
            return false;
        }

        if (self.options.auto && self.autoPlayTimer[self.id] != null) {
            clearInterval(self.autoPlayTimer[self.id]);
            self.autoPlayTimer[self.id] = null;
        }

        var total = self._total;
        var currentIndex = self.currentIndex;
        if (playIndex > total) {
            playIndex = 1;
        }
        if (playIndex < 1) {
            playIndex = total;
        }
        if (playIndex == currentIndex) {
            return false;
        }

        var mode = self.options.mode;
        $(self.itemSelector).each(function () {
            var item = $(this);

            var itemIndex = parseInt(item.attr('data-wrap'));
            if (itemIndex == self.currentIndex) {
                switch (mode) {
                    case 0:
                    default:
                        item.hide();
                        break;
                    case 3:
                        item.fadeOut(self.options.animateTimespan);
                        break;
                    case 1:
                        if (itemIndex > playIndex) {
                            item.animate({ "left": -(self.options.direction * (self.options.width + self.options.left)) + "px" }, function () {
                                item.hide();
                            });
                        }
                        else {
                            item.animate({ "left": (self.options.direction * (self.options.width + self.options.left)) + "px" }, function () {
                                item.hide();
                            });
                        }
                        break;
                    case 2:
                        if (itemIndex > playIndex) {
                            item.animate({ "top": -(self.options.direction * (self.options.height + self.options.top)) + "px" }, function () {
                                item.hide();
                            });
                        }
                        else {
                            item.animate({ "top": (self.options.direction * (self.options.height + self.options.top)) + "px" }, function () {
                                item.hide();
                            });
                        }
                        break;
                }
            }

            if (itemIndex == playIndex) {
                switch (mode) {
                    case 0:
                    default:
                        item.show();
                        break;
                    case 3:
                        item.fadeIn(self.options.animateTimespan);
                        break;
                    case 1:
                        if (self.currentIndex > playIndex) {
                            item.css({ "left": (self.options.direction * (self.options.width + self.options.left)) + "px", "top": self.options.top + "px" });
                        }
                        else {
                            item.css({ "left": -(self.options.direction * (self.options.width + self.options.left)) + "px", "top": self.options.top + "px" });
                        }

                        item.show();
                        item.animate({ "left": self.options.left + "px" });
                        break;
                    case 2:
                        if (self.currentIndex > playIndex) {
                            item.css({ "top": (self.options.direction * (self.options.height + self.options.top)) + "px", "left": self.options.left + "px" });
                        }
                        else {
                            item.css({ "top": -(self.options.direction * (self.options.height + self.options.top)) + "px", "left": self.options.left + "px" });
                        }

                        item.show();
                        item.animate({ "top": self.options.top + "px" });
                        break;
                }
            }
        });

        if (self.options.menuActiveCss.length > 0) {
            $(self.menuSelector).each(function () {
                var menu = $(this);
                var itemIndex = parseInt(menu.attr('data-wrap'));

                if (itemIndex == playIndex) {
                    menu.addClass(self.options.menuActiveCss);
                }
                else {
                    menu.removeClass(self.options.menuActiveCss);
                }
            });
        }

        if (window.lib51.isSet(self.options.callback)) {
            self.options.callback(playIndex, currentIndex);
        }

        self.currentIndex = playIndex;

        if (self.options.auto) {
            self.autoPlay();
        }
    };
    slider.prototype.playNext = function () {
        this.play(this.currentIndex + 1);
    };
    slider.prototype.playPrev = function () {
        this.play(this.currentIndex - 1);
    };
    /*
    自动播放
    */
    slider.prototype.autoPlay = function () {
        var self = this;
        if (self.options.auto && self.autoPlayTimer[self.id] == null) {
            self.autoPlayTimer[self.id] = setInterval(function () {
                self.play(self.currentIndex + 1);
            }, self.options.autoTimespan);
        }
    };
    //-------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------
    /*
    输入框自动完成
    @inputId[String] 输入框id
    @dataSource[mixed] 提供数据的url 或 json对象
    @panelId[String] 自动完成展示容器id
    @templateId[String] 自动完成展示模板id
    @options[Json] 
    */
    function autoComplete(inputId, dataSource, panelId, templateId, options) {
        var self = this;
        self.inputId = inputId;
        self.panelId = panelId;
        self.templateId = templateId;
        self.dataSource = dataSource;
        self.options = mergeJson(options, self.options);

        var input = $('#' + inputId);
        input.attr('autocomplete', 'off');

        input.bind('keyup', self.input);
        input.bind('blur', self.blur);
        input.bind('focus', function () {
            self.focus(inputId);
        });

        var panel = $('#' + panelId);
        panel.bind('mouseover', function () {
            self.hoverPanel(inputId);
        });
    }
    autoComplete.prototype.inputId = null;
    autoComplete.prototype.panelId = null;
    autoComplete.prototype.templateId = null;
    autoComplete.prototype.dataSource = null;
    autoComplete.prototype.options = {
        width: 0,
        minWords: 1,
        fieldName: 'kw',
        isPosition: false
    };
    autoComplete.prototype.triggerTimer = {};
    autoComplete.prototype.closeTimer = {};
    autoComplete.prototype.lastInputKeywords = null;

    autoComplete.prototype.input = function (oEvent) {
        var oEvent = getEvent();
        var oElem = getEventObj(oEvent);
        var input = $(oElem);

        var keycode = oEvent.keyCode || oEvent.which;

        var inputId = input.attr('id');
        var ac = window.lib51.autoCompletes[inputId];
        var panelId = ac["panelId"];
        var lastInputKeywords = ac["lastInputKeywords"];
        var fieldName = ac["options"]["fieldName"];
        var isPosition = ac["options"]["isPosition"];

        var kw = $.trim(input.val());
        if (kw.length < ac["options"]["minWords"]) {
            $('#' + panelId).hide();
            return;
        }
        if (kw == lastInputKeywords) {
            return;
        }

        //判断键盘码，只有输入数字与英文字母键盘位需要处理
        if (keycode != null) {
            keycode = parseInt(keycode);
            if (keycode <= 0) {
                return;
            }

            var isAuto = false;
            if ((keycode >= 65 && keycode <= 90) || (keycode >= 48 && keycode <= 57) || (keycode >= 96 && keycode <= 105)) {
                isAuto = true;
            }
            else {
                if (keycode == 32 || keycode == 13) {
                    if (kw.length > 0 && kw != lastInputKeywords) {
                        isAuto = true;
                    }
                }
                if (keycode == 8) {
                    if (kw.length > 0 && kw != lastInputKeywords) {
                        isAuto = true;
                    }
                }
            }

            if (false == isAuto) {
                return;
            }
        }

        //保存最后的输入
        ac["lastInputKeywords"] = kw;

        var dataSource = ac["dataSource"];
        if (typeof (dataSource) == 'string') { //从Url地址加载

            if (ac["triggerTimer"][inputId] != null) {
                clearTimeout(ac["triggerTimer"][inputId]);
            }

            ac["triggerTimer"][inputId] = setTimeout(function () {
                var postData = {};
                postData[fieldName] = $.trim(input.val());

                $.ajax({
                    url: dataSource,
                    data: postData,
                    type: "GET",
                    dataType: "json",
                    cache: false,
                    success: function (jsonData) {
                        var ac = window.lib51.autoCompletes[inputId];
                        var panelId = ac["panelId"];
                        var templateId = ac["templateId"];
                        var isPosition = ac["options"]["isPosition"] || false;
                        var panel = $('#' + panelId);

                        if (jsonData) {
                            var input = $('#' + inputId);

                            var txtTop = isPosition ? input.position().top : input.offset().top;
                            var txtLeft = isPosition ? input.position().left : input.offset().left;
                            var txtHeight = input.height();
                            var txtWidth = input.width();
                            var boxHeight = panel.height();
                            var boxWidth = panel.width();

                            var fixTop = txtTop + txtHeight;
                            var fixLeft = txtLeft;
                            var fixWidth = ac["options"]["width"];
                            if (fixWidth == 0) {
                                fixWidth = txtWidth;
                            }

                            panel.show();
                            panel.css({ "position": "absolute", "top": fixTop, "left": fixLeft, "width": fixWidth + 'px' });

                            var tmpl = $('#' + templateId).html();
                            var html = juicer(tmpl, { "ListData": jsonData, "Keywords": kw });
                            panel.html(html);
                        }
                        else {
                            if (panel.is(":visible")) {
                                panel.hide();
                            }
                        }
                    },
                    error: function (xmlHttpRequest, errorMessage, ex) {
                    }
                });

            }, 300);
        }
        else { //指定Json数据源

        }

    };
    autoComplete.prototype.blur = function (oEvent) {
        var oEvent = getEvent();
        var oElem = getEventObj(oEvent);
        var input = $(oElem);

        var inputId = input.attr("id");
        var ac = window.lib51.autoCompletes[inputId];
        var panelId = ac["panelId"];

        ac["closeTimer"][inputId] = setTimeout(function () {
            var panel = $('#' + panelId);
            panel.hide();
        }, 300);
    };
    autoComplete.prototype.focus = function (inputId) {
        var ac = window.lib51.autoCompletes[inputId];
        if (ac["lastInputKeywords"] != null) {
            $('#' + ac["panelId"]).show();
        }
    };
    autoComplete.prototype.hoverPanel = function (inputId) {
        var ac = window.lib51.autoCompletes[inputId];
        if (ac["closeTimer"][inputId] != null) {
            clearTimeout(ac["closeTimer"][inputId]);
        }
    };
    //-------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------
    function lib51() {
    }

    lib51.prototype = {
        version: 0.1,
        build: 20150612,

        /*
        合并json到目标json中
        @newJson[Json] 新json
        @targetJson[Json] 源json
        */
        mergeJson: function(newJson, targetJson){
            return mergeJson(newJson, targetJson);
        },
        /*
        模拟c# String.Format
        @str[String] 要格式化的字符串
        @args[Array] 格式化参数数组
        */
        stringFormat: function(str, args){
            return str.format(args);
        },

        /*
        文件大小友好格式字符串
        @size[Number] 大小，字节
        @format[String] 可选，输出格式，{0}代表大小数字，{1}代表单位
        */
        formatSize: function (size, format) {
            if (isNaN(size)) {
                return '';
            }
            format = format || '{0} {1}';
            var units = 'BKMG';

            var n;
            var unit;

            if (size >= 1024) {
                var handleSize = size;
                var indx = 0;
                do
                {
                    handleSize = handleSize / 1024.00;
                    indx++;
                } while (handleSize >= 1024 && indx < unit.Length - 1);

                n = handleSize.toFixed(1);
                unit = units[indx];
            }
            else {
                if (size <= 0) {
                    n = 0;
                    unit = 'K';
                }
                else {
                    n = size;
                    if (size > 1) {
                        unit = 'Bytes';
                    }
                    else {
                        unit = 'Byte';
                    }
                }
            }

            var o = format.format([n, unit]);
            return o;
        },

        /*
        将指定时间对象转为时间戳(秒)
        @dt[Date] 时间
        */
        getTimestamp: function(dt){
            dt = dt || new Date();
            var timestamp = Date.parse(dt);
            return timestamp / 1000;
        },
        /*
        将指定时间戳(毫秒)转为时间对象
        @timestamp[Number] 时间戳
        */
        getDateTime: function (timestamp) {
            if (typeof (timestamp) == 'string') {
                var reg = /\/Date(\d+)\//ig;
                var rs = '';
                if (rs = reg.exec(timestamp)) {
                    timestamp = parseInt(rs[1]);
                }
            }
            if (false == isNaN(timestamp)) {
                if (timestamp <= 9999999999) {
                    timestamp = timestamp * 1000;
                }
            }
            var newDate = new Date();
            newDate.setTime(timestamp);
            return newDate;
        },
        /*
        将时间转换为时间字符串
        @dt[Date] 时间或时间戳
        */
        getDateTimeString: function (dt, formatStr) {
            if (typeof (dt) == 'undefined') {
                return '';
            }
            if (!isNaN(dt)) {
                dt = window.lib51.getDateTime(dt);
            }
            else if (typeof (dt) == 'string') {
                return dt;
            }
            formatStr = formatStr || "yyyy-MM-dd HH:mm:ss";
            var str = dt.Format(formatStr);
            return str;
        },

        /*
        获取网址参数
        @name[string] 参数名
        */
        getQueryString: function (name) {
            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
            var r = window.location.search.substr(1).match(reg);
            if (r != null && typeof (r) != 'undefined') {
                return unescape(r[2]);
            }
            return null;
        },

        /*
        获取事件源
        */
        getEvent: function () {
            return getEvent();
        },
        /*
        获取事件源对象
        */
        getEventObj: function (oEvent) {
            return getEventObj(oEvent);
        },
        /*
        判断变量是否不为undefined与null
        */
        isSet: function (v) {
            return isSet(v);
        },
        /*
        判断变量是否为function
        */
        isFunc: function (v) {
            return isFunc(v);
        },

        /*
        url地址添加新queryString项　
        @urlInput[string] url
        @newParamKey[string]
        @newParamValue[string]
        */
        resolveUrl: function (urlInput, newParamKey, newParamValue) {
            if (newParamValue.length < 1) {
                if (urlInput.indexOf(newParamKey + "=") < 0) {
                    return urlInput;
                }
            }
            //判断指定的网址本身是否带参数
            if (urlInput.indexOf("?") < 0) {
                return urlInput + "?" + newParamKey + "=" + newParamValue;
            }
            else {
                //判断指定的网址是否包含指定的参数
                if (urlInput.indexOf(newParamKey + "=") < 0) {
                    return urlInput + "&" + newParamKey + "=" + newParamValue;
                }
                else {
                    var paramString = '';

                    //不带参数的纯网址
                    var pathString = urlInput.substr(0, urlInput.indexOf("?") + 1);
                    //第一部分参数字串
                    var paramString1 = urlInput.substr(urlInput.indexOf("?") + 1, urlInput.indexOf(newParamKey + "=") - urlInput.indexOf("?") - 1);
                    //第二部分参数字串
                    var paramString2 = urlInput.substr(urlInput.indexOf(newParamKey + "="));

                    //如果含有"&",第二部分参数字串就是不止包含指定的参数
                    if (paramString2.indexOf("&") > 0) {
                        paramString = pathString + paramString1 + newParamKey + "=" + newParamValue + paramString2.substr(paramString2.indexOf("&"));
                    }
                    else {
                        paramString = pathString + paramString1 + newParamKey + "=" + newParamValue;
                    }

                    return paramString;
                }
            }
        },
        /*
        补全不够长度的数字字符串，如将110填充为0000110
        */
        fillText: function(text, len, fillStr){
            var textStr = text + '';
            if (textStr.length < len)
            {
                var l = len - textStr.length;
                for (var i = 0; i < l; i++)
                {
                    textStr = fillStr + textStr;
                }
            }
            return textStr;
        },

        /*
        获取表单数据，返回json-key:表单项name, value:表单项值
        @formId[string] 表单id
        */
        getFormData: function (formId) {
            var formObj = document.getElementById(formId);
            var formElements = formObj.elements;

            var postData = {};
            var savedItemNames = [];

            for (var i = 0; i < formElements.length; i++) {
                var formElement = formElements[i];

                var itemName = formElement.getAttribute('name');
                var isItemSaved = false;
                for (var j = 0; j < savedItemNames.length; j++) {
                    if (savedItemNames[j] == itemName) {
                        isItemSaved = true;
                        break;
                    }
                }
                if (isItemSaved) {
                    continue;
                }

                var itemType = formElement.type.toLowerCase();
                var itemValue = '';
                switch (itemType) {
                    case "text":
                    default:
                        itemValue = formElement.value;
                        break;
                    case "checkbox":
                        var chks = $('#' + formId + ' input[name=' + itemName + ']');

                        var chkValue = '';
                        chks.each(function () {
                            var chk = this;
                            if (chk['checked'] || chk.checked) {
                                chkValue += ',' + chk.value;
                            }
                        });
                        if (chkValue.length > 0) {
                            chkValue = chkValue.substr(1);
                        }
                        itemValue = chkValue;

                        break;
                    case "radio":
                        var rdos = $('#' + formId + ' input[name=' + itemName + ']');
                        var isRdoChk = false;
                        rdos.each(function () {
                            var rdo = this;
                            if (isRdoChk == false && rdo['checked'] || rdo.checked) {
                                itemValue = rdo.value;
                                isRdoChk = true;
                            }
                        });
                        break;
                    case "textarea":
                        itemValue = formElement.value;
                        break;
                    case "select-one":
                        itemValue = formElement.options[formElement.selectedIndex].value;
                        break;
                    case "select-multiple":
                        var seltValue = '';
                        for (var o = 0; o < formElement.options.length; o++) {
                            var opt = formElement.options[o];
                            if (opt['selected'] || opt.selected) {
                                seltValue += ',' + opt.value;
                            }
                        }
                        if (seltValue.length > 0) {
                            seltValue = seltValue.substr(1);
                        }
                        itemValue = seltValue;
                        break;
                }

                savedItemNames.push(itemName);
                postData[itemName] = itemValue;
            }

            return postData;
        },

        /*
        设置表单数据
        @formId[string] 表单id
        @formData[json] 表单各项的值, key:表单项name, value:表彰项值
        */
        setFormData: function (formId, formData) {
            var setItemNames = [];

            for (var itemName in formData) {
                var itemValue = formData[itemName];
                var formItem = $('#' + formId + ' input[name=' + itemName + '], #' + formId + ' select[name=' + itemName + '], #' + formId + ' textarea[name=' + itemName + ']');

                var formElement = formItem[0];
                if (!formElement) {
                    continue;
                }

                var itemType = formElement.type.toLowerCase();
                switch (itemType) {
                    case "text":
                    default:
                        formItem.val(itemValue);
                        break;
                    case "checkbox":
                        if (formItem.val() == itemValue) {
                            formItem.attr("checked", true);
                        }
                        break;
                    case "radio":
                        if (formItem.val() == itemValue) {
                            formItem.attr("checked", true);
                        }
                        break;
                    case "textarea":
                        formItem.val(itemValue);
                        break;
                    case "select-one":
                        formItem.val(itemValue);
                        break;
                    case "select-multiple":
                        var itemValueArr = itemValue.split(',');
                        for (var o = 0; o < formElement.options.length; o++) {
                            var opt = formElement.options[o];
                            if ($.inArray(opt.value, itemValueArr)) {
                                $(opt).attr('selected', true);
                            }
                        }
                        break;
                }
            }
        },

        /*
        jQuery ajax请求封装，带加载提示等
        @url[string] 请求地址
        @data[json] 数据
        @options[json] 配置
                        type: 请求类型 GET / POST
                        success: 成功的回调函数
                        error: 出错的回调函数
                        begin: 开始请求时的执行函数
                        complete: 结束请求时的执行函数
        */
        ajaxQuery: function (url, data, options) {
            var data = data || {};
            var type = options["type"] || 'GET';
            var successFunc = options["success"] || null;
            var errorFunc = options["error"] || null;
            var beginFunc = options["begin"] || null;
            var completeFunc = options["complete"] || null;

            url = this.resolveUrl(url, "rand", this.getTimestamp());

            if (isFunc(beginFunc)) {
                beginFunc(url, data, options);
            }

            $.ajax({
                url: url,
                data: data,
                type: type,
                cache: false,
                success: function (jsonData) {
                    if (isFunc(successFunc)) {
                        successFunc(jsonData);
                    }

                    if (isFunc(completeFunc)) {
                        completeFunc(jsonData);
                    }

                },
                error: function (xmlHttpRequest, errorMessage, ex) {
                    if (isFunc(errorFunc)) {
                        errorFunc(xmlHttpRequest, errorMessage, ex);
                    }

                    if (isFunc(completeFunc)) {
                        completeFunc(xmlHttpRequest, errorMessage, ex);
                    }

                }
            });
        },
        /*
        POST提交表单数据
        @formId[string] 表单id
        @buttonId[string] 按钮id
        @success[function] 成功回调
        @begin[function] 开始请求时执行的函数
        @complete[function] 结束请求时执行的函数
        @appendData[json] 额外附加的数据
        */
        postForm: function (formId, buttonId, success, begin, complete, appendData) {
            var form = $('#' + formId);
            if (form.length == 0) {
                return;
            }

            var url = form.attr('action');
            var data = window.lib51.getFormData(formId);
            if (isSet(appendData)) {
                data = mergeJson(appendData, data);
            }
            var options = {};
            options["type"] = 'POST';
            options["success"] = success;
            options["begin"] = function (url, data, options) {
                if (isSet(buttonId)) {
                    $('#' + buttonId).attr('disabled', true);
                }
                begin(url, data, options);
            };
            options["complete"] = function (jsonData) {
                if (isSet(buttonId)) {
                    $('#' + buttonId).attr('disabled', false);
                }
                complete(jsonData);
            };
            options["error"] = function (xmlHttpRequest, errorMessage, ex) {
                alert(errorMessage);
            };

            window.lib51.ajaxQuery(url, data, options);
        },

        /*
        某些浏览器下修正不支持placeholder的属性
        */
        fixPlaceHolder: function(){
            if ($.browser.msie) {
                var ieVersion = parseFloat($.browser.version);
                if (ieVersion < 10.0) {
                    $('input[type=text], textarea').each(function () {
                        var input = $(this);
                        var ph = input.attr('placeholder');
                        if (ph) {
                            input.val(ph);
                            input.bind('focus', function () {
                                var thisInput = $(this);
                                var thisPH = thisInput.attr('placeholder');
                                var inputValue = thisInput.val();
                                if (inputValue == ph) {
                                    input.val('');
                                }
                            });
                            input.bind('blur', function () {
                                var thisInput = $(this);
                                var inputValue = thisInput.val();
                                var thisPH = thisInput.attr('placeholder');

                                if (inputValue.length == 0) {
                                    input.val(thisPH);
                                }
                            });
                        }
                    });
                }
            }
        },

        /*
        窗口滚动条滚动到指定目标位置
        @targetId[string] 目标id
        @ms[string] 毫秒
        @callback[function] 回调函数
        */
        scrollToPosition: function (targetId, ms, callback) {
            ms = ms || 500;
            var targetTop = $("#" + targetId).offset().top;
            $("html,body").animate({
                scrollTop: targetTop
            }, ms, callback);
        },

        /*
        设置cookie值
        @key[String] cookie键名
        @value[String] cookie值
        @options[Json] 参数
                        expires: 定义cookie的有效时间，值可以是一个数字（从创建cookie时算起，以天为单位）或一个Date 对象。如果省略，那么创建的cookie是会话cookie，将在用户退出浏览器时被删除。例:365
                        path: 定义cookie的有效路径。默认情况下， 该参数的值为创建 cookie 的网页所在路径（标准浏览器的行为）。如果你想在整个网站中访问这个cookie需要这样设置有效路径：path: '/'。
                        domain: 创建 cookie的网页所拥有的域名。
                        secure: 默认值：false。如果为true，cookie的传输需要使用安全协议（HTTPS）。
                        raw: 默认值：false。如果为true，cookie的传输需要使用安全协议（HTTPS）。
        */
        setCookie: function(key, value, options){
            $.cookie(key, value, options);
        },
        /*
        获取cookie值
        */
        getCookie: function (key) {
            return $.cookie(key);
        },
        /*
        移除cookie
        */
        removeCookie: function (key, options) {
            $.cookie(key, null, options);
        },

        /*
        给指定文本里的指定关键字加上标签与样式
        @text[String] 指定文本
        @keywords[String] 指定关键字
        @tag[String] 打上的标签
        @css[String] 标签使用的样式
        */
        tagKeywords: function (text, keywords, tag, css) {
            css = css || "";
            if (keywords.length == 0) {
                return text;
            }

            var keywordParts = keywords.split(' ');
            for (var i = 0; i < keywordParts.length; i++) {
                var keywordPart = keywordParts[i];
                if (keywordPart.replace(' ', '').length == 0) {
                    continue;
                }

                text = text.replace(keywordPart, "<" + tag + " class=\"" + css + "\">" + keywordPart + "</" + tag + ">");
            }
            return text;
        },

        /*
        已经设置了静止定位的元素
        */
        fixedObjects: {},
        /*
        使指定的元素静止定位，兼容所有浏览器
        @objId[string] 元素id
        @options[json] 设置
                        top[int] 距离顶部像素
                        left[int] 距离左边像素
                        bottom[int] 距离底部像素
                        right[int] 距离右边像素
        */
        _fixObjectPosition: function (objId, options) {
            var top = options["top"];
            var left = options["left"];
            var bottom = options["bottom"];
            var right = options["right"];

            try {
                var obj = $('#' + objId);
                if (obj.is(":visible")) {
                    if ($.browser.msie && ($.browser.version == "6.0") && !$.support.style) {
                        var scrollTop = $(document).scrollTop();
                        var scrollLeft = $(document).scrollLeft();

                        var winW = $(window).width();
                        var winH = $(window).height();

                        var objW = obj.width();
                        var objH = obj.height();
                        obj.css({
                            "position": "absolute"
                        });

                        if (typeof (top) != 'undefined') {
                            obj.css({
                                "top": (top + scrollTop) + 'px'
                            });
                        }
                        if (typeof (left) != 'undefined') {
                            obj.css({
                                "left": (left + scrollLeft) + 'px'
                            });
                        }
                        if (typeof (bottom) != 'undefined') {
                            top = winH - bottom - objH;
                            obj.css({
                                "top": (top + scrollTop) + 'px'
                            });
                        }
                        if (typeof (right) != 'undefined') {
                            left = winW - right - objW;
                            obj.css({
                                "left": (left + scrollLeft) + 'px'
                            });
                        }
                    }
                    else { //非IE6支持fixed定位，可直接使用CSS控制
                        obj.css({
                            "position": "fixed"
                        });
                        if (typeof (top) != 'undefined') {
                            obj.css({
                                "top": (top) + 'px'
                            });
                        }
                        if (typeof (left) != 'undefined') {
                            obj.css({
                                "left": (left) + 'px'
                            });
                        }
                        if (typeof (bottom) != 'undefined') {
                            obj.css({
                                "bottom": (bottom) + 'px'
                            });
                        }
                        if (typeof (right) != 'undefined') {
                            obj.css({
                                "right": (right) + 'px'
                            });
                        }
                    }
                }
            }
            catch (ex) {

            }
        },
        /*
        使指定的元素静止定位，兼容所有浏览器
        @objId[string] 元素id
        @options[json] 设置
                        top[int] 距离顶部像素
                        left[int] 距离左边像素
                        bottom[int] 距离底部像素
                        right[int] 距离右边像素
                        initPosition[bool或number] 是否初始化位置，没有滚动到初始位置时不静止定位。为数字时表示滚动高于或低于初始位置时才静止定位(正值表示高于，负值表示低于)。
                        showPosition[int, int] 滚动条超过指定位置时才显示，否则隐藏
        */
        fixObjectPosition: function (objId, options) {
            var top = options["top"];
            var left = options["left"];
            var bottom = options["bottom"];
            var right = options["right"];
            var initPosition = options["initPosition"] || false;
            var initPositionTop = parseFloat(initPosition);
            var showPosition = options["showPosition"] || null;
            
            if (typeof (this.fixedObjects[objId]) == 'undefined') {
                var obj = $('#' + objId);
                var offset = obj.offset();
                var orgTop = offset.top;
                var orgLeft = offset.left;
                var position = obj[0].style.position;

                var winHeight = $(window).height();
                var scrollTop = $(document).scrollTop();
                var scrollLeft = $(document).scrollLeft();

                if (showPosition != null) {
                    if (scrollTop >= showPosition["top"] && scrollLeft >= showPosition["left"]) {
                        obj.show();
                    }
                    else {
                        obj.hide();
                    }
                }

                if (false == initPosition || isNaN(initPositionTop)
                    || (false == isNaN(initPositionTop) && ((initPositionTop >= 0 && scrollTop >= orgTop) || (initPositionTop < 0 && (scrollTop + winHeight) <= orgTop)))
                    ) {
                    this._fixObjectPosition(objId, options);
                }

                $(window).bind("scroll resize", function () {
                    var winHeight = $(window).height();
                    var scrollTop = $(document).scrollTop();
                    var scrollLeft = $(document).scrollLeft();

                    if (showPosition != null) {
                        if (scrollTop >= showPosition["top"] && scrollLeft >= showPosition["left"]) {
                            obj.show();
                        }
                        else {
                            obj.hide();
                        }
                    }

                    if (false == initPosition || isNaN(initPositionTop)
                    || (false == isNaN(initPositionTop) && ((initPositionTop >= 0 && scrollTop >= orgTop) || (initPositionTop < 0 && (scrollTop + winHeight) <= orgTop)))
                    ) {
                        window.lib51._fixObjectPosition(objId, options);
                    }
                    else {
                        obj.css({
                            "position": position
                        });
                    }
                });

                this.fixedObjects[objId] = { "offset": offset, "position": position, "options": options };
            }
        },
        /*
        使指定的元素静止定位于中间位置，兼容所有浏览器
        @objId[string] 元素id
        */
        _fixObjectMiddlePosition: function(objId) {
            try {
                var obj = $('#' + objId);
                if (obj.is(":visible")) {

                    var winH = $(window).height();
                    var winW = $(window).width();
                    var objW = obj.width();
                    var objH = obj.height();

                    if ($.browser.msie && ($.browser.version == "6.0") && !$.support.style) {
                        var scrollTop = $(document).scrollTop();
                        var scrollLeft = $(document).scrollLeft();
                        
                        obj.css({
                            "position": "absolute",
                            "top": (winH / 2 - objH / 2 + scrollTop) + 'px',
                            "left": (winW / 2 - objW / 2 + scrollLeft) + 'px'
                        });
                    }
                    else {
                        obj.css({
                            "position": "fixed",
                            "top": (winH / 2 - objH / 2) + 'px',
                            "left": (winW / 2 - objW / 2) + 'px'
                        });
                    }
                }
            }
            catch (ex) {

            }
        },
        /*
        使指定的元素静止定位于中间位置，兼容所有浏览器
        @objId[string] 元素id
        */
        fixObjectMiddlePosition: function (objId) {
            if (typeof (this.fixedObjects[objId]) == 'undefined') {
                this._fixObjectMiddlePosition(objId);
                $(window).bind("scroll resize", function () {
                    window.lib51._fixObjectMiddlePosition(objId);
                });

                this.fixedObjects[objId] = $('#' + objId);
            }
        },

        panelSwitches: {},
        /*
        点击菜单切换显示隐藏相应的容器
        */
        panelSwitch: function (panels, options, key) {
            var menuPanel = new panelSwitch(panels, options);
            if (typeof (key) != 'undefined') {
                this.panelSwitches[key] = popBox;
            }
            return menuPanel;
        },

        popupBoxes: {},
        /*
        移上移出菜单显示隐藏相应的容器
        */
        popupBox: function (menuList, options, key) {
            var popBox = new popupBox(menuList, options);
            if (typeof (key) != 'undefined') {
                this.popupBoxes[key] = popBox;
            }
            return popBox;
        },

        sliders: {},
        /*
        幻灯片播放
        */
        slider: function (id, containerSelector, itemSelector, menuSelector, options) {
            var slide = new slider(id, containerSelector, itemSelector, menuSelector, options);
            this.sliders[id] = slide;
            return slide;
        },

        /*
        滚动条垂直滚动到不同层块位置切换不同的菜单显示，同时静止定位菜单栏
        @barId[string] 菜单栏id
        @panelSelector[string] 层块选择器
        @options[json] 配置
        */
        scrollStatusBar: function(barId, panelSelector, options){
            window.lib51.fixObjectPosition(barId, { "top": 0, "initPosition": true });
            //TODO...
        },

        autoCompletes:{},
        /*
        自动完成
        */
        autoComplete: function (inputId, dataSource, panelId, templateId, options) {
            var auto = new autoComplete(inputId, dataSource, panelId, templateId, options);
            this.autoCompletes[inputId] = auto;
            return auto;
        }
    };

    window.lib51 = new lib51();
    //-------------------------------------------------------------------------------------------

})(window);






