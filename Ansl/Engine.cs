using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ansl
{
    /// <summary>
    /// An implementation of the TF IDF algorithm for searching text documents
    /// </summary>
    public class Engine
    {
        private IIndexStore _store;
        private EngineOptions _options;
        private Regex _nonAlphaNumChars;

        /// <summary>
        /// Creates a new Engine for indexing and searching documents
        /// using the default options and the in-memory store
        /// </summary>
        public Engine()
            : this(new EngineOptions(), new IndexMemoryStore())
        {
        }

        /// <summary>
        /// Creates a new Engine for indexing and searching documents
        /// </summary>
        /// <param name="options">
        /// An instance of EngineOptions containing the settings for
        /// the search engine
        /// </param>
        /// <param name="storageProvider">
        /// A implementation of the index store interface
        /// that the engine will use to store its index
        /// </param>
        public Engine(EngineOptions options, IIndexStore storageProvider)
        {
            _options = options;
            _store = storageProvider;
            _nonAlphaNumChars = new Regex("[^a-zA-Z0-9]", RegexOptions.Compiled);
        }

        /// <summary>
        /// Adds a document to the index, so it can appear in search results
        /// </summary>
        public void Index(Document document)
        {
            var isNewDocument = true;
            var documentWordCount = 0;
            var documentCount = _store.DocumentCount;
            var wordfrequency = new Dictionary<string, double>();
            var wordsToRemove = new List<string>();
            
            // Was this document indexed before? If so load the old word frequencies, and re-calc the IDF for each word in the union of the two sets
            var previousDocumentVersionWords = _store.LoadDocumentWordList(document.UniquieId);
            if (previousDocumentVersionWords == null)
            {
                // This is a new document, so up the document count
                documentCount++;
                _store.DocumentCount = documentCount;
            }
            else
            {
                isNewDocument = false;
                wordsToRemove.AddRange(previousDocumentVersionWords);
            }

            var idfByWord = new Dictionary<string, double>();
            var tfByWord = new Dictionary<string, double>();

            // First pass, count the word frequencies for in this document
            foreach (var documentWord in document)
            {
                var word = _nonAlphaNumChars.Replace(documentWord, "");
                if (String.IsNullOrWhiteSpace(word) == false)
                {
                    if (_options.CaseSensitve == false)
                        word = word.ToLower();

                    if (wordsToRemove != null && wordsToRemove.Contains(word))
                        wordsToRemove.Remove(word);

                    documentWordCount++;
                    
                    // first time we've seen this word in this document?
                    if (wordfrequency.ContainsKey(word) == false)
                        wordfrequency[word] = 1;                        
                    else
                        wordfrequency[word] += 1;

                    tfByWord[word] = wordfrequency[word] / (double)documentWordCount;

                    var documentCountForWord = _store.LoadDocumentCountForWord(word);

                    if (isNewDocument)
                        documentCountForWord++;

                    idfByWord[word] = Math.Log(1.0 + (documentCount / (double)documentCountForWord));
                }
            }

            // Second pass, save the tfidf for each word in that document
            foreach (var word in tfByWord.Keys)
            {
                IDictionary<string, double> docIdsAndWordWeightings = null;

                if (_store.ContainsWord(word))
                    docIdsAndWordWeightings = _store.LoadDocumentIdsAndWeightings(word);
                else
                    docIdsAndWordWeightings = new Dictionary<string, double>();

                docIdsAndWordWeightings[document.UniquieId] = (tfByWord[word] * idfByWord[word]);

                _store.SaveDocumentIdsAndWeightings(word, docIdsAndWordWeightings);
            }

            // Finally, any words that were in the previous version but not in the new one should be removed from the index
            foreach (var word in wordsToRemove)
            {
                if (_store.ContainsWord(word))
                {
                    var docIdsAndWordWeightings = _store.LoadDocumentIdsAndWeightings(word);
                    docIdsAndWordWeightings.Remove(document.UniquieId);
                }
            }

            _store.SaveDocumentWordList(document.UniquieId, tfByWord.Keys);
        }

        /// <summary>
        /// Searches the document index and brings back the top results
        /// for a the given words
        /// </summary>
        /// <param name="words">A collection of words to search for</param>
        /// <returns>
        /// An ordered list of document IDs of documents containing the 
        /// words being searched for. The most relevant documents will 
        /// be at the start of the list
        /// </returns>
        public IEnumerable<string> Search(IEnumerable<string> words)
        {
            // structure for holding the top results and their ranking
            var results = new Dictionary<string, double>();

            foreach (var word in words)
            {
                string searchWord = word;

                if (_options.CaseSensitve == false)
                    searchWord = word.ToLower();

                if (_store.ContainsWord(searchWord))
                {
                    var wordWeightings = _store.LoadDocumentIdsAndWeightings(searchWord);

                    foreach (var documentId in wordWeightings.Keys)
                    {
                        if (results.ContainsKey(documentId))
                        {
                            results[documentId] += wordWeightings[documentId];
                        }
                        else
                            results.Add(documentId, wordWeightings[documentId]);

                    }
                }
            }

            // Sort by score, take a max of 10, then select the document Id
            return results.OrderByDescending(kv => kv.Value).Take(10).Select(kv => kv.Key);
        }
    }
}
