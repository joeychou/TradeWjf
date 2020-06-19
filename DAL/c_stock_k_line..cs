using System; 
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic; 
using MySql.Data.MySqlClient;
using DBUtility;
namespace DAL  
{
	 	//c_stock_k_line
		public partial class stock_k_lineDAL
	{
		public bool Exists(int id)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) from c_stock_k_line");
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
		public int Add(Model.stock_k_lineInfo model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into c_stock_k_line(");			
            strSql.Append("stock_code,stock_name,update_time,open,preClose,highest,lowest,deal_vol,deal_price,fprice");
			strSql.Append(") values (");
            strSql.Append("@stock_code,@stock_name,@update_time,@open,@preClose,@highest,@lowest,@deal_vol,@deal_price,@fprice");            
            strSql.Append("); Select LAST_INSERT_ID() ");            
	
			MySqlParameter[] parameters = {
			            new MySqlParameter("@stock_code", MySqlDbType.VarChar,12) ,            
                        new MySqlParameter("@stock_name", MySqlDbType.VarChar,32) ,            
                        new MySqlParameter("@update_time", MySqlDbType.DateTime) ,            
                        new MySqlParameter("@open", MySqlDbType.Decimal,13) ,            
                        new MySqlParameter("@preClose", MySqlDbType.Decimal,13) ,            
                        new MySqlParameter("@highest", MySqlDbType.Decimal,13) ,            
                        new MySqlParameter("@lowest", MySqlDbType.Decimal,13) ,            
                        new MySqlParameter("@deal_vol", MySqlDbType.Int32,11) ,            
                        new MySqlParameter("@deal_price", MySqlDbType.Decimal,13) ,            
                        new MySqlParameter("@fprice", MySqlDbType.Decimal,13)             
              
            };
			            
            parameters[0].Value = model.stock_code;                        
            parameters[1].Value = model.stock_name;                        
            parameters[2].Value = model.update_time;                        
            parameters[3].Value = model.open;                        
            parameters[4].Value = model.preClose;                        
            parameters[5].Value = model.highest;                        
            parameters[6].Value = model.lowest;                        
            parameters[7].Value = model.deal_vol;                        
            parameters[8].Value = model.deal_price;                        
            parameters[9].Value = model.fprice;                        
            int rows = Convert.ToInt32(DbHelperMySQL.GetSingle(strSql.ToString(), parameters));
            return rows;
		}
		
		
		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(Model.stock_k_lineInfo model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update c_stock_k_line set ");
			                                                
            strSql.Append(" stock_code = @stock_code , ");                                    
            strSql.Append(" stock_name = @stock_name , ");                                    
            strSql.Append(" update_time = @update_time , ");                                    
            strSql.Append(" open = @open , ");                                    
            strSql.Append(" preClose = @preClose , ");                                    
            strSql.Append(" highest = @highest , ");                                    
            strSql.Append(" lowest = @lowest , ");                                    
            strSql.Append(" deal_vol = @deal_vol , ");                                    
            strSql.Append(" deal_price = @deal_price , ");                                    
            strSql.Append(" fprice = @fprice  ");            			
			strSql.Append(" where id=@id ");			
			MySqlParameter[] parameters = {
			            new MySqlParameter("@stock_code", MySqlDbType.VarChar,12) ,      
                        new MySqlParameter("@stock_name", MySqlDbType.VarChar,32) ,      
                        new MySqlParameter("@update_time", MySqlDbType.DateTime) ,      
                        new MySqlParameter("@open", MySqlDbType.Decimal,13) ,      
                        new MySqlParameter("@preClose", MySqlDbType.Decimal,13) ,      
                        new MySqlParameter("@highest", MySqlDbType.Decimal,13) ,      
                        new MySqlParameter("@lowest", MySqlDbType.Decimal,13) ,      
                        new MySqlParameter("@deal_vol", MySqlDbType.Int32,11) ,      
                        new MySqlParameter("@deal_price", MySqlDbType.Decimal,13) ,      
                        new MySqlParameter("@fprice", MySqlDbType.Decimal,13) ,      
              
            new MySqlParameter("@id", MySqlDbType.Int32,11)};
                        parameters[0].Value = model.stock_code;
                        parameters[1].Value = model.stock_name;
                        parameters[2].Value = model.update_time;
                        parameters[3].Value = model.open;
                        parameters[4].Value = model.preClose;
                        parameters[5].Value = model.highest;
                        parameters[6].Value = model.lowest;
                        parameters[7].Value = model.deal_vol;
                        parameters[8].Value = model.deal_price;
                        parameters[9].Value = model.fprice;
            			parameters[10].Value = model.id;
            
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
			strSql.Append("delete from c_stock_k_line ");
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
		public Model.stock_k_lineInfo GetModel(int id)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select id, stock_code, stock_name, update_time, open, preClose, highest, lowest, deal_vol, deal_price, fprice  ");			
			strSql.Append("  from c_stock_k_line ");
			strSql.Append(" where id=@id");
						MySqlParameter[] parameters = {
					new MySqlParameter("@id", MySqlDbType.Int32)
};
			parameters[0].Value = id;

			
			Model.stock_k_lineInfo model=new Model.stock_k_lineInfo();
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
		public Model.stock_k_lineInfo GetModel(string strWhere)
		{	
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select id, stock_code, stock_name, update_time, open, preClose, highest, lowest, deal_vol, deal_price, fprice  ");			
			strSql.Append("  from c_stock_k_line ");
			if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
			strSql.Append(" limit 0,1");
			Model.stock_k_lineInfo model=new Model.stock_k_lineInfo();
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
			strSql.Append(" FROM c_stock_k_line ");
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
			strSql.Append(" FROM c_stock_k_line ");
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
                strSql.AppendFormat("select * from c_stock_k_line {1} {2} limit 0,{0}", pageSize, strWhere.Length > 0 ? "where " + strWhere : "", filedOrder.Length > 0 ? "order by " + filedOrder : "");
            }
            else
            {

                strSql.AppendFormat("select * from c_stock_k_line {0} {1} limit {2},{3}",
                    strWhere.Length > 0 ? "where" + strWhere : "",
                    filedOrder.Length > 0 ? "order by " + filedOrder : "",
                    pageSize * pageIndex,
                    pageSize);
            }
            return DbHelperMySQL.Query(strSql.ToString());
        }
        
        public int GetRecordCount(string strWhere)
        {
            string sql = "select count(1) from c_stock_k_line {0}";
            return Convert.ToInt32(DbHelperMySQL.GetSingle(string.Format(sql, strWhere.Length > 0 ? "where " + strWhere : "")));
        }
        
        /// <summary>
        /// 更新指定列数据
        /// </summary>
        public bool Update(int SysNo, string colValue)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update c_stock_k_line set ");
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
        public Model.stock_k_lineInfo DataRowToModel(DataRow row)
        {
            Model.stock_k_lineInfo model=new Model.stock_k_lineInfo();
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
																																if(row["open"]!=null && row["open"].ToString()!="")
				{
					model.open=decimal.Parse(row["open"].ToString());
				}
																																if(row["preClose"]!=null && row["preClose"].ToString()!="")
				{
					model.preClose=decimal.Parse(row["preClose"].ToString());
				}
																																if(row["highest"]!=null && row["highest"].ToString()!="")
				{
					model.highest=decimal.Parse(row["highest"].ToString());
				}
																																if(row["lowest"]!=null && row["lowest"].ToString()!="")
				{
					model.lowest=decimal.Parse(row["lowest"].ToString());
				}
																																if(row["deal_vol"]!=null && row["deal_vol"].ToString()!="")
				{
					model.deal_vol=int.Parse(row["deal_vol"].ToString());
				}
																																if(row["deal_price"]!=null && row["deal_price"].ToString()!="")
				{
					model.deal_price=decimal.Parse(row["deal_price"].ToString());
				}
																																if(row["fprice"]!=null && row["fprice"].ToString()!="")
				{
					model.fprice=decimal.Parse(row["fprice"].ToString());
				}
																								            }
            return model;
        }
	}
}
