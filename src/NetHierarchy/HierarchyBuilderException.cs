using System;
using System.Runtime.Serialization;

namespace NetHierarchy
{
    [Serializable]
    public class HierarchyBuilderException : Exception
    {
        public HierarchyBuilderException()
        {
        }

        public HierarchyBuilderException(string message) : base(message)
        {
        }

        public HierarchyBuilderException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected HierarchyBuilderException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}