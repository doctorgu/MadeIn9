using System;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;

namespace DoctorGu
{
	/// <summary>
	/// Summary description for TreeView.
	/// </summary>
	public class CTreeView
	{
		public CTreeView()
		{
		}

		private static bool IsFound = false;
		private static int i = 0;
		/// <summary>
		/// 지정한 노드 안에 있는 특정한 문자열을 찾음. IncludeSubNode가
		/// True이면 하위 노드도 찾게 됨.
		/// </summary>
		/// <param name="CurNode"></param>
		/// <param name="Text"></param>
		/// <param name="IsIncludeSubNode"></param>
		/// <returns></returns>
		/// <example>
		/// TreeNode n = CTreeView.FindFirstNode(t.SelectedNode, txtToFind.Text, true, true);
		/// t.SelectedNode = n;
		/// t.Focus();
		/// </example>
		public static TreeNode FindFirstNode(TreeNode CurNode, string Text, bool IgnoreCase, bool IncludeSubNode)
		{
			//재귀호출의 깊이를 증가시킴.
			i++;

			TreeNode NodeFound = null;

			while (CurNode != null)
			{
				if (string.Compare(CurNode.Text, Text, IgnoreCase) == 0)
				{
					IsFound = true;
					NodeFound = CurNode;
					break;
				}

				if (IncludeSubNode)
				{
					NodeFound = FindFirstSubNode(CurNode, Text, IgnoreCase);
					if (IsFound) break;
				}

				CurNode = CurNode.NextNode;
			}

			//재귀호출의 깊이를 감소시킴.
			i--;

			//i가 0이면 재귀호출의 상위 함수이며, 곧 마지막 호출이므로
			//다음 번 호출 때는 false로 찾기 시작할 수 있도록 함.
			if (i == 0)
				IsFound = false;
			
			return NodeFound;
		}
		private static TreeNode FindFirstSubNode(TreeNode CurNode, string Text, bool IgnoreCase)
		{
			foreach (TreeNode tn in CurNode.Nodes)
			{
				TreeNode NodeFound = FindFirstNode(tn, Text, IgnoreCase, true);
				if (IsFound)
					return NodeFound;
			}

			return null;
		}

		private static List<TreeNode> aNodeFound = new List<TreeNode>();
		public static List<TreeNode> FindAllNode(TreeNode CurNode, string Text, bool IgnoreCase, bool IncludeSubNode)
		{
			while (CurNode != null)
			{
				if (string.Compare(CurNode.Text, Text, IgnoreCase) == 0)
				{
					aNodeFound.Add(CurNode);
				}

				if (IncludeSubNode)
				{
					foreach (TreeNode tn in CurNode.Nodes)
					{
						if (string.Compare(tn.Text, Text, IgnoreCase) == 0)
						{
							aNodeFound.Add(tn);
						}
					}
				}

				CurNode = CurNode.NextNode;
			}

			if (aNodeFound.Count == 0) return null;

			return aNodeFound;
		}
		
		public static bool RemoveAllNodeHasText(TreeNode CurNode, string Text, bool IgnoreCase, bool IncludeSubNode)
		{
			List<TreeNode> aNode = FindAllNode(CurNode, Text, IgnoreCase, IncludeSubNode);
			if (aNode == null) return false;

			for (int i = 0, j = aNode.Count; i < j; i++)
			{
				try
				{
					TreeNode n = (TreeNode)aNode[i];
					n.Remove();
				}
				catch (Exception)
				{
					//
				}
			}

			return true;
		}

		/// <summary>
		/// NodeRoot의 모든 하위노드를 삭제함.
		/// </summary>
		/// <param name="NodeRoot"></param>
		/// <param name="ExceptOne">하위노드 중 하나만을 남길 지 여부</param>
		/// <example>
		/// CTreeView.RemoveAllSubNode(t.SelectedNode);
		/// </example>
		public static void RemoveAllSubNode(TreeNode NodeRoot, bool ExceptOne)
		{
			int nRemain = Convert.ToInt32(ExceptOne);
			while (NodeRoot.Nodes.Count > nRemain)
			{
				NodeRoot.Nodes.Remove(NodeRoot.Nodes[nRemain]);
			}
		}
		public static void RemoveAllSubNode(TreeNode NodeRoot)
		{
			RemoveAllSubNode(NodeRoot, false);
		}

		/// <summary>
		/// CurNode가 현재 레벨에서 몇번째에 있는 지 알아냄.
		/// </summary>
		/// <param name="tv"></param>
		/// <param name="CurNode"></param>
		/// <returns></returns>
		/// <example>
		/// MessageBox.Show(DrTreeView.GetnTHOfCurNodeLevel(t, t.SelectedNode).ToString());
		/// </example>
		public static int GetnTHOfCurNodeLevel(TreeView tv, TreeNode CurNode)
		{
			if (CurNode == null) return 0;

			TreeNode ParentNode = CurNode.Parent;

			//Root가 여러 개인 경우
			if (ParentNode == null)
			{
				int i = -1;
				TreeNode TempNode = tv.Nodes[0];
				while (TempNode != null)
				{
					i++;

					if (TempNode == CurNode)
					{
						return i;
					}

					TempNode = TempNode.NextNode;
				}
			}
			else
			{
				for (int i = 0, j = ParentNode.Nodes.Count; i < j; i++)
				{
					if (ParentNode.Nodes[i] == CurNode)
					{
						return i;
					}
				}
			}

			return 0;
		}

