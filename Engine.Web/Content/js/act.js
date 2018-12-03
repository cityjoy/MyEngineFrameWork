var globalOpt = {
  headerNav: function() {
    $('.header-nav li').hover(function() {
      $(this).find('dl').show();
    }, function() {
      $(this).find('dl').hide();
    });
  },
  headerUser: function() {
    $('.header-user').hover(function() {
      $(this).find('ul').show();
    }, function() {
      $(this).find('ul').hide();
    });
  },
  sidebar: function() {
    $('.sidebar').hover(function() {
      $(this).addClass('show');
    }, function() {
      $(this).removeClass('show');
    });
  },
  loginMsg: function() {
    if ($('.header-login').length) {
      $('#login').click(function(event) {
        $('.msg-login').show();
      });
      $('.close').click(function(event) {
        $('.msg').hide();
      });
    }
  },
  doOpt: function() {
    this.headerNav();
    this.headerUser();
    this.sidebar();
    this.loginMsg();
  }
};
var loginBox = {
  tab: function() {
    $('.index-login-tab li').click(function() {
      var i = $(this).index();
      $('.index-login-tab li').removeClass('on');
      $(this).addClass('on');
      $('.index-login-item').hide();
      $('.index-login-item').eq(i).show();
    });
  },
  placeholder: function() {
    $('.index-login-con span').click(function() {
      $(this).siblings('input').focus();
    });
    $('.index-login-con input').keyup(function(event) {
      if ($(this).val() == '') {
        $(this).siblings('span').show();
      } else {
        $(this).siblings('span').hide();
      }
    });
  },
  doOpt: function() {
    if ($('.index-login').length) {
      this.tab();
      this.placeholder();
    }
  }
};

var DatumDetail = {
  tip: function() {
    $('body').append('<em class="done" style="display: none;">-1</em><em class="do" style="display: none;">+1</em>');
  },
  collectOpt: function() {
    if ($('.datum-detail-collect').length) {
      $('.datum-detail-collect').click(function() {
        if ($(this).hasClass('on')) {
          $(this).removeClass('on');
        } else {
          $(this).addClass('on');
        }
      });
    }
  },
  likeOpt: function() {
    if ($('.datum-detail-like').length) {
      var x = 10,
        y = -15;
      this.tip();
      $('.datum-detail-like').click(function(e) {
        var sum = parseInt($(this).find('em i').text());
        if ($(this).hasClass('on')) {
          $(this).removeClass('on');
          sum = sum - 1;
          $(this).find('em i').text(sum).end();
          $('.done').css({
            "top": (e.pageY + y) + "px",
            "left": (e.pageX + x) + "px"
          }).fadeIn("fast").animate({
            marginTop: "-15px"
          }, 500).fadeOut();
          $('.done').css("marginTop", 0);
          return false;
        } else {
          $(this).addClass('on');
          sum = sum + 1;
          $(this).find('em i').text(sum).end();
          $('.do').css({
            "top": (e.pageY + y) + "px",
            "left": (e.pageX + x) + "px"
          }).fadeIn("fast").animate({
            marginTop: "-15px"
          }, 500).fadeOut();
          $('.do').css("marginTop", 0);
        }
      });
    }
  },
  doOpt: function() {
    if ($('.datum-detail').length) {
      this.collectOpt();
      this.likeOpt();
    }
  }
};

var specialtyChart = {
  sData: function() {
    var arr = new Array();
    $('.col-l-article-table tr').each(function() {
      if ($(this).has('td') && $(this).index() != 0) {
        var arr1 = new Array($(this).find('.table-col-1').text(), parseInt($(this).find('.table-col-3').text().split("%")[0]));
        arr.push(arr1);
      }
    });
    return arr;
  },
  sChart: function() {
    $('#container').highcharts({
      chart: {
        plotBackgroundColor: null,
        plotBorderWidth: null,
        plotShadow: false
      },
      colors: ['#ffcb3e', '#ff8b3e', '#42bdeb', '#8ed740', '#24CBE5', '#64E572', '#FF9655', '#FFF263', '#6AF9C4'],
      title: {
        text: ''
      },
      tooltip: {
        pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
      },
      plotOptions: {
        pie: {
          innerSize: 100,
          allowPointSelect: true,
          cursor: 'pointer',
          dataLabels: {
            enabled: false
          },
          showInLegend: true
        }
      },
      series: [{
        type: 'pie',
        name: '比例',
        data: this.sData()
      }]
    });
  },
  sHover: function() {
    $('.col-l-article-item span').hover(function() {
      $(this).find('.col-l-article-tip').show();
    }, function() {
      $(this).find('.col-l-article-tip').hide();
    });
  },
  doOpt: function() {
    if ($('#specialtyDetail').length) {
      this.sChart();
      this.sHover();
    }
  }
};

