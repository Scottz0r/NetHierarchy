using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetHierarchy.Serialization;
using System.Linq;
using NetHierarchy;
using System.Collections.Generic;

namespace NetHierarchyTests
{
    [TestClass]
    public class SerializableNode_Tests
    {
        #region Constructor
        [TestMethod]
        public void SerializableNode_DefaultConstructor()
        {
            var node = new SerializableNode<string>();
            Assert.IsNotNull(node.Children);
        }

        [TestMethod]
        public void SerializableNode_ConstructorData()
        {
            var node = new SerializableNode<string>("Data");

            Assert.IsNotNull(node.Children);
            Assert.AreEqual("Data", node.Data);
        }

        [TestMethod]
        public void SerializableNode_ConstructorDataChildren()
        {
            var child1 = new SerializableNode<string>("Child1");
            var child2 = new SerializableNode<string>("Child 2");
            var childNodes = new List<SerializableNode<string>>{ child1, child2 };
            var node = new SerializableNode<string>("Data", childNodes);

            Assert.IsNotNull(node.Children);
            Assert.AreEqual("Data", node.Data);
            CollectionAssert.Contains(node.Children.ToList(), child1);
            CollectionAssert.Contains(node.Children.ToList(), child2);
        }
        #endregion

        [TestMethod]
        public void SerializableNode_AddChild()
        {
            var node = new SerializableNode<string>("Parent");
            var child = new SerializableNode<string>("Child");
            node.AddChild(child);

            Assert.AreEqual(1, node.Children.Count);
            Assert.AreEqual(child, node.Children.First());
        }

        [TestMethod]
        public void SerializableNode_AsNode()
        {
            var node = new SerializableNode<string>("Parent");
            var child = new SerializableNode<string>("Child");
            var grandchild = new SerializableNode<string>("Grandchild");
            var grandchild2 = new SerializableNode<string>("Grandchild2");
            node.AddChild(child);
            child.AddChild(grandchild);
            child.AddChild(grandchild2);

            var actual = node.AsNode();
            var childNode = actual.Children.First();
            var grandchildNode = childNode.Children.First();

            Assert.AreEqual(1, actual.Children.Count);
            Assert.AreEqual("Child", actual.Children.First().Data);
            Assert.IsNotNull(childNode.Parent);
            Assert.AreEqual(childNode.Parent, actual);
            Assert.AreEqual(2, childNode.Children.Count);
        }

        [TestMethod]
        public void SerializableNode_CastNode()
        {
            var node = new SerializableNode<string>("Parent");
            var child = new SerializableNode<string>("Child");
            var grandchild = new SerializableNode<string>("Grandchild");
            var grandchild2 = new SerializableNode<string>("Grandchild2");
            node.AddChild(child);
            child.AddChild(grandchild);
            child.AddChild(grandchild2);

            var actual = (Node<string>)node;
            var childNode = actual.Children.First();
            var grandchildNode = childNode.Children.First();

            Assert.AreEqual(1, actual.Children.Count);
            Assert.AreEqual("Child", actual.Children.First().Data);
            Assert.IsNotNull(childNode.Parent);
            Assert.AreEqual(childNode.Parent, actual);
            Assert.AreEqual(2, childNode.Children.Count);
        }

        [TestMethod]
        public void SerializableNode_ToString()
        {
            var node = new SerializableNode<string>("Data");
            var actual = node.ToString();
        }
    }
}
