﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="t.aspx.cs" Inherits="WebQuotation.line.black.t" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width,initial-scale=1,minimum-scale=1,maximum-scale=1,user-scalable=no" />
    <meta name="apple-mobile-web-app-capable" content="yes">
    <meta name="apple-mobile-web-app-status-bar-style" content="black">
    <meta name="format-detection" content="telephone=no">
    <title>分时图</title>
    <link href="//js.zouhongsoft.com/layout.css" rel="stylesheet" />
</head>
<body style="margin: 0; padding: 0;">
    <div class="w-loading"><b class="w-loading-ico"></b></div>
    <div class="wrap">
        <canvas id="canvas" width="414" height="480" style="z-index: 3; background: transparent; margin: 0 auto; height: 240px;">
            <p>
                你的浏览器不支持html5哟
            </p>
        </canvas>
    </div>
    <input id="stock_code" type="hidden" value="<%=stock_code%>" />
    <input id="flag" type="hidden" value="<%=flag%>" />
    <input id="url" type="hidden" value="<%=url%>" />
    <input id="time" type="hidden" value="<%=time%>" />
    <input id="token" type="hidden" value="<%=token%>" />
    <input id="hidDataInfo" type="hidden" value="" />
    <script type="text/javascript" src="//js.zouhongsoft.com/scripts/jquery-1.11.2.min.js"></script>
    <%if (Common.Utils.IsTradeTimeCK() == 1)
        { %>
    <script src="//js.zouhongsoft.com/scripts/js/absPainter.js"></script>
    <script src="//js.zouhongsoft.com/scripts/js/absPainterArea.js"></script>
    <script src="//js.zouhongsoft.com/scripts/js/util.js"></script>
    <script src="//js.zouhongsoft.com/scripts/js/absPainter.js"></script>
    <script src="//js.zouhongsoft.com/scripts/js/ajax.js"></script>
    <script src="//js.zouhongsoft.com/scripts/js/crossLines.js"></script>
    <script src="//js.zouhongsoft.com/scripts/js/axis-x.js"></script>
    <script src="//js.zouhongsoft.com/scripts/js/linepainter.js"></script>
    <script src="//js.zouhongsoft.com/scripts/js/linepainter_l.js"></script>
    <script src="//js.zouhongsoft.com/scripts/js/volumePainter.js"></script>
    <script src="//js.zouhongsoft.com/scripts/js/axis-y.js"></script>
    <script src="//js.zouhongsoft.com/scripts/js/chartEventHelper.js"></script>
    <script src="//js.zouhongsoft.com/scripts/js/t-data-black.js"></script>
    <%}
        else
        { %>
    <script src="//js.zouhongsoft.com/scripts/js/absPainter.js"></script>
    <script src="//js.zouhongsoft.com/scripts/js/absPainterArea.js"></script>
    <script src="//js.zouhongsoft.com/scripts/js/util.js"></script>
    <script src="//js.zouhongsoft.com/scripts/js/absPainter.js"></script>
    <script src="//js.zouhongsoft.com/scripts/js/ajax.js"></script>
    <script src="//js.zouhongsoft.com/scripts/js/crossLines.js"></script>
    <script src="//js.zouhongsoft.com/scripts/js/axis-x.js"></script>
    <script src="//js.zouhongsoft.com/scripts/js/linepainter.js"></script>
    <script src="//js.zouhongsoft.com/scripts/js/linepainter_l.js"></script>
    <script src="//js.zouhongsoft.com/scripts/js/volumePainter.js"></script>
    <script src="//js.zouhongsoft.com/scripts/js/axis-y.js"></script>
    <script src="//js.zouhongsoft.com/scripts/js/chartEventHelper.js"></script>
    <script src="//js.zouhongsoft.com/scripts/js/t-data-black.js"></script>
    <%} %>
    <!---->
    <script type="text/javascript">
        var setWidth = $(window).width();
        if (setWidth > 750) {
            setWidth = 750;
        }
        $("#canvas").attr("width", setWidth * 2);
        $("#canvas").width(setWidth);
        $("#wrap").width(setWidth);
        var wid_px;

        wid_px = {
            w1: setWidth - 99,
            w2: setWidth - 40.5,
        }
        /*var setHeight = $("#canvas").hight();
        var mSetHeight = setHeight * 0.8;
        var sSetHeight = setHeight * 0.15;*/


        function line(ctx, x0, y0, x1, y1, color, width) {
            ctx.beginPath();
            ctx.moveTo(x0, y0);
            ctx.lineTo(x1, y1);
            ctx.strokeStyle = color;
            ctx.lineWidth = width || 1;
            ctx.stroke();
        }

        function getMinTime(minIndex) {
            //上午09：30-11：30
            //下午13：00-15：00
            var d = new Date();
            if (minIndex <= 120) {
                d.setHours(9, 30, 30);
                d = new Date(d.getTime() + (minIndex) * 60 * 1000);
            } else {
                d.setHours(13, 0, 0);
                d = new Date(d.getTime() + (minIndex - 120) * 60 * 1000);
            }

            var hour = d.getHours() > 9 ? new String(d.getHours()) : '0' + d.getHours();
            var minutes = d.getMinutes() > 9 ? new String(d.getMinutes()) : '0' + d.getMinutes();
            var seconds = '30';
            return hour + '' + minutes + seconds;
        }

        function Tip(options) {
            this.options = options;
            this.canvas = options.canvas;
            this.canvas.tip = this;
        }

        Tip.prototype = {
            show: function (relativePoint, html) {
                var dc = this.dataContext;
                var painter = this.canvas.painter;
                if (dc) {
                    if (dc.isNewQuote) painter.fillTopText(dc.data);
                    else painter.fillTopText(dc.data, dc.index);
                }
            },
            update: function (relativePoint, html) {
                this.show(relativePoint, html);
            },
            hide: function () {
                var dc = this.dataContext;
                var painter = this.canvas.painter;
                if (dc) {
                    painter.fillTopText(dc.data);
                }
            }
        };

        function minsChart(canvasId, options) {
            extendObject(options, this);
            this.canvas = $id(canvasId);
            this.ctx = this.canvas.getContext('2d');
            this.canvas.painter = this;
        }

        minsChart.prototype = {
            paint: function (data) {
                this.fillTopText(data);
                this.paintChart(data);
                this.paintxAxis();
                this.fillBottomText(data);
                this.paintVolume(data);
            },

            paintVolume: function (data) {
                var ctx = this.ctx;
                var options = this.volume;
                ctx.beginPath();
                ctx.rect(options.region.x, options.region.y, options.region.width, options.region.height);
                ctx.strokeStyle = options.borderColor;
                ctx.stroke();
                line(ctx, options.region.x, options.region.y + options.region.height / 2, options.region.x + options.region.width, options.region.y + options.region.height / 2, options.splitLineColor);
                options.getDataLength = function () { return this.data.items.length; };
                options.maxDotsCount = this.maxDotsCount;
                var volumePainterImp = new volumePainter(options);
                var painter = new Painter(this.canvas.id, volumePainterImp, { items: data.mins });
                painter.paint();

                var max = painter.maxVolume;
                var unit = "手";
                var scalers = [max.toFixed(0), (max / 2).toFixed(0), '(' + unit + ')'];
                var yscaler = new yAxis(this.volume.yScaler);
                var painter = new Painter(this.canvas.id, yscaler, scalers);
                painter.paint();
            },

            fillBottomText: function (data) {
                if (!this.bottomText) return;
                //高9999 低9999 成交888999
                var ctx = this.ctx;
                var txt = '高';
                var options = this.bottomText;
                ctx.font = options.font;
                ctx.fillStyle = options.color;
                var w = ctx.measureText(txt).width;
                ctx.fillText(txt, options.region.x, options.region.y);
                var x = options.region.x + w;
                var quote = data.quote;
                var me = this;

                function getTxtColor(val) { return val > quote.preClose ? me.riseColor : (val == quote.preClose ? me.normalColor : me.fallColor); }
                var highColor = getTxtColor(quote.highest);
                var high = toMoney(quote.highest);
                ctx.fillStyle = highColor;
                w = ctx.measureText(high).width;
                ctx.fillText(high, x, options.region.y);
                x += w;
                txt = ' 低';
                ctx.fillStyle = options.color;
                w = ctx.measureText(txt).width;
                ctx.fillText(txt, x, options.region.y);
                x += w;
                var lowColor = getTxtColor(quote.lowest);
                var low = toMoney(quote.lowest);
                w = ctx.measureText(low).width;
                ctx.fillStyle = lowColor;
                ctx.fillText(low, x, options.region.y);
                x += w;
                ctx.fillStyle = options.color;
                var amount = ' 成交' + bigNumberToText(quote.amount);
                ctx.fillText(amount, x, options.region.y);
            },

            paintxAxis: function () {
                var xAxisImpl = new xAxis(this.xScaler);
                var xAxisPainter = new Painter(this.canvas.id, xAxisImpl, this.xScaler.data);
                xAxisPainter.paint();
            },

            paintChart: function (data) {
                var minsChartOptions = this.minsChart;
                var region = this.minsChart.region;
                var ctx = this.ctx;
                ctx.beginPath();
                ctx.strokeStyle = minsChartOptions.borderColor;
                ctx.rect(region.x, region.y, region.width, region.height);
                ctx.stroke();

                //水平线
                var middleIndex = (this.minsChart.horizontalLineCount + this.minsChart.horizontalLineCount % 2) / 2;
                var splitCount = this.minsChart.horizontalLineCount + 1;
                for (var i = 1; i <= this.minsChart.horizontalLineCount; i++) {
                    var color = (i == middleIndex ? minsChartOptions.middleLineColor : minsChartOptions.otherSplitLineColor);
                    var y = region.y + region.height * i / splitCount;
                    line(ctx, region.x, y, region.x + region.width, y, color);
                }
                //垂直线
                splitCount = this.minsChart.verticalLineCount + 1;
                for (var i = 1; i <= this.minsChart.verticalLineCount; i++) {
                    var x = region.x + region.width * i / splitCount;
                    line(ctx, x, region.y, x, region.y + region.height, minsChartOptions.otherSplitLineColor);
                }

                //价格线
                var lineOptions = {
                    region: region,
                    maxDotsCount: this.maxDotsCount,
                    getDataLength: function () { return this.data.items.length; },
                    getItemValue: function (item) { return item.price; },
                    middleValue: data.quote.preClose, //通常是昨收
                    lineColor: minsChartOptions.priceLineColor,
                    gradientColor: '#143841',
                    paintArea: true
                };
                var linePainterImp = new linePainter(lineOptions);
                var priceLinePainter = new PainterArea(this.canvas.id, linePainterImp, { items: data.mins });
                priceLinePainter.paint();

                //y轴
                var yOptions = this.minsChart.yScalerLeft;
                var preClose = data.quote.preClose;
                var me = this;
                yOptions.color = function (val) {
                    return val > preClose ? me.riseColor : (val == preClose ? me.normalColor : me.fallColor);
                };
                var scalersLeft = [];
                var scalersRight = [];
                var min = preClose - priceLinePainter.maxDiff;
                var space = priceLinePainter.maxDiff * 2 / (this.minsChart.horizontalLineCount + 1);
                for (var i = this.minsChart.horizontalLineCount + 1; i >= 0; i--) {
                    var val = min + i * space;
                    scalersLeft.push(val.toFixed(2));
                    var percent = (val - preClose) * 100 / preClose;
                    scalersRight.push(percent.toFixed(2) + '%');
                }
                var yx = new yAxis(yOptions);
                var yAxisPainter = new Painter(this.canvas.id, yx, scalersLeft);
                yAxisPainter.paint();

                var yPercentOptions = this.minsChart.yScalerRight;
                yPercentOptions.color = function (val) {
                    return (val == '0.00%' ? 'gray' : (val.charAt(0) == '-' ? 'green' : 'red'));
                };
                var yxPercent = new yAxis(yPercentOptions);
                var yxPercentPainter = new Painter(this.canvas.id, yxPercent, scalersRight);
                yxPercentPainter.paint();

                //均线
                if (this.needPaintAvgPriceLine) {
                    //生成移动均线数据
                    var items = [];
                    var totalVolume = 0;
                    var totalAmount = 0;
                    data.mins.each(function (item) {
                        totalVolume += item.volume;
                        totalAmount += item.amount;
                        items.push(totalAmount / totalVolume);
                    });
                    lineOptions.lineColor = minsChartOptions.avgPriceLineColor;
                    lineOptions.getItemValue = function (item) { return item; };
                    linePainterImp = new linePainter(lineOptions);
                    var painterAvg = new Painter(this.canvas.id, linePainterImp, { items: items });
                    painterAvg.paint();
                }

                var me = this;
                var chartRegion = me.minsChart.region;

                function getY(x) {
                    var index = Math.ceil((x - me.minsChart.region.x / 2) * me.maxDotsCount / me.minsChart.region.width) * 2;
                    var val;
                    var isNewQuote;
                    if (index >= 0 && index < data.mins.length) {
                        val = data.mins[index].price;
                        isNewQuote = false;
                    } else {
                        val = data.quote.price;
                        isNewQuote = true;
                    }

                    if (me.canvas.tip) me.canvas.tip.dataContext = { data: data, isNewQuote: isNewQuote, index: index };
                    var diff = val - preClose;
                    var middleY = (me.minsChart.region.y + me.minsChart.region.height / 2) / 2;
                    return middleY - diff * me.minsChart.region.height / 2 / priceLinePainter.maxDiff / 2;
                }

                //添加鼠标事件
                addCrossLinesAndTipEvents(this.canvas, {
                    getCrossPoint: function (ev) {
                        return { x: ev.offsetX, y: getY(ev.offsetX) };
                    },
                    triggerEventRanges: { x: chartRegion.x / 2, y: chartRegion.y / 2, width: chartRegion.width / 2, height: (me.volume.region.y + me.volume.region.height - chartRegion.y) / 2 },
                    tipOptions: {
                        getTipHtml: function (ev) { return null; },
                        position: { x: false, y: false }
                    },
                    crossLineOptions: {
                        color: '#999'
                    }
                });
            },
            fillTopText: function (data, minIndex) {
                var quote = data.quote;
                var ctx = this.ctx;
                var topText = this.topText;
                var region = topText.region;
                ctx.clearRect(region.x, region.y, region.width, region.height);
                if (topText.backgroundColor) {
                    ctx.beginPath();
                    ctx.fillStyle = topText.backgroundColor;
                    ctx.rect(topText.region.x, topText.region.y, topText.region.width, topText.region.height);
                    ctx.fill();
                    //ctx.closePath();
                }
                var price;
                var time;
                if (typeof minIndex == 'undefined') {
                    price = quote.price;
                    time = quote.time;
                } else {
                    price = data.mins[minIndex].price;
                    time = quote.time.toString().substr(0, 8) + getMinTime(minIndex);
                }

                ctx.fillStyle = topText.color;
                ctx.font = topText.font;
                if (topText.textBaseline) ctx.textBaseline = topText.textBaseline;
                var txt = quote.stock_name + ' 最新' + toMoney(price);
                var width = ctx.measureText(txt).width;
                ctx.fillText(txt, topText.region.x, topText.region.y);

                var isRise = price > quote.preClose;
                var isEqual = price == quote.preClose;
                var isFall = price < quote.preClose;
                var diff = toMoney(price - quote.preClose);
                var txtRiseFall = (isRise ? '↑' : (isFall ? '↓' : '')) + diff +
                    ('(') +
                    toMoney(diff * 100 / quote.preClose) +
                    '%)';

                var x = topText.region.x + width;
                ctx.fillStyle = isRise ? this.riseColor : (isFall ? this.fallColor : this.normalColor);
                ctx.fillText(txtRiseFall, x, topText.region.y);

                var temp = new String(time);
                var txtTime = temp.charAt(8) + temp.charAt(9) + ':' + temp.charAt(10) + temp.charAt(11);
                ctx.fillStyle = topText.color;
                var timeWidth = ctx.measureText(txtTime).width;
                ctx.fillText(txtTime, topText.region.x + topText.region.width - timeWidth, topText.region.y);
            }
        };
        var chart = new minsChart('canvas', {
            fallColor: '#2c7e2e',
            riseColor: '#c2382e',
            normalColor: '#999',
            maxDotsCount: 241,
            needPaintAvgPriceLine: false,
            backgroundColor: '#1f1f26',
            topText: { font: '20px 宋体', color: '#999', region: { x: 58.5 * 2, y: 5.5 * 2, width: wid_px.w1 * 2, height: 14 * 2 }, textBaseline: 'top', backgroundColor: '#1f1f26' },
            minsChart: {
                region: { x: 56.5 * 2, y: 21.5 * 2, width: wid_px.w1 * 2, height: 150 * 2 },
                priceLineColor: '#36e3f7', avgPriceLineColor: '#36e3f7', middleLineColor: '#e0a40c', otherSplitLineColor: '#2b2b33', borderColor: '#2b2b33',
                horizontalLineCount: 7, verticalLineCount: 3,
                yScalerLeft: { font: '20px Arial', region: { x: .5 * 2, y: 20 * 2, width: 50.5 * 2, height: 150 * 2 }, align: 'right', fontHeight: 9 * 2, textBaseline: 'top' },
                yScalerRight: { font: '20px Arial', region: { x: wid_px.w2 * 2, y: 20 * 2, width: 40.5 * 2, height: 150 * 2 }, align: 'right', fontHeight: 9 * 2, textBaseline: 'top' }
            },
            xScaler: {
                font: '20px Arial', color: '#999',
                region: { x: 56.5 * 2, y: 175 * 2, width: wid_px.w1 * 2, height: 15 * 2 },
                data: ['09:30', '10:30', '11:30/13:00', '14:00', '15:00']
            },

            //bottomText: { font: '11px 宋体', color: 'black', region: { x: 5.5, y: 260, width: 400, height: 20} },
            volume: {
                region: { x: 56.5 * 2, y: 190.5 * 2, width: wid_px.w1 * 2, height: 40 * 2 },
                bar: { color: '#32a532', width: 2 * 2 },
                borderColor: '#2b2b33', splitLineColor: '#2b2b33',
                yScaler: { font: '20px Arial', region: { x: .5 * 2, y: 187.5 * 2, width: 50.5 * 2, height: 40 * 2 }, color: '#999', align: 'right', fontHeight: 12 * 2, textBaseline: 'top' }
            }
        });
        //判断交易时间
        is_trade_time();
        //渲染canvas
        getQuote();
    </script>
    <span style="display: none;">
        <script src="https://s96.cnzz.com/z_stat.php?id=1275703377&web_id=1275703377" language="JavaScript"></script>
    </span>
</body>
</html>
