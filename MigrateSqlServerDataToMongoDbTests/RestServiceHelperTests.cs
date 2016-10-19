using Microsoft.VisualStudio.TestTools.UnitTesting;
using MigrateSqlServerDataToMongoDb;

namespace MigrateSqlServerDataToMongoDbTests
{
    [TestClass]
    public class RestServiceHelperTests
    {
        [TestMethod]
        public void Call_Test()
        {
            string url = "http://192.168.56.101:5000/api/product";

            new RestServiceHelper().Call(url,false);

            //post
            var p = new Product()
            {
                Description = "ptest From .Net Client",
                Name = "ptest Name form .net post"
            };

            var pJson = new JsonHelper().ConvertToJson(p);
            new RestServiceHelper().Call(url,true,pJson);
            var r = 0;
        }

        class Product
        {
            public string Name { get; set; }
            public string Description { get; set; }

        }

    }
}
