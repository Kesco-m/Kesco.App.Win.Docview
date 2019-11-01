using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Windows.Forms;
using Kesco.App.Win.DocView.Items;

namespace Kesco.App.Win.DocView.PropertiesDialogs.FaxItem
{
	public class FaxPropertiButtonControl : FaxPropertyControl
	{
		private ResourceManager resources;
		private Button buttonAdd;
		private IContainer components;

		public FaxPropertiButtonControl(string propertyName, string propertyValue, int type) : base(propertyName, propertyValue)
		{
			InitializeComponent();
			switch(type)
			{
				case 1:
					buttonAdd.Text = resources.GetString("buttonAdd.Text");
					buttonAdd.Click += buttonAdd_Click_AddContact;
					break;
				case 2:
					buttonAdd.Text = Environment.StringResources.GetString("FaxPropertiButtonControl.Message1");
					buttonAdd.Click += buttonAdd_Click_GoToDoc;
					break;
			}
		}

		/// <summary>
		///   Clean up any resources being used.
		/// </summary>
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

		#region Designer generated code

		/// <summary>
		///   Required method for Designer support - do not modify the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			resources = new System.Resources.ResourceManager(typeof(FaxPropertiButtonControl));
			this.buttonAdd = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// buttonAdd
			// 
			this.buttonAdd.AccessibleDescription = resources.GetString("buttonAdd.AccessibleDescription");
			this.buttonAdd.AccessibleName = resources.GetString("buttonAdd.AccessibleName");
			this.buttonAdd.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("buttonAdd.Anchor")));
			this.buttonAdd.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonAdd.BackgroundImage")));
			this.buttonAdd.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("buttonAdd.Dock")));
			this.buttonAdd.Enabled = ((bool)(resources.GetObject("buttonAdd.Enabled")));
			this.buttonAdd.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("buttonAdd.FlatStyle")));
			this.buttonAdd.Font = ((System.Drawing.Font)(resources.GetObject("buttonAdd.Font")));
			this.buttonAdd.ForeColor = System.Drawing.SystemColors.Highlight;
			this.buttonAdd.Image = ((System.Drawing.Image)(resources.GetObject("buttonAdd.Image")));
			this.buttonAdd.ImageAlign =
				((System.Drawing.ContentAlignment)(resources.GetObject("buttonAdd.ImageAlign")));
			this.buttonAdd.ImageIndex = ((int)(resources.GetObject("buttonAdd.ImageIndex")));
			this.buttonAdd.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("buttonAdd.ImeMode")));
			this.buttonAdd.Location = ((System.Drawing.Point)(resources.GetObject("buttonAdd.Location")));
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.RightToLeft =
				((System.Windows.Forms.RightToLeft)(resources.GetObject("buttonAdd.RightToLeft")));
			this.buttonAdd.Size = ((System.Drawing.Size)(resources.GetObject("buttonAdd.Size")));
			this.buttonAdd.TabIndex = ((int)(resources.GetObject("buttonAdd.TabIndex")));
			this.buttonAdd.Text = resources.GetString("buttonAdd.Text");
			this.buttonAdd.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("buttonAdd.TextAlign")));
			this.buttonAdd.Visible = ((bool)(resources.GetObject("buttonAdd.Visible")));
			// 
			// FaxPropertiButtonControl
			// 
			this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
			this.AccessibleName = resources.GetString("$this.AccessibleName");
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.Controls.Add(this.buttonAdd);
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.Name = "FaxPropertiButtonControl";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.Size = ((System.Drawing.Size)(resources.GetObject("$this.Size")));
			this.Controls.SetChildIndex(this.buttonAdd, 0);
			this.ResumeLayout(false);
		}

		#endregion

		public int FaxID { get; set; }

		private void buttonAdd_Click_AddContact(object sender, EventArgs e)
		{
			Kesco.Lib.Win.Web.ContactDialog ccDialog = new Kesco.Lib.Win.Web.ContactDialog(Environment.CreateContactString,
											 "personContactCategor=3&docview=yes&personContactText=" +
											 labelValue.Text) { PersonID = FaxID, Owner = FindForm() };
			ccDialog.DialogEvent += createContactDialog_DialogEvent;
			ccDialog.Show();
		}

		private void createContactDialog_DialogEvent(object source, Lib.Win.DialogEventArgs e)
		{
			if(e.Dialog.DialogResult != DialogResult.OK)
				return;
			Lib.Win.Web.ContactDialog dialog = e.Dialog as Lib.Win.Web.ContactDialog;
			if(dialog == null)
				return;
			dialog.DialogEvent -= createContactDialog_DialogEvent;
			if(dialog.ContactID > 0)
				buttonAdd.Visible = false;
		}

		private void buttonAdd_Click_GoToDoc(object sender, EventArgs e)
		{
			try
			{
				// есть ли у факса изображения, сохраненные в архив?
				List<int> ids = Environment.FaxData.GetFaxDBDocs(FaxID);
				var menu = new ContextMenu();
				if(ids != null)
					foreach(
						var item in from t in ids where t > 0 select new IDMenuItem(t) { Text = Lib.Win.Document.DBDocString.Format(t) })
					{
						item.Click += toDBDoc_Click;
						menu.MenuItems.Add(item);
					}
				menu.Show(buttonAdd, new Point(0, buttonAdd.Bottom));
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void toDBDoc_Click(object sender, EventArgs e)
		{
			try
			{
				var item = sender as IDMenuItem;
				if(item == null)
					return;

				Forms.MainFormDialog.returnID = item.ID;
				Environment.CmdManager.Commands["Return"].Execute();

				var findForm = FindForm();
				if(findForm != null)
					findForm.Close();
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}
	}
}