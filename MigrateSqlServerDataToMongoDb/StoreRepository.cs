using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using MigrateSqlServerDataToMongoDb.Model;

namespace MigrateSqlServerDataToMongoDb
{
    public class StoreRepository
    {
        private AdoDotNetHelper _adoDotNetHelper;

        public StoreRepository()
        {
            _adoDotNetHelper = new AdoDotNetHelper();
        }

        public List<StoreModel> GetStores()
        {
            string sql = @"Select Id,StoreIdentifier,Name,ClientId
                            From pimstore
                            
                            ";
            //Where Id not in (32,48,21,49,39,58)
            var stores = new List<StoreModel>();
            try
            {

           
            using (SqlConnection connection = _adoDotNetHelper.GetSqlConnection())
            {
                connection.Open();
                using (SqlCommand command = _adoDotNetHelper.GetSqlCommand(sql))
                {
                    command.Connection = connection;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            return null;
                        }

                        while (reader.Read())
                        {
                            stores.Add(new StoreModel()
                            {
                                Name = reader["Name"].ToString(),
                                ClientId = Convert.ToInt32(reader["ClientId"].ToString()),
                                StoreId = Convert.ToInt32(reader["Id"].ToString()),
                                StoreIdentifier = reader["StoreIdentifier"].ToString()
                            });
                        }

                    }
                }
            }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
                //throw;
            }

            return stores;
        }

    }
}
