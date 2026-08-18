using System.ComponentModel;
using SystemEx.Utils;

namespace SystemEx.Collections.Model {
	public class TreeNode<T> : IParentebleNode<T, TreeNode<T> > , IComparableEx<TreeNode<T>> 
        where T : IComparableEx<T> {

        private TreeNode<T>?[] m_childs;
        private Optional<T> m_data;

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

		public CompareResult CompareTo ( TreeNode<T> a ) {
            return Data.CompareTo(a.Data);
		}
	}
}
