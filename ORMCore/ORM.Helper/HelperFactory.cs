using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORMCore
{
 public    class HelperFactory
    {
        #region 链接字符串
        private static string MySqlStr=
            ConfigurationManager.ConnectionStrings["MySQLconnString"].ToString();

        private static string MSSqlStr =
            ConfigurationManager.ConnectionStrings["MSSQLconnString"].ToString();
        #endregion

        //Access数据库访问类
        public static IBaseDALHelper<OleDbDataReader, OleDbParameter> OleDbHelper =
            new ImplBaseDALHelper<OleDbConnection, OleDbCommand, OleDbDataReader, OleDbParameter>();

        //SqlServer数据访问类
        public static IBaseDALHelper<SqlDataReader, SqlParameter> MSSQLHelper =
           new ImplBaseDALHelper<SqlConnection, SqlCommand, SqlDataReader, SqlParameter>(MSSqlStr);

        //mySql 数据库访问类
        public  static IBaseDALHelper<MySql.Data.MySqlClient.MySqlDataReader, MySql.Data.MySqlClient.MySqlParameter> MySQLHelper = 
           new ImplBaseDALHelper<MySql.Data.MySqlClient.MySqlConnection, 
                                MySql.Data.MySqlClient.MySqlCommand, 
                                MySql.Data.MySqlClient.MySqlDataReader,
                                MySql.Data.MySqlClient.MySqlParameter>(MySqlStr);

        //其他数据访问类...
    }
}
