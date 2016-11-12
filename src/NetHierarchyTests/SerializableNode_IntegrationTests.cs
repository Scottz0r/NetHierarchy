using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using NetHierarchy.Serialization;
using System.IO;
using Newtonsoft.Json;
using NetHierarchy;

namespace NetHierarchyTests
{
    [TestClass]
    public class SerializableNode_IntegrationTests
    {
        [TestMethod]
        [TestCategory("Integration")]
        public void SerialableNode_XmlSerialization()
        {
            var root = new SerializableNode<string>("Root");
            var child = new SerializableNode<string>("Child");
            root.AddChild(child);

            var serializer = new XmlSerializer(typeof(SerializableNode<string>));

            string actual;

            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, root);
                actual = writer.ToString();
            }
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void SerializableNode_XmlDeserialization()
        {
            var root = new SerializableNode<string>("Root");
            var child = new SerializableNode<string>("Child");
            root.AddChild(child);

            var serializer = new XmlSerializer(typeof(SerializableNode<string>));

            string actual;

            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, root);
                actual = writer.ToString();
            }

            using (StringReader reader = new StringReader(actual))
            {
                var deseralized = (SerializableNode<string>)serializer.Deserialize(reader);
            }
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void SerializableNode_JsonSerialize()
        {
            var root = new SerializableNode<string>("Root");
            var child = new SerializableNode<string>("Child");
            root.AddChild(child);

            var obj = JsonConvert.SerializeObject(root);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void SerializableNode_JsonDesearilized()
        {
            var root = new SerializableNode<string>("Root");
            var child = new SerializableNode<string>("Child");
            root.AddChild(child);

            var json = JsonConvert.SerializeObject(root);

            var desearilized = JsonConvert.DeserializeObject<SerializableNode<string>>(json);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void Node_ToSerializableIntegration()
        {
            var root = new Node<int>(1);
            var child1 = new Node<int>(2);
            var child2 = new Node<int>(3);
            var grandchild = new Node<int>(4);
            var grandchild2 = new Node<int>(5);

            root.AddChild(child1);
            root.AddChild(child2);
            child1.AddChild(grandchild);
            child1.AddChild(grandchild2);

            var serializer = new XmlSerializer(typeof(SerializableNode<int>));
            string xml;
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, (SerializableNode<int>)root);
                xml = writer.ToString();
            }

            SerializableNode<int> desearilized;
            using (var reader = new StringReader(xml))
            {
                desearilized = (SerializableNode<int>)serializer.Deserialize(reader);
            }

            var fullCircle = desearilized.AsNode();

            Assert.AreEqual(1, desearilized.Data);
            Assert.IsNotNull(fullCircle.Children.First().Parent);
        }
    }
}
