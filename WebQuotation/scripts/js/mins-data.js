var api_domain_url = "https://quotation.qianniusoft.com/";
var DIVID = '#report';
var chart = null;
function clickTrend() {
    var flag = $("#flag").val();
    var stock_code = $("#stock_code").val();
    var url = $("#url").val();
    var time = $("#time").val();
    var token = $("#token").val();
    $.ajax({
        url: api_domain_url + 'tools/stock_line_t.ashx',
        data: { act: "timeline", code: stock_code, flag: flag, url: url, time: time, token: token, t: new Date() },
        async: true,
        dataType: 'JSON',
        success: function (data) {
            callbackReal(data);

        },
        error: function (err) {
            //failCallback();
        }
    });
}

//图表上的成交量第一条的数据红绿的判断 是根据昨日的收盘价preclose_px 和今日的最新价last_px进行对比
//同时获取的昨日收盘价  用于涨幅的计算
var isFirstLineColorflag = true;
//保存昨收数据
var yesterdayClose;
//可以获取今开 与昨收
function callbackReal(val) {
    var res, res_info;
    if (val.succeess == "false") {
        res_info = "{quote: {time:201703300930,open:0.01,preClose:0.02,highest:0.02,lowest:0.01,price:0.02,volume:0,amount:0.01},mins: [{ price:0.01, volume:0, amount:0.01, time:930}]}";
    } else {
        res_info = val;
    }
    res = res_info;
    localStorage.datainfo = JSON.stringify(res);
    //今开
    var open_px = res.quote.open;
    open = open_px;
    //昨收
    var preclose_px = res.quote.preClose;
    high = res.quote.highest;
    low = res.quote.lowest;
    price = res.quote.price;
    yesterdayClose = preclose_px;
    isFirstLineColorflag = open_px > preclose_px ? true : false;
    //画图
    trendChart(DIVID, res);
    $(".w-loading").css("display", "none");
    $(".wrap").css("display", "block");
}


function convertDate(val, isUTC) {
    var hour = parseInt(val / 100);
    var minute = val % 100;
    var d = new Date();
    d.setHours(hour);
    d.setMinutes(minute);
    d.setSeconds(0);
    if (hour < 10) hour = "0" + hour;
    if (minute < 10) minute = "0" + minute;
    if (isUTC) {
        return Number(Date.UTC(d.getFullYear(), parseInt(d.getMonth() + 1), d.getDate(), hour, minute));
    } else {
        return new Date(d.getFullYear(), parseInt(d.getMonth() + 1), d.getDate(), d.getHours(), d.getMinutes(), d.getSeconds());
    }

    //return new Date(d.getFullYear(), parseInt(d.getMonth() + 1) , d.getDate(), d.getHours() , d.getMinutes() , d.getSeconds());
}

//拼接日期
function joinDate(val, isFull) {
    var hour = parseInt(val / 100);
    var minute = val % 100;
    var d = new Date();
    d.setHours(hour);
    d.setMinutes(minute);
    d.setSeconds(0);
    if (hour < 10) hour = "0" + hour;
    if (minute < 10) minute = "0" + minute;
    if (isFull) {
        return d.getFullYear() + "" + (d.getMonth() + 1) + "" + d.getDate() + "" + hour + "" + minute;
    } else {
        return hour + ":" + minute;
    }

}

/*
 * 5日
 */
