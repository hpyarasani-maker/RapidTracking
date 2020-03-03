using System;
using System.Data;
using System.Data.SqlClient;
using System.Xml;

namespace RapidTrackingSingleThread
{
    public static class Common
    {

        internal static DataTable getIPsFromDB()
        {
            DataTable dt = new DataTable();
            string strQry = "Select id, address, '' as AccessHour, 0 as NoOfTimes, 0 as isError, '' as ErrTime From oxylabs_proxies";

            using (SqlDataAdapter da = new SqlDataAdapter(strQry, ReadConnection()))
            {
                da.Fill(dt);
            }
            return dt;
        }
        internal static string ReadConnection()
        {
            XmlDocument xml = new XmlDocument();
            string fileName = @"C:\Inetpub\wwwroot\Callback_TrackingTrending.xml";
            // You'll need to put the correct path to your xml file here
            xml.Load(fileName);

            // Select a specific node
            XmlNode node = xml.SelectSingleNode("ConnectionString/con");
            // Get its value
            string name = node.InnerText.Trim();

            return name;
        }


        internal static int GetTime()
        {
            int time = 0;
            string strQuery = "exec [dbo].[GetSleepTime_0]";
            SqlConnection objCon = new SqlConnection(ReadConnection());
            try
            {
                objCon.Open();
                SqlCommand objCmd = new SqlCommand(strQuery, objCon);
                objCmd.CommandTimeout = 0;
                SqlDataReader objData = null;
                objData = objCmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (objData.Read())
                {
                    time = Convert.ToInt16(objData[0]);
                }
                objData.Close();
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
            SqlConnection objCon = new SqlConnection(ReadConnection());
            try
            {
                objCon.Open();
                SqlCommand objCmd = new SqlCommand(strQuery, objCon);
                objCmd.CommandTimeout = 0;
                SqlDataReader objData = null;
                objData = objCmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (objData.Read())
                {
                    time = Convert.ToInt32(objData[0]);
                }
                objData.Close();
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

        internal static int GetServerIPsCount()
        {
            int count = 0;
            string strQuery = "exec [dbo].[GetServerIPsCount]";
            SqlConnection objCon = new SqlConnection(ReadConnection());
            try
            {
                objCon.Open();
                SqlCommand objCmd = new SqlCommand(strQuery, objCon);
                objCmd.CommandTimeout = 0;
                SqlDataReader objData = null;
                objData = objCmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (objData.Read())
                {
                    count = Convert.ToInt16(objData[0]);
                }
                objData.Close();
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
            return count;
        }

    }
}
