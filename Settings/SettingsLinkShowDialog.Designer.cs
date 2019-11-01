namespace Kesco.App.Win.DocView.Settings
{
	partial class SettingsLinkShowDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsLinkShowDialog));
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.comboBoxType = new System.Windows.Forms.ComboBox();
			this.comboBoxDocs = new System.Windows.Forms.ComboBox();
			this.textBoxCount = new System.Windows.Forms.TextBox();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.SuspendLayout();
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
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			// 
			// comboBoxType
			// 
			resources.ApplyResources(this.comboBoxType, "comboBoxType");
			this.comboBoxType.AutoCompleteCustomSource.AddRange(new string[] {
            resources.GetString("comboBoxType.AutoCompleteCustomSource"),
            resources.GetString("comboBoxType.AutoCompleteCustomSource1")});
			this.comboBoxType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxType.DropDownWidth = 200;
			this.comboBoxType.FormattingEnabled = true;
			this.comboBoxType.Items.AddRange(new object[] {
            resources.GetString("comboBoxType.Items"),
            resources.GetString("comboBoxType.Items1")});
			this.comboBoxType.Name = "comboBoxType";
			// 
			// comboBoxDocs
			// 
			resources.ApplyResources(this.comboBoxDocs, "comboBoxDocs");
			this.comboBoxDocs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxDocs.DropDownWidth = 230;
			this.comboBoxDocs.Items.AddRange(new object[] {
            resources.GetString("comboBoxDocs.Items"),
            resources.GetString("comboBoxDocs.Items1"),
            resources.GetString("comboBoxDocs.Items2")});
			this.comboBoxDocs.Name = "comboBoxDocs";
			// 
			// textBoxCount
			// 
			resources.ApplyResources(this.textBoxCount, "textBoxCount");
			this.textBoxCount.Name = "textBoxCount";
			// 
			// buttonCancel
			// 
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// buttonOK
			// 
			resources.ApplyResources(this.buttonOK, "buttonOK");
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			// 
			// SettingsLinkShowDialog
			// 
			this.AcceptButton = this.buttonOK;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.label4);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.textBoxCount);
			this.Controls.Add(this.comboBoxDocs);
			this.Controls.Add(this.comboBoxType);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SettingsLinkShowDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Load += new System.EventHandler(this.SettingsLinkShowDialog_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox comboBoxType;
		private System.Windows.Forms.ComboBox comboBoxDocs;
		private System.Windows.Forms.TextBox textBoxCount;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Label label4;
	}
}