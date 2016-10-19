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

        public void MigrateProductData(StoreModel store)
        {
            string url = "http://192.168.56.101:5000/api/product";
            var restServiceHelper = new RestServiceHelper();
            var jsonHelper = new JsonHelper();
            var products = new List<Dictionary<string, object>>();
            string storedProcedure = "pujan_sp_get_customfield_data";
            var parameters = new Dictionary<string, object>();
            parameters.Add("@StoreId", store.StoreId);
            try
            {
                using (SqlConnection connection = _adoDotNetHelper.GetSqlConnection())
                {
                    connection.Open();
                    using (SqlCommand command = _adoDotNetHelper.GetSqlCommand(
                        storedProcedure, parameters, true
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

                                        product.Add("storeId", store.StoreId);
                                        product.Add("storeIdentifier", store.StoreIdentifier);
                                        product.Add("storeName", store.Name);
                                        product.Add("storeClientId", store.ClientId);

                                        var productDataJson = jsonHelper.ConvertToJson(product);

                                        restServiceHelper.Call(url, true, productDataJson);
                                        //Console.WriteLine(" Store = {0} , Product Id = {1}, ProdCount = {2}",
                                        //  store.StoreId, response, count);
                                        //products.Add(product);
                                        count++;
                                        Console.WriteLine("Store: " + store.StoreId + ", ProdCount= " + count);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //return null;
                //throw;
            }

            // return products;
        }

        public List<Dictionary<string, object>> GetProductData(int storeId)
        {
            string url = "http://192.168.56.101:5000/api/product";
            var products = new List<Dictionary<string, object>>();
            string storedProcedure = "pujan_sp_get_customfield_data";
            var parameters = new Dictionary<string, object>();
            parameters.Add("@StoreId", storeId);
            var productsFromDb = _adoDotNetHelper.GetData(storedProcedure,
                parameters, true);
            if (productsFromDb == null)
            {
                return null;
            }
            var columns = productsFromDb.Columns;

            foreach (DataRow dr in productsFromDb.Rows)
            {
                var p = 0;
            }

            return products;

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
