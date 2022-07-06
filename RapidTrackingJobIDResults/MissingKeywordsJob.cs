using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RapidTrackingJobIDResults
{
    public class MissingKeywordsJob
    {

        public async Task ExecuteMissingKeywordsJob(int number)
        {
            await Task.Delay(number);

            SqlConnection DbConn = new SqlConnection(Common.ReadConnection());
            SqlCommand ExecJob = new SqlCommand();
            ExecJob.CommandType = CommandType.StoredProcedure;
            ExecJob.CommandText = "msdb.dbo.sp_start_job";
            ExecJob.Parameters.AddWithValue("@job_name", "MissingKeywords");
            ExecJob.Connection = DbConn; //assign the connection to the command.
            try
            {
                using (DbConn)
                {
                    DbConn.Open();
                    using (ExecJob)
                    {
                        ExecJob.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                string errorMessage = "Database Error: \r\n";
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessage += "Index #" + i + "\n" +
                                     "Message: " + ex.Errors[i].Message + "\n" +
                                     "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                                     "Source: " + ex.Errors[i].Source + "\n" +
                                     "Procedure: " + ex.Errors[i].Procedure + "\n" +
                                     "Server: " + ex.Errors[i].Server + "\n";
                }

                //throw new Exception(errorMessage);
            }
            catch (Exception)
            {
                //throw ex;
            }

        }
    }
}
