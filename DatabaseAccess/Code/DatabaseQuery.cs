using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Code
{
    public class DatabaseQuery
    {
        public static SqlConnection conn;

        public static SqlConnection ConnOpen()
        {
            if (conn == null)
            {
                var conString = @"data source=TIMIRBHINGRADIY;initial catalog=CloudErpV;user id=sa;password=sqlserver;trustservercertificate=True;";
                conn = new SqlConnection(conString);
            }

            if (conn.State != System.Data.ConnectionState.Open)
            {
                conn.Open();
            }
            return conn;
        }

        public static bool Insert(string query)
        {
            try
            {
                int noofrows = 0;
                SqlCommand cmd = new SqlCommand(query, ConnOpen());
                noofrows = cmd.ExecuteNonQuery();
                if (noofrows > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool Update(string query)
        {
            try
            {
                int noofrows = 0;
                SqlCommand cmd = new SqlCommand(query, ConnOpen());
                noofrows = cmd.ExecuteNonQuery();
                if (noofrows > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool Delete(string query)
        {
            try
            {
                int noofrows = 0;
                SqlCommand cmd = new SqlCommand(query, ConnOpen());
                noofrows = cmd.ExecuteNonQuery();
                if (noofrows > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public static DataTable Retrive(string query)
        {
            try
            {
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(query, ConnOpen());
                da.Fill(dt);
                return dt;
            }
            catch { return null; }
        }

        //public static bool ExpiryProduct()
        //{
        //    using (SqlCommand cmd = new SqlCommand("GetExpiryProductList", ConnOpen()))
        //    {
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        using (SqlDataReader dr = cmd.ExecuteReader()) 
        //        {
        //            if (dr.Read())
        //            {
        //                return true;
        //            }
        //            return false;
        //        }
        //    }
        //}

        //public static bool LowStock()
        //{
        //    using (SqlCommand cmd = new SqlCommand("GetExpiryProductList", ConnOpen()))
        //    {
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        using (SqlDataReader dr = cmd.ExecuteReader())
        //        {
        //            if (dr.Read())
        //            {
        //                return true;
        //            }
        //            return false;
        //        }
        //    }
        //}
    }
}

