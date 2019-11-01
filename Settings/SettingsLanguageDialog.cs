using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using Kesco.Lib.Win;

namespace Kesco.App.Win.DocView.Settings
{
    public class SettingsLanguageDialog : FreeDialog
    {
        private Button buttonOK;
        private Button buttonCancel;
        private ComboBox comboBoxLanguage;

        private Container components;

        public SettingsLanguageDialog()
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsLanguageDialog));
			this.comboBoxLanguage = new System.Windows.Forms.ComboBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// comboBoxLanguage
			// 
			resources.ApplyResources(this.comboBoxLanguage, "comboBoxLanguage");
			this.comboBoxLanguage.Name = "comboBoxLanguage";
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
			// SettingsLanguageDialog
			// 
			this.AcceptButton = this.buttonOK;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.comboBoxLanguage);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SettingsLanguageDialog";
			this.ShowInTaskbar = false;
			this.Load += new System.EventHandler(this.SettingsLanguage_Load);
			this.ResumeLayout(false);

        }

        #endregion

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            End(DialogResult.Cancel);
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (comboBoxLanguage.SelectedItem != null)
                Environment.LangData.ChangeLanguage(comboBoxLanguage.SelectedValue.ToString());
            End(DialogResult.OK);
        }

        private void SettingsLanguage_Load(object sender, EventArgs e)
        {
            DataTable dt = Environment.LangData.GetLanguages();
            if (dt != null)
            {
                comboBoxLanguage.DataSource = dt;
                comboBoxLanguage.DisplayMember = Environment.LangData.NameField;
                comboBoxLanguage.ValueMember = Environment.LangData.IDField;
                comboBoxLanguage.SelectedValue = Environment.CurCultureInfo.TwoLetterISOLanguageName;
            }
            else
                Close();
        }
    }
}