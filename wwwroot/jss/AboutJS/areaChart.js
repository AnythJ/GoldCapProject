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



            $(function () {
                var areaChart = Highcharts.chart('container-area', {
                    chart: {
                        type: 'area'
                    },
                    accessibility: {
                        description: 'Image description: An area chart compares the nuclear stockpiles of the USA and the USSR/Russia between 1945 and 2017. The number of nuclear weapons is plotted on the Y-axis and the years on the X-axis. The chart is interactive, and the year-on-year stockpile levels can be traced for each country. The US has a stockpile of 6 nuclear weapons at the dawn of the nuclear age in 1945. This number has gradually increased to 369 by 1950 when the USSR enters the arms race with 6 weapons. At this point, the US starts to rapidly build its stockpile culminating in 32,040 warheads by 1966 compared to the USSR’s 7,089. From this peak in 1966, the US stockpile gradually decreases as the USSR’s stockpile expands. By 1978 the USSR has closed the nuclear gap at 25,393. The USSR stockpile continues to grow until it reaches a peak of 45,000 in 1986 compared to the US arsenal of 24,401. From 1986, the nuclear stockpiles of both countries start to fall. By 2000, the numbers have fallen to 10,577 and 21,000 for the US and Russia, respectively. The decreases continue until 2017 at which point the US holds 4,018 weapons compared to Russia’s 4,500.'
                    },
                    title: {
                        text: 'US and USSR nuclear stockpiles'
                    },
                    subtitle: {
                        text: 'Sources: <a href="https://thebulletin.org/2006/july/global-nuclear-stockpiles-1945-2006">' +
                            'thebulletin.org</a> &amp; <a href="https://www.armscontrol.org/factsheets/Nuclearweaponswhohaswhat">' +
                            'armscontrol.org</a>'
                    },
                    xAxis: {
                        title: {
                            text: 'Day of the month'
                        },
                        categories: lineTimeStamps
                    },
                    yAxis: {
                        title: {
                            text: 'Amount of expenses [$]'
                        }
                    },
                    tooltip: {
                        pointFormat: '{series.name} had stockpiled <b>{point.y:,.0f}</b><br/>warheads in {point.x}'
                    },
                    plotOptions: {
                        series: {
                            cursor: 'pointer',
                            events: {
                                click: function (event) {
                                    sendToList(result.listLast30[event.point.x].oneId, null);
                                    if (tooltipStrings[event.point.x] == '') {
                                        $.notify(
                                            "No expenses to show",
                                            { globalPosition: "top right", clickToHide: true, autoHide: true, autoHideDelay: 1500, className: 'info' }
                                        );
                                    }
                                }
                            }
                        }
                    },
                    series: [{
                        name: 'Amount',
                        data: lineAmounts
                    }]
                });
            });
        
    });
    
});
