using Kesco.Lib.Win.Document.Blocks;

namespace Kesco.App.Win.DocView.Dialogs
{
	partial class RuleEditDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RuleEditDialog));
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.employeeBlock = new Kesco.Lib.Win.Document.Blocks.EmployeeBlock();
			this.personSearchBlock = new Kesco.Lib.Win.Document.Blocks.PersonSearchBlock_1();
			this.docTypeBlock = new Kesco.Lib.Win.Document.Blocks.DocTypeBlock();
			this.DocTypeAllchkBox = new System.Windows.Forms.CheckBox();
			this.PersonAllchkBox = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
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
			// employeeBlock
			// 
			resources.ApplyResources(this.employeeBlock, "employeeBlock");
			this.employeeBlock.BackColor = System.Drawing.SystemColors.Control;
			this.employeeBlock.ButtonSide = Kesco.Lib.Win.Document.Blocks.EmployeeBlock.ButtonSideEnum.Right;
			this.employeeBlock.FullText = "";
			this.employeeBlock.Name = "employeeBlock";
			this.employeeBlock.ParamStr = "clid=3&UserAccountDisabled=0&return=1";
			this.employeeBlock.EmployeeSelected += new Kesco.Lib.Win.Document.Blocks.EmployeeBlockEventHandler(this.employeeBlock_EmployeeSelected);
			this.employeeBlock.ButtonTextChanged += new System.EventHandler(this.employeeBlock_ButtonTextChanged);
			// 
			// personSearchBlock
			// 
			resources.ApplyResources(this.personSearchBlock, "personSearchBlock");
			this.personSearchBlock.ButtonWight = 72;
			this.personSearchBlock.FullText = "";
			this.personSearchBlock.Name = "personSearchBlock";
			this.personSearchBlock.FindPerson += new System.EventHandler(this.personSearchBlock_FindPerson);
			// 
			// docTypeBlock
			// 
			resources.ApplyResources(this.docTypeBlock, "docTypeBlock");
			this.docTypeBlock.ID = 0;
			this.docTypeBlock.Name = "docTypeBlock";
			this.docTypeBlock.Selected += new Kesco.Lib.Win.Document.Blocks.BlockEventHandler(this.docTypeBlock_Selected);
			// 
			// DocTypeAllchkBox
			// 
			resources.ApplyResources(this.DocTypeAllchkBox, "DocTypeAllchkBox");
			this.DocTypeAllchkBox.Name = "DocTypeAllchkBox";
			this.DocTypeAllchkBox.UseVisualStyleBackColor = true;
			// 
			// PersonAllchkBox
			// 
			resources.ApplyResources(this.PersonAllchkBox, "PersonAllchkBox");
			this.PersonAllchkBox.Name = "PersonAllchkBox";
			this.PersonAllchkBox.UseVisualStyleBackColor = true;
			// 
			// RuleEditDialog
			// 
			this.AcceptButton = this.btnOK;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ControlBox = false;
			this.Controls.Add(this.PersonAllchkBox);
			this.Controls.Add(this.DocTypeAllchkBox);
			this.Controls.Add(this.employeeBlock);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.personSearchBlock);
			this.Controls.Add(this.docTypeBlock);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "RuleEditDialog";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private DocTypeBlock docTypeBlock;
		private Lib.Win.Document.Blocks.PersonSearchBlock_1 personSearchBlock;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private Lib.Win.Document.Blocks.EmployeeBlock employeeBlock;
		private System.Windows.Forms.CheckBox DocTypeAllchkBox;
		private System.Windows.Forms.CheckBox PersonAllchkBox;
	}
}