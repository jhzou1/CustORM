using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustORMDemo
{
    /// <summary>
    /// 抽象工厂
    /// </summary>
 public    class abstractFactory
    {
        public static T CreateProduct<T>(string filePath,string AssString, string ClassName) where T : class
        {
            //反射出来
            return (T)Assembly.LoadFrom(filePath)
                .CreateInstance($"{AssString}.{ClassName}");// 创建类的实例
        }
    }
}