//function clickFiveDayTrend(obj) {
//    $(obj).parent().parent().children().each(function () {
//        $(this).removeClass("hover");
//    });
//    $(obj).parent().addClass("hover");
//}
var priceyAxisMin;
var priceyAxisMax;
var percentageyAxisMin;
var percentageyAxisMax;
var volume_yAxisMin;
var volume_yAxisMax;
var red = "#ff0000";
var blue = "#00a800";
function trendChart(DIVID, data) {
    var ohlc = [],
        volume = [];
    data = data.mins;
    formatData(ohlc, volume, data);
    //将剩下的时间信息补全
    appendTimeMessage(ohlc, volume, data);
    createTrendChart(data, ohlc, volume);
};
function formatData(ohlc, volume, data) {
    //容错判断
    if (data != undefined && data != null && data.length == 0) {
        //failCallback();
        return;
    }
    // split the data set into ohlc and volume
    //数据处理
    $.each(data, function (i, v) {
        var dateUTC = convertDate(v.time, true);
        var business_amount = v.volume; //交易量
        var columnColor = red;
        if (i == 0) {//第一笔的 红绿柱 判断依据是根据 今天开盘价与昨日收盘价比较
            if (isFirstLineColorflag == false) {
                columnColor = blue;
            }
            priceyAxisMin = v.price;
            priceyAxisMax = v.price;
            percentageyAxisMin = Number(100 * (v.price / yesterdayClose - 1));
            percentageyAxisMax = Number(100 * (v.price / yesterdayClose - 1));
            volume_yAxisMin = v.volume;
            volume_yAxisMax = v.volume;
        }
        else {
            //除了第一笔，其它都是  返回的 last_px 与前一个对比
            if (data[i].price - yesterdayClose < 0) {
                columnColor = blue;
            }
            //business_amount = data[i].volume - data[i - 1].volume;
        }
        priceyAxisMin = priceyAxisMin > v.price ? v.price : priceyAxisMin;
        priceyAxisMax = priceyAxisMax > v.price ? priceyAxisMax : v.price;
        percentageyAxisMin = percentageyAxisMin > Number(100 * (v.price / yesterdayClose - 1)) ? Number(100 * (v.price / yesterdayClose - 1)) : percentageyAxisMin;
        percentageyAxisMax = percentageyAxisMax > Number(100 * (v.price / yesterdayClose - 1)) ? percentageyAxisMax : Number(100 * (v.price / yesterdayClose - 1));
        volume_yAxisMin = volume_yAxisMin > business_amount ? business_amount : volume_yAxisMin;
        volume_yAxisMax = volume_yAxisMax > business_amount ? volume_yAxisMax : business_amount;
        //将数据放入 ohlc volume 数组中
        ohlc.push({ x: dateUTC, y: Number(v.price) });
        volume.push({ x: dateUTC, y: Number(v.volume), color: columnColor });
    })
}
function createTrendChart(data, ohlc, volume) {
    var date;
    if (data.length > 0) {
        date = joinDate(data[data.length - 1].time, true);
        var dArr = new Array();
        for (var hh = 0; hh < 5; hh++) {
            var numb;
            if (hh == 0) {
                numb = Number(date.slice(0, 4));
            }
            else {
                numb = Number(date.slice((hh - 1) * 2 + 4, hh * 2 + 4));
            };
            dArr.push(numb);
        }
    }
    var last_dataTime = new Date(dArr[0], dArr[1] - 1, dArr[2], dArr[3], dArr[4]);
    var $reporting = $("#show_data");
    $reporting.html("");
    // Create the chart
    var am_startTime = new Date(last_dataTime);
    am_startTime.setHours(9, 30, 0, 0);
    var am_startTimeUTC = Number(Date.UTC(am_startTime.getFullYear(), am_startTime.getMonth(), am_startTime.getDate(), am_startTime.getHours(), am_startTime.getMinutes()));

    var am_midTime = new Date(last_dataTime);
    am_midTime.setHours(10, 30, 0, 0);
    var am_midTimeUTC = Number(Date.UTC(am_midTime.getFullYear(), am_midTime.getMonth(), am_midTime.getDate(), am_midTime.getHours(), am_midTime.getMinutes()));

    //股票交易早上最后的时间
    var am_lastTime = new Date(last_dataTime);
    am_lastTime.setHours(11, 30, 0, 0);
    var am_lastTimeUTC = Number(Date.UTC(am_lastTime.getFullYear(), am_lastTime.getMonth(), am_lastTime.getDate(), am_lastTime.getHours(), am_lastTime.getMinutes()));
    //股票交易下午最后的时间
    var pm_startTime = new Date(last_dataTime);
    pm_startTime.setHours(13, 1, 0, 0);
    var pm_startTimeUTC = Number(Date.UTC(pm_startTime.getFullYear(), pm_startTime.getMonth(), pm_startTime.getDate(), pm_startTime.getHours(), pm_startTime.getMinutes()));

    var pm_midTime = new Date(last_dataTime);
    pm_midTime.setHours(14, 0, 0, 0);
    var pm_midTimeUTC = Number(Date.UTC(pm_midTime.getFullYear(), pm_midTime.getMonth(), pm_midTime.getDate(), pm_midTime.getHours(), pm_midTime.getMinutes()));

    var pm_lastTime = new Date(last_dataTime);
    pm_lastTime.setHours(15, 0, 0, 0);
    var pm_lastTimeUTC = Number(Date.UTC(pm_lastTime.getFullYear(), pm_lastTime.getMonth(), pm_lastTime.getDate(), pm_lastTime.getHours(), pm_lastTime.getMinutes()));
    var timePositions = [am_startTimeUTC, am_midTimeUTC, am_lastTimeUTC, pm_midTimeUTC, pm_lastTimeUTC];
    //常量本地化
    Highcharts.setOptions({
        global: {
            useUTC: true
        }
    });

    //开始画图
    chart = $(DIVID).highcharts('StockChart', {
        chart: {
            renderTo: $('#report')[0],
            margin: [30, 30, 30, 30],
            plotBorderColor: '#3C94C4',
            plotBorderWidth: 0.3,
            events: {
                load: function () {
                    var chart = $(DIVID).highcharts();
                    chart.yAxis[0].addPlotLine({
                        value: yesterdayClose,
                        width: 1,
                        color: '#FFA500',
                        zIndex: 2
                    })
                }
            }

        },
        tooltip: {
            split: false,
            shared: true,
            formatter: function () {
                var me = this;
                var newtime, price, zde, zdf, volume;
                $.each(data, function (i, v) {
                    var time = convertDate(v.time, true);
                    if (me.x == time) {
                        newtime = joinDate(v.time, false);
                        price = (v.price).toFixed(2);
                        zde = (price - yesterdayClose).toFixed(2);
                        zdf = ((price - yesterdayClose) / yesterdayClose * 100).toFixed(2);
                        volume = (v.volume).toFixed(2);
                    }
                })
                var tip = '<b>' + newtime + '</b><br/>';
                tip += '昨收价：<span>' + yesterdayClose + ' </span><br/>';
                tip += '最新价：<span>' + price + ' </span><br/>';
                if (zde > 0) {
                    tip += '涨跌额：<span style="color: #DD2200;">' + zde + ' </span><br/>';
                } else {
                    tip += '涨跌额：<span style="color: #33AA11;">' + zde + ' </span><br/>';
                }
                if (zdf > 0) {
                    tip += '涨跌幅：<span style="color: #DD2200;">' + zdf + ' %</span><br/>';
                } else {
                    tip += '涨跌幅：<span style="color: #33AA11;">' + zdf + ' %</span><br/>';
                }
                if (volume > 10000) {
                    tip += "交易量：" + (volume * 0.0001).toFixed(2) + "(万张)<br/>";
                } else {
                    tip += "交易量：" + volume + "(张)<br/>";
                }
                //if (amount > 1000) {
                //    tip += "交易额：" + (amount * 0.0001).toFixed(2) + "(亿)<br/>";
                //} else {
                //    tip += "交易额：" + amount + "(万)<br/>";
                //}


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
            }
        },
        rangeSelector: {
            enabled: false,
        },
        /*导出配置*/
        exporting: {
            enabled: false,
        },
        /*创建者信息*/
        credits: {
            enabled: false,
        },
        /*下部时间拖拉选择*/
        navigator: {
            enabled: false,
            /*关闭时间选择*/
            baseseries: 0
        },
        scrollbar: {
            enabled: false /*关闭下方滚动条*/
        },
        /*底部滚动条*/
        scrollbar: {
            enabled: false
        },
        xAxis: {
            showFirstLabel: true,
            showLastLabel: true,
            scrollbar: { enabled: true },
            type: 'datetime',
            //labels: {
            //    formatter: function () {
            //        var returnTime = Highcharts.dateFormat('%H:%M ', this.value);
            //        if (returnTime == "11:30 ") {
            //            return "11:30/13:00";
            //        }
            //        return returnTime;
            //    },
            //},
            //tickPositioner: function () {

            //    var positions = [am_startTimeUTC, am_midTimeUTC, am_lastTimeUTC, pm_midTimeUTC, pm_lastTimeUTC];
            //    console.log(positions);
            //    return [am_startTimeUTC, am_midTimeUTC, am_lastTimeUTC, pm_midTimeUTC, pm_lastTimeUTC];
            //},
            gridLineWidth: 1
        },
        yAxis: [{
            opposite: false,//是否把它显示到另一边（右边）
            labels: {
                overflow: 'justify',
                align: 'right',
                x: 15,
                y: 5,
                formatter: function () {
                    //最新价  px_last/preclose昨收盘-1
                    return (this.value).toFixed(2);
                }
            },
            title: {
                text: ''
            },
            top: '0%',
            height: '75%',
            lineWidth: 1,
            showFirstLabel: true,
            showLastLabel: true,

            tickPositioner: function () {    //以yesterdayClose为界限，统一间隔值，从 最小到最大步进
                positions = [],
                    tick = Number((priceyAxisMin)),
                    increment = Number(((priceyAxisMax - priceyAxisMin) / 5));
                var tickMin = Number((priceyAxisMin)), tickMax = Number((priceyAxisMax));
                if (0 == data.length) {//还没有数据时，yesterdayClose 的幅值 在 -1% - 1%上下浮动
                    tickMin = 0.99 * yesterdayClose;
                    tickMax = 1.01 * yesterdayClose;
                } else if (0 == increment) {//有数据了  但是数据都是一样的幅值
                    if (yesterdayClose > Number(priceyAxisMin)) {
                        tickMin = Number(priceyAxisMin);
                        tickMax = 2 * yesterdayClose - Number(priceyAxisMin);
                    } else if (yesterdayClose < Number(priceyAxisMin)) {
                        tickMax = Number(priceyAxisMax);
                        tickMin = yesterdayClose - (Number(priceyAxisMax) - yesterdayClose);
                    } else {
                        tickMin = 0.99 * yesterdayClose;
                        tickMax = 1.01 * yesterdayClose;
                    }
                } else if (priceyAxisMin - yesterdayClose < 0 && priceyAxisMax - yesterdayClose > 0) {//最小值在昨日收盘价下面，最大值在昨日收盘价上面
                    var limit = Math.max(Math.abs(priceyAxisMin - yesterdayClose), Math.abs(priceyAxisMax - yesterdayClose));
                    tickMin = yesterdayClose - limit;
                    tickMax = yesterdayClose + limit;
                } else if (priceyAxisMin > yesterdayClose && priceyAxisMax > yesterdayClose) {//最小最大值均在昨日收盘价上面
                    tickMax = priceyAxisMax;
                    tickMin = yesterdayClose - (tickMax - yesterdayClose);

                } else if (priceyAxisMin < yesterdayClose && priceyAxisMax < yesterdayClose) {//最小最大值均在昨日收盘价下面
                    tickMin = priceyAxisMin;
                    tickMax = yesterdayClose + (yesterdayClose - tickMin);
                } else if (priceyAxisMax == yesterdayClose && priceyAxisMin < yesterdayClose) {//最大值等于昨日收盘价
                    var limit = Math.abs(priceyAxisMin - yesterdayClose)
                    tickMin = yesterdayClose - limit;
                    tickMax = yesterdayClose + limit;
                } else if (priceyAxisMin == yesterdayClose && priceyAxisMax > yesterdayClose) {//最小值等于昨日收盘价
                    var limit = Math.abs(priceyAxisMax - yesterdayClose)
                    tickMax = yesterdayClose + limit;
                    tickMin = yesterdayClose - limit;
                }
                if (tickMax > 2 * yesterdayClose) {//数据超过100%了
                    tickMax = 2 * yesterdayClose;
                    tickMin = 0;
                }
                var interval = Number(tickMax - yesterdayClose) / 10;
                tickMax += interval;
                tickMin = yesterdayClose - (tickMax - yesterdayClose);
                increment = (tickMax - yesterdayClose) / 2;
                tick = tickMin;
                var i = 0;
                for (tick; i++ < 5; tick += increment) {
                    positions.push(Number(tick));
                }
                return positions;
            },
        },
        {
            opposite: true,//是否把它显示到另一边（右边）
            showFirstLabel: true,
            showLastLabel: true,
            labels: {
                overflow: 'justify',
                align: 'right',
                x: 25,
                y: 5,
                formatter: function () {//涨跌幅
                    return (100 * (this.value / yesterdayClose - 1)).toFixed(2) + "%";
                }
            },
            title: {
                text: ''
            },
            lineWidth: 1,
            top: '0%',
            height: '75%',
            gridLineWidth: 1,
            tickPositioner: function () {
                return positions;
            }
        },
        {
            opposite: false,//是否把它显示到另一边（右边）
            labels: {
                overflow: 'justify',
                align: 'right',
                x: 15,
                y: 5,
                formatter: function () {
                    return this.value;

                    //if (this.value > 1000000000) {
                    //    return Number((this.value / 1000000000).toFixed(2)) + "G";
                    //} else if (this.value > 1000000) {
                    //    return Number((this.value / 1000000).toFixed(2)) + "M";
                    //} else if (this.value > 1000) {
                    //    return Number((this.value / 1000).toFixed(2)) + "K";
                    //} else {
                    //    return Number(this.value.toFixed(2));
                    //}
                }
            },
            title: {
                text: ''
            },
            top: '80%',
            height: '20%',
            width: '100%',
            offset: 0,
            lineWidth: 1,
            showFirstLabel: true,
            showLastLabel: true,
            tickPositioner: function () {
                var positions = [],
                    tickMax = volume_yAxisMax;
                //tickMin = volume_yAxisMin,
                //tick = 0,
                //increment = 0;
                //var limit = tickMax / 2;
                //tickMax += limit;
                //increment = tickMax / 2;
                //tick = 0;
                //for (tick; tick <= tickMax; tick += increment) {
                //    positions.push(Number(tick.toFixed(0)));
                //    if (increment == 0) {
                //        break;
                //    }
                //}
                positions.push(0, Number(tickMax));
                return positions;
            },
        }],
        series: [{
            name: 'AAPL Stock Price',
            data: ohlc,
            type: 'area',
            tooltip: {
                valueDecimals: 2
            },
            fillColor: {
                linearGradient: {
                    x1: 0,
                    y1: 0,
                    x2: 0,
                    y2: 1
                },
                stops: [
                    [0, Highcharts.getOptions().colors[0]],
                    [1, Highcharts.Color(Highcharts.getOptions().colors[0]).setOpacity(0).get('rgba')]
                ]
            },
            id: 'ohlc-data',
            yAxis: 0,
        },
        {
            name: 'AAPL Stock Price',
            data: ohlc,
            type: 'scatter',
            cursor: 'pointer',
            onSeries: 'candlestick',
            color: 'transparent',
            tooltip: {
                valueDecimals: 2
            },
            style: {
                fontSize: '0px',
                fontWeight: '0',
                textAlign: 'center'
            },
            id: 'ohlc-price',
            zIndex: -1000,
            yAxis: 1,
        },
        {
            type: 'column',
            name: '成交量',
            data: volume,
            dataGrouping: {
                enabled: false,
                forced: true
            },
            id: 'ohlc-colume',
            yAxis: 2,
            zIndex: -1000
        }]
    });

}
/**
 * 错误处理
 */
