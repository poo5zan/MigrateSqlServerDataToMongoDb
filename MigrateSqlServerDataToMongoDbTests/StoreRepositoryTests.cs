using Microsoft.VisualStudio.TestTools.UnitTesting;
using MigrateSqlServerDataToMongoDb;

namespace MigrateSqlServerDataToMongoDbTests
{
    [TestClass]
    public class StoreRepositoryTests
    {
        [TestMethod]
        public void GetStores_Test()
        {
            var stores = new StoreRepository().GetStores();

            var o = 0;

        }


    }
}
