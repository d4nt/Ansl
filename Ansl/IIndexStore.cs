using System.Collections.Generic;

namespace Ansl
{
    public interface IIndexStore
    {
        /// <summary>
        /// Indicates whether the store contains a reference to a particlar word
        /// </summary>
        /// <param name="word">The word to look for in the store</param>
        /// <returns>True, if the word is in the store, false otherwise</returns>
        bool ContainsWord(string word);

        /// <summary>
        /// Stores a set of document IDs and weightings in the store for a particular word
        /// </summary>
        /// <param name="word">The word that the second parameter relates to</param>
        /// <param name="documentWeightings">A dictionary of document IDs and weightings</param>
        void SaveDocumentIdsAndWeightings(string word, IDictionary<string, double> documentIdsAndWeightings);

        /// <summary>
        /// Loads a set of document IDs and weightings from the store for a particular word
        /// </summary>
        /// <param name="word">The word to load the document IDs and weightings for</param>
        /// <returns>A dictionary of document IDs and weights</returns>
        IDictionary<string, double> LoadDocumentIdsAndWeightings(string word);

        /// <summary>
        /// The number of documents that are referenced in this index
        /// </summary>
        int DocumentCount { get; set; }

        /// <summary>
        /// Retrieves a count of how many documents the specified word appears in
        /// </summary>
        /// <remarks>
        /// The engine could just load the document Ids and weightings for the particular word
        /// but this method allows the engine to optimise for when only the document count
        /// is needed. If implementors do not wich to optimise this then simply load the
        /// dictionary of document Ids and weights and return the number of items in that
        /// dictionary
        /// </remarks>
        /// <param name="word">The word</param>
        /// <returns>The number of documents it appears in</returns>
        int LoadDocumentCountForWord(string word);

        /// <summary>
        /// Loads a list of unique words from the previously indexed version of the document
        /// </summary>
        /// <param name="documentId">The unique ID of the document to load the words for</param>
        /// <returns>A list of words contained in the document</returns>
        IEnumerable<string> LoadDocumentWordList(string documentId);

        /// <summary>
        /// Saves a new list of unique words for the specified document 
        /// </summary>
        /// <param name="documentId">The unique ID of the document</param>
        /// <param name="words">A list of words contains in the document</param>
        void SaveDocumentWordList(string documentId, IEnumerable<string> words);
    }
}
