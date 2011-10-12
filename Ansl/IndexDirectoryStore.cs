using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Ansl
{
    public class IndexDirectoryStore : IIndexStore
    {
        private DirectoryInfo _dir;

        private const string DOC_INFO_SUFFIX = ".docInfo";
        private const string TERM_INFO_SUFFIX = ".termInfo";

        public IndexDirectoryStore(string directoryPath)
        {
            _dir = new DirectoryInfo(directoryPath);
            if (_dir.Exists == false)
                _dir.Create();
        }
        
        public bool ContainsTerm(string term)
        {
            return File.Exists(Path.Combine(_dir.FullName, term + TERM_INFO_SUFFIX));
        }

        public void SaveTermInfo(TermInfo info)
        {
            var formatter = new BinaryFormatter();

            using (var fs = new FileStream(Path.Combine(_dir.FullName, info.Term + TERM_INFO_SUFFIX), FileMode.Create))
            {
                formatter.Serialize(fs, info);
            }
        }

        public TermInfo LoadTermInfo(string term)
        {
            TermInfo result = null;
            var formatter = new BinaryFormatter();

            using (var fs = new FileStream(Path.Combine(_dir.FullName, term + TERM_INFO_SUFFIX), FileMode.Open))
            {
                result = (TermInfo)formatter.Deserialize(fs);
            }

            return result;
        }

        public int GetDocumentCount()
        {
            var docFiles = _dir.GetFiles("*" + DOC_INFO_SUFFIX);
            return docFiles != null ? docFiles.Length : 0;
        }

        public bool ContainsDocumentInfo(string documentId)
        {
            return File.Exists(Path.Combine(_dir.FullName, documentId + DOC_INFO_SUFFIX));
        }

        public void SaveDocumentInfo(string documentId, IList<string> terms)
        {
            var formatter = new BinaryFormatter();

            using (var fs = new FileStream(Path.Combine(_dir.FullName, documentId.GetHashCode() + DOC_INFO_SUFFIX), FileMode.Create))
            {
                formatter.Serialize(fs, terms);
            }
        }

        public IList<string> LoadDocumentInfo(string documentId)
        {
            IList<string> result = null;
            var formatter = new BinaryFormatter();

            using (var fs = new FileStream(Path.Combine(_dir.FullName, documentId.GetHashCode() + DOC_INFO_SUFFIX), FileMode.Open))
            {
                result = (IList<string>)formatter.Deserialize(fs);
            }

            return result;
        }
    }
}
