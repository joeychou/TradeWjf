﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="km.aspx.cs" Inherits="WebQuotation.line.black.km" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="author" content="joeychou.me" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=0" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <meta name="apple-mobile-web-app-status-bar-style" content="black" />
    <meta name="format-detection" content="telephone=no" />
    <title>k线图手机版</title>
    <link href="//js.zouhongsoft.com/layout.css" rel="stylesheet" />
</head>
<body onload="drawKL()" style="padding: 0; margin: 0px">
    <div class="w-loading"><b class="w-loading-ico"></b></div>
    <div class="wrap">
        <canvas id="canvasKL" width="" height="480" style="z-index: 2; height: 240px;">
            <p>hey,您的浏览器不支持html5，换个浏览器吧，比如google chrome，firefox</p>
        </canvas>
        <div id="debug"></div>
    </div>
    <input id="stock_code" type="hidden" value="<%=stock_code%>" />
    <input id="flag" type="hidden" value="<%=flag%>" />
    <input id="url" type="hidden" value="<%=url%>" />
    <input id="time" type="hidden" value="<%=time%>" />
    <input id="token" type="hidden" value="<%=token%>" />
    <input id="hidDataInfo" type="hidden" value="" />
    <input id="ranges" type="hidden" value="" />
    <script type="text/javascript" src="//js.zouhongsoft.com/scripts/jquery-1.11.2.min.js"></script>
    <%if (Common.Utils.IsTradeTimeCK() == 1)
        { %>
    <script src="//js.zouhongsoft.com/scripts/js/km-data-s.js"></script>
    <script src="//js.zouhongsoft.com/scripts/libs/loading.js"></script>
    <script src="//js.zouhongsoft.com/scripts/libs/util.js"></script>
    <script src="//js.zouhongsoft.com/scripts/libs/absPainter.js"></script>
    <script src="//js.zouhongsoft.com/scripts/libs/ajax.js"></script>
    <script src="//js.zouhongsoft.com/scripts/libs/crossLines.js"></script>
    <script src="//js.zouhongsoft.com/scripts/libs/axis-x.js"></script>
    <script src="//js.zouhongsoft.com/scripts/libs/tip.js"></script>
    <script src="//js.zouhongsoft.com/scripts/libs/linepainter.js"></script>
    <script src="//js.zouhongsoft.com/scripts/libs/volumePainter.js"></script>
    <script src="//js.zouhongsoft.com/scripts/libs/axis-y.js"></script>
    <script src="//js.zouhongsoft.com/scripts/libs/chartEventHelper.kl.js?a2"></script>
    <script src="//js.zouhongsoft.com/scripts/libs/controller.js"></script>
    <%}
        else
        { %>
    <script src="//js.zouhongsoft.com/scripts/js/km-data-s.js"></script>
    <script src="//js.zouhongsoft.com/scripts/libs/loading.js"></script>
    <script src="//js.zouhongsoft.com/scripts/libs/util.js"></script>
    <script src="//js.zouhongsoft.com/scripts/libs/absPainter.js"></script>
    <script src="//js.zouhongsoft.com/scripts/libs/ajax.js"></script>
    <script src="//js.zouhongsoft.com/scripts/libs/crossLines.js"></script>
    <script src="//js.zouhongsoft.com/scripts/libs/axis-x.js"></script>
    <script src="//js.zouhongsoft.com/scripts/libs/tip.js"></script>
    <script src="//js.zouhongsoft.com/scripts/libs/linepainter.js"></script>
    <script src="//js.zouhongsoft.com/scripts/libs/volumePainter.js"></script>
    <script src="//js.zouhongsoft.com/scripts/libs/axis-y.js"></script>
    <script src="//js.zouhongsoft.com/scripts/libs/chartEventHelper.kl.js?a2"></script>
    <script src="//js.zouhongsoft.com/scripts/libs/controller.js"></script>
    <%} %>
    <script type="text/javascript">
        function convertDate(val, withWeek) {
            var year = Math.ceil(val / 10000) - 1;
            var day = val % 100;
            var month = (Math.ceil(val / 100) - 1) % 100;
            var d = new Date();
            d.setYear(year);
            d.setMonth(month - 1);
            d.setDate(day);
            if (month < 10) month = '0' + month;
            if (day < 10) day = '0' + day;
            if (withWeek) {
                var weekNames = ['日', '一', '二', '三', '四', '五', '六'];
                return year + '-' + month + '-' + day + '(星期' + weekNames[d.getDay()] + ')';
            } else {
                return year + '-' + month + '-' + day;
            }
        }

        function calcMAPrices(ks, startIndex, count, daysCn) {
            var result = new Array();
            for (var i = startIndex; i < startIndex + count; i++) {
                var startCalcIndex = i - daysCn + 1;
                if (startCalcIndex < 0) {
                    result.push(false);
                    continue;
                }
                var sum = 0;
                for (var k = startCalcIndex; k <= i; k++) {
                    sum += ks[k].close;
                }
                var val = sum / daysCn;
                result.push(val);
            }
            return result;
        }

        var timer = {
            start: function (step) {
                this.startTime = new Date();
                this.stepName = step;
            },
            stop: function () {
                var timeSpan = new Date().getTime() - this.startTime.getTime();
                setDebugMsg(this.stepName + '耗时' + timeSpan + 'ms');
            }
        };

        function kLine(options) {
            this.options = options;
            this.dataRanges = null;
        }

        kLine.prototype = {
            initialize: function (painter) {
                painter.klOptions = this.options;
                painter.implement = this;
            },
            start: function () {
                //timer.start('start');
                var canvas = this.canvas;
                var ctx = this.ctx;
                this.painting = true;
                var options = this.klOptions;
                var clearPart = { width: canvas.width, height: options.priceLine.region.y - 3 };
                ctx.clearRect(0, 0, clearPart.width, clearPart.height);

                ctx.save();
                window.riseColor = options.riseColor;
                window.fallColor = options.fallColor;
                window.normalColor = options.normalColor;
                if (options.backgroundColor && !this.drawnBackground) {
                    ctx.beginPath();
                    ctx.fillStyle = options.backgroundColor;
                    ctx.rect(0, 0, clearPart.width, clearPart.height);
                    ctx.fill();
                    //ctx.closePath();
                    this.drawnBackground = true;
                }
                ctx.translate(options.region.x, options.region.y);
                ctx.strokeStyle = options.borderColor;
                ctx.beginPath();
                ctx.rect(0, 0, options.region.width, options.region.height);
                ctx.stroke();
                //画水平底纹线
                var spaceHeight = options.region.height / (options.horizontalLineCount + 1);
                for (var i = 1; i <= options.horizontalLineCount; i++) {
                    var y = spaceHeight * i;
                    if (y * 10 % 10 == 0) y += .5;
                    this.drawHLine(options.splitLineColor, 0, y, options.region.width, 1, options.lineStyle);
                }
                //画垂直底纹线
                var spaceWidth = options.region.width / (options.verticalLineCount + 1);
                for (var i = 1; i <= options.verticalLineCount; i++) {
                    var w = spaceWidth * i;
                    if (w * 10 % 10 == 0) w += .5;
                    this.drawVLine(options.splitLineColor, w, 0, options.region.height, 1, options.lineStyle);
                }
                //timer.stop();
            },
            end: function () {
                this.ctx.restore();
                var me = this;
                var options = me.klOptions;
                var region = options.region;
                var volumeRegion = options.volume.region;

                function getIndex(x) {
                    x -= region.x / 2;
                    var index = Math.ceil(x / (me.klOptions.spaceWidth + me.klOptions.barWidth) * 2) - 1;
                    var count = me.toIndex - me.startIndex + 1;
                    if (index >= count) index = count - 1;
                    return index;
                }

                function getX(x) {
                    var index = getIndex(x);
                    return (region.x + me.klOptions.spaceWidth * (index + 1) + me.klOptions.barWidth * index + me.klOptions.barWidth * .5) / 2;
                }

                function getPriceColor(ki, price) {
                    if (price > ki.preClose) {
                        return riseColor;
                    } else if (price < ki.preClose) {
                        return fallColor;
                    } else {
                        return normalColor;
                    }
                }

                function getTipHtml(x) {
                    var index = me.startIndex + getIndex(x);
                    if (index >= me.data.ks.length) index = me.data.ks.length - 1;
                    if (index < 0) index = 0;
                    var ki = me.data.ks[index];
                    var tipHtml = '<div><b>' + convertDate(ki.quoteTime) + '</b></div>' +
                        '昨收价：<font color="' + getPriceColor(ki, ki.preClose) + '">' + toMoneyFix(ki.preClose, 4) + '</font><br/>' +
                        '开盘价：<font color="' + getPriceColor(ki, ki.open) + '">' + toMoneyFix(ki.open, 4) + '</font><br/>' +
                        '最高价：<font color="' + getPriceColor(ki, ki.high) + '">' + toMoneyFix(ki.high, 4) + '</font><br/>' +
                        '最低价：<font color="' + getPriceColor(ki, ki.low) + '">' + toMoneyFix(ki.low, 4) + '</font><br/>' +
                        '收盘价：<font color="' + getPriceColor(ki, ki.close) + '">' + toMoneyFix(ki.close, 4) + '</font><br/>' +
                        '成交量：' + bigNumberToText(ki.volume) + '手<br/>' +
                        '成交额：' + bigNumberToText(ki.amount);
                    return tipHtml;
                }

                function getEventOffsetPosition(ev) {
                    var offset = isTouchDevice() ?
                        setTouchEventOffsetPosition(ev, getPageCoord(me.canvas)) :
                        getOffset(ev);
                    return offset;
                }

                var controllerEvents = {
                    onStart: function (ev) {
                        ev = ev || event;
                        var offset = getEventOffsetPosition(ev);
                        me.controllerStartOffset = offset;
                        me.controllerStartRange = me.dataRanges;
                    },
                    onEnd: function (ev) {
                        me.controllerStartOffset = null;
                        me.controllerStartRange = null;
                    },
                    onMove: function (ev) {
                        if (!me.controllerStartOffset) return;
                        ev = ev || event;
                        var offset = getEventOffsetPosition(ev);
                        var moveX = offset.offsetX - me.controllerStartOffset.offsetX;
                        var currentRange = me.controllerStartRange; /*0-100*/
                        var regionWidth = region.width;
                        var moveValue = 0 - (moveX / regionWidth) * (currentRange.to - currentRange.start);
                        var start = currentRange.start + moveValue;
                        var to = currentRange.to + moveValue;
                        if (start < 0) {
                            start = 0;
                            to = start + (currentRange.to - currentRange.start);
                        } else {
                            if (to > 100) {
                                to = 100;
                                start = to - (currentRange.to - currentRange.start);
                            }
                        }
                        var changeToValue = { left: start, right: to };
                        if (!me.painting) drawKL({ start: changeToValue.left, to: changeToValue.right });
                    }
                };

                /*
                当touchstart时的位置在K线柱附近时表示要显示柱的描述信息框；否则则要控制K线的范围
                */
                var fingerSize = { width: 30, height: 20 };

                function shouldDoControllerEvent(ev, evtType) {
                    if (evtType == undefined) return true;
                    if (typeof me.shouldController == 'undefined') me.shouldController = true;
                    if (evtType == 'touchstart') {
                        var offset = getEventOffsetPosition(ev);
                        var showTip = false;

                        var x = offset.offsetX;
                        x -= region.x / 2;
                        var index = Math.ceil(x / (me.klOptions.spaceWidth + me.klOptions.barWidth) * 2) - 1;
                        var indexRange = Math.ceil(fingerSize.width / (me.klOptions.spaceWidth + me.klOptions.barWidth) * 2) + 1;
                        var indexStart = Math.max(0, index - indexRange / 2);
                        var indexTo = Math.min(me.filteredData.length - 1, index + indexRange / 2);

                        var yMin = 999999;
                        var yMax = -1;
                        for (index = indexStart; index <= indexTo; index++) {
                            var dataAtIndex = me.filteredData[index];
                            var yTop = region.y + (me.high - dataAtIndex.high) * region.height / (me.high - me.low) - fingerSize.height;
                            var yBottom = region.y + (me.high - dataAtIndex.low) * region.height / (me.high - me.low) + fingerSize.height;
                            yMin = Math.min(yTop, yMin);
                            yMax = Math.max(yBottom, yMax);
                        }
                        showTip = offset.offsetY >= yMin && offset.offsetY <= yMax;
                        setDebugMsg('yMin=' + yMin + ',yMax=' + yMax + ',offsetY=' + offset.offsetY + ',showTip=' + showTip);
                        me.shouldController = !showTip;

                    }
                    //setDebugMsg('shouldController=' + me.shouldController);
                    return me.shouldController;
                }

                if (!me.crossLineAndTipMgrInstance) {
                    me.crossLineAndTipMgrInstance = new crossLinesAndTipMgr(me.canvas, {
                        getCrossPoint: function (ev) { return { x: getX(ev.offsetX), y: ev.offsetY }; },
                        triggerEventRanges: { x: region.x / 2, y: (region.y + 1) / 2, width: region.width / 2, height: (volumeRegion.y + volumeRegion.height - region.y) / 2 },
                        tipOptions: {
                            getTipHtml: function (ev) { return getTipHtml(ev.offsetX); },
                            size: { width: 120, height: 150 },
                            position: { x: false, y: region.y / 2 + (region.height - 150) / 3 / 2 }, //position中的值是相对于canvas的左上角的
                            opacity: 80,
                            cssClass: '',
                            offsetToPoint: 30
                        },
                        crossLineOptions: {
                            color: 'black'
                        },
                        shouldDoControllerEvent: shouldDoControllerEvent,
                        controllerEvents: controllerEvents
                    });
                    me.crossLineAndTipMgrInstance.addCrossLinesAndTipEvents();
                } else {
                    me.crossLineAndTipMgrInstance.updateOptions({
                        getCrossPoint: function (ev) { return { x: getX(ev.offsetX), y: ev.offsetY }; },
                        triggerEventRanges: { x: region.x / 2, y: (region.y + 1) / 2, width: region.width / 2, height: (volumeRegion.y + volumeRegion.height - region.y) / 2 },
                        tipOptions: {
                            getTipHtml: function (ev) { return getTipHtml(ev.offsetX); },
                            size: { width: 120, height: 150 },
                            position: { x: false, y: region.y / 2 + (region.height - 150) / 3 / 2 }, //position中的值是相对于canvas的左上角的
                            opacity: 80,
                            cssClass: '',
                            offsetToPoint: 30
                        },
                        crossLineOptions: {
                            color: 'black'
                        },
                        shouldDoControllerEvent: shouldDoControllerEvent,
                        controllerEvents: controllerEvents
                    });
                }
                if (!me.addOrentationChangedEvent) {
                    me.addOrentationChangedEvent = true;

                    addEvent(window, 'orientationchange', function (ev) {
                        me.requestController = true;
                        me.implement.onOrentationChanged.call(me);
                    });
                }

                me.painting = false;
            },
            paintItems: function () {
                var options = this.klOptions;
                var region = options.region;
                var maxDataLength = this.data.ks.length;
                var needCalcSpaceAndBarWidth = true;
                if (this.dataRanges == null) {
                    //计算dataRanges
                    var dataCount = Math.ceil(region.width / (options.spaceWidth + options.barWidth)) - 1;
                    if (dataCount > maxDataLength) dataCount = maxDataLength;

                    this.dataRanges = {
                        start: 100 * (this.data.ks.length - dataCount) / this.data.ks.length,
                        to: 100
                    };

                    needCalcSpaceAndBarWidth = false;
                }
                var dataRanges = this.dataRanges;
                var startIndex = Math.ceil(dataRanges.start / 100 * maxDataLength);
                var toIndex = Math.ceil(dataRanges.to / 100 * maxDataLength) + 1;
                if (toIndex == maxDataLength) toIndex = maxDataLength - 1;
                this.startIndex = startIndex;
                this.toIndex = toIndex;
                var itemsCount = toIndex - startIndex + 1;
                if (needCalcSpaceAndBarWidth) {
                    //重新计算spaceWidth和barWidth属性
                    function isOptionsOK() { return (options.spaceWidth + options.barWidth) * itemsCount <= region.width; }
                    var spaceWidth, barWidth;
                    if (isOptionsOK()) {
                        //柱足够细了
                        spaceWidth = 1;
                        barWidth = (region.width - spaceWidth * itemsCount) / itemsCount;
                        if (barWidth > 4) {
                            spaceWidth = 2;
                            barWidth = ((region.width - spaceWidth * itemsCount) / itemsCount);
                        }
                    } else {
                        spaceWidth = 1;
                        barWidth = (region.width - spaceWidth * itemsCount) / itemsCount;
                        if (barWidth <= 2) {
                            spaceWidth = 0;
                            barWidth = (region.width - spaceWidth * itemsCount) / itemsCount;
                        } else if (barWidth > 4) {
                            spaceWidth = 2;
                            barWidth = ((region.width - spaceWidth * itemsCount) / itemsCount);
                        }
                    }

                    options.barWidth = barWidth;
                    options.spaceWidth = spaceWidth;
                }

                var filteredData = [];
                for (var i = startIndex; i <= toIndex && i < maxDataLength; i++) {
                    filteredData.push(this.data.ks[i]);
                }
                var high, low;
                filteredData.each(function (val, a, i) {
                    if (i == 0) {
                        high = val.high;
                        low = val.low;
                    } else {
                        high = Math.max(val.high, high);
                        low = Math.min(low, val.low);
                    }
                });

                this.high = high;
                this.low = low;
                var ctx = this.ctx;
                var me = this;
                //timer.start('paintItems:移动均线');
                //画移动平均线
                this.implement.paintMAs.call(this, filteredData, getY);
                //timer.stop();
                //timer.start('paintItems:画柱');
                function getY(price) { return (me.high - price) * region.height / (me.high - me.low); }

                function getCandleLineX(i) { var result = i * (options.spaceWidth + options.barWidth) + (options.spaceWidth + options.barWidth) * .5; if (result * 10 % 10 == 0) result += .5; return result; }

                var currentX = 0;
                var needCandleRect = options.barWidth > 1.5;
                var drawCandle = function (ki, a, i) {
                    var isRise = ki.close > ki.open;

                    var color = isRise ? riseColor : fallColor;

                    var lineX = getCandleLineX(i);
                    if (currentX == 0) currentX = lineX;
                    else {
                        if (lineX - currentX < 1) return;
                    }
                    currentX = lineX;
                    var topY = getY(ki.high);
                    var bottomY = getY(ki.low);
                    if (needCandleRect) {
                        ctx.fillStyle = color;
                        ctx.strokeStyle = color;
                        var candleY, candleHeight;
                        if (isRise) {
                            candleY = getY(ki.close);
                            candleHeight = getY(ki.open) - candleY;
                        } else {
                            candleY = getY(ki.open);
                            candleHeight = getY(ki.close) - candleY;
                        }

                        //画线
                        ctx.beginPath();
                        ctx.moveTo(lineX, topY);
                        ctx.lineTo(lineX, bottomY);
                        ctx.stroke();

                        var candleX = lineX - options.barWidth / 2;
                        ctx.beginPath();
                        ctx.fillRect(candleX, candleY, options.barWidth, candleHeight);
                    } else {
                        ctx.strokeStyle = color;
                        //画线
                        ctx.beginPath();
                        ctx.moveTo(lineX, topY);
                        ctx.lineTo(lineX, bottomY);
                        ctx.stroke();
                    }

                };
                //画蜡烛
                filteredData.each(drawCandle);
                this.filteredData = filteredData;
                //timer.stop();
                //timer.start('paintItems:纵轴');
                var yAxisOptions = options.yAxis;
                yAxisOptions.region = yAxisOptions.region || { x: 0 - region.x, y: 0 - 3, height: region.height, width: region.x - 3 };
                //画y轴
                var yAxisImp = new yAxis(yAxisOptions);
                var yPainter = new Painter(
                    this.canvas.id,
                    yAxisImp,
                    calcAxisValues(high, low, (options.horizontalLineCount + 2))
                );
                yPainter.paint();
                //timer.stop();
                //timer.start('paintItems:横轴');
                //画X轴
                var xAxisOptions = options.xAxis;
                xAxisOptions.region = { x: 0, y: region.height + 2, width: region.width, height: 20 };
                var xAxisImp = new xAxis(xAxisOptions);
                var xScalers = [];
                var stepLength = filteredData.length / (options.xAxis.scalerCount - 1);
                if (stepLength < 1) {
                    options.xAxis.scalerCount = filteredData.length;
                    stepLength = 1;
                }
                xScalers.push(convertDate(filteredData[0].quoteTime, false).substr(2));
                for (var i = 1; i < options.xAxis.scalerCount; i++) {
                    var index = Math.ceil(i * stepLength);
                    if (index >= filteredData.length) index = filteredData.length - 1;
                    var quoteTime = convertDate(filteredData[index].quoteTime, false);
                    quoteTime = quoteTime.substr(2);
                    xScalers.push(quoteTime);
                }
                var xPainter = new Painter(this.canvas.id, xAxisImp, xScalers);
                xPainter.paint();
                // timer.stop();

                //timer.start('volume');
                //画量
                this.implement.paintVolume.call(this, filteredData);
                //timer.stop();
                //画价格线
                //this.implement.paintPriceLine.call(this);
            },
            paintPriceLine: function () {
                if (this.hasPaintPriceLine) return;
                this.hasPaintPriceLine = true;
                var ctx = this.ctx;
                var options = this.klOptions.priceLine;
                var region = options.region;
                ctx.save();
                ctx.translate(region.x, region.y);

                ctx.clearRect(0 - region.x, 0, this.canvas.width, region.height);
                //画水平底纹线
                var spaceHeight = region.height / (options.horizontalLineCount + 1);
                for (var i = 1; i <= options.horizontalLineCount; i++) {
                    var y = spaceHeight * i;
                    if (y * 10 % 10 == 0) y += .5;
                    this.drawHLine(options.splitLineColor, 0, y, region.width, 1, options.lineStyle);
                }
                //画垂直底纹线
                var spaceWidth = region.width / (options.verticalLineCount + 1);
                for (var i = 1; i <= options.verticalLineCount; i++) {
                    var w = spaceWidth * i;
                    if (w * 10 % 10 == 0) w += .5;
                    this.drawVLine(options.splitLineColor, w, 0, region.height, 1, options.lineStyle);
                }
                var ks = this.data.ks;

                var ksLength = ks.length;

                var priceRange;
                ks.each(function (val, arr, i) {
                    if (i == 0) { priceRange = { high: val.high, low: val.low }; }
                    else {
                        priceRange.high = Math.max(priceRange.high, val.close);
                        priceRange.low = Math.min(priceRange.low, val.close);
                    }
                });
                if (priceRange.low > 1) priceRange.low -= 1;

                function getRangeX(i) { return i * region.width / ksLength; }

                function getRangeY(val) { return (priceRange.high - val) * region.height / (priceRange.high - priceRange.low); }
                var currentX = 0;
                ks.each(function (val, arr, i) {
                    var x = getRangeX(i);
                    if (currentX == 0) currentX = x;
                    else {
                        if (x - currentX < 1) return;
                    }
                    currentX = x;
                    var y = getRangeY(val.close);
                    if (i == 0) {
                        ctx.beginPath();
                        ctx.moveTo(x, y);
                    } else {
                        ctx.lineTo(x, y);
                    }
                });
                ctx.strokeStype = options.borderColor;
                ctx.stroke();
                ctx.lineTo(region.width, region.height);
                ctx.lineTo(0, region.height);
                ctx.closePath();
                ctx.fillStyle = options.fillColor;
                ctx.globalAlpha = options.alpha;
                ctx.fill();
                ctx.globalAlpha = 1;
                var yAxisOptions = options.yAxis;
                yAxisOptions.region = yAxisOptions.region || { x: 0 - region.x, y: 0 - 3, height: region.height, width: region.x - 3 };
                //画y轴
                var yAxisImp = new yAxis(yAxisOptions);
                var yPainter = new Painter(
                    this.canvas.id,
                    yAxisImp,
                    calcAxisValues(priceRange.high, priceRange.low, (options.horizontalLineCount + 2))
                );

                yPainter.paint();
                ctx.restore();
            },
            paintMAs: function (filteredData, funcGetY) {
                var ctx = this.ctx;
                var options = this.klOptions;
                var MAs = options.MAs;
                var me = this;
                MAs.each(function (val, arr, index) {
                    var MA = calcMAPrices(me.data.ks, me.startIndex, filteredData.length, val.daysCount);
                    val.values = MA;
                    MA.each(function (val, arr, i) {
                        if (val) {
                            me.high = Math.max(me.high, val);
                            me.low = Math.min(me.low, val);
                        }
                    });
                });

                MAs.each(function (val, arr, index) {
                    var MA = val.values;
                    ctx.strokeStyle = val.color;
                    ctx.beginPath();
                    var currentX = 0;
                    MA.each(function (val, arr, i) {
                        var x = i * (options.spaceWidth + options.barWidth) + (options.spaceWidth + options.barWidth) * .5;

                        if (!val) return;
                        var y = funcGetY(val);
                        if (y && i == 0) {
                            ctx.moveTo(x, y);
                        } else {
                            ctx.lineTo(x, y);
                        }
                    });
                    ctx.stroke();
                });
            },
            paintVolume: function (filteredData) {
                var ctx = this.ctx;
                var options = this.klOptions;
                //画量线
                var volumeOptions = options.volume;
                var volumeRegion = volumeOptions.region;
                ctx.restore();
                ctx.save();
                ctx.translate(volumeRegion.x, volumeRegion.y);
                ctx.globalAlpha = 1;
                //画水平底纹线
                var spaceHeight = volumeRegion.height / (volumeOptions.horizontalLineCount + 1);
                for (var i = 1; i <= volumeOptions.horizontalLineCount; i++) {
                    var y = spaceHeight * i;
                    if (y * 10 % 10 == 0) y += .5;
                    this.drawHLine(options.splitLineColor, 0, y, options.region.width, 1, options.lineStyle);
                }
                //画垂直底纹线
                var spaceWidth = options.region.width / (options.verticalLineCount + 1);
                for (var i = 1; i <= options.verticalLineCount; i++) {
                    var w = spaceWidth * i;
                    if (w * 10 % 10 == 0) w += .5;
                    this.drawVLine(options.splitLineColor, w, 0, volumeRegion.height, 1, options.lineStyle);
                }

                ctx.strokeStyle = options.borderColor;
                ctx.beginPath();
                ctx.rect(0, 0, volumeRegion.width, volumeRegion.height);
                ctx.stroke();
                //drawLines(ctx, [{ direction: 'H', position: .50, color: 'lightgray'}]);
                var maxVolume = 0;

                filteredData.each(function (val, arr, i) {
                    maxVolume = Math.max(maxVolume, val.volume);
                });
                maxVolume *= 1.05;

                function getVolumeY(v) { return volumeRegion.height - volumeRegion.height / maxVolume * v; }

                function getVolumeX(i) { return i * (options.spaceWidth + options.barWidth) + (options.spaceWidth) * .5; }
                ctx.globalAlpha = 1;
                filteredData.each(function (val, arr, i) {
                    var x = getVolumeX(i);
                    var y = getVolumeY(val.volume);
                    ctx.beginPath();
                    ctx.rect(x, y, options.barWidth, volumeRegion.height / maxVolume * val.volume);
                    ctx.fillStyle = val.close > val.open ? riseColor : fallColor;
                    ctx.fill();
                });

                maxVolume = maxVolume / 10000;
                var volumeScalers = [];
                volumeScalers.push((maxVolume).toFixed(0));
                volumeScalers.push((maxVolume / 2).toFixed(0));
                volumeScalers.push("万");
                var volumeScalerOptions = volumeOptions.yAxis;
                volumeScalerOptions.region = volumeScalerOptions.region || { x: 0 - volumeRegion.x, y: -3, width: volumeRegion.x - 3, height: volumeRegion.height };
                var volumeScalerImp = new yAxis(volumeScalerOptions);
                var volumeScalerPainter = new Painter(this.canvas.id, volumeScalerImp, volumeScalers);
                volumeScalerPainter.paint();
                ctx.restore();
                ctx.save();
            },
            onOrentationChanged: function (e) {
                var orientation = window.orientation;

                function getWidth() { return $(window).width() - 40; }
                if (typeof orientation != 'undefined') {
                    setDebugMsg('orientation=' + orientation + ',getWidth=' + getWidth());
                    //if(orientation == 90 || orientation == -90 || orientation == 0){
                    var me = this;
                    var width = getWidth();
                    //var rate = width/me.canvas.width;
                    me.canvas.width = width;
                    var options = me.klOptions;
                    var chartWidth = width - options.chartMargin.left - options.chartMargin.right;
                    me.klOptions.volume.region.width =
                        me.klOptions.priceLine.region.width =
                        me.klOptions.region.width = chartWidth;
                    //方向改变了，要重画controller
                    me.controller = null;
                    me.hasPaintPriceLine = false;
                    drawKL({ start: me.dataRanges.start, to: me.dataRanges.to });
                    // }
                }
            }
        };

        var painter; // = new Painter('canvasKL', kl, data);
        var initialWidth = Math.min($(window).width(), 1024) * 2;

        function drawKL(ranges) {
            if (ranges != null) {
                var res = JSON.stringify(ranges);
                $("#ranges").val(res);
            }
            var kOptions = {
                backgroundColor: 'transparent',
                riseColor: '#fb4231',
                fallColor: '#37ca35',
                normalColor: '#13151D',
                //主图区域的边距
                chartMargin: { left: 45.5 * 2, top: 10.5 * 2, right: 10.5 * 2 },
                region: { x: 45.5 * 2, y: 10.5 * 2, width: initialWidth - 45.5 * 2 - 10.5 * 2, height: 170 * 2 },
                barWidth: 5 * 2,
                spaceWidth: 2 * 2,
                horizontalLineCount: 5,
                verticalLineCount: 5,
                lineStyle: 'solid',
                borderColor: '#242a32',
                splitLineColor: '#242a32',
                lineWidth: 1,
                MAs: [
                    { color: 'rgb(255,70,251)', daysCount: 5 },
                    { color: 'rgb(227,150,34)', daysCount: 10 },
                    { color: 'rgb(53,71,107)', daysCount: 20 }
                    /*,
                                        { color: 'rgb(0,0,0)', daysCount: 60 }*/
                ],
                yAxis: {
                    font: '20px Arial', // region: { },
                    color: '#999',
                    align: 'right',
                    fontHeight: 8,
                    textBaseline: 'top'
                },
                xAxis: {
                    font: '20px Arial', // region: { },
                    color: '#999',
                    align: 'right',
                    fontHeight: 8,
                    textBaseline: 'top',
                    scalerCount: 4
                },
                volume: {
                    region: { x: 45.5 * 2, y: 195.5 * 2, height: 40 * 2, width: initialWidth - 45.5 * 2 - 10.5 * 2 },
                    horizontalLineCount: 1,
                    yAxis: {
                        font: '20px Arial', // region: { },
                        color: '#999',
                        align: 'right',
                        fontHeight: 8,
                        textBaseline: 'top'
                    }
                },
                priceLine: {
                    region: { x: 45.5 * 2, y: 380.5 * 2, height: 60 * 2, width: initialWidth - 45.5 * 2 - 10.5 * 2 },
                    verticalLineCount: 7,
                    horizontalLineCount: 1,
                    lineStyle: 'solid',
                    borderColor: '#242a32',
                    splitLineColor: '#242a32',
                    fillColor: '#1f1f26',
                    alpha: .5,
                    yAxis: {
                        font: '20px Arial', // region: { },
                        color: '#999',
                        align: 'right',
                        fontHeight: 8,
                        textBaseline: 'top'
                    }
                },
                controller: {
                    bar: { width: 20 * 2, height: 35 * 2, borderColor: '#242a32', fillColor: '#1f1f26' },
                    minBarDistance: 20 * 2
                }

            };

            if (!painter) {
                var canvas = $id('canvasKL');
                if (canvas.width != initialWidth) canvas.width = initialWidth;
                $("#canvasKL").width(initialWidth / 2);
                var kl = new kLine(kOptions);
                var data = getKLData();
                painter = new Painter('canvasKL', kl, data);
            }
            painter.dataRanges = ranges;
            painter.paint();
        }
        function drawKLRef() {
            var ranges = null;
            var res = $("#ranges").val();
            if (res.length > 10) {
                ranges = eval("(" + res + ")");
            }
            var kOptions = {
                backgroundColor: 'transparent',
                riseColor: '#fb4231',
                fallColor: '#37ca35',
                normalColor: '#13151D',
                //主图区域的边距
                chartMargin: { left: 45.5 * 2, top: 10.5 * 2, right: 10.5 * 2 },
                region: { x: 45.5 * 2, y: 10.5 * 2, width: initialWidth - 45.5 * 2 - 10.5 * 2, height: 170 * 2 },
                barWidth: 5 * 2,
                spaceWidth: 2 * 2,
                horizontalLineCount: 5,
                verticalLineCount: 5,
                lineStyle: 'solid',
                borderColor: '#242a32',
                splitLineColor: '#242a32',
                lineWidth: 1,
                MAs: [
                    { color: 'rgb(255,70,251)', daysCount: 5 },
                    { color: 'rgb(227,150,34)', daysCount: 10 },
                    { color: 'rgb(53,71,107)', daysCount: 20 }
                    /*,
                                        { color: 'rgb(0,0,0)', daysCount: 60 }*/
                ],
                yAxis: {
                    font: '20px Arial', // region: { },
                    color: '#999',
                    align: 'right',
                    fontHeight: 8,
                    textBaseline: 'top'
                },
                xAxis: {
                    font: '20px Arial', // region: { },
                    color: '#999',
                    align: 'right',
                    fontHeight: 8,
                    textBaseline: 'top',
                    scalerCount: 4
                },
                volume: {
                    region: { x: 45.5 * 2, y: 195.5 * 2, height: 40 * 2, width: initialWidth - 45.5 * 2 - 10.5 * 2 },
                    horizontalLineCount: 1,
                    yAxis: {
                        font: '20px Arial', // region: { },
                        color: '#999',
                        align: 'right',
                        fontHeight: 8,
                        textBaseline: 'top'
                    }
                },
                priceLine: {
                    region: { x: 45.5 * 2, y: 380.5 * 2, height: 60 * 2, width: initialWidth - 45.5 * 2 - 10.5 * 2 },
                    verticalLineCount: 7,
                    horizontalLineCount: 1,
                    lineStyle: 'solid',
                    borderColor: '#242a32',
                    splitLineColor: '#242a32',
                    fillColor: '#1f1f26',
                    alpha: .5,
                    yAxis: {
                        font: '20px Arial', // region: { },
                        color: '#999',
                        align: 'right',
                        fontHeight: 8,
                        textBaseline: 'top'
                    }
                },
                controller: {
                    bar: { width: 20 * 2, height: 35 * 2, borderColor: '#242a32', fillColor: '#1f1f26' },
                    minBarDistance: 20 * 2
                }

            };

            var canvas = $id('canvasKL');
            if (canvas.width != initialWidth) canvas.width = initialWidth;
            $("#canvasKL").width(initialWidth / 2);
            var kl = new kLine(kOptions);
            var data = getKLData();
            painter = new Painter('canvasKL', kl, data);
            painter.dataRanges = ranges;
            painter.paint();
        }
        $(function () {
            //判断交易时间
            is_trade_time();
        });
    </script>
    <span style="display: none;">
        <script src="https://s96.cnzz.com/z_stat.php?id=1275703377&web_id=1275703377" language="JavaScript"></script>
    </span>
</body>
</html>
