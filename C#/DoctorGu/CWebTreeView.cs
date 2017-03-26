using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;

namespace DoctorGu
{
	public class CWebTreeView
	{
		public static void AddHierarchical(TreeView tvw, DataTable dt,
			string ColumnNameId, string ColumnNameParentId, string ColumnNameText,
			string ValueToSelect, EventHandler SelectedNodeChanged)
		{
			tvw.Nodes.Clear();

			if (dt.Rows.Count > 0)
			{
				dt = CDataTable.GetHierarchicalDataTable(dt, ColumnNameId, ColumnNameParentId, "R");

				int Seq = Convert.ToInt32(dt.Rows[0][ColumnNameId]);
				string Title = dt.Rows[0][ColumnNameText].ToString();

				TreeNode tnRoot = new TreeNode(Title, Seq.ToString());
				if (tnRoot.Value == ValueToSelect)
				{
					tnRoot.Selected = true;

					if (SelectedNodeChanged != null)
						SelectedNodeChanged(tvw, new EventArgs());
				}

				tnRoot.Expanded = true;
				tvw.Nodes.Add(tnRoot);
				AddChild(dt.Rows[0], tvw.Nodes[0], ColumnNameId, ColumnNameText, ValueToSelect, SelectedNodeChanged);
			}
		}

		public static void AddChild(DataRow drParent, TreeNode tvwParent, string ColumnNameValue, string ColumnNameText,
			string ValueToSelect, EventHandler SelectedNodeChanged)
		{
			DataRow[] adr = drParent.GetChildRows("R");
			foreach (DataRow dr in adr)
			{
				int SeqCur = Convert.ToInt32(dr[ColumnNameValue]);
				string TitleCur = dr[ColumnNameText].ToString();

				TreeNode tnCur = new TreeNode(TitleCur, SeqCur.ToString());
				if (tnCur.Value == ValueToSelect)
				{
					tnCur.Selected = true;

					if (SelectedNodeChanged != null)
						SelectedNodeChanged(tvwParent, new EventArgs());
				}

				tnCur.Expanded = true;
				tvwParent.ChildNodes.Add(tnCur);

				AddChild(dr, tnCur, ColumnNameValue, ColumnNameText, ValueToSelect, SelectedNodeChanged);
			}
		}

		public static void AddTestNodes(TreeView tvw)
		{
			tvw.Nodes.Clear();

			for (int i = 0, i2 = 3; i < i2; i++)
			{
				TreeNode tnNew = new TreeNode("제목" + i.ToString(), i.ToString());
				tvw.Nodes.Add(tnNew);

				for (int j = 0, j2 = 6; j < j2; j++)
				{
					TreeNode tnNew2 = new TreeNode("하위" + j.ToString(), i.ToString() + "." + j.ToString());
					tnNew.ChildNodes.Add(tnNew2);
				}
			}
		}

		public static void ShowFoldersTreeView(DirectoryInfo Parent, TreeView Tvw, TreeNode NodeParent)
		{
			if (NodeParent == null)
			{
				NodeParent = new TreeNode(Parent.Name, Parent.FullName);
				Tvw.Nodes.Clear();
				Tvw.Nodes.Add(NodeParent);
			}

			DirectoryInfo[] adi = Parent.GetDirectories("*.*", SearchOption.TopDirectoryOnly);
			foreach (DirectoryInfo di in adi)
			{
				TreeNode NodeChild = new TreeNode(di.Name, di.FullName);
				NodeParent.ChildNodes.Add(NodeChild);

				ShowFoldersTreeView(di, Tvw, NodeChild);
			}
		}

		public static void ExpandParentAll(TreeNode tn)
		{
			TreeNode tnParent = tn;
			while (true)
			{
				tnParent = tnParent.Parent;
				if (tnParent == null)
					break;

				tnParent.Expanded = true;
			}
		}

		public static TreeNodeCollection GetCollectionOfParentNode(TreeView tvw, TreeNode tn)
		{
			return (tn.Parent == null) ? tvw.Nodes : tn.Parent.ChildNodes;
		}

		public static int GetIndex(TreeView tvw, TreeNode tn)
		{
			TreeNodeCollection tnNodes = GetCollectionOfParentNode(tvw, tn);

			for (int i = 0, i2 = tnNodes.Count; i < i2; i++)
			{
				if (tnNodes[i] == tn)
				{
					return i;
				}
			}

			return -1;
		}

		public static TreeNode GetPreviousNode(TreeView tvw, TreeNode tnCur, out int IndexIs)
		{
			IndexIs = -1;

			TreeNodeCollection tnNodes = GetCollectionOfParentNode(tvw, tnCur);

			for (int i = 1, i2 = tnNodes.Count; i < i2; i++)
			{
				TreeNode tnChild = tnNodes[i];
				if (tnChild == tnCur)
				{
					IndexIs = i - 1;
					return tnNodes[IndexIs];
				}
			}

			return null;
		}
		public static TreeNode GetPreviousNode(TreeView tvw, TreeNode tnCur)
		{
			int IndexIs;
			return GetPreviousNode(tvw, tnCur, out IndexIs);
		}

		public static TreeNode GetNextNode(TreeView tvw, TreeNode tnCur, out int IndexIs)
		{
			IndexIs = -1;

			TreeNodeCollection tnNodes = GetCollectionOfParentNode(tvw, tnCur);

			for (int i = 0, i2 = (tnNodes.Count - 1); i < i2; i++)
			{
				TreeNode tnChild = tnNodes[i];
				if (tnChild == tnCur)
				{
					IndexIs = i + 1;
					return tnNodes[IndexIs];
				}
			}

			return null;
		}
		public static TreeNode GetNextNode(TreeView tvw, TreeNode tnCur)
		{
			int IndexIs;
			return GetNextNode(tvw, tnCur, out IndexIs);
		}

		public static string GetTextPath(TreeNode tn, string PathSeparator)
		{
			Stack<string> st = new Stack<string>();

			while (tn != null)
			{
				st.Push(tn.Text);
				tn = tn.Parent;
			}

			string[] aPath = st.ToArray();
			string Path = string.Join(PathSeparator, aPath);
			return Path;
		}

		public static List<TreeNode> GetAllNode(TreeView Tvw)
		{
			List<TreeNode> aNode = new List<TreeNode>();

			foreach (TreeNode TNodeCur in Tvw.Nodes)
			{
				aNode.Add(TNodeCur);
				if (TNodeCur.ChildNodes.Count > 0)
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
			foreach (TreeNode TNodeChild in TNodeParent.ChildNodes)
			{
				aNode.Add(TNodeChild);
				if (TNodeChild.ChildNodes.Count > 0)
				{
					GetAllChildNode(ref aNode, TNodeChild);
				}
			}
		}

	}
}