		/// <summary>
		/// CurNode의 현재 레벨에서 nTH번째의 노드를 선택함.
		/// </summary>
		/// <param name="tv"></param>
		/// <param name="CurNode"></param>
		/// <param name="nTH"></param>
		/// <param name="SelectLastIfNotFound">nTH번째의 노드를 못 찾았을 경우엔 현재 레벨의 마지막 노드를 선택함.</param>
		/// <returns></returns>
		/// <example>
		/// CTreeView.SetSelectedInCurNode(t, t.SelectedNode, int.Parse(txtnTH.Text), true);
		/// t.Focus();
		/// </example>
		public static TreeNode SetSelectedInCurNode(TreeView tv, TreeNode CurNode, int nTH, 
			bool SelectLastIfNotFound)
		{
			if (CurNode == null) return null;

			TreeNode NodeToSelect = null;
			TreeNode ParentNode = CurNode.Parent;

			if (ParentNode == null)
			{
				TreeNode TempNode = tv.Nodes[0];
				int i = -1;
				while (TempNode != null)
				{
					i++;

					if (i == nTH)
					{
						NodeToSelect = TempNode;
						break;
					}
				}
				if ((SelectLastIfNotFound) && (NodeToSelect == null))
				{
					NodeToSelect = tv.Nodes[i];
				}
			}
			else
			{
				for (int i = 0, j = ParentNode.Nodes.Count; i < j; i++)
				{
					if (i == nTH)
					{
						NodeToSelect = ParentNode.Nodes[i];
						break;
					}
				}
				if ((SelectLastIfNotFound) && (NodeToSelect == null))
				{
					NodeToSelect = ParentNode.Nodes[ParentNode.Nodes.Count - 1];
				}
			}

			tv.SelectedNode = NodeToSelect;

			return NodeToSelect;
		}

		/// <summary>
		/// TreeView의 Node가 몇번째에 위치했는 지를 리턴함.
		/// </summary>
		/// <param name="tv"></param>
		/// <param name="CurNode"></param>
		/// <returns></returns>
		/// <example>
		/// this.Text = CTreeView.GetNodeLevel(t, t.SelectedNode).ToString();
		/// </example>
		public static int GetNodeLevel(TreeNode CurNode)
		{
			if (CurNode == null) return -1;

			TreeNode ParentNode = CurNode;
			int i = -1;
			while (ParentNode != null)
			{
				i++;
				ParentNode = ParentNode.Parent;
			}

			return i;
		}

		/// <summary>
		/// 현재 노드를 삭제한 후 상위 노드가 하위노드를 가지고 있지 않다면 상위노드를 삭제함.
		/// 이런 과정을 더 이상 삭제할 노드가 없을 때까지 반복함.
		/// </summary>
		/// <param name="tv"></param>
		/// <param name="CurNode"></param>
		public static void RemoveCurrentNodeAndRemoveParentNodeIfNoChild(TreeNode CurNode)
		{
			TreeNode ParentNode = CurNode.Parent;

			while (ParentNode != null)
			{
				CurNode.Remove();

				if (ParentNode.Nodes.Count > 0)
				{
					return;
				}
				else
				{
					CurNode = ParentNode;
					ParentNode = CurNode.Parent;
				}
			}
		}

		public static void ReplaceNode(TreeNode Node1, TreeNode Node2)
		{
			string Text1 = Node1.Text;
			object Tag1 = Node1.Tag;

			Node1.Text = Node2.Text;
			Node1.Tag = Node2.Tag;

			Node2.Text = Text1;
			Node2.Tag = Tag1;
		}

		public static List<TreeNode> GetAllNode(TreeView Tvw)
		{
			List<TreeNode> aNode = new List<TreeNode>();

			foreach (TreeNode TNodeCur in Tvw.Nodes)
			{
				aNode.Add(TNodeCur);
				if (TNodeCur.Nodes.Count > 0)
				{
					GetAllChildNode(ref aNode, TNodeCur);
				}
			}

			return aNode;
		}
		public static List<TreeNode> GetChildNodes(TreeNode Parent)
		{
			List<TreeNode> aNode = new List<TreeNode>();
			GetAllChildNode(ref aNode, Parent);
			return aNode;
		}
		private static void GetAllChildNode(ref List<TreeNode> aNode, TreeNode TNodeParent)
		{
			foreach (TreeNode TNodeChild in TNodeParent.Nodes)
			{
				aNode.Add(TNodeChild);
				if (TNodeChild.Nodes.Count > 0)
				{
					GetAllChildNode(ref aNode, TNodeChild);
				}
			}
		}

		public static TreeNode MoveNode(TreeView Tvw, TreeNode NodeFrom, TreeNode NodeTo,
			bool IsMoveAsChild, out string ErrMsgIs)
		{
			ErrMsgIs = "";

			TreeNode NodeNew = null;
			try
			{
				NodeNew = (TreeNode)NodeFrom.Clone();

				if (IsMoveAsChild)
				{
					NodeTo.Nodes.Add(NodeNew);
				}
				else
				{
					TreeNode NodeToParent = NodeTo.Parent;
					if (NodeToParent == null)
					{
						ErrMsgIs = "최상위로 이동할 수 없습니다.";
						return null;
					}

					NodeToParent.Nodes.Insert(NodeTo.Index, NodeNew);
				}

				NodeFrom.Remove();

				Tvw.SelectedNode = NodeNew;
			}
			catch (Exception ex)
			{
				ErrMsgIs = ex.Message + "\r\n" + ex.StackTrace;
				return null;
			}

			return NodeNew;
		}
	}
}
