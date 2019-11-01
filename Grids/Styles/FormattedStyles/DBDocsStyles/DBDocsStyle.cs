using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Kesco.App.Win.DocView.FolderTree.FolderNodes.PathNodes.CatalogNodes;
using Kesco.App.Win.DocView.Items;
using Kesco.Lib.Win.Document;

namespace Kesco.App.Win.DocView.Grids.Styles.FormattedStyles.DBDocsStyles
{
	public class DBDocsStyle : FormattedStyle
	{
		private static DBDocsStyle instance;

		protected MenuItem newEFormItem;
		protected MenuItem createAnswerItem;
		protected MenuItem linkEFormItem;
		protected MenuItem toWorkItem;
		protected MenuItem endWorkItem;
		protected MenuItem moveItem;
		protected MenuItem deleteItem;
		protected MenuItem sendMessageItem;
		protected MenuItem scanCurrentDocument;
		protected MenuItem addImageCurrentDoc;
		protected MenuItem delFoundItem;
		protected MenuItem markReadMessagesItem;
		protected MenuItem toPersonItem;

		#region Constructor & Instance

		protected DBDocsStyle(DocGrid grid)
			: base(grid)
		{
			newEFormItem = new MenuItem();
			linkEFormItem = new MenuItem();
			createAnswerItem = new MenuItem();
			toWorkItem = new MenuItem();
			endWorkItem = new MenuItem();
			moveItem = new MenuItem();
			deleteItem = new MenuItem();
			sendMessageItem = new MenuItem();

			scanCurrentDocument = new MenuItem();
			addImageCurrentDoc = new MenuItem();
			delFoundItem = new MenuItem();
			markReadMessagesItem = new MenuItem();
			toPersonItem = new MenuItem();

			//создание электорнной формы
			newEFormItem.Text = Environment.StringResources.GetString("DocGrid.Styles.DBDocsStyle.Message1") + "...";
			newEFormItem.Click += newEFormItem_Click;

			scanCurrentDocument.Text = Environment.StringResources.GetString("DocGrid.Styles.DBDocsStyle.Message12") + "...";
			scanCurrentDocument.Click += scanCurrentDocument_Click;

			addImageCurrentDoc.Text = Environment.StringResources.GetString("DocGrid.Styles.DBDocsStyle.Message13") + "...";
			addImageCurrentDoc.Click += addImageCurrentDoc_Click;
			//создание вытекающего документа
			linkEFormItem.Text = Environment.StringResources.GetString("DocGrid.Styles.DBDocsStyle.Message2") + "...";

			//создание ответа по документу
			createAnswerItem.Text = Environment.StringResources.GetString("DocGrid.Styles.DBDocsStyle.Message11") + "...";
			createAnswerItem.Click += createAnswerItem_Click;

			// добавить
			toWorkItem.Text = Environment.StringResources.GetString("DocGrid.Styles.DBDocsStyle.Message3") + "...";
			toWorkItem.Click += toWorkItem_Click;

			// завершить работу с документом
			endWorkItem.Text = Environment.StringResources.GetString("DocGrid.Styles.DBDocsStyle.Message4");
			endWorkItem.Shortcut = Shortcut.Del;
			endWorkItem.Click += endWorkItem_Click;

			// переместить/скопировать документ в другую папку
			moveItem.Text = Environment.StringResources.GetString("DocGrid.Styles.DBDocsStyle.Message5") + "...";
			moveItem.Click += moveItem_Click;

			// удалить документ из архива
			deleteItem.Text = Environment.StringResources.GetString("DocGrid.Styles.DBDocsStyle.Message6");
			deleteItem.Click += deleteItem_Click;

			// отправить сообщение по документу
			sendMessageItem.Text = Environment.StringResources.GetString("DocGrid.Styles.DBDocsStyle.Message7") + "...";
			sendMessageItem.Shortcut = Shortcut.F7;
			sendMessageItem.Click += sendMessageItem_Click;

			// удаление из найденного
			delFoundItem.Text = Environment.StringResources.GetString("DocGrid.Styles.DBDocsStyle.Message8");
			delFoundItem.Click += delFoundItem_Click;

			// отметить сообщени€ по документу как прочитанные
			markReadMessagesItem.Click += markReadMessagesItem_Click;

			// перейти к папке лица
			toPersonItem.Text = Environment.StringResources.GetString("DocGrid.Styles.DBDocsStyle.Message10") + "...";

			idField = Environment.DocData.IDField;
			keyField = Environment.DocData.IDField;
			needBoldField = Environment.DocData.WorkDocReadField;
			FriendlyName = Environment.StringResources.GetString("ArchiveDoc");
		}

