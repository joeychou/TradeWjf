using System;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using DBUtility;
namespace DAL
{
    //sys_stock_code
    public partial class s_stock_codeDAL
    {
        public bool Exists(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from sys_stock_code");
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
        public int Add(Model.s_stock_codeInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into sys_stock_code(");
            strSql.Append("stock_code,stock_name,stock_name_capital,rd_decl,rd_deal,rd_num,rd_money,rd_txt,remark,input_time");
            strSql.Append(") values (");
            strSql.Append("@stock_code,@stock_name,@stock_name_capital,@rd_decl,@rd_deal,@rd_num,@rd_money,@rd_txt,@remark,@input_time");
            strSql.Append("); Select LAST_INSERT_ID() ");

            MySqlParameter[] parameters = {
                        new MySqlParameter("@stock_code", MySqlDbType.VarChar,8) ,
                        new MySqlParameter("@stock_name", MySqlDbType.VarChar,8) ,
                        new MySqlParameter("@stock_name_capital", MySqlDbType.VarChar,8) ,
                        new MySqlParameter("@rd_decl", MySqlDbType.DateTime) ,
                        new MySqlParameter("@rd_deal", MySqlDbType.DateTime) ,
                        new MySqlParameter("@rd_num", MySqlDbType.Decimal,12) ,
                        new MySqlParameter("@rd_money", MySqlDbType.Decimal,12) ,
                        new MySqlParameter("@rd_txt", MySqlDbType.VarChar,64) ,
                        new MySqlParameter("@remark", MySqlDbType.VarChar,128) ,
                        new MySqlParameter("@input_time", MySqlDbType.DateTime)

            };

            parameters[0].Value = model.stock_code;
            parameters[1].Value = model.stock_name;
            parameters[2].Value = model.stock_name_capital;
            parameters[3].Value = model.rd_decl;
            parameters[4].Value = model.rd_deal;
            parameters[5].Value = model.rd_num;
            parameters[6].Value = model.rd_money;
            parameters[7].Value = model.rd_txt;
            parameters[8].Value = model.remark;
            parameters[9].Value = model.input_time;
            int rows = Convert.ToInt32(DbHelperMySQL.GetSingle(strSql.ToString(), parameters));
            return rows;
        }


        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Model.s_stock_codeInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update sys_stock_code set ");

            strSql.Append(" stock_code = @stock_code , ");
            strSql.Append(" stock_name = @stock_name , ");
            strSql.Append(" stock_name_capital = @stock_name_capital , ");
            strSql.Append(" rd_decl = @rd_decl , ");
            strSql.Append(" rd_deal = @rd_deal , ");
            strSql.Append(" rd_num = @rd_num , ");
            strSql.Append(" rd_money = @rd_money , ");
            strSql.Append(" rd_txt = @rd_txt , ");
            strSql.Append(" remark = @remark , ");
            strSql.Append(" input_time = @input_time  ");
            strSql.Append(" where id=@id ");
            MySqlParameter[] parameters = {
                        new MySqlParameter("@stock_code", MySqlDbType.VarChar,8) ,
                        new MySqlParameter("@stock_name", MySqlDbType.VarChar,8) ,
                        new MySqlParameter("@stock_name_capital", MySqlDbType.VarChar,8) ,
                        new MySqlParameter("@rd_decl", MySqlDbType.DateTime) ,
                        new MySqlParameter("@rd_deal", MySqlDbType.DateTime) ,
                        new MySqlParameter("@rd_num", MySqlDbType.Decimal,12) ,
                        new MySqlParameter("@rd_money", MySqlDbType.Decimal,12) ,
                        new MySqlParameter("@rd_txt", MySqlDbType.VarChar,64) ,
                        new MySqlParameter("@remark", MySqlDbType.VarChar,128) ,
                        new MySqlParameter("@input_time", MySqlDbType.DateTime) ,

            new MySqlParameter("@id", MySqlDbType.Int32,11)};
            parameters[0].Value = model.stock_code;
            parameters[1].Value = model.stock_name;
            parameters[2].Value = model.stock_name_capital;
            parameters[3].Value = model.rd_decl;
            parameters[4].Value = model.rd_deal;
            parameters[5].Value = model.rd_num;
            parameters[6].Value = model.rd_money;
            parameters[7].Value = model.rd_txt;
            parameters[8].Value = model.remark;
            parameters[9].Value = model.input_time;
            parameters[10].Value = model.id;

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
            strSql.Append("delete from sys_stock_code ");
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
        public Model.s_stock_codeInfo GetModel(int id)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select id, stock_code, stock_name, stock_name_capital, rd_decl, rd_deal, rd_num, rd_money, rd_txt, remark, input_time  ");
            strSql.Append("  from sys_stock_code ");
            strSql.Append(" where id=@id");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@id", MySqlDbType.Int32)
};
            parameters[0].Value = id;


            Model.s_stock_codeInfo model = new Model.s_stock_codeInfo();
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
        public Model.s_stock_codeInfo GetModel(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select id, stock_code, stock_name, stock_name_capital, rd_decl, rd_deal, rd_num, rd_money, rd_txt, remark, input_time  ");
            strSql.Append("  from sys_stock_code ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            strSql.Append(" limit 0,1");
            Model.s_stock_codeInfo model = new Model.s_stock_codeInfo();
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
            strSql.Append(" FROM sys_stock_code ");
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
            strSql.Append(" FROM sys_stock_code ");
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
                strSql.AppendFormat("select * from sys_stock_code {1} {2} limit 0,{0}", pageSize, strWhere.Length > 0 ? "where " + strWhere : "", filedOrder.Length > 0 ? "order by " + filedOrder : "");
            }
            else
            {

                strSql.AppendFormat("select * from sys_stock_code {0} {1} limit {2},{3}",
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
                strSql.AppendFormat("SELECT DISTINCT stock_code,concat(IF(SUBSTR(stock_code FROM 1 FOR 1)='6','sh','sz'),stock_code) as stock_code_s,yesterday_price FROM sys_stock_code {1} {2} limit 0,{0}", pageSize, strWhere.Length > 0 ? "where " + strWhere : "", filedOrder.Length > 0 ? "order by " + filedOrder : "");
            }
            else
            {

                strSql.AppendFormat("SELECT DISTINCT stock_code,concat(IF(SUBSTR(stock_code FROM 1 FOR 1)='6','sh','sz'),stock_code) as stock_code_s,yesterday_price FROM sys_stock_code {0} {1} limit {2},{3}",
                    strWhere.Length > 0 ? "where" + strWhere : "",
                    filedOrder.Length > 0 ? "order by " + filedOrder : "",
                    pageSize * pageIndex,
                    pageSize);
            }
            return DbHelperMySQL.Query(strSql.ToString());
        }
        public int GetRecordCount(string strWhere)
        {
            string sql = "select count(1) from sys_stock_code {0}";
            return Convert.ToInt32(DbHelperMySQL.GetSingle(string.Format(sql, strWhere.Length > 0 ? "where " + strWhere : "")));
        }
        public int GetSRecordCount(string strWhere)
        {
            string sql = "select count(1) from sys_stock_code {0}";
            return Convert.ToInt32(DbHelperMySQL.GetSingle(string.Format(sql, strWhere.Length > 0 ? "where " + strWhere : "")));
        }
        public int GetSRecordCountTo(string strWhere)
        {
            string sql = "select count(1) from sys_stock_code {0}";
            return Convert.ToInt32(DbHelperMySQL.GetSingleTo(string.Format(sql, strWhere.Length > 0 ? "where " + strWhere : "")));
        }
        /// <summary>
        /// 更新指定列数据
        /// </summary>
        public bool Update(int SysNo, string colValue)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update sys_stock_code set ");
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
        public Model.s_stock_codeInfo DataRowToModel(DataRow row)
        {
            Model.s_stock_codeInfo model = new Model.s_stock_codeInfo();
            if (row != null)
            {
                if (row["id"] != null && row["id"].ToString() != "")
                {
                    model.id = int.Parse(row["id"].ToString());
                }
                model.stock_code = row["stock_code"].ToString();
                model.stock_name = row["stock_name"].ToString();
                model.stock_name_capital = row["stock_name_capital"].ToString();
                if (row["rd_decl"] != null && row["rd_decl"].ToString() != "")
                {
                    model.rd_decl = DateTime.Parse(row["rd_decl"].ToString());
                }
                if (row["rd_deal"] != null && row["rd_deal"].ToString() != "")
                {
                    model.rd_deal = DateTime.Parse(row["rd_deal"].ToString());
                }
                if (row["rd_num"] != null && row["rd_num"].ToString() != "")
                {
                    model.rd_num = decimal.Parse(row["rd_num"].ToString());
                }
                if (row["rd_money"] != null && row["rd_money"].ToString() != "")
                {
                    model.rd_money = decimal.Parse(row["rd_money"].ToString());
                }
                model.rd_txt = row["rd_txt"].ToString();
                model.remark = row["remark"].ToString();
                if (row["input_time"] != null && row["input_time"].ToString() != "")
                {
                    model.input_time = DateTime.Parse(row["input_time"].ToString());
                }
            }
            return model;
        }
    }
}
