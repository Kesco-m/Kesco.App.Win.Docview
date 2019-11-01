using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Kesco.Lib.Win.Data.Temp.Objects;

namespace Kesco.App.Win.DocView.FolderTree.Dialogs
{
	/// <summary>
	///   Summary description for ShareWorkFolder.
	/// </summary>
	public class ShareWorkFolder : Lib.Win.FreeDialog
	{
		private IContainer components;
		private Button buttonAdd;
		private GroupBox groupBox1;
		private Button buttonRemove;
		private CheckBox checkFullAccess;
		private ListView list;
		private ImageList imageList;
		private Button buttonOK;

		private int folderID;

		public ShareWorkFolder(int folderID, string title)
		{
			InitializeComponent();

			this.folderID = folderID;

			Text += " \"" + title + "\"";
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
			this.components = new System.ComponentModel.Container();
			var resources =
				new System.ComponentModel.ComponentResourceManager(typeof(ShareWorkFolder));
			this.buttonAdd = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.list = new System.Windows.Forms.ListView();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.buttonRemove = new System.Windows.Forms.Button();
			this.checkFullAccess = new System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonAdd
			// 
			resources.ApplyResources(this.buttonAdd, "buttonAdd");
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
			// 
			// buttonOK
			// 
			resources.ApplyResources(this.buttonOK, "buttonOK");
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// groupBox1
			// 
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Controls.Add(this.list);
			this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			// 
			// list
			// 
			resources.ApplyResources(this.list, "list");
			this.list.FullRowSelect = true;
			this.list.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.list.HideSelection = false;
			this.list.Name = "list";
			this.list.SmallImageList = this.imageList;
			this.list.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.list.UseCompatibleStateImageBehavior = false;
			this.list.View = System.Windows.Forms.View.Details;
			this.list.SelectedIndexChanged += new System.EventHandler(this.list_SelectedIndexChanged);
			// 
			// imageList
			// 
			this.imageList.ImageStream =
				((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.SystemColors.Control;
			this.imageList.Images.SetKeyName(0, "");
			// 
			// buttonRemove
			// 
			resources.ApplyResources(this.buttonRemove, "buttonRemove");
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
			// 
			// checkFullAccess
			// 
			resources.ApplyResources(this.checkFullAccess, "checkFullAccess");
			this.checkFullAccess.Name = "checkFullAccess";
			this.checkFullAccess.ThreeState = true;
			this.checkFullAccess.EnabledChanged += new System.EventHandler(this.checkFullAccess_EnabledChanged);
			this.checkFullAccess.Click += new System.EventHandler(this.checkFullAccess_Click);
			// 
			// ShareWorkFolder
			// 
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.buttonOK;
			this.Controls.Add(this.checkFullAccess);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.buttonAdd);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonRemove);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ShareWorkFolder";
			this.Load += new System.EventHandler(this.ShareWorkFolder_Load);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
		}

		#endregion

		private void ShareWorkFolder_Load(object sender, EventArgs e)
		{
			list.Columns.Add(Environment.StringResources.GetString("Employee"),
							 list.Width - SystemInformation.VerticalScrollBarWidth - 4, HorizontalAlignment.Left);
			FillList();
		}

		private void FillList()
		{
			try
			{
				list.BeginUpdate();
				list.Items.Clear();

				using(DataTable dt = Environment.FolderData.GetClients(folderID))
				using(DataTableReader dr = dt.CreateDataReader())
				{
					while(dr.Read())
					{
						int id = (int)dr[Environment.FolderData.ShareIDField];
						int empID = (int)dr[Environment.FolderData.ClientIDField];
						Employee emp = new Employee(empID, Environment.EmpData);

						AccessLevel rights = AccessLevel.ReadOnly;
						var byteRights = (byte)dr[Environment.FolderData.RightsField];
						if(byteRights == 1)
							rights = AccessLevel.FullAccess;

						var wfItem = new WFAccessItem(id, empID, emp.ShortName, rights);

						if(rights == AccessLevel.FullAccess)
							wfItem.ImageIndex = 0;

						list.Items.Add(wfItem);
					}
					dr.Close();
					dr.Dispose();
					dt.Dispose();
				}

				list.Sort();
				list.EndUpdate();
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void buttonAdd_Click(object sender, EventArgs e)
		{
			var dialog = new Kesco.Lib.Win.Web.UserDialog(Environment.EmployeeSearchString,
										Forms.MainFormDialog.userMultipleParamStr);
			dialog.DialogEvent += UserDialog_DialogEvent;
			dialog.Show();
			Enabled = false;
		}

		private void UserDialog_DialogEvent(object source, Kesco.Lib.Win.DialogEventArgs e)
		{
			Enabled = true;
			Focus();

			var dialog = e.Dialog as Kesco.Lib.Win.Web.UserDialog;
			if(dialog == null || dialog.DialogResult != DialogResult.OK || dialog.Users == null)
				return;
			try
			{
				foreach(
					var newUser in
						dialog.Users.Cast<Kesco.Lib.Win.Web.UserInfo>().Where(
							newUser => list.Items.Cast<WFAccessItem>().Count(li => li.EmpID == newUser.ID) == 0))
					Environment.FolderData.AddShare(folderID, newUser.ID, (int)AccessLevel.ReadOnly);
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
			FillList();
		}

		private void list_SelectedIndexChanged(object sender, EventArgs e)
		{
			bool enabled = (list.SelectedItems.Count > 0);

			checkFullAccess.Enabled = enabled;
			buttonRemove.Enabled = enabled;

			int setCount = list.SelectedItems.Cast<WFAccessItem>().Count(li => li.Rights == AccessLevel.FullAccess);

			checkFullAccess.CheckState = setCount == list.SelectedItems.Count
											 ? CheckState.Checked
											 : (setCount > 0 ? CheckState.Indeterminate : CheckState.Unchecked);
		}

		private void buttonRemove_Click(object sender, EventArgs e)
		{
			try
			{
				if((list.SelectedItems.Count > 0) &&
					(MessageBox.Show(
						Environment.StringResources.GetString("FolderTree.ShareWorkFolder.buttonRemove_Click.Message1"),
						Environment.StringResources.GetString("Confirmation"), MessageBoxButtons.YesNoCancel,
						MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes))
				{
					for(int i = 0; i < list.SelectedItems.Count; i++)
					{
						var li = (WFAccessItem)list.SelectedItems[i];
						Environment.FolderData.RemoveShare(li.ID);
					}

					FillList();
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void checkFullAccess_Click(object sender, EventArgs e)
		{
			try
			{
				CheckState state = checkFullAccess.CheckState;

				if(state == CheckState.Indeterminate)
					checkFullAccess.CheckState = CheckState.Unchecked;

				AccessLevel rights = AccessLevel.ReadOnly;

				if(state == CheckState.Checked)
					rights = AccessLevel.FullAccess;

				list.BeginUpdate();

				foreach(
					WFAccessItem li in
						list.SelectedItems.Cast<WFAccessItem>().Where(
							li => Environment.FolderData.SetRights(li.ID, (int)rights)))
				{
					li.Rights = rights;
					li.ImageIndex = rights == AccessLevel.ReadOnly ? -1 : 0;
				}

				list.EndUpdate();
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void checkFullAccess_EnabledChanged(object sender, EventArgs e)
		{
			checkFullAccess.CheckState = CheckState.Unchecked;
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			End(DialogResult.OK);
		}
	}
}