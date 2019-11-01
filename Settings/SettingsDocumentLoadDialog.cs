using System;
using System.ComponentModel;
using System.Windows.Forms;
using Kesco.Lib.Win;

namespace Kesco.App.Win.DocView.Settings
{
    public class SettingsDocumentLoadDialog : FreeDialog
	{
        private Button buttonOK;
		private Button buttonCancel;
        private GroupBox groupBoxMainWindow;
        private RadioButton radioButtonMainLeft;
        private RadioButton radioButtonMainBetween;
        private RadioButton radioButtonMainUnder;
        private GroupBox groupBoxSubWindow;
        private RadioButton radioButtonSubUnder;
        private RadioButton radioButtonSubWindow;
		private RadioButton radioButtonSubLeft;
		private CheckBox checkBoxMessageFirst;
        private IContainer components;

        public SettingsDocumentLoadDialog()
        {
            InitializeComponent();
            LoadSettings();
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

        #region Designer generated code

        /// <summary>
        ///   Required method for Designer support - do not modify the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsDocumentLoadDialog));
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.groupBoxMainWindow = new System.Windows.Forms.GroupBox();
			this.radioButtonMainUnder = new System.Windows.Forms.RadioButton();
			this.radioButtonMainBetween = new System.Windows.Forms.RadioButton();
			this.radioButtonMainLeft = new System.Windows.Forms.RadioButton();
			this.groupBoxSubWindow = new System.Windows.Forms.GroupBox();
			this.radioButtonSubUnder = new System.Windows.Forms.RadioButton();
			this.radioButtonSubWindow = new System.Windows.Forms.RadioButton();
			this.radioButtonSubLeft = new System.Windows.Forms.RadioButton();
			this.checkBoxMessageFirst = new System.Windows.Forms.CheckBox();
			this.groupBoxMainWindow.SuspendLayout();
			this.groupBoxSubWindow.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonOK
			// 
			resources.ApplyResources(this.buttonOK, "buttonOK");
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
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
			// groupBoxMainWindow
			// 
			resources.ApplyResources(this.groupBoxMainWindow, "groupBoxMainWindow");
			this.groupBoxMainWindow.Controls.Add(this.radioButtonMainUnder);
			this.groupBoxMainWindow.Controls.Add(this.radioButtonMainBetween);
			this.groupBoxMainWindow.Controls.Add(this.radioButtonMainLeft);
			this.groupBoxMainWindow.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBoxMainWindow.Name = "groupBoxMainWindow";
			this.groupBoxMainWindow.TabStop = false;
			// 
			// radioButtonMainUnder
			// 
			resources.ApplyResources(this.radioButtonMainUnder, "radioButtonMainUnder");
			this.radioButtonMainUnder.Name = "radioButtonMainUnder";
			// 
			// radioButtonMainBetween
			// 
			resources.ApplyResources(this.radioButtonMainBetween, "radioButtonMainBetween");
			this.radioButtonMainBetween.Name = "radioButtonMainBetween";
			// 
			// radioButtonMainLeft
			// 
			resources.ApplyResources(this.radioButtonMainLeft, "radioButtonMainLeft");
			this.radioButtonMainLeft.Name = "radioButtonMainLeft";
			// 
			// groupBoxSubWindow
			// 
			resources.ApplyResources(this.groupBoxSubWindow, "groupBoxSubWindow");
			this.groupBoxSubWindow.Controls.Add(this.radioButtonSubUnder);
			this.groupBoxSubWindow.Controls.Add(this.radioButtonSubWindow);
			this.groupBoxSubWindow.Controls.Add(this.radioButtonSubLeft);
			this.groupBoxSubWindow.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBoxSubWindow.Name = "groupBoxSubWindow";
			this.groupBoxSubWindow.TabStop = false;
			// 
			// radioButtonSubUnder
			// 
			resources.ApplyResources(this.radioButtonSubUnder, "radioButtonSubUnder");
			this.radioButtonSubUnder.Name = "radioButtonSubUnder";
			// 
			// radioButtonSubWindow
			// 
			resources.ApplyResources(this.radioButtonSubWindow, "radioButtonSubWindow");
			this.radioButtonSubWindow.Name = "radioButtonSubWindow";
			// 
			// radioButtonSubLeft
			// 
			resources.ApplyResources(this.radioButtonSubLeft, "radioButtonSubLeft");
			this.radioButtonSubLeft.Name = "radioButtonSubLeft";
			// 
			// checkBoxMessageFirst
			// 
			resources.ApplyResources(this.checkBoxMessageFirst, "checkBoxMessageFirst");
			this.checkBoxMessageFirst.Name = "checkBoxMessageFirst";
			// 
			// SettingsDocumentLoadDialog
			// 
			this.AcceptButton = this.buttonOK;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.checkBoxMessageFirst);
			this.Controls.Add(this.groupBoxSubWindow);
			this.Controls.Add(this.groupBoxMainWindow);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SettingsDocumentLoadDialog";
			this.ShowInTaskbar = false;
			this.TransparencyKey = System.Drawing.Color.Red;
			this.Load += new System.EventHandler(this.SettingsDocumentLoadDialog_Load);
			this.groupBoxMainWindow.ResumeLayout(false);
			this.groupBoxSubWindow.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private void buttonOK_Click(object sender, EventArgs e)
        {
            SaveSettings();
            Close();
        }

