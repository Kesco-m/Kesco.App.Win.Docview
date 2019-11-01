using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Kesco.Lib.Win;
using Kesco.Lib.Win.Document;

namespace Kesco.App.Win.DocView.PropertiesDialogs
{
    public class PropertiesScanDialog : FreeDialog
    {
        private Label scanDate;
        private Label label1;
        private TextBox descr;
        private Button buttonSave;
        private Button buttonCancel;

        private Container components;

        private FileInfo f;
        private ScanInfo info;

        public PropertiesScanDialog(string fullFileName)
        {
            InitializeComponent();

            f = new FileInfo(fullFileName);
            info = TextProcessor.ParseScanInfo(f);

            if (info != null)
            {
                scanDate.Text += info.Date.ToString();
                descr.Text = info.Descr;
            }
            else
            {
                DateTime date = f.CreationTime;
                scanDate.Text += date.ToString();
                info = new ScanInfo(date, "");
            }
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
                new System.ComponentModel.ComponentResourceManager(typeof (PropertiesScanDialog));
            this.scanDate = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.descr = new System.Windows.Forms.TextBox();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // scanDate
            // 
            resources.ApplyResources(this.scanDate, "scanDate");
            this.scanDate.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.scanDate.Name = "scanDate";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label1.Name = "label1";
            // 
            // descr
            // 
            resources.ApplyResources(this.descr, "descr");
            this.descr.Name = "descr";
            // 
            // buttonSave
            // 
            resources.ApplyResources(this.buttonSave, "buttonSave");
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // PropertiesScanDialog
            // 
            this.AcceptButton = this.buttonSave;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.descr);
            this.Controls.Add(this.scanDate);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PropertiesScanDialog";

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (descr.Text.IndexOfAny("\\/:*?\"><|".ToCharArray()) > -1)
            {
                MessageBox.Show(
                    Environment.StringResources.GetString("Properties.PropertiesScanDialog.buttonSave_Click.Message1") +
                    ":" + System.Environment.NewLine + "\\ / : * ? \" > < |",
                    Environment.StringResources.GetString("Error"));
                return;
            }

            info.Descr = descr.Text;
            string newName = f.DirectoryName + @"\" + TextProcessor.FormScanFileName(f, info);
            if (!f.FullName.Equals(newName, StringComparison.CurrentCultureIgnoreCase))
                try
                {
                    File.Move(f.FullName,
                              f.DirectoryName + @"\" + TextProcessor.FormScanFileName(f, info));
                }
                catch (IOException)
                {
                } // Возможны сетевые задержки при репликации

            End(DialogResult.OK);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            End(DialogResult.Cancel);
        }
    }
}