using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using SystemEx.Utils;

namespace SystemEx.Collections.Model {
    public enum TreeColor {
        Red,
        Black
    }

    public class RBTreeNode<T> : Tree<T, RBTreeNode<T>> {
        const int ILEFT = 2;
        const int IRIGHT = 1;
        const int IPARENT = 0;

        private CompFunc<T> m_cmp;

        public bool AutoRebalance { get; set; }

        public CompFunc<T> CompareFunc {
            protected set => m_cmp = value;
            get => m_cmp;
        }

        internal RBTreeNode<T>? Parent { get => m_pElemtents[IPARENT]; set => m_pElemtents[IPARENT] = value; }
        public RBTreeNode<T>? Left { get => m_pElemtents[ILEFT]; internal set => m_pElemtents[ILEFT] = value; }

        public RBTreeNode<T>? Right { get => m_pElemtents[IRIGHT]; internal set => m_pElemtents[IRIGHT] = value; }

        public bool IsEmpty => Value == null;

        public long Count => get_count();

        public bool IsLeaf => Left == null && Right == null;

        public TreeColor Color { get; protected set; }

        private RBTreeNode () : base(2, default(T)) { Color = TreeColor.Black;  }

        public RBTreeNode ( T value, CompFunc<T> cmp )
            : base(2, value) {
            m_pElemtents[IPARENT] = m_pElemtents[IRIGHT] = m_pElemtents[ILEFT] = null;
            m_cmp = cmp;
            Color = TreeColor.Black;
        }

        public RBTreeNode ( RBTreeNode<T> other ) : base(2, other.Value) {
            Parent = other.Parent;
            Left = other.Left;
            Right = other.Right;
            Value = other.Value;
            Color = other.Color;
        }

        public RBTreeNode<T>? Begin () {
            RBTreeNode<T>? iter = null;

            if ( Parent != null ) {
                iter = Parent;

                while ( iter.Left != null )
                    iter = iter.Left;
            }
            return iter;
        }

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

            // Fall 3: n ist Root und hat keinen rechten Teilbaum
            // → Root hat keinen Next
            return _ret;
        }


        public bool Erase ( T v ) {
            RBTreeNode<T>? _toErase = Find(v);

            return Erase(_toErase);
        }
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

        public void Clear () {
            if ( !IsEmpty ) {
                FreeNode(Parent, true);
                Parent = null;
            }
        }

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

        public delegate bool TraverseFunc ( RBTreeNode<T> n, bool left, long depth );
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

        public RBTreeNode<T> Traverse ( TraverseFunc func ) {
            RBTreeNode<T> n = new RBTreeNode<T>();
            Traverse(this, func, 0, ref n);
            return n;
        }

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
        void validate () {
            if(Parent.Color != TreeColor.Black);
                Validate(Parent);
        }

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
}
