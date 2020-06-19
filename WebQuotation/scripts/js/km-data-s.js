var api_domain_url = "https://quotation.qianniusoft.com/";
function getKLData() {
    var stock_code = $("#stock_code").val();
    var flag = $("#flag").val();
    var stock_code = $("#stock_code").val();
    var url = $("#url").val();
    var time = $("#time").val();
    var token = $("#token").val();
    var result = {};
    var ks = [];
    $.ajax({
        url: api_domain_url + 'tools/stock_line_km.ashx',
        data: { act: "timeline", code: stock_code, url: url, time: time, token: token, t: new Date() },
        async: false,
        dataType: 'json',
        success: function (data) {
            $.each(data.kmins, function (i, v) {
                var item = {
                    quoteTime: v.time,
                    preClose: v.preClose,
                    open: v.open,
                    high: v.highest,
                    low: v.lowest,
                    close: v.price,
                    volume: v.volume,
                    amount: v.amount
                };
                if (ks.length == 0) {
                    result.low = item.low;
                    result.high = item.high;
                } else {
                    result.high = Math.max(result.high, item.high);
                    result.low = Math.min(result.low, item.low);
                }
                ks.push(item);
            });
        }
    });
    result.ks = ks;
    $(".w-loading").css("display", "none");
    $(".wrap").css("display", "block");
    return result;
}
//获取是否是交易时间
var is_trade_time = function () {
    $.getJSON(api_domain_url + "tools/api_ajax.ashx", { act: "is_trade_time", t: new Date() }, function (data) {
        //交易时间内定时刷新
        if (data.is_trade_time == 1) {
            setInterval("drawKLRef()", 0.5 * 60 * 1000);
        }
    });
}