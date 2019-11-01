#region

using System.Drawing;

#endregion

namespace Kesco.App.Win.DocView.FolderTree.FolderNodes.FaxNodes
{
    /// <summary>
    ///   Summary description for FaxNode.
    /// </summary>
    public class FaxNode : Node
    {
        protected string path;

        protected FaxNode(string name)
        {
            unselectedCollapsed = Images.Fax;
            selectedCollapsed = Images.Fax;
            unselectedExpanded = Images.Fax;
            selectedExpanded = Images.Fax;

            UpdateImages();

            this.name = name;
            Text = name;
        }

		public string FaxPath
		{
			get { return path; }
		}

        public void UpdateStatus()
        {
            if (TreeView == null)
                return;
            int unread, overall;
            if (Environment.FaxData.GetStatus(ID, out unread, out overall, true))
            {
                NodeFont = unread > 0 ? TreeView.Font : new Font(TreeView.Font, FontStyle.Regular);
                Text = overall > 0 ? string.Format("{0} ({1}/{2})", name, unread, overall) : name;
            }
        }
    }
}