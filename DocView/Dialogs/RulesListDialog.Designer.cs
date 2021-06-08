namespace Kesco.App.Win.DocView.Dialogs
{
    partial class RulesListDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RulesListDialog));
            this.listViewRules = new System.Windows.Forms.ListView();
            this.columnHeaderFIO = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderOrganization = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderDocType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnDel = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listViewRules
            // 
            resources.ApplyResources(this.listViewRules, "listViewRules");
            this.listViewRules.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderFIO,
            this.columnHeaderOrganization,
            this.columnHeaderDocType});
            this.listViewRules.FullRowSelect = true;
            this.listViewRules.GridLines = true;
            this.listViewRules.HideSelection = false;
            this.listViewRules.Name = "listViewRules";
            this.listViewRules.UseCompatibleStateImageBehavior = false;
            this.listViewRules.View = System.Windows.Forms.View.Details;
            this.listViewRules.SelectedIndexChanged += new System.EventHandler(this.listViewRules_SelectedIndexChanged);
            this.listViewRules.DoubleClick += new System.EventHandler(this.listViewRules_DoubleClick);
            this.listViewRules.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listViewRules_KeyUp);
            // 
            // columnHeaderFIO
            // 
            resources.ApplyResources(this.columnHeaderFIO, "columnHeaderFIO");
            // 
            // columnHeaderOrganization
            // 
            resources.ApplyResources(this.columnHeaderOrganization, "columnHeaderOrganization");
            // 
            // columnHeaderDocType
            // 
            resources.ApplyResources(this.columnHeaderDocType, "columnHeaderDocType");
            // 
            // btnEdit
            // 
            resources.ApplyResources(this.btnEdit, "btnEdit");
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnDel
            // 
            resources.ApplyResources(this.btnDel, "btnDel");
            this.btnDel.Name = "btnDel";
            this.btnDel.UseVisualStyleBackColor = true;
            this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnAdd
            // 
            resources.ApplyResources(this.btnAdd, "btnAdd");
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // RulesListDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ControlBox = false;
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnDel);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.listViewRules);
            this.DoubleBuffered = true;
            this.Name = "RulesListDialog";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewRules;
        private System.Windows.Forms.ColumnHeader columnHeaderFIO;
        private System.Windows.Forms.ColumnHeader columnHeaderDocType;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnDel;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.ColumnHeader columnHeaderOrganization;
    }
}