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
        void SaveDocumentIdsAndWeightings(string word, Dictionary<string, double> documentIdsAndWeightings);

        /// <summary>
        /// Loads a set of document IDs and weightings from the store for a particular word
        /// </summary>
        /// <param name="word">The word to load the document IDs and weightings for</param>
        /// <returns>A dictionary of document IDs and weights</returns>
        Dictionary<string, double> LoadDocumentIdsAndWeightings(string word);

        /// <summary>
        /// The number of documents that are referenced in this index
        /// </summary>
        int DocumentCount { get; set; }

        /// <summary>
        /// Stores a count of how many documents the specified word appears in
        /// </summary>
        /// <param name="word">The word</param>
        /// <param name="documentCount">The number of documents it appears in</param>
        void SaveDocumentCountByWord(string word, int documentCount);

        /// <summary>
        /// Retrieves a count of how many documents the specified word appears in
        /// </summary>
        /// <param name="word">The word</param>
        /// <returns>The number of documents it appears in</returns>
        int LoadDocumentCountByWord(string word);
    }
}
