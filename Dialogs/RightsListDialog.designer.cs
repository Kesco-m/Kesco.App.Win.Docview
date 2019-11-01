namespace Kesco.App.Win.DocView.Dialogs
{
	partial class RightsListDialog
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RightsListDialog));
            this.listViewRights = new System.Windows.Forms.ListView();
            this.columnHeaderFIO = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderProxies = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnDel = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listViewRights
            // 
            resources.ApplyResources(this.listViewRights, "listViewRights");
            this.listViewRights.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderFIO,
            this.columnHeaderProxies});
            this.listViewRights.FullRowSelect = true;
            this.listViewRights.GridLines = true;
            this.listViewRights.Name = "listViewRights";
            this.listViewRights.UseCompatibleStateImageBehavior = false;
            this.listViewRights.View = System.Windows.Forms.View.Details;
            this.listViewRights.SelectedIndexChanged += new System.EventHandler(this.listViewRights_SelectedIndexChanged);
            // 
            // columnHeaderFIO
            // 
            resources.ApplyResources(this.columnHeaderFIO, "columnHeaderFIO");
            // 
            // columnHeaderProxies
            // 
            resources.ApplyResources(this.columnHeaderProxies, "columnHeaderProxies");
            // 
            // btnAdd
            // 
            resources.ApplyResources(this.btnAdd, "btnAdd");
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnDel
            // 
            resources.ApplyResources(this.btnDel, "btnDel");
            this.btnDel.Name = "btnDel";
            this.btnDel.UseVisualStyleBackColor = true;
            this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
            // 
            // btnEdit
            // 
            resources.ApplyResources(this.btnEdit, "btnEdit");
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // RightsListDialog
            // 
            this.AcceptButton = this.btnOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ControlBox = false;
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnDel);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.listViewRights);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "RightsListDialog";
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView listViewRights;
		private System.Windows.Forms.ColumnHeader columnHeaderFIO;
		private System.Windows.Forms.ColumnHeader columnHeaderProxies;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnDel;
		private System.Windows.Forms.Button btnEdit;

	}
}