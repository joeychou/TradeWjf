using System; 
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic; 
using MySql.Data.MySqlClient;
using DBUtility;
namespace DAL  
{
	 	//c_stock_ex
		public partial class stock_exDAL
	{
		public bool Exists(int id)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) from c_stock_ex");
			strSql.Append(" where ");
			                                       strSql.Append(" id = @id  ");
                            						MySqlParameter[] parameters = {
					new MySqlParameter("@id", MySqlDbType.Int32)};
			parameters[0].Value = id;

			return DbHelperMySQL.Exists(strSql.ToString(),parameters);
		}
		
				
		
		/// <summary>
		/// 增加一条数据
		/// </summary>
		public int Add(Model.stock_exInfo model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into c_stock_ex(");			
            strSql.Append("stock_code,stock_name,update_time,time,fgive,fpei,fpei_price,fprofit");
			strSql.Append(") values (");
            strSql.Append("@stock_code,@stock_name,@update_time,@time,@fgive,@fpei,@fpei_price,@fprofit");            
            strSql.Append("); Select LAST_INSERT_ID() ");            
	
			MySqlParameter[] parameters = {
			            new MySqlParameter("@stock_code", MySqlDbType.VarChar,12) ,            
                        new MySqlParameter("@stock_name", MySqlDbType.VarChar,32) ,            
                        new MySqlParameter("@update_time", MySqlDbType.DateTime) ,            
                        new MySqlParameter("@time", MySqlDbType.Int32,11) ,            
                        new MySqlParameter("@fgive", MySqlDbType.Decimal,13) ,            
                        new MySqlParameter("@fpei", MySqlDbType.Decimal,13) ,            
                        new MySqlParameter("@fpei_price", MySqlDbType.Decimal,13) ,            
                        new MySqlParameter("@fprofit", MySqlDbType.Decimal,13)             
              
            };
			            
            parameters[0].Value = model.stock_code;                        
            parameters[1].Value = model.stock_name;                        
            parameters[2].Value = model.update_time;                        
            parameters[3].Value = model.time;                        
            parameters[4].Value = model.fgive;                        
            parameters[5].Value = model.fpei;                        
            parameters[6].Value = model.fpei_price;                        
            parameters[7].Value = model.fprofit;                        
            int rows = Convert.ToInt32(DbHelperMySQL.GetSingle(strSql.ToString(), parameters));
            return rows;
		}
		
		
		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(Model.stock_exInfo model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update c_stock_ex set ");
			                                                
            strSql.Append(" stock_code = @stock_code , ");                                    
            strSql.Append(" stock_name = @stock_name , ");                                    
            strSql.Append(" update_time = @update_time , ");                                    
            strSql.Append(" time = @time , ");                                    
            strSql.Append(" fgive = @fgive , ");                                    
            strSql.Append(" fpei = @fpei , ");                                    
            strSql.Append(" fpei_price = @fpei_price , ");                                    
            strSql.Append(" fprofit = @fprofit  ");            			
			strSql.Append(" where id=@id ");			
			MySqlParameter[] parameters = {
			            new MySqlParameter("@stock_code", MySqlDbType.VarChar,12) ,      
                        new MySqlParameter("@stock_name", MySqlDbType.VarChar,32) ,      
                        new MySqlParameter("@update_time", MySqlDbType.DateTime) ,      
                        new MySqlParameter("@time", MySqlDbType.Int32,11) ,      
                        new MySqlParameter("@fgive", MySqlDbType.Decimal,13) ,      
                        new MySqlParameter("@fpei", MySqlDbType.Decimal,13) ,      
                        new MySqlParameter("@fpei_price", MySqlDbType.Decimal,13) ,      
                        new MySqlParameter("@fprofit", MySqlDbType.Decimal,13) ,      
              
            new MySqlParameter("@id", MySqlDbType.Int32,11)};
                        parameters[0].Value = model.stock_code;
                        parameters[1].Value = model.stock_name;
                        parameters[2].Value = model.update_time;
                        parameters[3].Value = model.time;
                        parameters[4].Value = model.fgive;
                        parameters[5].Value = model.fpei;
                        parameters[6].Value = model.fpei_price;
                        parameters[7].Value = model.fprofit;
            			parameters[8].Value = model.id;
            
            int rows=DbHelperMySQL.ExecuteSql(strSql.ToString(),parameters);
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
			StringBuilder strSql=new StringBuilder();
			strSql.Append("delete from c_stock_ex ");
			strSql.Append(" where id=@id");
						MySqlParameter[] parameters = {
					new MySqlParameter("@id", MySqlDbType.Int32)
};
			parameters[0].Value = id;

			int rows=DbHelperMySQL.ExecuteSql(strSql.ToString(),parameters);
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
		public Model.stock_exInfo GetModel(int id)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select id, stock_code, stock_name, update_time, time, fgive, fpei, fpei_price, fprofit  ");			
			strSql.Append("  from c_stock_ex ");
			strSql.Append(" where id=@id");
						MySqlParameter[] parameters = {
					new MySqlParameter("@id", MySqlDbType.Int32)
};
			parameters[0].Value = id;

			
			Model.stock_exInfo model=new Model.stock_exInfo();
			DataSet ds=DbHelperMySQL.Query(strSql.ToString(),parameters);
			
			if(ds.Tables[0].Rows.Count>0)
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
		public Model.stock_exInfo GetModel(string strWhere)
		{	
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select id, stock_code, stock_name, update_time, time, fgive, fpei, fpei_price, fprofit  ");			
			strSql.Append("  from c_stock_ex ");
			if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
			strSql.Append(" limit 0,1");
			Model.stock_exInfo model=new Model.stock_exInfo();
			DataSet ds=DbHelperMySQL.Query(strSql.ToString());
			
			if(ds.Tables[0].Rows.Count>0)
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
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select * ");
			strSql.Append(" FROM c_stock_ex ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}
			return DbHelperMySQL.Query(strSql.ToString());
		}
		
		/// <summary>
		/// 获得前几行数据
		/// </summary>
		public DataSet GetList(int Top,string strWhere,string filedOrder)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select ");
			strSql.Append(" * ");
			strSql.Append(" FROM c_stock_ex ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
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
                strSql.AppendFormat("select * from c_stock_ex {1} {2} limit 0,{0}", pageSize, strWhere.Length > 0 ? "where " + strWhere : "", filedOrder.Length > 0 ? "order by " + filedOrder : "");
            }
            else
            {

                strSql.AppendFormat("select * from c_stock_ex {0} {1} limit {2},{3}",
                    strWhere.Length > 0 ? "where" + strWhere : "",
                    filedOrder.Length > 0 ? "order by " + filedOrder : "",
                    pageSize * pageIndex,
                    pageSize);
            }
            return DbHelperMySQL.Query(strSql.ToString());
        }
        
        public int GetRecordCount(string strWhere)
        {
            string sql = "select count(1) from c_stock_ex {0}";
            return Convert.ToInt32(DbHelperMySQL.GetSingle(string.Format(sql, strWhere.Length > 0 ? "where " + strWhere : "")));
        }
        
        /// <summary>
        /// 更新指定列数据
        /// </summary>
        public bool Update(int SysNo, string colValue)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update c_stock_ex set ");
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
        public Model.stock_exInfo DataRowToModel(DataRow row)
        {
            Model.stock_exInfo model=new Model.stock_exInfo();
            if (row != null)
            {
											if(row["id"]!=null && row["id"].ToString()!="")
				{
					model.id=int.Parse(row["id"].ToString());
				}
																																				model.stock_code= row["stock_code"].ToString();
																																model.stock_name= row["stock_name"].ToString();
																												if(row["update_time"]!=null && row["update_time"].ToString()!="")
				{
					model.update_time=DateTime.Parse(row["update_time"].ToString());
				}
																																if(row["time"]!=null && row["time"].ToString()!="")
				{
					model.time=int.Parse(row["time"].ToString());
				}
																																if(row["fgive"]!=null && row["fgive"].ToString()!="")
				{
					model.fgive=decimal.Parse(row["fgive"].ToString());
				}
																																if(row["fpei"]!=null && row["fpei"].ToString()!="")
				{
					model.fpei=decimal.Parse(row["fpei"].ToString());
				}
																																if(row["fpei_price"]!=null && row["fpei_price"].ToString()!="")
				{
					model.fpei_price=decimal.Parse(row["fpei_price"].ToString());
				}
																																if(row["fprofit"]!=null && row["fprofit"].ToString()!="")
				{
					model.fprofit=decimal.Parse(row["fprofit"].ToString());
				}
																								            }
            return model;
        }
	}
}
