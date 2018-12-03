/************************************************************************************************************************
文件说明：      51JS框架/类库
                注意：基于jQuery，必须先引入jQuery脚本才能使用
                集成了jQuery.cookie,juicer ，引用了本脚本，就不需要再引用jQuery.cookie.js,juicer.js                
*************************************************************************************************************************/



//-------------------------------------------------------------------------------------------
/*
jQuery.cookie
*/
(function(factory) {
    if (typeof define === 'function' && define.amd) {
        // AMD. Register as anonymous module.
        define(['jquery'], factory);
    } else {
        // Browser globals.
        factory(jQuery);
    }
}(function($) {

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
        } catch (er) {}
    }

    var config = $.cookie = function(key, value, options) {

        // write
        if (value !== undefined) {
            options = $.extend({}, config.defaults, options);

            if (typeof options.expires === 'number') {
                var days = options.expires,
                    t = options.expires = new Date();
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

    $.removeCookie = function(key, options) {
        if ($.cookie(key) !== undefined) {
            // Must not alter options, thus extending a fresh object...
            $.cookie(key, '', $.extend({}, options, {
                expires: -1
            }));
            return true;
        }
        return false;
    };

}));
//-------------------------------------------------------------------------------------------

//-------------------------------------------------------------------------------------------
/*
    ********** Juicer **********
    ${A Fast template engine}
    Project Home: http://juicer.name

    Author: Guokai
    Gtalk: badkaikai@gmail.com
    Blog: http://benben.cc
    Licence: MIT License
    Version: 0.6.14
*/

(function() {

    // This is the main function for not only compiling but also rendering.
    // there's at least two parameters need to be provided, one is the tpl, 
    // another is the data, the tpl can either be a string, or an id like #id.
    // if only tpl was given, it'll return the compiled reusable function.
    // if tpl and data were given at the same time, it'll return the rendered 
    // result immediately.

    var juicer = function() {
        var args = [].slice.call(arguments);

        args.push(juicer.options);

        if (args[0].match(/^\s*#([\w:\-\.]+)\s*$/igm)) {
            args[0].replace(/^\s*#([\w:\-\.]+)\s*$/igm, function($, $id) {
                var _document = document;
                var elem = _document && _document.getElementById($id);
                args[0] = elem ? (elem.value || elem.innerHTML) : $;
            });
        }

        if (juicer.documentHTML) {
            juicer.compile.call(juicer, juicer.documentHTML);
            juicer.documentHTML = '';
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
        escapereplace: function(k) {
            return __escapehtml.escapehash[k];
        },
        escaping: function(str) {
            return typeof(str) !== 'string' ? str : str.replace(/[&<>"']/igm, this.escapereplace);
        },
        detection: function(data) {
            return typeof(data) === 'undefined' ? '' : data;
        }
    };

    var __throw = function(error) {
        if (typeof(console) !== 'undefined') {
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

    var __creator = function(o, proto) {
        o = o !== Object(o) ? {} : o;

        if (o.__proto__) {
            o.__proto__ = proto;
            return o;
        }

        var empty = function() {};
        var n = Object.create ?
            Object.create(proto) :
            new(empty.prototype = proto, empty);

        for (var i in o) {
            if (o.hasOwnProperty(i)) {
                n[i] = o[i];
            }
        }

        return n;
    };

    var annotate = function(fn) {
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

        fnText = fnText.trim();
        argDecl = fnText.match(FN_ARGS);
        fnBody = fnText.match(FN_BODY)[1].trim();

        for (var i = 0; i < argDecl[1].split(FN_ARG_SPLIT).length; i++) {
            var arg = argDecl[1].split(FN_ARG_SPLIT)[i];
            arg.replace(FN_ARG, function(all, underscore, name) {
                args.push(name);
            });
        }

        return [args, fnBody];
    };

    juicer.__cache = {};
    juicer.version = '0.6.14';
    juicer.settings = {};
    juicer.documentHTML = '';

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

    juicer.tagInit = function() {
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

    juicer.set = function(conf, value) {
        var that = this;

        var escapePattern = function(v) {
            return v.replace(/[\$\(\)\[\]\+\^\{\}\?\*\|\.]/igm, function($) {
                return '\\' + $;
            });
        };

        var set = function(conf, value) {
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

    juicer.register = function(fname, fn) {
        var _method = this.options._method;

        if (_method.hasOwnProperty(fname)) {
            return false;
        }

        return _method[fname] = fn;
    };

    // remove the registered function in the memory by the provided function name.
    // for example: juicer.unregister('fnName').

    juicer.unregister = function(fname) {
        var _method = this.options._method;

        if (_method.hasOwnProperty(fname)) {
            return delete _method[fname];
        }
    };

    juicer.template = function(options) {
        var that = this;

        this.options = options;

        this.__interpolate = function(_name, _escape, options) {
            var _define = _name.split('|'),
                _fn = _define[0] || '',
                _cluster;

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

        this.__removeShell = function(tpl, options) {
            var _counter = 0;

            tpl = tpl
                // inline helper register
                .replace(juicer.settings.helperRegister, function($, helperName, fnText) {
                    var anno = annotate(fnText);
                    var fnArgs = anno[0];
                    var fnBody = anno[1];
                    var fn = new Function(fnArgs.join(','), fnBody);

                    juicer.register(helperName, fn);
                    return $;
                })

            // for expression
            .replace(juicer.settings.forstart, function($, _name, alias, key) {
                    var alias = alias || 'value',
                        key = key && key.substr(1);
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
            .replace(juicer.settings.ifstart, function($, condition) {
                    return '<% if(' + condition + ') { %>';
                })
                .replace(juicer.settings.ifend, '<% } %>')

            // else expression
            .replace(juicer.settings.elsestart, function($) {
                return '<% } else { %>';
            })

            // else if expression
            .replace(juicer.settings.elseifstart, function($, condition) {
                return '<% } else if(' + condition + ') { %>';
            })

            // interpolate without escape
            .replace(juicer.settings.noneencode, function($, _name) {
                return that.__interpolate(_name, false, options);
            })

            // interpolate with escape
            .replace(juicer.settings.interpolate, function($, _name) {
                return that.__interpolate(_name, true, options);
            })

            // clean up comments
            .replace(juicer.settings.inlinecomment, '')

            // range expression
            .replace(juicer.settings.rangestart, function($, _name, start, end) {
                var _iterate = 'j' + _counter++;
                return '<% ~function() {' +
                    'for(var ' + _iterate + '=' + start + ';' + _iterate + '<' + end + ';' + _iterate + '++) {{' +
                    'var ' + _name + '=' + _iterate + ';' +
                    ' %>';
            })

            // include sub-template
            .replace(juicer.settings.include, function($, tpl, data) {
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

        this.__toNative = function(tpl, options) {
            return this.__convert(tpl, !options || options.strip);
        };

        this.__lexicalAnalyze = function(tpl) {
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

            var indexOf = function(array, item) {
                if (Array.prototype.indexOf && array.indexOf === Array.prototype.indexOf) {
                    return array.indexOf(item);
                }

                for (var i = 0; i < array.length; i++) {
                    if (array[i] === item) return i;
                }

                return -1;
            };

            var variableAnalyze = function($, statement) {
                statement = statement.match(/\w+/igm)[0];

                if (indexOf(buffer, statement) === -1 && indexOf(reserved, statement) === -1 && indexOf(method, statement) === -1) {

                    // avoid re-declare native function, if not do this, template 
                    // `{@if encodeURIComponent(name)}` could be throw undefined.

                    if (typeof(window) !== 'undefined' && typeof(window[statement]) === 'function' && window[statement].toString().match(/^\s*?function \w+\(\) \{\s*?\[native code\]\s*?\}\s*?$/i)) {
                        return $;
                    }

                    // compatible for node.js
                    if (typeof(global) !== 'undefined' && typeof(global[statement]) === 'function' && global[statement].toString().match(/^\s*?function \w+\(\) \{\s*?\[native code\]\s*?\}\s*?$/i)) {
                        return $;
                    }

                    // avoid re-declare registered function, if not do this, template 
                    // `{@if registered_func(name)}` could be throw undefined.

                    if (typeof(juicer.options._method[statement]) === 'function' || juicer.options._method.hasOwnProperty(statement)) {
                        method.push(statement);
                        return $;
                    }

                    // avoid SyntaxError: Unexpected number

                    if (statement.match(/^\d+/igm)) {
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
            replace(/[\+\-\*\/%!\?\|\^&~<>=,\(\)\[\]]\s*([A-Za-z_0-9]+)/igm, variableAnalyze);

            for (var i = 0; i < buffer.length; i++) {
                prefix += 'var ' + buffer[i] + '=_.' + buffer[i] + ';';
            }

            for (var i = 0; i < method.length; i++) {
                prefix += 'var ' + method[i] + '=_method.' + method[i] + ';';
            }

            return '<% ' + prefix + ' %>';
        };

        this.__convert = function(tpl, strip) {
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

        this.parse = function(tpl, options) {
            var _that = this;

            if (!options || options.loose !== false) {
                tpl = this.__lexicalAnalyze(tpl) + tpl;
            }

            tpl = this.__removeShell(tpl, options);
            tpl = this.__toNative(tpl, options);

            this._render = new Function('_, _method', tpl);

            this.render = function(_, _method) {
                if (!_method || _method !== that.options._method) {
                    _method = __creator(_method, that.options._method);
                }

                return _that._render.call(this, _, _method);
            };

            return this;
        };
    };

    juicer.compile = function(tpl, options) {
        if (!options || options !== this.options) {
            options = __creator(options, this.options);
        }

        var that = this;
        var cacheStore = {
            get: function(tpl) {
                if (options.cachestore) {
                    return options.cachestore.get(tpl);
                }

                return that.__cache[tpl];
            },

            set: function(tpl, val) {
                if (options.cachestore) {
                    return options.cachestore.set(tpl, val);
                }

                return that.__cache[tpl] = val;
            }
        };

        try {
            var engine = cacheStore.get(tpl) ?
                cacheStore.get(tpl) :
                new this.template(this.options).parse(tpl, options);

            if (!options || options.cache !== false) {
                cacheStore.set(tpl, engine);
            }

            return engine;

        } catch (e) {
            __throw('Juicer Compile Exception: ' + e.message);

            return {
                render: function() {} // noop
            };
        }
    };

    juicer.to_html = function(tpl, data, options) {
        if (!options || options !== this.options) {
            options = __creator(options, this.options);
        }

        return this.compile(tpl, options).render(data, options._method);
    };

    // avoid memory leak for node.js
    if (typeof(global) !== 'undefined' && typeof(window) === 'undefined') {
        juicer.set('cache', false);
    }

    if (typeof(document) !== 'undefined' && document.body) {
        juicer.documentHTML = document.body.innerHTML;
    }

    typeof(module) !== 'undefined' && module.exports ? module.exports = juicer : this.juicer = juicer;

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
(function($) {

    $.fn.kxbdSuperMarquee = function(options) {
        var opts = $.extend({}, $.fn.kxbdSuperMarquee.defaults, options);

        return this.each(function() {
            var $marquee = $(this); //滚动元素容器
            var _scrollObj = $marquee.get(0); //滚动元素容器DOM
            var scrollW = $marquee.width(); //滚动元素容器的宽度
            var scrollH = $marquee.height(); //滚动元素容器的高度
            var $element = $marquee.children(); //滚动元素
            var $kids = $element.children(); //滚动子元素
            var scrollSize = 0; //滚动元素尺寸
            var _type = (opts.direction == 'left' || opts.direction == 'right') ? 1 : 0; //滚动类型，1左右，0上下
            var scrollId, rollId, isMove, marqueeId;
            var t, b, c, d, e; //滚动动画的参数,t:当前时间，b:开始的位置，c:改变的位置，d:持续的时间，e:结束的位置
            var _size, _len; //子元素的尺寸与个数
            var $nav, $navBtns;
            var arrPos = [];
            var numView = 0; //当前所看子元素
            var numRoll = 0; //轮换的次数
            var numMoved = 0; //已经移动的距离

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
                $kids.each(function(i) {
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
                $navBtns.each(function(i) {
                    $(this).bind(opts.eventNav, function() {
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
                    function() {
                        clearInterval(marqueeId);
                    },
                    function() {
                        //marqueeId = setInterval(scrollFunc, opts.scrollDelay);
                        clearInterval(marqueeId);
                        marqueeId = setTimeout(scrollFunc, opts.scrollDelay);
                    }
                );

                //控制加速运动
                if (opts.controlBtn) {
                    $.each(opts.controlBtn, function(i, val) {
                        $(val).bind(opts.eventA, function() {
                            opts.direction = i;
                            opts.oldAmount = opts.scrollAmount;
                            opts.scrollAmount = opts.newAmount;
                        }).bind(opts.eventB, function() {
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
                    $.each(opts.btnGo, function(i, val) {
                        $(val).bind(opts.eventGo, function() {
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
                rollId = setInterval(function() {
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
        isMarquee: false, //是否为Marquee
        isEqual: true, //所有滚动的元素长宽是否相等,true,false
        loop: 0, //循环滚动次数，0时无限
        newAmount: 3, //加速滚动的步长
        eventA: 'mousedown', //鼠标事件，加速
        eventB: 'mouseup', //鼠标事件，原速
        isAuto: true, //是否自动轮换
        time: 5, //停顿时间，单位为秒
        duration: 50, //缓动效果，单次移动时间，越小速度越快，为0时无缓动效果
        eventGo: 'click', //鼠标事件，向前向后走
        direction: 'left', //滚动方向，'left','right','up','down'
        scrollAmount: 1, //步长
        scrollDelay: 10, //时长
        eventNav: 'click' //导航事件
    };

    $.fn.kxbdSuperMarquee.setDefaults = function(settings) {
        $.extend($.fn.kxbdSuperMarquee.defaults, settings);
    };

})(jQuery);
//-------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------
/*! layer-v2.4 弹层组件 License LGPL  http://layer.layui.com/ By 贤心 */
;
! function(a, b) {
    "use strict";
    var c, d, e = {
            getPath: function() {
                var a = document.scripts,
                    b = a[a.length - 1],
                    c = b.src;
                if (!b.getAttribute("merge")) return c.substring(0, c.lastIndexOf("/") + 1)
            }(),
            enter: function(a) {
                //13 === a.keyCode && a.preventDefault()
            },
            config: {},
            end: {},
            btn: ["&#x786E;&#x5B9A;", "&#x53D6;&#x6D88;"],
            type: ["dialog", "page", "iframe", "loading", "tips"]
        },
        f = {
            v: "2.4",
            ie6: !!a.ActiveXObject && !a.XMLHttpRequest,
            index: 0,
            path: e.getPath,
            config: function(a, b) {
                var d = 0;
                return a = a || {}, f.cache = e.config = c.extend(e.config, a), f.path = e.config.path || f.path, "string" == typeof a.extend && (a.extend = [a.extend]), f.use("skin/layer.css", a.extend && a.extend.length > 0 ? function g() {
                    var c = a.extend;
                    f.use(c[c[d] ? d : d - 1], d < c.length ? function() {
                        return ++d, g
                    }() : b)
                }() : b), this
            },
            use: function(a, b, d) {
                var e = c("head")[0],
                    a = a.replace(/\s/g, ""),
                    g = /\.css$/.test(a),
                    h = document.createElement(g ? "link" : "script"),
                    i = "layui_layer_" + a.replace(/\.|\//g, "");
                return f.path ? (g && (h.rel = "stylesheet"), h[g ? "href" : "src"] = /^http:\/\//.test(a) ? a : f.path + a, h.id = i, c("#" + i)[0] || e.appendChild(h), function j() {
                    (g ? 1989 === parseInt(c("#" + i).css("width")) : f[d || i]) ? function() {
                        b && b();
                        try {
                            g || e.removeChild(h)
                        } catch (a) {}
                    }() : setTimeout(j, 100)
                }(), this) : void 0
            },
            ready: function(a, b) {
                var d = "function" == typeof a;
                return d && (b = a), f.config(c.extend(e.config, function() {
                    return d ? {} : {
                        path: a
                    }
                }()), b), this
            },
            alert: function(a, b, d) {
                var e = "function" == typeof b;
                return e && (d = b), f.open(c.extend({
                    content: a,
                    yes: d
                }, e ? {} : b))
            },
            confirm: function(a, b, d, g) {
                var h = "function" == typeof b;
                return h && (g = d, d = b), f.open(c.extend({
                    content: a,
                    btn: e.btn,
                    yes: d,
                    btn2: g
                }, h ? {} : b))
            },
            msg: function(a, d, g) {
                var i = "function" == typeof d,
                    j = e.config.skin,
                    k = (j ? j + " " + j + "-msg" : "") || "layui-layer-msg",
                    l = h.anim.length - 1;
                return i && (g = d), f.open(c.extend({
                    content: a,
                    time: 3e3,
                    shade: !1,
                    skin: k,
                    title: !1,
                    closeBtn: !1,
                    btn: !1,
                    end: g
                }, i && !e.config.skin ? {
                    skin: k + " layui-layer-hui",
                    shift: l
                } : function() {
                    return d = d || {}, (-1 === d.icon || d.icon === b && !e.config.skin) && (d.skin = k + " " + (d.skin || "layui-layer-hui")), d
                }()))
            },
            load: function(a, b) {
                return f.open(c.extend({
                    type: 3,
                    icon: a || 0,
                    shade: .01
                }, b))
            },
            tips: function(a, b, d) {
                return f.open(c.extend({
                    type: 4,
                    content: [a, b],
                    closeBtn: !1,
                    time: 3e3,
                    shade: !1,
                    fix: !1,
                    maxWidth: 210
                }, d))
            }
        },
        g = function(a) {
            var b = this;
            b.index = ++f.index, b.config = c.extend({}, b.config, e.config, a), b.creat()
        };
    g.pt = g.prototype;
    var h = ["layui-layer", ".layui-layer-title", ".layui-layer-main", ".layui-layer-dialog", "layui-layer-iframe", "layui-layer-content", "layui-layer-btn", "layui-layer-close"];
    h.anim = ["layer-anim", "layer-anim-01", "layer-anim-02", "layer-anim-03", "layer-anim-04", "layer-anim-05", "layer-anim-06"], g.pt.config = {
        type: 0,
        shade: .3,
        fix: !0,
        move: h[1],
        title: "&#x4FE1;&#x606F;",
        offset: "auto",
        area: "auto",
        closeBtn: 1,
        time: 0,
        zIndex: 19891014,
        maxWidth: 360,
        shift: 0,
        icon: -1,
        scrollbar: !0,
        tips: 2
    }, g.pt.vessel = function(a, b) {
        var c = this,
            d = c.index,
            f = c.config,
            g = f.zIndex + d,
            i = "object" == typeof f.title,
            j = f.maxmin && (1 === f.type || 2 === f.type),
            k = f.title ? '<div class="layui-layer-title" style="' + (i ? f.title[1] : "") + '">' + (i ? f.title[0] : f.title) + "</div>" : "";
        return f.zIndex = g, b([f.shade ? '<div class="layui-layer-shade" id="layui-layer-shade' + d + '" times="' + d + '" style="' + ("z-index:" + (g - 1) + "; background-color:" + (f.shade[1] || "#000") + "; opacity:" + (f.shade[0] || f.shade) + "; filter:alpha(opacity=" + (100 * f.shade[0] || 100 * f.shade) + ");") + '"></div>' : "", '<div class="' + h[0] + (" layui-layer-" + e.type[f.type]) + (0 != f.type && 2 != f.type || f.shade ? "" : " layui-layer-border") + " " + (f.skin || "") + '" id="' + h[0] + d + '" type="' + e.type[f.type] + '" times="' + d + '" showtime="' + f.time + '" conType="' + (a ? "object" : "string") + '" style="z-index: ' + g + "; width:" + f.area[0] + ";height:" + f.area[1] + (f.fix ? "" : ";position:absolute;") + '">' + (a && 2 != f.type ? "" : k) + '<div id="' + (f.id || "") + '" class="layui-layer-content' + (0 == f.type && -1 !== f.icon ? " layui-layer-padding" : "") + (3 == f.type ? " layui-layer-loading" + f.icon : "") + '">' + (0 == f.type && -1 !== f.icon ? '<i class="layui-layer-ico layui-layer-ico' + f.icon + '"></i>' : "") + (1 == f.type && a ? "" : f.content || "") + '</div><span class="layui-layer-setwin">' + function() {
            var a = j ? '<a class="layui-layer-min" href="javascript:;"><cite></cite></a><a class="layui-layer-ico layui-layer-max" href="javascript:;"></a>' : "";
            return f.closeBtn && (a += '<a class="layui-layer-ico ' + h[7] + " " + h[7] + (f.title ? f.closeBtn : 4 == f.type ? "1" : "2") + '" href="javascript:;"></a>'), a
        }() + "</span>" + (f.btn ? function() {
            var a = "";
            "string" == typeof f.btn && (f.btn = [f.btn]);
            for (var b = 0, c = f.btn.length; c > b; b++) a += '<a class="' + h[6] + b + '">' + f.btn[b] + "</a>";
            return '<div class="' + h[6] + '">' + a + "</div>"
        }() : "") + "</div>"], k), c
    }, g.pt.creat = function() {
        var a = this,
            b = a.config,
            g = a.index,
            i = b.content,
            j = "object" == typeof i;
        if (!c("#" + b.id)[0]) {
            switch ("string" == typeof b.area && (b.area = "auto" === b.area ? ["", ""] : [b.area, ""]), b.type) {
                case 0:
                    b.btn = "btn" in b ? b.btn : e.btn[0], f.closeAll("dialog");
                    break;
                case 2:
                    var i = b.content = j ? b.content : [b.content || "http://layer.layui.com", "auto"];
                    b.content = '<iframe scrolling="' + (b.content[1] || "auto") + '" allowtransparency="true" id="' + h[4] + g + '" name="' + h[4] + g + '" onload="this.className=\'\';" class="layui-layer-load" frameborder="0" src="' + b.content[0] + '"></iframe>';
                    break;
                case 3:
                    b.title = !1, b.closeBtn = !1, -1 === b.icon && 0 === b.icon, f.closeAll("loading");
                    break;
                case 4:
                    j || (b.content = [b.content, "body"]), b.follow = b.content[1], b.content = b.content[0] + '<i class="layui-layer-TipsG"></i>', b.title = !1, b.tips = "object" == typeof b.tips ? b.tips : [b.tips, !0], b.tipsMore || f.closeAll("tips")
            }
            a.vessel(j, function(d, e) {
                c("body").append(d[0]), j ? function() {
                    2 == b.type || 4 == b.type ? function() {
                        c("body").append(d[1])
                    }() : function() {
                        i.parents("." + h[0])[0] || (i.show().addClass("layui-layer-wrap").wrap(d[1]), c("#" + h[0] + g).find("." + h[5]).before(e))
                    }()
                }() : c("body").append(d[1]), a.layero = c("#" + h[0] + g), b.scrollbar || h.html.css("overflow", "hidden").attr("layer-full", g)
            }).auto(g), 2 == b.type && f.ie6 && a.layero.find("iframe").attr("src", i[0]), c(document).off("keydown", e.enter).on("keydown", e.enter), a.layero.on("keydown", function(a) {
                c(document).off("keydown", e.enter)
            }), 4 == b.type ? a.tips() : a.offset(), b.fix && d.on("resize", function() {
                a.offset(), (/^\d+%$/.test(b.area[0]) || /^\d+%$/.test(b.area[1])) && a.auto(g), 4 == b.type && a.tips()
            }), b.time <= 0 || setTimeout(function() {
                f.close(a.index)
            }, b.time), a.move().callback(), h.anim[b.shift] && a.layero.addClass(h.anim[b.shift])
        }
    }, g.pt.auto = function(a) {
        function b(a) {
            a = g.find(a), a.height(i[1] - j - k - 2 * (0 | parseFloat(a.css("padding"))))
        }
        var e = this,
            f = e.config,
            g = c("#" + h[0] + a);
        "" === f.area[0] && f.maxWidth > 0 && (/MSIE 7/.test(navigator.userAgent) && f.btn && g.width(g.innerWidth()), g.outerWidth() > f.maxWidth && g.width(f.maxWidth));
        var i = [g.innerWidth(), g.innerHeight()],
            j = g.find(h[1]).outerHeight() || 0,
            k = g.find("." + h[6]).outerHeight() || 0;
        switch (f.type) {
            case 2:
                b("iframe");
                break;
            default:
                "" === f.area[1] ? f.fix && i[1] >= d.height() && (i[1] = d.height(), b("." + h[5])) : b("." + h[5])
        }
        return e
    }, g.pt.offset = function() {
        var a = this,
            b = a.config,
            c = a.layero,
            e = [c.outerWidth(), c.outerHeight()],
            f = "object" == typeof b.offset;
        a.offsetTop = (d.height() - e[1]) / 2, a.offsetLeft = (d.width() - e[0]) / 2, f ? (a.offsetTop = b.offset[0], a.offsetLeft = b.offset[1] || a.offsetLeft) : "auto" !== b.offset && (a.offsetTop = b.offset, "rb" === b.offset && (a.offsetTop = d.height() - e[1], a.offsetLeft = d.width() - e[0])), b.fix || (a.offsetTop = /%$/.test(a.offsetTop) ? d.height() * parseFloat(a.offsetTop) / 100 : parseFloat(a.offsetTop), a.offsetLeft = /%$/.test(a.offsetLeft) ? d.width() * parseFloat(a.offsetLeft) / 100 : parseFloat(a.offsetLeft), a.offsetTop += d.scrollTop(), a.offsetLeft += d.scrollLeft()), c.css({
            top: a.offsetTop,
            left: a.offsetLeft
        })
    }, g.pt.tips = function() {
        var a = this,
            b = a.config,
            e = a.layero,
            f = [e.outerWidth(), e.outerHeight()],
            g = c(b.follow);
        g[0] || (g = c("body"));
        var i = {
                width: g.outerWidth(),
                height: g.outerHeight(),
                top: g.offset().top,
                left: g.offset().left
            },
            j = e.find(".layui-layer-TipsG"),
            k = b.tips[0];
        b.tips[1] || j.remove(), i.autoLeft = function() {
            i.left + f[0] - d.width() > 0 ? (i.tipLeft = i.left + i.width - f[0], j.css({
                right: 12,
                left: "auto"
            })) : i.tipLeft = i.left
        }, i.where = [function() {
            i.autoLeft(), i.tipTop = i.top - f[1] - 10, j.removeClass("layui-layer-TipsB").addClass("layui-layer-TipsT").css("border-right-color", b.tips[1])
        }, function() {
            i.tipLeft = i.left + i.width + 10, i.tipTop = i.top, j.removeClass("layui-layer-TipsL").addClass("layui-layer-TipsR").css("border-bottom-color", b.tips[1])
        }, function() {
            i.autoLeft(), i.tipTop = i.top + i.height + 10, j.removeClass("layui-layer-TipsT").addClass("layui-layer-TipsB").css("border-right-color", b.tips[1])
        }, function() {
            i.tipLeft = i.left - f[0] - 10, i.tipTop = i.top, j.removeClass("layui-layer-TipsR").addClass("layui-layer-TipsL").css("border-bottom-color", b.tips[1])
        }], i.where[k - 1](), 1 === k ? i.top - (d.scrollTop() + f[1] + 16) < 0 && i.where[2]() : 2 === k ? d.width() - (i.left + i.width + f[0] + 16) > 0 || i.where[3]() : 3 === k ? i.top - d.scrollTop() + i.height + f[1] + 16 - d.height() > 0 && i.where[0]() : 4 === k && f[0] + 16 - i.left > 0 && i.where[1](), e.find("." + h[5]).css({
            "background-color": b.tips[1],
            "padding-right": b.closeBtn ? "30px" : ""
        }), e.css({
            left: i.tipLeft - (b.fix ? d.scrollLeft() : 0),
            top: i.tipTop - (b.fix ? d.scrollTop() : 0)
        })
    }, g.pt.move = function() {
        var a = this,
            b = a.config,
            e = {
                setY: 0,
                moveLayer: function() {
                    var a = e.layero,
                        b = parseInt(a.css("margin-left")),
                        c = parseInt(e.move.css("left"));
                    0 === b || (c -= b), "fixed" !== a.css("position") && (c -= a.parent().offset().left, e.setY = 0), a.css({
                        left: c,
                        top: parseInt(e.move.css("top")) - e.setY
                    })
                }
            },
            f = a.layero.find(b.move);
        return b.move && f.attr("move", "ok"), f.css({
            cursor: b.move ? "move" : "auto"
        }), c(b.move).on("mousedown", function(a) {
            if (a.preventDefault(), "ok" === c(this).attr("move")) {
                e.ismove = !0, e.layero = c(this).parents("." + h[0]);
                var f = e.layero.offset().left,
                    g = e.layero.offset().top,
                    i = e.layero.outerWidth() - 6,
                    j = e.layero.outerHeight() - 6;
                c("#layui-layer-moves")[0] || c("body").append('<div id="layui-layer-moves" class="layui-layer-moves" style="left:' + f + "px; top:" + g + "px; width:" + i + "px; height:" + j + 'px; z-index:2147483584"></div>'), e.move = c("#layui-layer-moves"), b.moveType && e.move.css({
                    visibility: "hidden"
                }), e.moveX = a.pageX - e.move.position().left, e.moveY = a.pageY - e.move.position().top, "fixed" !== e.layero.css("position") || (e.setY = d.scrollTop())
            }
        }), c(document).mousemove(function(a) {
            if (e.ismove) {
                var c = a.pageX - e.moveX,
                    f = a.pageY - e.moveY;
                if (a.preventDefault(), !b.moveOut) {
                    e.setY = d.scrollTop();
                    var g = d.width() - e.move.outerWidth(),
                        h = e.setY;
                    0 > c && (c = 0), c > g && (c = g), h > f && (f = h), f > d.height() - e.move.outerHeight() + e.setY && (f = d.height() - e.move.outerHeight() + e.setY)
                }
                e.move.css({
                    left: c,
                    top: f
                }), b.moveType && e.moveLayer(), c = f = g = h = null
            }
        }).mouseup(function() {
            try {
                e.ismove && (e.moveLayer(), e.move.remove(), b.moveEnd && b.moveEnd()), e.ismove = !1
            } catch (a) {
                e.ismove = !1
            }
        }), a
    }, g.pt.callback = function() {
        function a() {
            var a = g.cancel && g.cancel(b.index, d);
            a === !1 || f.close(b.index)
        }
        var b = this,
            d = b.layero,
            g = b.config;
        b.openLayer(), g.success && (2 == g.type ? d.find("iframe").on("load", function() {
            g.success(d, b.index)
        }) : g.success(d, b.index)), f.ie6 && b.IE6(d), d.find("." + h[6]).children("a").on("click", function() {
            var a = c(this).index();
            if (0 === a) g.yes ? g.yes(b.index, d) : g.btn1 ? g.btn1(b.index, d) : f.close(b.index);
            else {
                var e = g["btn" + (a + 1)] && g["btn" + (a + 1)](b.index, d);
                e === !1 || f.close(b.index)
            }
        }), d.find("." + h[7]).on("click", a), g.shadeClose && c("#layui-layer-shade" + b.index).on("click", function() {
            f.close(b.index)
        }), d.find(".layui-layer-min").on("click", function() {
            var a = g.min && g.min(d);
            a === !1 || f.min(b.index, g)
        }), d.find(".layui-layer-max").on("click", function() {
            c(this).hasClass("layui-layer-maxmin") ? (f.restore(b.index), g.restore && g.restore(d)) : (f.full(b.index, g), setTimeout(function() {
                g.full && g.full(d)
            }, 100))
        }), g.end && (e.end[b.index] = g.end)
    }, e.reselect = function() {
        c.each(c("select"), function(a, b) {
            var d = c(this);
            d.parents("." + h[0])[0] || 1 == d.attr("layer") && c("." + h[0]).length < 1 && d.removeAttr("layer").show(), d = null
        })
    }, g.pt.IE6 = function(a) {
        function b() {
            a.css({
                top: f + (e.config.fix ? d.scrollTop() : 0)
            })
        }
        var e = this,
            f = a.offset().top;
        b(), d.scroll(b), c("select").each(function(a, b) {
            var d = c(this);
            d.parents("." + h[0])[0] || "none" === d.css("display") || d.attr({
                layer: "1"
            }).hide(), d = null
        })
    }, g.pt.openLayer = function() {
        var a = this;
        f.zIndex = a.config.zIndex, f.setTop = function(a) {
            var b = function() {
                f.zIndex++, a.css("z-index", f.zIndex + 1)
            };
            return f.zIndex = parseInt(a[0].style.zIndex), a.on("mousedown", b), f.zIndex
        }
    }, e.record = function(a) {
        var b = [a.width(), a.height(), a.position().top, a.position().left + parseFloat(a.css("margin-left"))];
        a.find(".layui-layer-max").addClass("layui-layer-maxmin"), a.attr({
            area: b
        })
    }, e.rescollbar = function(a) {
        h.html.attr("layer-full") == a && (h.html[0].style.removeProperty ? h.html[0].style.removeProperty("overflow") : h.html[0].style.removeAttribute("overflow"), h.html.removeAttr("layer-full"))
    }, a.layer = f, f.getChildFrame = function(a, b) {
        return b = b || c("." + h[4]).attr("times"), c("#" + h[0] + b).find("iframe").contents().find(a)
    }, f.getFrameIndex = function(a) {
        return c("#" + a).parents("." + h[4]).attr("times")
    }, f.iframeAuto = function(a) {
        if (a) {
            var b = f.getChildFrame("html", a).outerHeight(),
                d = c("#" + h[0] + a),
                e = d.find(h[1]).outerHeight() || 0,
                g = d.find("." + h[6]).outerHeight() || 0;
            d.css({
                height: b + e + g
            }), d.find("iframe").css({
                height: b
            })
        }
    }, f.iframeSrc = function(a, b) {
        c("#" + h[0] + a).find("iframe").attr("src", b)
    }, f.style = function(a, b) {
        var d = c("#" + h[0] + a),
            f = d.attr("type"),
            g = d.find(h[1]).outerHeight() || 0,
            i = d.find("." + h[6]).outerHeight() || 0;
        (f === e.type[1] || f === e.type[2]) && (d.css(b), f === e.type[2] && d.find("iframe").css({
            height: parseFloat(b.height) - g - i
        }))
    }, f.min = function(a, b) {
        var d = c("#" + h[0] + a),
            g = d.find(h[1]).outerHeight() || 0;
        e.record(d), f.style(a, {
            width: 180,
            height: g,
            overflow: "hidden"
        }), d.find(".layui-layer-min").hide(), "page" === d.attr("type") && d.find(h[4]).hide(), e.rescollbar(a)
    }, f.restore = function(a) {
        var b = c("#" + h[0] + a),
            d = b.attr("area").split(",");
        b.attr("type");
        f.style(a, {
            width: parseFloat(d[0]),
            height: parseFloat(d[1]),
            top: parseFloat(d[2]),
            left: parseFloat(d[3]),
            overflow: "visible"
        }), b.find(".layui-layer-max").removeClass("layui-layer-maxmin"), b.find(".layui-layer-min").show(), "page" === b.attr("type") && b.find(h[4]).show(), e.rescollbar(a)
    }, f.full = function(a) {
        var b, g = c("#" + h[0] + a);
        e.record(g), h.html.attr("layer-full") || h.html.css("overflow", "hidden").attr("layer-full", a), clearTimeout(b), b = setTimeout(function() {
            var b = "fixed" === g.css("position");
            f.style(a, {
                top: b ? 0 : d.scrollTop(),
                left: b ? 0 : d.scrollLeft(),
                width: d.width(),
                height: d.height()
            }), g.find(".layui-layer-min").hide()
        }, 100)
    }, f.title = function(a, b) {
        var d = c("#" + h[0] + (b || f.index)).find(h[1]);
        d.html(a)
    }, f.close = function(a) {
        var b = c("#" + h[0] + a),
            d = b.attr("type");
        if (b[0]) {
            if (d === e.type[1] && "object" === b.attr("conType")) {
                b.children(":not(." + h[5] + ")").remove();
                for (var g = 0; 2 > g; g++) b.find(".layui-layer-wrap").unwrap().hide()
            } else {
                if (d === e.type[2]) try {
                    var i = c("#" + h[4] + a)[0];
                    i.contentWindow.document.write(""), i.contentWindow.close(), b.find("." + h[5])[0].removeChild(i)
                } catch (j) {}
                b[0].innerHTML = "", b.remove()
            }
            c("#layui-layer-moves, #layui-layer-shade" + a).remove(), f.ie6 && e.reselect(), e.rescollbar(a), c(document).off("keydown", e.enter), "function" == typeof e.end[a] && e.end[a](), delete e.end[a]
        }
    }, f.closeAll = function(a) {
        c.each(c("." + h[0]), function() {
            var b = c(this),
                d = a ? b.attr("type") === a : 1;
            d && f.close(b.attr("times")), d = null
        })
    };
    var i = f.cache || {},
        j = function(a) {
            return i.skin ? " " + i.skin + " " + i.skin + "-" + a : ""
        };
    f.prompt = function(a, b) {
        a = a || {}, "function" == typeof a && (b = a);
        var d, e = 2 == a.formType ? '<textarea class="layui-layer-input">' + (a.value || "") + "</textarea>" : function() {
            return '<input type="' + (1 == a.formType ? "password" : "text") + '" class="layui-layer-input" value="' + (a.value || "") + '">'
        }();
        return f.open(c.extend({
            btn: ["&#x786E;&#x5B9A;", "&#x53D6;&#x6D88;"],
            content: e,
            skin: "layui-layer-prompt" + j("prompt"),
            success: function(a) {
                d = a.find(".layui-layer-input"), d.focus()
            },
            yes: function(c) {
                var e = d.val();
                "" === e ? d.focus() : e.length > (a.maxlength || 500) ? f.tips("&#x6700;&#x591A;&#x8F93;&#x5165;" + (a.maxlength || 500) + "&#x4E2A;&#x5B57;&#x6570;", d, {
                    tips: 1
                }) : b && b(e, c, d)
            }
        }, a))
    }, f.tab = function(a) {
        a = a || {};
        var b = a.tab || {};
        return f.open(c.extend({
            type: 1,
            skin: "layui-layer-tab" + j("tab"),
            title: function() {
                var a = b.length,
                    c = 1,
                    d = "";
                if (a > 0)
                    for (d = '<span class="layui-layer-tabnow">' + b[0].title + "</span>"; a > c; c++) d += "<span>" + b[c].title + "</span>";
                return d
            }(),
            content: '<ul class="layui-layer-tabmain">' + function() {
                var a = b.length,
                    c = 1,
                    d = "";
                if (a > 0)
                    for (d = '<li class="layui-layer-tabli xubox_tab_layer">' + (b[0].content || "no content") + "</li>"; a > c; c++) d += '<li class="layui-layer-tabli">' + (b[c].content || "no  content") + "</li>";
                return d
            }() + "</ul>",
            success: function(b) {
                var d = b.find(".layui-layer-title").children(),
                    e = b.find(".layui-layer-tabmain").children();
                d.on("mousedown", function(b) {
                    b.stopPropagation ? b.stopPropagation() : b.cancelBubble = !0;
                    var d = c(this),
                        f = d.index();
                    d.addClass("layui-layer-tabnow").siblings().removeClass("layui-layer-tabnow"), e.eq(f).show().siblings().hide(), "function" == typeof a.change && a.change(f)
                })
            }
        }, a))
    }, f.photos = function(b, d, e) {
        function g(a, b, c) {
            var d = new Image;
            return d.src = a, d.complete ? b(d) : (d.onload = function() {
                d.onload = null, b(d)
            }, void(d.onerror = function(a) {
                d.onerror = null, c(a)
            }))
        }
        var h = {};
        if (b = b || {}, b.photos) {
            var i = b.photos.constructor === Object,
                k = i ? b.photos : {},
                l = k.data || [],
                m = k.start || 0;
            if (h.imgIndex = (0 | m) + 1, b.img = b.img || "img", i) {
                if (0 === l.length) return f.msg("&#x6CA1;&#x6709;&#x56FE;&#x7247;")
            } else {
                var n = c(b.photos),
                    o = function() {
                        l = [], n.find(b.img).each(function(a) {
                            var b = c(this);
                            b.attr("layer-index", a), l.push({
                                alt: b.attr("alt"),
                                pid: b.attr("layer-pid"),
                                src: b.attr("layer-src") || b.attr("src"),
                                thumb: b.attr("src")
                            })
                        })
                    };
                if (o(), 0 === l.length) return;
                if (d || n.on("click", b.img, function() {
                        var a = c(this),
                            d = a.attr("layer-index");
                        f.photos(c.extend(b, {
                            photos: {
                                start: d,
                                data: l,
                                tab: b.tab
                            },
                            full: b.full
                        }), !0), o()
                    }), !d) return
            }
            h.imgprev = function(a) {
                h.imgIndex--, h.imgIndex < 1 && (h.imgIndex = l.length), h.tabimg(a)
            }, h.imgnext = function(a, b) {
                h.imgIndex++, h.imgIndex > l.length && (h.imgIndex = 1, b) || h.tabimg(a)
            }, h.keyup = function(a) {
                if (!h.end) {
                    var b = a.keyCode;
                    a.preventDefault(), 37 === b ? h.imgprev(!0) : 39 === b ? h.imgnext(!0) : 27 === b && f.close(h.index)
                }
            }, h.tabimg = function(a) {
                l.length <= 1 || (k.start = h.imgIndex - 1, f.close(h.index), f.photos(b, !0, a))
            }, h.event = function() {
                h.bigimg.hover(function() {
                    h.imgsee.show()
                }, function() {
                    h.imgsee.hide()
                }), h.bigimg.find(".layui-layer-imgprev").on("click", function(a) {
                    a.preventDefault(), h.imgprev()
                }), h.bigimg.find(".layui-layer-imgnext").on("click", function(a) {
                    a.preventDefault(), h.imgnext()
                }), c(document).on("keyup", h.keyup)
            }, h.loadi = f.load(1, {
                shade: "shade" in b ? !1 : .9,
                scrollbar: !1
            }), g(l[m].src, function(d) {
                f.close(h.loadi), h.index = f.open(c.extend({
                    type: 1,
                    area: function() {
                        var e = [d.width, d.height],
                            f = [c(a).width() - 50, c(a).height() - 50];
                        return !b.full && e[0] > f[0] && (e[0] = f[0], e[1] = e[0] * d.height / d.width), [e[0] + "px", e[1] + "px"]
                    }(),
                    title: !1,
                    shade: .9,
                    shadeClose: !0,
                    closeBtn: !1,
                    move: ".layui-layer-phimg img",
                    moveType: 1,
                    scrollbar: !1,
                    moveOut: !0,
                    shift: 5 * Math.random() | 0,
                    skin: "layui-layer-photos" + j("photos"),
                    content: '<div class="layui-layer-phimg"><img src="' + l[m].src + '" alt="' + (l[m].alt || "") + '" layer-pid="' + l[m].pid + '"><div class="layui-layer-imgsee">' + (l.length > 1 ? '<span class="layui-layer-imguide"><a href="javascript:;" class="layui-layer-iconext layui-layer-imgprev"></a><a href="javascript:;" class="layui-layer-iconext layui-layer-imgnext"></a></span>' : "") + '<div class="layui-layer-imgbar" style="display:' + (e ? "block" : "") + '"><span class="layui-layer-imgtit"><a href="javascript:;">' + (l[m].alt || "") + "</a><em>" + h.imgIndex + "/" + l.length + "</em></span></div></div></div>",
                    success: function(a, c) {
                        h.bigimg = a.find(".layui-layer-phimg"), h.imgsee = a.find(".layui-layer-imguide,.layui-layer-imgbar"), h.event(a), b.tab && b.tab(l[m], a)
                    },
                    end: function() {
                        h.end = !0, c(document).off("keyup", h.keyup)
                    }
                }, b))
            }, function() {
                f.close(h.loadi), f.msg("&#x5F53;&#x524D;&#x56FE;&#x7247;&#x5730;&#x5740;&#x5F02;&#x5E38;<br>&#x662F;&#x5426;&#x7EE7;&#x7EED;&#x67E5;&#x770B;&#x4E0B;&#x4E00;&#x5F20;&#xFF1F;", {
                    time: 3e4,
                    btn: ["&#x4E0B;&#x4E00;&#x5F20;", "&#x4E0D;&#x770B;&#x4E86;"],
                    yes: function() {
                        l.length > 1 && h.imgnext(!0, !0)
                    }
                })
            })
        }
    }, e.run = function() {
        c = jQuery, d = c(a), h.html = c("html"), f.open = function(a) {
            var b = new g(a);
            return b.index
        }
    }, "function" == typeof define ? define(function() {
        return e.run(), f
    }) : function() {
        e.run()
    }()
}(window);

//-------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------

(function(window, undefined) {
    //-------------------------------------------------------------------------------------------
    var document = window.document,
        navigator = window.navigator,
        location = window.location;

    //-------------------------------------------------------------------------------------------
    /*
    模拟c# String.Format函数
    */
    String.prototype.format = function(args) {
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
    Date.prototype.Format = function(formatStr) {
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
                if ((arg0.constructor == Event || arg0.constructor == MouseEvent) || (typeof(arg0) == "object" && arg0.preventDefault && arg0.stopPropagation)) {
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
        if (typeof(v) == 'undefined' || v == null) {
            return false;
        }
        return true;
    }
    /*
    变量为函数
    */
    function isFunc(v) {
        if (typeof(v) == 'function') {
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
            } else {
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
            $('#' + menuId).bind(self.options.eventType, function() {
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
    panelSwitch.prototype.show = function(showMenuId) {
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
                        } else {
                            $('#' + panelId).show();
                        }
                        break;
                    case 1:
                        if (self.options.animateTimespan > 0) {
                            $('#' + panelId).fadeIn(self.options.animateTimespan);
                        } else {
                            $('#' + panelId).fadeIn();
                        }
                        break;
                }
                if (self.options.callback != null) {
                    self.options.callback(showMenuId, panelId);
                }
            } else {
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

            menu.bind(self.options.triggerMethod, function() {
                self.show($(this).attr("id"));
            });
            menu.bind(self.options.cancelMethod, function() {
                self.hide($(this).attr("id"));
            });
            box.bind("mouseover", function() {
                self.show($(this).attr("id"));
            });
            box.bind("mouseout", function() {
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
    popupBox.prototype._getMenuAndBox = function(objId) {
        var self = this;
        var mid = null,
            bid = null;
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
            return {
                "menu": m,
                "box": b
            };
        } else {
            return null;
        }
    };
    /*
    显示容器块
    */
    popupBox.prototype.show = function(objId) {
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
                box.css({
                    "position": fixedPosition,
                    "top": fixTop,
                    "left": fixLeft
                });
            }
        }

        box.css({
            "z-index": 2015
        });

        if (self.options.activeMenuCss.length > 0) {
            if (!menu.hasClass(self.options.activeMenuCss)) {
                menu.addClass(self.options.activeMenuCss);
            }
        }

        if (self.options.fadeInMilSeconds > 0) {
            box.fadeIn(self.options.fadeInMilSeconds);
        } else {
            box.show();
        }
    };
    /*
    隐藏容器块
    */
    popupBox.prototype.hide = function(objId) {
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

        self.hoverTimer[menuId] = setTimeout(function() {
            if (self.options.activeMenuCss.length > 0) {
                if (menu.hasClass(self.options.activeMenuCss)) {
                    menu.removeClass(self.options.activeMenuCss);
                }
            }
            if (self.options.fadeOutMilSeconds > 0) {
                box.fadeOut(self.options.fadeOutMilSeconds);
            } else {
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
        幻灯片 20160721新增
     * SuperSlide v2.1.2
     * 轻松解决网站大部分特效展示问题
     * 详尽信息请看官网：http://www.SuperSlide2.com/
     *
     * Copyright 2011-2015, 大话主席
     * 
     * 查看参数：http://www.superslide2.com/param.html
     * 普通用法：jQuery(".picMarquee-left").slide({mainCell:".bd ul",autoPlay:true,effect:"leftMarquee",vis:3,interTime:50,trigger:"click"});
    */
    (function($) {
        $.fn.slide = function(options) {
            $.fn.slide.defaults = {
                type: "slide",
                effect: "fade",
                autoPlay: false,
                delayTime: 500,
                interTime: 2500,
                triggerTime: 150,
                defaultIndex: 0,
                titCell: ".hd li",
                mainCell: ".bd",
                targetCell: null,
                trigger: "mouseover",
                scroll: 1,
                vis: 1,
                titOnClassName: "on",
                autoPage: false,
                prevCell: ".prev",
                nextCell: ".next",
                pageStateCell: ".pageState",
                opp: false,
                pnLoop: true,
                easing: "swing",
                startFun: null,
                endFun: null,
                switchLoad: null,

                playStateCell: ".playState",
                mouseOverStop: true,
                defaultPlay: true,
                returnDefault: false
            };

            return this.each(function() {

                var opts = $.extend({}, $.fn.slide.defaults, options);
                var slider = $(this);
                var effect = opts.effect;
                var prevBtn = $(opts.prevCell, slider);
                var nextBtn = $(opts.nextCell, slider);
                var pageState = $(opts.pageStateCell, slider);
                var playState = $(opts.playStateCell, slider);

                var navObj = $(opts.titCell, slider); //导航子元素结合
                var navObjSize = navObj.size();
                var conBox = $(opts.mainCell, slider); //内容元素父层对象
                var conBoxSize = conBox.children().size();
                var sLoad = opts.switchLoad;
                var tarObj = $(opts.targetCell, slider);

                /*字符串转换*/
                var index = parseInt(opts.defaultIndex);
                var delayTime = parseInt(opts.delayTime);
                var interTime = parseInt(opts.interTime);
                var triggerTime = parseInt(opts.triggerTime);
                var scroll = parseInt(opts.scroll);
                var autoPlay = (opts.autoPlay == "false" || opts.autoPlay == false) ? false : true;
                var opp = (opts.opp == "false" || opts.opp == false) ? false : true;
                var autoPage = (opts.autoPage == "false" || opts.autoPage == false) ? false : true;
                var pnLoop = (opts.pnLoop == "false" || opts.pnLoop == false) ? false : true;
                var mouseOverStop = (opts.mouseOverStop == "false" || opts.mouseOverStop == false) ? false : true;
                var defaultPlay = (opts.defaultPlay == "false" || opts.defaultPlay == false) ? false : true;
                var returnDefault = (opts.returnDefault == "false" || opts.returnDefault == false) ? false : true;
                var vis = isNaN(opts.vis) ? 1 : parseInt(opts.vis);

                var isIE6 = !-[1, ] && !window.XMLHttpRequest; //v2.1.2

                var slideH = 0;
                var slideW = 0;
                var selfW = 0;
                var selfH = 0;
                var easing = opts.easing;
                var inter = null; //autoPlay-setInterval 
                var mst = null; //trigger-setTimeout
                var rtnST = null; //returnDefault-setTimeout
                var titOn = opts.titOnClassName;

                var onIndex = navObj.index(slider.find("." + titOn));
                var oldIndex = index = onIndex == -1 ? index : onIndex;
                var defaultIndex = index;


                var _ind = index;
                var cloneNum = conBoxSize >= vis ? (conBoxSize % scroll != 0 ? conBoxSize % scroll : scroll) : 0;
                var _tar;
                var isMarq = effect == "leftMarquee" || effect == "topMarquee" ? true : false;

                var doStartFun = function() {
                    if ($.isFunction(opts.startFun)) {
                        opts.startFun(index, navObjSize, slider, $(opts.titCell, slider), conBox, tarObj, prevBtn, nextBtn)
                    }
                }
                var doEndFun = function() {
                    if ($.isFunction(opts.endFun)) {
                        opts.endFun(index, navObjSize, slider, $(opts.titCell, slider), conBox, tarObj, prevBtn, nextBtn)
                    }
                }
                var resetOn = function() {
                    navObj.removeClass(titOn);
                    if (defaultPlay) navObj.eq(defaultIndex).addClass(titOn)
                }



                //单独处理菜单效果
                if (opts.type == "menu") {

                    if (defaultPlay) {
                        navObj.removeClass(titOn).eq(index).addClass(titOn);
                    }
                    navObj.hover(
                        function() {
                            _tar = $(this).find(opts.targetCell);
                            var hoverInd = navObj.index($(this));

                            mst = setTimeout(function() {
                                index = hoverInd;
                                navObj.removeClass(titOn).eq(index).addClass(titOn);
                                doStartFun();
                                switch (effect) {
                                    case "fade":
                                        _tar.stop(true, true).animate({
                                            opacity: "show"
                                        }, delayTime, easing, doEndFun);
                                        break;
                                    case "slideDown":
                                        _tar.stop(true, true).animate({
                                            height: "show"
                                        }, delayTime, easing, doEndFun);
                                        break;
                                }
                            }, opts.triggerTime);

                        },
                        function() {
                            clearTimeout(mst);
                            switch (effect) {
                                case "fade":
                                    _tar.animate({
                                        opacity: "hide"
                                    }, delayTime, easing);
                                    break;
                                case "slideDown":
                                    _tar.animate({
                                        height: "hide"
                                    }, delayTime, easing);
                                    break;
                            }
                        }
                    );

                    if (returnDefault) {
                        slider.hover(function() {
                            clearTimeout(rtnST);
                        }, function() {
                            rtnST = setTimeout(resetOn, delayTime);
                        });
                    }


                    return;
                }


                //处理分页
                if (navObjSize == 0) navObjSize = conBoxSize; //只有左右按钮
                if (isMarq) navObjSize = 2;
                if (autoPage) {
                    if (conBoxSize >= vis) {
                        if (effect == "leftLoop" || effect == "topLoop") {
                            navObjSize = conBoxSize % scroll != 0 ? (conBoxSize / scroll ^ 0) + 1 : conBoxSize / scroll;
                        } else {
                            var tempS = conBoxSize - vis;
                            navObjSize = 1 + parseInt(tempS % scroll != 0 ? (tempS / scroll + 1) : (tempS / scroll));
                            if (navObjSize <= 0) navObjSize = 1;
                        }
                    } else {
                        navObjSize = 1
                    }

                    navObj.html("");
                    var str = "";

                    if (opts.autoPage == true || opts.autoPage == "true") {
                        for (var i = 0; i < navObjSize; i++) {
                            str += "<li>" + (i + 1) + "</li>"
                        }
                    } else {
                        for (var i = 0; i < navObjSize; i++) {
                            str += opts.autoPage.replace("$", (i + 1))
                        }
                    }
                    navObj.html(str);

                    var navObj = navObj.children(); //重置导航子元素对象
                }


                if (conBoxSize >= vis) { //当内容个数少于可视个数，不执行效果。
                    conBox.children().each(function() { //取最大值
                        if ($(this).width() > selfW) {
                            selfW = $(this).width();
                            slideW = $(this).outerWidth(true);
                        }
                        if ($(this).height() > selfH) {
                            selfH = $(this).height();
                            slideH = $(this).outerHeight(true);
                        }
                    });

                    var _chr = conBox.children();
                    var cloneEle = function() {
                        for (var i = 0; i < vis; i++) {
                            _chr.eq(i).clone().addClass("clone").appendTo(conBox);
                        }
                        for (var i = 0; i < cloneNum; i++) {
                            _chr.eq(conBoxSize - i - 1).clone().addClass("clone").prependTo(conBox);
                        }
                    }

                    switch (effect) {
                        case "fold":
                            conBox.css({
                                "position": "relative",
                                "width": slideW,
                                "height": slideH
                            }).children().css({
                                "position": "absolute",
                                "width": selfW,
                                "left": 0,
                                "top": 0,
                                "display": "none"
                            });
                            break;
                        case "top":
                            conBox.wrap('<div class="tempWrap" style="overflow:hidden; position:relative; height:' + vis * slideH + 'px"></div>').css({
                                "top": -(index * scroll) * slideH,
                                "position": "relative",
                                "padding": "0",
                                "margin": "0"
                            }).children().css({
                                "height": selfH
                            });
                            break;
                        case "left":
                            conBox.wrap('<div class="tempWrap" style="overflow:hidden; position:relative; width:' + vis * slideW + 'px"></div>').css({
                                "width": conBoxSize * slideW,
                                "left": -(index * scroll) * slideW,
                                "position": "relative",
                                "overflow": "hidden",
                                "padding": "0",
                                "margin": "0"
                            }).children().css({
                                "float": "left",
                                "width": selfW
                            });
                            break;
                        case "leftLoop":
                        case "leftMarquee":
                            cloneEle();
                            conBox.wrap('<div class="tempWrap" style="overflow:hidden; position:relative; width:' + vis * slideW + 'px"></div>').css({
                                "width": (conBoxSize + vis + cloneNum) * slideW,
                                "position": "relative",
                                "overflow": "hidden",
                                "padding": "0",
                                "margin": "0",
                                "left": -(cloneNum + index * scroll) * slideW
                            }).children().css({
                                "float": "left",
                                "width": selfW
                            });
                            break;
                        case "topLoop":
                        case "topMarquee":
                            cloneEle();
                            conBox.wrap('<div class="tempWrap" style="overflow:hidden; position:relative; height:' + vis * slideH + 'px"></div>').css({
                                "height": (conBoxSize + vis + cloneNum) * slideH,
                                "position": "relative",
                                "padding": "0",
                                "margin": "0",
                                "top": -(cloneNum + index * scroll) * slideH
                            }).children().css({
                                "height": selfH
                            });
                            break;
                    }
                }



                //针对leftLoop、topLoop的滚动个数
                var scrollNum = function(ind) {
                    var _tempCs = ind * scroll;
                    if (ind == navObjSize) {
                        _tempCs = conBoxSize;
                    } else if (ind == -1 && conBoxSize % scroll != 0) {
                        _tempCs = -conBoxSize % scroll;
                    }
                    return _tempCs;
                }

                //切换加载
                var doSwitchLoad = function(objs) {

                        var changeImg = function(t) {
                            for (var i = t; i < (vis + t); i++) {
                                objs.eq(i).find("img[" + sLoad + "]").each(function() {
                                    var _this = $(this);
                                    _this.attr("src", _this.attr(sLoad)).removeAttr(sLoad);
                                    if (conBox.find(".clone")[0]) { //如果存在.clone
                                        var chir = conBox.children();
                                        for (var j = 0; j < chir.size(); j++) {
                                            chir.eq(j).find("img[" + sLoad + "]").each(function() {
                                                if ($(this).attr(sLoad) == _this.attr("src")) $(this).attr("src", $(this).attr(sLoad)).removeAttr(sLoad)
                                            })
                                        }
                                    }
                                })
                            }
                        }

                        switch (effect) {
                            case "fade":
                            case "fold":
                            case "top":
                            case "left":
                            case "slideDown":
                                changeImg(index * scroll);
                                break;
                            case "leftLoop":
                            case "topLoop":
                                changeImg(cloneNum + scrollNum(_ind));
                                break;
                            case "leftMarquee":
                            case "topMarquee":
                                var curS = effect == "leftMarquee" ? conBox.css("left").replace("px", "") : conBox.css("top").replace("px", "");
                                var slideT = effect == "leftMarquee" ? slideW : slideH;
                                var mNum = cloneNum;
                                if (curS % slideT != 0) {
                                    var curP = Math.abs(curS / slideT ^ 0);
                                    if (index == 1) {
                                        mNum = cloneNum + curP
                                    } else {
                                        mNum = cloneNum + curP - 1
                                    }
                                }
                                changeImg(mNum);
                                break;
                        }
                    } //doSwitchLoad end


                //效果函数
                var doPlay = function(init) {
                    // 当前页状态不触发效果
                    if (defaultPlay && oldIndex == index && !init && !isMarq) return;

                    //处理页码
                    if (isMarq) {
                        if (index >= 1) {
                            index = 1;
                        } else if (index <= 0) {
                            index = 0;
                        }
                    } else {
                        _ind = index;
                        if (index >= navObjSize) {
                            index = 0;
                        } else if (index < 0) {
                            index = navObjSize - 1;
                        }
                    }

                    doStartFun();

                    //处理切换加载
                    if (sLoad != null) {
                        doSwitchLoad(conBox.children())
                    }

                    //处理targetCell
                    if (tarObj[0]) {
                        _tar = tarObj.eq(index);
                        if (sLoad != null) {
                            doSwitchLoad(tarObj)
                        }
                        if (effect == "slideDown") {
                            tarObj.not(_tar).stop(true, true).slideUp(delayTime);
                            _tar.slideDown(delayTime, easing, function() {
                                if (!conBox[0]) doEndFun()
                            });
                        } else {
                            tarObj.not(_tar).stop(true, true).hide();
                            _tar.animate({
                                opacity: "show"
                            }, delayTime, function() {
                                if (!conBox[0]) doEndFun()
                            });
                        }
                    }

                    if (conBoxSize >= vis) { //当内容个数少于可视个数，不执行效果。
                        switch (effect) {
                            case "fade":
                                conBox.children().stop(true, true).eq(index).animate({
                                    opacity: "show"
                                }, delayTime, easing, function() {
                                    doEndFun()
                                }).siblings().hide();
                                break;
                            case "fold":
                                conBox.children().stop(true, true).eq(index).animate({
                                    opacity: "show"
                                }, delayTime, easing, function() {
                                    doEndFun()
                                }).siblings().animate({
                                    opacity: "hide"
                                }, delayTime, easing);
                                break;
                            case "top":
                                conBox.stop(true, false).animate({
                                    "top": -index * scroll * slideH
                                }, delayTime, easing, function() {
                                    doEndFun()
                                });
                                break;
                            case "left":
                                conBox.stop(true, false).animate({
                                    "left": -index * scroll * slideW
                                }, delayTime, easing, function() {
                                    doEndFun()
                                });
                                break;
                            case "leftLoop":
                                var __ind = _ind;
                                conBox.stop(true, true).animate({
                                    "left": -(scrollNum(_ind) + cloneNum) * slideW
                                }, delayTime, easing, function() {
                                    if (__ind <= -1) {
                                        conBox.css("left", -(cloneNum + (navObjSize - 1) * scroll) * slideW);
                                    } else if (__ind >= navObjSize) {
                                        conBox.css("left", -cloneNum * slideW);
                                    }
                                    doEndFun();
                                });
                                break; //leftLoop end

                            case "topLoop":
                                var __ind = _ind;
                                conBox.stop(true, true).animate({
                                    "top": -(scrollNum(_ind) + cloneNum) * slideH
                                }, delayTime, easing, function() {
                                    if (__ind <= -1) {
                                        conBox.css("top", -(cloneNum + (navObjSize - 1) * scroll) * slideH);
                                    } else if (__ind >= navObjSize) {
                                        conBox.css("top", -cloneNum * slideH);
                                    }
                                    doEndFun();
                                });
                                break; //topLoop end

                            case "leftMarquee":
                                var tempLeft = conBox.css("left").replace("px", "");
                                if (index == 0) {
                                    conBox.animate({
                                        "left": ++tempLeft
                                    }, 0, function() {
                                        if (conBox.css("left").replace("px", "") >= 0) {
                                            conBox.css("left", -conBoxSize * slideW)
                                        }
                                    });
                                } else {
                                    conBox.animate({
                                        "left": --tempLeft
                                    }, 0, function() {
                                        if (conBox.css("left").replace("px", "") <= -(conBoxSize + cloneNum) * slideW) {
                                            conBox.css("left", -cloneNum * slideW)
                                        }
                                    });
                                }
                                break; // leftMarquee end

                            case "topMarquee":
                                var tempTop = conBox.css("top").replace("px", "");
                                if (index == 0) {
                                    conBox.animate({
                                        "top": ++tempTop
                                    }, 0, function() {
                                        if (conBox.css("top").replace("px", "") >= 0) {
                                            conBox.css("top", -conBoxSize * slideH)
                                        }
                                    });
                                } else {
                                    conBox.animate({
                                        "top": --tempTop
                                    }, 0, function() {
                                        if (conBox.css("top").replace("px", "") <= -(conBoxSize + cloneNum) * slideH) {
                                            conBox.css("top", -cloneNum * slideH)
                                        }
                                    });
                                }
                                break; // topMarquee end

                        } //switch end
                    }

                    navObj.removeClass(titOn).eq(index).addClass(titOn);
                    oldIndex = index;
                    if (!pnLoop) { //pnLoop控制前后按钮是否继续循环
                        nextBtn.removeClass("nextStop");
                        prevBtn.removeClass("prevStop");
                        if (index == 0) {
                            prevBtn.addClass("prevStop");
                        }
                        if (index == navObjSize - 1) {
                            nextBtn.addClass("nextStop");
                        }
                    }

                    pageState.html("<span>" + (index + 1) + "</span>/" + navObjSize);

                }; // doPlay end

                //初始化执行
                if (defaultPlay) {
                    doPlay(true);
                }

                if (returnDefault) //返回默认状态
                {
                    slider.hover(function() {
                        clearTimeout(rtnST)
                    }, function() {
                        rtnST = setTimeout(function() {
                            index = defaultIndex;
                            if (defaultPlay) {
                                doPlay();
                            } else {
                                if (effect == "slideDown") {
                                    _tar.slideUp(delayTime, resetOn);
                                } else {
                                    _tar.animate({
                                        opacity: "hide"
                                    }, delayTime, resetOn);
                                }
                            }
                            oldIndex = index;
                        }, 300);
                    });
                }

                ///自动播放函数
                var setInter = function(time) {
                    inter = setInterval(function() {
                        opp ? index-- : index++;
                        doPlay()
                    }, !!time ? time : interTime);
                }
                var setMarInter = function(time) {
                        inter = setInterval(doPlay, !!time ? time : interTime);
                    }
                    // 处理mouseOverStop
                var resetInter = function() {
                        if (!mouseOverStop && autoPlay && !playState.hasClass("pauseState")) {
                            clearInterval(inter);
                            setInter()
                        }
                    } /* 修复 mouseOverStop 和 autoPlay均为false下，点击切换按钮后会自动播放bug */
                    // 前后按钮触发
                var nextTrigger = function() {
                    if (pnLoop || index != navObjSize - 1) {
                        index++;
                        doPlay();
                        if (!isMarq) resetInter();
                    }
                }
                var prevTrigger = function() {
                        if (pnLoop || index != 0) {
                            index--;
                            doPlay();
                            if (!isMarq) resetInter();
                        }
                    }
                    //处理playState
                var playStateFun = function() {
                    clearInterval(inter);
                    isMarq ? setMarInter() : setInter();
                    playState.removeClass("pauseState")
                }
                var pauseStateFun = function() {
                    clearInterval(inter);
                    playState.addClass("pauseState");
                }

                //自动播放
                if (autoPlay) {
                    if (isMarq) {
                        opp ? index-- : index++;
                        setMarInter();
                        if (mouseOverStop) conBox.hover(pauseStateFun, playStateFun);
                    } else {
                        setInter();
                        if (mouseOverStop) slider.hover(pauseStateFun, playStateFun);
                    }
                } else {
                    if (isMarq) {
                        opp ? index-- : index++;
                    }
                    playState.addClass("pauseState");
                }


                //暂停按钮
                playState.click(function() {
                    playState.hasClass("pauseState") ? playStateFun() : pauseStateFun()
                });

                //titCell事件
                if (opts.trigger == "mouseover") {
                    navObj.hover(function() {
                        var hoverInd = navObj.index(this);
                        mst = setTimeout(function() {
                            index = hoverInd;
                            doPlay();
                            resetInter();
                        }, opts.triggerTime);
                    }, function() {
                        clearTimeout(mst)
                    });
                } else {
                    navObj.click(function() {
                        index = navObj.index(this);
                        doPlay();
                        resetInter();
                    })
                }

                //前后按钮事件
                if (isMarq) {

                    nextBtn.mousedown(nextTrigger);
                    prevBtn.mousedown(prevTrigger);
                    //前后按钮长按10倍加速
                    if (pnLoop) {
                        var st;
                        var marDown = function() {
                            st = setTimeout(function() {
                                clearInterval(inter);
                                setMarInter(interTime / 10 ^ 0)
                            }, 150)
                        }
                        var marUp = function() {
                            clearTimeout(st);
                            clearInterval(inter);
                            setMarInter()
                        }
                        nextBtn.mousedown(marDown);
                        nextBtn.mouseup(marUp);
                        prevBtn.mousedown(marDown);
                        prevBtn.mouseup(marUp);
                    }
                    //前后按钮mouseover事件
                    if (opts.trigger == "mouseover") {
                        nextBtn.hover(nextTrigger, function() {});
                        prevBtn.hover(prevTrigger, function() {});
                    }
                } else {
                    nextBtn.click(nextTrigger);
                    prevBtn.click(prevTrigger);
                }


                //检测设备尺寸变化
                if (opts.vis == "auto" && scroll == 1 && (effect == "left" || effect == "leftLoop")) {

                    var resizeTimer;

                    var orientationChange = function() {

                        if (isIE6) {
                            conBox.width("auto");
                            conBox.children().width("auto");
                        }
                        conBox.parent().width("auto");
                        slideW = conBox.parent().width();

                        if (isIE6) {
                            conBox.parent().width(slideW)
                        }

                        conBox.children().width(slideW);

                        if (effect == "left") {
                            conBox.width(slideW * conBoxSize);
                            conBox.stop(true, false).animate({
                                "left": -index * slideW
                            }, 0);
                        } else {
                            conBox.width(slideW * (conBoxSize + 2));
                            conBox.stop(true, false).animate({
                                "left": -(index + 1) * slideW
                            }, 0);
                        }

                        if (!isIE6 && (slideW != conBox.parent().width())) {
                            orientationChange();
                        }

                    }

                    $(window).resize(function() {

                        clearTimeout(resizeTimer);
                        resizeTimer = setTimeout(orientationChange, 100);

                    });
                    orientationChange();
                }


            }); //each End

        }; //slide End

    })(jQuery);

    jQuery.easing['jswing'] = jQuery.easing['swing'];
    jQuery.extend(jQuery.easing, {
        def: 'easeOutQuad',
        swing: function(x, t, b, c, d) {
            return jQuery.easing[jQuery.easing.def](x, t, b, c, d);
        },
        easeInQuad: function(x, t, b, c, d) {
            return c * (t /= d) * t + b;
        },
        easeOutQuad: function(x, t, b, c, d) {
            return -c * (t /= d) * (t - 2) + b
        },
        easeInOutQuad: function(x, t, b, c, d) {
            if ((t /= d / 2) < 1) return c / 2 * t * t + b;
            return -c / 2 * ((--t) * (t - 2) - 1) + b
        },
        easeInCubic: function(x, t, b, c, d) {
            return c * (t /= d) * t * t + b
        },
        easeOutCubic: function(x, t, b, c, d) {
            return c * ((t = t / d - 1) * t * t + 1) + b
        },
        easeInOutCubic: function(x, t, b, c, d) {
            if ((t /= d / 2) < 1) return c / 2 * t * t * t + b;
            return c / 2 * ((t -= 2) * t * t + 2) + b
        },
        easeInQuart: function(x, t, b, c, d) {
            return c * (t /= d) * t * t * t + b
        },
        easeOutQuart: function(x, t, b, c, d) {
            return -c * ((t = t / d - 1) * t * t * t - 1) + b
        },
        easeInOutQuart: function(x, t, b, c, d) {
            if ((t /= d / 2) < 1) return c / 2 * t * t * t * t + b;
            return -c / 2 * ((t -= 2) * t * t * t - 2) + b
        },
        easeInQuint: function(x, t, b, c, d) {
            return c * (t /= d) * t * t * t * t + b
        },
        easeOutQuint: function(x, t, b, c, d) {
            return c * ((t = t / d - 1) * t * t * t * t + 1) + b
        },
        easeInOutQuint: function(x, t, b, c, d) {
            if ((t /= d / 2) < 1) return c / 2 * t * t * t * t * t + b;
            return c / 2 * ((t -= 2) * t * t * t * t + 2) + b
        },
        easeInSine: function(x, t, b, c, d) {
            return -c * Math.cos(t / d * (Math.PI / 2)) + c + b
        },
        easeOutSine: function(x, t, b, c, d) {
            return c * Math.sin(t / d * (Math.PI / 2)) + b
        },
        easeInOutSine: function(x, t, b, c, d) {
            return -c / 2 * (Math.cos(Math.PI * t / d) - 1) + b
        },
        easeInExpo: function(x, t, b, c, d) {
            return (t == 0) ? b : c * Math.pow(2, 10 * (t / d - 1)) + b
        },
        easeOutExpo: function(x, t, b, c, d) {
            return (t == d) ? b + c : c * (-Math.pow(2, -10 * t / d) + 1) + b
        },
        easeInOutExpo: function(x, t, b, c, d) {
            if (t == 0) return b;
            if (t == d) return b + c;
            if ((t /= d / 2) < 1) return c / 2 * Math.pow(2, 10 * (t - 1)) + b;
            return c / 2 * (-Math.pow(2, -10 * --t) + 2) + b
        },
        easeInCirc: function(x, t, b, c, d) {
            return -c * (Math.sqrt(1 - (t /= d) * t) - 1) + b
        },
        easeOutCirc: function(x, t, b, c, d) {
            return c * Math.sqrt(1 - (t = t / d - 1) * t) + b
        },
        easeInOutCirc: function(x, t, b, c, d) {
            if ((t /= d / 2) < 1) return -c / 2 * (Math.sqrt(1 - t * t) - 1) + b;
            return c / 2 * (Math.sqrt(1 - (t -= 2) * t) + 1) + b
        },
        easeInElastic: function(x, t, b, c, d) {
            var s = 1.70158;
            var p = 0;
            var a = c;
            if (t == 0) return b;
            if ((t /= d) == 1) return b + c;
            if (!p) p = d * .3;
            if (a < Math.abs(c)) {
                a = c;
                var s = p / 4;
            } else var s = p / (2 * Math.PI) * Math.asin(c / a);
            return -(a * Math.pow(2, 10 * (t -= 1)) * Math.sin((t * d - s) * (2 * Math.PI) / p)) + b
        },
        easeOutElastic: function(x, t, b, c, d) {
            var s = 1.70158;
            var p = 0;
            var a = c;
            if (t == 0) return b;
            if ((t /= d) == 1) return b + c;
            if (!p) p = d * .3;
            if (a < Math.abs(c)) {
                a = c;
                var s = p / 4;
            } else var s = p / (2 * Math.PI) * Math.asin(c / a);
            return a * Math.pow(2, -10 * t) * Math.sin((t * d - s) * (2 * Math.PI) / p) + c + b
        },
        easeInOutElastic: function(x, t, b, c, d) {
            var s = 1.70158;
            var p = 0;
            var a = c;
            if (t == 0) return b;
            if ((t /= d / 2) == 2) return b + c;
            if (!p) p = d * (.3 * 1.5);
            if (a < Math.abs(c)) {
                a = c;
                var s = p / 4;
            } else var s = p / (2 * Math.PI) * Math.asin(c / a);
            if (t < 1) return -.5 * (a * Math.pow(2, 10 * (t -= 1)) * Math.sin((t * d - s) * (2 * Math.PI) / p)) + b;
            return a * Math.pow(2, -10 * (t -= 1)) * Math.sin((t * d - s) * (2 * Math.PI) / p) * .5 + c + b
        },
        easeInBack: function(x, t, b, c, d, s) {
            if (s == undefined) s = 1.70158;
            return c * (t /= d) * t * ((s + 1) * t - s) + b
        },
        easeOutBack: function(x, t, b, c, d, s) {
            if (s == undefined) s = 1.70158;
            return c * ((t = t / d - 1) * t * ((s + 1) * t + s) + 1) + b
        },
        easeInOutBack: function(x, t, b, c, d, s) {
            if (s == undefined) s = 1.70158;
            if ((t /= d / 2) < 1) return c / 2 * (t * t * (((s *= (1.525)) + 1) * t - s)) + b;
            return c / 2 * ((t -= 2) * t * (((s *= (1.525)) + 1) * t + s) + 2) + b
        },
        easeInBounce: function(x, t, b, c, d) {
            return c - jQuery.easing.easeOutBounce(x, d - t, 0, c, d) + b
        },
        easeOutBounce: function(x, t, b, c, d) {
            if ((t /= d) < (1 / 2.75)) {
                return c * (7.5625 * t * t) + b;
            } else if (t < (2 / 2.75)) {
                return c * (7.5625 * (t -= (1.5 / 2.75)) * t + .75) + b;
            } else if (t < (2.5 / 2.75)) {
                return c * (7.5625 * (t -= (2.25 / 2.75)) * t + .9375) + b;
            } else {
                return c * (7.5625 * (t -= (2.625 / 2.75)) * t + .984375) + b;
            }
        },
        easeInOutBounce: function(x, t, b, c, d) {
            if (t < d / 2) return jQuery.easing.easeInBounce(x, t * 2, 0, c, d) * .5 + b;
            return jQuery.easing.easeOutBounce(x, t * 2 - d, 0, c, d) * .5 + c * .5 + b;
        }
    });
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
        input.bind('focus', function() {
            self.focus(inputId);
        });

        var panel = $('#' + panelId);
        panel.bind('mouseover', function() {
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

    autoComplete.prototype.input = function(oEvent) {
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
            } else {
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
        if (typeof(dataSource) == 'string') { //从Url地址加载

            if (ac["triggerTimer"][inputId] != null) {
                clearTimeout(ac["triggerTimer"][inputId]);
            }

            ac["triggerTimer"][inputId] = setTimeout(function() {
                var postData = {};
                postData[fieldName] = $.trim(input.val());
                if(input.val()!=""){
                $.ajax({
                    url: dataSource,
                    data: postData,
                    type: "GET",
                    dataType: "json",
                    cache: false,
                    success: function(jsonData) {
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
                            panel.css({
                                "position": "absolute",
                                "top": fixTop,
                                "left": fixLeft,
                                "width": fixWidth + 'px'
                            });

                            var tmpl = $('#' + templateId).html();
                            var html = juicer(tmpl, {
                                "ListData": jsonData,
                                "Keywords": kw
                            });
                            panel.html(html);
                        } else {
                            if (panel.is(":visible")) {
                                panel.hide();
                            }
                        }
                    },
                    error: function(xmlHttpRequest, errorMessage, ex) {}
                });
                }
                

            }, 300);
        } else { //指定Json数据源

        }

    };
    autoComplete.prototype.blur = function(oEvent) {
        var oEvent = getEvent();
        var oElem = getEventObj(oEvent);
        var input = $(oElem);

        var inputId = input.attr("id");
        var ac = window.lib51.autoCompletes[inputId];
        var panelId = ac["panelId"];

        ac["closeTimer"][inputId] = setTimeout(function() {
            var panel = $('#' + panelId);
            panel.hide();
        }, 300);
    };
    autoComplete.prototype.focus = function(inputId) {
        var ac = window.lib51.autoCompletes[inputId];
        if (ac["lastInputKeywords"] != null) {
            $('#' + ac["panelId"]).show();
        }
    };
    autoComplete.prototype.hoverPanel = function(inputId) {
        var ac = window.lib51.autoCompletes[inputId];
        if (ac["closeTimer"][inputId] != null) {
            clearTimeout(ac["closeTimer"][inputId]);
        }
    };
    //-------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------
    function lib51() {}

    lib51.prototype = {
        version: 0.1,
        build: 20150612,

        /*
        合并json到目标json中
        @newJson[Json] 新json
        @targetJson[Json] 源json
        */
        mergeJson: function(newJson, targetJson) {
            return mergeJson(newJson, targetJson);
        },
        /*
        模拟c# String.Format
        @str[String] 要格式化的字符串
        @args[Array] 格式化参数数组
        */
        stringFormat: function(str, args) {
            return str.format(args);
        },

        /*
        文件大小友好格式字符串
        @size[Number] 大小，字节
        @format[String] 可选，输出格式，{0}代表大小数字，{1}代表单位
        */
        formatSize: function(size, format) {
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
                do {
                    handleSize = handleSize / 1024.00;
                    indx++;
                } while (handleSize >= 1024 && indx < unit.Length - 1);

                n = handleSize.toFixed(1);
                unit = units[indx];
            } else {
                if (size <= 0) {
                    n = 0;
                    unit = 'K';
                } else {
                    n = size;
                    if (size > 1) {
                        unit = 'Bytes';
                    } else {
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
        getTimestamp: function(dt) {
            dt = dt || new Date();
            var timestamp = Date.parse(dt);
            return timestamp / 1000;
        },
        /*
        将指定时间戳(毫秒)转为时间对象
        @timestamp[Number] 时间戳
        */
        getDateTime: function(timestamp) {
            if (typeof(timestamp) == 'string') {
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
        getDateTimeString: function(dt, formatStr) {
            if (typeof(dt) == 'undefined') {
                return '';
            }
            if (!isNaN(dt)) {
                dt = window.lib51.getDateTime(dt);
            } else if (typeof(dt) == 'string') {
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
        getQueryString: function(name) {
            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
            var r = window.location.search.substr(1).match(reg);
            if (r != null && typeof(r) != 'undefined') {
                return unescape(r[2]);
            }
            return null;
        },

        /*
        获取事件源
        */
        getEvent: function() {
            return getEvent();
        },
        /*
        获取事件源对象
        */
        getEventObj: function(oEvent) {
            return getEventObj(oEvent);
        },
        /*
        判断变量是否不为undefined与null
        */
        isSet: function(v) {
            return isSet(v);
        },
        /*
        判断变量是否为function
        */
        isFunc: function(v) {
            return isFunc(v);
        },

        /*
        url地址添加新queryString项　
        @urlInput[string] url
        @newParamKey[string]
        @newParamValue[string]
        */
        resolveUrl: function(urlInput, newParamKey, newParamValue) {
            if (newParamValue.length < 1) {
                if (urlInput.indexOf(newParamKey + "=") < 0) {
                    return urlInput;
                }
            }
            //判断指定的网址本身是否带参数
            if (urlInput.indexOf("?") < 0) {
                return urlInput + "?" + newParamKey + "=" + newParamValue;
            } else {
                //判断指定的网址是否包含指定的参数
                if (urlInput.indexOf(newParamKey + "=") < 0) {
                    return urlInput + "&" + newParamKey + "=" + newParamValue;
                } else {
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
                    } else {
                        paramString = pathString + paramString1 + newParamKey + "=" + newParamValue;
                    }

                    return paramString;
                }
            }
        },
        /*
        补全不够长度的数字字符串，如将110填充为0000110
        */
        fillText: function(text, len, fillStr) {
            var textStr = text + '';
            if (textStr.length < len) {
                var l = len - textStr.length;
                for (var i = 0; i < l; i++) {
                    textStr = fillStr + textStr;
                }
            }
            return textStr;
        },

        /*
        获取表单数据，返回json-key:表单项name, value:表单项值
        @formId[string] 表单id
        */
        getFormData: function(formId) {
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
                        chks.each(function() {
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
                        rdos.each(function() {
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
        setFormData: function(formId, formData) {
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
        ajaxQuery: function(url, data, options) {
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
                success: function(jsonData) {
                    if (isFunc(successFunc)) {
                        successFunc(jsonData);
                    }

                    if (isFunc(completeFunc)) {
                        completeFunc(jsonData);
                    }

                },
                error: function(xmlHttpRequest, errorMessage, ex) {
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
        postForm: function(formId, buttonId, success, begin, complete, appendData) {
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
            options["begin"] = function(url, data, options) {
                if (isSet(buttonId)) {
                    $('#' + buttonId).attr('disabled', true);
                }
                begin(url, data, options);
            };
            options["complete"] = function(jsonData) {
                if (isSet(buttonId)) {
                    $('#' + buttonId).attr('disabled', false);
                }
                complete(jsonData);
            };
            options["error"] = function(xmlHttpRequest, errorMessage, ex) {
                alert(errorMessage);
            };

            window.lib51.ajaxQuery(url, data, options);
        },

        /*
        某些浏览器下修正不支持placeholder的属性
        */
        fixPlaceHolder: function() {
            if ($.browser.msie) {
                var ieVersion = parseFloat($.browser.version);
                if (ieVersion < 10.0) {
                    $('input[type=text], textarea').each(function() {
                        var input = $(this);
                        var ph = input.attr('placeholder');
                        if (ph) {
                            input.val(ph);
                            input.bind('focus', function() {
                                var thisInput = $(this);
                                var thisPH = thisInput.attr('placeholder');
                                var inputValue = thisInput.val();
                                if (inputValue == ph) {
                                    input.val('');
                                }
                            });
                            input.bind('blur', function() {
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
        scrollToPosition: function(targetId, ms, callback) {
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
        setCookie: function(key, value, options) {
            $.cookie(key, value, options);
        },
        /*
        获取cookie值
        */
        getCookie: function(key) {
            return $.cookie(key);
        },
        /*
        移除cookie
        */
        removeCookie: function(key, options) {
            $.cookie(key, null, options);
        },

        /*
        给指定文本里的指定关键字加上标签与样式
        @text[String] 指定文本
        @keywords[String] 指定关键字
        @tag[String] 打上的标签
        @css[String] 标签使用的样式
        */
        tagKeywords: function(text, keywords, tag, css) {
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
        _fixObjectPosition: function(objId, options) {
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

                        if (typeof(top) != 'undefined') {
                            obj.css({
                                "top": (top + scrollTop) + 'px'
                            });
                        }
                        if (typeof(left) != 'undefined') {
                            obj.css({
                                "left": (left + scrollLeft) + 'px'
                            });
                        }
                        if (typeof(bottom) != 'undefined') {
                            top = winH - bottom - objH;
                            obj.css({
                                "top": (top + scrollTop) + 'px'
                            });
                        }
                        if (typeof(right) != 'undefined') {
                            left = winW - right - objW;
                            obj.css({
                                "left": (left + scrollLeft) + 'px'
                            });
                        }
                    } else { //非IE6支持fixed定位，可直接使用CSS控制
                        obj.css({
                            "position": "fixed"
                        });
                        if (typeof(top) != 'undefined') {
                            obj.css({
                                "top": (top) + 'px'
                            });
                        }
                        if (typeof(left) != 'undefined') {
                            obj.css({
                                "left": (left) + 'px'
                            });
                        }
                        if (typeof(bottom) != 'undefined') {
                            obj.css({
                                "bottom": (bottom) + 'px'
                            });
                        }
                        if (typeof(right) != 'undefined') {
                            obj.css({
                                "right": (right) + 'px'
                            });
                        }
                    }
                }
            } catch (ex) {

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
        fixObjectPosition: function(objId, options) {
            var top = options["top"];
            var left = options["left"];
            var bottom = options["bottom"];
            var right = options["right"];
            var initPosition = options["initPosition"] || false;
            var initPositionTop = parseFloat(initPosition);
            var showPosition = options["showPosition"] || null;

            if (typeof(this.fixedObjects[objId]) == 'undefined') {
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
                    } else {
                        obj.hide();
                    }
                }

                if (false == initPosition || isNaN(initPositionTop) || (false == isNaN(initPositionTop) && ((initPositionTop >= 0 && scrollTop >= orgTop) || (initPositionTop < 0 && (scrollTop + winHeight) <= orgTop)))) {
                    this._fixObjectPosition(objId, options);
                }

                $(window).bind("scroll resize", function() {
                    var winHeight = $(window).height();
                    var scrollTop = $(document).scrollTop();
                    var scrollLeft = $(document).scrollLeft();

                    if (showPosition != null) {
                        if (scrollTop >= showPosition["top"] && scrollLeft >= showPosition["left"]) {
                            obj.show();
                        } else {
                            obj.hide();
                        }
                    }

                    if (false == initPosition || isNaN(initPositionTop) || (false == isNaN(initPositionTop) && ((initPositionTop >= 0 && scrollTop >= orgTop) || (initPositionTop < 0 && (scrollTop + winHeight) <= orgTop)))) {
                        window.lib51._fixObjectPosition(objId, options);
                    } else {
                        obj.css({
                            "position": position
                        });
                    }
                });

                this.fixedObjects[objId] = {
                    "offset": offset,
                    "position": position,
                    "options": options
                };
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
                    } else {
                        obj.css({
                            "position": "fixed",
                            "top": (winH / 2 - objH / 2) + 'px',
                            "left": (winW / 2 - objW / 2) + 'px'
                        });
                    }
                }
            } catch (ex) {

            }
        },
        /*
        使指定的元素静止定位于中间位置，兼容所有浏览器
        @objId[string] 元素id
        */
        fixObjectMiddlePosition: function(objId) {
            if (typeof(this.fixedObjects[objId]) == 'undefined') {
                this._fixObjectMiddlePosition(objId);
                $(window).bind("scroll resize", function() {
                    window.lib51._fixObjectMiddlePosition(objId);
                });

                this.fixedObjects[objId] = $('#' + objId);
            }
        },

        panelSwitches: {},
        /*
        点击菜单切换显示隐藏相应的容器
        */
        panelSwitch: function(panels, options, key) {
            var menuPanel = new panelSwitch(panels, options);
            if (typeof(key) != 'undefined') {
                this.panelSwitches[key] = popBox;
            }
            return menuPanel;
        },

        popupBoxes: {},
        /*
        移上移出菜单显示隐藏相应的容器
        */
        popupBox: function(menuList, options, key) {
            var popBox = new popupBox(menuList, options);
            if (typeof(key) != 'undefined') {
                this.popupBoxes[key] = popBox;
            }
            return popBox;
        },

        sliders: {},
        /*
        幻灯片播放
        */
        slider: function(id, containerSelector, itemSelector, menuSelector, options) {
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
        scrollStatusBar: function(barId, panelSelector, options) {
            window.lib51.fixObjectPosition(barId, {
                "top": 0,
                "initPosition": true
            });
            //TODO...
        },

        autoCompletes: {},
        /*
        自动完成
        */
        autoComplete: function(inputId, dataSource, panelId, templateId, options) {
            var auto = new autoComplete(inputId, dataSource, panelId, templateId, options);
            this.autoCompletes[inputId] = auto;
            return auto;
        }
    };

    window.lib51 = new lib51();
    //-------------------------------------------------------------------------------------------

})(window);


//juicer组件注册
if (typeof(juicer) != 'undefined') {
    $(function() {
        //自定义juicer的操作符避免与Razor语法的冲突
        juicer.set({
            'tag::operationOpen': '{$',
            'tag::operationClose': '}',
            'tag::interpolateOpen': '${',
            'tag::interpolateClose': '}',
            'tag::noneencodeOpen': '$${',
            'tag::noneencodeClose': '}',
            'tag::commentOpen': '{#',
            'tag::commentClose': '}'
        });
    });
    // var JSON={};
    juicer.register('JsonToString', JSON.stringify);
    juicer.register('StringToJson', $.parseJSON);
    juicer.register('TagKeywords', lib51.tagKeywords);
    juicer.register('GetDateTime', lib51.getDateTime);
    juicer.register('GetDateTimeString', lib51.getDateTimeString);
    juicer.register('IsWebImage', IsWebImage);
}

/*
确认操作框
*/
function ConfirmAction(message, action, okStr, cancelStr) {
    okStr = okStr || '确定';
    cancelStr = cancelStr || '取消';

    layer.confirm(message, {
        btn: [okStr, cancelStr]
    }, function(index) {
        action();
        layer.close(index);
        return true;
    }, function(index) {
        layer.close(index);
        return true;
    });
}

/*
显示加载条
*/
function ShowLoading() {
    layer.load(0, {
        shade: false
    });
}
/*
隐藏加载条
*/
function CloseLoading() {
    var loading = $('.layui-layer-loading');
    loading.remove();
}

/*
显示成功提示
*/
function ShowSuccess(message, callback) {
    CloseLoading();
    if (typeof(callback) == 'function') {
        layer.alert(message, {
            icon: 6,
            title: '提示消息',
            shadeClose:true,
            yes: function(index) {
                layer.close(index);
            },
            end: function() {
                callback();
            }
        });
    } else {
        layer.alert(message, {
            icon: 6,
            title: '提示消息',
            shadeClose:true,
            yes: function(index) {
                layer.close(index);
            }
        });
    }
}
/*
显示失败提示
*/
function ShowAlert(message,okstr, callback) {
    CloseLoading();
    okstr = okstr||"确定";
    if (typeof(callback) == 'function') {
        layer.alert(message, {
            icon: 5,
            title: '提示消息',
            btn:okstr,
            shadeClose:true,
            yes: function(index) {
                layer.close(index);
                callback();
            }
        });
    } else {
        layer.alert(message, {
            icon: 5,
            title: '提示消息',
            btn:okstr,
            shadeClose:true,
            yes: function(index) {
                layer.close(index);
            }
        });
    }
}


/*
jQuery ajax GET请求封装，带加载提示等
@url[string] 请求地址
@data[json] 发送的数据
@callback[function] 请求成功后的回调方法
@errorCallback[function] 请求出错的回调方法。
*/
function AjaxQuery(url, type, data, callback, errorCallback) {
    if (!lib51.isFunc(callback)) {
        callback = function(jsonData) {
            var result = parseInt(jsonData['Result']);
            var message = jsonData['Message'];
            if (result > 0) {
                if (lib51.isSet(message)) {
                    ShowSuccess(message);
                }
            } else {
                if (lib51.isSet(message)) {
                    ShowAlert(message);
                }
            }
        }
    }
    if (!lib51.isFunc(errorCallback)) {
        errorCallback = function(jsonData) {
            var result = parseInt(jsonData['Result']);
            var message = jsonData['Message'];
            if (result > 0) {
                if (lib51.isSet(message)) {
                    ShowSuccess(message);
                }
            } else {
                if (lib51.isSet(message)) {
                    ShowAlert(message);
                }
            }
        }
    }

    var options = {};
    options["type"] = type;
    options["success"] = callback;
    options["error"] = errorCallback;
    options["begin"] = function() {
        ShowLoading();
    };
    options["complete"] = function() {
        CloseLoading();
    };

    lib51.ajaxQuery(url, data, options);
}
/*
jQuery ajax GET请求封装，带加载提示等
@url[string] 请求地址
@data[json] 发送的数据
@callback[function] 请求成功后的回调方法
@errorCallback[function] 请求出错的回调方法。[可选]
*/
function AjaxGet(url, data, callback, errorCallback) {
    AjaxQuery(url, "GET", data, callback, errorCallback);
}
/*
jQuery ajax POST请求封装，带加载提示等
@url[string] 请求地址
@data[json] 发送的数据
@callback[function] 请求成功后的回调方法
@errorCallback[function] 请求出错的回调方法。[可选]
*/
function AjaxPost(url, data, callback, errorCallback) {
    AjaxQuery(url, "POST", data, callback, errorCallback);
}

/*
POST提交表单数据
@formId[string] 表单id
@buttonId[string] 表单提交按钮id
@callback[function]
*/
function PostFormData(formId, buttonId, callback, extraData) {
    if (!lib51.isFunc(callback)) {
        callback = function(jsonData) {
            var result = parseInt(jsonData['Result']);
            var message = jsonData['Message'];
            if (result > 0) {
                if (lib51.isSet(message)) {
                    ShowSuccess(message);
                }
            } else {
                if (lib51.isSet(message)) {
                    ShowAlert(message);
                }
            }
        }
    }
    lib51.postForm(formId, buttonId, callback, function() {
        ShowLoading();
    }, function() {
        CloseLoading();
    }, extraData);
}

/*
是否图片
*/
function IsWebImage(fileName) {
    var dotPos = fileName.lastIndexOf('.');
    if (dotPos > 0) {
        var ext = fileName.substr(dotPos);
        ext = ext.toLowerCase();
        if (
            ext == '.jpg' ||
            ext == '.png' ||
            ext == '.gif' ||
            ext == '.bmp' ||
            ext == '.ico' ||
            ext == '.jpeg'
        ) {
            return true;
        }
        return false;
    } else {
        return false;
    }
}


/*判断是否绑定手机和邮箱的弹窗*/
function alertMsgBox(url) {
    $("body").append("<div class='alertMarks'></div>");
    $(".alertMarks").append("<div class='msgBg'></div>");
    $(".alertMarks").append("<div class='msgMain'></div>");
    $(".msgMain").append("<img src='../../Content/images/immediateBind.jpg'>");
    $(".msgMain").append("<p>绑定手机或电子邮箱，更方便账户登录！</p>");
    $(".msgMain").append("<a class='immediateBind' href='/my/account'>马上绑定</a>");
    $(".msgMain").append("<a class='closeMsgBox' data-url='" + url + "' href='javascript:void(0)'>下次再说</a>");
    $(".closeMsgBox").click(function () {
        $(".alertMarks").hide();
        var eduUrl = $('#hdHomeUrl').val(),
            returnUrl = $('#hdReturnUrl').val(),
            qianhaiUrl = $(this).data("url");
        if (qianhaiUrl.length > 0) {
            location.href = qianhaiUrl;
        } else if (returnUrl.length > 0) {
            location.href = returnUrl;
        } else {
            location.href = eduUrl;
        }
    });
}

//退出系统
function Logout() {
    var url = '/Account/QueryLogout';
    AjaxGet(url, null, function (jsonData) {
        var result = parseInt(jsonData["Result"]),
            message = jsonData["Message"],
            url = jsonData.Data.ReturnUrl;
        ShowSuccess('成功退出登录', function () {
            if (url.length > 0) {
                location.href = url;
            }
        });
    });
}