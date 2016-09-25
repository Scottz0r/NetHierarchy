using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetHierarchy
{
    /// <summary>
    /// This class holds helper methods that mimic popular LINQ methods for the <see cref="Node{T}"/> class. 
    /// </summary>
    public static class NodeLinq
    {
        /// <summary>
        /// Filters the descendant nodes based on a predicate.
        /// </summary>
        /// <param name="Predicate">A function to test each descendant for a condition.</param>
        public static IEnumerable<Node<T>> DescendantsWhere<T>(this Node<T> node, Func<Node<T>, bool> Predicate)
        {
            if (Predicate == null) throw new ArgumentNullException(nameof(Predicate));

            if (Predicate(node))
                yield return node;

            foreach (var child in node.Children)
            {
                foreach (var result in child.DescendantsWhere(Predicate))
                {
                    yield return result;
                }
            }
        }

        /// <summary>
        /// Determines whether any descendant satisfies a condition.
        /// </summary>
        /// <param name="Predicate">A function to test each descendant.</param>
        public static bool DescendantsAny<T>(this Node<T> node, Func<Node<T>, bool> Predicate)
        {
            if (Predicate == null) throw new ArgumentNullException(nameof(Predicate));

            if (Predicate(node))
                return true;

            foreach (var child in node.Children)
            {
                var result = child.DescendantsAny(Predicate);
                if (result)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Determines if the node's descendants contains the value using the default equality comparer.
        /// </summary>
        /// <param name="Value">The value to locate in the sequence.</param>
        public static bool DescendantsContains<T>(this Node<T> node, T Value)
        {
            if (Value == null) return false;

            if (node.Data.Equals(Value))
                return true;

            foreach (var child in node.Children)
            {
                var result = child.DescendantsContains(Value);
                if (result)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Filters the parent nodes based on a predicate.
        /// </summary>
        /// <param name="Predicate">A function to test each parent for a condition.</param>
        public static IEnumerable<Node<T>> ParentsWhere<T>(this Node<T> node, Func<Node<T>, bool> Predicate)
        {
            if (Predicate == null) throw new ArgumentNullException(nameof(Predicate));

            if (node.Parent == null)
                yield break;

            if (Predicate(node.Parent))
                yield return node.Parent;

            foreach (var parent in node.Parent.ParentsWhere(Predicate))
                yield return parent;
        }

        /// <summary>
        /// Determines if a node's parents contains any value.
        /// </summary>
        public static bool ParentsAny<T>(this Node<T> node)
        {
            return node.Parent != null;
        }

        /// <summary>
        /// Determines whether any parent satisfies a condition.
        /// </summary>
        /// <param name="Predicate">A function to test each parent.</param>
        public static bool ParentsAny<T>(this Node<T> node, Func<Node<T>, bool> Predicate)
        {
            if (Predicate == null) throw new ArgumentNullException(nameof(Predicate));

            if (node.Parent == null)
                return false;

            if (Predicate(node.Parent))
                return true;
            else
                return node.Parent.ParentsAny(Predicate);
        }

        /// <summary>
        /// Determines if the node's parents contains the value using the default equality comparer.
        /// </summary>
        /// <param name="Value">The value to locate in the sequence.</param>
        public static bool ParentsContains<T>(this Node<T> node, T Value)
        {
            if (Value == null) return false;

            if (node.Parent == null)
                return false;

            if (node.Parent.Data.Equals(Value))
                return true;
            else
                return node.Parent.ParentsContains(Value);
        }
    }
}
