using System.ComponentModel;
using System.Windows.Forms;

namespace Kesco.App.Win.DocView.Settings
{
	partial class SettingsMessagesAndConfirmsDialog : Lib.Win.FreeDialog
	{

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(components != null)
				{
					components.Dispose();
				}
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsMessagesAndConfirmsDialog));
			this.groupMessage = new System.Windows.Forms.GroupBox();
			this.checkBoxSignMessage = new System.Windows.Forms.CheckBox();
			this.checkBoxNext = new System.Windows.Forms.CheckBox();
			this.checkBoxPersonMessage = new System.Windows.Forms.CheckBox();
			this.timeout = new System.Windows.Forms.TextBox();
			this.checkTimer = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.checkNewMessageNotification = new System.Windows.Forms.CheckBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.groupConfirm = new System.Windows.Forms.GroupBox();
			this.checkDelete = new System.Windows.Forms.CheckBox();
			this.checkBoxScan = new System.Windows.Forms.CheckBox();
			this.checkBoxBar = new System.Windows.Forms.CheckBox();
			this.rbReadOnEndAlways = new System.Windows.Forms.RadioButton();
			this.rbReadOnEndNever = new System.Windows.Forms.RadioButton();
			this.rbReadOnEndAskMe = new System.Windows.Forms.RadioButton();
			this.groupReadOnEndWork = new System.Windows.Forms.GroupBox();
			this.checkBoxUpdateSearchFolder = new System.Windows.Forms.CheckBox();
			this.textBoxUpdateFolderTime = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupMessage.SuspendLayout();
			this.groupConfirm.SuspendLayout();
			this.groupReadOnEndWork.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupMessage
			// 
			resources.ApplyResources(this.groupMessage, "groupMessage");
			this.groupMessage.Controls.Add(this.checkBoxSignMessage);
			this.groupMessage.Controls.Add(this.checkBoxNext);
			this.groupMessage.Controls.Add(this.checkBoxPersonMessage);
			this.groupMessage.Controls.Add(this.timeout);
			this.groupMessage.Controls.Add(this.checkTimer);
			this.groupMessage.Controls.Add(this.label2);
			this.groupMessage.Controls.Add(this.checkNewMessageNotification);
			this.groupMessage.Name = "groupMessage";
			this.groupMessage.TabStop = false;
			// 
			// checkBoxSignMessage
			// 
			resources.ApplyResources(this.checkBoxSignMessage, "checkBoxSignMessage");
			this.checkBoxSignMessage.Name = "checkBoxSignMessage";
			// 
			// checkBoxNext
			// 
			resources.ApplyResources(this.checkBoxNext, "checkBoxNext");
			this.checkBoxNext.Name = "checkBoxNext";
			// 
			// checkBoxPersonMessage
			// 
			resources.ApplyResources(this.checkBoxPersonMessage, "checkBoxPersonMessage");
			this.checkBoxPersonMessage.Name = "checkBoxPersonMessage";
			// 
			// timeout
			// 
			resources.ApplyResources(this.timeout, "timeout");
			this.timeout.Name = "timeout";
			// 
			// checkTimer
			// 
			resources.ApplyResources(this.checkTimer, "checkTimer");
			this.checkTimer.Name = "checkTimer";
			this.checkTimer.CheckedChanged += new System.EventHandler(this.checkTimer_CheckedChanged);
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label2.Name = "label2";
			// 
			// checkNewMessageNotification
			// 
			resources.ApplyResources(this.checkNewMessageNotification, "checkNewMessageNotification");
			this.checkNewMessageNotification.Name = "checkNewMessageNotification";
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
			// groupConfirm
			// 
			resources.ApplyResources(this.groupConfirm, "groupConfirm");
			this.groupConfirm.Controls.Add(this.checkDelete);
			this.groupConfirm.Name = "groupConfirm";
			this.groupConfirm.TabStop = false;
			// 
			// checkDelete
			// 
			resources.ApplyResources(this.checkDelete, "checkDelete");
			this.checkDelete.Name = "checkDelete";
			// 
			// checkBoxScan
			// 
			resources.ApplyResources(this.checkBoxScan, "checkBoxScan");
			this.checkBoxScan.Name = "checkBoxScan";
			// 
			// checkBoxBar
			// 
			resources.ApplyResources(this.checkBoxBar, "checkBoxBar");
			this.checkBoxBar.Name = "checkBoxBar";
			this.checkBoxBar.CheckedChanged += new System.EventHandler(this.checkBoxBar_CheckedChanged);
			// 
			// rbReadOnEndAlways
			// 
			resources.ApplyResources(this.rbReadOnEndAlways, "rbReadOnEndAlways");
			this.rbReadOnEndAlways.Name = "rbReadOnEndAlways";
			this.rbReadOnEndAlways.TabStop = true;
			this.rbReadOnEndAlways.UseVisualStyleBackColor = true;
			// 
			// rbReadOnEndNever
			// 
			resources.ApplyResources(this.rbReadOnEndNever, "rbReadOnEndNever");
			this.rbReadOnEndNever.Name = "rbReadOnEndNever";
			this.rbReadOnEndNever.TabStop = true;
			this.rbReadOnEndNever.UseVisualStyleBackColor = true;
			// 
			// rbReadOnEndAskMe
			// 
			resources.ApplyResources(this.rbReadOnEndAskMe, "rbReadOnEndAskMe");
			this.rbReadOnEndAskMe.Name = "rbReadOnEndAskMe";
			this.rbReadOnEndAskMe.TabStop = true;
			this.rbReadOnEndAskMe.UseVisualStyleBackColor = true;
			// 
			// groupReadOnEndWork
			// 
			resources.ApplyResources(this.groupReadOnEndWork, "groupReadOnEndWork");
			this.groupReadOnEndWork.Controls.Add(this.rbReadOnEndAskMe);
			this.groupReadOnEndWork.Controls.Add(this.rbReadOnEndNever);
			this.groupReadOnEndWork.Controls.Add(this.rbReadOnEndAlways);
			this.groupReadOnEndWork.Name = "groupReadOnEndWork";
			this.groupReadOnEndWork.TabStop = false;
			// 
			// checkBoxUpdateSearchFolder
			// 
			resources.ApplyResources(this.checkBoxUpdateSearchFolder, "checkBoxUpdateSearchFolder");
			this.checkBoxUpdateSearchFolder.Checked = true;
			this.checkBoxUpdateSearchFolder.CheckState = System.Windows.Forms.CheckState.Indeterminate;
			this.checkBoxUpdateSearchFolder.Name = "checkBoxUpdateSearchFolder";
			this.checkBoxUpdateSearchFolder.UseVisualStyleBackColor = true;
			this.checkBoxUpdateSearchFolder.CheckedChanged += new System.EventHandler(this.checkBoxUpdateSearchFolder_CheckedChanged);
			// 
			// textBoxUpdateFolderTime
			// 
			resources.ApplyResources(this.textBoxUpdateFolderTime, "textBoxUpdateFolderTime");
			this.textBoxUpdateFolderTime.Name = "textBoxUpdateFolderTime";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// SettingsMessagesAndConfirmsDialog
			// 
			this.AcceptButton = this.buttonOK;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBoxUpdateFolderTime);
			this.Controls.Add(this.checkBoxUpdateSearchFolder);
			this.Controls.Add(this.groupReadOnEndWork);
			this.Controls.Add(this.checkBoxBar);
			this.Controls.Add(this.checkBoxScan);
			this.Controls.Add(this.groupMessage);
			this.Controls.Add(this.groupConfirm);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SettingsMessagesAndConfirmsDialog";
			this.Load += new System.EventHandler(this.SettingsMessagesDialog_Load);
			this.groupMessage.ResumeLayout(false);
			this.groupMessage.PerformLayout();
			this.groupConfirm.ResumeLayout(false);
			this.groupReadOnEndWork.ResumeLayout(false);
			this.groupReadOnEndWork.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private Button buttonOK;
		private Button buttonCancel;
		private Label label2;
		private CheckBox checkNewMessageNotification;
		private TextBox timeout;
		private GroupBox groupMessage;
		private GroupBox groupConfirm;
		private CheckBox checkDelete;
		private CheckBox checkTimer;
		private CheckBox checkBoxPersonMessage;
		private CheckBox checkBoxScan;
		private CheckBox checkBoxNext;
		private CheckBox checkBoxBar;
		private CheckBox checkBoxSignMessage;
		private RadioButton rbReadOnEndAlways;
		private RadioButton rbReadOnEndNever;
		private RadioButton rbReadOnEndAskMe;
		private GroupBox groupReadOnEndWork;
		private Container components;
		private CheckBox checkBoxUpdateSearchFolder;
		private TextBox textBoxUpdateFolderTime;
		private Label label1;

	}
}