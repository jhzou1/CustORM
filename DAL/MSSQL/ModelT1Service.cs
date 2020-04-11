using ORMCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.MSSQL
{
    public class ModelT1Service:IModelT1Service
    {
        protected ORMCore.MSDbContext dbContext = new ORMCore.MSDbContext();

        /// <summary>
        /// 通过ORM添加模型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddModelT1(Models.ModelT1 model)
        {
            //[通过生成SQL添加]
            //return dbContext.InsertModelByParamSql(model,true);

            //通过存储过程添加
            return dbContext.SaveByStoreProcedure(model, "AddModelForT1");
        }

        /// <summary>
        /// 修改更新实体对象数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int UpdateModelT1(Models.ModelT1 model)
        {
            //return dbContext.UpdateByParamSql(model);

            return dbContext.UpdateByStoreProcedure(model, "ModifyModelT1");
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int DeleteModelT1(Models.ModelT1 model)
        {
            return dbContext.DeleteModelByParamSql(model);
        }

        /// <summary>
        /// 查询所有的ModelList
        /// </summary>
        /// <returns></returns>
        public List<Models.ModelT1> QueryModelList()
        {
            //可以考虑用Linq生成Sql，这样就进一步封装到dbContext里面
            string sql = "select * from T1";

            IDataReader reader =HelperFactory.MSSQLHelper.GetReader(sql,null);

            return dbContext.GetEntitiesFromReader<Models.ModelT1>(reader);
        }
    }
}
