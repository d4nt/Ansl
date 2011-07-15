using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Ansl.TestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            string documentsFolder = args[0];
            string[] searchWords = args[1].Split('|');

            var engine = new Engine();

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

            foreach (var result in engine.Search(searchWords))
            {
                Console.WriteLine("Result: " + result);
            }

            Console.ReadKey();
        }
    }
}
