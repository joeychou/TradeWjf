<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="test.aspx.cs" Inherits="wjf.test" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <div class="fr">
        <table class="depute-tab" cellpadding="0" cellspacing="0">
            <tr>
                <td width="18%">代码</td>
                <td id="search" style="position: relative;">
                    <div style="height: 2.05em;">
                        <input type="text" class="depute-inp" style="width: 70%;" id="stock_code" name="stock_code" onkeyup="keyup(event)" onblur="searchIpt();" /><a class="delete" href="javascript:" onclick="clearSearch();"></a>
                    </div>
                    <div style="display: none;" id="keyup_d" class="sokeyup">
                    </div>
                </td>
            </tr>
            <tr>
                <td>名称</td>
                <td id="name">--</td>
            </tr>
            <tr>
                <td>价格</td>
                <td><a class="cut" href="javascript:" onclick="reducePrice();">-</a><input type="text" class="depute-inp" style="width: 50%;" id="decl_price" name="decl_price" maxprice="0" minprice="0" onblur="uptPrice();" /><a class="add" href="javascript:" onclick="addPrice();">+</a></td>
            </tr>
            <tr>
                <td colspan="2">
                    <div class="fl w50">今开 <span class="p_open">0.00</span></div>
                    <div class="fl w50">现价 <span class="p_now">0.00</span></div>
                    <div class="fl w50">昨收 <span class="p_close">0.00</span></div>
                    <div class="fl w50">涨跌 <span class="p_zd">0.00</span></div>
                </td>
            </tr>
        </table>
        <table class="depute-tab-1" cellpadding="0" cellspacing="0">
            <tr>
                <td>卖五</td>
                <td id="sale5_p">--</td>
                <td id="sale5_g">--</td>
            </tr>
            <tr>
                <td>卖四</td>
                <td id="sale4_p">--</td>
                <td id="sale4_g">--</td>
            </tr>
            <tr>
                <td>卖三</td>
                <td id="sale3_p">--</td>
                <td id="sale3_g">--</td>
            </tr>
            <tr>
                <td>卖二</td>
                <td id="sale2_p">--</td>
                <td id="sale2_g">--</td>
            </tr>
            <tr>
                <td>卖一</td>
                <td id="sale1_p">--</td>
                <td id="sale1_g">--</td>
            </tr>
        </table>
        <table class="depute-tab-1" cellpadding="0" cellspacing="0">
            <tr>
                <td>买一</td>
                <td id="buy1_p">--</td>
                <td id="buy1_g">--</td>
            </tr>
            <tr>
                <td>买二</td>
                <td id="buy2_p">--</td>
                <td id="buy2_g">--</td>
            </tr>
            <tr>
                <td>买三</td>
                <td id="buy3_p">--</td>
                <td id="buy3_g">--</td>
            </tr>
            <tr>
                <td>买四</td>
                <td id="buy4_p">--</td>
                <td id="buy4_g">--</td>
            </tr>
            <tr>
                <td>买五</td>
                <td id="buy5_p">--</td>
                <td id="buy5_g">--</td>
            </tr>
        </table>
        <table class="depute-tab-1" style="border-bottom: none;">
            <tr>
                <td class="red">涨停价</td>
                <td class="red" colspan="2" id="upstop">--</td>
            </tr>
            <tr>
                <td class="green">跌停价</td>
                <td class="green" colspan="2" id="downstop">--</td>
            </tr>
            <tr>
                <td class="red">&nbsp;</td>
                <td colspan="2"><a class="fresh" href="javascript:" onclick="return apply_list();">刷新</a></td>
            </tr>
        </table>
    </div>
    <script src="/scripts/jquery-1.11.2.min.js"></script>
    <script type="text/javascript">
        $(function () {
            setInterval("search();", 3000);
        });
        function search() {
            $.get("/stock.ashx", { code: "000001", t: new Date() }, function (data) {
                var api = data.split(',');
                if (api.length > 30) {
                    var yprice = Number(api[2]).toFixed(2), decl_price = Number(api[3]).toFixed(2),
                        p_zd = Number((((decl_price / yprice) - 1) * 100).toFixed(2)), quantity = 0, q_100 = 0;//昨日收盘价,账户当前总额,当前价格
                    $("#name").html(api[0]);//股票名字
                    $("#decl_price").val(decl_price);
                    $("#decl_price").attr("maxprice", Number(yprice * 1.1).toFixed(2));//涨停
                    $("#decl_price").attr("minprice", Number(yprice * 0.9).toFixed(2));//跌停
                    $("#sale5_p").html(Number(api[29]).toFixed(2));//“卖五”报价
                    $("#sale5_g").html(parseInt(parseFloat(api[28]) / 100));//“卖五”申请4695股，即X手；
                    $("#sale4_p").html(Number(api[27]).toFixed(2));
                    $("#sale4_g").html(parseInt(parseFloat(api[26]) / 100));
                    $("#sale3_p").html(Number(api[25]).toFixed(2));
                    $("#sale3_g").html(parseInt(parseFloat(api[24]) / 100));
                    $("#sale2_p").html(Number(api[23]).toFixed(2));
                    $("#sale2_g").html(parseInt(parseFloat(api[22]) / 100));
                    $("#sale1_p").html(Number(api[21]).toFixed(2));
                    $("#sale1_g").html(parseInt(parseFloat(api[20]) / 100));
                    $("#buy1_p").html(Number(api[11]).toFixed(2));//“买五”报价
                    $("#buy1_g").html(parseInt(parseFloat(api[10]) / 100));//“买五”申请4695股，即X手；
                    $("#buy2_p").html(Number(api[13]).toFixed(2));
                    $("#buy2_g").html(parseInt(parseFloat(api[12]) / 100));
                    $("#buy3_p").html(Number(api[15]).toFixed(2));
                    $("#buy3_g").html(parseInt(parseFloat(api[14]) / 100));
                    $("#buy4_p").html(Number(api[17]).toFixed(2));
                    $("#buy4_g").html(parseInt(parseFloat(api[16]) / 100));
                    $("#buy5_p").html(Number(api[19]).toFixed(2));
                    $("#buy5_g").html(parseInt(parseFloat(api[18]) / 100));
                    $("#upstop").html(Number(yprice * 1.1).toFixed(2));
                    $("#downstop").html(Number(yprice * 0.9).toFixed(2));
                    $("#can_quantity").html(q_100);
                    $("#decl_num").val(0);

                    $(".p_open").html(Number(api[1]).toFixed(2));
                    $(".p_now").html(decl_price);
                    $(".p_close").html(yprice);
                    $(".p_zd").html(p_zd + "%");//涨幅
                    $(".p_zd").removeClass("red");
                    $(".p_zd").removeClass("green");
                    $(".p_now").removeClass("red");
                    $(".p_now").removeClass("green");
                    if (p_zd >= 0) {
                        $(".p_zd").addClass("red");
                        $(".p_now").addClass("red");
                    } else {
                        $(".p_zd").addClass("green");
                        $(".p_now").addClass("green");
                    }
                }
                else {
                    jsprint("没有查询到相关数据！");
                }
            });
        }
    </script>
</body>
</html>
