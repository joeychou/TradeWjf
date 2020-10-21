var api_domain_url = "https://quotation.zouhongsoft.com/";
var getQuote = function () {
    var res;
    var flag = $("#flag").val();
    var stock_code = $("#stock_code").val();
    var url = $("#url").val();
    var time = $("#time").val();
    var token = $("#token").val();
    $.ajax({
        url: api_domain_url + 'tools/stock_line_t.ashx',
        data: { act: "timeline", code: stock_code, flag: flag, url: url, time: time, token: token, t: new Date() },
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
            //��Ⱦcanvas
            chart.paint(res);
            $(".w-loading").css("display", "none");
            $(".wrap").css("display", "block");
        }
    });
    return res;
}

var getQuoteRef = function () {
    var result = localStorage.datainfo, stock_code = $("#stock_code").val(), flag = $("#flag").val();
    var url = $("#url").val();
    var time = $("#time").val();
    var token = $("#token").val();
    var stock_info = eval("(" + result + ")");
    var mins_len = parseInt(stock_info.mins.length), mins_len_last = mins_len - 1;
    $.getJSON(api_domain_url + "tools/api_ajax.ashx", { act: "stock_t", code: stock_code, flag: flag, url: url, time: time, token: token, t: new Date() }, function (data) {
        if (data == null) {
            return false;
        }
        var new_time = data.mins[mins_len_last].time;
        //�ж��Ƿ���ʱ��
        if ((new_time > 930 && new_time < 1130) || (new_time > 1300 && new_time < 1500)) {
            clearCanvas();
            chart.paint(data);
            localStorage.datainfo = JSON.stringify(data);
        }
    });
}
//��ȡ�Ƿ��ǽ���ʱ��
var is_trade_time = function () {
    $.getJSON(api_domain_url + "tools/api_ajax.ashx", { act: "is_trade_time", t: new Date() }, function (data) {
        //����ʱ���ڶ�ʱˢ��
        if (data.is_trade_time == 1) {
            setInterval("getQuoteRef()", 5 * 1000);
        }
    });
}
//�������
var clearCanvas = function () {
    var c = document.getElementById("canvas");
    var cxt = c.getContext("2d");
    cxt.fillStyle = "transparent";
    cxt.beginPath();
    cxt.fillRect(0, 0, c.width, c.height);
    cxt.closePath();
}