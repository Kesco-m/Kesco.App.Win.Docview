using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Kesco.App.Win.DocView.FolderTree.FolderNodes;
using Kesco.App.Win.DocView.FolderTree.FolderNodes.WorkNodes;
using Kesco.Lib.Win;

namespace Kesco.App.Win.DocView.PropertiesDialogs
{
    public class PropertiesFolderRuleApplyDialog : FreeDialog
    {
        private Button buttonOK;
        private Button buttonCancel;
        private IContainer components;

        private Label label1;
        private Label label2;
        private ImageList foldersList;
        private FolderTree.FolderTree tree;
        private CheckBox ApplySubFoldersCheckBox;
        private Label ruleNameLabel;
        private readonly string FolderName = "";
        private Node PreviousCheckedNode;

        private ArrayList Nodes;
        private bool UnCheckIsWork;

        public PropertiesFolderRuleApplyDialog(int RuleID, string RuleName, string FolderName)
        {
            InitializeComponent();

            this.FolderName = FolderName;
            this.RuleID = RuleID;
            ruleNameLabel.Text = RuleName;
        }

        #region Accessors

        public int SelectedFolderID { get; private set; }

        public bool ApplySubFolders { get; private set; }

        public int RuleID { get; private set; }

        #endregion

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
        ///   Required method for Designer support - do not modify the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertiesFolderRuleApplyDialog));
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.ruleNameLabel = new System.Windows.Forms.Label();
			this.foldersList = new System.Windows.Forms.ImageList(this.components);
			this.tree = new Kesco.App.Win.DocView.FolderTree.FolderTree();
			this.ApplySubFoldersCheckBox = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
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
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// ruleNameLabel
			// 
			resources.ApplyResources(this.ruleNameLabel, "ruleNameLabel");
			this.ruleNameLabel.Name = "ruleNameLabel";
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
			// tree
			// 
			resources.ApplyResources(this.tree, "tree");
			this.tree.CheckBoxes = true;
			this.tree.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
			this.tree.ImageList = this.foldersList;
			this.tree.ItemHeight = 16;
			this.tree.Name = "tree";
			this.tree.SelectedNode = null;
			this.tree.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tree_AfterCheck);
			// 
			// ApplySubFoldersCheckBox
			// 
			resources.ApplyResources(this.ApplySubFoldersCheckBox, "ApplySubFoldersCheckBox");
			this.ApplySubFoldersCheckBox.Name = "ApplySubFoldersCheckBox";
			// 
			// PropertiesFolderRuleApplyDialog
			// 
			this.AcceptButton = this.buttonOK;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.tree);
			this.Controls.Add(this.ApplySubFoldersCheckBox);
			this.Controls.Add(this.ruleNameLabel);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PropertiesFolderRuleApplyDialog";
			this.Load += new System.EventHandler(this.PropertiesFolderRuleApplyDialog_Load);
			this.ResumeLayout(false);

        }

        #endregion

        private void PropertiesFolderRuleApplyDialog_Load(object sender, EventArgs e)
        {
            try
            {
                tree.CreateWorkFolderRoot();
                tree.WorkFolderNode.Checked = true;
                PreviousCheckedNode = tree.WorkFolderNode;
                tree.CreateSharedWorkFolderRoot(true);

                tree.ExpandAll();
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        private void UnCheckAllNode()
        {
            try
            {
                UnCheckIsWork = true;
                foreach (TreeNode subNode in tree.Nodes)
                {
                    subNode.Checked = false;
                    UnCheckAllSubNode(subNode);
                }
                UnCheckIsWork = false;
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        private void UnCheckAllSubNode(TreeNode node)
        {
            try
            {
                node.Checked = false;
                foreach (TreeNode subNode in node.Nodes)
                    UnCheckAllSubNode(subNode);
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            End(DialogResult.Cancel);
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (Nodes == null)
                    Nodes = new ArrayList();
                else if (Nodes.Count > 0)
                    Nodes.Clear();

                SelectAllNode(tree.WorkFolderNode);

                if (Nodes.Count == 1)
                {
                    SelectedFolderID = ((WorkNode) Nodes[0]).ID;
                    ApplySubFolders = ApplySubFoldersCheckBox.Checked;
                    End(DialogResult.OK);
                }
                else
                    End(DialogResult.Cancel);
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        private void SelectAllNode(TreeNode node)
        {
            try
            {
                if (node.Checked)
                    Nodes.Add(node);
                foreach (TreeNode subNode in node.Nodes)
                    SelectAllNode(subNode);
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        private void tree_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (UnCheckIsWork)
                return;
            if (e.Action != TreeViewAction.ByMouse && e.Action != TreeViewAction.ByKeyboard)
                return;
            if (!e.Node.Checked)
                return;
            try
            {
                UnCheckAllNode();
                if (string.Equals(e.Node.Text, FolderName))
                {
                    if (PreviousCheckedNode != null)
                    {
                        PreviousCheckedNode.Checked = true;
                        e.Node.Checked = false;
                    }
                }
                else
                {
                    PreviousCheckedNode = (Node) e.Node;
                    e.Node.Checked = true;
                }
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }
    }
}