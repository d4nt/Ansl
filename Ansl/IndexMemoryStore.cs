using System.Collections.Generic;

namespace Ansl
{
    public class IndexMemoryStore : IIndexStore
    {
        private Dictionary<string, int> _docCountPerWord = new Dictionary<string, int>();
        private Dictionary<string, Dictionary<string, double>> _docIdsAndWordWeightings = new Dictionary<string, Dictionary<string, double>>();
        private int _docCount = 0;

        public void SaveDocumentIdsAndWeightings(string word, Dictionary<string, double> documentIdsAndWordWeightings)
        {
            _docIdsAndWordWeightings[word] = documentIdsAndWordWeightings;
        }

        public Dictionary<string, double> LoadDocumentIdsAndWeightings(string word)
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

        public void SaveDocumentCountByWord(string word, int documentCount)
        {
            _docCountPerWord[word] = documentCount;
        }

        public int LoadDocumentCountByWord(string word)
        {
            return _docCountPerWord.ContainsKey(word) ? _docCountPerWord[word] : 0;
        }
    }
}
