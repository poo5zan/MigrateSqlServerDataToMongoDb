using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MigrateSqlServerDataToMongoDb.Model;

namespace MigrateSqlServerDataToMongoDb
{
    class Program
    {
        private static ProductDataRepository _productDataRepository;
        private static RestServiceHelper _restServiceHelper;
        private static JsonHelper _jsonHelper;
        private static ILoggingService _loggingService;
        private static string UrlPostSingleProduct = "http://192.168.56.101:5000/api/product";
        private static string UrlPostMultipleProduct = "http://192.168.56.101:5000/api/products";

        static void Main(string[] args)
        {
            //Console.WriteLine("Start");
            _loggingService = new LoggingService();
            _productDataRepository = new ProductDataRepository();
            _restServiceHelper = new RestServiceHelper();
            _jsonHelper = new JsonHelper();

            _loggingService.Write("Started Application ",
                LogLevel.Info, typeof(Program));
            //var stopWatch = new Stopwatch();
            //stopWatch.Start();

            // Get Stores
            var stores = new StoreRepository().GetStores();
            //Console.WriteLine("Time Elapsed to fetch Stores " + stopWatch.Elapsed.TotalMinutes);
            //_loggingService.Write("Time Elapsed to fetch Stores " + stopWatch.Elapsed.TotalMinutes);
            if (stores == null)
            {
                Console.WriteLine("No stores");
            }
            else
            {
                // Console.WriteLine("Total Stored Found " + stores.Count);


                foreach (var store in stores)
                {
                    Task.Factory.StartNew(() =>
                   {
                       StoreModel storeModel = store;
                       try
                       {
                           var productToDump = new List<Dictionary<string, object>>();
                           //Console.WriteLine("Getting product data for Store = " + store.StoreId);
                           _loggingService.Write("Getting product data for Store = " + storeModel.StoreId,
                               LogLevel.Info, typeof(Program));
                           var products = _productDataRepository.GetProducts(storeModel.StoreId);
                           int _productCount = 0;
                           foreach (var product in products)
                           {
                               _productCount++;
                               //Console.WriteLine("Received prouct count=" + _productCount + ",store=" + storeModel.StoreId);
                               _loggingService.Write("Received product ,Count=" + _productCount + ",store=" + storeModel.StoreId);// + ", StoreId=" + storeModel.StoreId);
                                                                                                                                  //add store data along with product data
                                                                                                                                  //product.Add("storeId", storeModel.StoreId);
                                                                                                                                  //product.Add("storeIdentifier", storeModel.StoreIdentifier);
                                                                                                                                  //product.Add("storeName", storeModel.Name);
                                                                                                                                  //product.Add("storeClientId", storeModel.ClientId);

                               productToDump.Add(product);

                               //_loggingService.Write("ProductToDump Count=");
                               if (productToDump.Count == 50)
                               {
                                   _loggingService.Write("Writing 50 product to MongoDb",
                                         LogLevel.Info, typeof(Program));

                                   InsertDataToDb(productToDump);

                                   //remove all elements
                                   _loggingService.Write("Clearing productToDump", LogLevel.Info,
                                       typeof(Program));
                                   productToDump.Clear();
                                   _loggingService.Write("ProductToDump after clearance " + productToDump.Count);
                               }
                           }
                           //dump remaining data
                           if (productToDump.Any())
                           {
                               InsertDataToDb(productToDump);
                           }
                           _loggingService.Write("Time Elapsed for this store = " + storeModel.StoreId);// + ", time=" + stopWatch.Elapsed.TotalMinutes);
                       }
                       catch (Exception e)
                       {
                           _loggingService.Write(e, LogLevel.Error,
                                 typeof(Program), true);
                           //throw;
                       }
                   });
                }

            }

            //_loggingService.Write("Completed Time Elapsed " + stopWatch.Elapsed.TotalMinutes,
            //    LogLevel.Info, typeof(Program));
            //stopWatch.Stop();
            Console.WriteLine("Running process in background   ");
            Console.ReadLine();

        }


        static void InsertDataToDb(List<Dictionary<string, object>> data)
        {
            try
            {
                // Task.Factory.StartNew(() =>
                // {
                _loggingService.Write("Converting product data to Json. DataCount=" + data.Count,
                LogLevel.Info, typeof(Program));
                try
                {
                    var productDataJson = _jsonHelper.ConvertToJson(data);

                    _loggingService.Write("Writing data to mongoDb",
                        LogLevel.Info, typeof(Program));

                    _restServiceHelper.Call(UrlPostMultipleProduct, true, productDataJson);
                }
                catch (OutOfMemoryException oum)
                {
                    _loggingService.Write("Handled OutOfMemoryException in JsonSerialize. Splitting data to two.");
                    //split the data to two
                    var dataList1 = data.GetRange(0, data.Count / 2);
                    InsertDataToDb(dataList1);
                    var dataList2 = data.GetRange(data.Count / 2, data.Count / 2);
                    InsertDataToDb(dataList2);
                    //throw;
                }
                
                // });
            }
            catch (Exception ex)
            {
                _loggingService.Write(ex, LogLevel.Error, typeof(Program), true);
            }
        }
    }
}
