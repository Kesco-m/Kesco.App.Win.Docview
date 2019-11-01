using System;
using System.ComponentModel;
using System.Windows.Forms;
using Kesco.Lib.Win;

namespace Kesco.App.Win.DocView.Settings
{
    public class SettingsDateFiltersDialog : FreeDialog
    {
        private RadioButton[] docDate;
        private RadioButton radioButtonDocThisYear;
        private RadioButton radioButtonDocThisQuarter;
        private RadioButton radioButtonDocThisMonth;
        private RadioButton radioButtonDocLastYear;
        private RadioButton radioButtonDocLastQuarter;
        private RadioButton radioButtonDocLastMonth;
        private Button buttonOK;
        private Button buttonCancel;
        private GroupBox groupBoxDoc;
        private RadioButton radioButtonDocAll;

        private Container components;

        public SettingsDateFiltersDialog()
        {
            InitializeComponent();

            // docDate
            docDate = new RadioButton[7];

            docDate[0] = radioButtonDocAll;
            docDate[1] = radioButtonDocThisQuarter;
            docDate[2] = radioButtonDocLastQuarter;
            docDate[3] = radioButtonDocThisMonth;
            docDate[4] = radioButtonDocLastMonth;
            docDate[5] = radioButtonDocThisYear;
            docDate[6] = radioButtonDocLastYear;

            for (int i = 0; i < docDate.Length; i++)
                docDate[i].Tag = i;

            docDate[Environment.UserSettings.FilterDocDate].Checked = true;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsDateFiltersDialog));
            this.groupBoxDoc = new System.Windows.Forms.GroupBox();
            this.radioButtonDocThisYear = new System.Windows.Forms.RadioButton();
            this.radioButtonDocThisQuarter = new System.Windows.Forms.RadioButton();
            this.radioButtonDocThisMonth = new System.Windows.Forms.RadioButton();
            this.radioButtonDocLastYear = new System.Windows.Forms.RadioButton();
            this.radioButtonDocLastQuarter = new System.Windows.Forms.RadioButton();
            this.radioButtonDocLastMonth = new System.Windows.Forms.RadioButton();
            this.radioButtonDocAll = new System.Windows.Forms.RadioButton();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBoxDoc.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxDoc
            // 
            this.groupBoxDoc.Controls.Add(this.radioButtonDocThisYear);
            this.groupBoxDoc.Controls.Add(this.radioButtonDocThisQuarter);
            this.groupBoxDoc.Controls.Add(this.radioButtonDocThisMonth);
            this.groupBoxDoc.Controls.Add(this.radioButtonDocLastYear);
            this.groupBoxDoc.Controls.Add(this.radioButtonDocLastQuarter);
            this.groupBoxDoc.Controls.Add(this.radioButtonDocLastMonth);
            this.groupBoxDoc.Controls.Add(this.radioButtonDocAll);
            this.groupBoxDoc.FlatStyle = System.Windows.Forms.FlatStyle.System;
            resources.ApplyResources(this.groupBoxDoc, "groupBoxDoc");
            this.groupBoxDoc.Name = "groupBoxDoc";
            this.groupBoxDoc.TabStop = false;
            // 
            // radioButtonDocThisYear
            // 
            resources.ApplyResources(this.radioButtonDocThisYear, "radioButtonDocThisYear");
            this.radioButtonDocThisYear.Name = "radioButtonDocThisYear";
            // 
            // radioButtonDocThisQuarter
            // 
            resources.ApplyResources(this.radioButtonDocThisQuarter, "radioButtonDocThisQuarter");
            this.radioButtonDocThisQuarter.Name = "radioButtonDocThisQuarter";
            // 
            // radioButtonDocThisMonth
            // 
            resources.ApplyResources(this.radioButtonDocThisMonth, "radioButtonDocThisMonth");
            this.radioButtonDocThisMonth.Name = "radioButtonDocThisMonth";
            // 
            // radioButtonDocLastYear
            // 
            resources.ApplyResources(this.radioButtonDocLastYear, "radioButtonDocLastYear");
            this.radioButtonDocLastYear.Name = "radioButtonDocLastYear";
            // 
            // radioButtonDocLastQuarter
            // 
            resources.ApplyResources(this.radioButtonDocLastQuarter, "radioButtonDocLastQuarter");
            this.radioButtonDocLastQuarter.Name = "radioButtonDocLastQuarter";
            // 
            // radioButtonDocLastMonth
            // 
            resources.ApplyResources(this.radioButtonDocLastMonth, "radioButtonDocLastMonth");
            this.radioButtonDocLastMonth.Name = "radioButtonDocLastMonth";
            // 
            // radioButtonDocAll
            // 
            resources.ApplyResources(this.radioButtonDocAll, "radioButtonDocAll");
            this.radioButtonDocAll.Name = "radioButtonDocAll";
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
            // SettingsDateFiltersDialog
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBoxDoc);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsDateFiltersDialog";
            this.groupBoxDoc.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private void buttonOK_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < docDate.Length; i++)
                if (docDate[i].Checked)
                {
                    Environment.UserSettings.FilterDocDate = i;
                    break;
                }

            End(DialogResult.OK);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            End(DialogResult.Cancel);
        }
    }
}