using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Ansl.TestHarness
{
    class Program
    {
        /// <summary>
        /// This test harness program will index and search the text
        /// files in a specified folder and will report out the file names
        /// of files containing the search terms
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // First argument, a folder containing text documents to index
            string documentsFolder = args[0];

            // Second argument, a pipe separated list of words to search for
            string[] searchWords = args[1].Split('|');

            var engine = new Engine(new EngineOptions(), new IndexDirectoryStore(@"C:\temp\AnslIndex\"));

            // Index all files in the arg[0] folder
            foreach (var fileName in Directory.EnumerateFiles(documentsFolder))
            {
                string content;

                using (var stream = new StreamReader(fileName))
                {
                    content = stream.ReadToEnd();
                }

                if (content != null)
                {
                    var document = new Document(fileName, content);
                    engine.Index(document);
                }
            }

            // Search for the words and output the file names of the matcing files
            foreach (var result in engine.Search(searchWords))
            {
                Console.WriteLine("Result: " + result);
            }

            // Wait before closing
            Console.ReadKey();
        }
    }
}
