using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MigrateSqlServerDataToMongoDb.Model;

namespace MigrateSqlServerDataToMongoDb
{
    public class ProductDataRepository
    {
        private AdoDotNetHelper _adoDotNetHelper;
        private ILoggingService _loggingService;

        public ProductDataRepository()
        {
            _adoDotNetHelper = new AdoDotNetHelper();
            _loggingService = new LoggingService();
        }
        
        public IEnumerable<Dictionary<string, object>> GetProducts(int storeId)
        {
            string storedProcedureName = "pujan_sp_get_customfield_data";
            var parameters = new Dictionary<string, object>();
            parameters.Add("@StoreId", storeId);

            using (SqlConnection connection = _adoDotNetHelper.GetSqlConnection())
            {
                try
                {
                    connection.Open();
                }
                catch (Exception e)
                {
                    _loggingService.Write(e,LogLevel.Error,
                        typeof(ProductDataRepository),true);
                    throw;
                }
                
                using (SqlCommand command = _adoDotNetHelper.GetSqlCommand(
                    storedProcedureName, parameters, true
                    ))
                {
                    command.Connection = connection;
                    command.CommandTimeout = 0;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            //return null;
                            var columns = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();
                            if (columns.Any())
                            {
                                int count = 0;
                                while (reader.Read())
                                {
                                    var product = new Dictionary<string, object>();

                                    foreach (var column in columns)
                                    {
                                        if (!product.ContainsKey(column))
                                        {
                                            product.Add(column, reader[column]);
                                        }
                                        else
                                        {
                                            product.Add(column + "_1", reader[column]);
                                        }
                                    }
                                   
                                    yield return product;

                                }
                            }
                        }
                    }
                }
            }
            
        }

    }
}
