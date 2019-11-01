using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Kesco.Lib.Win;

namespace Kesco.App.Win.DocView.Dialogs
{
    public class DropDownForm : FreeDialog
    {
        private IContainer components;
        internal Rectangle rect;

        public DropDownForm() : this(new Rectangle(0, 0, 0, 0))
        {
        }

        public DropDownForm(Rectangle rect)
        {
            this.rect = rect;

            InitializeComponent();
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
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
            var resources = new System.Resources.ResourceManager(typeof (DropDownForm));
            // 
            // DropDownForm
            // 
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuPopup;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.ControlBox = false;
            this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DropDownForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.DropDownForm_Paint);
            this.Deactivate += new System.EventHandler(this.DropDownForm_Deactivate);
        }

        #endregion

        internal virtual Size GetMaxSize()
        {
            return Size;
        }

        internal virtual bool CanClose()
        {
            return true;
        }

        internal void DropDown()
        {
            if (DesignMode)
                return;

            var screenRect = Screen.FromControl(this).WorkingArea;
            Size = GetMaxSize();
            var locationX = (screenRect.Width - rect.X - Size.Width > 0 || screenRect.Width/2 > rect.X)
                                ? rect.X
                                : (screenRect.Width - Size.Width - 1);
            var locationY = (screenRect.Height - rect.Bottom - Size.Height > 0 || screenRect.Height/2 > rect.Bottom)
                                ? (rect.Bottom + rect.Height + 3)
								: (rect.Bottom - Size.Height);
            Location = new Point(locationX, locationY);
        }

        private void DropDownForm_Deactivate(object sender, EventArgs e)
        {
            if (!CanClose())
                return;
            if (Owner != null)
                Owner.Activate();
            Close();
        }

        private void DropDownForm_Paint(object sender, PaintEventArgs e)
        {
            var myPen = new Pen(Color.Black, 10);
            var myRectangle = new Rectangle(0, 0, Width - 1, Height - 1);
            e.Graphics.DrawRectangle(myPen, myRectangle);
        }
    }
}