var hcharts = {
    getData: function () {
        var arr = [],
            arr1 = [],
            arr2 = [],
            arr3 = 0;
        $(".table-data tbody tr").each(function () {
            arr1.push($(this).find("td").eq(1).text());
            arr2.push(parseInt($(this).find("td").eq(2).text()));
            arr3 += parseInt($(this).find("td").eq(2).text());
        });
        arr.push(arr1, arr2, arr3);
        return arr
    },
    getPercent: function () {
        var percentArr = [],
            per = 0,
            dataArr = this.getData();
        for (var i = 0; i < dataArr[1].length; i++) {
            //per = parseFloat(parseFloat(dataArr[1][i] / dataArr[2] * 100).toFixed(3));
            per = parseFloat(parseFloat(dataArr[1][i]));
            percentArr.push([dataArr[0][i], per]);
        }
        return percentArr;
    },
    column: function (data) {
        var dataArr = this.getData();
        $('#container').highcharts({
            chart: {
                type: 'column',
                marginBottom: 20,
                marginLeft: 20,
                options3d: {
                    enabled: true,
                    alpha: 5,
                    beta: -19,
                    depth: 50,
                    viewDistance: 25
                }
            },
            title: {
                text: null
            },
            subtitle: {
                text: null
            },
            legend: {
                enabled: false
            },
            plotOptions: {
                column: {
                    depth: 25,
                    dataLabels: {
                        align: "left",
                        color: "#666",
                        enabled: true,
                        allowOverlap: true
                    }
                }
            },
            xAxis: {
                categories: dataArr[0]
            },
            yAxis: {
                title: {
                    text: null
                }
            },
            tooltip: {
                shared: true,
                useHTML: true,
                formatter: function () {
                    var s = "";
                    $.each(this.points, function () {
                        s += chartsData.data[this.point.index];
                    });
                    return s;
                },
                valueDecimals: 2
            },
            series: [{
                name: '',
                colorByPoint: true,
                data: dataArr[1],
            }],
            credits: {
                enabled: false
            }
        });
    },
    bar: function (data) {
        var dataArr = this.getData();
        $('#container').highcharts({
            chart: {
                type: 'bar',
                marginRight: 100,
                marginLeft: 0,
                marginTop: 0,
                marginBottom: 20,
                options3d: {
                    enabled: true,
                    alpha: 0,
                    beta: 0,
                    depth: 30,
                    viewDistance: 25
                }
            },
            title: {
                text: ''
            },
            subtitle: {
                text: ''
            },
            legend: {
                enabled: false
            },
            plotOptions: {
                bar: {
                    depth: 0,
                    dataLabels: {
                        align: "left",
                        color: "#666",
                        enabled: true,
                        allowOverlap: true,
                        inside: false,
                        overflow: "none",
                        formatter: function () {
                            return this.x + " : " + this.y
                        }
                    }
                }
            },
            tooltip: {
                shared: true,
                useHTML: true,
                formatter: function () {
                    var s = "";
                    $.each(this.points, function () {
                        s += chartsData.data[this.point.index];
                    });
                    return s;
                }
            },
            xAxis: {
                categories: dataArr[0],
                visible: true
            },
            yAxis: {
                title: {
                    text: null
                },
                visible: true
            },
            series: [{
                name: '',
                colorByPoint: true,
                data: dataArr[1],
            }],
            credits: {
                enabled: false
            }
        });
    },
    pie: function (data) {
        var dataArr = this.getData();
        // this.getPercent();
        $('#container').highcharts({
            chart: {
                type: 'pie',
                options3d: {
                    enabled: true,
                    alpha: 45,
                    beta: 0
                }
            },
            title: {
                text: null
            },
            legend: {
                enabled: true
            },
            tooltip: {
                shared: true,
                useHTML: true,
                headerFormat: '',
                pointFormat: '{point.name}: <b>{point.percentage:.1f}%</b>'
            },
            plotOptions: {
                pie: {
                    dataLabels: {
                        enabled: true,
                        // inside:true,
                        format: '人数：{y}',
                        shadow:true
                    },
                    depth:20,
                    showInLegend: true
                }
            },
            series: [{
                type: 'pie',
                name: null,
                data: this.getPercent()
            }],
            credits: {
                enabled: false
            }
        });
    }
}
var table2charts = function () {
    switch (chartsData.type) {
        case "single":
            hcharts.column(chartsData.data);
            break;
        case "double":
            hcharts.bar(chartsData.data);
            break;
        case "triple":
            hcharts.pie(chartsData.data);
            break;
        default:
            break;
    }
    $(".btn-show").click(function () {
        var subIds = $(this).data("id");
        var id = $("#hdTaskId").val();
        layer_show("列表", "/Management/CourseSubject/StatisticsDepartmentList?subIds=" + subIds + "&taskId=" + id, 700, 600);
    });
}
table2charts();