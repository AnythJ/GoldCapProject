
$(document).ready(function () {
    $.getJSON("Dashboard/GetData", function (result) {
        
        var lineTimeStamps = [];
        var lineAmounts = [];
        $.each(result.listLast30, function (index, element) {
            lineTimeStamps.push({
                name: element.timeStamp,
            });
            lineAmounts.push({
                name: parseFloat(element.listLast30.amount)
            });
        }),
            console.log(result),
            Highcharts.chart('container', {
                title: {
                    text: 'Expenses in the last 30 days'
                },

                yAxis: {
                    title: {
                        text: 'Amount of expenses'
                    }
                },

                xAxis: {
                    title: {
                        text: 'Day of the month'
                    },
                    //categories: [
                    //    [result.listLast30[0].timeStamp],
                    //    [result.listLast30[1].timeStamp],
                    //    [result.listLast30[2].timeStamp],
                    //    [result.listLast30[3].timeStamp],
                    //    [result.listLast30[4].timeStamp],
                    //    [result.listLast30[5].timeStamp],
                    //    [result.listLast30[6].timeStamp],
                    //    [result.listLast30[7].timeStamp],
                    //    [result.listLast30[8].timeStamp],
                    //    [result.listLast30[9].timeStamp],
                    //    [result.listLast30[10].timeStamp],
                    //    [result.listLast30[11].timeStamp],
                    //    [result.listLast30[12].timeStamp],
                    //    [result.listLast30[13].timeStamp],
                    //    [result.listLast30[14].timeStamp],
                    //    [result.listLast30[15].timeStamp],
                    //    [result.listLast30[16].timeStamp],
                    //    [result.listLast30[17].timeStamp],
                    //    [result.listLast30[18].timeStamp],
                    //    [result.listLast30[19].timeStamp],
                    //    [result.listLast30[20].timeStamp],
                    //    [result.listLast30[21].timeStamp],
                    //    [result.listLast30[22].timeStamp],
                    //    [result.listLast30[23].timeStamp],
                    //    [result.listLast30[24].timeStamp],
                    //    [result.listLast30[25].timeStamp],
                    //    [result.listLast30[26].timeStamp],
                    //    [result.listLast30[27].timeStamp],
                    //    [result.listLast30[28].timeStamp],
                    //    [result.listLast30[29].timeStamp],
                    //    [result.listLast30[30].timeStamp]
                    //]
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
                    name: 'Expenses',
                    //data: [
                    //    [result.listLast30[0].amount],
                    //    [result.listLast30[1].amount],
                    //    [result.listLast30[2].amount],
                    //    [result.listLast30[3].amount],
                    //    [result.listLast30[4].amount],
                    //    [result.listLast30[5].amount],
                    //    [result.listLast30[6].amount],
                    //    [result.listLast30[7].amount],
                    //    [result.listLast30[8].amount],
                    //    [result.listLast30[9].amount],
                    //    [result.listLast30[10].amount],
                    //    [result.listLast30[11].amount],
                    //    [result.listLast30[12].amount],
                    //    [result.listLast30[13].amount],
                    //    [result.listLast30[14].amount],
                    //    [result.listLast30[15].amount],
                    //    [result.listLast30[16].amount],
                    //    [result.listLast30[17].amount],
                    //    [result.listLast30[18].amount],
                    //    [result.listLast30[19].amount],
                    //    [result.listLast30[20].amount],
                    //    [result.listLast30[21].amount],
                    //    [result.listLast30[22].amount],
                    //    [result.listLast30[23].amount],
                    //    [result.listLast30[24].amount],
                    //    [result.listLast30[25].amount],
                    //    [result.listLast30[26].amount],
                    //    [result.listLast30[27].amount],
                    //    [result.listLast30[28].amount],
                    //    [result.listLast30[29].amount],
                    //    [result.listLast30[30].amount]

                    //]
                    data: lineAmounts
                }],

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