using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace K3WISE_WebService.Entity
{
    public class DBHelper
    {

        public static SqlConnection GetConnectionByDBName(string DBName)
        {
            var conn = "Data Source=193.1.11.159;Initial Catalog="+ DBName + ";User ID=sa;Password=Gdhg_K3&2018^Wise";
            return new SqlConnection(conn);
        }

        /// <summary>
        /// 返回数据集
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static DataTable Fill(string sql, SqlConnection connection)
        {
            string tableName = "_tempTable";
            try
            {
                //创建数据适配器对象
                SqlDataAdapter sda = new SqlDataAdapter(sql, connection );

                //创建数据集
                DataSet ds = new DataSet();
                sda.Fill(ds, tableName); //填充数据集

                return ds.Tables[tableName];
            }
            catch (Exception e)
            {
                //将异常引发出现
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// 返回数据集
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static DataTable FillAlone(string sql, SqlConnection connection)
        {
            string tableName = "_tempTable";
            try
            {
                connection.Open();  //打开连接
                //创建数据适配器对象
                SqlDataAdapter sda = new SqlDataAdapter(sql, connection);
                //创建数据集
                DataSet ds = new DataSet();
                sda.Fill(ds, tableName); //填充数据集

                return ds.Tables[tableName];
            }
            catch (Exception e)
            {
                //将异常引发出现
                throw new Exception(e.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// 返回数据集，带事务
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static DataTable Fill(string sql, SqlConnection connection, SqlTransaction transaction)
        {
            string tableName = "_tempTable";
            try
            {
                
                //创建数据适配器对象
                SqlDataAdapter sda = new SqlDataAdapter(sql, connection);

                sda.SelectCommand.Transaction = transaction;

                //创建数据集
                DataSet ds = new DataSet();
                sda.Fill(ds, tableName); //填充数据集

                return ds.Tables[tableName];
            }
            catch (Exception e)
            {
                //将异常引发出现
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// 查询第一行第一个数据值是否
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static int Exists(string sql, SqlConnection connection)
        {
            try
            {
                SqlCommand sqlcommand = new SqlCommand();
                sqlcommand.Connection = connection;
                sqlcommand.CommandType = CommandType.Text;
                sqlcommand.CommandText = sql;
                object ob = sqlcommand.ExecuteScalar();
                if (null == ob)
                    return -1;
                
                return null == ob ? -1 : Convert.ToInt32(ob);
            }
            catch (Exception e)
            {
                //将异常引发出现
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// 执行语句并返回影响行数，数据连接不打开和关闭
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static int Query(string sql, SqlConnection connection)
        {
            try
            {
                SqlCommand sqlcommand = new SqlCommand();
                sqlcommand.Connection = connection;
                sqlcommand.CommandType = CommandType.Text;
                sqlcommand.CommandText = sql;
                return sqlcommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //将异常引发出现
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// 执行语句并返回影响行数，数据连接不打开和关闭
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static int Query(string sql, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                SqlCommand sqlcommand = new SqlCommand();
                sqlcommand.Connection = connection;
                sqlcommand.CommandType = CommandType.Text;
                sqlcommand.CommandText = sql;
                sqlcommand.Transaction = transaction;
                return sqlcommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //将异常引发出现
                throw new Exception(e.Message);
            }
        }

    }
}