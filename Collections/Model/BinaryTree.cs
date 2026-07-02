/* 
 * SPDX-License-Identifier: EUPL-1.2
 *
 * Copyright (c) 2026 Amber-Sophia Schröck <ambersophia.schroeck@mail.de>
 *
 * This file is licensed under the European Union Public Licence (EUPL) version 1.2.
 * You can obtain a copy of the licence at:
 *   https://joinup.ec.europa.eu/collection/eupl/eupl-text-eupl-12
 *
 * Unless required by applicable law or agreed to in writing, software distributed
 * under the Licence is distributed on an "AS IS" basis, WITHOUT WARRANTIES OR
 * CONDITIONS OF ANY KIND, either express or implied.
 *
 * If you modify this file, retain this notice and add a short description of your
 * changes and the date.
 */

using SystemEx.Utils;

namespace SystemEx.Collections.Model {
    /// \addtogroup model
    /// @{
    /// <summary>
    /// Represents a binary tree node.
    /// </summary>
    /// <typeparam name="T">The type of the value stored in the tree node.</typeparam>
    public class BinaryTree<T> : Tree<T, BinaryTree<T>> {
        const int ILEFT = 2;
        const int IRIGHT = 1;
        const int IPARENT = 0;

        private CompFunc<T> m_cmp;

        /// <summary>
        /// Gets or sets the comparison function for the binary tree.
        /// </summary>
        /// <value></value>
        public CompFunc<T> CompareFunc {
            protected set => m_cmp = value;
            get => m_cmp;
        }
        /// <summary>
        /// Gets or sets the parent node of the binary tree node.
        /// </summary>
        internal BinaryTree<T>? Parent { get => m_pElemtents[IPARENT]; set => m_pElemtents[IPARENT] = value; }

        /// <summary>
        /// Gets or sets the left child node of the binary tree node.
        /// </summary>
        public BinaryTree<T>? Left { get => m_pElemtents[ILEFT]; internal set => m_pElemtents[ILEFT] = value; }

        /// <summary>
        /// Gets or sets the right child node of the binary tree node.
        /// </summary>
        public BinaryTree<T>? Right { get => m_pElemtents[IRIGHT]; internal set => m_pElemtents[IRIGHT] = value; }

        /// <summary>
        /// Gets a value indicating whether the binary tree node is empty.
        /// </summary>
        public bool IsEmpty => Value == null;

        /// <summary>
        /// Gets the number of nodes in the binary tree.
        /// </summary>
        public long Count => get_count();
        /// <summary>
        /// Gets a value indicating whether the binary tree node is a leaf.
        /// </summary>
        public bool IsLeaf => Left == null && Right == null;
        /// <summary>
        /// Initializes a new instance of the BinaryTree class.
        /// </summary>
        /// <returns></returns>
        private BinaryTree() : base(2, default(T) ) { }
        /// <summary>
        /// Initializes a new instance of the BinaryTree class.
        /// </summary>
        /// <param name="value">The value to store in the tree node.</param>
        /// <param name="cmp">The comparison function for the binary tree.</param>
        public BinaryTree ( T value, CompFunc<T> cmp )
            : base(2, value) {
            m_pElemtents[IPARENT] = m_pElemtents[IRIGHT] = m_pElemtents[ILEFT] = null;
            m_cmp = cmp;
        }
        /// <summary>
        /// Initializes a new instance of the BinaryTree class.
        /// </summary>
        /// <param name="binaryTree">The binary tree node to copy.</param>
        public BinaryTree ( BinaryTree<T> binaryTree ) : base(2, binaryTree.Value)  {
            Parent = binaryTree.Parent;
            Left = binaryTree.Left;
            Right = binaryTree.Right;
            Value = binaryTree.Value;
        }

        /// <summary>
        /// Gets the first node in the binary tree.
        /// </summary>
        /// <returns>The first node in the binary tree.</returns>
        public BinaryTree<T> Beginn () {
            BinaryTree<T>? iter = null;

            if ( Parent != null ) {
                iter = Parent;

                while ( iter.Left != null )
                    iter = iter.Left;
            }
            return iter;
        }
        /// <summary>
        /// Inserts a new node into the binary tree.
        /// </summary>
        /// <param name="value">The value to insert.</param>
        /// <returns>The node that was inserted or found.</returns>
        public BinaryTree<T> Insert ( T value ) {
            BinaryTree<T>? iter = this;
            BinaryTree<T>? parent = null;
            BinaryTree<T> _ret = iter;
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
                var newNode = new BinaryTree<T>(value, m_cmp);

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

                _ret = newNode;
            }

            
            return _ret!;
        }

