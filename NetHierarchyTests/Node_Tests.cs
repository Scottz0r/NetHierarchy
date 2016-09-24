using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetHierarchy;
using System.Linq;

namespace NetHierarchyTests
{
    [TestClass]
    public class Node_Tests
    {
        [TestMethod]
        public void Node_IsLeaf_NoChildren()
        {
            var node = new Node<int>();

            var actual = node.IsLeaf;

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void Node_IsLeaf_WithChildren()
        {
            var node = new Node<int>();
            node.Children.Add(new Node<int>(1));

            var actual = node.IsLeaf;

            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void Node_AddChild()
        {
            var node = new Node<int>(1);
            var childNode = new Node<int>(2);

            node.AddChild(childNode);

            Assert.AreEqual(childNode, node.Children.ElementAt(0));
            Assert.AreEqual(node, childNode.Parent);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Node_AddChild_ArgumentNull()
        {
            var node = new Node<int>(1);
            Node<int> childNode = null;

            node.AddChild(childNode);
        }

        [TestMethod]
        public void Node_GetDescendants()
        {
            var root = new Node<string>("Root");
            var child1 = new Node<string>("Child1");
            var child2 = new Node<string>("Child2");
            var child1Child = new Node<string>("Child1Child");

            root.AddChild(child1);
            root.AddChild(child2);
            child1.AddChild(child1Child);

            var actual = root.GetDescendants();
            var actualList = actual.ToList();

            Assert.AreEqual(4, actual.Count());
            CollectionAssert.Contains(actualList, root);
            CollectionAssert.Contains(actualList, child1);
            CollectionAssert.Contains(actualList, child2);
            CollectionAssert.Contains(actualList, child1Child);
        }

        [TestMethod]
        public void Node_GetDescendants_Singe()
        {
            var root = new Node<string>("Root");

            var actual = root.GetDescendants();
            var actualList = actual.ToList();

            Assert.AreEqual(1, actual.Count());
            CollectionAssert.Contains(actualList, root);
        }

        [TestMethod]
        public void Node_GetAncestors()
        {
            var root = new Node<string>("Root");
            var grandparent = new Node<string>("Grandparent");
            var parent = new Node<string>("Parent");
            var parent2 = new Node<string>("Parent2");
            var child = new Node<string>("Child");

            root.AddChild(grandparent);
            grandparent.AddChild(parent);
            grandparent.AddChild(parent2);
            parent.AddChild(child);

            var actual = child.GetAncestors();
            var actualList = actual.ToList();

            Assert.AreEqual(3, actualList.Count);
            CollectionAssert.Contains(actualList, root);
            CollectionAssert.Contains(actualList, grandparent);
            CollectionAssert.Contains(actualList, parent);
        }

        [TestMethod]
        public void Node_GetAncestors_NoParent()
        {
            var root = new Node<string>("Root");

            var actual = root.GetAncestors();

            Assert.AreEqual(0, actual.Count());
        }

        [TestMethod]
        public void Node_IsDescendantOf()
        {
            var root = new Node<string>("Root");
            var grandparent = new Node<string>("Grandparent");
            var parent = new Node<string>("Parent");
            var child = new Node<string>("Child");

            root.AddChild(grandparent);
            grandparent.AddChild(parent);
            parent.AddChild(child);

            var actual = child.IsDescendantOf(root);

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void Node_IsDescendantOf_NotDescendant()
        {
            var root = new Node<string>("Root");
            var grandparent = new Node<string>("Grandparent");
            var parent = new Node<string>("Parent");
            var parent2 = new Node<string>("Parent2");
            var child = new Node<string>("Child");

            root.AddChild(grandparent);
            grandparent.AddChild(parent);
            grandparent.AddChild(parent2);
            parent.AddChild(child);

            var actual = child.IsDescendantOf(parent2);

            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void Node_IsDescendantOf_Immediate()
        {
            var grandparent = new Node<string>("Grandparent");
            var parent = new Node<string>("Parent");

            grandparent.AddChild(parent);

            var actual = parent.IsDescendantOf(grandparent);

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void Node_IsDescendantOf_NoParent()
        {
            var node = new Node<string>("LittleOrphanBoy");
            var other = new Node<string>("OtherNode");

            var actual = node.IsDescendantOf(other);

            Assert.IsFalse(actual);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Node_IsDescendantOf_NullException()
        {
            var node = new Node<string>("Cats");

            var actual = node.IsDescendantOf(null);
        }

        [TestMethod]
        public void Node_DescendantsWhere()
        {
            var root = new Node<int>(1);
            var grandparent = new Node<int>(2);
            var parent = new Node<int>(3);
            var parent2 = new Node<int>(4);
            var child = new Node<int>(5);

            root.AddChild(grandparent);
            grandparent.AddChild(parent);
            grandparent.AddChild(parent2);
            parent.AddChild(child);

            var actual = root.DescendantsWhere(x => x.Data > 2);
            var actualList = actual.ToList();

            Assert.AreEqual(3, actualList.Count);
            CollectionAssert.Contains(actualList, parent);
            CollectionAssert.Contains(actualList, parent2);
            CollectionAssert.Contains(actualList, child);
        }

        [TestMethod]
        public void Node_DescendantsWhere_NoMatch()
        {
            var root = new Node<int>(1);
            var grandparent = new Node<int>(2);
            var parent = new Node<int>(3);
            var parent2 = new Node<int>(4);
            var child = new Node<int>(5);

            root.AddChild(grandparent);
            grandparent.AddChild(parent);
            grandparent.AddChild(parent2);
            parent.AddChild(child);

            var actual = root.DescendantsWhere(x => x.Data == 1337);
            var actualList = actual.ToList();

            Assert.AreEqual(0, actualList.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Node_DescendantsWhere_ArgumentNull()
        {
            var root = new Node<int>(1);

            var actual = root.DescendantsWhere(null);
            var actualList = actual.ToList();
        }
    }
}
