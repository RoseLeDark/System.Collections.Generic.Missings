using System;
using System.Collections.Generic;
using System.Text;
using SystemEx.Algorithms;
using SystemEx.Algorithms.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SystemEx.Collections.Model {
	public class BinaryTree<T>  : Tree<T> 
		where T : IComparable<T> {


		private ISimpleCompare<T> m_compare;

		public BinaryTree ( T value ) : base(value) {
			m_compare = new Less<T>();
		}

		public BinaryTree ( T value, ISimpleCompare<T> cmp ) : base(value) { 
			m_compare = cmp;
		}
		 public void ResetParent() {
			while(m_rootNode.Parent != null)
			{
				m_rootNode = m_rootNode.Parent;
			}
		 }

		public void Insert(T newData) {
			var _c = new TreeNode<T>(2, newData);

			m_rootNode = InsertTree(m_rootNode, _c, m_compare);
		}

		private TreeNode<T> InsertTree ( TreeNode<T>? root, TreeNode<T> newnode, ISimpleCompare<T> cmp ) {

			if ( root == null ) {
				root = newnode;
			} else {
				TreeNode<T>? prev = null;
				TreeNode<T>? curr = root;

				while ( curr != null ) {
					prev = curr;

					var _comparer = cmp.Compare(curr.Data.Value, newnode.Data.Value);

					curr = (_comparer) ? curr.Right : curr.Left;

				}

				var _cmp = cmp.Compare(prev!.Data.Value, newnode.Data.Value);

				if ( _cmp ) prev.Right = newnode;
				else prev.Left = newnode;

			}
			return root;
		}

	}
}
