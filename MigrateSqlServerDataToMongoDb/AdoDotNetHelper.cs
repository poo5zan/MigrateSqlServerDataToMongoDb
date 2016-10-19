using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace MigrateSqlServerDataToMongoDb
{
    public class AdoDotNetHelper
    {
        public DataTable GetData(string sqlOrStoredProcedureName,
            Dictionary<string, object> parameters = null,
           bool isStoredProcedure = false
           )
        {
            DataTable dataTable = new DataTable();
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
            SqlCommand command = GetSqlCommand(sqlOrStoredProcedureName,
                parameters,
                isStoredProcedure);

            try
            {
                command.Connection.Open();
                sqlDataAdapter.SelectCommand = command;
                sqlDataAdapter.Fill(dataTable);
                //TODO: If dataTable is empty return null
                return dataTable;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                command.Connection.Close();
                sqlDataAdapter.Dispose();
                command.Connection.Dispose();
            }
        }

        public bool InsertUpdateData(string sqlOrStoredProcedureName,
            Dictionary<string, object> parameters = null,
            bool isStoredProcedure = false)
        {
            SqlCommand command = GetSqlCommand(sqlOrStoredProcedureName,
                parameters,isStoredProcedure);

            try
            {
                command.Connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                command.Connection.Close();
                command.Connection.Dispose();
            }
        }

        public SqlConnection GetSqlConnection()
        {
            string conn =
            @"Data Source=.; Initial Catalog=DataHub; 
            Integrated Security=True; MultipleActiveResultSets=True;max pool size=5000";

            return new SqlConnection(conn);
        }

        public SqlCommand GetSqlCommand(string sqlOrStoredProcedureName,
            Dictionary<string, object> parameters = null,
            bool isStoreProcedure = false)
        {
            SqlCommand command = new SqlCommand(sqlOrStoredProcedureName);
            //Comment this line if not from UniTest
            command.CommandTimeout = 0;
            command.Connection = GetSqlConnection();
            command.CommandType = isStoreProcedure ? CommandType.StoredProcedure : CommandType.Text;

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                }
            }
            return command;
        }

    }
}
