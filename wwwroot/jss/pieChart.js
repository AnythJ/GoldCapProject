
function showPieChart(period) {
    $.getJSON("Dashboard/GetData/?period=" + String(period), function (result) {
        var cateData = [];
        var cateNames = [];
        $.each(result.categoryRatios, function (index, element) {
            if (element.categoryPercentage > 0) {
                cateData.push({
                    name: element.categoryName,
                    y: parseFloat(element.categoryPercentage),
                });
                cateNames.push(element.categoryName);
            }
        }),

            //Highcharts.setOptions({
            //    colors: [
            //        "#63B598", "#CE7D78", "#EA9E70", "#A48A9E", "#C6E1E8", "#648177", "#0D5AC1",
            //        "#F205E6", "#1C0365", "#14A9AD", "#4CA2F9", "#A4E43F", "#D298E2", "#6119D0",
            //        "#D2737D", "#C0A43C", "#F2510E", "#651BE6", "#79806E", "#61DA5E", "#CD2F00",
            //        "#9348AF", "#01AC53", "#C5A4FB", "#996635", "#B11573", "#4BB473", "#75D89E",
            //        "#2F3F94", "#2F7B99", "#DA967D", "#34891F", "#B0D87B", "#CA4751", "#7E50A8",
            //        "#C4D647", "#E0EEB8", "#11DEC1", "#289812", "#566CA0", "#FFDBE1", "#2F1179",
            //        "#935B6D", "#916988", "#513D98", "#AEAD3A", "#9E6D71", "#4B5BDC", "#0CD36D",
            //        "#250662", "#CB5BEA", "#228916", "#AC3E1B", "#DF514A", "#539397", "#880977",
            //        "#F697C1", "#BA96CE", "#679C9D", "#C6C42C", "#5D2C52", "#48B41B", "#E1CF3B",
            //        "#5BE4F0", "#57C4D8", "#A4D17A", "#225B8", "#BE608B", "#96B00C", "#088BAF",
            //        "#F158BF", "#E145BA", "#EE91E3", "#05D371", "#5426E0", "#4834D0", "#802234",
            //        "#6749E8", "#0971F0", "#8FB413", "#B2B4F0", "#C3C89D", "#C9A941", "#41D158",
            //        "#FB21A3", "#51AED9", "#5BB32D", "#807FB", "#21538E", "#89D534", "#D36647",
            //        "#7FB411", "#0023B8", "#3B8C2A", "#986B53", "#F50422", "#983F7A", "#EA24A3",
            //        "#79352C", "#521250", "#C79ED2", "#D6DD92", "#E33E52", "#B2BE57", "#FA06EC",
            //        "#1BB699", "#6B2E5F", "#64820F", "#1C271", "#21538E", "#89D534", "#D36647",
            //        "#7FB411", "#0023B8", "#3B8C2A", "#986B53", "#F50422", "#983F7A", "#EA24A3",
            //        "#79352C", "#521250", "#C79ED2", "#D6DD92", "#E33E52", "#B2BE57", "#FA06EC",
            //        "#1BB699", "#6B2E5F", "#64820F", "#1C271", "#9CB64A", "#996C48", "#9AB9B7",
            //        "#06E052", "#E3A481", "#0EB621", "#FC458E", "#B2DB15", "#AA226D", "#792ED8",
            //        "#73872A", "#520D3A", "#CEFCB8", "#A5B3D9", "#7D1D85", "#C4FD57", "#F1AE16",
            //        "#8FE22A", "#EF6E3C", "#243EEB", "#1DC18", "#DD93FD", "#3F8473", "#E7DBCE",
            //        "#421F79", "#7A3D93", "#635F6D", "#93F2D7", "#9B5C2A", "#15B9EE", "#0F5997",
            //        "#409188", "#911E20", "#1350CE", "#10E5B1", "#FFF4D7", "#CB2582", "#CE00BE",
            //        "#32D5D6", "#17232", "#608572", "#C79BC2", "#00F87C", "#77772A", "#6995BA",
            //        "#FC6B57", "#F07815", "#8FD883", "#060E27", "#96E591", "#21D52E", "#D00043",
            //        "#B47162", "#1EC227", "#4F0F6F", "#1D1D58", "#947002", "#BDE052", "#E08C56",
            //        "#28FCFD", "#BB09B", "#36486A", "#D02E29", "#1AE6DB", "#3E464C", "#A84A8F",
            //        "#911E7E", "#3F16D9", "#0F525F", "#AC7C0A", "#B4C086", "#C9D730", "#30CC49",
            //        "#3D6751", "#FB4C03", "#640FC1", "#62C03E", "#D3493A", "#88AA0B", "#406DF9",
            //        "#615AF0", "#4BE47", "#2A3434", "#4A543F", "#79BCA0", "#A8B8D4", "#00EFD4",
            //        "#7AD236", "#7260D8", "#1DEAA7", "#06F43A", "#823C59", "#E3D94C", "#DC1C06",
            //        "#F53B2A", "#B46238", "#2DFFF6", "#A82B89", "#1A8011", "#436A9F", "#1A806A",
            //        "#4CF09D", "#C188A2", "#67EB4B", "#B308D3", "#FC7E41", "#AF3101", "#FF065",
            //        "#71B1F4", "#A2F8A5", "#E23DD0", "#D3486D", "#00F7F9", "#474893", "#3CEC35",
            //        "#1C65CB", "#5D1D0C", "#2D7D2A", "#FF3420", "#5CDD87", "#A259A4", "#E4AC44",
            //        "#1BEDE6", "#8798A4", "#D7790F", "#B2C24F", "#DE73C2", "#D70A9C", "#25B67",
            //        "#88E9B8", "#C2B0E2", "#86E98F", "#AE90E2", "#1A806B", "#436A9E", "#0EC0FF",
            //        "#F812B3", "#B17FC9", "#8D6C2F", "#D3277A", "#2CA1AE", "#9685EB", "#8A96C6",
            //        "#DBA2E6", "#76FC1B", "#608FA4", "#20F6BA", "#07D7F6", "#DCE77A", "#77ECCA"]
            //});

            $(function () {
                var pieColors = [];
                while (pieColors.length < 100) {
                    do {
                        var color = Math.floor((Math.random() * 1000000) + 1);
                    } while (pieColors.indexOf(color) >= 0);
                    pieColors.push("#" + ("000000" + color.toString(16)).slice(-6));
                }
                //var base = '#0DCAF0';
                //for (var i = 0; i < 25; i++) {
                //    pieColors.push(Highcharts.color(base).brighten((i - 7.5) / 21).get());
                //}
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
        });
            
    });
};