		public static Style Instance(DocGrid grid)
		{
			return instance ?? (instance = new DBDocsStyle(grid));
		}

		#endregion

		#region Columns

		public override int GetColumnWidth(string colName)
		{
			if(colName == Environment.DocData.IDField)
				return 50;

			if(colName == Environment.DocData.DocTypeField)
				return 30;

			if(colName == Environment.DocData.NameField)
				return 120;

			if(colName == Environment.DocData.DateField)
				return 70;

			if(colName == Environment.DocData.NumberField)
				return 70;

			if(colName == Environment.DocData.DescriptionField)
				return 220;

			if(colName == Environment.DocData.SpentField)
				return 30;

			if(colName == Environment.DocData.WorkDocReadField)
				return 20;

			return base.GetColumnWidth(colName);
		}

		public override string GetColumnHeaderName(string colName)
		{
			if(colName == Environment.DocData.IDField)
				return Environment.StringResources.GetString("ID");

			if(colName == Environment.DocData.DocTypeField)
				return Environment.StringResources.GetString("DocumentType");

			if(colName == Environment.DocData.NameField)
				return Lib.Win.Document.Environment.StringResources.GetString("DocumentName");

			if(colName == Environment.DocData.DateField)
				return Environment.StringResources.GetString("Date");

			if(colName == Environment.DocData.NumberField)
				return Environment.StringResources.GetString("DocumentNumber");

			if(colName == Environment.DocData.DescriptionField)
				return Environment.StringResources.GetString("Description");

			if(colName == Environment.DocData.SpentField)
				return Environment.StringResources.GetString("Spent");

			if(colName == Environment.DocData.WorkDocReadField)
				return Environment.StringResources.GetString("Read");

			return base.GetColumnHeaderName(colName);
		}

		public override bool IsColumnBool(string colName)
		{
			if(colName == Environment.DocData.WorkDocReadField)
				return true;
			return base.IsColumnBool(colName);
		}

		public override bool IsColumnSystem(string column)
		{
			return base.IsColumnSystem(column) || (column != Environment.DocData.IDField &&
				column != Environment.DocData.DocTypeField &&
				column != Environment.DocData.NameField &&
				column != Environment.DocData.DateField &&
				column != Environment.DocData.NumberField &&
				column != Environment.DocData.DescriptionField &&
				column != Environment.DocData.SpentField &&
				column != Environment.DocData.WorkDocReadField);
		}

		#endregion

		#region Init

