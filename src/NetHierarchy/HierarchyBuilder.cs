using System;
using System.Collections.Generic;
using System.Linq;

namespace NetHierarchy
{
    /// <summary>
    /// This class contains generic functions that automatically builds hierarchies of <see cref="Node{T}"/> from a set of input data.
    /// </summary>
    public static class HierarchyBuilder
    {
        /// <summary>
        /// Automatically generates a hierarchy from a collection of input data. Only one root node is expected
        /// to be found with this function. Uses the default value of <see cref="T"/> to mark a root node.
        /// </summary>
        /// <typeparam name="T">The type of data the <see cref="Node{T}"/> will hold.</typeparam>
        /// <typeparam name="TKey">The type of the primary and parent keys.</typeparam>
        /// <param name="collection">A collection of input data.</param>
        /// <param name="primaryKey">A function that returns a primary key from <typeparamref name="T"/>.</param>
        /// <param name="parentKey">A function that returns a parent key from <typeparamref name="T"/>.</param>
        /// <param name="rootParentValue">The value that defines a root node.</param>
        /// <returns>Returns a root node populated with ancestors from the input collection.</returns>
        public static Node<T> GenerateHierarchy<T, TKey>(IEnumerable<T> collection, Func<T, TKey> primayKey, Func<T, TKey> parentKey)
            where T: class
        {
            return GenerateHierarchy(collection, primayKey, parentKey, default(TKey));
        }

        /// <summary>
        /// Automatically generates a hierarchy from a collection of input data using a specified value to indicate a root node. Only one root node is expected
        /// to be found with this function.
        /// </summary>
        /// <typeparam name="T">The type of data the <see cref="Node{T}"/> will hold.</typeparam>
        /// <typeparam name="TKey">The type of the primary and parent keys.</typeparam>
        /// <param name="collection">A collection of input data.</param>
        /// <param name="primaryKey">A function that returns a primary key from <typeparamref name="T"/>.</param>
        /// <param name="parentKey">A function that returns a parent key from <typeparamref name="T"/>.</param>
        /// <param name="rootParentValue">The value that defines a root node.</param>
        /// <returns>Returns a root node populated with ancestors from the input collection.</returns>
        public static Node<T> GenerateHierarchy<T, TKey>(IEnumerable<T> collection, Func<T, TKey> primaryKey, Func<T, TKey> parentKey, TKey rootParentValue)
            where T: class
        {
            collection.ArgumentNullCheck(nameof(collection));
            primaryKey.ArgumentNullCheck(nameof(primaryKey));
            parentKey.ArgumentNullCheck(nameof(parentKey));

            var groups = collection.GroupBy(x => parentKey(x));

            //Get all elements with a null primary key.
            var root = groups.FirstOrDefault(x => EqualityComparer<TKey>.Default.Equals(x.Key, rootParentValue));
            if(root == null)
            {
                throw new HierarchyBuilderException("A root node was not found in the collection.");
            }

            if(root.Count() > 1)
            {
                throw new HierarchyBuilderException("More than one root node was found in the collection. Use GenerateHierarchies to build multiple hierarchies.");
            }

            //Build a Dictionary that maps children to their parent keys.
            var parentChildDict = groups.Where(x => x.Key != null).ToDictionary(x => x.Key, x => x.ToList());

            //Recursively add children to the root node.
            var rootNode = new Node<T>(root.First());
            addChildren(rootNode, parentChildDict, primaryKey);

            return rootNode;
        }

        /// <summary>
        /// Automatically generate a hierarchy from a collection of input data. This will return one or more root nodes.
        /// Uses the default value of <see cref="T"/> to mark a root node.
        /// </summary>
        /// <typeparam name="T">The type of data the <see cref="Node{T}"/> will hold.</typeparam>
        /// <typeparam name="TKey">The type of the primary and parent keys.</typeparam>
        /// <param name="collection">A collection of input data.</param>
        /// <param name="primaryKey">A function that returns a primary key from <typeparamref name="T"/>.</param>
        /// <param name="parentKey">A function that returns a parent key from <typeparamref name="T"/>.</param>
        /// <param name="rootParentValue">The value that defines a root node.</param>
        /// <returns>Returns an <see cref="Enumerable"/> collection of root nodes populated from the input collection.</returns>
        public static IEnumerable<Node<T>> GenerateHierarchies<T, TKey>(IEnumerable<T> collection, Func<T, TKey> primaryKey, Func<T, TKey> parentKey)
        {
            return GenerateHierarchies(collection, primaryKey, parentKey, default(TKey));
        }

        /// <summary>
        /// Automatically generate a hierarchy from a collection of input data using a specified value to indicate a root node. This will return one or
        /// more root nodes.
        /// </summary>
        /// <typeparam name="T">The type of data the <see cref="Node{T}"/> will hold.</typeparam>
        /// <typeparam name="TKey">The type of the primary and parent keys.</typeparam>
        /// <param name="collection">A collection of input data.</param>
        /// <param name="primaryKey">A function that returns a primary key from <typeparamref name="T"/>.</param>
        /// <param name="parentKey">A function that returns a parent key from <typeparamref name="T"/>.</param>
        /// <param name="rootParentValue">The value that defines a root node.</param>
        /// <returns>Returns an <see cref="Enumerable"/> collection of root nodes populated from the input collection.</returns>
        public static IEnumerable<Node<T>> GenerateHierarchies<T, TKey>(IEnumerable<T> collection, Func<T, TKey> primaryKey, Func<T, TKey> parentKey, TKey rootParentValue)
        {
            collection.ArgumentNullCheck(nameof(collection));
            primaryKey.ArgumentNullCheck(nameof(primaryKey));
            parentKey.ArgumentNullCheck(nameof(parentKey));

            var groups = collection.GroupBy(x => parentKey(x));

            //Get all elements with a null primary key.
            var roots = groups.FirstOrDefault(x => EqualityComparer<TKey>.Default.Equals(x.Key, rootParentValue));
            if (roots == null)
            {
                throw new HierarchyBuilderException("A root node was not found in the collection.");
            }

            //Build a Dictionary that maps children to their parent keys.
            var parentChildDict = groups.Where(x => x.Key != null).ToDictionary(x => x.Key, x => x.ToList());

            //Recursively add children to the root node.
            foreach(var val in roots)
            {
                var rootNode = new Node<T>(val);
                addChildren(rootNode, parentChildDict, primaryKey);

                yield return rootNode;
            }
        }

        //Recursively add children to a node. Children are found via the key lookup dictionary.
        private static void addChildren<T, TKey>(Node<T> node, IDictionary<TKey, List<T>> source, Func<T, TKey> primayKey)
        {
            if(source.ContainsKey(primayKey(node.Data)))
            {
                node.AddChild(source[primayKey(node.Data)].Select(x => new Node<T>(x)).ToArray());
                foreach(var child in node.Children)
                {
                    addChildren(child, source, primayKey);
                }
            }
        }
    }
}
