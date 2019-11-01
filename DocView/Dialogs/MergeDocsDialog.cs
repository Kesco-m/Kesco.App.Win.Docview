using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using Kesco.Lib.Win;
using Kesco.Lib.Win.Document;

namespace Kesco.App.Win.DocView.Dialogs
{
    /// <summary>
    /// Диалог выбора документов для слияния
    /// </summary>
    public class MergeDocsDialog : FreeDialog
    {
        private int firstDocID;
        private int secondDocID;

        private Label label;
        private RadioButton radioFirstDoc;
        private RadioButton radioSecondDoc;
        private Button buttonOK;
        private Button buttonCancel;

        private Container components;

        public MergeDocsDialog(int firstDocID, int secondDocID)
        {
            InitializeComponent();

            this.firstDocID = firstDocID;
            this.secondDocID = secondDocID;

            radioFirstDoc.Text = DBDocString.Format(firstDocID);
            radioSecondDoc.Text = DBDocString.Format(secondDocID);
        }

        #region Accessors

        public int MainDocID { get; private set; }

        public int SecDocID { get; private set; }

        #endregion

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
                new System.ComponentModel.ComponentResourceManager(typeof (MergeDocsDialog));
            this.label = new System.Windows.Forms.Label();
            this.radioFirstDoc = new System.Windows.Forms.RadioButton();
            this.radioSecondDoc = new System.Windows.Forms.RadioButton();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label
            // 
            resources.ApplyResources(this.label, "label");
            this.label.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label.Name = "label";
            // 
            // radioFirstDoc
            // 
            resources.ApplyResources(this.radioFirstDoc, "radioFirstDoc");
            this.radioFirstDoc.Checked = true;
            this.radioFirstDoc.Name = "radioFirstDoc";
            this.radioFirstDoc.TabStop = true;
            // 
            // radioSecondDoc
            // 
            resources.ApplyResources(this.radioSecondDoc, "radioSecondDoc");
            this.radioSecondDoc.Name = "radioSecondDoc";
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
            // MergeDocsDialog
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.radioFirstDoc);
            this.Controls.Add(this.label);
            this.Controls.Add(this.radioSecondDoc);
            this.Controls.Add(this.buttonCancel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MergeDocsDialog";
            this.ResumeLayout(false);
        }

        #endregion

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (radioFirstDoc.Checked)
            {
                MainDocID = secondDocID;
                SecDocID = firstDocID;
            }
            else
            {
                MainDocID = firstDocID;
                SecDocID = secondDocID;
            }

            // checking data...
            bool mainDocData = Environment.DocDataData.IsDataPresent(MainDocID);
            bool secDocData = Environment.DocDataData.IsDataPresent(SecDocID);

            // checking types
            int mainDocTypeID;
            int secDocTypeID;

            DataRow dr = Environment.DocData.GetDocProperties(MainDocID, Environment.CurCultureInfo.Name);
            mainDocTypeID = (int) dr[Environment.DocData.DocTypeIDField];

            dr = Environment.DocData.GetDocProperties(SecDocID, Environment.CurCultureInfo.Name);
            secDocTypeID = (int) dr[Environment.DocData.DocTypeIDField];

            if (secDocData)
            {
                if (mainDocData || (mainDocTypeID != secDocTypeID))
                {
                    string message = Environment.StringResources.GetString("MergeDocsDialog.buttonOK_Click.Message1") +
                                     System.Environment.NewLine
                                     + Environment.StringResources.GetString("MergeDocsDialog.buttonOK_Click.Message2") +
                                     ":" + System.Environment.NewLine + System.Environment.NewLine;
                    if (mainDocData)
                        message += Environment.StringResources.GetString("MergeDocsDialog.buttonOK_Click.Message3") +
                                   System.Environment.NewLine;
                    if (mainDocTypeID != secDocTypeID)
                        message += Environment.StringResources.GetString("MergeDocsDialog.buttonOK_Click.Message4") +
                                   System.Environment.NewLine;
                    message += System.Environment.NewLine + Environment.StringResources.GetString("Continue");

                    if (
                        MessageBox.Show(message,
                                        Environment.StringResources.GetString("MergeDocsDialog.buttonOK_Click.Title1"),
                                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question,
                                        MessageBoxDefaultButton.Button2) == DialogResult.No)
                        return;
                }
            }
            End(DialogResult.OK);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            End(DialogResult.Cancel);
        }
    }
}