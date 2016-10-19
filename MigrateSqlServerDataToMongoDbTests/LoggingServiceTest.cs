using Microsoft.VisualStudio.TestTools.UnitTesting;
using MigrateSqlServerDataToMongoDb;

namespace MigrateSqlServerDataToMongoDbTests
{
    [TestClass]
    public class LoggingServiceTest
    {
        [TestMethod]
        public void Write_Tests()
        {
            ILoggingService loggingService = new LoggingService();

            loggingService.Write("pujan writes log");

            var p = 0;
        }

    }
}
