using System;
using GoLive.Saturn.Data.Abstractions;
using MongoDB.Driver;

namespace GoLive.Saturn.Auditing.AuditProvider
{
    public static class Registration
    {
        public static void Register(RepositoryOptions options, string collectionName = "Audit")
        {
            if (string.IsNullOrWhiteSpace(collectionName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(collectionName));
            }

            var mongoUrl = new MongoUrl(options.ConnectionString);

            Audit.Core.Configuration.DataProvider = new Audit.MongoDB.Providers.MongoDataProvider()
            {
                ConnectionString = mongoUrl.Url,
                Database = mongoUrl.DatabaseName,
                Collection = collectionName
            };
        }
    }
}
