using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ORMCore
{
    /// <summary>
    /// 操作MySql的上下文
    /// </summary>
    public class MySqlDBContext : ObastractDbContext
    {
        
        public override int DeleteModelByParamSql<T>(T model)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 查询返回列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 插入模型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="returnIdentity"></param>
        /// <returns></returns>
        public override int InsertModelByParamSql<T>(T model, bool returnIdentity = false)
        {
            //先验证模型
            base.ValidateModel(model);

            //【1】定义要组合的SQL语句（请大家自己完成，能够自动读取表的别名内容）
            StringBuilder sqlFields = new StringBuilder($"insert into {base.GetDbTableName<T>(model)} (");
            StringBuilder sqlValues = new StringBuilder(" values (");
            List<MySqlParameter> paramList = new List<MySqlParameter>();

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
                paramList.Add(new MySqlParameter($"@{item.Name }", item.GetValue(model, null)));
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
                return Convert.ToInt32(HelperFactory.MySQLHelper.GetSingleResult(sql, paramList.ToArray()));
            }
            else
            {
                return HelperFactory.MySQLHelper.Update(sql, paramList.ToArray());
            }

        }

        public override int SaveByStoreProcedure<T>(T model, string ProcedureName)
        {
            throw new NotImplementedException();
        }

        public override int UpdateByParamSql<T>(T model)
        {
            throw new NotImplementedException();
        }

        public override int UpdateByStoreProcedure<T>(T model, string procedureNane)
        {
            throw new NotImplementedException();
        }
    }
}
