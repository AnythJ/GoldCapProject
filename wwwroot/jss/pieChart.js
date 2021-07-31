﻿$(document).ready(function () {
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
                        },
                        point: {
                            events: { /*Function to trigger selection on Most used categories list at the same time as pie chart selection*/
                                select: function () {

                                    var tBody = document.getElementById('tableBody');
                                    var trRows = tBody.getElementsByClassName('trClass');
                                    for (var i = 0; i < trRows.length; i++) {
                                        trRows[i].style.backgroundColor = "white";
                                        trRows[i].style.color = "black";
                                    }
                                    var found;
                                    for (var i = 0, row; row = tBody.rows[i]; i++) {
                                        for (var j = 0, col; col = row.cells[j]; j++) {
                                            if (col.textContent.replace(/[\n\r]+|[\s]{2,}/g, ' ').trim() == this.name) {
                                                row.style.backgroundColor = "#5f666d";
                                                row.style.color = "white";
                                            }
                                        }
                                    }
                                }
                            }
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