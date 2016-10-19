using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MigrateSqlServerDataToMongoDb;

namespace MigrateSqlServerDataToMongoDbTests
{
    [TestClass]
    public class ProductDataRepositoryTest
    {
        [TestMethod]
        public void GetProductData_Test()
        {

            //var products = 
            var prod = new ProductDataRepository().GetProductData(48);

           // var productsJson = new JsonHelper().ConvertToJson(products);



            //var prodObj = new JsonHelper().ConvertFromJson(productsJson);


            var p = 0;

        }


        [TestMethod]
        public void GetProducts_Tests()
        {
            int storeId = 32;
            var sw = new Stopwatch();
            sw.Start();
            var products = new ProductDataRepository().GetProducts(storeId);
            Debug.WriteLine("Elapsed after getting IEnumerable " + sw.Elapsed.TotalMinutes);
            Assert.IsNotNull(products);
            foreach (var product in products)
            {
                Debug.WriteLine("Iterating products ienumerable " + product["LocalSku"]);
            }

            sw.Stop();
            Debug.WriteLine("Elapsed After Finishing " + sw.Elapsed.TotalMinutes);

             var p = 0;

        }

        [TestMethod]
        public void TestListSplit()
        {
            var list = new List<string>();
            list.Add("1");
            list.Add("2");
            list.Add("3");
            list.Add("4");
            list.Add("5");
            list.Add("6");
            list.Add("7");
            list.Add("8");

            var list1 = list.GetRange(0,list.Count/2);
            var list2 = list.GetRange(list.Count/2, list.Count/2);
            var p = 0;
        }
    }
}
