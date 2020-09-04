using System;
using System.Data;
using System.Data.SqlClient;
using System.Xml;

namespace RapidTrackingSingleThread
{
    public static class Common
    {
        internal static DataTable GetIPsFromDB()
        {
            DataTable dt = new DataTable();
            string strQry = "Select id, address From oxylabs_proxies order by newID()";
            //string strQry = "Select id, address From IP_Address where id between 1 and 120 ";

            using (SqlDataAdapter da = new SqlDataAdapter(strQry, ReadConnection()))
            {
                da.Fill(dt);
            }
            return dt;
        }

        internal static string ReadConnection()
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                string fileName = @"C:\Inetpub\wwwroot\Callback_TrackingTrending.xml";
                // You'll need to put the correct path to your xml file here
                xml.Load(fileName);
                // Select a specific node
                XmlNode node = xml.SelectSingleNode("ConnectionString/con");

                // Get its value
                string name = node.InnerText;

                return name;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal static string ReadAPI()
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                string fileName = @"C:\Inetpub\wwwroot\Callback_TrackingTrending.xml";

                // You'll need to put the correct path to your xml file here
                xml.Load(fileName);

                // Select a specific node
                XmlNode node = xml.SelectSingleNode("ConnectionString/apiSubmit");
                // Get its value
                string name = node.InnerText;

                return name;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal static int GetTime()
        {
            int time = 0;
            string strQuery = "exec [dbo].[GetSleepTime_0]";
            SqlConnection objCon = new SqlConnection(ReadConnection());
            try
            {
                using (SqlConnection con = new SqlConnection(Common.ReadConnection()))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(strQuery, con))
                    {
                        cmd.CommandTimeout = 0;
                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (dr.Read())
                            {
                                time = Convert.ToInt32(dr[0]);
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                string errMsg = "Database Connection is temporarily not working\n" + e.ToString();
            }
            finally
            {
                if (objCon.State == ConnectionState.Open)
                {
                    objCon.Close();
                }
            }
            return time;
        }

        internal static int GetServerIPsTime()
        {
            int time = 0;
            string strQuery = "exec [dbo].[GetServerIPsTime]";
            try
            {
                using (SqlConnection con = new SqlConnection(Common.ReadConnection()))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(strQuery, con))
                    {
                        cmd.CommandTimeout = 0;
                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (dr.Read())
                            {
                                time = Convert.ToInt32(dr[0]);
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                string errMsg = "Database Connection is temporarily not working\n" + e.ToString();
            }
            finally
            {

            }
            return time;
        }

        internal static int GetServerIPsCount()
        {
            int count = 0;
            string strQuery = "exec [dbo].[GetServerIPsCount]";

            try
            {
                using (SqlConnection con = new SqlConnection(Common.ReadConnection()))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(strQuery, con))
                    {
                        cmd.CommandTimeout = 0;
                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (dr.Read())
                            {
                                count = Convert.ToInt16(dr[0]);
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                string errMsg = "Database Connection is temporarily not working\n" + e.ToString();
            }
            finally
            {

            }
            return count;
        }

    }
}
