using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class AppKeys
    {
        //系统版本
        /// <summary>
        /// 版本号全称
        /// </summary>
        public const string ASSEMBLY_VERSION = "3.0.0";
        /// <summary>
        /// 版本年号
        /// </summary>
        public const string ASSEMBLY_YEAR = "2013";
        //File======================================================    
        /// <summary>
        /// 站点配置文件名
        /// </summary>
        public const string FILE_SITE_XML_CONFING = "sitepath";
        /// <summary>
        /// 数据配置文件名
        /// </summary>
        public const string FILE_DATA_XML_CONFING = "datapath";
        /// <summary>
        /// URL配置文件名
        /// </summary>
        public const string FILE_URL_XML_CONFING = "Urlspath";

        //Cache======================================================
        /// <summary>
        /// 站点配置
        /// </summary>
        public const string CACHE_SITE_CONFIG = "bs_cache_site_config";
        /// <summary>
        /// 数据配置
        /// </summary>
        public const string CACHE_DATA_CONFIG = "bs_cache_data_config";
        /// <summary>
        /// 用户配置
        /// </summary>
        public const string CACHE_USER_CONFIG = "bs_cache_user_config";
        /// <summary>
        /// URL重写映射表
        /// </summary>
        public const string CACHE_SITE_URLS = "dt_cache_site_urls";
        //Session=====================================================
        /// <summary>
        /// 网页验证码
        /// </summary>
        public const string SESSION_CODE = "bs_session_code";
        /// <summary>
        /// 管理员登录次数
        /// </summary>
        public const string SESSION_LOGIN_SUM = "bs_session_login_sum";
        /// <summary>
        /// 后台管理员
        /// </summary>
        public const string SESSION_ADMIN_INFO = "session_admin_info";
        /// <summary>
        /// 手机验证码
        /// </summary>
        public const string SESSION_SMS_CODE = "bs_session_sms_code";
        /// <summary>
        /// 会员用户
        /// </summary>
        public const string SESSION_USER_INFO = "bs_session_user_info";
        /// <summary>
        /// 代理
        /// </summary>
        public const string SESSION_AGENTS_INFO = "session_agents_info";
        //Cookies=====================================================
        /// <summary>
        /// 记住会员用户ID
        /// </summary>
        public const string COOKIE_USER_ID = "bs_cookie_user_id";
        /// <summary>
        /// 记住会员账号
        /// </summary>
        public const string COOKIE_ACCOUNT_NO = "ck_account_no";
        /// <summary>
        /// 记住会员用户名
        /// </summary>
        public const string COOKIE_USER_NAME = "bs_cookie_user_name";
        /// <summary>
        /// 记住会员密码
        /// </summary>
        public const string COOKIE_USER_PWD = "bs_cookie_user_pwd";
        /// <summary>
        /// 记住用户登录时间
        /// </summary>
        public const string COOKIE_USER_LOG = "bs_cookie_user_log";
        /// <summary>
        /// 管理员账号
        /// </summary>
        public const string COOKIE_ADMIN_NAME = "AdminName";
        /// <summary>
        /// 管理员密码
        /// </summary>
        public const string COOKIE_ADMIN_PWD = "AdminPwd";
        /// <summary>
        /// 邀请人
        /// </summary>
        public const string COOKIE_REFERENCE = "bs_cookie_reference";
        /// <summary>
        /// 渠道来源
        /// </summary>
        public const string COOKIE_CHANNELS_FROM = "ChannelsFrom";
        /// <summary>
        /// 邀请代理
        /// </summary>
        public const string COOKIE_AGENTS = "bs_cookie_agents";
        /// <summary>
        /// 代理账号
        /// </summary>
        public const string COOKIE_AGENTS_NAME = "AgentsName";
        /// <summary>
        /// 代理密码
        /// </summary>
        public const string COOKIE_AGENTS_PWD = "AgentsPwd";
        /// <summary>
        /// 行情TOKEN校验
        /// </summary>
        public const string TOKEN_CHECK = "ziniuQuotation";

        /// <summary>
        /// 聚合数据API KEY值
        /// </summary>
        public const string JUHE_API_KEY = "9a5c67179f02bde88baa0e89f91684e2";
        /// <summary>
        /// 聚合数据API 调用地址
        /// </summary>
        public const string JUHE_API_URL = "http://web.juhe.cn:8080/finance/stock/hs";
        /// <summary>
        /// 行情接口地址
        /// </summary>
        public static string QuotationAPI = "114.80.63.12";//119.147.212.81
        /// <summary>
        /// 分时图时间
        /// </summary>
        public const string CACHE_HQ_TIME = "930,931,932,933,934,935,936,937,938,939,940,941,942,943,944,945,946,947,948,949,950,951,952,953,954,955,956,957,958,959,"
            + "1000,1001,1002,1003,1004,1005,1006,1007,1008,1009,1010,1011,1012,1013,1014,1015,1016,1017,1018,1019,1020,1021,1022,1023,1024,1025,1026,1027,1028,1029,1030,1031,1032,1033,1034,1035,1036,1037,1038,1039,1040,1041,1042,1043,1044,1045,1046,1047,1048,1049,1050,1051,1052,1053,1054,1055,1056,1057,1058,1059,"
            + "1100,1101,1102,1103,1104,1105,1106,1107,1108,1109,1110,1111,1112,1113,1114,1115,1116,1117,1118,1119,1120,1121,1122,1123,1124,1125,1126,1127,1128,1129,"
            + "1130,1301,1302,1303,1304,1305,1306,1307,1308,1309,1310,1311,1312,1313,1314,1315,1316,1317,1318,1319,1320,1321,1322,1323,1324,1325,1326,1327,1328,1329,1330,1331,1332,1333,1334,1335,1336,1337,1338,1339,1340,1341,1342,1343,1344,1345,1346,1347,1348,1349,1350,1351,1352,1353,1354,1355,1356,1357,1358,1359,"
            + "1400,1401,1402,1403,1404,1405,1406,1407,1408,1409,1410,1411,1412,1413,1414,1415,1416,1417,1418,1419,1420,1421,1422,1423,1424,1425,1426,1427,1428,1429,1430,1431,1432,1433,1434,1435,1436,1437,1438,1439,1440,1441,1442,1443,1444,1445,1446,1447,1448,1449,1450,1451,1452,1453,1454,1455,1456,1457,1458,1459,1500";
    }
}
