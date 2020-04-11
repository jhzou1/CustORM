using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustORMDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //获取数据库平台
            string DbType = ConfigurationManager.AppSettings["DataType"].ToString();

            DAL.IModelT1Service modelT1Service =
                abstractFactory.CreateProduct<DAL.IModelT1Service>("DAL.DLL",DbType, "ModelT1Service");


            Models.ModelT1 model = new Models.ModelT1()
            {
                //T0005
                QRCode = "T000"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Name = "Product xxxxx",
                ProductDesc= "Product "+ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Remark="程序内部使用，不存入数据库!"
            };

            try
            {
                Console.WriteLine($"当前数据库: {DbType} \n");

                //添加
                int result = modelT1Service.AddModelT1(model);

                //修改
                //int result = modelT1Service.UpdateModelT1(model);

                //删除
                //int result = modelT1Service.DeleteModelT1(model);

                Console.WriteLine($"受影响行数{result}");

                //查询
                List<Models.ModelT1> list = modelT1Service.QueryModelList();

                foreach (var item in list)
                {
                    Console.WriteLine($"{item.QRCode} {item.Name} {item.ProductDesc}");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.Read();
        }
    }
}