        private void SettingsDocumentLoadDialog_Load(object sender, EventArgs e)
        {
        }

        private void LoadSettings()
        {
			//int showID = Environment.General.LoadIntOption("ShowDocAndMessage", 0);
			int mainID = Environment.General.LoadIntOption("ShowInMainWindow", 0);
            int subID = Environment.General.LoadIntOption("ShowInSubWindow", 0);

			//switch (showID)
			//{
			//    case 0:
			//        radioButtonAll.Checked = true;
			//        break;
			//    case 1:
			//        radioButtonImage.Checked = true;
			//        break;
			//    case 2:
			//        radioButtonMessage.Checked = true;
			//        break;
			//    default:
			//        radioButtonAll.Checked = true;
			//        break;
			//}

            switch (mainID)
            {
                case 0:
                    radioButtonMainBetween.Checked = true;
                    break;
                case 1:
                    radioButtonMainLeft.Checked = true;
                    break;
                case 2:
                    radioButtonMainUnder.Checked = true;
                    break;
                default:
                    radioButtonMainBetween.Checked = true;
                    break;
            }

            switch (subID)
            {
                case 0:
                    radioButtonSubWindow.Checked = true;
                    break;
                case 1:
                    radioButtonSubLeft.Checked = true;
                    break;
                case 2:
                    radioButtonSubUnder.Checked = true;
                    break;
                default:
                    radioButtonSubWindow.Checked = true;
                    break;
            }

            checkBoxMessageFirst.Checked = Environment.LoadMessageFirst;
        }

        private void SaveSettings()
        {
            int showID = 0;
            int mainID = 0;
            int subID = 0;

			//if (radioButtonMessage.Checked)
			//    showID = 2;
			//else if (radioButtonImage.Checked)
			//    showID = 1;

            if (radioButtonMainUnder.Checked)
                mainID = 2;
            else if (radioButtonMainLeft.Checked)
                mainID = 1;

            if (radioButtonSubUnder.Checked)
                subID = 2;
            else if (radioButtonSubLeft.Checked)
                subID = 1;

			//Environment.General.Option("ShowDocAndMessage").Value = showID;
            Environment.General.Option("ShowInMainWindow").Value = mainID;
            Environment.General.Option("ShowInSubWindow").Value = subID;
            Environment.General.Save();
            Environment.LoadMessageFirst = checkBoxMessageFirst.Checked;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
		}
    }
}