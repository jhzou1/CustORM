using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORMCore
{
        /// <summary>
        /// 标识列（标识作用,只能用在属性和字段上面）
        /// </summary>
        [AttributeUsage(AttributeTargets.Field|AttributeTargets.Property)]
        public class IdentityAttribute : Attribute
        {
            public bool IsPKey { get; } = true;
        }

        /// <summary>
        /// 主键特性,只能用在属性和字段上面
        /// </summary>
        /// 
        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
        public class PrimaryKeyAttribute : Attribute
        {
            public bool IsPKey { get; } = true;
        }

        /// <summary>
        ///扩展属性
        /// </summary>
        public class NonTableAttribute : Attribute
        {
            public bool IsNonFiled { get; } = true;
        }

        /// <summary>
        /// 转换成数据库表名特性
        /// </summary>
        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
        public class TableNameByAttribute : Attribute
        {
            public TableNameByAttribute(string dbTableName)
            {
                this.TableNmae = dbTableName;
            }
            /// <summary>
            /// 数据库中的表名字
            /// </summary>
            public string TableNmae { get; }
        }
    
}
