using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Kesco.Lib.Win.Error;

namespace Kesco.App.Win.DocView.Forms
{
    /// <summary>
    /// Окно, всплывающее при запуске программы
    /// </summary>
    public class SplashForm : Form
    {
        private PictureBox cabinet;
        private Label label1;

        private Container components;

        public SplashForm()
        {
            ErrorShower.StartErrorShow += ErrorShower_StartErrorShow;
            ErrorShower.ErrorShowEnd += ErrorShower_ErrorShowEnd;
            InitializeComponent();
            CenterToScreen();
            if (ErrorShower.ErrorShown)
                TopMost = false;
        }

        protected override void Dispose(bool disposing)
        {
            ErrorShower.StartErrorShow -= ErrorShower_StartErrorShow;
            ErrorShower.ErrorShowEnd -= ErrorShower_ErrorShowEnd;

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashForm));
			this.cabinet = new System.Windows.Forms.PictureBox();
			this.label1 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.cabinet)).BeginInit();
			this.SuspendLayout();
			// 
			// cabinet
			// 
			resources.ApplyResources(this.cabinet, "cabinet");
			this.cabinet.Name = "cabinet";
			this.cabinet.TabStop = false;
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// SplashForm
			// 
			resources.ApplyResources(this, "$this");
			this.BackColor = System.Drawing.SystemColors.Window;
			this.ControlBox = false;
			this.Controls.Add(this.label1);
			this.Controls.Add(this.cabinet);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SplashForm";
			this.ShowInTaskbar = false;
			this.TopMost = true;
			((System.ComponentModel.ISupportInitialize)(this.cabinet)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
        }

        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var myPen = new Pen(Color.Black, 1);
            var myRectangle = new Rectangle(0, 0, Width - 1, Height - 1);
            e.Graphics.DrawRectangle(myPen, myRectangle);
        }

        private void ErrorShower_StartErrorShow(object sender, EventArgs e)
        {
            TopMost = false;
        }

        private void ErrorShower_ErrorShowEnd(object sender, EventArgs e)
        {
            TopMost = true;
        }

		internal void ReloadUI()
		{
			System.Threading.Thread.CurrentThread.CurrentUICulture = Environment.CurCultureInfo;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashForm));
			this.label1.Text = resources.GetString("label1.Text");
			this.label1.Refresh();
		}
	}
}