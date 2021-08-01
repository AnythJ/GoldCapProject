
$(document).ready(function () {
    $.getJSON("Dashboard/GetData", function (result) {
        
        var lineTimeStamps = [];
        var lineAmounts = [];
        var tooltipStrings = [];
        var tooltipDecimals = [];
        var rd = [];
        for (var i = 0; i < 31; i++) {
            rd.push(String(i));
        }
        var xStart = result.listLast30[0].timeStamp;
        $.each(result.listLast30, function (index, element) {
            lineTimeStamps.push(
                String(element.timeStamp),
            );
            lineAmounts.push({
                y: parseFloat(element.amount),
            });
        }),
            $.each(result.tooltipList, function (index, element) {
                tooltipStrings.push(
                    String(element.categoryListTooltip),
                );
                //if (element.amount != 0) {
                //    tooltipDecimals.push(
                //        parseFloat(element.amount),
                //    );
                //}
                //else {
                //    tooltipDecimals.push(
                //        parseFloat(0),
                //    );
                //}

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

            console.log(result),
            console.log(tooltipStrings),
            console.log(tooltipDecimals),
            
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
                    layout: 'vertical',
                    align: 'right',
                    verticalAlign: 'middle'
                },

                plotOptions: {
                    series: {
                        label: {
                            connectorAllowed: false
                        },
                        /*pointStart: result.day30*/
                    }
                },

                series: [{
                    name: 'Amount',
                    data: lineAmounts
                }],

                tooltip: {
                    formatter: function () {
                        /*return tooltipStrings[this.point.x] + "=" + tooltipDecimals[this.point.x];*/
                        return 'The value for <b>' + this.x +
                            '</b> is <b>' + this.y + '</b>';
                    }
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