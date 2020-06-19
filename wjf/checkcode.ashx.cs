using aliyun_api_gateway_sdk;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace wjf
{
    /// <summary>
    /// 图片验证码识别
    /// </summary>
    public class checkcode : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            string convert_to_jpg = AppRequest.GetString("convert_to_jpg"),
               img_base64 = AppRequest.GetString("img_base64").Replace(" ", "+"),
               typeId = AppRequest.GetString("typeId");

            //string img_base642 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAADkAAAAXCAYAAACxvufDAAABCklEQVRYhe2YQQ6DIBBFS8MRuiDuyz168t6DnsCkGzZs20IyjVYYRgKo0LfBGEfny/AZZVrr16lx+DiOW+dQDCGEG88b51GFLkRyOLjIa/Cip3pEbyQ/8SpynUSeEYqVdyTmFs/Lwn9PUAQtEkGS9xF7Gd4YoqApjDE3ZinXlKRr0sWa/ItshYXxtAQYz1ckuKpvK0lxXB9gUD43DpkXuKpvK6E6LlNKob0riI4JpeyTWKxlTTyIxoQOw+DGLtYkSWSucsUo2SCgxmNLtbTAlDK3pUoRGO14ji5wymwmsSY9F2v7XBeDNOkUZiKPvvZCZHFXmJ2UWapBlo5nr18hWT+19g4zxjT/t+4Nka5mvtukWa8AAAAASUVORK5CYII=";
            //img_base64 = "data:image/jpg;base64,Qk1CEgAAAAAAADYAAAAoAAAANwAAABUAAAABABgAAAAAAMgNAAAAAAAAAAAAAAAAAAAAAAAA2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2traAAAA2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2traAAAA2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2traAIAA2tra2tra2tra2tra2tra2tra2tra2tra2traAAD/AAD/AAD/AAD/AAD/AAD/AAD/2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2traAAAA2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2traAIAAAIAAAIAA2tra2tra2tra2tra2tra2tra2traAAD/AAD/2tra2tra2tra2tra2traAAD/AAD/2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2traAAAA2tra2tra2tra2traAIAAAIAAAIAAAIAA2tra2tra2tra2tra2tra2tra2tra2tra2traAIAAAIAA2traAIAAAIAA2tra2tra2tra2tra2tra2traAAD/AAD/2tra2tra2tra2tra2traAAD/AAD/2tra2tra2tra2tra2traAIAAAIAAAIAAAIAA2tra2tra2tra2tra2tra2tra2tra2tra2traAAAA2tra2tra2tra2tra2tra2tra2traAIAA2tra2tra2tra2tra2tra2tra2tra2tra2traAIAA2tra2tra2traAIAA2tra2tra2tra2tra2tra2traAAD/AAD/2tra2tra2tra2tra2traAAD/AAD/2tra2tra2tra2tra2tra2tra2tra2traAIAA2tra2tra2tra2tra2tra2tra2tra2tra2traAAAA2tra2tra2tra2tra2tra2tra2tra2traAIAA2tra2tra2tra2tra2tra2tra2traAIAA2tra2tra2tra2tra2traAIAA2tra2tra2tra2tra2traAAD/AAD/2tra2tra2tra2tra2traAAD/AAD/2tra2tra2tra2tra2tra2tra2tra2tra2traAIAA2tra2tra2tra2tra2tra2tra2tra2traAAAA2tra2tra2tra2tra2tra2tra2tra2tra2traAIAA2tra2tra2tra2tra2tra2traAIAA2tra2tra2tra2tra2traAIAA2tra2tra2tra2tra2traAAD/AAD/2tra2tra2tra2tra2traAAD/AAD/2tra2tra2tra2tra2tra2tra2tra2tra2tra2traAIAA2tra2tra2tra2tra2tra2tra2traAAAA2tra2tra2tra2tra2tra2tra2tra2tra2traAIAAAIAA2tra2tra2tra2tra2traAIAA2tra2tra2tra2tra2traAIAA2tra2tra2tra2tra2tra2traAAD/2tra2tra2tra2tra2traAAD/2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2traAIAAAIAA2tra2tra2tra2tra2tra2traAAAA2tra2tra2tra2tra2tra2tra2tra2tra2traAIAAAIAA2tra2tra2tra2tra2traAIAA2tra2tra2tra2tra2traAIAA2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2traAIAAAIAA2tra2tra2tra2tra2tra2traAAAA2tra2tra2tra2tra2traAIAAAIAAAIAAAIAAAIAA2tra2tra2tra2tra2tra2traAIAA2tra2tra2tra2tra2traAIAA2tra2tra2tra2tra2tra2traAAD/AAD/AAD/AAD/AAD/AAD/2tra2tra2tra2tra2tra2tra2tra2traAIAAAIAAAIAAAIAAAIAA2tra2tra2tra2tra2tra2tra2traAAAA2tra2tra2traAIAAAIAA2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2traAIAA2tra2tra2tra2tra2traAIAA2tra2tra2tra2tra2tra2traAAD/2tra2tra2tra2tra2traAAD/2tra2tra2tra2tra2traAIAAAIAA2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2traAAAA2tra2tra2traAIAAAIAA2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2traAIAA2tra2tra2tra2tra2traAIAA2tra2tra2tra2tra2traAAD/AAD/2tra2tra2tra2tra2traAAD/AAD/2tra2tra2tra2traAIAAAIAA2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2traAAAA2tra2tra2traAIAAAIAA2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2traAIAA2tra2tra2tra2tra2traAIAA2tra2tra2tra2tra2traAAD/AAD/2tra2tra2tra2tra2traAAD/AAD/2tra2tra2tra2traAIAAAIAA2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2traAAAA2tra2tra2traAIAAAIAA2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2traAIAAAIAA2tra2tra2traAIAAAIAA2tra2tra2tra2tra2traAAD/AAD/2tra2tra2tra2tra2traAAD/AAD/2tra2tra2tra2traAIAAAIAA2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2traAAAA2tra2tra2traAIAAAIAA2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2traAIAAAIAA2traAIAAAIAA2tra2tra2tra2tra2tra2traAAD/AAD/2tra2tra2tra2tra2traAAD/AAD/2tra2tra2tra2traAIAAAIAA2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2traAAAA2tra2tra2traAIAAAIAA2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2traAIAAAIAAAIAA2tra2tra2tra2tra2tra2tra2traAAD/AAD/2tra2tra2tra2tra2traAAD/AAD/2tra2tra2tra2traAIAAAIAA2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2traAAAA2tra2tra2traAIAAAIAAAIAAAIAAAIAAAIAAAIAA2tra2tra2tra2tra2tra2tra2tra2tra2traAIAA2tra2tra2tra2tra2tra2tra2tra2tra2traAAD/AAD/AAD/AAD/AAD/AAD/AAD/2tra2tra2tra2tra2traAIAAAIAAAIAAAIAAAIAAAIAAAIAA2tra2tra2tra2tra2tra2tra2traAAAA2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2traAAAA2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2traAAAA2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2tra2traAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";
            if (convert_to_jpg.Trim().Length < 1 || typeId.Trim().Length < 4 || img_base64.Trim().Length < 128)
            {
                context.Response.Write("fail");
                return;
            }
            try
            {
                ShowapiRequest req = new ShowapiRequest("http://ali-checkcode.showapi.com/checkcode", "0ad8f94cf43b456bafcf9bc79905cee4");
                string ret =
                    req.addTextPara("convert_to_jpg", convert_to_jpg)
                    .addTextPara("img_base64", img_base64)
                    .addTextPara("typeId", typeId)
                    .doPost();
                
                context.Response.Write(ret);
                return;
            }
            catch (WebException ex)
            {
                context.Response.Write("fail:" + ex.ToString());
                return;
            }
        }
        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}