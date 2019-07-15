using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace DFC.Composite.Paths.Storage.Cosmos
{
    public class CosmosDocumentStorage : IDocumentStorage
    {
        private readonly CosmosConnectionString _cosmosConnectionString;
        private readonly string _partitionKey;
        private readonly string _databaseId;
        private readonly string _collectionId;

        private readonly DocumentClient _documentClient;

        public CosmosDocumentStorage(CosmosConnectionString cosmosConnectionString, string partitionKey, string databaseId, string collectionId)
        {
            _cosmosConnectionString = cosmosConnectionString;
            _partitionKey = partitionKey;
            _databaseId = databaseId;
            _collectionId = collectionId;

            _documentClient = Init(_databaseId, _collectionId).Result;
        }

        public async Task<string> Add<T>(T document)
        {
            string result = null;

            try
            {
                var documentResponse = await _documentClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId), document);
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

        public async Task<T> Get<T>(string documentId)
        {
            var link = UriFactory.CreateDocumentUri(_databaseId, _collectionId, documentId);
            var readResponse = await _documentClient.ReadDocumentAsync<T>(link);

            return readResponse.Document;
        }

        public async Task<IEnumerable<T>> Search<T>(Expression<Func<T, bool>> expression)
        {
            var queryOptions = new FeedOptions { MaxItemCount = int.MaxValue, EnableCrossPartitionQuery = true };

            IDocumentQuery<T> query;
            if (expression != null)
            {
                query = _documentClient.CreateDocumentQuery<T>(
                    UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId), queryOptions)
                    .Where(expression)
                    .AsDocumentQuery();
            }
            else
            {
                query = _documentClient.CreateDocumentQuery<T>(
                    UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId), queryOptions)
                    .AsDocumentQuery();
            }

            var results = new List<T>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }

            return results;
        }

        public async Task Update<T>(string documentId, T document)
        {
            var link = UriFactory.CreateDocumentUri(_databaseId, _collectionId, documentId);
            await _documentClient.ReplaceDocumentAsync(link, document);
        }

        public async Task Delete(string documentId)
        {
            var link = UriFactory.CreateDocumentUri(_databaseId, _collectionId, documentId);
            await _documentClient.DeleteDocumentAsync(link, new RequestOptions() { PartitionKey = new PartitionKey(Undefined.Value) });
        }

        private async Task<DocumentClient> Init(string databaseId, string collectionId)
        {
            var db = new Database
            {
                Id = databaseId
            };

            //create db
            var client = new DocumentClient(_cosmosConnectionString.Endpoint, _cosmosConnectionString.AuthKey);
            await client.CreateDatabaseIfNotExistsAsync(db);

            //Specify the partition key definition
            var pkDef = new PartitionKeyDefinition
            {
                Paths = new Collection<string>() { _partitionKey }
            };

            //create document collection with the specified partition key definition
            _ = await client.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(databaseId),
                new DocumentCollection { Id = collectionId, PartitionKey = pkDef });

            return client;
        }

    }
}