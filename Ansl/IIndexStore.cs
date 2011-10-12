using System.Collections.Generic;

namespace Ansl
{
    public interface IIndexStore
    {
        /// <summary>
        /// Indicates whether the store contains a reference to a particlar word
        /// </summary>
        /// <param name="term">The word to look for in the store</param>
        /// <returns>True, if the word is in the store, false otherwise</returns>
        bool ContainsTerm(string term);
        
        /// <summary>
        /// Asks the store to save the term info object
        /// </summary>
        /// <param name="term">The term/word that this term info relates to</param>
        /// <param name="info">The TermInfo object to save</param>
        void SaveTermInfo(TermInfo info);
        
        /// <summary>
        /// Asks the store to retrieve a term info object
        /// </summary>
        /// <param name="term">The term/word to load the info for</param>
        /// <returns>The requested term info object</returns>
        TermInfo LoadTermInfo(string term);
        
        /// <summary>
        /// The number of documents that are referenced in this index
        /// </summary>
        int GetDocumentCount();

        /// <summary>
        /// Asks the store whether a specific document id is in the store
        /// </summary>
        /// <param name="documentId">The document id to check for</param>
        /// <returns>True if the store contains that document, false otherwise</returns>
        bool ContainsDocumentInfo(string documentId);

        /// <summary>
        /// Tells the store to record that the engine has indexed this document id
        /// </summary>
        /// <param name="terms">The terms included in this document</param>
        /// <param name="documentId">The unique ID of the document to record</param>
        void SaveDocumentInfo(string documentId, IList<string> terms);

        /// <summary>
        /// Retrieves the a list of terms that the document includes
        /// </summary>
        /// <param name="documentId">The unique ID of the document</param>
        /// <returns>A list of terms contained within the document</returns>
        IList<string> LoadDocumentInfo(string documentId);        
    }
}
