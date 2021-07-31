
$(document).ready(function () {
    $.getJSON("Dashboard/GetData", function (result) {
        
        var lineTimeStamps = [];
        var lineAmounts = [];
        $.each(result.listLast30, function (index, element) {
            lineTimeStamps.push(
                String(element.timeStamp),
            );
            lineAmounts.push({
                y: parseFloat(element.amount),
            });
        }),
            console.log(result),
            console.log(lineTimeStamps),
            console.log(lineAmounts),
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
                        return 'The value for <b>' + this.x +
                            '</b> is <b>' + this.y + '</b>'; //HERE 31.07
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