function failCallback() {
    var last_dataTime = new Date();
    var $reporting = $("#show_data");
    $reporting.html("");
    // Create the chart
    var am_startTime = new Date(last_dataTime);
    am_startTime.setHours(9, 30, 0, 0);
    var am_startTimeUTC = Number(Date.UTC(am_startTime.getFullYear(), am_startTime.getMonth(), am_startTime.getDate(), am_startTime.getHours(), am_startTime.getMinutes()));

    var am_midTime = new Date(last_dataTime);
    am_midTime.setHours(10, 30, 0, 0);
    var am_midTimeUTC = Number(Date.UTC(am_midTime.getFullYear(), am_midTime.getMonth(), am_midTime.getDate(), am_midTime.getHours(), am_midTime.getMinutes()));

    //股票交易早上最后的时间
    var am_lastTime = new Date(last_dataTime);
    am_lastTime.setHours(11, 30, 0, 0);
    var am_lastTimeUTC = Number(Date.UTC(am_lastTime.getFullYear(), am_lastTime.getMonth(), am_lastTime.getDate(), am_lastTime.getHours(), am_lastTime.getMinutes()));
    //股票交易下午最后的时间
    var pm_startTime = new Date(last_dataTime);
    pm_startTime.setHours(13, 1, 0, 0);
    var pm_startTimeUTC = Number(Date.UTC(pm_startTime.getFullYear(), pm_startTime.getMonth(), pm_startTime.getDate(), pm_startTime.getHours(), pm_startTime.getMinutes()));

    var pm_midTime = new Date(last_dataTime);
    pm_midTime.setHours(14, 0, 0, 0);
    var pm_midTimeUTC = Number(Date.UTC(pm_midTime.getFullYear(), pm_midTime.getMonth(), pm_midTime.getDate(), pm_midTime.getHours(), pm_midTime.getMinutes()));

    var pm_lastTime = new Date(last_dataTime);
    pm_lastTime.setHours(15, 0, 0, 0);
    var pm_lastTimeUTC = Number(Date.UTC(pm_lastTime.getFullYear(), pm_lastTime.getMonth(), pm_lastTime.getDate(), pm_lastTime.getHours(), pm_lastTime.getMinutes()));
    var data = [];
    data.push({ x: am_startTimeUTC, y: 1 });
    data.push({ x: am_midTimeUTC, y: 2 });
    data.push({ x: am_lastTimeUTC, y: 3 });
    data.push({ x: pm_midTimeUTC, y: 4 });
    data.push({ x: pm_lastTimeUTC, y: 5 });
    //常量本地化
    Highcharts.setOptions({
        global: {
            useUTC: true
        }
    });

    //开始画图
    chart = $(DIVID).highcharts('StockChart', {
        chart: {
            renderTo: "report",
            margin: [0, 0, 0, 0],
            plotBorderColor: '#3C94C4',
            plotBorderWidth: 0.3,
            events: {
                load: function () {
                    var chart = $(DIVID).highcharts();
                    chart.yAxis[0].addPlotLine({
                        value: yesterdayClose,
                        width: 1,
                        color: '#FFA500',
                        zIndex: 2
                    })
                }
            }

        },
        tooltip: {
            split: false,
            shared: true,
            formatter: function () {
                var me = this;
                var newtime, price, zde, zdf, volume;
                $.each(data, function (i, v) {
                    var time = convertDate(v.time, true);
                    if (me.x == time) {
                        newtime = convertDate(v.time, false);
                        price = (v.price).toFixed(2);
                        zde = (price - yesterdayClose).toFixed(2);
                        zdf = ((price - yesterdayClose) / yesterdayClose * 100).toFixed(2);
                        volume = (v.volume).toFixed(2);
                    }
                })

                var tip = '<b>' + newtime + '</b><br/>';
                tip += '昨收价：<span>' + yesterdayClose + ' </span><br/>';
                //if (open > zs) {
                //    tip += '开盘价：<span style="color: #DD2200;">' + open + ' </span><br/>';
                //} else {
                //    tip += '开盘价：<span style="color: #33AA11;">' + open + ' </span><br/>';
                //}
                //if (high > zs) {
                //    tip += '最高价：<span style="color: #DD2200;">' + high + ' </span><br/>';
                //} else {
                //    tip += '最高价：<span style="color: #33AA11;">' + high + ' </span><br/>';
                //}
                //if (low > zs) {
                //    tip += '最低价：<span style="color: #DD2200;">' + low + ' </span><br/>';
                //} else {
                //    tip += '最低价：<span style="color: #33AA11;">' + low + ' </span><br/>';
                //}
                if (zde > 0) {
                    tip += '涨跌额：<span style="color: #DD2200;">' + zde + ' </span><br/>';
                } else {
                    tip += '涨跌额：<span style="color: #33AA11;">' + zde + ' </span><br/>';
                }
                if (zdf > 0) {
                    tip += '涨跌幅：<span style="color: #DD2200;">' + zdf + ' %</span><br/>';
                } else {
                    tip += '涨跌幅：<span style="color: #33AA11;">' + zdf + ' %</span><br/>';
                }
                if (volume > 10000) {
                    tip += "交易量：" + (volume * 0.0001).toFixed(2) + "(万张)<br/>";
                } else {
                    tip += "交易量：" + volume + "(张)<br/>";
                }
                //if (amount > 1000) {
                //    tip += "交易额：" + (amount * 0.0001).toFixed(2) + "(亿)<br/>";
                //} else {
                //    tip += "交易额：" + amount + "(万)<br/>";
                //}


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
            }
        },
        rangeSelector: {
            enabled: false,
        },
        /*导出配置*/
        exporting: {
            enabled: false,
        },
        /*创建者信息*/
        credits: {
            enabled: false,
        },
        /*下部时间拖拉选择*/
        navigator: {
            enabled: false,
            /*关闭时间选择*/
            baseseries: 0
        },
        scrollbar: {
            enabled: false /*关闭下方滚动条*/
        },
        /*底部滚动条*/
        scrollbar: {
            enabled: false
        },
        xAxis: {
            showFirstLabel: true,
            showLastLabel: true,
            scrollbar: { enabled: true },
            type: 'datetime',
            //labels: {
            //    formatter: function () {
            //        var returnTime = Highcharts.dateFormat('%H:%M ', this.value);
            //        if (returnTime == "11:30 ") {
            //            return "11:30/13:00";
            //        }
            //        return returnTime;
            //    },
            //},
            //tickPositioner: function () {
            //    var positions = [am_startTimeUTC, am_midTimeUTC, am_lastTimeUTC, pm_midTimeUTC, pm_lastTimeUTC];
            //    return positions;
            //},
            gridLineWidth: 1,
        },
        yAxis: [{
            opposite: false,//是否把它显示到另一边（右边）
            labels: {
                overflow: 'justify',
                align: 'right',
                x: 15,
                y: 5,
                formatter: function () {
                    //最新价  px_last/preclose昨收盘-1
                    return (this.value).toFixed(2);
                }
            },
            title: {
                text: ''
            },
            top: '0%',
            height: '75%',
            lineWidth: 1,
            showFirstLabel: true,
            showLastLabel: true,

            tickPositioner: function () {    //以yesterdayClose为界限，统一间隔值，从 最小到最大步进
                positions = [],
                    tick = Number((priceyAxisMin)),
                    increment = Number(((priceyAxisMax - priceyAxisMin) / 5));
                var tickMin = Number((priceyAxisMin)), tickMax = Number((priceyAxisMax));
                if (0 == data.length) {//还没有数据时，yesterdayClose 的幅值 在 -1% - 1%上下浮动
                    tickMin = 0.99 * yesterdayClose;
                    tickMax = 1.01 * yesterdayClose;
                } else if (0 == increment) {//有数据了  但是数据都是一样的幅值
                    if (yesterdayClose > Number(priceyAxisMin)) {
                        tickMin = Number(priceyAxisMin);
                        tickMax = 2 * yesterdayClose - Number(priceyAxisMin);
                    } else if (yesterdayClose < Number(priceyAxisMin)) {
                        tickMax = Number(priceyAxisMax);
                        tickMin = yesterdayClose - (Number(priceyAxisMax) - yesterdayClose);
                    } else {
                        tickMin = 0.99 * yesterdayClose;
                        tickMax = 1.01 * yesterdayClose;
                    }
                } else if (priceyAxisMin - yesterdayClose < 0 && priceyAxisMax - yesterdayClose > 0) {//最小值在昨日收盘价下面，最大值在昨日收盘价上面
                    var limit = Math.max(Math.abs(priceyAxisMin - yesterdayClose), Math.abs(priceyAxisMax - yesterdayClose));
                    tickMin = yesterdayClose - limit;
                    tickMax = yesterdayClose + limit;
                } else if (priceyAxisMin > yesterdayClose && priceyAxisMax > yesterdayClose) {//最小最大值均在昨日收盘价上面
                    tickMax = priceyAxisMax;
                    tickMin = yesterdayClose - (tickMax - yesterdayClose);

                } else if (priceyAxisMin < yesterdayClose && priceyAxisMax < yesterdayClose) {//最小最大值均在昨日收盘价下面
                    tickMin = priceyAxisMin;
                    tickMax = yesterdayClose + (yesterdayClose - tickMin);
                }
                if (tickMax > 2 * yesterdayClose) {//数据超过100%了
                    tickMax = 2 * yesterdayClose;
                    tickMin = 0;
                }
                var interval = Number(tickMax - yesterdayClose) / 10;
                tickMax += interval;
                tickMin = yesterdayClose - (tickMax - yesterdayClose);
                increment = (tickMax - yesterdayClose) / 2;
                tick = tickMin;
                var i = 0;
                for (tick; i++ < 5; tick += increment) {
                    positions.push(Number(tick));
                }

                return positions;
            },
        },
        {
            opposite: true,//是否把它显示到另一边（右边）
            showFirstLabel: true,
            showLastLabel: true,
            labels: {
                overflow: 'justify',
                align: 'right',
                x: 25,
                y: 5,
                formatter: function () {//涨跌幅
                    return (100 * (this.value / yesterdayClose - 1)).toFixed(2) + "%";
                }
            },
            title: {
                text: ''
            },
            lineWidth: 1,
            top: '0%',
            height: '75%',
            gridLineWidth: 1,
            tickPositioner: function () {
                return positions;
            }
        },
        {
            opposite: false,//是否把它显示到另一边（右边）
            labels: {
                overflow: 'justify',
                align: 'right',
                x: 15,
                y: 5,
                formatter: function () {
                    return this.value;
                    //if (this.value > 1000000000) {
                    //    return Number((this.value / 1000000000).toFixed(2)) + "G";
                    //} else if (this.value > 1000000) {
                    //    return Number((this.value / 1000000).toFixed(2)) + "M";
                    //} else if (this.value > 1000) {
                    //    return Number((this.value / 1000).toFixed(2)) + "K";
                    //} else {
                    //    return Number(this.value.toFixed(2));
                    //}
                }
            },
            title: {
                text: ''
            },
            top: '80%',
            height: '20%',
            width: '100%',
            offset: 0,
            lineWidth: 1,
            showFirstLabel: true,
            showLastLabel: true,
            tickPositioner: function () {
                var positions = [],
                    tickMax = volume_yAxisMax;
                //tickMin = volume_yAxisMin,
                //tick = 0,
                //increment = 0;
                //var limit = tickMax / 2;
                //tickMax += limit;
                //increment = tickMax / 2;
                //tick = 0;
                //for (tick; tick <= tickMax; tick += increment) {
                //    positions.push(Number(tick.toFixed(0)));
                //    if (increment == 0) {
                //        break;
                //    }
                //}
                positions.push(0, Number(tickMax));
                return positions;
            },
        }],
        series: [{
            name: 'AAPL Stock Price',
            data: ohlc,
            type: 'area',
            tooltip: {
                valueDecimals: 2
            },
            fillColor: {
                linearGradient: {
                    x1: 0,
                    y1: 0,
                    x2: 0,
                    y2: 1
                },
                stops: [
                    [0, Highcharts.getOptions().colors[0]],
                    [1, Highcharts.Color(Highcharts.getOptions().colors[0]).setOpacity(0).get('rgba')]
                ]
            },
            yAxis: 0,
        },
        {
            name: 'AAPL Stock Price',
            data: ohlc,
            type: 'scatter',
            cursor: 'pointer',
            onSeries: 'candlestick',
            color: 'transparent',
            tooltip: {
                valueDecimals: 2
            },
            style: {
                fontSize: '0px',
                fontWeight: '0',
                textAlign: 'center'
            },
            zIndex: -1000,
            yAxis: 1,
        },
        {
            type: 'column',
            name: '成交量',
            data: volume,
            dataGrouping: {
                enabled: false,
                forced: true
            },
            yAxis: 2,
            zIndex: -1000
        }]
    });
}
/**
 * 获取日期对象，如果isUTC为true获取 日期的UTC对象，false则获取普通日期对象
 * @param date
 * @param isUTC
 * @returns
 */
