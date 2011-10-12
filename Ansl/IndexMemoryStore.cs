using System.Collections.Generic;

namespace Ansl
{
    public class IndexMemoryStore : IIndexStore
    {
        private Dictionary<string, TermInfo> _termInfos = new Dictionary<string, TermInfo>();
        private Dictionary<string, IList<string>> _documentInfos = new Dictionary<string, IList<string>>();

        public bool ContainsTerm(string term)
        {
            return _termInfos.ContainsKey(term);
        }

        public void SaveTermInfo(TermInfo info)
        {
            _termInfos[info.Term] = info;
        }

        public TermInfo LoadTermInfo(string term)
        {
            return _termInfos.ContainsKey(term) ? _termInfos[term] : null;
        }

        public int GetDocumentCount()
        {
            return _documentInfos.Count;
        }

        public void SaveDocumentInfo(string documentId, IList<string> terms)
        {
            _documentInfos[documentId] = terms;
        }

        public IList<string> LoadDocumentInfo(string documentId)
        {
            return _documentInfos[documentId];
        }

        public bool ContainsDocumentInfo(string documentId)
        {
            return _documentInfos.ContainsKey(documentId);
        }
    }
}
