using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Kesco.Lib.Win;
using Kesco.Lib.Win.Document;

namespace Kesco.App.Win.DocView.Settings
{
    public class SettingsGroupOrderDialog : FreeDialog
    {
        private SynchronizedCollection<KeyValuePair<string, string>> groupNames;
        private string goBuf;

        private Label label1;
        private ComboBox combo1;
        private Label label2;
        private ComboBox combo2;
        private Button buttonOK;
        private Button buttonCancel;
        private CheckBox checkBoxSubLevels;

        private Container components;

        public SettingsGroupOrderDialog()
        {
            InitializeComponent();

            groupNames = new SynchronizedCollection<KeyValuePair<string, string>>
                             {
                                 new KeyValuePair<string, string>("L",
                                                                  Environment.PersonWord.GetForm(Cases.I, false, true)),
                                 new KeyValuePair<string, string>("T",
                                                                  Environment.StringResources.GetString("DocumentType"))
                             };

            goBuf = Environment.UserSettings.GroupOrder;
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
            var resources =
                new System.ComponentModel.ComponentResourceManager(typeof (SettingsGroupOrderDialog));
            this.label1 = new System.Windows.Forms.Label();
            this.combo1 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.combo2 = new System.Windows.Forms.ComboBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.checkBoxSubLevels = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label1.Name = "label1";
            // 
            // combo1
            // 
            resources.ApplyResources(this.combo1, "combo1");
            this.combo1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo1.Name = "combo1";
            this.combo1.SelectedIndexChanged += new System.EventHandler(this.combo1_SelectedIndexChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label2.Name = "label2";
            // 
            // combo2
            // 
            resources.ApplyResources(this.combo2, "combo2");
            this.combo2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo2.Name = "combo2";
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
            // checkBoxSubLevels
            // 
            resources.ApplyResources(this.checkBoxSubLevels, "checkBoxSubLevels");
            this.checkBoxSubLevels.Name = "checkBoxSubLevels";
            // 
            // SettingsGroupOrderDialog
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.checkBoxSubLevels);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.combo1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.combo2);
            this.Controls.Add(this.buttonCancel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsGroupOrderDialog";

            this.Load += new System.EventHandler(this.SettingsGroupOrderDialog_Load);
            this.ResumeLayout(false);
        }

        #endregion

        private void SettingsGroupOrderDialog_Load(object sender, EventArgs e)
        {
            try
            {
                // filling combo1
                foreach (KeyValuePair<string, string> t in groupNames)
                {
                    string key = t.Key;
                    combo1.Items.Add(t.Value);
                    if (goBuf != null && goBuf.Length >= 1)
                    {
                        // 1st combo check
                        if (goBuf.Substring(0, 1) == key)
                            combo1.SelectedIndex = combo1.Items.Count - 1;
                    }
                }

                checkBoxSubLevels.Checked = Environment.UserSettings.SubLevelDocs;
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        private void combo1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                combo2.Items.Clear();
                combo2.Items.Add("[" + Environment.StringResources.GetString("Absent") + "]");

                // filling combo2

                foreach (KeyValuePair<string, string> t in groupNames.Where(t => combo1.Text != t.Value))
                {
                    combo2.Items.Add(t.Value);
                    if (goBuf != null && goBuf.Length >= 2)
                    {
                        // 2nd combo check
                        if (goBuf.Substring(1, 1) == t.Key)
                            combo2.SelectedIndex = combo2.Items.Count - 1;
                    }
                }

                if (combo2.Text == "")
                    combo2.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            string first = "";
            string second = "";
            try
            {
                foreach (KeyValuePair<string, string> t in groupNames)
                {
                    if (combo1.Text == t.Value)
                        first = t.Key;
                    if (combo2.Text == t.Value)
                        second = t.Key;
                }

                Environment.UserSettings.GroupOrder = first + second;
                Environment.UserSettings.SubLevelDocs = checkBoxSubLevels.Checked;
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
            End(DialogResult.OK);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            End(DialogResult.Cancel);
        }
    }
}