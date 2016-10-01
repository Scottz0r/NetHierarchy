using NetHierarchy.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetHierarchy
{
    /// <summary>
    /// Represents a point on a hierarchy and defines methods to manipulate hierarchies.
    /// </summary>
    /// <typeparam name="T">The type of data that will be stored in the <see cref="Node{T}"/> </typeparam>
    public class Node<T>
    {
        #region Public Properties

        /// <summary>
        /// Get or set that data that belongs to the node.
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Get or set an <see cref="ICollection{T}"/> of nodes that are a child to the node.
        /// </summary>
        public ICollection<Node<T>> Children { get; set; }

        /// <summary>
        /// Gets or sets the parent node of this node.
        /// </summary>
        public Node<T> Parent { get; set; }

        /// <summary>
        /// Gets a value indicating if the node is a leaf-node. A leaf-node has no children.
        /// </summary>
        public bool IsLeaf
        {
            get { return Children.Count == 0; }
        }

        /// <summary>
        /// Gets a value indicating if the node is a root-node. A root-node is at the top of hierarchy and has no parents.
        /// </summary>
        public bool IsRoot
        {
            get { return Parent == null; }
        }

        #endregion

        #region Constructors

        #region Without Children

        /// <summary>
        /// Initialize a new <see cref="Node{T}"/>. 
        /// </summary>
        public Node()
        {
            this.Children = new List<Node<T>>();
        }

        /// <summary>
        /// Initialize a new <see cref="Node{T}"/>. 
        /// </summary>
        /// <param name="Data">The data that the node will hold.</param>
        public Node(T Data) : this()
        {
            this.Data = Data;
        }

        /// <summary>
        /// Initialize a new <see cref="Node{T}"/> and create and map a parent <see cref="Node{T}"/>. 
        /// </summary>
        /// <param name="Data">The data that the node will hold.</param>
        /// <param name="Parent">The Parent <see cref="Node{T}"/> of this node.</param>
        public Node(T Data, Node<T> Parent) : this(Data)
        {
            this.Parent = Parent;

            if (!Parent.Children.Contains(this))
                Parent.AddChild(this);
        }

        /// <summary>
        /// Initialize a new <see cref="Node{T}"/> and create and map a parent <see cref="Node{T}"/>. 
        /// </summary>
        /// <param name="Data">The data that the node will hold.</param>
        /// <param name="ParentData">The data that the node's parent <see cref="Node{T}"/> will hold. A new <see cref="Node{T}"/> will be created with this value.</param>
        public Node(T Data, T ParentData) : this(Data)
        {
            var parentNode = new Node<T>(ParentData);
            parentNode.AddChild(this);
        }

        #endregion

        #region With Children
        /// <summary>
        /// Initialize a new <see cref="Node{T}"/> and add a collection of Children under the new <see cref="Node{T}"/> .
        /// </summary>
        /// <param name="Children">Collection of Child nodes that belong to this node.</param>
        public Node(ICollection<Node<T>> Children)
        {
            Children.ArgumentNullCheck(nameof(Children));
            this.Children = Children;
            foreach (var child in this.Children)
                if (child.Parent != this)
                    child.Parent = this;
        }

        /// <summary>
        /// Initialize a new <see cref="Node{T}"/> and add a collection of Children under the new <see cref="Node{T}"/> .
        /// </summary>
        /// <param name="Data">The data that the node will hold.</param>
        /// <param name="Children">Collection of Child nodes that belong to this node.</param>
        public Node(T Data, ICollection<Node<T>> Children) : this(Children)
        {
            this.Data = Data;
        }

        /// <summary>
        /// Initialize a new <see cref="Node{T}"/>. Adds a collection of Children under the new <see cref="Node{T}"/>.
        /// Adds the newly created node under an existing parent <see cref="Node{T}"/>. 
        /// </summary>
        /// <param name="Data">The data that the node will hold.</param>
        /// <param name="Children">Collection of Child nodes that belong to this node.</param>
        /// <param name="Parent">The Parent <see cref="Node{T}"/> of this node.</param>
        public Node(T Data, ICollection<Node<T>> Children, Node<T> Parent) :this(Data, Children)
        {
            this.Parent = Parent;
            if (!Parent.Children.Contains(this))
                Parent.AddChild(this);
        }

        /// <summary>
        /// Initialize a new <see cref="Node{T}"/>. Adds a collection of Children under the new <see cref="Node{T}"/>.
        /// Adds the newly created node under an existing parent <see cref="Node{T}"/>. 
        /// </summary>
        /// <param name="Data">The data that the node will hold.</param>
        /// <param name="Children">Collection of Child nodes that belong to this node.</param>
        /// <param name="Parent">The data that the node's parent <see cref="Node{T}"/> will hold. A new <see cref="Node{T}"/> will be created with this value.</param>
        public Node(T Data, ICollection<Node<T>> Children, T Parent) : this(Data, Children, new Node<T>(Parent))
        {
            
        }
        #endregion

        #endregion

        #region Hierarchy Methods
        /// <summary>
        /// Add a child to this node and creates the parent/child relationship between the nodes.
        /// </summary>
        /// <param name="ChildNode">The child item to add.</param>
        public void AddChild(Node<T> ChildNode)
        {
            ChildNode.ArgumentNullCheck(nameof(ChildNode));

            this.Children.Add(ChildNode);
            ChildNode.Parent = this;
        }

        /// <summary>
        /// Add a child with data to this node and create a parent/child relationship between the nodes.
        /// </summary>
        /// <param name="ChildData">The data that the child <see cref="Node{T}"/> will hold.</param>
        public void AddChild(T ChildData)
        {
            var childNode = new Node<T>(ChildData);

            this.Children.Add(childNode);
            childNode.Parent = this;
        }

        /// <summary>
        /// Add a collection of children to this node and create a parent/child relationship between the nodes.
        /// </summary>
        /// <param name="ChildNodes">A collection of <see cref="Node{T}"/> to add.</param>
        public void AddChild(params Node<T>[] ChildNodes)
        {
            ChildNodes.ArgumentNullCheck(nameof(ChildNodes));

            foreach(var node in ChildNodes)
            {
                this.Children.Add(node);
                node.Parent = this;
            }
        }

        /// <summary>
        /// Creates an <see cref="IEnumerable{T}"/> of all of the nodes underneath and including this node.
        /// </summary>
        public IEnumerable<Node<T>> GetDescendants()
        {
            yield return this;

            foreach(var node in Children)
            {
                foreach(var childAll in node.GetDescendants())
                {
                    yield return childAll;
                }
            }
        }

        /// <summary>
        /// Get all of the ancestors of the <see cref="Node{T}"/>. 
        /// </summary>
        public IEnumerable<Node<T>> GetAncestors()
        {
            if(this.Parent != null)
            {
                yield return this.Parent;

                foreach (var ancestor in this.Parent.GetAncestors())
                    yield return ancestor;
            }
        }

        /// <summary>
        /// Checks to see if the <see cref="Node{T}"/> is a child of the given <see cref="Node{T}"/>.
        /// Checks are done with reference checks.
        /// </summary>
        /// <param name="Check">The <see cref="Node{T}"/> to check under.</param>
        public bool IsDescendantOf(Node<T> Check)
        {
            Check.ArgumentNullCheck(nameof(Check));

            //No parent then it is not a descendant of anything.
            if (this.Parent == null)
                return false;

            //If Check is immediate parent, then true.
            if(object.ReferenceEquals(this.Parent, Check))
            {
                return true;
            }
            //Else begin walking up the hierarchy.
            else
            {
                return this.Parent.IsDescendantOf(Check);
            }
        }
        #endregion

        #region SerializableNode Casts
        /// <summary>
        /// Convert the hierarchy, starting at this node, into an object that can be serialized without circular dependency errors. 
        /// The hierarchy under this node will be copied into a new <see cref="SerializableNode{T}"/>. 
        /// </summary>
        /// <returns>A <see cref="SerializableNode{T}"/> populated with children.</returns>
        public SerializableNode<T> AsSerializable()
        {
            var serNode = new SerializableNode<T>(this.Data);
            foreach (var child in this.Children)
                serNode.AddChild(child.AsSerializable());

            return serNode;
        }

        /// <summary>
        /// Convert the <see cref="Node{T}"/> into a <see cref="SerializableNode{T}"/>. The Parent field will be lost in this conversion.
        /// </summary>
        /// <param name="Node">The <see cref="Node{T}"/> to cast.</param>
        public static explicit operator SerializableNode<T>(Node<T> Node)
        {
            return Node.AsSerializable();
        }

        #endregion

        #region Object Overrides
        /// <summary>
        /// Returns a string that represents the object.
        /// </summary>
        public override string ToString()
        {
            return this.Data.ToString();
        }

        /// <summary>
        /// Determines if the supplied object is equal to the current object.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            Node<T> n = obj as Node<T>;
            if ((object)n == null)
                return false;

            return this.Data.Equals(n.Data);
        }

        /// <summary>
        /// Returns the hash code of the Node's data.
        /// </summary>
        public override int GetHashCode()
        {
            return this.Data.GetHashCode();
        }
        #endregion
    }
}
