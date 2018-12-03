$(function () {
    tbuild.cellbuild("DaysForWeek", $("[name=DaysForWeek]").val());
    tbuild.cellbuild("MorningSection", $("[name=MorningSection]").val());
    tbuild.cellbuild("AfternoonSection", $("[name=AfternoonSection]").val());
    tbuild.cellbuild("NightSection", $("[name=NightSection]").val());
    $("select").change(function (e, index) {
        tbuild.cellbuild($(this).attr("name"), $(this).val());
    });
});
var tbuild = {
    day: ["星期一", "星期二", "星期三", "星期四", "星期五", "星期六", "星期日"],
    cellbuild: function (obj, val) {
        switch (obj) {
            case "DaysForWeek":
                this.daybuild(val);
                break;
            case "MorningSection":
                this.classbuild(val, "tablemorning", "morning", "上午");
                break;
            case "AfternoonSection":
                this.classbuild(val, "tableafternoon", "afternoon", "下午");
                break;
            case "NightSection":
                this.classbuild(val, "tableevening", "evening", "晚上");
                break;
        }
        this.serialnumberbuild();
    },
    daybuild: function (VAL) {
        var theadHtml = "<th colspan='2'>节次</th>",
            tbodyHtml = "";
        if (VAL.length > 0) {
            for (var i = 0; i < VAL; i++) {
                theadHtml += "<th>" + this.day[i] + "</th>";
                tbodyHtml += "<td></td>";
            }
            $(".table tbody tr").each(function () {
                $(this).find("td").remove();
                $(this).append(tbodyHtml);
            });
        }
        $(".table thead tr").html(theadHtml);
        $("[name=tableday]").val(VAL);
    },
    classbuild: function (VAL, HIDDENINPUT, CLASSNAME, TIME) {
        var atext = $(".table tbody tr th").index(),
            currentNumber = $("[name=" + HIDDENINPUT + "]").val(),
            arr = [],
            i = "",
            tbodyhtml = "<tr class='text-c table-" + CLASSNAME + "'>",
            aa = "";
        $(".table tbody tr th[rowspan]").each(function () {
            arr.push($(".table tbody tr th").index(this));
            if ($(this).text() == TIME) {
                i = $(".table tbody tr th").index(this);
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
                    $(".table").prepend(tbodyhtml);
                    break;
                case "下午":
                    if ($(".table-morning").length > 0) {
                        $(".table-morning:last").after(tbodyhtml);
                    } else if ($(".table-evening").length > 0) {
                        $(".table-evening:first").before(tbodyhtml);
                    } else {
                        $(".table").prepend(tbodyhtml);
                    }
                    break;
                case "晚上":
                    $(".table").append(tbodyhtml);
                    break;
            }
            this.daybuild($("[name=tableday]").val());
        }
        $("[name=" + HIDDENINPUT + "]").val(VAL);
    },
    serialnumberbuild: function () {
        var len = $("tbody th:not([rowspan])").length;
        for(var i=0;i<len;i++){
            $("tbody th:not([rowspan])").eq(i).text("第"+parseInt(i+1)+"节");
        }
    }
};