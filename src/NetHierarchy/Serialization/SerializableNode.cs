using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetHierarchy.Serialization
{
    /// <summary>
    /// Represents a point on a hierarchy in a way that allows for Serialization and De-serialization.
    /// </summary>
    /// <typeparam name="T">The type of data that will be sorted in the <see cref="SerializableNode{T}"/> .</typeparam>
    public class SerializableNode<T>
    {
        #region Public Properties
        /// <summary>
        /// Get or set that data that belongs to the node.
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Get or set an <see cref="ICollection{T}"/> of nodes that are a child to the node.
        /// </summary>
        public List<SerializableNode<T>> Children { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initialize a new instance of <see cref="SerializableNode{T}"/>.
        /// </summary>
        public SerializableNode()
        {
            this.Children = new List<SerializableNode<T>>();
        }

        /// <summary>
        /// Initialize a new instance of <see cref="SerializableNode{T}"/>.
        /// </summary>
        /// <param name="Data">That data that the <see cref="SerializableNode{T}"/> will hold.</param>
        public SerializableNode(T Data) : this()
        {
            this.Data = Data;
        }

        /// <summary>
        /// Initialize a new instance of <see cref="SerializableNode{T}"/>.
        /// </summary>
        /// <param name="Data">The data that the <see cref="SerializableNode{T}"/> will hold.</param>
        /// <param name="Children">An <see cref="ICollection{T}"/> of <see cref="SerializableNode{T}"/> that are children to this node.</param>
        public SerializableNode(T Data, List<SerializableNode<T>> Children)
        {
            this.Data = Data;
            this.Children = Children;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Add a <see cref="SerializableNode{T}"/> as a child under the node.
        /// </summary>
        /// <param name="ChildNode">The child node to add.</param>
        public void AddChild(SerializableNode<T> ChildNode)
        {
            this.Children.Add(ChildNode);
        }
        #endregion

        #region Cast Methods
        /// <summary>
        /// Convert this hierarchy, starting at this node, into a <see cref="Node{T}"/>. Parent/child relationships will be created.
        /// </summary>
        /// <returns>A <see cref="Node{T}"/> populated with children.</returns>
        public Node<T> AsNode()
        {
            var node = new Node<T>(this.Data);

            foreach (var child in this.Children)
                node.AddChild(child.AsNode());

            return node;
        }

        /// <summary>
        /// Convert the <see cref="SerializableNode{T}"/> into a <see cref="Node{T}"/>, populating the parent/child relationships.  
        /// </summary>
        /// <param name="SerializableNode">The <see cref="SerializableNode{T}"/> to cast.</param>
        public static explicit operator Node<T>(SerializableNode<T> SerializableNode)
        {
            return SerializableNode.AsNode();
        }
        #endregion

        #region Object Overrides
        public override string ToString()
        {
            return this.Data.ToString();
        }
        #endregion
    }
}
