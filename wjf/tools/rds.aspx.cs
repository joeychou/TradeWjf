using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace wjf.tools
{
    public partial class rds : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string act = AppRequest.GetQueryString("act");
                string key = AppRequest.GetQueryString("key");
                if (act == "remove")
                {
                    RedisHelper.Remove(key);
                    Response.Write("操作成功");
                    Response.End();
                }

            }
        }
    }
}