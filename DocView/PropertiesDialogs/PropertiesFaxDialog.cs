using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Kesco.App.Win.DocView.PropertiesDialogs.FaxItem;

namespace Kesco.App.Win.DocView.PropertiesDialogs
{
	public class PropertiesFaxDialog : Form
	{
		private Panel panelButton;
		private Button buttonCancel;
		private Panel panelControl;

		private Container components;

		public PropertiesFaxDialog()
		{
			InitializeComponent();
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(components != null)
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
				new System.ComponentModel.ComponentResourceManager(typeof(PropertiesFaxDialog));
			this.panelButton = new System.Windows.Forms.Panel();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.panelControl = new System.Windows.Forms.Panel();
			this.panelButton.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelButton
			// 
			resources.ApplyResources(this.panelButton, "panelButton");
			this.panelButton.Controls.Add(this.buttonCancel);
			this.panelButton.Name = "panelButton";
			// 
			// buttonCancel
			// 
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Click += new System.EventHandler(this.button1_Click);
			// 
			// panelControl
			// 
			resources.ApplyResources(this.panelControl, "panelControl");
			this.panelControl.Name = "panelControl";
			// 
			// PropertiesFaxDialog
			// 
			this.AcceptButton = this.buttonCancel;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.panelButton);
			this.Controls.Add(this.panelControl);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PropertiesFaxDialog";
			this.panelButton.ResumeLayout(false);
			this.ResumeLayout(false);
		}

		#endregion

		private void button1_Click(object sender, EventArgs e)
		{
			Close();
		}

		#region Metods

		public void PutControl(FaxPropertyControl control)
		{
			try
			{
				panelControl.SuspendLayout();
				SuspendLayout();

				int y = 0;
				foreach(
					Control con in
						panelControl.Controls.Cast<Control>().Where(con => con is FaxPropertyControl && con.Bottom > y)
					)
					y = con.Bottom;

				control.Size = new Size(panelControl.Width, control.Height);
				control.Location = new Point(0, y);
				control.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
				panelControl.Controls.Add(control);
				panelControl.ResumeLayout(false);
				ResumeLayout(false);
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		public void ResizeForm()
		{
			try
			{
				int y = (from Control con in panelControl.Controls select con.Bottom).Concat(new[] { 0 }).Max();
				int h = panelButton.Height + y + panelControl.Location.X + 4;
				ClientSize = new Size(Width, h);
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		#endregion
	}
}