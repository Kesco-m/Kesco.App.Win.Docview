using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using Kesco.App.Win.DocView.Items;
using Rule = Kesco.Lib.Win.Data.DALC.Documents.Rule;

namespace Kesco.App.Win.DocView.PropertiesDialogs
{
	public class PropertiesFolderRuleViewDialog : Lib.Win.FreeDialog
	{
		private Button CreateNewRuleButton;
		private Button EditRuleButton;
		private Button DeleteRuleButton;
		private Label FolderNameLabel;

		private int folderID;
		private string folderName;

		private DataTable folderRulesDT;
		private Button ApplyRuleButton;
		private ListView list;
		private Button buttonCancel;

		private Container components;

		public PropertiesFolderRuleViewDialog()
		{
			InitializeComponent();

			list.Columns.Add("Rule", list.Width - 20, HorizontalAlignment.Left);
		}


		public PropertiesFolderRuleViewDialog(int folderID, string folderName) : this()
		{
			CreateNewRuleButton.Select();

			this.folderID = folderID;
			this.folderName = folderName;

			FolderNameLabel.Text =
				Environment.StringResources.GetString("Properties.PropertiesFolderRuleViewDialog.Text1") + "\"" +
				folderName + "\" " +
				Environment.StringResources.GetString("Properties.PropertiesFolderRuleViewDialog.Text2");

			FillList();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertiesFolderRuleViewDialog));
			this.CreateNewRuleButton = new System.Windows.Forms.Button();
			this.EditRuleButton = new System.Windows.Forms.Button();
			this.DeleteRuleButton = new System.Windows.Forms.Button();
			this.FolderNameLabel = new System.Windows.Forms.Label();
			this.ApplyRuleButton = new System.Windows.Forms.Button();
			this.list = new System.Windows.Forms.ListView();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// CreateNewRuleButton
			// 
			resources.ApplyResources(this.CreateNewRuleButton, "CreateNewRuleButton");
			this.CreateNewRuleButton.Name = "CreateNewRuleButton";
			this.CreateNewRuleButton.Click += new System.EventHandler(this.CreateNewRuleButton_Click);
			// 
			// EditRuleButton
			// 
			resources.ApplyResources(this.EditRuleButton, "EditRuleButton");
			this.EditRuleButton.Name = "EditRuleButton";
			this.EditRuleButton.Click += new System.EventHandler(this.EditRuleButton_Click);
			// 
			// DeleteRuleButton
			// 
			resources.ApplyResources(this.DeleteRuleButton, "DeleteRuleButton");
			this.DeleteRuleButton.Name = "DeleteRuleButton";
			this.DeleteRuleButton.Click += new System.EventHandler(this.DeleteRuleButton_Click);
			// 
			// FolderNameLabel
			// 
			resources.ApplyResources(this.FolderNameLabel, "FolderNameLabel");
			this.FolderNameLabel.Name = "FolderNameLabel";
			// 
			// ApplyRuleButton
			// 
			resources.ApplyResources(this.ApplyRuleButton, "ApplyRuleButton");
			this.ApplyRuleButton.Name = "ApplyRuleButton";
			this.ApplyRuleButton.Click += new System.EventHandler(this.ApplyRuleButton_Click);
			// 
			// list
			// 
			this.list.FullRowSelect = true;
			this.list.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.list.HideSelection = false;
			resources.ApplyResources(this.list, "list");
			this.list.Name = "list";
			this.list.ShowItemToolTips = true;
			this.list.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.list.UseCompatibleStateImageBehavior = false;
			this.list.View = System.Windows.Forms.View.Details;
			this.list.SelectedIndexChanged += new System.EventHandler(this.list_SelectedIndexChanged);
			this.list.DoubleClick += new System.EventHandler(this.list_DoubleClick);
			// 
			// buttonCancel
			// 
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// PropertiesFolderRuleViewDialog
			// 
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.list);
			this.Controls.Add(this.ApplyRuleButton);
			this.Controls.Add(this.FolderNameLabel);
			this.Controls.Add(this.DeleteRuleButton);
			this.Controls.Add(this.EditRuleButton);
			this.Controls.Add(this.CreateNewRuleButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PropertiesFolderRuleViewDialog";
			this.ResumeLayout(false);

		}

		#endregion

		private void CreateNewRuleButton_Click(object sender, EventArgs e)
		{
			if(!CheckFolderExist())
				return;

			var propertiesFolderRuleSearchDialog =
				new PropertiesFolderRuleSearchDialog(folderID, folderName, new Rule()) { ParentViewDialog = this };

			ShowSubForm(propertiesFolderRuleSearchDialog);
		}

		private void EditRuleButton_Click(object sender, EventArgs e)
		{
			if(!CheckFolderExist())
				return;

			ShowEditDialog();
		}

		private void ShowEditDialog()
		{
			if(list.SelectedItems.Count != 1)
				return;
			var item = list.SelectedItems[0] as RuleListItem;
			if(item != null)
			{
				var propertiesFolderRuleSearchDialog =
					new PropertiesFolderRuleSearchDialog(folderID, folderName, item.Rule) { ParentViewDialog = this };
				ShowSubForm(propertiesFolderRuleSearchDialog);
			}
		}

		private void DeleteRuleButton_Click(object sender, EventArgs e)
		{
			if(!CheckFolderExist())
				return;

			if(list.SelectedItems.Count == 0)
				return;

			string deleteMessage = Environment.StringResources.GetString("Properties.PropertiesFolderRuleViewDialog.DeleteRuleButton_Click.Message1");
			if(MessageBox.Show(deleteMessage, Environment.StringResources.GetString( "Properties.PropertiesFolderRuleViewDialog.DeleteRuleButton_Click.Title1"),
								MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
			{
				for(int i = 0; i < list.SelectedItems.Count; i++)
				{
					var item = list.SelectedItems[i] as RuleListItem;
					if(item == null)
						continue;
					Environment.FolderRuleData.Delete(item.Rule.ID);
				}
				FillList();
			}
		}

		private void FillList()
		{
			int MaxRowWidth = 0;
			if(folderRulesDT != null)
				folderRulesDT.Dispose();

			folderRulesDT = Environment.FolderRuleData.GetFolderRules(folderID); //обновление информации

			list.Items.Clear();

			if(folderRulesDT != null)
			{
				foreach(DataRow row in folderRulesDT.Rows)
				{
					Rule rule = new Rule(row);
					RuleListItem item = new RuleListItem(rule, rule.Name);
					item.ToolTipText = item.Text;
					list.Items.Add(item);
					if(MaxRowWidth < rule.Name.Length)
						MaxRowWidth = rule.Name.Length;
				}
			}
			if(list.Items.Count > 0)
			{
				list.Items[0].Selected = true;
				list.Columns[0].Width = MaxRowWidth < 68 ? Convert.ToInt32(5.8 * 68) : Convert.ToInt32(5.7 * MaxRowWidth);
			}
			LockButtons();
		}

		private void ApplyRuleButton_Click(object sender, EventArgs e)
		{
			if(!CheckFolderExist())
				return;

			if(list.SelectedItems.Count <= 0)
				return;

			RuleListItem item = list.SelectedItems[0] as RuleListItem;
			if(item == null)
				return;

			PropertiesFolderRuleApplyDialog propertiesFolderRuleApplyDialog = new PropertiesFolderRuleApplyDialog(item.Rule.ID, item.Rule.Name, folderName);
			propertiesFolderRuleApplyDialog.DialogEvent += PropertiesFolderRuleApplyDialog_DialogEvent;
			propertiesFolderRuleApplyDialog.Show();
			Enabled = false;
		}

		private void PropertiesFolderRuleApplyDialog_DialogEvent(object source, Kesco.Lib.Win.DialogEventArgs e)
		{
			Enabled = true;
			Focus();

			var dialog = e.Dialog as PropertiesFolderRuleApplyDialog;
			if(dialog != null && dialog.DialogResult == DialogResult.OK)
			{
				Environment.FolderRuleData.ApplyFolderRule(dialog.RuleID, dialog.SelectedFolderID,
														   dialog.ApplySubFolders);
				Environment.RefreshDocs();

				// shit, this form doesn't want to pop up
				Focus();
				BringToFront();
			}
		}

		private bool CheckFolderExist()
		{
			if(!Environment.FolderData.FolderExists(folderID))
			{
				MessageBox.Show(Environment.StringResources.GetString("FolderNotExist"),
								Environment.StringResources.GetString("Error"), MessageBoxButtons.OK,
								MessageBoxIcon.Question);
				DialogResult = DialogResult.None;
				Close();
				return false;
			}
			return true;
		}


		public void FilterDialogClose(bool changeMode, DialogResult dResult)
		{
			Enabled = true;
			Focus();

			if(!changeMode) //новое правило
			{
				if(dResult == DialogResult.Ignore)
				{
					Close();
					return;
				}
				BringToFront();
				FillList();
			}
			else //редактирование
			{
				if(dResult == DialogResult.Ignore)
				{
					Close();
					return;
				}
				if(dResult == DialogResult.OK)
				{
					BringToFront();
					FillList();
				}
			}
		}

		private void list_SelectedIndexChanged(object sender, EventArgs e)
		{
			LockButtons();
		}

		private void list_DoubleClick(object sender, EventArgs e)
		{
			ShowEditDialog();
		}

		private void LockButtons()
		{
			int rulesSelected = list.SelectedItems.Count;

			DeleteRuleButton.Enabled = (rulesSelected > 0);
			ApplyRuleButton.Enabled = (rulesSelected > 0);
			EditRuleButton.Enabled = (rulesSelected == 1);
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			End(System.Windows.Forms.DialogResult.Cancel);
		}
	}
}