function getDateUTCOrNot(date, isUTC) {
    if (!(date instanceof String)) {
        date += "";
    }
    var dArr = new Array();
    for (var hh = 0; hh < 5; hh++) {
        var numb;
        if (hh == 0) {
            numb = Number(date.slice(0, 4));
        }
        else {
            numb = Number(date.slice((hh - 1) * 2 + 4, hh * 2 + 4));
        };
        dArr.push(numb);
    }
    if (isUTC == false) {
        return new Date(dArr[0], dArr[1] - 1, dArr[2], dArr[3], dArr[4]);
    }
    var dateUTC = Number(Date.UTC(dArr[0], dArr[1] - 1, dArr[2], dArr[3], dArr[4]));//得出的UTC时间
    return dateUTC;
}

//数据补全
function appendTimeMessage(ohlc, volume, data) {
    var date = data[data.length - 1].time + "";
    var last_dataTime = convertDate(date, false);
    //股票交易早上最后的时间
    var am_lastTime = new Date(last_dataTime);
    am_lastTime.setHours(11, 30, 0, 0);
    //股票交易下午最后的时间
    var pm_startTime = new Date(last_dataTime);
    pm_startTime.setHours(13, 1, 0, 0);
    var pm_lastTime = new Date(last_dataTime);
    pm_lastTime.setHours(15, 0, 0, 0);

    //把时间日期格式转化成utc格式
    function convertDateToUTC(date) {
        return Number(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate(), date.getHours(), date.getMinutes()));
    }
    //如果获取的时间11：:30之前的计算

    if (last_dataTime < am_lastTime) {
        var i = last_dataTime;
        i.setMinutes((i.getMinutes() + 1));
        for (; i <= am_lastTime; i.setMinutes((i.getMinutes() + 1))) {
            ohlc.push({ x: convertDateToUTC(i) });
            volume.push({ x: convertDateToUTC(i) });
        }
        i = pm_startTime;
        for (; i <= pm_lastTime; i.setMinutes((i.getMinutes() + 1))) {
            ohlc.push({ x: convertDateToUTC(i) });
            volume.push({ x: convertDateToUTC(i) });
        }
    } else if (last_dataTime < pm_lastTime) {    //获取的时间下午13:00之后的计算
        var i;
        if (Number(last_dataTime) == Number(am_lastTime)) {
            i = pm_startTime;
        } else {
            i = last_dataTime;
        }
        i.setMinutes((i.getMinutes() + 1));
        for (; i <= pm_lastTime; i.setMinutes((i.getMinutes() + 1))) {
            ohlc.push({ x: convertDateToUTC(i) });
            volume.push({ x: convertDateToUTC(i) });
        }
    }
}
var getQuote = function () {
    var res, stock_code = $("#stock_code").val(), flag = $("#flag").val();
    $.ajax({
        url: api_domain_url + 'tools/stock_line_t.ashx',
        data: { act: "timeline", code: stock_code, flag: flag, t: new Date() },
        async: true,
        dataType: 'text',
        success: function (result) {
            var res_info;
            if (result.length < 32) {
                res_info = "{quote: {time:201703300930,open:0.01,preClose:0.02,highest:0.02,lowest:0.01,price:0.02,volume:0,amount:0.01},mins: [{ price:0.01, volume:0, amount:0.01, time:930}]}";
            } else {
                res_info = result;
            }
            res = res_info;
            localStorage.datainfo = JSON.stringify(res);
            //渲染canvas
            chart.paint(res);
            $(".w-loading").css("display", "none");
            $(".wrap").css("display", "block");
        }
    });
    return res;
}

