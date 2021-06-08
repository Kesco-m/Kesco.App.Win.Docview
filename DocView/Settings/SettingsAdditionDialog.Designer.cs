namespace Kesco.App.Win.DocView.Settings
{
	partial class SettingsAdditionDialog
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
			if(disposing && (components != null))
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsAdditionDialog));
			this.checkBoxSeachReload = new System.Windows.Forms.CheckBox();
			this.numericUpDownMinutes = new System.Windows.Forms.NumericUpDown();
			this.labelM = new System.Windows.Forms.Label();
			this.buttonOk = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownMinutes)).BeginInit();
			this.SuspendLayout();
			// 
			// checkBoxSeachReload
			// 
			resources.ApplyResources(this.checkBoxSeachReload, "checkBoxSeachReload");
			this.checkBoxSeachReload.Name = "checkBoxSeachReload";
			this.checkBoxSeachReload.UseVisualStyleBackColor = true;
			// 
			// numericUpDownMinutes
			// 
			resources.ApplyResources(this.numericUpDownMinutes, "numericUpDownMinutes");
			this.numericUpDownMinutes.Name = "numericUpDownMinutes";
			// 
			// labelM
			// 
			resources.ApplyResources(this.labelM, "labelM");
			this.labelM.Name = "labelM";
			// 
			// buttonOk
			// 
			resources.ApplyResources(this.buttonOk, "buttonOk");
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// SettingsAdditionDialog
			// 
			this.AcceptButton = this.buttonOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.labelM);
			this.Controls.Add(this.numericUpDownMinutes);
			this.Controls.Add(this.checkBoxSeachReload);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SettingsAdditionDialog";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsAdditionDialog_FormClosing);
			this.Load += new System.EventHandler(this.SettingsAdditionDiaolg_Load);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownMinutes)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox checkBoxSeachReload;
		private System.Windows.Forms.NumericUpDown numericUpDownMinutes;
		private System.Windows.Forms.Label labelM;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.Button buttonCancel;
	}
}