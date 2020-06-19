<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="t.aspx.cs" Inherits="WebQuotation.line.auto.t" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="author" content="joeychou.me" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=0" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <meta name="apple-mobile-web-app-status-bar-style" content="black" />
    <meta name="format-detection" content="telephone=no" />
    <link href="https://video.chw999.com/layout.css" rel="stylesheet" />
    <title>分时图</title>
</head>
<body>
    <div style="height: 100%">
        <div id="report" style="height: 100%"></div>
    </div>
    <div class="w-loading"><b class="w-loading-ico"></b></div>
    <input id="stock_code" type="hidden" value="<%=stock_code%>" />
    <input id="flag" type="hidden" value="<%=flag%>" />
    <input id="url" type="hidden" value="<%=url%>" />
    <input id="time" type="hidden" value="<%=time%>" />
    <input id="token" type="hidden" value="<%=token%>" />
    <input id="hidDataInfo" type="hidden" value="" />
    <script src="https://video.chw999.com/jquery-1.11.2.min.js"></script>
    <script src="https://video.chw999.com/highstock.js"></script>
    <script src="https://video.chw999.com/mins-data.js?rnd=201811271500"></script>
    <script type="text/javascript">
        var setHeight = $(window).height() - 10;
        $("#report").css("height", setHeight);
        //判断交易时间
        is_trade_time();
        clickTrend();
    </script>
    <span style="display: none;">
        <script src="https://s96.cnzz.com/z_stat.php?id=1275703377&web_id=1275703377" language="JavaScript"></script>
    </span>
</body>
</html>