var intelligenceCharts = {
  iCharts: function(IntelligenceData) {
    $('#container').highcharts({
      chart: {
        type: 'column'
      },
      colors: ['#6bcfff', '#bce9ff', '#6bcfff', '#bce9ff', '#6bcfff', '#bce9ff', '#6bcfff', '#bce9ff'],
      title: {
        text: ''
      },
      subtitle: {
        text: ''
      },
      xAxis: {
        type: 'category',
        labels: {
          rotation: 0,
          style: {
            fontSize: '12px',
            fontFamily: 'microsoft yahei'
          }
        },
        tickWidth: 0,
        gridLines: 0
      },
      yAxis: {
        min: 0,
        title: {
          text: ''
        },
        labels: {
          enabled: false
        },
        gridLineWidth: 0
      },
      legend: {
        enabled: false
      },
      tooltip: {
        pointFormat: ''
      },
      plotOptions: {
        column: {
          colorByPoint: true
        }
      },
      series: [{
        name: 'Population',
        data: IntelligenceData,
        dataLabels: {
          enabled: true,
          rotation: 0,
          color: '#FFFFFF',
          align: 'center',
          format: '{point.y:.1f}', // one decimal
          y: 30, // 10 pixels down from the top
          style: {
            fontSize: '14px',
            fontFamily: 'microsoft yahei'
          }
        }
      }]
    });
  },
  doOpt: function(IntelligenceData) {
    if ($('#intelligence').length) {
      this.iCharts(IntelligenceData);
    }
  }
};

var careerOrientationCharts = {
  cCharts: function(CareerOrientationData) {
    $('#container').highcharts({

      chart: {
        polar: true,
        type: 'line'
      },

      title: {
        text: ''
      },

      pane: {
        size: '80%'
      },

      xAxis: {
        categories: ['艺术型(A)', '社会型(S)', '企业型(E)', '常规型(C)', '实际型(R)', '调研型(I)'],
        tickmarkPlacement: 'on',
        lineWidth: 0,
        style: {
          fontSize: '12px',
          fontFamily: 'microsoft yahei'
        }
      },

      yAxis: {
        gridLineInterpolation: 'polygon',
        lineWidth: 0,
        min: 0
      },

      tooltip: {
        shared: true,
        pointFormat: '<span style="color:{series.color}">{series.name}: <b>{point.y:,.0f}</b><br/>'
      },

      legend: {
        enabled: false,
        align: 'right',
        verticalAlign: 'top',
        y: 70,
        layout: 'vertical'
      },

      series: [{
        name: ' ',
        data: CareerOrientationData,
        pointPlacement: 'on'
      }]

    });
  },
  doOpt: function(CareerOrientationData) {
    if ($('#careerOrientation').length) {
      this.cCharts(CareerOrientationData);
    }
  }
};

var accountMsgShow = {
  show: function() {
    var c_id = "";
    $('.user-btn').click(function(event) {
      c_id = $(this).attr('id');
      $('.msg-' + c_id).show();
    });
  },
  hide: function() {
    $('.close').click(function(event) {
      $('.msg').hide();
    });
  },
  doOpt: function() {
    if ($('#account').length) {
      this.show();
      this.hide();
    }
  }
};


var jobDetail = {
  jHover: function() {
    $('.col-l-article-item span').hover(function() {
      $(this).find('.col-l-article-tip').show();
    }, function() {
      $(this).find('.col-l-article-tip').hide();
    });
  },
  doOpt: function() {
    if ($('#jobDetail').length) {
      this.jHover();
    }
  }
};

