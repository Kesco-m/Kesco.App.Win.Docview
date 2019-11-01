using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Kesco.Lib.Win;
using Kesco.Lib.Win.Document;

namespace Kesco.App.Win.DocView.Dialogs
{
    public class FolderSelectDialog : FreeDialog
    {
        private Button buttonOK;
        private Button buttonCancel;
        private FolderTree.FolderTree tree;
        private IContainer components;

        private int[] docIDs;

        private ImageList foldersList;
        private RichTextBox doc;
        private Button buttonCreateFolder;
        private Label label1;

        public FolderSelectDialog(int[] docIDs, int empID)
        {
            InitializeComponent();

            Show();
            Cursor = Cursors.WaitCursor;
            Enabled = false;
            this.docIDs = docIDs;

            Slave.DoWork("BuildFriendlyDocList", new object[] { docIDs }, OnBuildFriendlyDocListComplete);

            tree.CreateWorkFolderRoot();
            tree.CreateSharedWorkFolderRoot(true);

            foreach (TreeNode node in tree.Nodes.Cast<TreeNode>().Where(node =>
                                                                            {
                                                                                var workFolderNode = node as FolderTree.FolderNodes.WorkNodes.WorkFolderNode;
                                                                                return workFolderNode != null && workFolderNode.Emp.ID != empID;
                                                                            }))
                node.Remove();

            tree.UpdateWorkFolderStatus();
            tree.ExpandAll();
        }

        private void OnBuildFriendlyDocListComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            doc.Text = e.Result.ToString();
            Enabled = true;
            Cursor = Cursors.Default;
        }
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FolderSelectDialog));
            this.tree = new Kesco.App.Win.DocView.FolderTree.FolderTree();
            this.foldersList = new System.Windows.Forms.ImageList(this.components);
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.doc = new System.Windows.Forms.RichTextBox();
            this.buttonCreateFolder = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tree
            // 
            this.tree.AllowDrop = true;
            resources.ApplyResources(this.tree, "tree");
            this.tree.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this.tree.HideSelection = false;
            this.tree.ImageList = this.foldersList;
            this.tree.ItemHeight = 16;
            this.tree.Name = "tree";
            this.tree.SelectedNode = null;
            this.tree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tree_AfterSelect);
            // 
            // foldersList
            // 
            this.foldersList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("foldersList.ImageStream")));
            this.foldersList.TransparentColor = System.Drawing.SystemColors.Window;
            this.foldersList.Images.SetKeyName(0, "");
            this.foldersList.Images.SetKeyName(1, "");
            this.foldersList.Images.SetKeyName(2, "");
            this.foldersList.Images.SetKeyName(3, "");
            this.foldersList.Images.SetKeyName(4, "");
            this.foldersList.Images.SetKeyName(5, "");
            this.foldersList.Images.SetKeyName(6, "");
            this.foldersList.Images.SetKeyName(7, "");
            this.foldersList.Images.SetKeyName(8, "");
            this.foldersList.Images.SetKeyName(9, "");
            this.foldersList.Images.SetKeyName(10, "");
            this.foldersList.Images.SetKeyName(11, "");
            // 
            // buttonOK
            // 
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // doc
            // 
            resources.ApplyResources(this.doc, "doc");
            this.doc.Name = "doc";
            this.doc.ReadOnly = true;
            // 
            // buttonCreateFolder
            // 
            resources.ApplyResources(this.buttonCreateFolder, "buttonCreateFolder");
            this.buttonCreateFolder.Name = "buttonCreateFolder";
            this.buttonCreateFolder.UseVisualStyleBackColor = true;
            this.buttonCreateFolder.Click += new System.EventHandler(this.buttonCreateFolder_Click);
            // 
            // FolderSelectDialog
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.buttonCreateFolder);
            this.Controls.Add(this.doc);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.tree);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FolderSelectDialog";
            this.ResumeLayout(false);

        }
        #endregion

        private void buttonOK_Click(object sender, EventArgs e)
        {
			var node = tree.SelectedNode as FolderTree.FolderNodes.WorkNodes.WorkNode;
            if (node != null)
            {
                Enabled = false;
                int[] result = Environment.WorkDocData.AddDocToWorkFolder(docIDs, node.ID, node.Emp.ID);
                for (int i = 0; i < result.Length; i++)
                {
                    if (result[i] != 1)
                    {
                        string docStr = DBDocString.Format(docIDs[i]);
                        Environment.UndoredoStack.Add("AddDocToWork", Environment.StringResources.GetString("AddDocToWork"),
                                string.Format(Environment.StringResources.GetString("UndoAddDocToWork"), docStr),
                                string.Format(Environment.StringResources.GetString("RedoAddDocToWork"), docStr),
                                UndoRedoCommands.UndoAddToWork, new object[] { docIDs[i], node.ID, node.Emp.ID }, node.Emp.ID);
                    }
                }
                End(DialogResult.OK);
            }
        }

        private void tree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            bool okEnabled = false;

            if (tree.SelectedNode != null)
            {
                FolderTree.FolderNodes.Node node = tree.SelectedNode;
                okEnabled = node.IsWork();
            }

            buttonOK.Enabled = okEnabled;
            buttonCreateFolder.Enabled = okEnabled;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            End(DialogResult.Cancel);
        }

        private void buttonCreateFolder_Click(object sender, EventArgs e)
        {
			var node = tree.SelectedNode as FolderTree.FolderNodes.WorkNodes.WorkFolderNode;
            if (node != null)
            try
            {
                using(var dialog = new EnterStringDialog(Environment.StringResources.GetString("FolderName"),
                                                           Environment.StringResources.GetString("FolderNameInput"),
                                                           Environment.StringResources.GetString("NewFolder")))
                {
                    if (dialog.ShowDialog(this) == DialogResult.OK)
						tree.SelectedNode = FolderTree.FolderNodes.WorkNodes.WorkFolderNode.CreateNewFolder(node, dialog.Input);
                }
            }
            catch(Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
            tree.UpdateWorkFolderStatus();
        }
    }
}