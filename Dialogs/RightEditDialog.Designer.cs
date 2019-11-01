namespace Kesco.App.Win.DocView.Dialogs
{
	partial class RightEditDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RightEditDialog));
            this.textBoxEmployee = new System.Windows.Forms.TextBox();
            this.checkBoxEnapleProxies = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.employeeBlock = new Lib.Win.Document.Blocks.EmployeeBlock();
            this.SuspendLayout();
            // 
            // textBoxEmployee
            // 
            resources.ApplyResources(this.textBoxEmployee, "textBoxEmployee");
            this.textBoxEmployee.Name = "textBoxEmployee";
            this.textBoxEmployee.ReadOnly = true;
            this.textBoxEmployee.TabStop = false;
            // 
            // checkBoxEnapleProxies
            // 
            resources.ApplyResources(this.checkBoxEnapleProxies, "checkBoxEnapleProxies");
            this.checkBoxEnapleProxies.Name = "checkBoxEnapleProxies";
            this.checkBoxEnapleProxies.UseVisualStyleBackColor = true;
            this.checkBoxEnapleProxies.CheckedChanged += new System.EventHandler(this.checkBoxEnapleProxies_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // employeeBlock
            // 
            this.employeeBlock.BackColor = System.Drawing.SystemColors.Control;
            this.employeeBlock.ButtonSide = Lib.Win.Document.Blocks.EmployeeBlock.ButtonSideEnum.Left;
            resources.ApplyResources(this.employeeBlock, "employeeBlock");
            this.employeeBlock.FullText = "";
            this.employeeBlock.Name = "employeeBlock";
            this.employeeBlock.ParamStr = "clid=3&UserAccountDisabled=0&return=2";
            this.employeeBlock.EmployeeSelected += new Lib.Win.Document.Blocks.EmployeeBlockEventHandler(this.employeeBlock_EmployeeSelected);
            // 
            // RightEdit
            // 
            this.AcceptButton = this.btnOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ControlBox = false;
            this.Controls.Add(this.employeeBlock);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.checkBoxEnapleProxies);
            this.Controls.Add(this.textBoxEmployee);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RightEdit";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBoxEmployee;
		private System.Windows.Forms.CheckBox checkBoxEnapleProxies;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private Lib.Win.Document.Blocks.EmployeeBlock employeeBlock;
	}
}