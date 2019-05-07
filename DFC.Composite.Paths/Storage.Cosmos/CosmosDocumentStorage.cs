using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace DFC.Composite.Paths.Storage.Cosmos
{
    public class CosmosDocumentStorage : IDocumentStorage
    {
        private readonly string _endpointUri;
        private readonly string _key;

        public CosmosDocumentStorage(string endpointUri, string key)
        {
            _endpointUri = endpointUri;
            _key = key;
        }

        public async Task<string> Add<T>(string databaseId, string collectionId, T document)
        {
            var client = await Init(databaseId, collectionId);
            string result = null;

            try
            {
                var documentResponse = await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseId, collectionId), document);
                result = documentResponse.Resource.Id;
            }
            catch (DocumentClientException dex)
            {
                if (dex.StatusCode != HttpStatusCode.Conflict)
                {
                    throw;
                }
            }

            return result;
        }

        public async Task<T> Get<T>(string databaseId, string collectionId, string documentId)
        {
            var client = await Init(databaseId, collectionId);

            var link = UriFactory.CreateDocumentUri(databaseId, collectionId, documentId);
            var readResponse = await client.ReadDocumentAsync<T>(link);

            return readResponse.Document;
        }

        public async Task<IEnumerable<T>> Search<T>(string databaseId, string collectionId, Expression<Func<T, bool>> expression)
        {
            var client = await Init(databaseId, collectionId);

            var queryOptions = new FeedOptions { MaxItemCount = int.MaxValue };

            IDocumentQuery<T> query = null;
            if (expression != null)
            {
                query = client.CreateDocumentQuery<T>(
                    UriFactory.CreateDocumentCollectionUri(databaseId, collectionId), queryOptions)
                    .Where(expression)
                    .AsDocumentQuery();
            }
            else
            {
                query = client.CreateDocumentQuery<T>(
                    UriFactory.CreateDocumentCollectionUri(databaseId, collectionId), queryOptions)
                    .AsDocumentQuery();
            }

            var results = new List<T>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }

            return results;
        }

        public async Task Update<T>(string databaseId, string collectionId, string documentId, T document)
        {
            var client = await Init(databaseId, collectionId);

            var link = UriFactory.CreateDocumentUri(databaseId, collectionId, documentId);
            await client.ReplaceDocumentAsync(link, document);
        }

        public async Task Delete(string databaseId, string collectionId, string documentId)
        {
            var client = await Init(databaseId, collectionId);

            var link = UriFactory.CreateDocumentUri(databaseId, collectionId, documentId);
            await client.DeleteDocumentAsync(link);
        }

        private async Task<DocumentClient> Init(string databaseId, string collectionId)
        {
            var db = new Database();
            db.Id = databaseId;

            //create db
            var client = new DocumentClient(new Uri(_endpointUri), _key);
            await client.CreateDatabaseIfNotExistsAsync(db);

            //create document collection
            var docCollection = await client.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(databaseId),
                new DocumentCollection { Id = collectionId });

            return client;
        }
    }
}