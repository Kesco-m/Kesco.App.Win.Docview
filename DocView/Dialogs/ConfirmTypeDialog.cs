using System;
using System.ComponentModel;
using System.Windows.Forms;
using Kesco.Lib.Win;
using Kesco.Lib.Win.Document.Select;

namespace Kesco.App.Win.DocView.Dialogs
{
    public class ConfirmTypeDialog : FreeDialog
    {
        private Button buttonNo;
        private Button buttonYes;
        private Label labelText;
        private string typeStr;
        private Button buttonSearch;

        private Container components;

        public ConfirmTypeDialog()
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
            var resources =
                new System.ComponentModel.ComponentResourceManager(typeof (ConfirmTypeDialog));
            this.buttonSearch = new System.Windows.Forms.Button();
            this.labelText = new System.Windows.Forms.Label();
            this.buttonNo = new System.Windows.Forms.Button();
            this.buttonYes = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonSearch
            // 
            resources.ApplyResources(this.buttonSearch, "buttonSearch");
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
            // 
            // labelText
            // 
            resources.ApplyResources(this.labelText, "labelText");
            this.labelText.Name = "labelText";
            // 
            // buttonNo
            // 
            resources.ApplyResources(this.buttonNo, "buttonNo");
            this.buttonNo.DialogResult = System.Windows.Forms.DialogResult.No;
            this.buttonNo.Name = "buttonNo";
            this.buttonNo.Click += new System.EventHandler(this.buttonNo_Click);
            // 
            // buttonYes
            // 
            resources.ApplyResources(this.buttonYes, "buttonYes");
            this.buttonYes.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.buttonYes.Name = "buttonYes";
            this.buttonYes.Click += new System.EventHandler(this.buttonYes_Click);
            // 
            // ConfirmTypeDialog
            // 
            this.AcceptButton = this.buttonNo;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonNo;
            this.Controls.Add(this.buttonYes);
            this.Controls.Add(this.buttonNo);
            this.Controls.Add(this.labelText);
            this.Controls.Add(this.buttonSearch);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfirmTypeDialog";
            this.ShowInTaskbar = false;
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ConfirmTypeDialog_KeyUp);
            this.ResumeLayout(false);
        }

        #endregion

        public string TypeString
        {
            get { return typeStr; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    typeStr = value;
                labelText.Text = Environment.StringResources.GetString("ConfirmTypeDialog.TypeString.Message1") +
                                 typeStr + ".\n" +
                                 Environment.StringResources.GetString("ConfirmTypeDialog.TypeString.Message2");
            }
        }

        public int TypeID { get; set; }

        public int DocID { get; set; }

        public int FieldID { get; set; }

        public string SearchString { get; set; }

        private void buttonNo_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonYes_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ConfirmTypeDialog_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.L:
                case Keys.Y:
                case Keys.G:
                    Close();
                    break;
            }
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            var dialog = new SelectDocUniversalDialog(SearchString, null) {Owner = this};
            dialog.Show();
        }
    }
}