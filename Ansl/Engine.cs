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
            var docCount = _store.DocumentCount + 1;
            var docWordCount = 0;
            var wordfrequencies = new Dictionary<string, double>();
            var tfByWord = new Dictionary<string, double>();
            var idfByWord = new Dictionary<string, double>();
            
            foreach (var docWord in document)
            {
                var word = _nonAlphaNumChars.Replace(docWord, "");
                if (String.IsNullOrWhiteSpace(word) == false)
                {
                    if (_options.CaseSensitve == false)
                        word = word.ToLower();

                    docWordCount++;

                    int docCountForWord = _store.LoadDocumentCountByWord(word) + 1;

                    _store.SaveDocumentCountByWord(word, docCountForWord);

                    // first time we've seen this word in this document?
                    if (wordfrequencies.ContainsKey(word) == false)
                    {
                        wordfrequencies[word] = 1;                        
                    }
                    else
                        wordfrequencies[word] += 1;

                    tfByWord[word] = wordfrequencies[word] / (double)docWordCount;
                    idfByWord[word] = Math.Log(1.0 + (docCount / (double)docCountForWord));
                }
            }

            _store.DocumentCount = docCount;

            // at the end of the document, update the tfidf for each word in that document
            foreach (var word in tfByWord.Keys)
            {
                Dictionary<string, double> docIdsAndWordWeightings = null;

                if (_store.ContainsWord(word))
                    docIdsAndWordWeightings = _store.LoadDocumentIdsAndWeightings(word);
                else
                    docIdsAndWordWeightings = new Dictionary<string, double>();

                docIdsAndWordWeightings[document.UniquieId] = (tfByWord[word] * idfByWord[word]);

                _store.SaveDocumentIdsAndWeightings(word, docIdsAndWordWeightings);
            }
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

            foreach (string word in words)
            {
                if (_store.ContainsWord(word))
                {
                    var wordWeightings = _store.LoadDocumentIdsAndWeightings(word);

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
