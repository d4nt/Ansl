using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Ansl
{
    public class IndexDirectoryStore : IIndexStore
    {
        private DirectoryInfo _dir;
        private const string DOC_COUNT_FILENAME = ".docCount";
        private const string DOC_COUNT_PER_WORD_SUFFIX = ".docCount";

        public IndexDirectoryStore(string directoryPath)
        {
            _dir = new DirectoryInfo(directoryPath);
            if (_dir.Exists == false)
                _dir.Create();
        }

        public bool ContainsWord(string word)
        {
            return File.Exists(Path.Combine(_dir.FullName, word));
        }

        public void SaveDocumentIdsAndWeightings(string word, Dictionary<string, double> documentIdsAndWeightings)
        {
            var formatter = new BinaryFormatter();

            using (var fs = new FileStream(Path.Combine(_dir.FullName, word), FileMode.Create))
            {
                formatter.Serialize(fs, documentIdsAndWeightings);
            }
        }

        public Dictionary<string, double> LoadDocumentIdsAndWeightings(string word)
        {
            var result = new Dictionary<string, double>();
            var formatter = new BinaryFormatter();

            using (var fs = new FileStream(Path.Combine(_dir.FullName, word), FileMode.Open))
            {
                result = (Dictionary<string, double>)formatter.Deserialize(fs);
            }

            return result;
        }


        public int DocumentCount
        {
            get
            {
                int result = 0;
                
                try
                {
                    result = ReadSingleInt(DOC_COUNT_FILENAME);
                }
                catch { }

                return result;
            }
            set
            {
                WriteSingleInt(DOC_COUNT_FILENAME, value);
            }
        }

        public void SaveDocumentCountByWord(string word, int documentCount)
        {
            WriteSingleInt(Path.Combine(_dir.FullName, word + DOC_COUNT_PER_WORD_SUFFIX), documentCount);
        }

        public int LoadDocumentCountByWord(string word)
        {
            return ReadSingleInt(Path.Combine(_dir.FullName, word + DOC_COUNT_PER_WORD_SUFFIX));
        }

        private void WriteSingleInt(string filename, int value)
        {
            using (var fs = new FileStream(Path.Combine(_dir.FullName, filename), FileMode.Create))
            {
                int val = value;

                var docCountBytes = new byte[4];
                docCountBytes[3] = (byte)val;
                val = val >> 8;
                docCountBytes[2] = (byte)val;
                val = val >> 8;
                docCountBytes[1] = (byte)val;
                val = val >> 8;
                docCountBytes[0] = (byte)val;
                fs.Write(docCountBytes, 0, 4);
                fs.Flush();
            }
        }

        private int ReadSingleInt(string filename)
        {
            int result = 0;

            try
            {
                using (var fs = new FileStream(Path.Combine(_dir.FullName, filename), FileMode.Open))
                {
                    var docCountBytes = new byte[4];
                    fs.Read(docCountBytes, 0, 4);
                    result = docCountBytes[0] << 24;
                    result += docCountBytes[1] << 16;
                    result += docCountBytes[2] << 8;
                    result += docCountBytes[3];
                }
            }
            catch { }

            return result;
        }
    }
}
