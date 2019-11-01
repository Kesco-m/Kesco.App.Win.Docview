using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Kesco.App.Win.DocView.Grids;
using Kesco.Lib.Win.Document;
using Kesco.Lib.Win.Options;

namespace Kesco.App.Win.DocView.Dialogs
{
    public class MessageShowDialog : DropDownForm
    {
        #region wndproc

		protected override void WndProc(ref Message m)
		{
			if(m.Msg == (int)Lib.Win.Document.Win32.Msgs.WM_SIZING)
				WmSizing(ref m);
			else
			{
				if(m.Msg == (int)Lib.Win.Document.Win32.Msgs.WM_EXITSIZEMOVE)
					MessageShowDialog_SizeChanged();
				base.WndProc(ref m);
			}
		}

		private void WmSizing(ref Message m)
		{
			var r = (Lib.Win.Document.Win32.User32.RECT)Marshal.PtrToStructure(m.LParam, typeof(Lib.Win.Document.Win32.User32.RECT));

			switch((Lib.Win.Document.Win32.User32.WMSZ)m.WParam)
			{
				case Lib.Win.Document.Win32.User32.WMSZ.WMSZ_LEFT:
				case Lib.Win.Document.Win32.User32.WMSZ.WMSZ_RIGHT:
					break;
				case Lib.Win.Document.Win32.User32.WMSZ.WMSZ_TOP:
					r.top = Top;
					m.Result = new IntPtr(-1);
					break;
				case Lib.Win.Document.Win32.User32.WMSZ.WMSZ_BOTTOM:
					r.bottom = Bottom;
					m.Result = new IntPtr(-1);
					break;
				case Lib.Win.Document.Win32.User32.WMSZ.WMSZ_TOPLEFT:
				case Lib.Win.Document.Win32.User32.WMSZ.WMSZ_TOPRIGHT:
					r.top = Top;
					break;
				case Lib.Win.Document.Win32.User32.WMSZ.WMSZ_BOTTOMLEFT:
				case Lib.Win.Document.Win32.User32.WMSZ.WMSZ_BOTTOMRIGHT:
					r.bottom = Bottom;
					break;
			}
			Marshal.StructureToPtr(r, m.LParam, true);

			base.WndProc(ref m);
		}

        #endregion

        private Folder optionFolder;
        private InfoGrid infoGrid;
        private IContainer components;

        public MessageShowDialog(int docID, Rectangle rect)
            : base(rect)
        {
            InitializeComponent();

            infoGrid.Init(Environment.Layout);

            optionFolder = Environment.Layout.Folders.Add(Name);
            Width = optionFolder.LoadIntOption("Width", Width);
            Height = 32;
            infoGrid.LoadInfo(docID);
			infoGrid.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(infoGrid_DataBindingComplete);
            MessageShowDialog_SizeChanged();
            DropDown();
        }

		void infoGrid_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
		{
			MessageShowDialog_SizeChanged();
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
            this.infoGrid = new InfoGrid();
            ((System.ComponentModel.ISupportInitialize) (this.infoGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // infoGrid
            // 
            this.infoGrid.BackgroundColor = System.Drawing.SystemColors.Window;
            this.infoGrid.DataMember = "";
            this.infoGrid.Dock = DockStyle.Fill;
            this.infoGrid.GridColor = System.Drawing.SystemColors.Window;
            this.infoGrid.ImageTime = new System.DateTime(((long) (0)));
            this.infoGrid.Location = new System.Drawing.Point(0, 0);
            this.infoGrid.MainForm = null;
            this.infoGrid.Name = "infoGrid";
            this.infoGrid.ReadOnly = true;
            this.infoGrid.RowHeadersVisible = false;
            this.infoGrid.DefaultCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            this.infoGrid.DefaultCellStyle.SelectionForeColor = System.Drawing.SystemColors.Window;
            this.infoGrid.Size = new System.Drawing.Size(292, 266);
            this.infoGrid.Style = null;
            this.infoGrid.TabIndex = 5;
            this.infoGrid.TabStop = false;
            // 
            // MessageShowDialog
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.infoGrid);
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "MessageShowDialog";
            this.Closed += new System.EventHandler(this.MessageShowDialog_Closed);
            ((System.ComponentModel.ISupportInitialize) (this.infoGrid)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private void MessageShowDialog_SizeChanged()
        {
            if (infoGrid.Rows.Count == 0) 
                return;

            int totalHeight = 16 + infoGrid.Rows.Cast<DataGridViewRow>().Sum(row => row.Height);
            var screenRect = Screen.FromControl(this).WorkingArea;
            Height = totalHeight > screenRect.Bottom - Top
                         ? screenRect.Bottom - Top
                         : totalHeight;

            infoGrid.Invalidate();
            Console.WriteLine("{0}: Message resize", DateTime.Now.ToString("HH:mm:ss fff"));
        }

        private void MessageShowDialog_Closed(object sender, EventArgs e)
        {
            optionFolder.Option("Width").Value = Width;
            optionFolder.Save();
        }
    }
}