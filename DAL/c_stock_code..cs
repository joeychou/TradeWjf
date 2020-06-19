using System;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using DBUtility;
namespace DAL
{
    //c_stock_code
    public partial class stock_codeDAL
    {
        #region 基础方法
        public bool Exists(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from c_stock_code");
            strSql.Append(" where ");
            strSql.Append(" id = @id  ");
            MySqlParameter[] parameters = {
					new MySqlParameter("@id", MySqlDbType.Int32)};
            parameters[0].Value = id;

            return DbHelperMySQL.Exists(strSql.ToString(), parameters);
        }



        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(Model.stock_codeInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into c_stock_code(");
            strSql.Append("stock_code,stock_name,stock_name_capital,stock_board,circulation_value,circulation,sort_id,status");
            strSql.Append(") values (");
            strSql.Append("@stock_code,@stock_name,@stock_name_capital,@stock_board,@circulation_value,@circulation,@sort_id,@status");
            strSql.Append("); Select LAST_INSERT_ID() ");

            MySqlParameter[] parameters = {
			            new MySqlParameter("@stock_code", MySqlDbType.VarChar,12) ,            
                        new MySqlParameter("@stock_name", MySqlDbType.VarChar,32) ,            
                        new MySqlParameter("@stock_name_capital", MySqlDbType.VarChar,32) ,            
                        new MySqlParameter("@stock_board", MySqlDbType.VarChar,32) ,            
                        new MySqlParameter("@circulation_value", MySqlDbType.Decimal,12) ,            
                        new MySqlParameter("@circulation", MySqlDbType.VarChar,32) ,            
                        new MySqlParameter("@sort_id", MySqlDbType.Int32,11) ,            
                        new MySqlParameter("@status", MySqlDbType.Int32,2)             
              
            };

            parameters[0].Value = model.stock_code;
            parameters[1].Value = model.stock_name;
            parameters[2].Value = model.stock_name_capital;
            parameters[3].Value = model.stock_board;
            parameters[4].Value = model.circulation_value;
            parameters[5].Value = model.circulation;
            parameters[6].Value = model.sort_id;
            parameters[7].Value = model.status;
            int rows = Convert.ToInt32(DbHelperMySQL.GetSingle(strSql.ToString(), parameters));
            return rows;
        }


        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Model.stock_codeInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update c_stock_code set ");

            strSql.Append(" stock_code = @stock_code , ");
            strSql.Append(" stock_name = @stock_name , ");
            strSql.Append(" stock_name_capital = @stock_name_capital , ");
            strSql.Append(" stock_board = @stock_board , ");
            strSql.Append(" circulation_value = @circulation_value , ");
            strSql.Append(" circulation = @circulation , ");
            strSql.Append(" sort_id = @sort_id , ");
            strSql.Append(" status = @status  ");
            strSql.Append(" where id=@id ");
            MySqlParameter[] parameters = {
			            new MySqlParameter("@stock_code", MySqlDbType.VarChar,12) ,      
                        new MySqlParameter("@stock_name", MySqlDbType.VarChar,32) ,      
                        new MySqlParameter("@stock_name_capital", MySqlDbType.VarChar,32) ,      
                        new MySqlParameter("@stock_board", MySqlDbType.VarChar,32) ,      
                        new MySqlParameter("@circulation_value", MySqlDbType.Decimal,12) ,      
                        new MySqlParameter("@circulation", MySqlDbType.VarChar,32) ,      
                        new MySqlParameter("@sort_id", MySqlDbType.Int32,11) ,      
                        new MySqlParameter("@status", MySqlDbType.Int32,2) ,      
              
            new MySqlParameter("@id", MySqlDbType.Int32,11)};
            parameters[0].Value = model.stock_code;
            parameters[1].Value = model.stock_name;
            parameters[2].Value = model.stock_name_capital;
            parameters[3].Value = model.stock_board;
            parameters[4].Value = model.circulation_value;
            parameters[5].Value = model.circulation;
            parameters[6].Value = model.sort_id;
            parameters[7].Value = model.status;
            parameters[8].Value = model.id;

            int rows = DbHelperMySQL.ExecuteSql(strSql.ToString(), parameters);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from c_stock_code ");
            strSql.Append(" where id=@id");
            MySqlParameter[] parameters = {
					new MySqlParameter("@id", MySqlDbType.Int32)
};
            parameters[0].Value = id;

            int rows = DbHelperMySQL.ExecuteSql(strSql.ToString(), parameters);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.stock_codeInfo GetModel(int id)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select id, stock_code, stock_name, stock_name_capital, stock_board, circulation_value, circulation, sort_id, status  ");
            strSql.Append("  from c_stock_code ");
            strSql.Append(" where id=@id");
            MySqlParameter[] parameters = {
					new MySqlParameter("@id", MySqlDbType.Int32)
};
            parameters[0].Value = id;


            Model.stock_codeInfo model = new Model.stock_codeInfo();
            DataSet ds = DbHelperMySQL.Query(strSql.ToString(), parameters);

            if (ds.Tables[0].Rows.Count > 0)
            {
                return DataRowToModel(ds.Tables[0].Rows[0]);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.stock_codeInfo GetModel(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select id, stock_code, stock_name, stock_name_capital, stock_board, circulation_value, circulation, sort_id, status  ");
            strSql.Append("  from c_stock_code ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            strSql.Append(" limit 0,1");
            Model.stock_codeInfo model = new Model.stock_codeInfo();
            DataSet ds = DbHelperMySQL.Query(strSql.ToString());

            if (ds.Tables[0].Rows.Count > 0)
            {
                return DataRowToModel(ds.Tables[0].Rows[0]);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * ");
            strSql.Append(" FROM c_stock_code ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return DbHelperMySQL.Query(strSql.ToString());
        }
        public DataSet GetSList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select id,stock_name,stock_code,concat(IF(SUBSTR(stock_code FROM 1 FOR 1)='6','sh','sz'),stock_code) as stock_code_s,concat(IF(SUBSTR(stock_code FROM 1 FOR 1)='6','1','0')) as stock_market FROM c_stock_code ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return DbHelperMySQL.Query(strSql.ToString());
        }
        public DataSet GetSysList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select id,stock_name,stock_code,concat(IF(SUBSTR(stock_code FROM 1 FOR 1)='6','sh','sz'),stock_code) as stock_code_s,concat(IF(SUBSTR(stock_code FROM 1 FOR 1)='6','1','0')) as stock_market FROM sys_stock_code ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return DbHelperMySQL.Query(strSql.ToString());
        }
        /// <summary>
        /// 获得前几行数据
        /// </summary>
        public DataSet GetList(int Top, string strWhere, string filedOrder)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ");
            strSql.Append(" * ");
            strSql.Append(" FROM c_stock_code ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            strSql.Append(" order by " + filedOrder);
            if (Top > 0)
            {
                strSql.Append(" limit 0," + Top.ToString());
            }
            return DbHelperMySQL.Query(strSql.ToString());
        }
        public DataSet GetList(string strWhere, string filedOrder, int pageSize, int pageIndex)
        {
            StringBuilder strSql = new StringBuilder();
            pageIndex = pageIndex - 1;
            if (pageIndex == 0)
            {
                strSql.AppendFormat("select * from c_stock_code {1} {2} limit 0,{0}", pageSize, strWhere.Length > 0 ? "where " + strWhere : "", filedOrder.Length > 0 ? "order by " + filedOrder : "");
            }
            else
            {

                strSql.AppendFormat("select * from c_stock_code {0} {1} limit {2},{3}",
                    strWhere.Length > 0 ? "where" + strWhere : "",
                    filedOrder.Length > 0 ? "order by " + filedOrder : "",
                    pageSize * pageIndex,
                    pageSize);
            }
            return DbHelperMySQL.Query(strSql.ToString());
        }
        public DataSet GetSList(string strWhere, string filedOrder, int pageSize, int pageIndex)
        {
            StringBuilder strSql = new StringBuilder();
            pageIndex = pageIndex - 1;
            if (pageIndex == 0)
            {
                strSql.AppendFormat("select id,stock_name,stock_code,concat(IF(SUBSTR(stock_code FROM 1 FOR 1)='6','sh','sz'),stock_code) as stock_code_s,concat(IF(SUBSTR(stock_code FROM 1 FOR 1)='6','1','0')) as stock_market from c_stock_code {1} {2} limit 0,{0}", pageSize, strWhere.Length > 0 ? "where " + strWhere : "", filedOrder.Length > 0 ? "order by " + filedOrder : "");
            }
            else
            {

                strSql.AppendFormat("select id,stock_name,stock_code,concat(IF(SUBSTR(stock_code FROM 1 FOR 1)='6','sh','sz'),stock_code) as stock_code_s,concat(IF(SUBSTR(stock_code FROM 1 FOR 1)='6','1','0')) as stock_market from c_stock_code {0} {1} limit {2},{3}",
                    strWhere.Length > 0 ? "where" + strWhere : "",
                    filedOrder.Length > 0 ? "order by " + filedOrder : "",
                    pageSize * pageIndex,
                    pageSize);
            }
            return DbHelperMySQL.Query(strSql.ToString());
        }

        public DataSet GetSysList(string strWhere, string filedOrder, int pageSize, int pageIndex)
        {
            StringBuilder strSql = new StringBuilder();
            pageIndex = pageIndex - 1;
            if (pageIndex == 0)
            {
                strSql.AppendFormat("select id,stock_name,stock_code,concat(IF(SUBSTR(stock_code FROM 1 FOR 1)='6','sh','sz'),stock_code) as stock_code_s,concat(IF(SUBSTR(stock_code FROM 1 FOR 1)='6','1','0')) as stock_market from sys_stock_code {1} {2} limit 0,{0}", pageSize, strWhere.Length > 0 ? "where " + strWhere : "", filedOrder.Length > 0 ? "order by " + filedOrder : "");
            }
            else
            {

                strSql.AppendFormat("select id,stock_name,stock_code,concat(IF(SUBSTR(stock_code FROM 1 FOR 1)='6','sh','sz'),stock_code) as stock_code_s,concat(IF(SUBSTR(stock_code FROM 1 FOR 1)='6','1','0')) as stock_market from sys_stock_code {0} {1} limit {2},{3}",
                    strWhere.Length > 0 ? "where" + strWhere : "",
                    filedOrder.Length > 0 ? "order by " + filedOrder : "",
                    pageSize * pageIndex,
                    pageSize);
            }
            return DbHelperMySQL.Query(strSql.ToString());
        }
        public int GetRecordCount(string strWhere)
        {
            string sql = "select count(1) from c_stock_code {0}";
            return Convert.ToInt32(DbHelperMySQL.GetSingle(string.Format(sql, strWhere.Length > 0 ? "where " + strWhere : "")));
        }

        /// <summary>
        /// 更新指定列数据
        /// </summary>
        public bool Update(int SysNo, string colValue)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update c_stock_code set ");
            if (colValue != string.Empty)
            {
                strSql.Append(colValue);
            }
            strSql.Append(" where id=" + SysNo);

            int rows = DbHelperMySQL.ExecuteSql(strSql.ToString());
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 将对象转换为实体
        /// </summary>
        public Model.stock_codeInfo DataRowToModel(DataRow row)
        {
            Model.stock_codeInfo model = new Model.stock_codeInfo();
            if (row != null)
            {
                if (row["id"] != null && row["id"].ToString() != "")
                {
                    model.id = int.Parse(row["id"].ToString());
                }
                model.stock_code = row["stock_code"].ToString();
                model.stock_name = row["stock_name"].ToString();
                model.stock_name_capital = row["stock_name_capital"].ToString();
                model.stock_board = row["stock_board"].ToString();
                if (row["circulation_value"] != null && row["circulation_value"].ToString() != "")
                {
                    model.circulation_value = decimal.Parse(row["circulation_value"].ToString());
                }
                model.circulation = row["circulation"].ToString();
                if (row["sort_id"] != null && row["sort_id"].ToString() != "")
                {
                    model.sort_id = int.Parse(row["sort_id"].ToString());
                }
                if (row["status"] != null && row["status"].ToString() != "")
                {
                    model.status = int.Parse(row["status"].ToString());
                }
            }
            return model;
        }
        #endregion

        #region 扩展方法
        public int GetSysEventLogCount(string strWhere)
        {
            string sql = "select count(1) from sys_event_log {0}";
            return Convert.ToInt32(DbHelperMySQL.GetSingle(string.Format(sql, strWhere.Length > 0 ? "where " + strWhere : "")));
        }
        /// <summary>
        /// 判断是否是交易时间(返回0表示是交易时间)
        /// </summary>
        /// <returns></returns>
        public int GetIsTradeTime()
        {
            DateTime dt = DateTime.Now;
            int is_week = 1, is_trade_time = 0;
            DateTime trade_time = Convert.ToDateTime(dt.ToString("HH:mm:ss"));
            if (trade_time >= Convert.ToDateTime("09:30:00") && trade_time <= Convert.ToDateTime("11:30:59"))
            {
                is_trade_time = 1;
            }
            if (trade_time >= Convert.ToDateTime("13:00:00") && trade_time <= Convert.ToDateTime("15:05:00"))
            {
                is_trade_time = 1;
            }

            if (is_trade_time == 1)
            {
                is_week = GetIsWeek(dt);
            }

            return is_week;
        }
        public int GetIsTradeTimeTline()
        {
            DateTime dt = DateTime.Now;
            int is_week = 1, is_trade_time = 0;
            DateTime trade_time = Convert.ToDateTime(dt.ToString("HH:mm:ss"));
            if (trade_time >= Convert.ToDateTime("09:30:00") && trade_time <= Convert.ToDateTime("11:30:59"))
            {
                is_trade_time = 1;
            }
            if (trade_time >= Convert.ToDateTime("13:00:00") && trade_time <= Convert.ToDateTime("15:00:59"))
            {
                is_trade_time = 1;
            }

            if (is_trade_time == 1)
            {
                is_week = GetIsWeek(dt);
            }

            return is_week;
        }
        /// <summary>
        /// 判断是否交易日(返回0表示是交易日)
        /// </summary>
        /// <returns></returns>
        public int GetIsWeek(DateTime dt)
        {
            MySqlParameter par1 = new MySqlParameter("@in_day", MySqlDbType.DateTime);
            par1.Value = dt.ToString("yyyy-MM-dd");
            par1.Direction = ParameterDirection.Input;

            MySqlParameter par2 = new MySqlParameter("@return_value", MySqlDbType.Int32, 11);
            par2.Direction = ParameterDirection.ReturnValue;
            MySqlParameter[] parameters = { par1, par2 };

            var sd = DbHelperMySQL.ExecuteScalar("f_isweek", parameters);

            return (int)par2.Value;
        }

        /// <summary>
        /// 批量运行语句
        /// </summary>
        /// <param name="SQLStringList"></param>
        /// <returns></returns>
        public int TranLot(List<string> SQLStringList)
        {
            return DbHelperMySQL.ExecuteSqlTran(SQLStringList);
        }
        /// <summary>
        /// 批量运行语句(其他库)
        /// </summary>
        /// <param name="SQLStringList"></param>
        /// <returns></returns>
        public int TranLotTo(List<string> SQLStringList)
        {
            return DbHelperMySQL.ExecuteSqlTranTo(SQLStringList);
        }
        #endregion
    }
}
