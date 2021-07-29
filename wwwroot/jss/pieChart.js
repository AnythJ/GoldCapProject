$(document).ready(function () {
    $.getJSON("Dashboard/GetData", function (result) {
        var cateData = [];
        $.each(result.categoryRatios, function (index, element) {
            cateData.push({
                name: element.categoryName,
                y: parseFloat(element.categoryPercentage),
            });
        }),
            Highcharts.chart('container-pie', {
                chart: {
                    plotBackgroundColor: null,
                    plotBorderWidth: null,
                    plotShadow: false,
                    type: 'pie'
                },
                title: {
                    text: 'Categories in %'
                },
                exporting: {
                    enabled: false
                },
                //tooltip: {
                //    pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
                //},
                accessibility: {
                    point: {
                        valueSuffix: '%'
                    }
                },
                plotOptions: {
                    pie: {
                        allowPointSelect: true,
                        cursor: 'pointer',
                        dataLabels: {
                            enabled: false,
                            format: '<b>{point.name}</b>: {point.percentage:.1f} %'
                        }
                    }
                },
                series: [{
                    name: '',
                    colorByPoint: true,
                    data: cateData
                }]
            });
    });
});