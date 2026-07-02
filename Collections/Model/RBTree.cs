using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using SystemEx.Utils;

namespace SystemEx.Collections.Model {
    /// \addtogroup model
    /// @{
    /// <summary>
    /// Represents a node in a red-black tree, a self-balancing binary search tree.
    /// </summary>
    public enum TreeColor {
        Red,
        Black
    }

    /// <summary>
    /// Represents a node in a red-black tree.
    /// </summary>
    /// <typeparam name="T">The type of the value stored in the node.</typeparam>
    public class RBTreeNode<T> : Tree<T, RBTreeNode<T>> {
        const int ILEFT = 2;
        const int IRIGHT = 1;
        const int IPARENT = 0;

        private CompFunc<T> m_cmp;

        /// <summary>
        /// Gets or sets a value indicating whether the tree should automatically rebalance itself.
        /// </summary>
        public bool AutoRebalance { get; set; }


        /// <summary>
        /// Gets or sets the comparison function used to compare values in the tree.
        /// </summary>
        public CompFunc<T> CompareFunc {
            protected set => m_cmp = value;
            get => m_cmp;
        }

        /// <summary>
        /// Gets or sets the parent node of this node.
        /// </summary>
        internal RBTreeNode<T>? Parent { get => m_pElemtents[IPARENT]; set => m_pElemtents[IPARENT] = value; }

        /// <summary>
        /// Gets or sets the left child node of this node.
        /// </summary> 
        public RBTreeNode<T>? Left { get => m_pElemtents[ILEFT]; internal set => m_pElemtents[ILEFT] = value; }
        /// <summary>
        /// Gets or sets the right child node of this node.
        /// </summary> 
        public RBTreeNode<T>? Right { get => m_pElemtents[IRIGHT]; internal set => m_pElemtents[IRIGHT] = value; }

        /// <summary>
        /// Gets a value indicating whether the node is empty.
        /// </summary>
        public bool IsEmpty => Value == null;

        /// <summary>
        /// Gets the number of nodes in the tree.
        /// </summary>
        public long Count => get_count();

        /// <summary>
        /// Gets a value indicating whether the node is a leaf.
        /// </summary>
        public bool IsLeaf => Left == null && Right == null;

