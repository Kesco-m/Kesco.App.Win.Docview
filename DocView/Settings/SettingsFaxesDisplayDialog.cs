using System;
using System.ComponentModel;
using System.Windows.Forms;
using Kesco.Lib.Win;

namespace Kesco.App.Win.DocView.Settings
{
    public class SettingsFaxesDisplayDialog : FreeDialog
    {
        private Button buttonOK;
        private Button buttonCancel;
        private CheckBox checkFaxesInDisplay;
        private CheckBox checkFaxesOutDisplay;

        private Container components;

        public SettingsFaxesDisplayDialog()
        {
            InitializeComponent();
        }

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

        #region Windows Form Designer generated code

        /// <summary>
        ///   Required method for Designer support - do not modify the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsFaxesDisplayDialog));
			this.checkFaxesInDisplay = new System.Windows.Forms.CheckBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.checkFaxesOutDisplay = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// checkFaxesInDisplay
			// 
			resources.ApplyResources(this.checkFaxesInDisplay, "checkFaxesInDisplay");
			this.checkFaxesInDisplay.Name = "checkFaxesInDisplay";
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
			// checkFaxesOutDisplay
			// 
			resources.ApplyResources(this.checkFaxesOutDisplay, "checkFaxesOutDisplay");
			this.checkFaxesOutDisplay.Name = "checkFaxesOutDisplay";
			// 
			// SettingsFaxesDisplayDialog
			// 
			this.AcceptButton = this.buttonOK;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.checkFaxesInDisplay);
			this.Controls.Add(this.checkFaxesOutDisplay);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SettingsFaxesDisplayDialog";
			this.Load += new System.EventHandler(this.SettingsMessagesDialog_Load);
			this.ResumeLayout(false);

        }

        #endregion

        private void SettingsMessagesDialog_Load(object sender, EventArgs e)
        {
            checkFaxesInDisplay.Checked = Environment.UserSettings.FaxesInUnsavedOnly;
            checkFaxesOutDisplay.Checked = Environment.UserSettings.FaxesOutUnsavedOnly;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Environment.UserSettings.FaxesInUnsavedOnly = checkFaxesInDisplay.Checked;
            Environment.UserSettings.FaxesOutUnsavedOnly = checkFaxesOutDisplay.Checked;

            End(DialogResult.OK);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            End(DialogResult.Cancel);
        }
    }
}