using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ORMCore
{
    /// <summary>
    /// 访问数据库上下文，jhzou 2020-03-31
    /// 
    /// 未解决问题 如何在DbContext中解决不同数据库的语法问题，毕竟有可能生成的Sql不通用!
    /// </summary>
 public    class MSDbContext: ObastractDbContext
    {

        #region 通过带参数SQL语句 Insert
        /// <summary>
        /// 自动生成带参数的SQL语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model">实体对象</param>
        /// <param name="returnIdentity">是否返回标识列的值</param>
        /// <returns></returns>
        public override int InsertModelByParamSql<T>(T model, bool returnIdentity = false)
        {
            //先验证模型
            base.ValidateModel(model);

            //【1】定义要组合的SQL语句（请大家自己完成，能够自动读取表的别名内容）
            StringBuilder sqlFields = new StringBuilder($"insert into {base.GetDbTableName<T>(model)} (");
            StringBuilder sqlValues = new StringBuilder(" values (");
            List<SqlParameter> paramList = new List<SqlParameter>();

            //【2】获取对象所有的属性，并找到我们需要过滤的特性列
            PropertyInfo[] properties = model.GetType().GetProperties();

            List<string> attrfilter = base.GetColumnsByAttribute(properties, ColumnAttribute.IdentityAttribute);
            attrfilter.AddRange(base.GetColumnsByAttribute(properties, ColumnAttribute.NonTableAttribute));

            //【3】考虑过滤其他的特别属性（null、有的时间默认值、考虑一下....)
            attrfilter.AddRange(PropertyValueFilter(properties, model));

            //【4】循环组合字段和字段值
            foreach (var item in properties)
            {
                if (attrfilter.Contains(item.Name)) continue;
                //组合SQL语句中的字段名和参数名称
                sqlFields.Append(item.Name + ",");
                sqlValues.Append($"@{item.Name},");

                //将参数添加到参数集合中
                paramList.Add(new SqlParameter($"@{item.Name }", item.GetValue(model, null)));
            }

            //【5】组合完整的SQL语句
            string fields = sqlFields.ToString().TrimEnd(',') + ")";
            string values = sqlValues.ToString().ToString().TrimEnd(',') + ")";
            string sql = fields + values;

            //【6】判断是否需要返回标识列
            if (returnIdentity)
            {
                sql += ";select @@Identity";

                //交给实现特定数据库的Helper来执行
                return Convert.ToInt32(HelperFactory.MSSQLHelper.GetSingleResult(sql, paramList.ToArray()));
            }
            else
            {
                return  HelperFactory.MSSQLHelper.Update(sql, paramList.ToArray());
            }
        }
        #endregion

        #region 通过存储过程添加
        /// <summary>
        /// 通过存储过程添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="ProcedureName"></param>
        /// <returns></returns>
        public override int SaveByStoreProcedure<T>(T model, string ProcedureName)
        {
            //先验证模型
            base.ValidateModel(model);

            PropertyInfo[] properties = model.GetType().GetProperties();

            List<string> attrfilter = base.GetColumnsByAttribute(properties, ColumnAttribute.IdentityAttribute);
            attrfilter.AddRange(base.GetColumnsByAttribute(properties, ColumnAttribute.NonTableAttribute));


            List<SqlParameter> paramList = new List<SqlParameter>();
            foreach (PropertyInfo item in properties)
            {
                if (attrfilter.Contains(item.Name)) continue;

                paramList.Add(new SqlParameter("@" + item.Name, item.GetValue(model, null)));
            }
            return HelperFactory.MSSQLHelper.Update(ProcedureName, paramList.ToArray(), true);
        }
        #endregion

        #region 自动生成带参数的SQL语句完成对象修改
        /// <summary>
        /// 自动生成带参数的SQL语句完成对象修改 
        /// 
        /// jhzou20200402:这里我们应该考虑下如果修改了单一字段能不能只生成修改单一字段的SQL?
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public override int UpdateByParamSql<T>(T model)
        {
            //同样需要验证
            base.ValidateModel(model);

            StringBuilder sqlField = new StringBuilder($"update {base.GetDbTableName<T>(model)} set ");
            List<SqlParameter> paramList = new List<SqlParameter>();

            PropertyInfo[] properties = model.GetType().GetProperties();
            List<string> attrfilter = GetColumnsByAttribute(properties, ColumnAttribute.IdentityAttribute);
            attrfilter.AddRange(GetColumnsByAttribute(properties, ColumnAttribute.NonTableAttribute));

            //(多主键需要考虑...）
            string primaryKey = GetColumnsByAttribute(properties, ColumnAttribute.PrimaryKeyAttribute)[0];

            foreach (var item in properties)
            {
                if (primaryKey.Equals(item.Name))
                {
                    //这里获取主键值
                    paramList.Add(new SqlParameter($"@{primaryKey}", item.GetValue(model, null)));
                    continue;
                }
                if (attrfilter.Contains(item.Name)) continue;

                sqlField.Append($"{item.Name }=@{item.Name },");
                paramList.Add(new SqlParameter($"@{item.Name}", item.GetValue(model, null)));
            }

            string sql = sqlField.ToString().TrimEnd(',') + $" where {primaryKey}=@{primaryKey}";

            return HelperFactory.MSSQLHelper.Update(sql, paramList.ToArray());
        }
        #endregion

        #region 通过存储过程修改
        /// <summary>
        /// 通过存储过程修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="procedureNane"></param>
        /// <returns></returns>
        public override int UpdateByStoreProcedure<T>(T model, string ProcedureName)
        {
            //同样需要验证
            base.ValidateModel(model);

            PropertyInfo[] properties = model.GetType().GetProperties();

            List<string> attrfilter = GetColumnsByAttribute(properties, ColumnAttribute.IdentityAttribute);
            attrfilter.AddRange(GetColumnsByAttribute(properties, ColumnAttribute.NonTableAttribute));


            List<SqlParameter> paramList = new List<SqlParameter>();
            foreach (PropertyInfo item in properties)
            {
                //过滤掉不符合的
                if (attrfilter.Contains(item.Name)) continue;

                paramList.Add(new SqlParameter("@" + item.Name, item.GetValue(model, null)));  //将参数添加到参数集合
            }

            //调用通用数据访问类
            return HelperFactory.MSSQLHelper.Update(ProcedureName, paramList.ToArray(), true);
        }

        #endregion

        #region 封装Delete操作
        /// <summary>
        /// 封装Delete操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public override int DeleteModelByParamSql<T>(T model)
        {
            PropertyInfo[] properties = model.GetType().GetProperties();
            string primaryKey = GetColumnsByAttribute(properties, ColumnAttribute.PrimaryKeyAttribute)[0];
            List<SqlParameter> paramList = new List<SqlParameter>();

            string sql = $"delete from {base.GetDbTableName<T>(model)} where {primaryKey}=@{primaryKey}";
            PropertyInfo p = model.GetType().GetProperty(primaryKey);
            paramList.Add(new SqlParameter($"@{primaryKey}", p.GetValue(model, null)));
            return HelperFactory.MSSQLHelper.Update(sql.ToString(), paramList.ToArray());
        }
        //存储过程调用...
        #endregion

        #region 查询操作
        public override List<T> GetEntitiesFromReader<T>(IDataReader reader)
        {
            Type type = typeof(T);   //得到当前实体的类型
            PropertyInfo[] proArray = type.GetProperties();    //获取属性集合
            List<T> entityList = new List<T>();//创建集合对象

            //获取当前查询的所有列名称（注意必须和数据库字段一致）
            List<string> filedNames = new List<string>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                filedNames.Add(reader.GetName(i).ToLower());
            }
            //循环读取并封装对象
            while (reader.Read())
            {
                T objEntity = new T();
                foreach (PropertyInfo item in proArray)//取出每一个属性信息
                {
                    if (filedNames.Contains(item.Name.ToLower()))//判断是否包含该列（因为很多时候属性和查询的字段不一致）
                    {
                        if (reader[item.Name] != null)   //如果当前列不为null，则给属性赋值
                        {
                            item.SetValue(objEntity, Convert.ChangeType(reader[item.Name], item.PropertyType), null);
                        }
                    }
                }
                entityList.Add(objEntity);
            }
            reader.Close();
            return entityList;
        }
        #endregion
    }
}
