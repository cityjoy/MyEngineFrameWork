using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using System.Xml;
using Dapper;


namespace  Engine.Infrastructure.Utils
{
    public abstract class DapperHelper
    {

        
        public enum DataProvider
        {
            Oracle,
            SqlServer,
            OleDb,
            Odbc,
            MySql
        }
        /// <summary>
        /// 从配置文件中选择数据库类型
        /// </summary>
        /// <returns>DataProvider枚举值</returns>
        private static DataProvider GetDataProvider()
        {
            XmlDocument doc = new XmlDocument();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            XmlReader reader = XmlReader.Create(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\web.config.xml", settings);
            doc.Load(reader);
            XmlNodeList nodes = doc.SelectNodes("configuration/appSettings/add");
            string providerType = "";
            foreach (XmlNode node in nodes)
            {
                if (node.Attributes["key"].Value == "DataProvider")
                {
                    providerType = node.Attributes["value"].Value;
                    break;
                }
            }



            //string providerType = ConfigurationManager.AppSettings["DataProvider"];
            DataProvider dataProvider;
            switch (providerType)
            {
                case "Oracle":
                    dataProvider = DataProvider.Oracle;
                    break;
                case "SqlServer":
                    dataProvider = DataProvider.SqlServer;
                    break;
                case "OleDb":
                    dataProvider = DataProvider.OleDb;
                    break;
                case "Odbc":
                    dataProvider = DataProvider.Odbc;
                    break;
                case "MySql":
                    dataProvider = DataProvider.MySql;
                    break;
                default:
                    return DataProvider.Odbc;
            }
            return dataProvider;
        }
        /// <summary>
        /// 从配置文件获取连接字符串
        /// </summary>
        /// <returns>连接字符串</returns>
        private static string GetConnectionString()
        {
            XmlDocument doc = new XmlDocument();
             XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreComments = true;
                XmlReader reader = XmlReader.Create(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\web.config.xml", settings);
                doc.Load(reader);

            XmlNodeList nodes = doc.SelectNodes("configuration/connectionStrings/add");
            string connstr="";
            foreach(XmlNode node in nodes){
                if (node.Attributes["name"].Value == "ConnString")
                {
                    connstr = node.Attributes["connectionString"].Value;
                    break;
                }
            }

            return connstr;// ConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            //return "Data Source=192.168.121.128/orcl;User Id=system;Password=123456;";
        }


        private static IDbConnection GetConnection()
        {

            DataProvider providerType = GetDataProvider();
            switch (providerType)
            {
                case DataProvider.SqlServer:
                    return new SqlConnection(GetConnectionString());
                case DataProvider.OleDb:
                    return new OleDbConnection(GetConnectionString());
                case DataProvider.Odbc:
                    return new OdbcConnection(GetConnectionString());
                case DataProvider.Oracle:
                    return new OracleConnection(GetConnectionString());
                //case DataProvider.MySql://using MySql.Data.MySqlClient;   //请自行安装MySQLConnector/Net后添加引用
                //    return new MySqlConnection(GetConnectionString());
                default:
                    return new SqlConnection(GetConnectionString());
            }

        }

        #region Oracle
        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public static int OracleGetSingle(string SQLString)
        {

            int count = 0;
            using (var conn = GetConnection())
            {
                object obj = conn.ExecuteScalar(SQLString);

                count = int.Parse(obj.ToString());
            }
            return count;
        }

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString"></param>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static int OracleGetSingle(string SQLString, object entities)
        {
            //DynamicParameters p = new DynamicParameters(entities);

            int count = 0;
            using (IDbConnection conn = GetConnection())
            {
                object obj = conn.ExecuteScalar(SQLString, entities);

                count = int.Parse(obj.ToString());
            }
            return count;

        }

        /// <summary>
        /// 执行查询语句，返回DataTable集合
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataTable OracleQuery(string sql)
        {
            DataTable dt = null;
            using (IDbConnection conn = GetConnection())
            {
                IDataReader reader = conn.ExecuteReader(sql);
                dt = GetDataTableFromIDataReader(reader);
                reader.Dispose();
            }
            return dt;
        }

        /// <summary>
        /// 执行查询语句，返回IEnumerable集合
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static IEnumerable<TElement> OracleSqlQuery<TElement>(string sql)
        {
            using (IDbConnection conn = GetConnection())
            {
                return conn.Query<TElement>(sql);
            }
        }
        /// <summary>
        /// 执行查询语句，返回IEnumerable集合
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <param name="entities"></param>
        /// <returns>IEnumerable</returns>
        public static IEnumerable<TElement> OracleSqlQuery<TElement>(string sql, IEnumerable<TElement> entities)
        {
            using (IDbConnection conn = GetConnection())
            {
                return conn.Query<TElement>(sql, entities);
            }
        }

        //执行SQL语句 返回影响行数
        public static int OracleExecuteSql(string sql)
        {
            using (IDbConnection conn = GetConnection())
            {
                int row = conn.Execute(sql, null);
                return row;
            }
        }
        //执行SQL语句 返回影响行数
        public static int OracleExecuteSql<T>(string sql, IEnumerable<T> entities)
        {
            using (IDbConnection conn = GetConnection())
            {
                int row = conn.Execute(sql, entities);
                return row;
            }
        }

        //执行SQL语句 返回影响行数
        public static int OracleExecuteSql(string sql, object entities)
        {
            using (IDbConnection conn = GetConnection())
            {
                int row = conn.Execute(sql, entities);
                return row;
            }
        }

        public static int OracleExecuteSqlTranByList(List<String> SQLStringList)
        {

            using (var conn = GetConnection())
            {
                conn.Open();
                IDbTransaction trans = conn.BeginTransaction();
                int row = 0;
                foreach (string str in SQLStringList)
                {
                    row += conn.Execute(str, null, trans, null, null);
                }
                trans.Commit();
                conn.Close();
                return row;
            }

        }

        //dapper集合批量插入
        public static int OracleInsertMultiple<T>(string sql, IEnumerable<T> entities, string connectionName = null) where T : class, new()
        {
            using (var cnn = GetConnection())
            {
                int records = 0;
                using (var trans = cnn.BeginTransaction())
                {
                    try
                    {
                        cnn.Execute(sql, entities, trans, 30, CommandType.Text);
                    }
                    catch (DataException ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                    trans.Commit();
                }
                //foreach (T entity in entities)
                //{
                // records += cnn.Execute(sql, entity);
                //}
                return records;
            }
        }


        #endregion

        #region MSSQL
        ////////////////////////////////////////////////////////
        //以下为MSSQL数据库

        ////数据库连接字符串(web.config来配置)，可以动态更改connectionString支持多数据库.
        //public static string connectionString =
        //   // "Data Source=192.168.121.128/orcl;User Id=system;Password=123456;";
        //    @"server=Provider=SQLOLEDB.1;Data Source=localhost\sql2005;Password=sa;Persist Security Info=True;User ID=sa;Initial Catalog=wlxdb;max pool size=300";
        //   //WebConfigurationManager.ConnectionStrings["oracleConnection"].ToString();



        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public static int GetSingle(string SQLString)
        {

            int count = 0;

            using (IDbConnection conn = GetConnection())
            {
                object obj = conn.ExecuteScalar(SQLString);

                count = int.Parse(obj.ToString());
            }
            return count;

        }
        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString"></param>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static int GetSingle(string SQLString, object entities)
        {
            //DynamicParameters p = new DynamicParameters(entities);

            int count = 0;
            using (IDbConnection conn = GetConnection())
            {
                object obj = conn.ExecuteScalar(SQLString, entities);

                count = int.Parse(obj.ToString());
            }
            return count;

        }

        /// <summary>
        /// 执行查询语句，返回DataTable集合
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataTable Query(string sql)
        {
            DataTable dt = null;
            using (IDbConnection conn = GetConnection())
            {
                IDataReader reader = conn.ExecuteReader(sql);
                dt = GetDataTableFromIDataReader(reader);
                 reader.Dispose();
            }
            return dt;
        }
        private static DataTable GetDataTableFromIDataReader(IDataReader reader)
        {
            DataTable dt = new DataTable();
            bool init = false;
            dt.BeginLoadData();
            object[] vals = new object[0];
            while (reader.Read())
            {
                if (!init)
                {
                    init = true;
                    int fieldCount = reader.FieldCount;
                    for (int i = 0; i < fieldCount; i++)
                    {
                        dt.Columns.Add(reader.GetName(i), reader.GetFieldType(i));
                    }
                    vals = new object[fieldCount];
                }
                reader.GetValues(vals);
                dt.LoadDataRow(vals, true);
            }
            reader.Close();
            dt.EndLoadData();
            return dt;
        }  

        /// <summary>
        /// 执行查询语句，返回IEnumerable集合
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static IEnumerable<TElement> SqlQuery<TElement>(string sql)
        {
            using (IDbConnection conn = GetConnection())
            {
                return conn.Query<TElement>(sql);
            }
        }
        /// <summary>
        /// 执行查询语句，返回IEnumerable集合
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <param name="entities"></param>
        /// <returns>IEnumerable</returns>
        public static IEnumerable<TElement> SqlQuery<TElement>(string sql, object entities)
        {
            using (IDbConnection conn = GetConnection())
            {
                return conn.Query<TElement>(sql, entities);
            }
        }

        //执行SQL语句 返回影响行数
        public static int ExecuteSql(string sql)
        {
            using (IDbConnection conn = GetConnection())
            {
                int row = conn.Execute(sql, null);
                return row;
            }
        }
        //执行SQL语句 返回影响行数
        public static int ExecuteSql<T>(string sql, IEnumerable<T> entities)
        {
            using (IDbConnection conn = GetConnection())
            {
                int row = conn.Execute(sql, entities);
                return row;
            }
        }

        //执行SQL语句 返回影响行数
        public static int ExecuteSql(string sql, object entities)
        {
            using (IDbConnection conn = GetConnection())
            {
                int row = conn.Execute(sql, entities);
                return row;
            }
        }

        public static int ExecuteSqlTranByList(List<String> SQLStringList)
        {

            using (var conn = GetConnection())
            {
                conn.Open();
                IDbTransaction trans = conn.BeginTransaction();
                int row = 0;
                foreach (string str in SQLStringList)
                {
                    row += conn.Execute(str, null, trans, null, null);
                }
                trans.Commit();
                conn.Close();
                return row;
            }

        }

        //dapper集合批量插入
        public static int InsertMultiple<T>(string sql, IEnumerable<T> entities, string connectionName = null) where T : class, new()
        {
            using (var cnn = GetConnection())
            {
                int records = 0;
                using (var trans = cnn.BeginTransaction())
                {
                    try
                    {
                        cnn.Execute(sql, entities, trans, 30, CommandType.Text);
                    }
                    catch (DataException ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                    trans.Commit();
                }
                //foreach (T entity in entities)
                //{
                // records += cnn.Execute(sql, entity);
                //}
                return records;
            }
        }


    #endregion


    }
}
