using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ansl.Tests
{
    [TestClass]
    public class EngineTests
    {
        [TestMethod]
        public void TestSearchForWordInOneOfTwoDocuments_Success()
        {
            var engine = new Ansl.Engine();
            engine.Index(
                new Ansl.Document(
                    "test1.txt",
                    "The quick brown fox jumped over the lazy dog"));
            engine.Index(
                new Ansl.Document(
                    "test2.txt",
                    "Hello World"));

            foreach (var result in engine.Search(new string[] { "fox" }))
            {
                Assert.AreEqual("test1.txt", result);
            }
        }

        [TestMethod]
        public void TestSearchForWordInOneOfTwoDocuments_Fail()
        {
            var engine = new Ansl.Engine();
            engine.Index(
                new Ansl.Document(
                    "test1.txt",
                    "The quick brown fox jumped over the lazy dog"));
            engine.Index(
                new Ansl.Document(
                    "test2.txt",
                    "Hello World"));

            foreach (var result in engine.Search(new string[] { "hippo" }))
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void TestSearchForWordInOneOfTwoDocuments_CaseSensitve_Success()
        {
            var engine = new Ansl.Engine(
                new Ansl.EngineOptions() { CaseSensitve = true },
                new Ansl.IndexMemoryStore());
            engine.Index(
                new Ansl.Document(
                    "test1.txt",
                    "Hello World"));
            engine.Index(
                new Ansl.Document(
                     "test1.txt",
                     "The quick brown FoX jumped over the lazy dog"));

            bool found = false;

            foreach (var result in engine.Search(new string[] { "FoX" }))
            {
                found = true;
            }

            Assert.AreEqual(true, found);
        }

        [TestMethod]
        public void TestSearchForWordInOneOfTwoDocuments_CaseSensitve_Fail()
        {
            var engine = new Ansl.Engine(
                new Ansl.EngineOptions() { CaseSensitve = true },
                new Ansl.IndexMemoryStore());
            engine.Index(
                new Ansl.Document(
                    "test1.txt",
                    "Hello World"));
            engine.Index(
                new Ansl.Document(
                     "test1.txt",
                     "The quick brown fox jumped over the lazy dog"));
            
            foreach (var result in engine.Search(new string[] { "FOX" }))
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void TestReIndexingTheSameDocumentWithSearchForWordOnlyInVersionOne()
        {
            var engine = new Ansl.Engine();
            engine.Index(
                new Ansl.Document(
                    "test1.txt",
                    "The quick brown fox jumped over the lazy dog"));
            engine.Index(
                new Ansl.Document(
                    "test1.txt",
                    "Hello World"));

            foreach (var result in engine.Search(new string[] { "fox" }))
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void TestReIndexingTheSameDocumentWithSearchForWordOnlyInVersionTwo()
        {
            var engine = new Ansl.Engine();
            engine.Index(
                new Ansl.Document(
                    "test1.txt",
                    "Hello World")); 
            engine.Index(
                new Ansl.Document(
                     "test1.txt",
                     "The quick brown fox jumped over the lazy dog"));

            bool found = false;

            foreach (var result in engine.Search(new string[] { "fox" }))
            {
                found = true;
            }

            Assert.AreEqual(true, found);
        }
    }
}
