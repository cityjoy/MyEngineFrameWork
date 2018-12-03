var courseGradeId = $("[name=courseGradeId]").val();
var assignmentTaskId = $("[name=assignmentTaskId]").val();
var tDay = tableParameter.day || 0;
var tMorning = tableParameter.morning || 0;
var tAfternoon = tableParameter.afternoon || 0;
var tEvening = tableParameter.evening || 0;


$(".success").click(function () {
    $(".success").removeClass("warning");
    $(this).addClass("warning");
});
$(".danger").click(function () {
    $(".danger").removeClass("warning");
    $(this).addClass("warning");
});

var timetablejustment = {
    modalScan: function () {
        var classTableStrA = $("[name=classTableStrA]").val(),
            classTableStrB = $("[name=classTableStrB]").val();
        if (classTableStrA == "" || classTableStrB == "") {
            ShowAlert("完成课表调整可预览");
        } else {
            var htmltemp = "",tdA=0,trA=0,tdB=0,trB=0;
            $(".adjusted").html($(".adjustedElement").html());
            $(".adjustment").html($(".adjustmentElement").html());

            tdA = $(".adjusted").find("td.success").index()+1;
            trA = $(".adjusted").find("td.success").parent().index()+1;
            tdB = $(".adjustment").find("td.warning").index()+1;
            trB = $(".adjustment").find("td.warning").parent().index()+1;

            $(".adjusted tbody").find("tr:nth-child(" + trB + ")").find("td:nth-child(" + tdB + ")").html($(".adjusted").find("td.success").html());
            $(".adjustment tbody").find("tr:nth-child(" + trA + ")").find("td:nth-child(" + tdA + ")").html($(".adjustment").find("td.warning").html());

            $(".adjusted").find("td.success").html("");
            $(".adjustment").find("td.warning").html("");
            $(".adjusted .success").removeClass("success");
            $(".adjustment .warning").removeClass("warning");
            $(".adjustment .danger").removeClass("danger");
            $(".adjusted .table-class").removeClass("table-class");
            $(".adjustment .table-class1").removeClass("table-class1");

            $("#modal-scan").modal("show");
        }
    },
    showTeacherList: function () {
        $("[name=class]").change(function () {
            if ($(this).val() != "") {
                AjaxGet("QueryGetAdministrativeClassTeachers", {
                    "assignmentTaskId": assignmentTaskId,
                    "courseGradeTaskId": courseGradeId,
                    "className": $(this).val()
                }, function (jsonData) {
                    $("[name=classTableStrA]").val("");
                    $("[name=classTableStrB]").val("");
                    $(".table td").removeClass("danger success warning");
                    $(".table tbody p").html("");
                    var result = parseInt(jsonData["Result"]),
                        message = jsonData["Message"];
                    if (result > 0) {
                        var html1 = juicer($("#tpl_option").html(), {
                            "title": "调课老师",
                            "flag": 0,
                            "list": jsonData.Data
                        });
                        var html2 = juicer($("#tpl_option").html(), {
                            "title": "协助调课老师",
                            "flag": 1,
                            "list": jsonData.Data
                        });
                        $("[name=teacherforchange]").html(html1);
                        $("[name=teacherforassist]").html(html2);
                    }
                });
            }
        });
    },
    showtimetable: function (select1, select2) {
        $(document).on("change", "[name=" + select1 + "]", function () {
            $("[name=" + select2 + "]").find("option").removeAttr("style");
            var _this = this,
                sId = $(_this).val();
            $("[name=" + select2 + "]").find("option[value=" + sId + "]").hide();
            $(_this).parents(".col-md-6").find("h4").text($(_this).find("option:selected").text());
            AjaxGet("QueryGetAdministrativClassTable", {
                "assignmentTaskId": assignmentTaskId,
                "courseGradeTaskId": courseGradeId,
                "teacherId": sId,
                "className": $("[name=class]").val()
            }, function (jsonData) {
                var ClassRoomName = jsonData.Data.CourseClassroom.ClassRoomName,
                    SubjectName = jsonData.Data.Subject.SubjectName,
                    tdHtml = SubjectName + "<br/>" + ClassRoomName;
                $(_this).parents(".col-md-6").find(".table tbody p").html("");
                $("[name=classTableStrA]").val("");
                $("[name=classTableStrB]").val("");
                $(".table td").removeClass("danger success warning");
                $(_this).parents(".col-md-6").find(".table td").attr("data-str", "");
                _.forEach(jsonData.Data.ClassTableList, function (value, key) {
                    var trList = 0,
                        tdList = 0,
                        str = value.ClassResourceSettingId + ',' + value.MorningOrNight + ',' + value.Weekdays + ',' + value.Section;
                    switch (value.MorningOrNight) {
                        case 1:
                            trList = 0;
                            break;
                        case 2:
                            trList = tMorning;
                            break;
                        case 3:
                            trList = tMorning + tAfternoon;
                            break;
                    }
                    trList += value.Section;
                    tdList = value.Weekdays - 1;
                    $(_this).parents(".col-md-6").find(".table tbody tr:nth-child(" + trList + ")").find("td").eq(tdList).attr("data-str", str);
                    $(_this).parents(".col-md-6").find(".table tbody tr:nth-child(" + trList + ")").find("td").eq(tdList).find("p").html(tdHtml);
                });
            });
        });
    },
    clickTableCell: function () {
        $(document).on("click", ".table-class td", function () {
            if ($(this).find("p").text().length == 0) {
                return false;
            } else {
                if ($("[name=teacherforassist]").val() == "") {
                    ShowAlert("请选择协助调课老师");
                    return false;
                }
                $(".table-class td").removeClass("success");
                $(this).addClass("success");
                $(".table-class1 td").removeClass("warning");
                $(".table-class1 td").removeClass("danger");
                $(".table-class1 td").each(function () {
                    if ($(this).find("p").text().length > 0) {
                        $(this).addClass("danger");
                    }
                });
                $("[name=classTableStrA]").val($(this).data("str"));
            }
        });
        $(document).on("click", ".table-class1 td.danger", function () {
            if ($(this).find("p").text().length == 0) {
                return false;
            } else {
                if ($("[name=teacherforchange]").val() == "") {
                    ShowAlert("请选择调课老师");
                    return false;
                }
                $(".table-class1 td.warning").addClass("danger");
                $(".table-class1 td").removeClass("warning");
                $(this).addClass("warning");
                $("[name=classTableStrB]").val($(this).data("str"));
            }
        });
    },
    submitData : function(){
        $("#submitAdjustment").click(function(){
            var classTableStrA = $("[name=classTableStrA]").val(),
            classTableStrB = $("[name=classTableStrB]").val();
            if(classTableStrA==""||classTableStrB==""){
                ShowAlert("请完成课表调整");
                return false;
            }
            ConfirmAction("是否确定调整", function(){
                AjaxPost("QueryChangeAdministrativClassTable",{
                    "classTableStrA":classTableStrA,
                    "classTableStrB":classTableStrB
                },function(jsonData){
                    var result = parseInt(jsonData['Result']);
                    var message = jsonData['Message'];
                    if (result > 0) {
                        if (lib51.isSet(message)) {
                            ShowSuccess(message,function(){
                                window.location.reload();
                            });
                        }
                    }
                    else {
                        if (lib51.isSet(message)) {
                            ShowAlert(message);
                        }
                    }
                });
            });
        });
    },
    doOpt: function () {
        this.showTeacherList();
        this.showtimetable("teacherforchange", "teacherforassist");
        this.showtimetable("teacherforassist", "teacherforchange");
        this.clickTableCell();
        this.submitData();
    }
};
var tablesetting = {
    tablebuild: {
        day: ["星期一", "星期二", "星期三", "星期四", "星期五", "星期六", "星期日"],
        cellbuild: function (tableName) {

            this.classbuild(tableParameter.morning, "tablemorning", "morning", "上午", tableName);
            this.classbuild(tableParameter.afternoon, "tableafternoon", "afternoon", "下午", tableName);
            this.classbuild(tableParameter.evening, "tableevening", "evening", "晚上", tableName);
            this.daybuild(tableParameter.day, tableName);
            this.serialnumberbuild(tableName);
        },
        daybuild: function (VAL, TABLENAME) {
            var theadHtml = "<th colspan='2' style='width:28.572%;'>节次</th>",
                tbodyHtml = "";
            if (VAL) {
                for (var i = 0; i < VAL; i++) {
                    theadHtml += "<th style='width:14.286%;'>" + this.day[i] + "</th>";
                    tbodyHtml += "<td data-str=''><p style='height:50px;'></p></td>";
                }
                $(TABLENAME + " tbody tr").each(function () {
                    $(this).find("td").remove();
                    $(this).append(tbodyHtml);
                });
            }
            $(TABLENAME + " thead tr").html(theadHtml);
        },
        classbuild: function (VAL, HIDDENINPUT, CLASSNAME, TIME, TABLENAME) {
            var atext = $(TABLENAME + " tbody tr th").index(),
                arr = [],
                i = "",
                tbodyhtml = "<tr class='text-c table-" + CLASSNAME + "'>",
                aa = "";
            $(TABLENAME + " tbody tr th[rowspan]").each(function () {
                arr.push($(TABLENAME + " tbody tr th").index(this));
                if ($(this).text() == TIME) {
                    i = $(TABLENAME + " tbody tr th").index(this);
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
                        $(TABLENAME).prepend(tbodyhtml);
                        break;
                    case "下午":
                        if ($(TABLENAME + ".table-morning").length > 0) {
                            $(TABLENAME + ".table-morning:last").after(tbodyhtml);
                        } else if ($(TABLENAME + ".table-evening").length > 0) {
                            $(TABLENAME + ".table-evening:first").before(tbodyhtml);
                        } else {
                            $(TABLENAME).prepend(tbodyhtml);
                        }
                        break;
                    case "晚上":
                        $(TABLENAME).append(tbodyhtml);
                        break;
                }
                this.daybuild(tableParameter.day);
            }
            $("[name=" + HIDDENINPUT + "]").val(VAL);
        },
        serialnumberbuild: function (TABLENAME) {
            var len = $(TABLENAME + " tbody th:not([rowspan])").length;
            for (var i = 0; i < len; i++) {
                $(TABLENAME + " tbody th:not([rowspan])").eq(i).text("第" + parseInt(i + 1) + "节");
            }
        }
    },
    colorfill: function (tableName) {
        this.tablebuild.cellbuild(tableName);
    }
};
tablesetting.colorfill(".table-class");
$(".table-class1").append($(".table-class").html());
timetablejustment.doOpt();