using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using Kesco.App.Win.DocView.Common;

namespace Kesco.App.Win.DocView.PropertiesDialogs
{
	/// <summary>
	/// диалог выбора текущего лица архива
	/// </summary>
	public class PropertiesCurrentPersonDialog : Lib.Win.FreeDialog
	{
		private Button buttonOK;
		private ComboBox comboPerson;
		private Button buttonCancel;
		private Container components;

		public PropertiesCurrentPersonDialog()
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertiesCurrentPersonDialog));
			this.comboPerson = new System.Windows.Forms.ComboBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// comboPerson
			// 
			this.comboPerson.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.comboPerson, "comboPerson");
			this.comboPerson.Name = "comboPerson";
			// 
			// buttonOK
			// 
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			resources.ApplyResources(this.buttonOK, "buttonOK");
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// PropertiesCurrentPersonDialog
			// 
			this.AcceptButton = this.buttonOK;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.comboPerson);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PropertiesCurrentPersonDialog";
			this.ShowInTaskbar = false;
			this.Load += new System.EventHandler(this.PropertesCurrentPersonDialog_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private void PropertesCurrentPersonDialog_Load(object sender, EventArgs e)
		{
			FillList();
		}

		private void FillList()
		{
			if(!Environment.IsConnected)
				return;
			try
			{
				using(DataTable dt = Environment.PersonData.GetCurrentPersons())
				{
					for(int i = 0; i < dt.Rows.Count; i++)
					{
						var per = new Person(dt.Rows[i]);
						comboPerson.Items.Add(per);
					}
					dt.Dispose();
				}
				Lib.Win.Document.Environment.PersonID = Environment.UserSettings.PersonID;
				Environment.CompanyName = null;
				for(int i = 0; i < comboPerson.Items.Count; i++)
				{
					var per = comboPerson.Items[i] as Person;
					if(per != null && per.ID == Environment.UserSettings.PersonID)
						comboPerson.SelectedIndex = i;
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		public void RefreshPerson()
		{
			FillList();
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			try
			{
				var per = comboPerson.SelectedItem as Person;
				if(per != null && per.ID != Environment.UserSettings.PersonID)
				{
					Environment.UserSettings.PersonID = per.ID;
					Environment.UserSettings.Save();
					Lib.Win.Document.Environment.PersonID = Environment.UserSettings.PersonID;
					Environment.CompanyName = null;
				}
				this.Close();
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			End(System.Windows.Forms.DialogResult.Cancel);
		}
	}
}