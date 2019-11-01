using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Kesco.App.Win.DocView.Dialogs
{
    /// <summary>
    /// Диалог для ввода нового имени каталога
    /// </summary>
    public class RenameFolderDialog : Form
    {
        private Label label1;
        private TextBox tbNewName;
        private Button btnOk;
        private Button btnCancel;
        private Container components;

        public RenameFolderDialog(string oldName)
        {
            InitializeComponent();
            tbNewName.Text = oldName;
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
			this.label1 = new System.Windows.Forms.Label();
			this.tbNewName = new System.Windows.Forms.TextBox();
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(224, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Переименовать текущую папку в :";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tbNewName
			// 
			this.tbNewName.Location = new System.Drawing.Point(8, 32);
			this.tbNewName.Name = "tbNewName";
			this.tbNewName.Size = new System.Drawing.Size(312, 20);
			this.tbNewName.TabIndex = 1;
			// 
			// btnOk
			// 
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Location = new System.Drawing.Point(165, 59);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 2;
			this.btnOk.Text = "Ок";
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(245, 59);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "Отмена";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// RenameFolderDialog
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(330, 88);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.tbNewName);
			this.Controls.Add(this.label1);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "RenameFolderDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Запрос пользователю";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        public string FolderName
        {
            get { return tbNewName.Text; }
        }
    }
}