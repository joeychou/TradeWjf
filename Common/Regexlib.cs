using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Common
{
    public class Regexlib
    {
        /// <summary>
        /// 判断字符串是否是a-zA-Z0-9_范围内（6-24位范围内）
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public static bool IsValidNormalChar(string strIn)
        {
            return Regex.IsMatch(strIn, @"^[A-Za-z0-9_]{6,24}$");
        }
        /// <summary>
        /// 密码必须包含数字、字母且必须在a-zA-Z0-9_范围内（8-24位范围内）
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public static bool IsValidPwdChar(string strIn)
        {
            return Regex.IsMatch(strIn, @"^(?![0-9]+$)(?![a-zA-Z]+$)[0-9A-Za-z_]{8,24}$");
        }
        /// <summary>
        /// 验证邮箱
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public static bool IsValidEmail(string strIn)
        {
            return Regex.IsMatch(strIn, @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
        }
        /// <summary>
        /// 判断字符串是否是汉字
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public static bool IsStringChinese(string strIn)
        {
            return Regex.IsMatch(strIn, @"^[\u4e00-\u9fa5]+$");
        }
        /// <summary>
        /// 验证手机
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public static bool IsValidMobile(string strIn)
        {
            return Regex.IsMatch(strIn, @"^[1]+[3,4,5,6,7,8,9]+\d{9}");
        }
        /// <summary>
        /// 验证手机号
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public static bool IsValidTelPhone(string strIn)
        {
            return Regex.IsMatch(strIn, @"^(\d{3,4}-)?\d{6,8}$");
        }
        /// <summary>
        /// 验证QQ号码
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public static bool IsValidQQ(string strIn)
        {
            return Regex.IsMatch(strIn, @"^\s*[.0-9]{5,13}\s*$");
        }
        /// <summary>
        /// 字母数字下划线验证
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public static bool IsValidChar(string strIn)
        {
            return Regex.IsMatch(strIn, @"^\w{6,32}$");
        }
        /// <summary>
        /// 是否为ip
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIP(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }
        /// <summary>  
        /// 验证身份证合理性  
        /// </summary>  
        /// <param name="Id"></param>  
        /// <returns></returns>  
        public static bool CheckIDCard(string idNumber)
        {
            if (idNumber.Length == 18)
            {
                bool check = CheckIDCard18(idNumber);
                return check;
            }
            else if (idNumber.Length == 15)
            {
                bool check = CheckIDCard15(idNumber);
                return check;
            }
            else
            {
                return false;
            }
        }
        /// <summary>  
        /// 18位身份证号码验证  
        /// </summary>  
        public static bool CheckIDCard18(string idNumber)
        {
            long n = 0;
            if (long.TryParse(idNumber.Remove(17), out n) == false
                || n < Math.Pow(10, 16) || long.TryParse(idNumber.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;//数字验证  
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(idNumber.Remove(2)) == -1)
            {
                return false;//省份验证  
            }
            string birth = idNumber.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证  
            }
            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] Ai = idNumber.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }
            int y = -1;
            Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != idNumber.Substring(17, 1).ToLower())
            {
                return false;//校验码验证  
            }
            return true;//符合GB11643-1999标准  
        }
        /// <summary>  
        /// 16位身份证号码验证  
        /// </summary>  
        public static bool CheckIDCard15(string idNumber)
        {
            long n = 0;
            if (long.TryParse(idNumber, out n) == false || n < Math.Pow(10, 14))
            {
                return false;//数字验证  
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(idNumber.Remove(2)) == -1)
            {
                return false;//省份验证  
            }
            string birth = idNumber.Substring(6, 6).Insert(4, "-").Insert(2, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证  
            }
            return true;
        }
    }
}
