using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Kesco.App.Win.DocView.FolderTree.FolderNodes
{
    /// <summary>
    ///   Общий класс для всех классов-узлов
    /// </summary>
    public class Node : TreeNode
    {
		protected internal CancellationTokenSource source;
        protected const int MaxLabelLength = 30;

		protected const int rootID = 0;

        protected string name;

		protected string curFileName;
		protected int curID = 0;

        protected Images unselectedCollapsed;
        protected Images unselectedExpanded;
        protected Images selectedCollapsed;
        protected Images selectedExpanded;

        protected Node()
        {
			source = new CancellationTokenSource();
        }

		/// <summary>
		/// Отмена асинхроных задач
		/// </summary>
		public virtual void CancelOperation()
		{
			source.Cancel();
			source.Dispose();
			source = new CancellationTokenSource();
		}

        public virtual void UpdateImages()
        {
            if (IsExpanded)
            {
                ImageIndex = (int) unselectedExpanded;
                SelectedImageIndex = (int) selectedExpanded;
            }
            else
            {
                ImageIndex = (int) unselectedCollapsed;
                SelectedImageIndex = (int) selectedCollapsed;
            }
        }

        public virtual void LoadSubNodes()
        {
            RemoveBoldRecursive();
        }

        public virtual void On_MouseUp(MouseEventArgs e)
        {
        }

        public virtual void On_AfterLabelEdit(NodeLabelEditEventArgs e)
        {
        }

        public bool IsAncestor(Node node)
        {
            while (node != null)
            {
                if (node.Equals(this))
                    return true;

                node = (Node) node.Parent;
            }

            return false;
        }

		public int ID { get; protected set; }

        public virtual bool DropAllowed(Node ground)
        {
            return false;
        }

        public virtual bool SortedDropAllowed(Node ground)
        {
            return false;
        }

        public virtual bool UnsortedMove(Node where)
        {
            return false;
        }

        public virtual bool SortedMove(Node before)
        {
            return false;
        }

        public virtual void RemoveBold()
        {
            try
            {
                if (TreeView != null)
                    NodeFont = new Font(TreeView.Font, FontStyle.Regular);
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        public virtual void RemoveBoldRecursive()
        {
            try
            {
                if (TreeView == null)
                    return;
                RemoveBold();

                foreach (Node node in Nodes.OfType<Node>())
                    node.RemoveBoldRecursive();
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

		public virtual void LoadDocs(Grids.DocGrid docGrid, bool clean, int curID, string filename = null)
		{
		}

        public virtual Context BuildContext()
        {
            return null;
        }

        public virtual void Nullify()
        {
            if (IsExpanded)
                Collapse();
        }

        #region Is

        public virtual bool IsSystemFolder()
        {
            return false;
        }

        public virtual bool IsCatalog()
        {
            return false;
        }

        public virtual bool IsWork()
        {
            return false;
        }

        public virtual bool IsWorkFolder()
        {
            return false;
        }

        public virtual bool IsSharedWorkFolder()
        {
            return false;
        }

        public virtual bool IsScaner()
        {
            return false;
        }

        public virtual bool IsDocument()
        {
            return false;
        }

        public virtual bool IsFound()
        {
            return false;
        }

		#endregion

		protected virtual void Sorted(object sender, System.EventArgs e)
		{
			Grids.DocGrid grid = sender as Grids.DocGrid;
			if(grid == null && grid.DataSource != null)
				return;
			grid.Sorted -= Sorted;
			if(curID > 0)
			{
				grid.SelectByID(curID);
				curID = 0;
			}
		}

        public virtual string GetToolTip()
        {
            return name;
        }

		public int CurID { get { return curID; } }

		internal void SetCurID(int docID)
		{
			curID = docID;
		}

		internal void SetCurFileName(string fileName)
		{
			curFileName = fileName;
		}
	}
}