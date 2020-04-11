using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using  ORMCore;

namespace ORMCore
{
 public  abstract  class ObastractDbContext
    {
       
        #region 抽象增删改查 为抽象必须全部实现

        /// <summary>
        /// 自动生成带参数的SQL语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="returnIdentity"></param>
        /// <returns></returns>
        public abstract int InsertModelByParamSql<T>(T model, bool returnIdentity = false) where T : class, new();

        /// <summary>
        /// 通过存储过程添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="ProcedureName"></param>
        /// <returns></returns>
        public abstract int SaveByStoreProcedure<T>(T model, string ProcedureName) where T : class, new();

        /// <summary>
        /// 自动生成带参数的SQL语句完成对象修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public abstract int UpdateByParamSql<T>(T model) where T : class, new();

        /// <summary>
        /// 通过存储过程修改对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="procedureNane"></param>
        /// <returns></returns>
        public abstract int UpdateByStoreProcedure<T>(T model, string procedureNane) where T : class, new();

        /// <summary>
        /// 删除操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public abstract int DeleteModelByParamSql<T>(T model) where T : class, new();

        /// <summary>
        /// 查询实体操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public abstract List<T> GetEntitiesFromReader<T>(IDataReader reader) where T : new();
        #endregion

        #region ORM辅助相关方法

            /// <summary>
            /// 列特性的枚举
            /// </summary>
        protected enum ColumnAttribute
        {
            IdentityAttribute = 0,
            PrimaryKeyAttribute = 1,
            NonTableAttribute = 2
        }
        /// <summary>
        /// 根据一个实体的所有属性，找到符合特性条件的所有列
        /// </summary>
        /// <param name="properites"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        protected virtual  List<string> GetColumnsByAttribute(PropertyInfo[] properites, ColumnAttribute type)
        {
            List<string> columnList = new List<string>();
            foreach (var item in properites)
            {
                //获取当前属性中所定义的特性，然后再次遍历这个特性
                var attributes = item.GetCustomAttributes(false);
                foreach (var attribute in attributes)
                {
                    //判断自定义的特性和我们要找的特性是不是一致
                    if (attribute.GetType().Name.Equals(type.ToString()))
                    {
                        columnList.Add(item.Name);//将找到的列名称添加到集合
                        break;
                    }
                }
            }
            return columnList;
        }

        /// <summary>
        /// 过滤属性值不符合要求的属性列 NoTable
        /// </summary>
        /// <param name="properites"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        protected virtual List<string> PropertyValueFilter(PropertyInfo[] properites, object model)
        {
            List<string> columnList = new List<string>();
            foreach (var item in properites)
            {
                var pvalue = item.GetValue(model, null);
                if (pvalue == null)//过滤null
                {
                    columnList.Add(item.Name);
                    continue;
                }
                if (item.PropertyType == typeof(DateTime))//过滤时间的默认值值
                {
                    DateTime dt;
                    DateTime.TryParse(pvalue.ToString(), out dt);
                    if (dt < SqlDateTime.MinValue.Value)
                    {
                        columnList.Add(item.Name);
                    }
                }
                //思考一下可为空类型如何兼容一下...

                //其他扩展的，请在这个地方加...

            }
            return columnList;
        }

        /// <summary>
        /// 在实体中获取数据表名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        protected virtual string GetDbTableName<T>(T model)
        {
            //获取实体中这个 TableNameByAttribute 特性的数据
            object[] attArray = model.GetType().GetCustomAttributes(typeof(TableNameByAttribute), true);

            //没有的话肯定是它本身就是数据库表名了也就是实体类名字
            if (attArray == null || attArray.Length == 0)
            {
                return model.GetType().Name;
            }
            else
            {   //返回特性中的表名
                return ((TableNameByAttribute)attArray[0]).TableNmae;
            }
        }

        /// <summary>
        /// 执行模型中的验证特性validate()
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        protected virtual void ValidateModel<T>(T model) where T : class, new()
        {
            bool isContinue = true;//是否继续执行
            string errorMsg = string.Empty;//临时报错错误信息

            foreach (PropertyInfo property in model.GetType().GetProperties())//遍历这个实体中的所有属性
            {
                if (!isContinue)//下面报错了一个这里就不再遍历了
                {
                    break;
                }
                //找到当前属性中指定类型的自定义特性 如果这个属性没有实现这个特性又循环下一个
                object[] cusAttribute = property.GetCustomAttributes(typeof(CustomRequiredAttribute), true);

                //遍历自定义特性
                foreach (ObstractValidateAttribute attribute in cusAttribute)
                {
                    //转换成父类 因为这个属性已经实现了这个特性而且这个特性又继承自ValidateAttribute
                    //ValidateAttribute att = (ValidateAttribute)attribute;

                    bool isValid = attribute.Validate(property.GetValue(model));//调用重写的验证方法把实体的值传输过去
                    if (!isValid)//如果验证不通过
                    {
                        isContinue = false;//不在访问其他属性
                        errorMsg = attribute.ErrorMessage;
                        break;
                    }
                }
            }
            //如果有验证特性没通过就直接报错
            if (errorMsg.Length > 0)
            {
                throw new Exception($"{ model.GetType().Name } ValidateModel: {errorMsg}");
            }
        }

        #endregion
    }
}
