using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
using Model;
namespace BLL
{
    //sys_stock_code
    public partial class s_stock_codeBLL
    {

        private readonly DAL.s_stock_codeDAL dal = new DAL.s_stock_codeDAL();
        public s_stock_codeBLL()
        { }

        #region  基本方法===============================
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int id)
        {
            return dal.Exists(id);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(Model.s_stock_codeInfo model)
        {
            return dal.Add(model);

        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Model.s_stock_codeInfo model)
        {
            return dal.Update(model);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int id)
        {
            return dal.Delete(id);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.s_stock_codeInfo GetModel(int id)
        {
            return dal.GetModel(id);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.s_stock_codeInfo GetModel(string strWhere)
        {
            return dal.GetModel(strWhere);
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(string strWhere)
        {
            return dal.GetList(strWhere);
        }
        /// <summary>
        /// 获得前几行数据
        /// </summary>
        public DataSet GetList(int Top, string strWhere, string filedOrder)
        {
            return dal.GetList(Top, strWhere, filedOrder);
        }
        /// <summary>
        /// 分页
        /// </summary>
        public DataSet GetList(string strWhere, string filedOrder, int pageSize, int pageIndex)
        {
            return dal.GetList(strWhere, filedOrder, pageSize, pageIndex);
        }
        /// <summary>
        /// 记录数量
        /// </summary>
        public int GetRecordCount(string strWhere)
        {
            return dal.GetRecordCount(strWhere);
        }
        /// <summary>
        /// 更新指定列的数据
        /// </summary>
        /// <param name="NewsSysNo">列ID</param>
        /// <param name="colName">列名=值</param>
        /// <returns></returns>
        public bool Update(int NewsSysNo, string colValue)
        {
            return dal.Update(NewsSysNo, colValue);
        }
        #endregion
    }
}