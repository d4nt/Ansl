using System.Collections.Generic;

namespace Ansl
{
    public class IndexMemoryStore : IIndexStore
    {
        private Dictionary<string, IEnumerable<string>> _docWordLists = new Dictionary<string, IEnumerable<string>>();
        private Dictionary<string, IDictionary<string, double>> _docIdsAndWordWeightings = new Dictionary<string, IDictionary<string, double>>();
        private int _docCount = 0;

        public void SaveDocumentIdsAndWeightings(string word, IDictionary<string, double> documentIdsAndWordWeightings)
        {
            _docIdsAndWordWeightings[word] = documentIdsAndWordWeightings;
        }

        public IDictionary<string, double> LoadDocumentIdsAndWeightings(string word)
        {
            return _docIdsAndWordWeightings[word];
        }

        public bool ContainsWord(string word)
        {
            return _docIdsAndWordWeightings.ContainsKey(word);
        }

        public int DocumentCount
        {
            get
            {
                return _docCount;
            }
            set
            {
                _docCount = value;
            }
        }

        public int LoadDocumentCountForWord(string word)
        {
            return _docIdsAndWordWeightings.ContainsKey(word) ? _docIdsAndWordWeightings[word].Count : 0;
        }
        
        public IEnumerable<string> LoadDocumentWordList(string documentId)
        {
            if (_docWordLists.ContainsKey(documentId))
                return _docWordLists[documentId];
            else
                return null;
        }

        public void SaveDocumentWordList(string documentId, IEnumerable<string> words)
        {
            _docWordLists[documentId] = words;
        }
    }
}
