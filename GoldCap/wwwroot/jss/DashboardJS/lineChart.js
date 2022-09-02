function showCharts(period) {
    var request = new XMLHttpRequest();
    request.open('GET', "Dashboard/GetData?period=" + String(period), true);

    request.onload = function () {
        if (this.status >= 200 && this.status < 400) {

            var result = JSON.parse(this.response);
            var periodName;
            switch (period) {
                case 7:
                    periodName = 'week'
                    break;
                case 30:
                    periodName = 'month'
                    break;
                case 365:
                    periodName = 'year'
                    break;
                default:
                    break;
            }
            var lineTimeStamps = [];
            var lineAmounts = [];
            var tooltipStrings = [];
            var tooltipDecimals = [];
            var monthNames = [];
            var rd = [];
            for (var i = 0; i < parseFloat(period) + 1; i++) {
                rd.push(String(i));
            }
            var xStart = result.listLast30[0].timeStamp;



            Array.from(result.listLast30).forEach(element => {
                lineTimeStamps.push(
                    String(element.timeStamp),
                );
                monthNames.push(
                    String(element.monthName),
                );
                lineAmounts.push({
                    y: parseFloat(element.amount),
                });
            });
            Array.from(result.tooltipList).forEach(element => {
                tooltipStrings.push(
                    element.categoryListTooltip,
                );
            });
            Array.from(result.tooltipList).forEach(element => {
                var tempArray = [];
                Array.from(element.amount).forEach(element2 => {
                    Array.from(element2).forEach(element3 => {
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
                    });
                });
                tooltipDecimals.push(tempArray);
            }); // initiate/load data for tooltips

            (function () {
                var chart = Highcharts.chart('container', {
                    chart: {
                        type: 'area',
                        style: {
                            fontFamily: 'Arial',
                            cursor: 'crosshair',
                            color: 'var(--text-secondary)'
                        },
                        backgroundColor: 'var(--bg-secondary)',
                    },
                    credits: {
                        enabled: false
                    },
                    title: {
                        text: 'Expenses in the last ' + String(lineTimeStamps.length - 1) + ' days',
                        style: {
                            color: 'var(--text-secondary)'
                        }
                    },

                    yAxis: {
                        title: {
                            style: {
                                color: 'var(--text-secondary)'
                            },
                            text: 'Amount of expenses [$]'
                        },
                        labels: {
                            style: {
                                color: 'var(--text-secondary)'
                            }
                        }
                    },

                    xAxis: {
                        title: {
                            style: {
                                color: 'var(--text-secondary)'
                            },
                            text: 'Day of the ' + periodName
                        },
                        labels: {
                            style: {
                                color: 'var(--text-secondary)'
                            }
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
                            fillColor: 'var(--bg-third)',
                            cursor: 'pointer',
                            events: {
                                click: function (event) {
                                    sendToList(result.listLast30[event.point.x].oneId, null, period);
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
                        color: 'var(--text-secondary)',
                        name: '',
                        data: lineAmounts
                    }],

                    tooltip: {
                        useHTML: true,
                        outside: true,
                        backgroundColor: 'var(--bg-secondary)',
                        style: {
                            color: 'var(--text-secondary)',
                            zIndex: 15,
                        },
                        formatter: function () {

                            function cateList(array1, array2) {
                                var firstText = '';
                                for (var i = 0; i < array1.length; i++) {
                                    firstText += '<tr style="color:var(--text-secondary);"><td>' + array1[i] + '&nbsp;&nbsp;' + '</td>' + '<td style="text-align: right">' + array2[i] + '$</td>' + '</tr>';
                                }

                                return firstText;
                            }


                            if (cateList(tooltipStrings[this.point.x], tooltipDecimals[this.point.x]) != '') {
                                return '<small>' + this.x + '&nbsp;&nbsp;' + monthNames[this.point.x] + '</small><br/>' + '<table style="color:var(--text-secondary);" class="table"><tr><th>Category</th><th style="text-align: right">Amount</th></tr>' +
                                    cateList(tooltipStrings[this.point.x], tooltipDecimals[this.point.x]) + '</table>';
                            }
                            else {
                                return '<small>' + this.x + '&nbsp;&nbsp;' + monthNames[this.point.x] + '</small><br/>' + '<p style="font-weight: bold">No expenses</p>';
                            }

                        },

                    },
                    exporting: {
                        enabled: false
                    },
                    responsive: {
                        rules: [{
                            condition: {

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
            })();

            var cateData = [];
            var cateNames = [];

            Array.from(result.categoryRatios).forEach(element => {
                if (element.categoryPercentage > 0) {
                    cateData.push({
                        name: element.categoryName,
                        y: parseFloat(element.categoryPercentage),
                    });
                    cateNames.push(element.categoryName);
                }
            });

           
                (function () {
                    var pieColors = [];
                    while (pieColors.length < 100) {
                        do {
                            var color = Math.floor((Math.random() * 1000000) + 1);
                        } while (pieColors.indexOf(color) >= 0);
                        pieColors.push("#" + ("000000" + color.toString(16)).slice(-6));
                    }
                    var pieChart = Highcharts.chart('container-pie', {
                        chart: {
                            plotBackgroundColor: null,
                            plotShadow: false,
                            spacingLeft: 0,
                            spacingRight: 0,
                            type: 'pie',
                            style: {
                                fontFamily: 'Arial'
                            },
                            backgroundColor: 'var(--bg-primary)'
                        },
                        title: {
                            style: {
                                color: 'var(--text-secondary)'
                            },
                            text: 'Categories in %'
                        },
                        exporting: {
                            enabled: false
                        },
                        tooltip: {
                            useHTML: true,
                            headerFormat: '',
                            backgroundColor: 'var(--bg-secondary)',
                            style: {
                                color: 'var(--text-secondary)'
                            },
                            pointFormat: '<span style="margin-right:0.3125rem;height:0.625rem;width:0.625rem;background-color:{point.color};border-radius:50%;display:inline-block;"></span><b>{point.name}</b>: <b>{point.percentage:.1f}%</b>'
                        },
                        accessibility: {
                            point: {
                                valueSuffix: '%'
                            }
                        },
                        plotOptions: {
                            pie: {
                                allowPointSelect: true,
                                cursor: 'pointer',
                                colors: pieColors,
                                borderColor: 'var(--piechart-border-color)',
                                dataLabels: {
                                    enabled: false,
                                },
                                point: {
                                    events: { /*Function to trigger selection on Most used categories list at the same time as pie chart selection*/
                                        click: function (event) {
                                            var tBody = document.getElementById('tableBody');
                                            var trRows = tBody.getElementsByClassName('trClass');
                                            for (var i = 0; i < trRows.length; i++) {
                                                trRows[i].style.backgroundColor = "var(--bg-secondary)";
                                                trRows[i].style.color = "var(--text-secondary)";
                                                trRows[i].style.fontWeight = "normal";
                                                trRows[i].cells[0].style.borderLeft = "none";
                                            }
                                            var found;
                                            var finalRow;

                                            for (var i = 0, row; row = tBody.rows[i]; i++) {
                                                var idElement = document.getElementById('active');

                                                for (var j = 0, col; col = row.cells[j]; j++) {
                                                    if (col.textContent.replace(/[\n\r]+|[\s]{2,}/g, ' ').trim() == this.name) {
                                                        if (idElement == null || idElement != row) {
                                                            //active
                                                            row.style.backgroundColor = "var(--piechart-row-bg)";
                                                            col.style.borderLeft = "0.625rem solid " + event.point.color;
                                                            row.style.color = "var(--text-secondary)";
                                                            row.style.fontWeight = "bold";
                                                            row.setAttribute("id", "active");
                                                            finalRow = row;
                                                            if (idElement != null) {
                                                                idElement.style.fontWeight = "normal";
                                                                idElement.setAttribute("id", "");
                                                            }

                                                            sendToList(-1, finalRow.textContent.replace(/[\n\r]+|[\s]{2,}/g, ' ').trim(), period);
                                                            break;

                                                        }
                                                        else {
                                                            //unactive
                                                            row.style.backgroundColor = "var(--bg-secondary)";
                                                            row.style.color = "var(--text-secondary)";
                                                            row.style.fontWeight = "normal";
                                                            row.setAttribute("id", "");
                                                            sendToList(-1, null, period);
                                                        }
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
                        }],
                        credits: {
                            enabled: false
                        }
                    });
                })();

        } else {


        }
    };

    request.onerror = function () {

    };

    request.send();
};



function reloadAreaChart(period) {
    var request = new XMLHttpRequest();
    request.open('GET', "Dashboard/GetData?period=" + String(period), true);

    request.onload = function () {
        if (this.status >= 200 && this.status < 400) {

            var result = JSON.parse(this.response);
            var periodName;
            switch (period) {
                case 7:
                    periodName = 'week'
                    break;
                case 30:
                    periodName = 'month'
                    break;
                case 365:
                    periodName = 'year'
                    break;
                default:
                    break;
            }
            var lineTimeStamps = [];
            var lineAmounts = [];
            var tooltipStrings = [];
            var tooltipDecimals = [];
            var monthNames = [];
            var rd = [];
            for (var i = 0; i < parseFloat(period) + 1; i++) {
                rd.push(String(i));
            }
            var xStart = result.listLast30[0].timeStamp;

            
            
            Array.from(result.listLast30).forEach(element => {
                lineTimeStamps.push(
                    String(element.timeStamp),
                );
                monthNames.push(
                    String(element.monthName),
                );
                lineAmounts.push({
                    y: parseFloat(element.amount),
                });
            });
            Array.from(result.tooltipList).forEach(element => {
                tooltipStrings.push(
                    element.categoryListTooltip,
                );
            });
            Array.from(result.tooltipList).forEach(element => {
                var tempArray = [];
                Array.from(element.amount).forEach(element2 => {
                    Array.from(element2).forEach(element3 => {
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
                    });
                });
                tooltipDecimals.push(tempArray);
            }); // initiate/load data for tooltips


                (function () {
                    var chart = Highcharts.chart('container', {
                        chart: {
                            type: 'area',
                            style: {
                                fontFamily: 'Arial',
                                cursor: 'crosshair',
                                color: 'var(--text-secondary)'
                            },
                            backgroundColor: 'var(--bg-secondary)',
                        },
                        credits: {
                            enabled: false
                        },
                        title: {
                            text: 'Expenses in the last ' + String(lineTimeStamps.length - 1) + ' days',
                            style: {
                                color: 'var(--text-secondary)'
                            }
                        },

                        yAxis: {
                            title: {
                                style: {
                                    color: 'var(--text-secondary)'
                                },
                                text: 'Amount of expenses [$]'
                            },
                            labels: {
                                style: {
                                    color: 'var(--text-secondary)'
                                }
                            }
                        },

                        xAxis: {
                            title: {
                                style: {
                                    color: 'var(--text-secondary)'
                                },
                                text: 'Day of the ' + periodName
                            },
                            labels: {
                                style: {
                                    color: 'var(--text-secondary)'
                                }
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
                                fillColor: 'var(--bg-third)',
                                cursor: 'pointer',
                                events: {
                                    click: function (event) {
                                        sendToList(result.listLast30[event.point.x].oneId, null, period);
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
                            color: 'var(--text-secondary)',
                            name: '',
                            data: lineAmounts
                        }],

                        tooltip: {
                            useHTML: true,
                            outside: true,
                            backgroundColor: 'var(--bg-secondary)',
                            style: {
                                color: 'var(--text-secondary)',
                                zIndex: 15,
                            },
                            formatter: function () {

                                function cateList(array1, array2) {
                                    var firstText = '';
                                    for (var i = 0; i < array1.length; i++) {
                                        firstText += '<tr style="color:var(--text-secondary);"><td>' + array1[i] + '&nbsp;&nbsp;' + '</td>' + '<td style="text-align: right">' + array2[i] + '$</td>' + '</tr>';
                                    }

                                    return firstText;
                                }


                                if (cateList(tooltipStrings[this.point.x], tooltipDecimals[this.point.x]) != '') {
                                    return '<small>' + this.x + '&nbsp;&nbsp;' + monthNames[this.point.x] + '</small><br/>' + '<table style="color:var(--text-secondary);" class="table"><tr><th>Category</th><th style="text-align: right">Amount</th></tr>' +
                                        cateList(tooltipStrings[this.point.x], tooltipDecimals[this.point.x]) + '</table>';
                                }
                                else {
                                    return '<small>' + this.x + '&nbsp;&nbsp;' + monthNames[this.point.x] + '</small><br/>' + '<p style="font-weight: bold">No expenses</p>';
                                }

                            },

                        },
                        exporting: {
                            enabled: false
                        },
                        responsive: {
                            rules: [{
                                condition: {

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
                })();

        } else {


        }
    };

    request.onerror = function () {

    };

    request.send();
}