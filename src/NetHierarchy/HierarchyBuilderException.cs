using System;

namespace NetHierarchy
{
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
    }
}