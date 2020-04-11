using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ORMCore
{

    /// <summary>
    /// 非空验证特性
    /// </summary>
    /// 
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class CustomRequiredAttribute : ObstractValidateAttribute
    {
        public CustomRequiredAttribute(string name)
        {
            base.DisplayName = name;
        }

        /// <summary>
        /// 验证传入的方法 返回一个 bool类型
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool Validate(object value)
        {
            if (value == null || value.ToString().Length == 0)
            {
                base.IsValid = false;
                base.ErrorMessage = $"{base.DisplayName} Can not be null!";
            }
            else
            {
                base.IsValid = true;
            }
            return base.IsValid;
        }
    }

    /// <summary>
    /// 字符串固定长度
    /// </summary>
    /// 
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class CustomFixedLengthAttribute : ObstractValidateAttribute
    {
        private int FixedLength = 0;


        public CustomFixedLengthAttribute(string DisplayName, int Length)
        {
            this.FixedLength = Length;
            base.DisplayName = DisplayName;
        }

        public override bool Validate(object value)
        {
            if (value == null || value.ToString().Length == 0)
            {
                base.IsValid = false;
                base.ErrorMessage = $"{base.DisplayName} Can not be null!";
            }
            else
            {
                if (value.ToString().Length != this.FixedLength)
                {
                    base.IsValid = false;
                    base.ErrorMessage = $"{base.DisplayName} FixedLength:{this.FixedLength}!";
                }
                else
                {
                    base.IsValid = true;
                }

            }
            return base.IsValid;
        }
    }
    /// <summary>
    ///范围验证...
    /// </summary>
    /// 
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class CustomStringRangeAttribute : ObstractValidateAttribute
    {
        private int min = 0;
        private int max = 0;

        public CustomStringRangeAttribute(string DisplayName, int MinSize, int MaxSize)
        {
            base.DisplayName = DisplayName;
            this.min = MinSize;
            this.max = MaxSize;
        }

        public override bool Validate(object value)
        {
            if (value == null || value.ToString().Length == 0)
            {
                base.IsValid = false;
                base.ErrorMessage = $"{base.DisplayName} Can not be null!";
            }
            else
            {
                if (value.ToString().Length < this.min || value.ToString().Length > this.max)
                {
                    base.IsValid = false;
                    base.ErrorMessage = $"{base.DisplayName} bound for range:{this.min} to {this.max}!";
                }
                else
                {
                    base.IsValid = true;
                }
            }
            return base.IsValid;
        }
    }


    //时间验证

    //比较验证

    //...其他验证
    /// <summary>
    /// 自定义正则表达式特性 [切入该特性的对象需要遍历这个特性然后调用 Validate()方法]
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class RegularExpressionsAttribute : ObstractValidateAttribute
    {
        private string RegEx = string.Empty;
        private string desc = string.Empty;

        /// <summary>
        /// 根据传入的正则表达式判断是否符合
        /// </summary>
        /// <param name="DisplayName"></param>
        /// <param name="RegularEx"></param>
        /// <param name="CustDesc"></param>
        public RegularExpressionsAttribute(string DisplayName, string RegularEx, string CustDesc)
        {
            this.RegEx = RegularEx;
            base.DisplayName = DisplayName;
            this.desc = CustDesc;
        }
        /// <summary>
        /// 抽血抽象类的验证方法
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool Validate(object value)
        {
            if (value == null || value.ToString().Length == 0)
            {
                base.IsValid = false;
                base.ErrorMessage = $"{base.DisplayName} Can not be null!";
            }
            else
            {
                Regex objReg = new Regex(this.RegEx);

                if (!objReg.IsMatch(value.ToString()))
                {
                    base.IsValid = false;
                    base.ErrorMessage = $"{base.DisplayName} Format error：{this.desc}";
                }
                else
                {
                    base.IsValid = true;
                }
            }
            return base.IsValid;
        }
    }

}