        /// <summary>
        /// Gets or sets the color of the node.
        /// </summary>
        public TreeColor Color { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the RBTreeNode class.
        /// </summary>
        private RBTreeNode () : base(2, default(T)) { Color = TreeColor.Black;  }

        /// <summary>
        /// Initializes a new instance of the RBTreeNode class.
        /// </summary>
        /// <param name="value">The value of this node</param>
        /// <param name="cmp">The compare functions, for rebalance.</param>
        public RBTreeNode ( T value, CompFunc<T> cmp )
            : base(2, value) {
            m_pElemtents[IPARENT] = m_pElemtents[IRIGHT] = m_pElemtents[ILEFT] = null;
            m_cmp = cmp;
            Color = TreeColor.Black;
        }

        /// <summary>
        /// Initializes a new instance of the RBTreeNode class.
        /// </summary>
        /// <param name="other"></param>
        public RBTreeNode ( RBTreeNode<T> other ) : base(2, other.Value) {
            Parent = other.Parent;
            Left = other.Left;
            Right = other.Right;
            Value = other.Value;
            Color = other.Color;
        }

        /// <summary>
        /// Gets the first node in the tree.
        /// </summary>
        /// <returns>The First node</returns>
        public RBTreeNode<T>? Begin () {
            RBTreeNode<T>? iter = null;

            if ( Parent != null ) {
                iter = Parent;

                while ( iter.Left != null )
                    iter = iter.Left;
            }
            return iter;
        }

        /// <summary>
        /// Inserts a new node into the tree.
        /// </summary>
        /// <param name="value">The value to insert</param>
        /// <returns>The node that was inserted</returns>
        public RBTreeNode<T> Insert ( T value ) {
            RBTreeNode<T>? iter = this;
            RBTreeNode<T>? parent = null;
            RBTreeNode<T> _ret = this;
            CompareResult _cr = CompareResult.AIsLargerB;

            while ( iter != null ) {
                parent = iter;

                _cr = m_cmp(value, iter.Value);

                if ( _cr == CompareResult.AIsLargerB ) {
                    iter = iter.Right;
                    continue;
                }

                if ( _cr == CompareResult.AIsSmallerB ) {
                    iter = iter.Left;
                    continue;
                }

                // Equal
                _ret = iter;
                break;
            }

            if ( _cr == CompareResult.Equal ) {
                // Node erzeugen
                var newNode = new RBTreeNode<T>(value, m_cmp);

                // Parent setzen
                newNode.m_pElemtents[0] = parent;

                // Kinder auf null
                newNode.m_pElemtents[ILEFT] = null;
                newNode.m_pElemtents[IRIGHT] = null;

                // Einhängen
                if ( parent != null ) {
                    _cr = m_cmp(value, parent.Value);

                    if ( _cr == CompareResult.AIsSmallerB )
                        parent.m_pElemtents[ILEFT] = newNode;
                    else
                        parent.m_pElemtents[IRIGHT] = newNode;
                }

                Rebalance(newNode);
                Validate(newNode);

                _ret = newNode;
            }


            return _ret;
        }

    
        /*
* 
AIsSmallerB	iter < v	Left
AIsLargerB	iter > v	Right
Equal	iter == v	Treffer
*/  
        /// <summary>
        /// Finds a node with the specified value in the tree.
        /// </summary>
        /// <param name="v">The value to find</param>
        /// <returns></returns>
        public RBTreeNode<T>? Find ( T v ) {
            RBTreeNode<T>? iter = this;
            RBTreeNode<T> ? _ret = null;

            CompareResult _cr = CompareResult.AIsLargerB;

            while ( iter != null ) {
                T? iter_key = iter.Value;

                _cr = m_cmp(iter_key, v);

                if ( _cr == CompareResult.AIsLargerB )
                    iter = iter.Right;
                else if ( _cr == CompareResult.AIsSmallerB )
                    iter = iter.Left;

                else {
                    _ret = iter;
                    break;
                }

            }
            return _ret;       // not found
        }

        /// <summary>
        /// Finds the next node in the in-order traversal.
        /// </summary>
        /// <param name="n">The node to find the next of</param>
        /// <returns>The next node, or null if none exists</returns>
        public RBTreeNode<T>? FindNext ( RBTreeNode<T>? n ) {
            if ( n == null ) return null;

            RBTreeNode<T>? next = null;
            RBTreeNode<T>? _ret = null;

            // Fall 1: rechter Teilbaum → Minimum rechts
            if ( n.Right != null ) {
                next = n.Right;
                while ( next.Left != null )
                    next = next.Left;

                _ret = next;
            } else if ( (n.Parent != null) ) { // Fall 2: kein rechter Teilbaum → Parent-Logik

                // Wenn wir linkes Kind sind → Parent ist der nächste
                if ( n == n.Parent!.Left )
                    _ret = n.Parent;
                else {
                    // Sonst hochlaufen, bis wir aus einem linken Kind kommen
                    next = n;

                    while ( next.Parent != null ) {
                        if ( next == next.Parent!.Right ) {
                            next = next.Parent;
                        } else {
                            _ret = next.Parent;
                            break;
                        }
                    }
                }
            }
            return _ret;
        }

        /// <summary>
        /// Removes a node with the specified value from the tree.
        /// </summary>
        /// <param name="v">The value to remove</param>
        /// <returns>true if the node was found and removed, false otherwise</returns>
        public bool Erase ( T v ) {
            RBTreeNode<T>? _toErase = Find(v);

            return Erase(_toErase);
        }

        /// <summary>
        /// Removes a node from the tree.
        /// </summary>
        /// <param name="node">The node to remove</param>
        /// <returns>true if the node was found and removed, false otherwise</returns>

        public bool Erase ( RBTreeNode<T>? node ) {
            if ( node == null ) return false;

            //  Bestimme den tatsächlichen Knoten, der entfernt wird
            RBTreeNode<T>? toErase;

            if ( node.Left == null || node.Right == null ) {
                // Node hat höchstens ein Kind → direkt entfernen
                toErase = node;
            } else {
                // Node hat zwei Kinder → successor suchen (Minimum im rechten Teilbaum)
                toErase = node.Right;
                while ( toErase!.Left != null )
                    toErase = toErase.Left;
            }

            // Bestimme das Kind des zu entfernenden Knotens
            RBTreeNode<T>? child = (toErase.Left != null ? toErase.Left : toErase.Right);

            // Parent-Verknüpfung aktualisieren
            if ( child != null )
                child.Parent = toErase.Parent;

            // toErase aus dem Baum entfernen
            if ( toErase.Parent != null ) {
                if ( toErase == toErase.Parent.Left )
                    toErase.Parent.Left = child;
                else
                    toErase.Parent.Right = child;
            } else {
                // toErase war die Wurzel
                Parent = child;
            }

            //Wert übertragen (nur wenn node != toErase)
            if ( toErase != node )
                node.Value = toErase.Value;


            if ( toErase.Color == TreeColor.Black )
                RebalanceErase(toErase);
            // Node freigeben
            FreeNode(toErase, false);

            return true;
        }
       

       /// <summary>
       /// Clears the tree, removing all nodes and freeing their resources.
       /// </summary>
       /// <param name="depth">The maximum depth to traverse when freeing nodes.</param>
        public void Clear (int depth = 10) {
            if ( !IsEmpty ) {
                FreeNode(Parent, true, depth);
                Parent = null;
            }
        }

        /// <summary>
        /// Swaps the contents of this tree with another tree.
        /// </summary>
        /// <param name="other">The other tree to swap with</param>
        public void Swap ( RBTreeNode<T> other ) {
            if ( other == this )
                return;

            RBTreeNode<T> tmp = new RBTreeNode<T>(this);

            // this = other
            Parent = other.Parent;
            Left = other.Left;
            Right = other.Right;
            Value = other.Value;

            // other = tmp
            other.Parent = tmp.Parent;
            other.Left = tmp.Left;
            other.Right = tmp.Right;
            other.Value = tmp.Value;
     
        }


        /// <summary>
        /// Performs a left rotation on the given node.
        /// </summary>
        /// <param name="n">The node to rotate.</param>
        /// <returns>The new root of the rotated subtree.</returns>
        public RBTreeNode<T> RotateLeft ( RBTreeNode<T> n ) {

            RBTreeNode<T>? rightChild = n.Right;
            RBTreeNode<T>? _ret = n;

            if ( rightChild != null ) {
                // Right child's left child wird n's right child
                n.Right = rightChild.Left;
                if ( n.Right != null )
                    n.Right.Parent = n;

                // rightChild ersetzt n
                rightChild.Parent = n.Parent;

                if ( n.Parent == null ) {
                    // n war die Wurzel → rightChild wird neue Wurzel
                    Parent = rightChild;
                } else {
                    if ( n == n.Parent.Left )
                        n.Parent.Left = rightChild;
                    else
                        n.Parent.Right = rightChild;
                }

                //  n wird left child von rightChild
                rightChild.Left = n;
                n.Parent = rightChild;

                _ret = rightChild;
            }
            return _ret;
        }

        /// <summary>
        /// Performs a right rotation on the given node.
        /// </summary>
        /// <param name="n">The node to rotate.</param>
        /// <returns>The new root of the rotated subtree.</returns>
        public RBTreeNode<T> RotateRight ( RBTreeNode<T> n ) {
            RBTreeNode<T>? leftChild = n.Left;
            RBTreeNode<T> ret = n;

            if ( leftChild != null ) {

                // Left child's right child wird n's left child
                n.Left = leftChild.Right;
                if ( n.Left != null )
                    n.Left.Parent = n;

                // leftChild ersetzt n
                leftChild.Parent = n.Parent;

                if ( n.Parent == null ) {
                    // n war die Wurzel → leftChild wird neue Wurzel
                    Parent = leftChild;
                } else {
                    if ( n == n.Parent.Left )
                        n.Parent.Left = leftChild;
                    else
                        n.Parent.Right = leftChild;
                }

                // n wird right child von leftChild
                leftChild.Right = n;
                n.Parent = leftChild;

                ret = leftChild;
            }
            return ret;
        }

        /// <summary>
        /// Frees the resources associated with a node.
        /// </summary>
        /// <param name="n">The node to free.</param>
        /// <param name="recursive">Indicates whether to free the node's children recursively.</param>
        /// <param name="depth">The maximum depth to traverse when freeing nodes.</param>
        public void FreeNode ( RBTreeNode<T> n, bool recursive, int depth = 10 ) {
            if ( recursive ) {
                if ( depth == 0 ) return;

                if ( n.Left != null ) FreeNode(n.Left, true, (depth - 1));
                if ( n.Right != null ) FreeNode(n.Right, true, (depth - 1));
            }
            if ( n != null ) {
                n.Left = null;
                n.Right = null;
                n.Parent = null;
                n.Value = default;
            }
        }

        /// <summary>
        /// A function that can be called for each node during traversal.
        /// </summary>
        /// <param name="n">The node to traverse.</param>
        /// <param name="left">Indicates whether the node is a left child.</param>
        /// <param name="depth">The current depth in the tree.</param>
        /// <returns>return false for stopping traversal.</returns>
        public delegate bool TraverseFunc ( RBTreeNode<T> n, bool left, long depth );

        /// <summary>
        /// Traverses the tree starting from the given node.
        /// </summary>
        /// <param name="n">The node to start traversal from.</param>
        /// <param name="func">The function to call for each node.</param>
        /// <param name="depth">The current depth in the tree.</param>
        /// <param name="k">The node that caused traversal to stop - For traversal with multiple tasks. </param>
        /// <returns>false when user stops traversal.</returns>
        public bool Traverse ( RBTreeNode<T> n, TraverseFunc func, long depth, ref RBTreeNode<T> k ) {
            bool left = false;

            if ( n.Parent != null ) {
                left = n.Parent.Left == n;
            }
            if ( func(n, left, depth) == false ) {
                k = n;
                return false;
            }

            if ( n.Left != null ) {
                if ( !Traverse(n.Left, func, depth + 1, ref k) ) return false;
            }
            if ( n.Right != null ) {
                if ( !Traverse(n.Right, func, depth + 1, ref k) ) return false;
            }

            return true;
        }

        /// <summary>
        /// Traverses the tree using the provided function.
        /// </summary>
        /// <param name="func">The function to call for each node.</param>
        /// <returns>The node that caused traversal to stop - For traversal with multiple tasks. </returns>
        public RBTreeNode<T> Traverse ( TraverseFunc func ) {
            RBTreeNode<T> n = new RBTreeNode<T>();
            Traverse(this, func, 0, ref n);
            return n;
        }

        /// <summary>
        /// Get The count of nodes in the tree.
        /// </summary>
        /// <returns>The number of nodes in the tree.</returns>
        private long get_count () {
            long c = 0;

            var stack = new System.Collections.Generic.Stack<RBTreeNode<T>>();
            stack.Push(this);

            while ( stack.Count > 0 ) {
                RBTreeNode<T> n = stack.Pop();
                if ( n == null )
                    continue;

                c++;

                if ( n.Left != null )
                    stack.Push(n.Left);

                if ( n.Right != null )
                    stack.Push(n.Right);
            }

            return c;
        }
        /// <summary>
        /// Validates the red-black tree properties.
        /// </summary>
        void validate () {
            if(Parent.Color != TreeColor.Black);
                Validate(Parent);
        }
        /// <summary>
        /// Validates the red-black tree properties for a given subtree.
        /// </summary>
        /// <param name="root">The root of the subtree to validate.</param>
        /// <returns>True if the subtree is a valid red-black tree, false otherwise.</returns>
        public bool Validate ( RBTreeNode<T> root ) {
            if ( root == null )
                return true;

            var stack = new Stack<RBTreeNode<T>>();
            stack.Push(root);

            while ( stack.Count > 0 ) {
                var n = stack.Pop();

                // --- Regel 1: Parent-Beziehung korrekt ---
                if ( n.Parent != null ) {
                    bool isChild =
                n.Parent.Left == n ||
                n.Parent.Right == n;

                    if ( !isChild )
                        return false;
                }

                // --- Regel 2: Rote Knoten haben schwarze Kinder ---
                if ( n.Color == TreeColor.Red ) {
                    if ( n.Left != null && n.Left.Color != TreeColor.Black )
                        return false;

                    if ( n.Right != null && n.Right.Color != TreeColor.Black )
                        return false;
                }

                // --- Iterative Traversal statt Rekursion ---
                if ( n.Left != null )
                    stack.Push(n.Left);

                if ( n.Right != null )
                    stack.Push(n.Right);
            }

            return true;
        }
        /// <summary>
        /// Rebalances the tree after an insertion.
        /// </summary>
        /// <param name="newNode">The node that was inserted.</param>
        /// <returns>True if the tree is valid, false otherwise.</returns>
        private bool Rebalance ( RBTreeNode<T> newNode ) {
            var iter = newNode;

            // C++: while (iter->parent->color == red)
            // C#: Parent kann null sein → also checken
            while ( iter.Parent != null && iter.Parent.Color == TreeColor.Red ) {
                RBTreeNode<T> parent = iter.Parent;
                var grandparent = parent.Parent;

                // C++: grandparent wird IMMER benutzt → also muss er existieren
                if ( grandparent == null )
                    break;

                if ( parent == grandparent.Left ) {
                    var uncle = grandparent.Right;

                    if ( uncle != null && uncle.Color == TreeColor.Red ) {
                        parent.Color = TreeColor.Black;
                        uncle.Color = TreeColor.Black;
                        grandparent.Color = TreeColor.Red;
                        iter = grandparent;
                    } else {
                        if ( iter == parent.Right ) {
                            iter = parent;
                            RotateLeft(iter);
                        }

                        grandparent = iter.Parent?.Parent;
                        iter.Parent!.Color = TreeColor.Black;
                        grandparent!.Color = TreeColor.Red;
                        RotateRight(grandparent);
                    }
                } else {
                    var uncle = grandparent.Left;

                    if ( uncle != null && uncle.Color == TreeColor.Red ) {
                        grandparent.Color = TreeColor.Red;
                        parent.Color = TreeColor.Black;
                        uncle.Color = TreeColor.Black;
                        iter = grandparent;
                    } else {
                        if ( iter == parent.Left ) {
                            iter = parent;
                            RotateRight(iter);
                        }

                        grandparent = iter.Parent?.Parent;
                        iter.Parent!.Color = TreeColor.Black;
                        grandparent!.Color = TreeColor.Red;
                        RotateLeft(grandparent);
                    }
                }
            }

            if ( Parent != null )
                Parent.Color = TreeColor.Black;

            return true;
        }

        /// <summary>
        /// Rearranges the tree after a deletion to maintain red-black properties.
        /// </summary>
        /// <param name="n">The node that was deleted.</param>
        private void RebalanceErase ( RBTreeNode<T> n ) {
            RBTreeNode<T> iter = n;

            while ( iter != Parent && iter.Color == TreeColor.Black ) {
                if ( iter.Parent == null )
                    break;

                bool isLeftChild = (iter.Parent.Left == iter);

                // Sibling wird berechnet, NICHT als Feld
                RBTreeNode<T>? sibling = isLeftChild
                    ? iter.Parent.Right
                    : iter.Parent.Left;

                // --- Fall Sibling ist rot ---
                if ( sibling != null && sibling.Color == TreeColor.Red ) {
                    sibling.Color = TreeColor.Black;
                    iter.Parent.Color = TreeColor.Red;

                    if ( isLeftChild )
                        RotateLeft(iter.Parent);
                    else
                        RotateRight(iter.Parent);

                    sibling = isLeftChild
                        ? iter.Parent.Right
                        : iter.Parent.Left;
                }

                // Kinderfarben absichern
                TreeColor sLeft  = sibling?.Left?.Color  ?? TreeColor.Black;
                TreeColor sRight = sibling?.Right?.Color ?? TreeColor.Black;

                // --- Fall  beide Kinder schwarz ---
                if ( sLeft == TreeColor.Black && sRight == TreeColor.Black ) {
                    if ( sibling != null )
                        sibling.Color = TreeColor.Red;

                    iter = iter.Parent;
                    continue;
                }

                // ---  inneres Kind schwarz ---
                if ( isLeftChild ) {
                    if ( sRight == TreeColor.Black ) {
                        if ( sibling?.Left != null )
                            sibling.Left.Color = TreeColor.Black;

                        if ( sibling != null ) {
                            sibling.Color = TreeColor.Red;
                            RotateRight(sibling);
                        }

                        sibling = iter.Parent.Right;
                        sRight = sibling?.Right?.Color ?? TreeColor.Black;
                    }
                } else {
                    if ( sLeft == TreeColor.Black ) {
                        if ( sibling?.Right != null )
                            sibling.Right.Color = TreeColor.Black;

                        if ( sibling != null ) {
                            sibling.Color = TreeColor.Red;
                            RotateLeft(sibling);
                        }

                        sibling = iter.Parent.Left;
                        sLeft = sibling?.Left?.Color ?? TreeColor.Black;
                    }
                }

                // --- äußeres Kind rot ---
                if ( sibling != null ) {
                    sibling.Color = iter.Parent.Color;
                }

                iter.Parent.Color = TreeColor.Black;

                if ( isLeftChild ) {
                    if ( sibling?.Right != null )
                        sibling.Right.Color = TreeColor.Black;

                    RotateLeft(iter.Parent);
                } else {
                    if ( sibling?.Left != null )
                        sibling.Left.Color = TreeColor.Black;

                    RotateRight(iter.Parent);
                }

                iter = Parent!; // Parent
            }

            iter.Color = TreeColor.Black;
        }
    }
    /// @}
}
