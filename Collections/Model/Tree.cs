using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Xml.Linq;
using SystemEx.Collections.Generic;
using SystemEx.Drawing;
using SystemEx.Random;
using static System.Net.Mime.MediaTypeNames;

namespace SystemEx.Collections.Model {

    public class TreeNode<T> : IParentebleNode<T, TreeNode<T>, TreeNode<T> >  {
        private TreeNode<T>?[] m_childs;
        private Optional<T> m_data;

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal bool Visited { get; set; }

        public TreeNode<T>? Parent { 
            get => m_childs[0]; 
            set => m_childs[0] = value; 
        }

        public TreeNode<T>? Left {
            get => m_childs[1];
            set => m_childs[1] = value;
        }

        public TreeNode<T>? Right {
            get => m_childs[2];
            set => m_childs[2] = value;
        }

        public int Count => m_childs.Length;

        public Optional<T> Data => m_data;

        public bool IsParent => GetChild(0) != null;

        public bool IsLeft  => GetChild(1) != null;
        public bool IsRight => GetChild(2) != null;

        public TreeNode(int numChilds) {
            m_childs = new TreeNode<T>?[numChilds+1];
            m_data = new Optional<T>();
        }
        public TreeNode ( int numChilds, T data ) {
            m_childs = new TreeNode<T>?[numChilds+1];
            m_data = new Optional<T>(data);
        }

        public TreeNode<T>? GetChild ( uint index ) {
            if ( index >= m_childs.Length ) throw new IndexOutOfRangeException(nameof(index));

            return m_childs[index];
        }

        

        public static void RotateLeft ( ref TreeNode<T> node, out TreeNode<T> newRoot ) {

            TreeNode<T>? y = node.Right;
            if ( y is null ) {
                newRoot = node;
                return;
            }

            node.Right = y.Left;
            y.Left = node;

            newRoot = y;
        }
    }
    public class Tree<T> {
        protected TreeNode<T> m_rootNode;

        public Tree() {
            m_rootNode = new TreeNode<T>(2);
        }
        public Tree (T data) {
            m_rootNode = new TreeNode<T>(2, data);
        }

        public Optional<T> Data => m_rootNode.Data;

        public Optional<TreeNode<T>> Left => m_rootNode.Left;

		public Optional<TreeNode<T>> Right => m_rootNode.Right;

		public TreeNode<T> InsertLeft ( T value) {
            TreeNode<T>? current = m_rootNode;

			while ( current != null ) {
                

                if ( !current!.IsLeft ) {
                    TreeNode<T> _newLeft = new TreeNode<T>(2, value);
                    _newLeft.Parent = current;
                    current.Left = _newLeft;
                    break;
                }
                current = current.Left;
            }
			return current!;
		}
        public TreeNode<T> InsertRight ( T value ) {
            TreeNode<T>? current = m_rootNode;

            while ( current != null ) {

                if ( !current!.IsRight) {
                    TreeNode<T> _newReight = new TreeNode<T>(2, value);
                    _newReight.Parent = current;
                    current.Right = _newReight;
                    break;
                }
                current = current.Right;
            }
            return current!;
        }

        protected void RotateLeft ( ref TreeNode<T> x ) {
            TreeNode<T>? newRoot;

            // lokale Rotation aus TreeNode<T> benutzen
            TreeNode<T>.RotateLeft(ref x, out newRoot);

            // neuen Root in den Baum einhängen
            if ( !x.IsParent) {
                // x war die Wurzel
                m_rootNode = newRoot;
            } else {
                // x war linkes oder rechtes Kind
                if ( x.Parent!.Left == x )
                    x.Parent.Left = newRoot;
                else
                    x.Parent.Right = newRoot;

                newRoot.Parent = x.Parent;
            }

            // Parent von x aktualisieren
            x.Parent = newRoot;
        }

        public static void PostOrder ( Tree<T> tree ) {
            TreeNode<T> temp = tree.m_rootNode;

            while ( temp != null && temp.Visited == false ) {

                // Visited left subtree
                if ( temp.IsLeft && temp.Left!.Visited == false )
                    temp = temp.Left;

                // Visited right subtree
                else if ( temp.IsRight && temp.Right!.Visited == false )
                    temp = temp.Right;

                // Print node
                else {
                    Console.Write("{0} ", temp.Data.ToString());
                    temp.Visited = true;
                    temp = tree.m_rootNode;
                }
            }
        }
		public static void PreOrder ( Tree<T> tree ) {
			TreeNode<T>? current = tree.m_rootNode;

			while ( current != null ) {
				// Kein linkes Kind → Node ausgeben und nach rechts gehen
				if ( current.Left == null ) {
					Console.Write("{0} ", current.Data);
					current = current.Right;
				} else {
					// Predecessor suchen (rechtester Knoten im linken Teilbaum)
					TreeNode<T> predecessor = current.Left;
					while ( predecessor.Right != null && predecessor.Right != current )
						predecessor = predecessor.Right;

					// Thread noch nicht gesetzt → Node ausgeben, Thread setzen, links weiter
					if ( predecessor.Right == null ) {
						Console.Write("{0} ", current.Data);
						predecessor.Right = current;
						current = current.Left;
					} else {
						// Thread existiert → entfernen und rechts weiter
						predecessor.Right = null;
						current = current.Right;
					}
				}
			}
		}
	}
}
