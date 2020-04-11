using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace DAL.MySQL
{
    public class ModelT1Service : IModelT1Service
    {
        private ORMCore.MySqlDBContext DBContext = new ORMCore.MySqlDBContext();

        /// <summary>
        /// 插入模型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddModelT1(ModelT1 model)
        {
            return DBContext.InsertModelByParamSql(model, false);
        }
        public List<ModelT1> QueryModelList()
        {
            //可以考虑用Linq生成Sql，这样就进一步封装到dbContext里面
            string sql = "select * from T1";

            IDataReader reader = ORMCore.HelperFactory.MySQLHelper.GetReader(sql, null);

            return DBContext.GetEntitiesFromReader<Models.ModelT1>(reader);
        }

        public int DeleteModelT1(ModelT1 model)
        {
            throw new NotImplementedException();
        }

        public int UpdateModelT1(ModelT1 model)
        {
            throw new NotImplementedException();
        }
    }
}
