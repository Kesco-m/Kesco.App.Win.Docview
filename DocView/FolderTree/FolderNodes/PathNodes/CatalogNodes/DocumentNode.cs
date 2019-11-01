using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Kesco.App.Win.DocView.Misc;
using Kesco.Lib.Win.Document;

namespace Kesco.App.Win.DocView.FolderTree.FolderNodes.PathNodes.CatalogNodes
{
    /// <summary>
    /// ƒокумент, представленный в виде узла дерева
    /// </summary>
    public class DocumentNode : PathNode
    {
        private int docID;
        private bool isRoot;
        private bool isMain;

        protected MenuItem delItem;
        private Container components;

        public DocumentNode(int docID, string path, string nodeTitle, bool links)
            : this(docID, path, true, true, links)
        {
            if (!string.IsNullOrEmpty(nodeTitle))
                Text = nodeTitle;
        }

        public DocumentNode(int docID, string path, bool main)
            : this(docID, path, false, main, false)
        {
        }

        public DocumentNode(int docID, string path, bool root, bool main, bool links)
            : base(path)
        {
            this.docID = docID;

            isRoot = root;
            isMain = main;

            string text = !string.IsNullOrEmpty(Text) ? Text : DBDocString.Format(docID);
            if (text.Length == 0) text = "#" + path;
            if (root)
            {
                name = text;
                Text = text;
                delItem = new MenuItem(Environment.StringResources.GetString("FolderTree.FolderNodes.DocumentNode.Message1"));
                delItem.Click += delItem_Click;
            }
            else
            {
                if (main)
                {
                    name = text;
                    Text = Environment.StringResources.GetString("MainDocuments");
                }
                else
                {
                    name = text;
                    Text = Environment.StringResources.GetString("SubsequentDocuments");
                }
            }

            InitializeComponent();

            if (links && main)
                Nodes.Add("");

            SetImages();
            UpdateImages();
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
        }

        public DataTable GetDataTable()
        {
            return Environment.DocLinksData.GetLinksDocs(docID, isRoot, isMain);
        }

        private void InitializeComponent()
        {
            components = new Container();
        }

        protected virtual void SetImages()
        {
            unselectedCollapsed = Images.CatalogDocType;
            selectedCollapsed = Images.CatalogDocType;
            unselectedExpanded = Images.CatalogDocType;
            selectedExpanded = Images.CatalogDocType;
        }

        public override Context BuildContext()
        {
            return new Context(ContextMode.Document, docID);
        }

		public override void LoadDocs(Grids.DocGrid docGrid, bool clean, int curID, string filename = null)
		{
			this.curID = curID;
			docGrid.Sorted += Sorted;
			docGrid.LoadDocuments(GetDataTable());
		}

        public override void LoadSubNodes()
        {
            try
            {
                if (Nodes.Count > 0)
                    Nodes.Clear();
                List<int> li = Environment.DocLinksData.GetLinksData(docID);
                if (li != null)
                {
                    if (li.Contains(1))
                    {
                        var node = new DocumentNode(docID, docID.ToString(), true);
                        Nodes.Add(node);
                    }
                    if (li.Contains(-1))
                    {
                        var node = new DocumentNode(docID, docID.ToString(), false);
                        Nodes.Add(node);
                    }
                }
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }

            base.LoadSubNodes();
        }

        public override void On_MouseUp(MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;
            TreeView.SelectedNode = this;
            if (delItem == null)
                return;
            try
            {
                var contextMenu = new ContextMenu();
                contextMenu.MenuItems.Add(delItem);
                contextMenu.Show(TreeView, new Point(e.X, e.Y));
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        public override bool IsDocument()
        {
            return true;
        }

        private void delItem_Click(object sender, EventArgs e)
        {
            try
            {
                TreeView.Nodes.Remove(this);
                Environment.UserSettings.LinkDocIDs.Remove(docID);
                Environment.UserSettings.Save();
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }
    }
}