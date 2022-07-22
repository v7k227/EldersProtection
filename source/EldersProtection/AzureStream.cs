using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurePublic
{
    /// <summary>
    /// Check for scam phone numbers based on previous scam information.
    /// </summary>
    internal class AzureStream
    {
        private static string connectionString = @"Server=tcp:projectgodb.database.windows.net,1433;Initial Catalog=antifraud;Persist Security Info=False;User ID=AKAJACK;Password=2wsx#EDC;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public static bool PhoneAnalyzer(string phoneNo, ref string targetName)
        {
            try
            {
                string sqlCommand = @"SELECT phone_no, target_name FROM phone_info WHERE phone_no = '{0}'";

                bool isScam = false;

                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
                {
                    connection.Open();

                    using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(string.Format(sqlCommand, phoneNo), connection))
                    {
                        using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                        {
                            bool gotData = reader.Read();
                            do
                            {
                                if (gotData)
                                {
                                    isScam = true;

                                    targetName = reader.GetString(1);
                                }
                                gotData = reader.Read();
                            } while (gotData);
                        }
                    }
                }

                return isScam;
            }
            catch (Exception exc)
            {
                return false;
            }
        }
    }
}