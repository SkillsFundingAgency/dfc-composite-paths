using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DFC.Composite.Paths.Storage
{
    /// <summary>
    /// Defines methods for a storage for a document
    /// </summary>
    public interface IDocumentStorage
    {
        /// <summary>
        /// Adds a new document
        /// </summary>
        /// <typeparam name="T">The type of document to add</typeparam>
        /// <param name="document">The document to add</param>
        /// <returns>The id of the document that is added</returns>
        Task<string> Add<T>(T document);

        /// <summary>
        /// Gets a document
        /// </summary>
        /// <typeparam name="T">The type of document to add</typeparam>
        /// <param name="document">The id of the document to get</param>
        /// <returns>A document</returns>
        Task<T> Get<T>(string documentId);

        /// <summary>
        /// Performs a search
        /// </summary>
        /// <typeparam name="T">The type of document to fetch</typeparam>
        /// <param name="expression">An expression that constrains the search</param>
        /// <returns>A list of documents find</returns>
        Task<IEnumerable<T>> Search<T>(Expression<Func<T, bool>> expression);

        /// <summary>
        /// Updates a document
        /// </summary>
        /// <typeparam name="T">The type of document to fetch</typeparam>
        /// <param name="document">The document to update</param>
        /// <returns></returns>
        Task Update<T>(string documentId, T document);

        /// <summary>
        /// Deletes a document
        /// </summary>
        /// <param name="documentId">The id of the document to delete</param>
        /// <returns></returns>
        Task Delete(string documentId);
    }
}
