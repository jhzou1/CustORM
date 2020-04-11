using ORMCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    /// <summary>
    /// 测试实体类
    /// </summary>
    /// 
    [TableNameByAttribute("T1")]
public     class ModelT1
    {
        [IdentityAttribute]
        public int Id { get; set; }

        /// <summary>
        /// 非空验证
        /// </summary>
        [CustomRequiredAttribute("ModelT1.QRCode")]
        [PrimaryKeyAttribute]
        public string QRCode { get; set; }

        /// <summary>
        /// 非空验证
        /// </summary>
        [CustomRequiredAttribute("ModelT1.Name")]
        public string Name { get; set; }


        public string ProductDesc { get; set; }

        [NonTableAttribute]
        public string Remark { get; set; }
    }
}
