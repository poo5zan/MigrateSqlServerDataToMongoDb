using System;
using Newtonsoft.Json;

namespace MigrateSqlServerDataToMongoDb
{
    public class JsonHelper
    {
        public string ConvertToJson(Object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public Object ConvertFromJson(string jsonString)
        {
            return JsonConvert.DeserializeObject(jsonString);
        }


    }
}
