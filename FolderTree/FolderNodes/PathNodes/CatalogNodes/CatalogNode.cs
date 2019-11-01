using System;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Kesco.App.Win.DocView.Misc;
using Kesco.App.Win.DocView.PropertiesDialogs;

namespace Kesco.App.Win.DocView.FolderTree.FolderNodes.PathNodes.CatalogNodes
{
	public class CatalogNode : PathNode
	{
		private static string rootTitle = Environment.StringResources.GetString("Archive");

		internal const string CatalogPathInitial = "|"; // первый символ пути в дереве документов
		internal const string SlashDelimiter = @"\";
		internal const string ColonDelimiter = ":";
		internal const string PersonInitial = "L";
		internal const string DocTypeInitial = "T";

		protected MenuItem shareItem;
		protected MenuItem changePersonItem;

		private int docTypeID;
		private int personID = -1;

		protected CatalogNode(string path, string name, bool hasSubNodes) : base(path)
		{
			try
			{
				SetImages();

				this.name = name;
				Text = name;

				UpdateImages();

				// Analyzing path
				Path = path;

				// doc type
				Regex r = new Regex(DocTypeInitial + ColonDelimiter + @"(?<doctype>\d+)");
				Match m = r.Match(path);

				if(m.Success)
					Int32.TryParse(m.Groups["doctype"].Value, out docTypeID);

				// person
				r = new Regex(PersonInitial + ColonDelimiter + @"(?<person>\d+)");
				m = r.Match(path);

				if(m.Success)
					Int32.TryParse(m.Groups["person"].Value, out personID);

				if(hasSubNodes)
					Nodes.Add(new TreeNode());

				// пункты меню
				shareItem = new MenuItem();
				changePersonItem = new MenuItem();


				// передать папку сотруднику
				shareItem.Text = Environment.StringResources.GetString("FolderTree.FolderNodes.CatalogNode.Message1");
				shareItem.Click += shareItem_Click;

				// смена текущего лица сотрудника
				changePersonItem.Text = Environment.StringResources.GetString("SelectArchive");
				changePersonItem.Click += changePersonItem_Click;
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		protected virtual void SetImages()
		{
			unselectedCollapsed = Images.CatalogRoot;
			selectedCollapsed = Images.CatalogRoot;
			unselectedExpanded = Images.CatalogRoot;
			selectedExpanded = Images.CatalogRoot;
		}

		public override void LoadSubNodes()
		{
			try
			{
				Nodes.Clear();

				TreeView.Cursor = Cursors.WaitCursor;
                Console.WriteLine("{0}: sp_TreeSubNodes in", DateTime.Now.ToString("HH:mm:ss fff"));

#if AdvancedLogging
                using (Lib.Log.Logger.DurationMetter("CatalogNode LoadSubNodes() Path = " + Path))
#endif
				using(DataTable dt = Environment.DocTreeSPData.GetTreeSubNodes(Path))
                {
                    Console.WriteLine("{0}: sp_TreeSubNodes out", DateTime.Now.ToString("HH:mm:ss fff"));
					foreach(DataRow dr in dt.Rows)
					{
						bool hasSubNodes = true;
						object obj = dr[Environment.DocTreeSPData.SubNodesField];
						if(obj is int)
							hasSubNodes = ((int)obj == 1);

						var nodePath = (string)dr[Environment.DocTreeSPData.KeyField];
						var nodeType = (int)dr[Environment.DocTreeSPData.TypeField];

						string nodeName = "";
						if(!dr.IsNull(Environment.DocTreeSPData.TextField))
							nodeName = dr[Environment.DocTreeSPData.TextField].ToString();
						else
							Lib.Win.Data.Env.WriteToLog(new Kesco.Lib.Log.DetailedException("DB NULL в поле Text дл€ пути " + nodePath, null,
																 Kesco.Lib.Log.Priority.Error));

						try
						{
							var catalogNodeType = (CatalogNodeType)nodeType;
							CatalogNode node = CreateSubNode(
								catalogNodeType, nodePath, nodeName, hasSubNodes);

							if(node != null)
								Nodes.Add(node);
						}
						catch(Exception ex)
						{
							Lib.Win.Data.Env.WriteToLog(new Exception("Ќеизвестный тип узла каталога: " + nodeType, ex));
						}
					}
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
			base.LoadSubNodes();
			TreeView.Cursor = Cursors.Default;
		}

		protected CatalogNode CreateSubNode(CatalogNodeType type, string nodePath, string nodeName, bool hasSubNodes)
		{
			switch(type)
			{
				case CatalogNodeType.DocTypeSynonymGroup:
					return new DocTypeSynonymGroupNode(nodePath, nodeName, hasSubNodes);

				case CatalogNodeType.DocType:
					return new DocTypeNode(nodePath, nodeName, hasSubNodes);

				case CatalogNodeType.Company:
					return new CompanyNode(nodePath, nodeName, hasSubNodes);

				case CatalogNodeType.Person:
					return new PersonNode(nodePath, nodeName, hasSubNodes);

				case CatalogNodeType.NoPerson:
					return new NoPersonNode(nodePath, nodeName, hasSubNodes);
			}
			return null;
		}

		public static CatalogNode CreateRoot()
		{
			string person = "";
			if(Environment.UserSettings.PersonID > 0)
				person = " " + Environment.CompanyName;
			return new CatalogNode(CatalogPathInitial, rootTitle + person, true);
		}

		public override void On_MouseUp(MouseEventArgs e)
		{
			switch(e.Button)
			{
				case MouseButtons.Right:
					TreeView.SelectedNode = this;
					if(personID != -1 && docTypeID != 0)
					{
						var contextMenu = new ContextMenu(
							new[] { shareItem });

						contextMenu.Show(TreeView, new Point(e.X, e.Y));
					}
					if(Path == CatalogPathInitial)
					{
						var contextMenu = new ContextMenu(
							new[] { changePersonItem });

						contextMenu.Show(TreeView, new Point(e.X, e.Y));
					}
					break;
			}
		}

		protected void shareItem_Click(object sender, EventArgs e)
		{
			Kesco.Lib.Win.Web.UserDialog dialog = new Kesco.Lib.Win.Web.UserDialog(Environment.EmployeeSearchString, Forms.MainFormDialog.userParamStr);
			dialog.DialogEvent += UserDialog_DialogEvent;
			dialog.Show();
		}

		private void changePersonItem_Click(object sender, EventArgs e)
		{
			var dialog = new PropertiesCurrentPersonDialog { Owner = Program.MainFormDialog };
			dialog.DialogEvent += PropertesCurrentPersonDialog_DialogEvent;
			dialog.Show();
		}


		private void UserDialog_DialogEvent(object source, Kesco.Lib.Win.DialogEventArgs e)
		{
			Kesco.Lib.Win.Web.UserDialog dialog = e.Dialog as Kesco.Lib.Win.Web.UserDialog;
			if(dialog == null)
				return;

			dialog.DialogEvent -= UserDialog_DialogEvent;
			if(dialog.DialogResult == DialogResult.OK && (dialog.Users != null && dialog.Users.Count > 0))
				try
				{
					var newUser = (Kesco.Lib.Win.Web.UserInfo)dialog.Users[0];

					if(MessageBox.Show(
						Environment.StringResources.GetString(
							"FolderTree.FolderNodes.CatalogNode.UserDialog_DialogEvent.Message1") +
						" [" +
						Environment.DocTypeData.GetDocType(docTypeID,
														   Environment.CurCultureInfo.TwoLetterISOLanguageName) + "]," +
						System.Environment.NewLine +
						Environment.StringResources.GetString(
							"FolderTree.FolderNodes.CatalogNode.UserDialog_DialogEvent.Message2") +
						Environment.PersonWord.GetForm(Kesco.Lib.Win.Document.Cases.T, false, false) +
						" [" + ((personID > 0) ? Environment.PersonData.GetPerson(personID) : Environment.CompanyName ?? "")
															 + "]," + System.Environment.NewLine +
						Environment.StringResources.GetString(
							"FolderTree.FolderNodes.CatalogNode.UserDialog_DialogEvent.Message3") +
						" [" + newUser.Name + "]" + System.Environment.NewLine +
						Environment.StringResources.GetString(
							"FolderTree.FolderNodes.CatalogNode.UserDialog_DialogEvent.Message4") + "." +
						System.Environment.NewLine + System.Environment.NewLine +
						Environment.StringResources.GetString("Continue"),
						Environment.StringResources.GetString(
							"FolderTree.FolderNodes.CatalogNode.UserDialog_DialogEvent.Title1"), MessageBoxButtons.YesNoCancel,
						MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)

						Environment.DocData.ShareArchiveFolder(Path, newUser.ID);
				}
				catch(Exception ex)
				{
					Lib.Win.Data.Env.WriteToLog(ex);
				}
		}

		private void PropertesCurrentPersonDialog_DialogEvent(object source, Kesco.Lib.Win.DialogEventArgs e)
		{
			if(e.Dialog.DialogResult == DialogResult.OK)
			{
				if(Program.MainFormDialog != null)
				{
					Environment.CmdManager.Commands["Refresh"].Execute();
				}
			}
		}

		public override void LoadDocs(Grids.DocGrid docGrid, bool clean, int curID, string filename = null)
		{
			docGrid.LoadCatalogDocs(Path, clean, curID);
			this.curID = 0;
		}

		public override Context BuildContext()
		{
			return new Context(ContextMode.Catalog, Path);
		}

		public override void Nullify()
		{
			base.Nullify();

			Nodes.Clear();
			Nodes.Add(new TreeNode());
		}

		private static string WrapID(string initial, int id)
		{
			return SlashDelimiter + initial + ColonDelimiter + id;
		}

		public static string WrapPersonID(int personID)
		{
			return WrapID(PersonInitial, personID);
		}

		public static string WrapDocTypeID(int typeID)
		{
			return WrapID(DocTypeInitial, typeID);
		}

		public override bool IsCatalog()
		{
			return true;
		}
	}
}