var api_domain_url = "/";
var getQuote = function () {
    var res, stock_code = $("#stock_code").val(), flag = $("#flag").val();
    $.ajax({
        url: api_domain_url + 'tools/stock_line_t.ashx',
        data: { act: "timeline", code: stock_code, flag: flag, t: new Date() },
        async: false,
        dataType: 'text',
        success: function (result) {
            var res_info;
            if (result.length < 32) {
                res_info = "{quote: {time:201703300930,open:0.01,preClose:0.02,highest:0.02,lowest:0,price:0.02,volume:0,amount:0.01},mins: [{ price:0.01, volume:0, amount:0.01, time:930}]}";
            } else {
                res_info = result;
            }
            res = eval("(" + res_info + ")");
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
    var result = localStorage.datainfo, stock_code = $("#stock_code").val(), flag = $("#flag").val();
    var stock_info = eval("(" + result + ")");
    var mins_len = parseInt(stock_info.mins.length), mins_len_last = mins_len - 1;
    $.getJSON(api_domain_url + "tools/api_ajax.ashx", { act: "stock_t", code: stock_code, flag: flag, t: new Date() }, function (data) {
        if (data == null) {
            return false;
        }
        var new_time = data.mins[mins_len_last].time;
        //判断是否交易时间
        if ((new_time > 930 && new_time < 1130) || (new_time > 1300 && new_time < 1500)) {
            clearCanvas();
            chart.paint(data);
            localStorage.datainfo = JSON.stringify(data);
        }
    });
}
//获取是否是交易时间
var is_trade_time = function () {
    $.getJSON(api_domain_url + "tools/api_ajax.ashx", { act: "is_trade_time", t: new Date() }, function (data) {
        //交易时间内定时刷新
        if (data.is_trade_time == 1) {
            setInterval("getQuoteRef()", 5 * 1000);
        }
    });
}
//清除画布
var clearCanvas = function () {
    var c = document.getElementById("canvas");
    var cxt = c.getContext("2d");
    cxt.fillStyle = "transparent";
    cxt.beginPath();
    cxt.fillRect(0, 0, c.width, c.height);
    cxt.closePath();
}