﻿
$(document).ready(function () {
    $.getJSON("Dashboard/GetData", function (result) {
        
        var lineTimeStamps = [];
        var lineAmounts = [];
        var tooltipStrings = [];
        var tooltipDecimals = [];
        var monthNames = [];
        var rd = [];
        for (var i = 0; i < 31; i++) {
            rd.push(String(i));
        }
        var xStart = result.listLast30[0].timeStamp;
        $.each(result.listLast30, function (index, element) {
            lineTimeStamps.push(
                String(element.timeStamp),
            );
            monthNames.push(
                String(element.monthName),
            );
            lineAmounts.push({
                y: parseFloat(element.amount),
            });
        }),
            $.each(result.tooltipList, function (index, element) {
                tooltipStrings.push(
                    element.categoryListTooltip,
                );

            }),
            $.each(result.tooltipList, function (index, element) {
                var tempArray = [];
                $.each(element.amount, function (index2, element2) {
                    $.each(element2, function (index3, element3) {
                        if (element3 != 0) {
                            tempArray.push(
                                parseFloat(element3),
                            );
                        }
                        else {
                            tempArray.push(
                                parseFloat(0),
                            );
                        }
                    })

                })
                tooltipDecimals.push(tempArray);

            }), // initiate/load data for tooltips

            
            Highcharts.chart('container', {
                title: {
                    text: 'Expenses in the last 30 days'
                },

                yAxis: {
                    title: {
                        text: 'Amount of expenses [$]'
                    }
                },

                xAxis: {
                    title: {
                        text: 'Day of the month'
                    },
                    categories: lineTimeStamps
                },

                legend: {
                    enabled: false,
                },

                plotOptions: {
                    series: {
                        label: {
                            connectorAllowed: false
                        },
                        cursor: 'pointer',
                        events: {
                            click: function (event) {
                                sendToList(result.listLast30[event.point.x].oneId);
                                if (tooltipStrings[event.point.x] == '') {
                                    $.notify(
                                        "No expenses to show",
                                        { globalPosition: "top right", clickToHide: true, autoHide: true, autoHideDelay: 1500, className: 'info'}
                                    );
                                }
                            }
                        }
                    }
                },

                series: [{
                    name: 'Amount',
                    data: lineAmounts
                }],

                tooltip: {
                    useHTML: true,
                    outside: true,
                    formatter: function () {

                        function cateList(array1, array2) {
                            var firstText = '';
                            for (var i = 0; i < array1.length; i++) {
                                firstText += '<tr><td>' + array1[i] +'&nbsp;&nbsp;'+ '</td>'+'<td style="text-align: right">'+array2[i]+'$</td>'+'</tr>';
                            }

                            return firstText;
                        }
                        

                        if (cateList(tooltipStrings[this.point.x], tooltipDecimals[this.point.x]) != '') {
                            return '<small>' + this.x + '&nbsp;&nbsp;' + monthNames[this.point.x] + '</small><br/>' + '<table class="table"><tr><th>Category</th><th style="text-align: right">Amount</th></tr>' +
                                cateList(tooltipStrings[this.point.x], tooltipDecimals[this.point.x]) + '</table>';
                        }
                        else {
                            return '<small>' + this.x + '&nbsp;&nbsp;' + monthNames[this.point.x] + '</small><br/>' + '<p style="font-weight: bold">No expenses</p>';
                        }

                    },
                    
                },

                responsive: {
                    rules: [{
                        condition: {
                            maxWidth: 500
                        },
                        chartOptions: {
                            legend: {
                                layout: 'horizontal',
                                align: 'center',
                                verticalAlign: 'bottom'
                            }
                        }
                    }]
                }

            });
    });
})