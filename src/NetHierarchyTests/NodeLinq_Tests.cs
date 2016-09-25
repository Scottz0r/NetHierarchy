using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using NetHierarchy;

namespace NetHierarchyTests
{
    [TestClass]
    public class NodeLinq_Tests
    {
        [TestMethod]
        public void NodeLinq_DescendantsWhere()
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
        public void NodeLinq_DescendantsWhere_NoMatch()
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
        public void NodeLinq_DescendantsWhere_ArgumentNull()
        {
            var root = new Node<int>(1);

            var actual = root.DescendantsWhere(null);
            var actualList = actual.ToList();
        }

        [TestMethod]
        public void NodeLinq_DescendantsAny()
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

            var actual = root.DescendantsAny(x => x.Data == 2);

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void NodeLinq_DescendantsAny_NoMatch()
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

            var actual = root.DescendantsAny(x => x.Data == 1000000000);

            Assert.IsFalse(actual);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NodeLinq_DescendantsAny_ArgumentNull()
        {
            var root = new Node<int>(1);

            var actual = root.DescendantsAny(null);
        }

        [TestMethod]
        public void NodeLinq_DescendantsContains()
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

            var actual = root.DescendantsContains(3);

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void NodeLinq_DescendantsContains_NotContains()
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

            var actual = root.DescendantsContains(13333337);

            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void NodeLinq_DescendantsContains_ArgumentNull()
        {
            var root = new Node<string>("ASDF");

            var actual = root.DescendantsContains(null);

            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void NodeLinq_ParentsWhere()
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

            var actual = child.ParentsWhere(x => x.Data == 2);
            var actualList = actual.ToList();

            Assert.AreEqual(1, actualList.Count);
            CollectionAssert.Contains(actualList, grandparent);
        }

        [TestMethod]
        public void NodeLinq_ParentsWhere_Multiple()
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

            var actual = child.ParentsWhere(x => x.Data == 2 || x.Data == 1 || x.Data == 3);
            var actualList = actual.ToList();

            Assert.AreEqual(3, actualList.Count);
            CollectionAssert.Contains(actualList, grandparent);
            CollectionAssert.Contains(actualList, root);
            CollectionAssert.Contains(actualList, parent);
        }

        [TestMethod]
        public void NodeLinq_ParentsWhere_NonParent()
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

            var actual = child.ParentsWhere(x => x.Data == 4);
            var actualList = actual.ToList();

            Assert.AreEqual(0, actualList.Count);
        }

        [TestMethod]
        public void NodeLinq_ParentsWhere_NoParent()
        {
            var root = new Node<string>("Root");

            var actual = root.ParentsWhere(x => x.Data is string);

            Assert.AreEqual(0, actual.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NodeLinq_ParentsWhere_NullPredicate()
        {
            var root = new Node<string>("Root");

            var actual = root.ParentsWhere(null);
            var atualList = actual.ToList();
        }

        [TestMethod]
        public void NodeLinq_ParentsAny_EmptyOverload()
        {
            var root = new Node<int>(1);
            var child = new Node<int>(123);

            root.AddChild(child);

            var actualRoot = root.ParentsAny();
            var actualChild = child.ParentsAny();

            Assert.IsFalse(actualRoot);
            Assert.IsTrue(actualChild);
        }

        [TestMethod]
        public void NodeLinq_ParentsAny()
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

            var actual = child.ParentsAny(x => x.Data == 1);

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void NodeLinq_ParentsAny_NoMacth()
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

            var actual = child.ParentsAny(x => x.Data == 1000000000);

            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void NodeLinq_ParentsAny_NonParentMatch()
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

            var actual = child.ParentsAny(x => x.Data == 4);

            Assert.IsFalse(actual);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NodeLinq_ParentsAny_NullPredicate()
        {
            var root = new Node<int>(1);

            root.ParentsAny(null);
        }

        [TestMethod]
        public void NodeLinq_ParentsContains()
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

            var actual = child.ParentsContains(2);

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void NodeLinq_ParentsContains_NoMatch()
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

            var actual = child.ParentsContains(1337);

            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void NodeLinq_ParentsContains_NonParentMatch()
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

            var actual = child.ParentsContains(4);

            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void NodeLinq_ParentsContains_ArgumentNull()
        {
            var root = new Node<string>("Cats");

            var actual = root.ParentsContains(null);

            Assert.IsFalse(actual);
        }
    }
}
