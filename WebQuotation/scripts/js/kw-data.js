var api_domain_url = "https://quotation.zouhongsoft.com/";
var ohlc = [], volume = [];

function convertDate(val, withWeek) {
    var year = parseInt(val / 10000);
    var day = val % 100;
    var month = parseInt(val / 100) % 100;
    var d = new Date();
    d.setYear(year);
    d.setMonth(month - 1);
    d.setDate(day);
    if (month < 10) month = '0' + month;
    if (day < 10) day = '0' + day;
    if (withWeek) {
        return Number(Date.UTC(d.getFullYear(), parseInt(d.getMonth() + 1), d.getDate(), 0, 0));
        //var weekNames = ['日', '一', '二', '三', '四', '五', '六'];
        //return year + '-' + month + '-' + day + '(星期' + weekNames[d.getDay()] + ')';
    }
    else {
        return year + '-' + month + '-' + day;
    }
}

function getKLData() {
    var stock_code = $("#stock_code").val(), flag = $("#flag").val();
    var flag = $("#flag").val();
    var stock_code = $("#stock_code").val();
    var url = $("#url").val();
    var time = $("#time").val();
    var token = $("#token").val();
    var result = {};
    var ks = [];
    var open, high, low, close, y, zde, zdf, hsl, MA5, MA10, MA20, MA30, zs, relativeWidth, amount;
    $.ajax({
        url: api_domain_url + 'tools/stock_line_kw.ashx',
        data: { act: "timeline", code: stock_code, flag: flag, url: url, time: time, token: token, t: new Date() },
        async: false,
        dataType: 'JSON',
        success: function (data) {
            var res_info = data;
            var res = eval(res_info);
            localStorage.kdatainfo = JSON.stringify(res);
            ohlc = [];
            volume = [];
            var groupingUnits = [[
                'week',                         // unit name
                [1]                             // allowed multiples
            ], [
                'month',
                [1, 2, 3, 4, 6]
            ]];

            if (data.kmins) {

                var res = eval(data.kmins);
                $.each(res, function (i, v) {
                    ohlc.push([
                        convertDate(v.time,true), // the date
                        v.open, // open
                        v.highest, // high
                        v.lowest, // low
                        v.preClose // close
                    ]);
                    volume.push([
                        convertDate(v.time,true), // the date
                        v.volume // the volume
                    ]);
                });
                // create the chart
                var chart = Highcharts.stockChart('report', {
                    chart: {
                        renderTo: $('#report')[0],
                        margin: [30, 30, 30, 30],
                        plotBorderColor: '#3C94C4',
                        plotBorderWidth: 0.3

                    },
                    navigator: {
                        enabled: true,
                        height: 15
                    },
                    scrollbar: {
                        enabled: false
                    },
                    rangeSelector: {
                        enabled: false
                    },
                    rangeSelector: {
                        enabled: true,
                        buttons: [{
                            type: "month",
                            count: 3,
                            text: "周k"
                        }],
                        selected: 0,
                        inputEnabled: false,
                        inputDateFormat: '%Y-%m-%d'  //设置右上角的日期格式
                    },
                    credits: {
                        enabled: false
                    },
                    xAxis: {
                        type: 'datetime',
                        tickLength: 1,//X轴下标长度
                        // minRange: 3600 * 1000*24*30, // one month
                        dateTimeLabelFormats: {
                            millisecond: '%H:%M:%S.%L',
                            second: '%H:%M:%S',
                            minute: '%H:%M',
                            hour: '%H:%M',
                            day: '%Y-%m-%d',
                            week: '%m-%d',
                            month: '%y-%m',
                            year: '%Y'
                        }
                    },
                    //格式化悬浮框
                    tooltip: {
                        split: false,
                        shared: true,
                        formatter: function () {
                            var me = this;
                            var nowtime;
                            if (this.y == undefined) {
                                return;
                            }
                            data = JSON.parse(localStorage.kdatainfo);
                            $.each(data.kmins, function (i, v) {
                                var time = convertDate(v.time, true);
                                if (me.x == time) {
                                    nowtime = v.time;
                                    //zdf = parseFloat(data[i][7]).toFixed(2);
                                    //zde = parseFloat(data[i][8]).toFixed(2);
                                    //hsl = parseFloat(data[i][9]).toFixed(2);
                                    zs = parseFloat(v.preClose);
                                    amount = parseFloat(v.amount);

                                }
                            });
                            open = me.points[0].point.open;
                            high = me.points[0].point.high;
                            low = me.points[0].point.low;
                            close = me.points[0].point.close;
                            y = (me.points[1].point.y * 0.0001).toFixed(2);
                            amount = (amount * 0.0001).toFixed(2);
                            //MA5 = this.points[2].y.toFixed(2);
                            //MA10 = this.points[3].y.toFixed(2);
                            //MA30 = this.points[4].y.toFixed(2);
                            relativeWidth = me.points[0].point.shapeArgs.x;
                            var stockName = me.points[0].series.name;
                            var tip = '<b>' + convertDate(nowtime,false) + '</b><br/>';
                            //tip += stockName + "<br/>";
                            tip += '昨收价：<span>' + close + ' </span><br/>';
                            if (open > zs) {
                                tip += '开盘价：<span style="color: #DD2200;">' + open + ' </span><br/>';
                            } else {
                                tip += '开盘价：<span style="color: #33AA11;">' + open + ' </span><br/>';
                            }
                            if (high > zs) {
                                tip += '最高价：<span style="color: #DD2200;">' + high + ' </span><br/>';
                            } else {
                                tip += '最高价：<span style="color: #33AA11;">' + high + ' </span><br/>';
                            }
                            if (low > zs) {
                                tip += '最低价：<span style="color: #DD2200;">' + low + ' </span><br/>';
                            } else {
                                tip += '最低价：<span style="color: #33AA11;">' + low + ' </span><br/>';
                            }
                            //if (close > zs) {
                            //    tip += '昨收价：<span style="color: #DD2200;">' + close + ' </span><br/>';
                            //} else {
                            //    tip += '昨收价：<span style="color: #33AA11;">' + close + ' </span><br/>';
                            //}
                            //if (zde > 0) {
                            //    tip += '涨跌额：<span style="color: #DD2200;">' + zde + ' </span><br/>';
                            //} else {
                            //    tip += '涨跌额：<span style="color: #33AA11;">' + zde + ' </span><br/>';
                            //}
                            //if (zdf > 0) {
                            //    tip += '涨跌幅：<span style="color: #DD2200;">' + zdf + ' </span><br/>';
                            //} else {
                            //    tip += '涨跌幅：<span style="color: #33AA11;">' + zdf + ' </span><br/>';
                            //}
                            if (y > 10000) {
                                tip += "交易量：" + (y * 0.0001).toFixed(2) + "(亿股)<br/>";
                            } else {
                                tip += "交易量：" + y + "(万股)<br/>";
                            }
                            if (amount > 1000) {
                                tip += "交易额：" + (amount * 0.0001).toFixed(2) + "(亿)<br/>";
                            } else {
                                tip += "交易额：" + amount + "(万)<br/>";
                            }


                            /* tip += "换手率："+hsl+"<br/>";*/
                            //$reporting.html(
                            //    '  <span style="font-weight:bold">' + stockName + '</span>'
                            //    + '  <span>开盘:</span>' + open
                            //    + '  <span>收盘:</span>' + close
                            //    + '  <span>最高:</span>' + high
                            //    + '  <span>最低:</span>' + low
                            //    + '  <span style="padding-left:25px;"> </span>' + Highcharts.dateFormat('%Y-%m-%d', this.x)
                            //);
                            return tip;
                        },
                    },
                    yAxis: [{
                        labels: {
                            align: 'right',
                            x: -3
                        },
                        title: {
                            text: '价格'
                        },
                        top: '0%',
                        height: '65%',
                        resize: {
                            enabled: true
                        },
                        lineWidth: 1
                    }, {
                        labels: {
                            align: 'right',
                            x: -3
                        },
                        title: {
                            text: '成交量'
                        },
                        top: '75%',
                        height: '25%',
                        offset: 0
                    }],
                    series: [{
                        type: 'candlestick',
                        color: 'green',
                        lineColor: 'green',
                        upColor: 'red',
                        upLineColor: 'red',
                        tooltip: {
                        },
                        navigatorOptions: {
                            color: Highcharts.getOptions().colors[0]
                        },
                        data: ohlc,
                        dataGrouping: {
                            units: groupingUnits
                        },
                        id: 'sz'
                    }, {
                        type: 'column',
                        data: volume,
                        yAxis: 1,
                        dataGrouping: {
                            units: groupingUnits
                        },
                    }]
                });
            } else {
                alert('读取股票数据失败！');
                return false;
            }
        }
    });
    $(".w-loading").css("display", "none");
}
//function getKLData() {
//    var stock_code = $("#stock_code").val(), flag = $("#flag").val();
//    var result = {};
//    var ks = [];
//    $.ajax({
//        url: api_domain_url + 'tools/stock_line_kw.ashx',
//        data: { act: "timeline", code: stock_code, flag: flag, t: new Date() },
//        async: false,
//        dataType: 'json',
//        success: function (data) {
//            $.each(data.kmins, function (i, v) {
//                var item = {
//                    quoteTime: v.time,
//                    preClose: v.preClose,
//                    open: v.open,
//                    high: v.highest,
//                    low: v.lowest,
//                    close: v.price,
//                    volume: v.volume,
//                    amount: v.amount
//                };
//                if (ks.length == 0) {
//                    result.low = item.low;
//                    result.high = item.high;
//                } else {
//                    result.high = Math.max(result.high, item.high);
//                    result.low = Math.min(result.low, item.low);
//                }
//                ks.push(item);
//            });
//        }
//    });
//    result.ks = ks;
//    $(".w-loading").css("display", "none");
//    $(".wrap").css("display", "block");
//    return result;
//}
//获取是否是交易时间
var is_trade_time = function () {
    $.getJSON(api_domain_url + "tools/api_ajax.ashx", { act: "is_trade_time", t: new Date() }, function (data) {
        //交易时间内定时刷新
        if (data.is_trade_time == 1) {
            //setInterval("drawKLRef()", 0.5 * 60 * 1000);
        }
    });
}