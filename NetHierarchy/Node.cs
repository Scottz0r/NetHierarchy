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
    /// <typeparam name="T">The type of data that will be sotred in the node.</typeparam>
    public class Node<T>
    {
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

        #region Constructor

        #region Without Children
        public Node()
        {
            this.Children = new List<Node<T>>();
        }

        public Node(T Data) : this()
        {
            this.Data = Data;
        }

        public Node(T Data, Node<T> Parent) : this(Data)
        {
            this.Parent = Parent;

            if (!Parent.Children.Contains(this))
                Parent.AddChild(this);
        }

        public Node(T Data, T ParentData) : this(Data)
        {
            var parentNode = new Node<T>(ParentData);
            parentNode.AddChild(this);
        }

        #endregion

        #region With Children
        public Node(ICollection<Node<T>> Children)
        {
            this.Children = Children;
        }

        public Node(T Data, ICollection<Node<T>> Children) : this(Children)
        {
            this.Data = Data;
        }

        public Node(T Data, ICollection<Node<T>> Children, Node<T> Parent) :this(Data, Children)
        {
            this.Parent = Parent;
            if (!Parent.Children.Contains(this))
                Parent.AddChild(this);
        }

        public Node(T Data, ICollection<Node<T>> Children, T Parent) : this(Data, Children, new Node<T>(Parent))
        {
            
        }
        #endregion

        #endregion

        /// <summary>
        /// Add a child to this node and creates the parent/child relationship between the nodes.
        /// </summary>
        /// <param name="ChildNode">The child item to add.</param>
        public void AddChild(Node<T> ChildNode)
        {
            if (ChildNode == null) throw new ArgumentNullException(nameof(ChildNode));

            this.Children.Add(ChildNode);
            ChildNode.Parent = this;
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
            if (Check == null) throw new ArgumentNullException(nameof(Check));

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

        #region Linq Like Methods

        /// <summary>
        /// Filters the descendant nodes based on a predicate.
        /// </summary>
        public IEnumerable<Node<T>> DescendantsWhere(Func<Node<T>, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            if (predicate(this))
                yield return this;

            foreach(var child in Children)
            {
                foreach(var result in child.DescendantsWhere(predicate))
                {
                    yield return result;
                }
            }
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
