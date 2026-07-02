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

    
    public class BinaryTree<T> : Tree<T, BinaryTree<T>> {
        const int ILEFT = 2;
        const int IRIGHT = 1;
        const int IPARENT = 0;

        private CompFunc<T> m_cmp;

        public CompFunc<T> CompareFunc {
            protected set => m_cmp = value;
            get => m_cmp;
        }

        internal BinaryTree<T>? Parent { get => m_pElemtents[IPARENT]; set => m_pElemtents[IPARENT] = value; }
        public BinaryTree<T>? Left { get => m_pElemtents[ILEFT]; internal set => m_pElemtents[ILEFT] = value; }

        public BinaryTree<T>? Right { get => m_pElemtents[IRIGHT]; internal set => m_pElemtents[IRIGHT] = value; }

        public bool IsEmpty => Value == null;

        public long Count => get_count();

        public bool IsLeaf => Left == null && Right == null;

        private BinaryTree() : base(2, default(T) ) { }

        public BinaryTree ( T value, CompFunc<T> cmp )
            : base(2, value) {
            m_pElemtents[IPARENT] = m_pElemtents[IRIGHT] = m_pElemtents[ILEFT] = null;
            m_cmp = cmp;
        }

        public BinaryTree ( BinaryTree<T> binaryTree ) : base(2, binaryTree.Value)  {
            Parent = binaryTree.Parent;
            Left = binaryTree.Left;
            Right = binaryTree.Right;
            Value = binaryTree.Value;
        }

        public BinaryTree<T> Beginn () {
            BinaryTree<T>? iter = null;

            if ( Parent != null ) {
                iter = Parent;

                while ( iter.Left != null )
                    iter = iter.Left;
            }
            return iter;
        }

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


        public bool Erase ( T v ) {
            BinaryTree<T>? _toErase = Find(v);

            return Erase(_toErase);
        }
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

        public void Clear () {
            if ( !IsEmpty ) {
                FreeNode(Parent, true);
                Parent = null;
            }
        }

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

        public delegate bool TraverseFunc( BinaryTree<T> n, bool left, long depth ); 
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

        public BinaryTree<T> Traverse ( TraverseFunc func ) {
            BinaryTree<T> n = new BinaryTree<T>();
            Traverse(this, func, 0, ref n);
            return n;
        }

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
}