        /*
* 
AIsSmallerB	iter < v	Left
AIsLargerB	iter > v	Right
Equal	iter == v	Treffer
*/
        /// <summary>
        /// Finds a node in the binary tree.
        /// </summary>
        /// <param name="v">The value to find.</param>
        /// <returns>The node that was found or null if not found.</returns>
        public BinaryTree<T>? Find ( T v ) {
            BinaryTree<T>? iter = this;
            BinaryTree<T> ? _ret = null;

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
        /// Finds the next node in the binary tree.
        /// </summary>
        /// <param name="n">The node for which to find the next.</param>
        /// <returns>The next node or null if not found.</returns>
        public BinaryTree<T>? FindNext ( BinaryTree<T>? n ) {
            if ( n == null ) return null;

            BinaryTree<T>? next = null;
            BinaryTree<T>? _ret = null;

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

            // Fall 3: n ist Root und hat keinen rechten Teilbaum
            // → Root hat keinen Next
            return _ret;
        }

        /// <summary>
        /// Removes a node from the binary tree.
        /// </summary>
        /// <param name="v">The value to remove.</param>
        /// <returns>true if the node was found and removed, false otherwise.</returns>
        public bool Erase ( T v ) {
            BinaryTree<T>? _toErase = Find(v);

            return Erase(_toErase);
        }

        /// <summary>
        /// Removes a node from the binary tree.
        /// </summary>
        /// <param name="node">The node to remove.</param>
        /// <returns>true if the node was found and removed, false otherwise.</returns>
        public bool Erase ( BinaryTree<T>? node ) {
            if ( node == null ) return false;

            //  Bestimme den tatsächlichen Knoten, der entfernt wird
            BinaryTree<T>? toErase;

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
            BinaryTree<T>? child = (toErase.Left != null ? toErase.Left : toErase.Right);

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


            // Node freigeben
            FreeNode(toErase, false);

            return true;
        }
        /// <summary>
        /// Clears the binary tree.
        /// </summary>
        public void Clear () {
            if ( !IsEmpty ) {
                FreeNode(Parent, true);
                Parent = null;
            }
        }
        /// <summary>
        /// Swaps the contents of this binary tree with another.
        /// </summary>
        /// <param name="other">The other binary tree.</param>
        public void Swap ( BinaryTree<T> other ) {
            if ( other == this )
                return;

            BinaryTree<T> tmp = new BinaryTree<T>(this);
            
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
        /// Rotates the binary tree to the left around the given node.
        /// </summary>
        /// <param name="n">The node to rotate around.</param>
        /// <returns>The new root of the rotated subtree.</returns>
        public BinaryTree<T> RotateLeft ( BinaryTree<T> n ) {

            BinaryTree<T>? rightChild = n.Right;
            BinaryTree<T>? _ret = n;

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
        /// Rotates the binary tree to the right around the given node.
        /// </summary>
        /// <param name="n">The node to rotate around.</param>
        /// <returns>The new root of the rotated subtree.</returns>
        public BinaryTree<T> RotateRight ( BinaryTree<T> n ) {
            BinaryTree<T>? leftChild = n.Left;
            BinaryTree<T> ret = n;

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
        /// Frees the memory allocated for a node in the binary tree.
        /// </summary>
        /// <param name="n">The node to free.</param>
        /// <param name="recursive">Indicates whether to free the node's children recursively.</param>
        /// <param name="depth">The maximum depth to traverse when freeing children.</param>
        public void FreeNode ( BinaryTree<T> n, bool recursive, int depth = 10 ) {
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
        /// A delegate for the function used to traverse the binary tree.
        /// </summary>
        /// <param name="n">The current node.</param>
        /// <param name="left">Indicates whether the current node is a left child.</param>
        /// <param name="depth">The depth of the current node.</param>
        /// <returns>True to continue traversal, false to stop.</returns>
        public delegate bool TraverseFunc( BinaryTree<T> n, bool left, long depth ); 

        /// <summary>
        /// Traverses the binary tree using the specified function.
        /// </summary>
        /// <param name="n">The current node.</param>
        /// <param name="func">The function to call for each node.</param>
        /// <param name="depth">The depth of the current node.</param>
        /// <param name="k">The node that caused the traversal to stop.</param>
        /// <returns>True if the traversal completed successfully, false otherwise.</returns>
        public bool Traverse ( BinaryTree<T> n, TraverseFunc func, long depth, ref BinaryTree<T> k ) {
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
        /// Traverses the binary tree using the specified function.
        /// </summary>
        /// <param name="func">The function to call for each node.</param>
        /// <returns>The node that caused the traversal to stop, or the root node if the traversal completed successfully.</returns>
        public BinaryTree<T> Traverse ( TraverseFunc func ) {
            BinaryTree<T> n = new BinaryTree<T>();
            Traverse(this, func, 0, ref n);
            return n;
        }
        /// <summary>
        /// Gets the count of nodes in the binary tree.
        /// </summary>
        /// <returns>The number of nodes in the binary tree.</returns>
        private long get_count () {
            long c = 0;

            var stack = new System.Collections.Generic.Stack<BinaryTree<T>>();
            stack.Push(this);

            while ( stack.Count > 0 ) {
                BinaryTree<T> n = stack.Pop();
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
    }
    /// @}
}
