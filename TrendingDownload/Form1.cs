using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace TrendingDownload
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Trending Keywords Downloads";

            try
            {
                string qry = "Exec DeleteKeywords ";
                ExecuteQuery(qry);

                GetKeywordsFromAPI();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            Environment.Exit(Environment.ExitCode);
        }

        public string readAPI()
        {
            XmlDocument xml = new XmlDocument();
            string fileName = @"C:\Inetpub\wwwroot\TrendingLiveAPI.xml";
            // You'll need to put the correct path to your xml file here
            xml.Load(fileName);

            // Select a specific node
            XmlNode node = xml.SelectSingleNode("TrendingAPI/downloadapi");
            // Get its value
            string name = node.InnerText;

            return name;
        }
        public string Connection()
        {
            XmlDocument xml = new XmlDocument();
            string fileName = @"C:\Inetpub\wwwroot\TrendingLiveAPI.xml";
            // You'll need to put the correct path to your xml file here
            xml.Load(fileName);

            // Select a specific node
            XmlNode node = xml.SelectSingleNode("TrendingAPI/con");
            // Get its value
            string name = node.InnerText;

            return name;
        }

        private void GetKeywordsFromAPI()
        {
            string url = readAPI();
            
            string authInfo = "pisoftware" + ":" + "r00t123456";

            StringBuilder stringBuilder = new StringBuilder();
            string value = string.Empty;
            Uri uri = new Uri(url);
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            httpWebRequest.Headers["Authorization"] = "Basic " + authInfo;
            XmlDocument doc = new XmlDocument();
            XmlReaderSettings settings = new XmlReaderSettings { CheckCharacters = false };


            //XmlReader writer = XmlReader.Create(reader, settings);

            using (HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse())
            using (XmlReader reader = XmlReader.Create(response.GetResponseStream(), settings))
            {
                try
                {
                    reader.MoveToContent();
                    doc.Load(reader);
                    reader.Close();                   

                    XmlNodeList lst = doc.GetElementsByTagName("query");                                                            
                    
                    foreach (XmlElement el in lst)
                    {
                        string sid = el.Attributes["search-engine-id"].Value;
                        string kw = el.Attributes["keyword"].Value;  

                        string qry = "Insert into [dbo].[Keywords1] (seid, keyword) values(" + sid + ", N'" + kw + "'); ";
                        ExecuteQuery(qry); 
                        string updateQuery = "Exec UpdateKeywords ";
                        ExecuteQuery(updateQuery);
                    }                 
                    
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private void ExecuteQuery(string qry)
        {
            //string qry = "Delete from [dbo].[Keywords]; ";
            //string qry = "Insert into [dbo].[Keywords] (seid, keword) values(" + seid + ", N'" + kw + "'); ";
            //string DbCon = "Data Source=82.136.46.2;User ID=sa;Password=Brisbane007;initial catalog=Trending;";
            //string DbCon = "Server=tcp:googlefirstpage.database.windows.net,1433;Initial Catalog=Trending;User ID=hemachander@googlefirstpage;Password=Brisbane007;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30";
            string DbCon = Connection();
            try
            {
                using (SqlConnection con = new SqlConnection(DbCon))
                {
                    con.Open();
                    using (SqlCommand comm = new SqlCommand(qry, con))
                    {
                        comm.CommandTimeout = 0;
                        comm.CommandType = CommandType.Text;
                        comm.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex) { throw ex; }
            finally { }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
