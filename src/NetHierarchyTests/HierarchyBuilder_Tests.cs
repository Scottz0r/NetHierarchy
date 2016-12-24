using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetHierarchy;
using System.Collections.Generic;
using System.Linq;
using System.Dynamic;

namespace NetHierarchyTests
{
    [TestClass]
    public class HierarchyBuilder_Tests
    {
        //Should generate a hierarchy of only related nodes.
        [TestMethod]
        public void HierarchyBuilder_GenerateHierarchy()
        {
            var testData = new List<TestData>
            {
                new TestData(1, null, "root"),
                new TestData(2, 1, "Child"),
                new TestData(3, 2, "Grand Child"),
                new TestData(4, 10, "Not Related"),
                new TestData(5, 1, "Child 2"),
                new TestData(6, 1, "Child 3")
            };

            var result = HierarchyBuilder.GenerateHierarchy(testData, x => x.Id, x => x.ParentId);

            Assert.AreEqual(3, result.Children.Count);
            Assert.AreEqual("root", result.Data.SomeValue);
            Assert.IsTrue(result.Children.Any(x => x.Data.SomeValue == "Child"));
            Assert.IsTrue(result.Children.Any(x => x.Data.SomeValue == "Child 3"));
            Assert.IsTrue(result.Children.Single(x => x.Data.Id == 2).Children.Any(x => x.Data.SomeValue == "Grand Child"));
            Assert.IsFalse(result.DescendantsAny(x => x.Data.SomeValue == "Not Related"));
        }

        //Should generate an error if there are no root nodes.
        [TestMethod]
        [ExpectedException(typeof(HierarchyBuilderException))]
        public void HierarchyBuilder_GenerateHierarchy_NoRoot()
        {
            var testData = new List<TestData>
            {
                new TestData(2, 1, "Child"),
                new TestData(3, 2, "Grand Child"),
                new TestData(4, 10, "Not Related"),
                new TestData(5, 1, "Child 2"),
                new TestData(6, 1, "Child 3")
            };

            var actual = HierarchyBuilder.GenerateHierarchy(testData, x => x.Id, x => x.ParentId);
        }

        //Should use a specified value when looking for the root node.
        [TestMethod]
        public void HierarchyBuilder_GenerateHierarchy_RootParentValue()
        {
            var testData = new List<TestData>
            {
                new TestData(1, -1337, "Root"),
                new TestData(2, 1, "Child"),
                new TestData(3, 2, "Grand Child"),
                new TestData(4, 10, "Not Related")
            };

            var actual = HierarchyBuilder.GenerateHierarchy(testData, x => x.Id, x => x.ParentId, -1337);

            Assert.AreEqual("Root", actual.Data.SomeValue);
            Assert.IsTrue(actual.Children.Any(x => x.Data.SomeValue == "Child"));
            Assert.IsTrue(actual.Children.First().Children.Any(x => x.Data.SomeValue == "Grand Child"));
            Assert.IsFalse(actual.DescendantsAny(x => x.Data.SomeValue == "Not Related"));
        }

        //Should produce an error when more than one root is found in the single hierarchy builder method.
        [TestMethod]
        [ExpectedException(typeof(HierarchyBuilderException))]
        public void HierarchyBuilder_GenerateHierarchy_MultipleRoots()
        {
            var testData = new List<TestData>
            {
                new TestData(1, null, "Root1"),
                new TestData(2, null, "Root2"),
                new TestData(3, 1, "Child1")
            };

            var actual = HierarchyBuilder.GenerateHierarchy(testData, x => x.Id, x => x.ParentId);
        }

        //Duplicate primary keys should not matter because the hierarchy is being build from top down.
        [TestMethod]
        public void HierarchyBuilder_GenerateHierarchy_NonUniquePrimaryKey()
        {
            var testData = new List<TestData>
            {
                new TestData(1, null, "Root"),
                new TestData(2, 1, "Child1"),
                new TestData(2, 1, "Dupe Child")
            };

            var actual = HierarchyBuilder.GenerateHierarchy(testData, x => x.Id, x => x.ParentId);

            Assert.AreEqual("Root", actual.Data.SomeValue);
            Assert.AreEqual(2, actual.Children.Count);
            Assert.AreEqual(0, actual.Children.First().Children.Count);
            Assert.AreEqual(0, actual.Children.Last().Children.Count);
        }

