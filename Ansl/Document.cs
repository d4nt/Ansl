using System;
using System.Collections.Generic;

namespace Ansl
{
    /// <summary>
    /// A document that can be indexed and searched for
    /// </summary>
    /// <remarks>
    /// This type must be passed to the Index method and a list of document IDs
    /// will be returned from the Search method
    /// </remarks>
    public class Document : List<string>
    {
        /// <summary>
        /// You must provide a unique ID and some content in order to 
        /// construct a document object
        /// </summary>
        /// <param name="uniqueId">A unique ID for this document</param>
        /// <param name="words">The content of this document</param>
        public Document(string uniqueId, string content)
        {
            UniquieId = uniqueId;
            AddRange(content.Split(new char[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries));
        }

        /// <summary>
        /// A string to Uniquely identify this document
        /// </summary>
        public string UniquieId { get; private set; }

        /// <summary>
        /// Use the UniqueId to test for equality
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return this.UniquieId == ((Document)obj).UniquieId;
        }

        /// <summary>
        /// Use the UniquieId when getting a hash code of this object
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.UniquieId.GetHashCode();
        }

        /// <summary>
        /// Output the UniqueId when converting to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.UniquieId;
        }
    }
}
