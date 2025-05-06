using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Xml;

namespace AIOSingleThread
{
    class Common
    {


        internal static async Task<string> ReadConnection()
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

                return await Task.FromResult<string>(name);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        internal static async Task<string> StrConn()
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                string fileName = @"C:\Inetpub\wwwroot\downloadKeywords.xml"; //download remaining keywords
                //string fileName = @"C:\Inetpub\wwwroot\Callback_TrackingTrending.xml"; // downloading full keywords

                // You'll need to put the correct path to your xml file here
                xml.Load(fileName);

                // Select a specific node
                //XmlNode node = xml.SelectSingleNode("ConnectionString/con"); // downloading full keywords
                XmlNode node = xml.SelectSingleNode("download/con");//download remaining keywords

                // Get its value
                string name = node.InnerText;

                return await Task.FromResult<string>(name);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        internal static async Task<string> buffaloConn()
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                string fileName = @"C:\Inetpub\wwwroot\buffaloCon.xml"; //download remaining keywords
                //string fileName = @"C:\Inetpub\wwwroot\Callback_TrackingTrending.xml"; // downloading full keywords

                // You'll need to put the correct path to your xml file here
                xml.Load(fileName);

                // Select a specific node
                //XmlNode node = xml.SelectSingleNode("ConnectionString/con"); // downloading full keywords
                XmlNode node = xml.SelectSingleNode("download/con");//download remaining keywords

                // Get its value
                string name = node.InnerText;

                return await Task.FromResult<string>(name);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal static async Task<int> GetOxylabsTime()
        {
            int time = 0;
            string strQuery = "exec [dbo].[GetOxylabsTime]";
            SqlConnection objCon = new SqlConnection(await ReadConnection());
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
            return await Task.FromResult<int>(time);
        }

        internal static async Task<int> GetOxylabsCount()
        {
            int count = 0;
            string strQuery = "exec [dbo].[GetOxylabsCount]";
            SqlConnection objCon = new SqlConnection(await ReadConnection());
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
            return await Task.FromResult<int>(count);
        }
    }
}
