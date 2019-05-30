using System;
using System.Data.Common;

namespace DFC.Composite.Paths.Storage.Cosmos
{
    public class CosmosConnectionString
    {
        public Uri Endpoint { get; set; }

        public string AuthKey { get; set; }

        public CosmosConnectionString(string connectionString)
        {
            var builder = new DbConnectionStringBuilder
            {
                ConnectionString = connectionString
            };

            if (builder.TryGetValue("AccountKey", out object key))
            {
                AuthKey = key.ToString();
            }

            if (builder.TryGetValue("AccountEndpoint", out object uri))
            {
                Endpoint = new Uri(uri.ToString());
            }
        }
    }
}
