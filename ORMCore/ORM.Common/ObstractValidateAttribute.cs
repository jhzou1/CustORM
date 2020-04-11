using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORMCore
{
    /// <summary>
    /// 验证抽象类
    /// </summary>
 public  abstract  class ObstractValidateAttribute:Attribute
    {
        // <summary>
        /// 给用户显示的实体属性名
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;
        /// <summary>
        /// 是否验证通过
        /// </summary>
        public bool IsValid { get; set; }
        /// <summary>
        /// 验证返回的信息
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 抽象的验证方法 记住类也要是抽象类才可以
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract bool Validate(object value);
    }
}