var doGood = {
  htmlString: function() {
    var html = '<div class="tooltip doGood"><ul><li><a class="like" href="javascript:void(0)"><div></div><span>1</span></a></li><li><a class="like" href="javascript:void(0)"><div></div><span>2</span></a></li><li><a class="like" href="javascript:void(0)"><div></div><span>3</span></a></li><li><a  class="like" href="javascript:void(0)"><div></div><span>4</span></a></li><li><a class="like" href="javascript:void(0)"><div></div><span>5</span></a></li><li><a class="unlike" href="javascript:void(0)"><div></div><span>1</span></a></li><li><a class="unlike" href="javascript:void(0)"><div></div><span>2</span></a></li><li><a class="unlike" href="javascript:void(0)"><div></div><span>3</span></a></li><li><a  class="unlike" href="javascript:void(0)"><div></div><span>4</span></a></li><li><a class="unlike" href="javascript:void(0)"><div></div><span>5</span></a></li></ul></div>';
    $('body').append(html);
  },
  dScore : function(sum,i){
    return i + sum;
  },
  dJSON : function(){
    var ele , i = 0,_this = this,param = {};
    $('.selected').each(function(index, el) {
      $(this).hasClass('like')? i = _this.dScore($(this).find('span').text(),''): i = _this.dScore($(this).find('span').text(),'-');
      ele = $(this).parents('li').index();
      param[ele]=i;
    });
    JSON.stringify(param);
    $('input[name="data"]').val(JSON.stringify(param));
  },
  dHover: function() {
    $('.radio-dd').mouseover(function() {
      var dTop = $(this).offset().top,
        dLeft = $(this).offset().left,
        i = $('.radio-dd').index(this);
      $('.doGood').removeClass('left');
      $('.doGood').show().css({
        top: dTop - $('.doGood').height() + 5,
        left: dLeft - $('.doGood').width() + $('.radio-dd').width()
      });
      if ($('.careervaluestestB').length && (i + 3) % 3 == 0) {
        $('.doGood').addClass('left').css({
          left: dLeft
        });
      }
      if ($('.careervaluestestA').length) {
        $('.doGood').css({
          left: dLeft - $('.doGood').width() + $('.radio-dd').width() + 8
        });
      }
      $('.doGood').data('id', i);
    });
    $('.doGood').mouseover(function() {
      $('.doGood').show();
    });
    $('.radio-dl,.doGood').mouseleave(function(event) {
      $('.doGood').hide();
    });
  },
  dClick: function() {
    var _this = this;
    $(document).on('click', '.doGood li', function(event) {
      event.preventDefault();
      if ($(this).hasClass('sel')) {
        return false;
      } else {
        var f_id = $('.doGood').data('id'),
          li_index = $(this).index();
        if ($('.radio-dd').eq(f_id).find('.selected').length) {
          return false;
        } else {
          li_index < 5 ?
            function() {
              $('.radio-dd').eq(f_id).find('.like').addClass('selected').find('span').text(li_index + 1);
            }() :
            function() {
              $('.radio-dd').eq(f_id).find('.unlike').addClass('selected').find('span').text(li_index - 4);
            }();
          $(this).addClass('sel');
          $('.radio-dd').eq(f_id).find('.selected').append('<i>×</i>');
        }
      }
      _this.dJSON();
    });
  },
  dRemove: function() {
    var _this = this;
    $(document).on('click', '.selected i', function(event) {
      event.preventDefault();
      var r_id = parseInt($(this).siblings('span').text());
      $(this).parent().hasClass('like') ? $('.doGood li').eq(r_id - 1).removeClass('sel') : $('.doGood li').eq(r_id + 4).removeClass('sel');
      $(this).parent().removeClass('selected');
      $(this).siblings('span').text('');
      $(this).remove();
      _this.dJSON();
    });
  },
  dReset: function() {
    $('.resetall').click(function(event) {
      $('.selected').each(function(index, el) {
        $(this).find('span').text('');
        $(this).find('i').remove();
        $(this).removeClass('selected');
      });
      $('.sel').each(function(index, el) {
        $(this).removeClass('sel');
      });
      $('input[name="data"]').val("");
    });
  },
  dSubmit: function() {
    $('.submitall').click(function(event) {
      if ($('.doGood .sel').length < 10) {
        alert('再看看，是不是漏了评分');
        return false;
      } else {
        // 提交表单数据
      }
    });
  },
  doOpt: function() {
    if ($('.careervaluestest').length) {
      this.htmlString();
      this.dHover();
      this.dClick();
      this.dRemove();
      this.dSubmit();
      this.dReset();
    }
  }
};

var IntelligenceTestA = {
  iData : function(obj){
    var attr = $(obj).find('.inline-title').text().split('(')[1].split(')')[0];
    return attr;
  },
  iSelect: function() {
    var _this = this,strS = '';
    $('.wrap-dl').click(function() {
      var len = $(".checkbox-listed").length;
      strS = '';
      if (len >= 3) {
        $(this).find("dd").removeClass("checkbox-listed");
        _this.iData(this);
      } else {
        $(this).find("dd").hasClass("checkbox-listed") ? $(this).find("dd").removeClass("checkbox-listed") : $(this).find("dd").addClass("checkbox-listed");
      }
      $('.wrap-dl').each(function(index, el) {
        if($(this).find("dd").hasClass("checkbox-listed")){
          strS += _this.iData(this) + ',';
        }
      });
      $('#hdIntelligenceCodes').val(strS);
    });
  },
  iSubmit:function(){
    $('input[type="submit"]').click(function(event) {
      if($('.checkbox-listed').length<3){
        alert('再看看，是不是漏了选择');
        return false;
      }else{
        // 提交表单数据
      }
    });
  },
  doOpt: function() {
    if ($('#IntelligenceTestA').length) {
      this.iSelect();
      this.iSubmit();
    }
  }
};

jQuery(function($) {
  globalOpt.doOpt();
  loginBox.doOpt();
  DatumDetail.doOpt();
  accountMsgShow.doOpt();
  jobDetail.doOpt();
  doGood.doOpt();
  IntelligenceTestA.doOpt();
});