        //Should build multiple hierarchies from multiple root node input.
        [TestMethod]
        public void HierarchyBuilder_GenerateHierarchies_Many()
        {
            var testData = new List<TestData>
            {
                new TestData(1, null, "Root1"),
                new TestData(2, null, "Root2"),
                new TestData(3, 1, "Child1"),
                new TestData(4, 2, "Child2"),
                new TestData(5, 3, "Grand Child1"),
                new TestData(6, 4, "Grand Child2")
            };

            var actual = HierarchyBuilder.GenerateHierarchies(testData, x => x.Id, x => x.ParentId).ToList();

            Assert.AreEqual(2, actual.Count());
            Assert.IsTrue(actual.First(x => x.Data.Id == 1).Children.Any(x => x.Data.SomeValue == "Child1"));
            Assert.IsTrue(actual.First(x => x.Data.Id == 2).Children.Any(x => x.Data.SomeValue == "Child2"));
            Assert.IsTrue(actual
                .First(x => x.Data.Id == 1)
                .Children.First(x => x.Data.Id == 3)
                .Children.Any(x => x.Data.SomeValue == "Grand Child1"));
            //Ensure no stragglers.
            Assert.AreEqual(0, actual.First().Children.First().Children.First().Children.Count);
            Assert.AreEqual(0, actual.Last().Children.First().Children.First().Children.Count);
        }

        //Should generate a collection with only one root.
        [TestMethod]
        public void HierarchyBuilder_GenerateHierarchies_Single()
        {
            var testData = new List<TestData>
            {
                new TestData(1, null, "Root1"),
                new TestData(3, 1, "Child1"),
                new TestData(5, 3, "Grand Child1"),
            };

            var actual = HierarchyBuilder.GenerateHierarchies(testData, x => x.Id, x => x.ParentId).ToList();

            Assert.AreEqual(1, actual.Count());
            Assert.IsTrue(actual.First(x => x.Data.Id == 1).Children.Any(x => x.Data.SomeValue == "Child1"));
            Assert.IsTrue(actual
                .First(x => x.Data.Id == 1)
                .Children.First(x => x.Data.Id == 3)
                .Children.Any(x => x.Data.SomeValue == "Grand Child1"));
            //Ensure there are no stragglers
            Assert.AreEqual(0, actual.First().Children.First().Children.First().Children.Count);
        }

        //Should throw an error if no roots found.
        [TestMethod]
        [ExpectedException(typeof(HierarchyBuilderException))]
        public void HierarchyBuilder_GenerateHierarchies_NoRoot()
        {
            var testData = new List<TestData>
            {
                new TestData(3, 1, "Child1"),
                new TestData(5, 3, "Grand Child1"),
            };

            var actual = HierarchyBuilder.GenerateHierarchies(testData, x => x.Id, x => x.ParentId).ToList();
        }

        //Should build multiple roots using a different root key value.
        [TestMethod]
        public void HierarchyBuilder_GenerateHierarchies_RootParentValue()
        {
            var testData = new List<TestData>
            {
                new TestData(1, -1337, "Root"),
                new TestData(20, -1337, "Root2"),
                new TestData(2, 1, "Child"),
                new TestData(3, 2, "Grand Child"),
                new TestData(4, 10, "Not Related")
            };

            var actual = HierarchyBuilder.GenerateHierarchies(testData, x => x.Id, x => x.ParentId, -1337).ToList();

            Assert.AreEqual("Root", actual.First(x => x.Data.Id == 1).Data.SomeValue);
            Assert.AreEqual("Root2", actual.First(x => x.Data.Id == 20).Data.SomeValue);
            Assert.IsTrue(actual.First(x => x.Data.Id == 1).Children.Any(x => x.Data.SomeValue == "Child"));
        }

        #region Various Keys Test

