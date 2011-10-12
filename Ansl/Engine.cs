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
            var documentTerms = new List<TermInfo>();            

            foreach (var documentWord in document)
            {
                var term = _nonAlphaNumChars.Replace(documentWord, "");
                if (String.IsNullOrWhiteSpace(term) == false)
                {
                    if (_options.CaseSensitve == false)
                        term = term.ToLower();

                    TermInfo termInfo = null;

                    // first time we've seen this word in this document?
                    if (documentTerms.Any(t => t.Term == term))
                    {
                        termInfo = documentTerms.First(t => t.Term == term);
                    }
                    else if (_store.ContainsTerm(term))
                    {
                        termInfo = _store.LoadTermInfo(term);
                        termInfo.DocumentsContaining.Add(document.UniquieId);
                        documentTerms.Add(termInfo);
                    }
                    else
                    {
                        termInfo = new TermInfo() { Term = term };
                        termInfo.DocumentsContaining.Add(document.UniquieId);
                        documentTerms.Add(termInfo);
                    }

                    if (termInfo.TermFrequencyByDocumentId.ContainsKey(document.UniquieId))
                        termInfo.TermFrequencyByDocumentId[document.UniquieId] += 1;
                    else
                        termInfo.TermFrequencyByDocumentId[document.UniquieId] = 1;
                }
            }

            // If this document has been indexed before, cleanup any terms that are no longer present
            if (_store.ContainsDocumentInfo(document.UniquieId))
            {
                foreach (var term in _store.LoadDocumentInfo(document.UniquieId))
                {
                    // If this term is no longer references by this document...
                    if (documentTerms.Any(t => t.Term == term) == false)
                    {
                        // remove out of date term infos and re-save
                        var termInfo = _store.LoadTermInfo(term);

                        termInfo.DocumentsContaining.Remove(document.UniquieId);

                        _store.SaveTermInfo(termInfo);
                    }
                }
            }

            // Tell the store about this document
            _store.SaveDocumentInfo(document.UniquieId, documentTerms.Select(t => t.Term).ToList());

            // Tell the store about all the term infos we've found in this document
            foreach (var termInfo in documentTerms)
                _store.SaveTermInfo(termInfo);
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

            int documentCount = _store.GetDocumentCount();

            foreach (var word in words)
            {
                string searchWord = word;

                if (_options.CaseSensitve == false)
                    searchWord = word.ToLower();

                if (_store.ContainsTerm(searchWord))
                {
                    var termInfo = _store.LoadTermInfo(searchWord);

                    foreach (var documentId in termInfo.DocumentsContaining)
                    {
                        if (results.ContainsKey(documentId))
                            results[documentId] += termInfo.GetTfIdf(documentId, documentCount);
                        else
                            results.Add(documentId, termInfo.GetTfIdf(documentId, documentCount));

                    }
                }
            }

            // Sort by score, take a max of 10, then select the document Id
            return results.OrderByDescending(kv => kv.Value).Take(10).Select(kv => kv.Key);
        }
    }
}
