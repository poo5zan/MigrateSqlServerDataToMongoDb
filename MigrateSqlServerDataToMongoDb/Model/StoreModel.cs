namespace MigrateSqlServerDataToMongoDb.Model
{
    public class StoreModel
    {
        public int StoreId { get; set; }

        public string StoreIdentifier { get; set; }

        public string Name { get; set; }

        public int ClientId { get; set; }

    }
}