        //Should be able to resolve some wonkey primary keys.
        [TestMethod]
        public void HierarchyBuilder_GenerateHierarchy_CompositeKeys()
        {
            var testData = new List<CompositeKeyTest>
            {
                new CompositeKeyTest(1, "test", -1, "None"),
                new CompositeKeyTest(2, "meep", 1, "test")
            };

            var actual = HierarchyBuilder.GenerateHierarchy(testData, x => x.Key1.ToString() + x.Key2, x => x.ParentKey1.ToString() + x.ParentKey2, "-1None");

            Assert.AreEqual("test", actual.Data.Key2);
            Assert.AreEqual(1, actual.Children.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(HierarchyBuilderException))]
        public void HierarchyBuilder_GenerateHierarchy_CompositeKeyNoRoot()
        {
            var testData = new List<CompositeKeyTest>
            {
                new CompositeKeyTest(1, "test", 10110, "BeepBoop"),
                new CompositeKeyTest(2, "meep", 1, "test")
            };

            var actual = HierarchyBuilder.GenerateHierarchy(testData, x => x.Key1.ToString() + x.Key2, x => x.ParentKey1.ToString() + x.ParentKey2, "-1None");
        }

        #endregion

        #region Test Classes
        class TestData
        {
            public int Id { get; set; }

            public int? ParentId { get; set; }

            public string SomeValue { get; set; }

            public TestData()
            {

            }

            public TestData(int id, int? parentId, string value)
            {
                Id = id;
                ParentId = parentId;
                SomeValue = value;
            }

            public override string ToString()
            {
                return SomeValue;
            }
        }

        class CompositeKeyTest
        {
            public int Key1 { get; set; }

            public string Key2 { get; set; }

            public int ParentKey1 { get; set; }

            public string ParentKey2 { get; set; }

            public CompositeKeyTest(int key1, string key2, int parentkey1, string parentkey2)
            {
                Key1 = key1;
                Key2 = key2;
                ParentKey1 = parentkey1;
                ParentKey2 = parentkey2;
            }
        }

        #endregion

        #region Stress Tests

        //Stress the builder method to see if it can handle large data sets.
        [TestMethod]
        [TestCategory("Stress")]
        public void HierarchyBuilder_StressTest()
        {
            var data = Enumerable.Range(1, 1000000)
                .Select(x => Tuple.Create(x, x != 1 ? (x / 10) + 1 : -1)).ToList();

            var root = HierarchyBuilder.GenerateHierarchy(data, x => x.Item1, x => x.Item2, -1);
        }

        #endregion

        #region Argument NullTests
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HierarchyBuilder_GenerateHierarchy_CollectionNull()
        {
            var actual = HierarchyBuilder.GenerateHierarchy<TestData, int?>(null, x => x.Id, x => x.ParentId, -1337);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HierarchyBuilder_GenerateHierarchy_PrimaryKeyNull()
        {
            var data = new List<TestData>();

            var actual = HierarchyBuilder.GenerateHierarchy(data, null, x => x.ParentId, -1337);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HierarchyBuilder_GenerateHierarchy_ParentKeyNulll()
        {
            var data = new List<TestData>();

            var actual = HierarchyBuilder.GenerateHierarchy(data, x => x.Id, null, -1337);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HierarchyBuilder_GenerateHierarchies_CollectionNull()
        {
            var actual = HierarchyBuilder.GenerateHierarchies<TestData, int?>(null, x => x.Id, x => x.ParentId, -1337).ToList();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HierarchyBuilder_GenerateHierarchies_PrimaryKeyNull()
        {
            var data = new List<TestData>();

            var actual = HierarchyBuilder.GenerateHierarchies(data, null, x => x.ParentId, -1337).ToList();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HierarchyBuilder_GenerateHierarchies_ParentKeyNulll()
        {
            var data = new List<TestData>();

            var actual = HierarchyBuilder.GenerateHierarchies(data, x => x.Id, null, -1337).ToList();
        }
        #endregion

        #region Dynamic Tests
        [TestMethod]
        public void HierarchyBuilder_Dynamic()
        {
            dynamic rootData = new ExpandoObject();
            rootData.Id = 1;
            rootData.PId = null;
            dynamic childData = new ExpandoObject();
            childData.Id = 2;
            childData.PId = 1;
            var input = new List<dynamic> { rootData, childData };

            var actual = HierarchyBuilder.GenerateHierarchy(input, x => x.Id, x => x.PId, null);

            Assert.AreEqual(1, actual.Data.Id);
        }
        #endregion
    }
}
