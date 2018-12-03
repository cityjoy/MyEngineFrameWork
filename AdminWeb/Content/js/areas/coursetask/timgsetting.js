$(function () {
    var tablesetting = {
        tablebuild: {
            day: ["星期一", "星期二", "星期三", "星期四", "星期五", "星期六", "星期日"],
            cellbuild: function (obj, val) {
                this.daybuild(tableParameter.day);
                this.classbuild(tableParameter.morning, "tablemorning", "morning", "上午");
                this.classbuild(tableParameter.afternoon, "tableafternoon", "afternoon", "下午");
                this.classbuild(tableParameter.evening, "tableevening", "evening", "晚上");
                this.serialnumberbuild();
            },
            daybuild: function (VAL) {
                var theadHtml = "<th colspan='2'>节次</th>",
                    tbodyHtml = "";
                if (VAL) {
                    for (var i = 0; i < VAL; i++) {
                        theadHtml += "<th>" + this.day[i] + "</th>";
                        tbodyHtml += "<td></td>";
                    }
                    $(".table-class tbody tr").each(function () {
                        $(this).find("td").remove();
                        $(this).append(tbodyHtml);
                    });
                }
                $(".table-class thead tr").html(theadHtml);
            },
            classbuild: function (VAL, HIDDENINPUT, CLASSNAME, TIME) {
                var atext = $(".table-class tbody tr th").index(),
                    currentNumber = $("[name=" + HIDDENINPUT + "]").val(),
                    arr = [],
                    i = "",
                    tbodyhtml = "<tr class='text-c table-" + CLASSNAME + "'>",
                    aa = "";
                $(".table-class tbody tr th[rowspan]").each(function () {
                    arr.push($(".table-class tbody tr th").index(this));
                    if ($(this).text() == TIME) {
                        i = $(".table-class tbody tr th").index(this);
                    }
                });
                $(".table-" + CLASSNAME).remove();
                if (VAL > 0) {
                    for (var i = 0; i <= VAL; i++) {
                        if (i == 0) {
                            tbodyhtml += "<th rowspan=" + VAL + ">" + TIME + "</th>";
                        } else if (i == 1) {
                            tbodyhtml += "<th></th></tr>";
                        } else {
                            tbodyhtml += "<tr class='text-c table-" + CLASSNAME + "'><th></th></tr>";
                        }
                    }
                    switch (TIME) {
                        case "上午":
                            $(".table-class").prepend(tbodyhtml);
                            break;
                        case "下午":
                            if ($(".table-morning").length > 0) {
                                $(".table-morning:last").after(tbodyhtml);
                            } else if ($(".table-evening").length > 0) {
                                $(".table-evening:first").before(tbodyhtml);
                            } else {
                                $(".table-class").prepend(tbodyhtml);
                            }
                            break;
                        case "晚上":
                            $(".table-class").append(tbodyhtml);
                            break;
                    }
                    this.daybuild(tableParameter.day);
                }
                $("[name=" + HIDDENINPUT + "]").val(VAL);
            },
            serialnumberbuild: function () {
                var len = $(".table-class tbody th:not([rowspan])").length;
                for(var i=0;i<len;i++){
                    $(".table-class tbody th:not([rowspan])").eq(i).text("第"+parseInt(i+1)+"节");
                }
            }
        },
        colorfill: function () {
            var classname = "";
            this.tablebuild.cellbuild();
            _.forEach(arrTimg, function (value, key) {
                switch (value.type) {
                    case 1:
                        classname = "text-c success";
                        break;
                    case 2:
                        classname = "text-c danger";
                        break;
                }
                _.forEach(value.rule, function (value1, key1) {
                    $(".table-class tbody tr:nth-child(" + value1[1] + ")").find("td").eq(value1[0] - 1).addClass(classname);
                });
            });
        }
    };
    tablesetting.colorfill();
});