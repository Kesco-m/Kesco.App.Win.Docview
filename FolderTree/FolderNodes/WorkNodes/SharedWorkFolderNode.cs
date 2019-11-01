using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Kesco.App.Win.DocView.FolderTree.FolderNodes.WorkNodes
{
    public class SharedWorkFolderNode : WorkNode
    {
        private bool fullAccessOnly;

        private static string rootTitle = Environment.StringResources.GetString("SharedFolders");
        protected new const int rootID = -1;

        protected MenuItem propertiesItem;

        internal SharedWorkFolderNode(int id, string name, AccessLevel rights, bool fullAccessOnly) : base(id, name)
        {
            Rights = rights;
            this.fullAccessOnly = fullAccessOnly;

            Emp = Environment.CurEmp;

            if (id == rootID)
            {
                unselectedCollapsed = Images.SharedWorkFolder;
                selectedCollapsed = Images.SharedWorkFolder;
                unselectedExpanded = Images.SharedWorkFolder;
                selectedExpanded = Images.SharedWorkFolder;
            }
            else
            {
                if (rights == AccessLevel.ReadOnly)
                {
                    unselectedCollapsed = Images.SystemFolder;
                    selectedCollapsed = Images.SystemFolder;
                    unselectedExpanded = Images.OpenSystemFolder;
                    selectedExpanded = Images.OpenSystemFolder;
                }
                else
                {
                    unselectedCollapsed = Images.FullAccessFolder;
                    selectedCollapsed = Images.FullAccessFolder;
                    unselectedExpanded = Images.OpenFullAccessFolder;
                    selectedExpanded = Images.OpenFullAccessFolder;
                }
            }

            UpdateImages();

            if (id == rootID)
                LoadSubNodes();

            propertiesItem = new MenuItem {Index = 0, Text = Environment.StringResources.GetString("Properties")};

            // свойства
            propertiesItem.Click += propertiesItem_Click;
        }

        #region Accessors

        public AccessLevel Rights { get; set; }

        #endregion

        private SharedWorkFolderNode Populate(DataRow dr, DataRelation rel)
        {
            AccessLevel nodeRights = AccessLevel.ReadOnly;
            var byteRights = (byte) dr[Environment.SharedFolderData.RightsField];
            if (byteRights == (int) AccessLevel.FullAccess)
                nodeRights = AccessLevel.FullAccess;

            var node = new SharedWorkFolderNode(
                (int) dr[Environment.SharedFolderData.IDField], // id
                (string) dr[Environment.SharedFolderData.NameField], // text
                nodeRights,
                fullAccessOnly);

            foreach (SharedWorkFolderNode newNode in dr.GetChildRows(rel).Select(row => Populate(row, rel)))
                node.Nodes.Add(newNode);

            return node;
        }

        public override void On_MouseUp(MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Right:
                    TreeView.SelectedNode = this;
                    if (ID != -1)
                    {
                        var contextMenu = new ContextMenu(
                            new[] {propertiesItem});

                        contextMenu.Show(TreeView, new Point(e.X, e.Y));
                    }
                    break;
            }
        }

        public override void On_AfterLabelEdit(NodeLabelEditEventArgs e)
        {
            TreeView.LabelEdit = false;

            if (string.IsNullOrEmpty(e.Label) || string.Equals(e.Label, e.Node.Text))
                e.CancelEdit = true;
            else
            {
                bool result = Environment.SharedFolderData.SetField(
                    Environment.SharedFolderData.NameField, SqlDbType.NVarChar, ID, e.Label);

                if (!result)
                    e.CancelEdit = true;
            }
        }
        
        private void propertiesItem_Click(object sender, EventArgs e)
        {
			(new PropertiesDialogs.PropertiesSharedWorkFolderDialog(ID, Text)).Show();
        }

        #region Drop

        public override bool DropAllowed(Node ground)
        {
            return !IsAncestor(ground);
        }

        public override bool SortedDropAllowed(Node ground)
        {
            try
            {
                var wfNode = (SharedWorkFolderNode) ground;
                return wfNode != null && (!IsAncestor(ground) && (wfNode.ID != 0));
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }

            return false;
        }

        #endregion

        #region Move

        public override bool UnsortedMove(Node where)
        {
            try
            {
                var wfNode = (SharedWorkFolderNode) where;
                bool result = Environment.SharedFolderData.UnsortedMove(ID, wfNode.ID);
                if (result)
                {
                    TreeView ownerTV = TreeView;
                    ownerTV.BeginUpdate();

                    Remove();
                    wfNode.Nodes.Add(this);

                    ownerTV.EndUpdate();
                }

                return result;
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }

            return false;
        }

        public override bool SortedMove(Node before)
        {
            try
            {
                var wfNode = (SharedWorkFolderNode) before;
                bool result = Environment.SharedFolderData.SortedMove(ID, wfNode.ID);
                if (result)
                {
                    TreeView ownerTV = TreeView;
                    ownerTV.BeginUpdate();

                    Remove();
                    wfNode.Parent.Nodes.Insert(wfNode.Index, this);

                    ownerTV.EndUpdate();
                }

                return result;
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }

            return false;
        }

        #endregion

        public new void LoadSubNodes()
        {
            try
            {
                Nodes.Clear();

                if (TreeView != null)
                    TreeView.Cursor = Cursors.WaitCursor;

                using (DataSet ds = fullAccessOnly ? Environment.SharedFolderData.GetFullAccessFolders(Emp.ID) : Environment.SharedFolderData.GetFolders(Emp.ID, System.Threading.CancellationToken.None))
                using (DataTable dt = ds.Tables[Environment.SharedFolderData.TableName])
                {
                    DataRelation rel = ds.Relations[Environment.SharedFolderData.ParentRelation];

                    foreach (DataRow dr in dt.Rows)
                    {
                        DataRow[] drs = dr.GetParentRows(rel);
                        if (drs.Length == 0)
                        {
                            SharedWorkFolderNode node = Populate(dr, rel);
                            Nodes.Add(node);
                        }
                    }
                }
                if (TreeView != null)
                    TreeView.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
            base.LoadSubNodes();
        }

        public static SharedWorkFolderNode CreateRoot(bool fullAccessOnly)
        {
            return new SharedWorkFolderNode(rootID, rootTitle, AccessLevel.ReadOnly, fullAccessOnly);
        }

		public override void LoadDocs(Grids.DocGrid docGrid, bool clean, int curID, string filename = null)
		{
			this.curID = curID;
			docGrid.Sorted += Sorted;
			if(Rights == AccessLevel.ReadOnly)
				docGrid.LoadSharedWorkFolderDocs(ID, Emp, clean);
			else
				docGrid.LoadFullAccessFolderDocs(ID, Emp, clean);
		}

        public override Context BuildContext()
        {
			return new Context(Misc.ContextMode.SharedWorkFolder, ID, Emp);
        }

        public override void Nullify()
        {
            base.Nullify();
            LoadSubNodes();
        }

        public override bool IsWork()
        {
            return true;
        }

        public override bool IsSharedWorkFolder()
        {
            return true;
        }
    }
}