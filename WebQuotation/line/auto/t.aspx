﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="t.aspx.cs" Inherits="WebQuotation.line.auto.t" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="author" content="joeychou.me" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=0" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <meta name="apple-mobile-web-app-status-bar-style" content="black" />
    <meta name="format-detection" content="telephone=no" />
    <link href="//js.zouhongsoft.com/layout.css" rel="stylesheet" />
    <title>分时图</title>
</head>
<body>
    <div class="w-loading"><b class="w-loading-ico"></b></div>
    <div class="wrap" style="height: 100%;display:none;">
        <div id="report" style="height: 100%"></div>
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
    <script src="//js.zouhongsoft.com/highstock.js"></script>
    <script src="//js.zouhongsoft.com/mins-data.js?rnd=201909091800"></script>
    <%}
        else
        { %>
    <script src="//js.zouhongsoft.com/highstock.js"></script>
    <script src="//js.zouhongsoft.com/mins-data.js?rnd=201909091800"></script>
    <%} %>
    <script type="text/javascript">
        var setHeight = $(window).height() - 10;
        $("#report").css("height", setHeight);
        clickTrend();
        //判断交易时间
        is_trade_time();
    </script>
    <span style="display: none;">
        <script src="https://s96.cnzz.com/z_stat.php?id=1275703377&web_id=1275703377" language="JavaScript"></script>
    </span>
</body>
</html>