		public override void Init()
		{
			base.Init();
			try
			{
				toPersonItem.Text = Environment.StringResources.GetString("DocGrid.Styles.DBDocsStyle.Message10") + " " +
									Environment.PersonWord.GetForm(Cases.R, false, false) + "...";

				if(grid.Columns.Contains(Environment.DocData.DateField))
					grid.Columns[Environment.DocData.DateField].DefaultCellStyle.Format = "dd.MM.yyyy";
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		#endregion

		#region Context Menu

		protected virtual ContextMenu AddOwnMenu(ContextMenu menu)
		{
			if(grid.Columns.Contains(Environment.WorkDocData.ReadField))
			{
				bool isRead = grid.GetSelectedBoolValuesSummary(Environment.WorkDocData.ReadField);
				markReadMessagesItem.Text = Environment.StringResources.GetString(isRead ? "DocGrid.Styles.DBDocsStyle.Message90" : "DocGrid.Styles.DBDocsStyle.Message9");
				if(grid.IsSingle)
					markReadMessagesItem.Enabled = grid.Columns.Contains(Environment.DocData.MessageField) ? grid.GetCurValue(Environment.DocData.MessageField) is DateTime : true;
				else
				{
					if(grid.IsMultiple && isRead)
						markReadMessagesItem.Enabled = false;
					else
						markReadMessagesItem.Enabled = true;
				}
				menu.MenuItems.AddRange(new[] { markReadMessagesItem });
			}
			return menu;
		}

		public override ContextMenu BuildContextMenu()
		{
			var contextMenu = new ContextMenu();

			if(grid.IsSingle)
			{
				contextMenu.MenuItems.AddRange(new[] { scanCurrentDocument, addImageCurrentDoc, linkEFormItem, separator.CloneMenu(), savePartItem, saveSelectedItem, separator.CloneMenu(), newEFormItem, separator.CloneMenu(), sendMessageItem, createAnswerItem, separator.CloneMenu() });

				if(grid.MainForm != null)
					saveSelectedItem.Enabled = grid.MainForm.docControl.RectDrawn() && grid.MainForm.docControl.CanSendOut;
				if(Environment.CmdManager.Commands.Contains("SavePart"))
					savePartItem.Enabled = grid.MainForm.docControl.CompliteLoading && Environment.CmdManager.Commands["SavePart"].Enabled;

				int docID = grid.GetCurID();
				var docTypeID = (int)grid.GetCurValue(Environment.DocData.DocTypeIDField);

				newEFormItem.Enabled = (!Environment.DocDataData.IsDataPresent(docID) && Environment.DocTypeData.GetDocBoolField(Environment.DocTypeData.FormPresentField, docTypeID));

				linkEFormItem.MenuItems.Clear();

				createAnswerItem.Enabled = Environment.DocTypeData.GetDocBoolField(Environment.DocTypeData.AnswerFormField, docTypeID);

				DataRow dar = null;
				using(DataTable typesTable = Environment.DocTypeLinkData.GetLinkedTypes(docTypeID))
				{
					for(int j = 0; j < typesTable.Rows.Count; j++)
					{
						dar = typesTable.Rows[j];
						IDMenuItem it = new IDMenuItem((int)dar[Environment.DocTypeLinkData.ChildTypeIDFeild]);
						it.Text = dar[Environment.DocTypeLinkData.NameField] + ((DBNull.Value.Equals(dar[Environment.FieldData.NameField]) ? "" : ("(" + Environment.StringResources.GetString("OnField") + " " + dar[Environment.FieldData.NameField]) + ")"));
						it.Tag = dar;
						it.Click += linkEFormItem_Click;
						linkEFormItem.MenuItems.Add(it);
					}
				}

				linkEFormItem.Enabled = (linkEFormItem.MenuItems.Count > 0);

				toPersonItem.MenuItems.Clear();

				// есть ли в группировке лица?
				if(Environment.UserSettings.GroupOrder.Contains(CatalogNode.PersonInitial))
				{
					using(DataTable personTable = Environment.DocData.GetDocPersonsLite(docID, false))
					using(DataTableReader dr = personTable.CreateDataReader())
					{
						Dictionary<int, IDMenuItem> dic = new Dictionary<int, IDMenuItem>();

						while(dr.Read())
						{
							var personID = (int)dr[Environment.PersonData.IDField];
							if(Environment.UserSettings.PersonID != personID || personTable.Rows.Count == 1)
							{
								if(dic.ContainsKey(personID))
									continue;
								IDMenuItem item = new IDMenuItem(personID);
								item.Text = dr[Environment.PersonData.NameField] as string ??
									Environment.StringResources.GetString("MainForm.MainFormDialog.menuDoc_Popup.Message1");
								item.Click += toPerson_Click;
								dic.Add(personID, item);
							}
						}
						dr.Close();
						dr.Dispose();
						personTable.Dispose();
						if(dic.Count > 0)
							toPersonItem.MenuItems.AddRange(dic.Values.ToArray());
						dic.Clear();
					}
				}

				contextMenu = AddOwnMenu(contextMenu);

				int empID = (grid.MainForm != null) ? grid.MainForm.curEmpID : Environment.CurEmp.ID;

				if(!Environment.WorkDocData.IsInWork(docID, empID))
					contextMenu.MenuItems.Add(toWorkItem);
				else
					contextMenu.MenuItems.AddRange(new[] { moveItem, separator.CloneMenu(), endWorkItem });

				contextMenu.MenuItems.AddRange(new[] { separator.CloneMenu(), toPersonItem, openInNewWindowItem, separator.CloneMenu(), deleteItem, separator.CloneMenu(), refreshItem, separator.CloneMenu(), propertiesItem });

				toPersonItem.Enabled = (toPersonItem.MenuItems.Count > 0);

				deleteItem.Enabled = (Environment.EmpData.IsDocDeleter() || !Environment.DocImageData.DocHasImages(docID, true));
			}
			else if(grid.IsMultiple)
			{
				contextMenu = AddOwnMenu(contextMenu);

				contextMenu.MenuItems.AddRange(new[] { separator.CloneMenu(), sendMessageItem, separator.CloneMenu() });

				if(grid.IsInWork())
					contextMenu.MenuItems.AddRange(new[] { moveItem, separator.CloneMenu(), endWorkItem });
				else
					contextMenu.MenuItems.Add(toWorkItem);

				contextMenu.MenuItems.AddRange(new[] { separator.CloneMenu(), openInNewWindowItem });
			}
			else if(grid.SelectedRows.Count > 500)
				MessageBox.Show(Environment.CurCultureInfo.Name.Equals("ru") ? "¬ыделено слишком много документов! ¬ыберите, пожалуйста, чуть меньше документов дл€ работы." : "Too many rows have been selected!");

			return contextMenu.MenuItems.Count > 0 ? contextMenu : null;
		}

		private void toWorkItem_Click(object sender, EventArgs e)
		{
			Environment.CmdManager.Commands["AddToWork"].Execute();
		}

		private void endWorkItem_Click(object sender, EventArgs e)
		{
			Environment.CmdManager.Commands["EndWork"].Execute();
		}

		private void moveItem_Click(object sender, EventArgs e)
		{
			Environment.CmdManager.Commands["WorkPlaces"].Execute();
		}

		private void deleteItem_Click(object sender, EventArgs e)
		{
			Environment.CmdManager.Commands["DeleteDoc"].Execute();
		}

		public void sendMessageItem_Click(object sender, EventArgs e)
		{
			Environment.CmdManager.Commands["SendMessage"].Execute();
		}

		private void delFoundItem_Click(object sender, EventArgs e)
		{
			Environment.CmdManager.Commands["DeleteFromFound"].Execute();
		}

		private void markReadMessagesItem_Click(object sender, EventArgs e)
		{
			Environment.CmdManager.Commands["MarkReadMessages"].Execute();
		}

		private void toPerson_Click(object sender, EventArgs e)
		{
			try
			{
				var item = sender as IDMenuItem;
				if(item == null)
					return;
				Forms.MainFormDialog.nextPersonID = item.ID;
				Environment.CmdManager.Commands["SelectPersonFolder"].Execute();
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void newEFormItem_Click(object sender, EventArgs e)
		{
			Environment.CmdManager.Commands["AddEForm"].Execute();
		}

		#endregion

		#region Is

		public override bool IsShownInSettings()
		{
			return true;
		}

		public override bool IsDBDocs()
		{
			return true;
		}

		public override bool IsDBDocsFullAccess()
		{
			return true;
		}

		#endregion

		#region CurDocString

		public override string MakeDocString(int row)
		{
			string s = "";
			try
			{
				{// doc type
					var name = grid.GetValue(row, Environment.DocData.NameField) as string;
					var type = grid.GetValue(row, Environment.DocData.DocTypeField) as string;

					if(!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(type) &&
						name.ToLower().Equals(type.ToLower()))
						s = TextProcessor.StuffSpace(s) + name;
					else if(!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(type) &&
							 !name.ToLower().Equals(type.ToLower()))
						s = TextProcessor.StuffSpace(s) + name + " / " + type;
					else if(string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(type))
						s = TextProcessor.StuffSpace(s) + type;
				}

				{// number
					string val = grid.GetValue(row, Environment.DocData.NumberField) as string;
					if(!string.IsNullOrEmpty(val))
						s = TextProcessor.StuffSpace(s) + Environment.StringResources.GetString("Num") + val;
				}

				{// date	
					object obj = grid.GetValue(row, Environment.DocData.DateField);
					if(obj is DateTime)
					{
						s = TextProcessor.StuffSpace(s) + Environment.StringResources.GetString("Of") +
							((DateTime)obj).ToString("dd.MM.yyyy");
					}
				}

				{// description
					string val = grid.GetValue(row, Environment.DocData.DescriptionField) as string;
					if(!string.IsNullOrEmpty(val))
						s = TextProcessor.StuffSpace(s) + "(" + val + ")";
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}

			return s;
		}

		#endregion

		private void linkEFormItem_Click(object sender, EventArgs e)
		{
			if(sender is IDMenuItem)
				try
				{
					var item = sender as IDMenuItem;
					int typeID = item.ID;
					var fieldID = (int)(item.Tag as DataRow)[Environment.DocLinksData.SubFieldIDField];
					int docID = grid.GetCurID();
					string searchString = "SELECT " + Environment.DocData.IDField + " FROM " +
										  Environment.DocData.TableName + " T0 " +
										  "WHERE (EXISTS (SELECT * FROM " + Environment.DocLinksData.TableName + " TI WHERE TI." + Environment.DocLinksData.ParentDocIDField + "=" + docID.ToString() +
										  " AND TI." + Environment.DocLinksData.ChildDocIDField + "=T0." + Environment.DocData.IDField +
										  " AND TI." + Environment.DocLinksData.SubFieldIDField + " = " + fieldID.ToString() + ")) AND (T0." + Environment.DocData.DocTypeIDField + " = " + typeID.ToString() + ")";

					DataTable dt = Environment.DocData.GetDocsByIDQuery(searchString, Environment.CurCultureInfo.Name);
					if(dt != null && dt.Rows.Count > 0)
					{
						Dialogs.ConfirmTypeDialog dialog = new Dialogs.ConfirmTypeDialog
										 {
											 TypeID = typeID,
											 DocID = docID,
											 FieldID = fieldID,
											 TypeString = item.Text,
											 SearchString = searchString
										 };
						dialog.DialogEvent += dialog_DialogEvent;
						dialog.Show();
					}
					else
					{
						CreateNewDoc(typeID, docID, fieldID);
					}
				}
				catch(Exception ex)
				{
					Lib.Win.Data.Env.WriteToLog(ex);
				}
		}

		private void dialog_DialogEvent(object source, Kesco.Lib.Win.DialogEventArgs e)
		{
			if(e.Dialog is Dialogs.ConfirmTypeDialog)
			{
				Dialogs.ConfirmTypeDialog dialog = e.Dialog as Dialogs.ConfirmTypeDialog;
				if(dialog.DialogResult == DialogResult.Yes)
					CreateNewDoc(dialog.TypeID, dialog.DocID, dialog.FieldID);
			}
		}

		private void CreateNewDoc(int typeID, int docID, int fieldID)
		{
			string url = Environment.DocTypeData.GetField(Environment.DocTypeData.URLField, typeID).ToString();
			string paramStr = ((Lib.Win.Document.Environment.PersonID > 0) ? ("&currentperson=" + Lib.Win.Document.Environment.PersonID.ToString()) : "") + ((docID > 0) ? "&docID=" + docID.ToString() + ((fieldID > 0) ? "&fieldID=" + fieldID.ToString() : "") : "");
			if(string.IsNullOrEmpty(url))
			{
				Kesco.Lib.Win.MessageForm.Show("Ќет формы создани€ документа");
				return;
			}
			if(paramStr.Length > 0)
			{
				url += (url.IndexOf("?") > 0) ? "&" : "?";
				url += paramStr.TrimStart('&');
			}
			Lib.Win.Document.Environment.IEOpenOnURL(url);
		}

		private void createAnswerItem_Click(object sender, EventArgs e)
		{
			int docID = grid.GetCurID();
			var docTypeID = (int)grid.GetCurValue(Environment.DocData.DocTypeIDField);
			int answerTypeID = Environment.DocTypeData.GetDocIntField(Environment.DocTypeData.AnswerFormField, docTypeID, -1);
			if(answerTypeID > -1)
			{
				string url = Environment.DocTypeData.GetField(Environment.DocTypeData.URLField, answerTypeID).ToString();
				string paramStr = ((Lib.Win.Document.Environment.PersonID > 0) ? ("&currentperson=" + Lib.Win.Document.Environment.PersonID.ToString()) : "") + ((docID > 0) ? "&docID=" + docID.ToString() : "");
				if(string.IsNullOrEmpty(url))
				{
					MessageBox.Show("Ќет формы создани€ документа");
					return;
				}
				if(paramStr.Length > 0)
				{
					url += (url.IndexOf("?") > 0) ? "&" : "?";
					url += paramStr.TrimStart('&');
				}
				Lib.Win.Document.Environment.IEOpenOnURL(url);
			}
		}

		private void scanCurrentDocument_Click(object sender, EventArgs e)
		{
			Environment.CmdManager.Commands["ScanCurrentDocument"].Execute();
		}

		private void addImageCurrentDoc_Click(object sender, EventArgs e)
		{
			Environment.CmdManager.Commands["AddImageCurrentDoc"].Execute();
		}

		internal static bool HasInstance()
		{
			return instance != null;
		}

		internal static void DropInstance()
		{
			if(instance != null)
				instance = null;
		}
	}
}