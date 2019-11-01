using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Kesco.Lib.Win;
using Kesco.Lib.Win.Document;

namespace Kesco.App.Win.DocView.Dialogs
{
	/// <summary>
	/// диалог отображения положения документа в Работе
	/// </summary>
    public partial class MoveDocDialog : FreeDialog
    {
        private readonly int[] docIDs;
        private readonly string docIDstr;
        private readonly int empID;

		private SynchronizedCollection<FolderTree.FolderNodes.WorkNodes.WorkNode> selected = new SynchronizedCollection<FolderTree.FolderNodes.WorkNodes.WorkNode>();
		private SynchronizedCollection<FolderTree.FolderNodes.WorkNodes.WorkNode> grayed = new SynchronizedCollection<FolderTree.FolderNodes.WorkNodes.WorkNode>();

		/// <summary>
		/// диалог отображения положения документа в Работе
		/// </summary>
		/// <param name="docIDs">Коды документов для отображения</param>
		/// <param name="empID">код сотрудника, по которому отображается состояние</param>
        public MoveDocDialog(int[] docIDs, int empID)
        {
            InitializeComponent();
            this.docIDs = docIDs;
            this.empID = empID;

            Slave.DoWork("BuildFriendlyDocList", new object[] { docIDs }, OnBuildFriendlyDocListComplete);

			Cursor = Cursors.WaitCursor;
            Enabled = false;

            var sb = new StringBuilder();
            foreach (int docID in docIDs)
                sb.Append(docID.ToString() + ",");

            docIDstr = sb.ToString().TrimEnd(",".ToCharArray());
        }

        void tree_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node.ForeColor == Color.FromKnownColor(KnownColor.DarkGray))
                e.Node.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
        }

        private void OnBuildFriendlyDocListComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            doc.Text = e.Result.ToString();
            doc.ReadOnly = true;
            Enabled = true;
            Cursor = Cursors.Default;
            Focus();
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

        private void CheckIfDocPresent(TreeNode tNode)
        {
            var node = tNode as FolderTree.FolderNodes.Node;
            if (node == null)
                return;

            if (node.IsWork())
            {
				var wNode = node as FolderTree.FolderNodes.WorkNodes.WorkNode;
                if (wNode != null)
                {
                    if (wNode.Emp.ID != empID)
                        wNode.Remove();
                    else
                    {
                        int docsPresentInWorkFolder = Environment.WorkDocData.DocPresentInFolder(docIDstr, wNode.ID, wNode.Emp.ID);

                        if (docsPresentInWorkFolder > 0)
                        {
                            node.Checked = true;
                            selected.Add(wNode);

                            if (docsPresentInWorkFolder < docIDs.Length)
                            {
                                node.ForeColor = Color.FromKnownColor(KnownColor.DarkGray);
                                grayed.Add(wNode);
                            }
                        }
                    }
                }

                foreach (TreeNode subNode in node.Nodes)
                    CheckIfDocPresent(subNode);
            }
            else
                node.Remove();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Enabled = false;
            Cursor = Cursors.WaitCursor;

            Slave.DoWork(ProceedNodeBackGround, new object[] { tree.Nodes }, OnProceedNodesComplete);
        }

        private void ProceedNodeBackGround(object sender, DoWorkEventArgs e)
        {
            var nodes = ((object[])e.Argument)[0] as TreeNodeCollection;
            if (nodes != null)
                foreach (TreeNode tNode in nodes)
                    ProceedNode(tNode);
        }

        private void ProceedNode(TreeNode tNode)
        {
			var wNode = tNode as FolderTree.FolderNodes.WorkNodes.WorkNode;
            if (wNode == null)
                return;
            if (wNode.Checked)
            {
                if (!selected.Contains(wNode) ||
                    (grayed.Contains(wNode) && wNode.ForeColor == Color.FromKnownColor(KnownColor.ControlText)))
                {
                    Environment.WorkDocData.AddDocToWorkFolder(docIDs, wNode.ID, wNode.Emp.ID);


                    string docStr = DBDocString.Format(docIDs[0]);

                    Environment.UndoredoStack.Add("AddDocToWork", Environment.StringResources.GetString("AddDocToWork"),
                                                  string.Format(Environment.StringResources.GetString("UndoAddDocToWork"), docStr),
                                                  string.Format(Environment.StringResources.GetString("RedoAddDocToWork"), docStr),
                                                  UndoRedoCommands.UndoAddToWork,
                                                  new object[] { docIDs[0], wNode.ID, wNode.Emp.ID }, wNode.Emp.ID);
                }
            }
            else
            {
                if (selected.Contains(wNode) ||
                    (grayed.Contains(wNode) && wNode.ForeColor == Color.FromKnownColor(KnownColor.ControlText)))
                {
                    Environment.WorkDocData.RemoveDocFromWorkFolder(docIDs, wNode.ID, wNode.Emp.ID);
                }
            }

            foreach (TreeNode subNode in wNode.Nodes)
                ProceedNode(subNode);
        }

        private void OnProceedNodesComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            End(DialogResult.OK);
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
                    using (var dialog = new EnterStringDialog(Environment.StringResources.GetString("FolderName"),
                                                              Environment.StringResources.GetString("FolderNameInput"),
                                                              Environment.StringResources.GetString("NewFolder")))
                    {
                        if (dialog.ShowDialog(this) == DialogResult.OK)
							tree.SelectedNode = FolderTree.FolderNodes.WorkNodes.WorkFolderNode.CreateNewFolder(node, dialog.Input);
                    }
                }
                catch (Exception ex)
                {
                    Lib.Win.Data.Env.WriteToLog(ex);
                }
            tree.UpdateWorkFolderStatus();
        }

        private void tree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            bool okEnabled = false;

            if (tree.SelectedNode != null)
            {
				FolderTree.FolderNodes.Node node = tree.SelectedNode;
                okEnabled = node.IsWork();
            }

            buttonCreateFolder.Enabled = okEnabled;
        }

		private void MoveDocDialog_LoadComplete(object sender, EventArgs e)
		{
			InvokeIfRequired(new MethodInvoker(()=>
			{
				foreach(TreeNode node in tree.Nodes)
					CheckIfDocPresent(node);

				//tree.ExpandAll();
				tree.AfterCheck += tree_AfterCheck;
			}));
		}

		private void MoveDocDialog_Load(object sender, EventArgs e)
		{
			tree.CreateWorkFolderRoot(new EventHandler(MoveDocDialog_LoadComplete));
			tree.CreateSharedWorkFolderRoot(true);
			tree.UpdateWorkFolderStatus();
		}
    }
}