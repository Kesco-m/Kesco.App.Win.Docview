using System.ComponentModel;
using System.Windows.Forms;

namespace Kesco.App.Win.DocView.PropertiesDialogs.FaxItem
{
    public class FaxPropertyControl : UserControl
    {
        private Label labelName;
        internal Label labelValue;
        private Container components;

        public FaxPropertyControl()
        {
            InitializeComponent();
        }

        public FaxPropertyControl(string propertyName, string propertyValue)
            : this()
        {
            labelName.Text = propertyName;
            labelValue.Text = propertyValue;
            Height = labelValue.Height + 1;
        }

        public FaxPropertyControl(string propertyName, string propertyValue, object obj)
            : this(propertyName, propertyValue)
        {
            Obj = obj;
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

        #region Component Designer generated code

        /// <summary>
        ///   Required method for Designer support - do not modify the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelName = new System.Windows.Forms.Label();
            this.labelValue = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelName
            // 
            this.labelName.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelName.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.labelName.Location = new System.Drawing.Point(0, 0);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(128, 16);
            this.labelName.TabIndex = 0;
            this.labelName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelValue
            // 
            this.labelValue.AutoSize = true;
            this.labelValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelValue.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.labelValue.Location = new System.Drawing.Point(128, 0);
            this.labelValue.Name = "labelValue";
            this.labelValue.Size = new System.Drawing.Size(0, 13);
            this.labelValue.TabIndex = 1;
            this.labelValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FaxPropertyControl
            // 
            this.Controls.Add(this.labelValue);
            this.Controls.Add(this.labelName);
            this.DoubleBuffered = true;
            this.Name = "FaxPropertyControl";
            this.Size = new System.Drawing.Size(288, 16);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Accessors

        public string PropertyName
        {
            get { return labelName.Text; }
            set { labelName.Text = value; }
        }

        public string Value
        {
            get { return labelValue.Text; }
            set { labelValue.Text = value; }
        }

        public object Obj { get; set; }

        #endregion
    }
}