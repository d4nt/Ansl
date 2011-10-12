using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ansl
{
    [Serializable]
    public class TermInfo
    {
        public string Term { get; set; }
        public IList<string> DocumentsContaining { get; set; }
        public IDictionary<string, double> TermFrequencyByDocumentId { get; set; }

        public TermInfo()
        {
            DocumentsContaining = new List<string>();
            TermFrequencyByDocumentId = new Dictionary<string, double>();
        }

        public double GetTfIdf(string documentId, int totalDocumentCount)
        {
            return TermFrequencyByDocumentId[documentId] * Math.Log(1.0 + (DocumentsContaining.Count / (double)totalDocumentCount));
        }
    }
}
