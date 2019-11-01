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
    partial class MoveDocDialog
    {
 
        #region Windows Form Designer generated code
		
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MoveDocDialog));
			this.foldersList = new System.Windows.Forms.ImageList(this.components);
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.doc = new System.Windows.Forms.RichTextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonCreateFolder = new System.Windows.Forms.Button();
			this.tree = new Kesco.App.Win.DocView.FolderTree.FolderTree();
			this.SuspendLayout();
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
			// doc
			// 
			resources.ApplyResources(this.doc, "doc");
			this.doc.Name = "doc";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label1.Name = "label1";
			// 
			// buttonCreateFolder
			// 
			resources.ApplyResources(this.buttonCreateFolder, "buttonCreateFolder");
			this.buttonCreateFolder.Name = "buttonCreateFolder";
			this.buttonCreateFolder.UseVisualStyleBackColor = true;
			this.buttonCreateFolder.Click += new System.EventHandler(this.buttonCreateFolder_Click);
			// 
			// tree
			// 
			this.tree.AllowDrop = true;
			resources.ApplyResources(this.tree, "tree");
			this.tree.CheckBoxes = true;
			this.tree.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
			this.tree.HideSelection = false;
			this.tree.ImageList = this.foldersList;
			this.tree.ItemHeight = 16;
			this.tree.Name = "tree";
			this.tree.SelectedNode = null;
			this.tree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tree_AfterSelect);
			// 
			// MoveDocDialog
			// 
			this.AcceptButton = this.buttonOK;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.buttonCreateFolder);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.doc);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.tree);
			this.Controls.Add(this.buttonCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MoveDocDialog";
			this.Load += new System.EventHandler(this.MoveDocDialog_Load);
			this.ResumeLayout(false);

        }
        #endregion

		private Button buttonOK;
        private Button buttonCancel;
        private FolderTree.FolderTree tree;
        private IContainer components;
		private RichTextBox doc;
        private Label label1;
        private Button buttonCreateFolder;

        private ImageList foldersList;

     }
}