var getQuoteRef = function () {
    var result = $.parseJSON(localStorage.datainfo), stock_code = $("#stock_code").val(), flag = $("#flag").val();
    var url = $("#url").val();
    var time = $("#time").val();
    var token = $("#token").val();
    var stock_info = result;
    try {
        var mins_len = parseInt(stock_info.mins.length), mins_len_last = mins_len - 1;
        $.getJSON(api_domain_url + "tools/api_ajax.ashx", { act: "stock_t", code: stock_code, flag: flag, url: url, time: time, token: token, t: new Date() }, function (data) {
            if (data == null) {
                return false;
            }
            var new_time = data.mins[mins_len_last].time;
            //判断是否交易时间
            if ((new_time > 930 && new_time < 1129) || (new_time > 1300 && new_time < 1459)) {
                //clearCanvas();
                //chart.paint(data);
                localStorage.datainfo = JSON.stringify(data);
                var ohlc = [],
                    volume = [];
                res = data;
                formatData(ohlc, volume, res.mins);
                appendTimeMessage(ohlc, volume, res.mins);
                var updateCHart = $(DIVID).highcharts();
                updateCHart.series[0].update({ data: ohlc });
                updateCHart.series[1].update({ data: ohlc });
                updateCHart.series[2].update({ data: volume });
            }
        });
    } catch (err) {
        return;
    }
}
//获取是否是交易时间
var is_trade_time = function () {
    $.getJSON(api_domain_url + "tools/api_ajax.ashx", { act: "is_trade_time", t: new Date() }, function (data) {
        //交易时间内定时刷新
        if (data.is_trade_time == 1) {
            setInterval("clickTrend()", 30 * 1000);
        }
    });
}
//清除画布
//var clearCanvas = function () {
//    var c = document.getElementById("canvas");
//    var cxt = c.getContext("2d");
//    cxt.clearRect(0, 0, c.width, c.height);
//}