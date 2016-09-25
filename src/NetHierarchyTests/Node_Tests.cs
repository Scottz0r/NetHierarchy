using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetHierarchy;
using System.Linq;
using System.Collections.Generic;
using NetHierarchy.Serialization;

namespace NetHierarchyTests
{
    [TestClass]
    public class Node_Tests
    {
        #region Constructors
        [TestMethod]
        public void Node_DefaultConstructor()
        {
            var node = new Node<string>();

            Assert.IsNotNull(node.Children);
            Assert.IsNull(node.Parent);
        }

        [TestMethod]
        public void Node_Constructor_Data()
        {
            var node = new Node<string>("Data");

            Assert.IsNotNull(node.Children);
            Assert.AreEqual("Data", node.Data);
            Assert.IsNull(node.Parent);
        }

        [TestMethod]
        public void Node_Constructor_DataParent()
        {
            var parent = new Node<string>("Parent");
            var node = new Node<string>("Data", parent);

            Assert.IsNotNull(node.Children);
            Assert.AreEqual("Data", node.Data);
            Assert.IsNotNull(node.Parent);
            Assert.AreEqual("Parent", node.Parent.Data);
        }

        [TestMethod]
        public void Node_Constructor_DataParentdata()
        {
            var node = new Node<string>("Data", "Parent");

            Assert.IsNotNull(node.Children);
            Assert.AreEqual("Data", node.Data);
            Assert.IsNotNull(node.Parent);
            Assert.AreEqual("Parent", node.Parent.Data);
        }

        [TestMethod]
        public void Node_Constructor_Children()
        {
            List<Node<string>> children = new List<Node<string>>
            {
                new Node<string>("Child1"),
                new Node<string>("Child2")
            };
            var node = new Node<string>(children);

            Assert.AreEqual(children, node.Children);
            Assert.AreEqual(node, node.Children.First().Parent);
            Assert.IsNull(node.Parent);
        }

        [TestMethod]
        public void Node_Constructor_DataChildren()
        {
            List<Node<string>> children = new List<Node<string>>
            {
                new Node<string>("Child1"),
                new Node<string>("Child2")
            };
            var node = new Node<string>("Data", children);

            Assert.AreEqual("Data", node.Data);
            Assert.AreEqual(children, node.Children);
            Assert.AreEqual(node, node.Children.First().Parent);
            Assert.IsNull(node.Parent);
        }

        [TestMethod]
        public void Node_Constructor_DataChildrenParent()
        {
            List<Node<string>> children = new List<Node<string>>
            {
                new Node<string>("Child1"),
                new Node<string>("Child2")
            };
            var node = new Node<string>("Data", children);
            var parent = new Node<string>("Parent");
            parent.AddChild(node);

            Assert.AreEqual("Data", node.Data);
            Assert.AreEqual(node.Parent, parent);
            Assert.AreEqual(children, node.Children);
            Assert.AreEqual(node, node.Children.First().Parent);
        }

        [TestMethod]
        public void Node_Constructor_DataChildrenParentdata()
        {
            List<Node<string>> children = new List<Node<string>>
            {
                new Node<string>("Child1"),
                new Node<string>("Child2")
            };
            var node = new Node<string>("Data", children, "Parent");

            Assert.AreEqual("Data", node.Data);
            Assert.AreEqual("Parent", node.Parent.Data);
            Assert.AreEqual(children, node.Children);
            Assert.AreEqual(node, node.Children.First().Parent);
        }
        #endregion

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
        public void Node_ToString()
        {
            var node = new Node<string>("Test");

            var actual = node.ToString();

            Assert.AreEqual("Test", actual);
        }

        #region SerializableNode Methods
        [TestMethod]
        public void Node_AsSerializable()
        {
            var root = new Node<string>("ASDF");
            var child = new Node<string>("Child");
            root.AddChild(child);

            var actual = root.AsSerializable();

            Assert.AreEqual(1, root.Children.Count);
            Assert.AreEqual("ASDF", actual.Data);
            Assert.AreEqual("Child", actual.Children.First().Data);
        }

        [TestMethod]
        public void Node_Cast_SerializableNode()
        {
            var root = new Node<string>("ASDF");
            var child = new Node<string>("Child");
            root.AddChild(child);

            var actual = (SerializableNode<string>)root;

            Assert.AreEqual(1, root.Children.Count);
            Assert.AreEqual("ASDF", actual.Data);
            Assert.AreEqual("Child", actual.Children.First().Data);
        }
        #endregion

        #region Equals Tests
        [TestMethod]
        public void Node_Equals_xx()
        {
            var x = new Node<int>(10);
            var y = new Node<int>(33);

            var actual = x.Equals(x);

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void Node_Equals_xy_yx()
        {
            var x = new Node<int>(10);
            var y = new Node<int>(33);

            var actualxy = x.Equals(y);
            var actualyx = y.Equals(x);

            Assert.AreEqual(actualxy, actualyx);
        }

        [TestMethod]
        public void Node_Equals_xyz()
        {
            var x = new Node<int>(10);
            var y = new Node<int>(10);
            var z = new Node<int>(10);

            var xy = x.Equals(y);
            var yz = y.Equals(z);
            var xz = x.Equals(z);

            Assert.IsTrue(xy);
            Assert.IsTrue(yz);
            Assert.IsTrue(xz);
        }

        [TestMethod]
        public void Node_Equals_xy_xy_xy()
        {
            var x = new Node<int>(10);
            var y = new Node<int>(33);

            var actual1 = x.Equals(y);
            var actual2 = x.Equals(y);
            var actual3 = x.Equals(y);

            Assert.AreEqual(actual1, actual2);
            Assert.AreEqual(actual1, actual3);
        }

        [TestMethod]
        public void Node_Equals_Null()
        {
            var x = new Node<int>(10);

            var actual = x.Equals(null);

            Assert.IsFalse(actual);
        }
        #endregion
    }
}
