namespace Kesco.App.Win.DocView.Dialogs
{
	partial class StampEditDialog
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
			if (disposing)
			{
				if (components != null)
					components.Dispose();
				if (_stampImage != null)
					_stampImage.Dispose();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StampEditDialog));
			this.btnOK = new System.Windows.Forms.Button();
			this.textName = new System.Windows.Forms.TextBox();
			this.lbStampName = new System.Windows.Forms.Label();
			this.pictureStamp = new System.Windows.Forms.PictureBox();
			this.btnRights = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnRules = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.textNameEn = new System.Windows.Forms.TextBox();
			this.buttonReplace = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.pictureStamp)).BeginInit();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Name = "btnOK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// textName
			// 
			resources.ApplyResources(this.textName, "textName");
			this.textName.Name = "textName";
			this.textName.TextChanged += new System.EventHandler(this.textName_TextChanged);
			this.textName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textName_KeyPress);
			// 
			// lbStampName
			// 
			resources.ApplyResources(this.lbStampName, "lbStampName");
			this.lbStampName.Name = "lbStampName";
			// 
			// pictureStamp
			// 
			resources.ApplyResources(this.pictureStamp, "pictureStamp");
			this.pictureStamp.BackColor = System.Drawing.SystemColors.Window;
			this.pictureStamp.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pictureStamp.Name = "pictureStamp";
			this.pictureStamp.TabStop = false;
			// 
			// btnRights
			// 
			resources.ApplyResources(this.btnRights, "btnRights");
			this.btnRights.Name = "btnRights";
			this.btnRights.UseVisualStyleBackColor = true;
			this.btnRights.Click += new System.EventHandler(this.btnRights_Click);
			// 
			// btnCancel
			// 
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnRules
			// 
			resources.ApplyResources(this.btnRules, "btnRules");
			this.btnRules.Name = "btnRules";
			this.btnRules.UseVisualStyleBackColor = true;
			this.btnRules.Click += new System.EventHandler(this.btnRules_Click);
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// textNameEn
			// 
			resources.ApplyResources(this.textNameEn, "textNameEn");
			this.textNameEn.Name = "textNameEn";
			this.textNameEn.TextChanged += new System.EventHandler(this.textName_TextChanged);
			// 
			// buttonReplace
			// 
			resources.ApplyResources(this.buttonReplace, "buttonReplace");
			this.buttonReplace.Name = "buttonReplace";
			this.buttonReplace.UseVisualStyleBackColor = true;
			this.buttonReplace.Click += new System.EventHandler(this.buttonReplace_Click);
			// 
			// StampEditDialog
			// 
			this.AcceptButton = this.btnOK;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ControlBox = false;
			this.Controls.Add(this.buttonReplace);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textNameEn);
			this.Controls.Add(this.btnRules);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnRights);
			this.Controls.Add(this.pictureStamp);
			this.Controls.Add(this.lbStampName);
			this.Controls.Add(this.textName);
			this.Controls.Add(this.btnOK);
			this.Name = "StampEditDialog";
			this.Load += new System.EventHandler(this.StampEditDialog_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureStamp)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.TextBox textName;
		private System.Windows.Forms.Label lbStampName;
		private System.Windows.Forms.PictureBox pictureStamp;
		private System.Windows.Forms.Button btnRights;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnRules;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textNameEn;
		private System.Windows.Forms.Button buttonReplace;
	}
}

