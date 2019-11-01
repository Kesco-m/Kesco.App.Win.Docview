using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Kesco.App.Win.DocView.Misc;
using Kesco.Lib.Win;
using Kesco.Lib.Win.Document.Dialogs;
using Kesco.Lib.Win.Document.Items;

namespace Kesco.App.Win.DocView.Dialogs
{
	public class SimilarDocsDialog : FreeDialog
	{
		private int docID;
		private DataTable table;
		private int typeID;
		private string typeStr;
		private string personIDs;
		private byte searchType;

		private byte internalSearchType = 2;

		private ContextMenu contextMenu;

		private MenuItem openItem;
		private MenuItem rightsItem;

		private RichTextBox labelX;
		private ListView list;
		private Label label2;
		private Label label1;
		private Button buttonMerge;
		private Button buttonLeave;
		private Button buttonCancel;
		private Label labelSame;
		private Button btnSearchByNumber;
		private Button btnSearchByPerson;

		private Container components;

		public SimilarDocsDialog(DataTable table, int typeID, string typeStr, string nameStr, string numberStr,
								 DateTime date, bool protectedDoc, string personIDs, int docID, string descrStr, int personCount)
		{
			InitializeComponent();

			this.table = table;
			this.typeID = typeID;
			this.typeStr = typeStr;
			NameStr = nameStr;
			NumberStr = numberStr;
			DocDate = date;
			this.personIDs = personIDs;
			this.docID = docID;
			ProtectedDoc = protectedDoc;
			DescrStr = descrStr;
			PersonCount = personCount;

			// контекстное меню
			contextMenu = new ContextMenu();

			// пункты меню
			openItem = new MenuItem();
			rightsItem = new MenuItem();

			// открыть
			openItem.Text = Environment.StringResources.GetString("Show");
			openItem.Click += openItem_Click;

			// права
			rightsItem.Text = Environment.StringResources.GetString("Rights");
			rightsItem.Click += rightsItem_Click;
		}

		#region Accessors

		public DocListItem SelectedItem
		{
			get { return list.SelectedItems.Count > 0 ? (DocListItem)list.SelectedItems[0] : null; }
		}

		public int MainDocID { get; private set; }

		public int SecDocID { get; private set; }

		public string NameStr { get; private set; }

		public string NumberStr { get; private set; }

		public DateTime DocDate { get; private set; }

		public bool ProtectedDoc { get; private set; }

		public string DescrStr { get; private set; }

		public int PersonCount { get; private set; }

		public byte SearchType
		{
			set
			{
				searchType = value;
				//если тип 2, то видимость остаётся у 2х кнопок
				btnSearchByPerson.Enabled = true;
				btnSearchByNumber.Enabled = true;
				if(value.Equals(1))
					btnSearchByNumber.Enabled = false;
				if(value.Equals(0))
				{
					btnSearchByPerson.Enabled = false;
					btnSearchByNumber.Enabled = false;
				}
			}
			get { return searchType; }
		}

		#endregion

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
				new System.ComponentModel.ComponentResourceManager(typeof(SimilarDocsDialog));
			this.labelX = new System.Windows.Forms.RichTextBox();
			this.list = new System.Windows.Forms.ListView();
			this.buttonMerge = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonLeave = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.labelSame = new System.Windows.Forms.Label();
			this.btnSearchByNumber = new System.Windows.Forms.Button();
			this.btnSearchByPerson = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// labelX
			// 
			resources.ApplyResources(this.labelX, "labelX");
			this.labelX.BackColor = System.Drawing.SystemColors.Control;
			this.labelX.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.labelX.Cursor = System.Windows.Forms.Cursors.Default;
			this.labelX.DetectUrls = false;
			this.labelX.Name = "labelX";
			this.labelX.ReadOnly = true;
			this.labelX.TabStop = false;
			// 
			// list
			// 
			resources.ApplyResources(this.list, "list");
			this.list.FullRowSelect = true;
			this.list.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.list.HideSelection = false;
			this.list.MultiSelect = false;
			this.list.Name = "list";
			this.list.UseCompatibleStateImageBehavior = false;
			this.list.View = System.Windows.Forms.View.Details;
			this.list.SelectedIndexChanged += new System.EventHandler(this.list_SelectedIndexChanged);
			this.list.DoubleClick += new System.EventHandler(this.list_DoubleClick);
			this.list.MouseUp += new System.Windows.Forms.MouseEventHandler(this.list_MouseUp);
			// 
			// buttonMerge
			// 
			resources.ApplyResources(this.buttonMerge, "buttonMerge");
			this.buttonMerge.Name = "buttonMerge";
			this.buttonMerge.Click += new System.EventHandler(this.buttonMerge_Click);
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label2.Name = "label2";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label1.Name = "label1";
			// 
			// buttonLeave
			// 
			resources.ApplyResources(this.buttonLeave, "buttonLeave");
			this.buttonLeave.Name = "buttonLeave";
			this.buttonLeave.Click += new System.EventHandler(this.buttonLeave_Click);
			// 
			// buttonCancel
			// 
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// labelSame
			// 
			resources.ApplyResources(this.labelSame, "labelSame");
			this.labelSame.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.labelSame.Name = "labelSame";
			// 
			// btnSearchByNumber
			// 
			resources.ApplyResources(this.btnSearchByNumber, "btnSearchByNumber");
			this.btnSearchByNumber.Name = "btnSearchByNumber";
			this.btnSearchByNumber.Click += new System.EventHandler(this.btSearchByNumber_Click);
			// 
			// btnSearchByPerson
			// 
			resources.ApplyResources(this.btnSearchByPerson, "btnSearchByPerson");
			this.btnSearchByPerson.Name = "btnSearchByPerson";
			this.btnSearchByPerson.Click += new System.EventHandler(this.btnSearchByPerson_Click);
			// 
			// SimilarDocsDialog
			// 
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.labelSame);
			this.Controls.Add(this.btnSearchByNumber);
			this.Controls.Add(this.btnSearchByPerson);
			this.Controls.Add(this.buttonMerge);
			this.Controls.Add(this.labelX);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.list);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonLeave);
			this.Controls.Add(this.buttonCancel);
			this.DoubleBuffered = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SimilarDocsDialog";
			this.Load += new System.EventHandler(this.ChooseDocDialog_Load);
			this.ResumeLayout(false);
		}

		#endregion

		private void UpdateControls()
		{
			btnSearchByPerson.FlatStyle = internalSearchType == 0 ? FlatStyle.Flat : FlatStyle.Standard;

			btnSearchByNumber.FlatStyle = internalSearchType == 1 ? FlatStyle.Flat : FlatStyle.Standard;

			buttonMerge.Enabled = list.SelectedItems.Cast<DocListItem>().Any(item => item.Rights);
		}

		private void ChooseDocDialog_Load(object sender, EventArgs e)
		{
			FillLV();
			btnSearchByPerson.Visible = searchType > 0;
			labelSame.Visible = btnSearchByPerson.Visible;
			btnSearchByNumber.Visible = searchType > 1;
			UpdateControls();
		}

		private void list_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateControls();
		}

		private void list_MouseUp(object sender, MouseEventArgs e)
		{
			if(e.Button != MouseButtons.Right || contextMenu == null)
				return;

			contextMenu.MenuItems.Clear();
			var item = (DocListItem)list.GetItemAt(e.X, e.Y);
			item.Selected = true;

			contextMenu.MenuItems.AddRange(item.Rights ? new[] { openItem, rightsItem } : new[] { rightsItem });

			if(contextMenu.MenuItems.Count > 0)
				contextMenu.Show(this, new Point(e.X + list.Left, e.Y + list.Top));
		}

		private void openItem_Click(object sender, EventArgs e)
		{
			if(list.SelectedItems.Count <= 0)
				return;

			var item = (DocListItem)list.SelectedItems[0];
			if(item.Rights)
				Environment.NewWindow(item.ID, Environment.ZoomString, new Context(ContextMode.Catalog));
		}

		private void rightsItem_Click(object sender, EventArgs e)
		{
			if(list.SelectedItems.Count <= 0)
				return;

			DocListItem item = (DocListItem)list.SelectedItems[0];
			DocUserListDialog dialog = new DocUserListDialog(item.ID);
			dialog.Show();
		}

		private void list_DoubleClick(object sender, EventArgs e)
		{
			openItem_Click(sender, e);
		}

		private void buttonMerge_Click(object sender, EventArgs e)
		{
			if(list.SelectedItems.Count != 1)
				return;

			var item = (DocListItem)list.SelectedItems[0];
			if(!item.Rights)
				return;

			int firstDocID = docID;
			int secondDocID = item.ID;

			// проверка, вдруг у одного документа только данные, а у другого - только изображения?
			bool firstDocData = Environment.DocDataData.IsDataPresent(firstDocID);
			bool secondDocData = Environment.DocDataData.IsDataPresent(secondDocID);
			bool firstDocImages = Environment.DocImageData.DocHasImages(firstDocID, true);
			bool secondDocImages = Environment.DocImageData.DocHasImages(secondDocID, true);

			// проверка, есть ли подписи
			bool firstDocSigned = Environment.DocSignatureData.IsDocSigned(firstDocID);
			bool secondDocSigned = Environment.DocSignatureData.IsDocSigned(secondDocID);

			if(firstDocSigned && secondDocSigned)
			{
				MessageBox.Show(Environment.StringResources.GetString("SimilarDocsDialog.buttonMerge_Click.Message1"),
					Environment.StringResources.GetString("JoinError"));
				return;
			}

			{
				// Если у документов разное описание, выбрать какое-то одно
				string secondDocDescription =
					Environment.DocData.GetField(Environment.DocData.DescriptionField, secondDocID).ToString();
				bool firstDescrStr = !string.IsNullOrEmpty(DescrStr);
				bool secondDescrStr = !string.IsNullOrEmpty(secondDocDescription);

				if(firstDescrStr && secondDescrStr)
				{
					using(Lib.Win.Document.Select.SimilarDocsDescriptionDialog dlg =
						new Lib.Win.Document.Select.SimilarDocsDescriptionDialog(DescrStr, secondDocDescription))
					if(dlg.ShowDialog(this) == DialogResult.OK)
					{
						DescrStr = dlg.Description;
					}
					else
						return;
				}
				else if(firstDescrStr && !secondDescrStr)
				{
					/* оставляем как есть*/
				}
				else if(!firstDescrStr && secondDescrStr)
				{
					DescrStr = secondDocDescription;
				}
				else if(!firstDescrStr && !secondDescrStr)
				{
					DescrStr = string.Empty;
				}
			}

			if((firstDocSigned && !secondDocSigned) || (!firstDocSigned && secondDocSigned))
			{
				MainDocID = firstDocSigned ? firstDocID : secondDocID;
				SecDocID = firstDocSigned ? secondDocID : firstDocID;

				End(DialogResult.OK);
			}
			else if((!firstDocData && secondDocData)
					 || (firstDocData && !secondDocData))
			{
				MainDocID = firstDocData ? firstDocID : secondDocID;
				SecDocID = firstDocData ? secondDocID : firstDocID;

				End(DialogResult.OK);
			}
			else
			{
				MergeDocsDialog dialog = new MergeDocsDialog(firstDocID, secondDocID);
				dialog.DialogEvent += MergeDocsDialog_DialogEvent;
				dialog.Show();
				Enabled = false;
			}
		}

		private void MergeDocsDialog_DialogEvent(object source, DialogEventArgs e)
		{
			Enabled = true;
			Focus();

			var dialog = e.Dialog as MergeDocsDialog;
			if(dialog != null && dialog.DialogResult == DialogResult.OK)
			{
				MainDocID = dialog.MainDocID;
				SecDocID = dialog.SecDocID;

				End(DialogResult.OK);
			}
		}

		private void buttonLeave_Click(object sender, EventArgs e)
		{
			End(DialogResult.No);
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			End(DialogResult.Cancel);
		}

		private void FillLV()
		{
			int count = table == null ? 0 : table.Rows.Count;
			string msg = Environment.StringResources.GetString("SimilarDocsDialog.ChooseDocDialog_Load.Message1") + " " +
						 count + " ";

			list.Items.Clear();

			if(count > 0 && table != null)
			{
				list.Visible = true;
				switch(count % 10)
				{
					case 1:
						if(Environment.CurCultureInfo.TwoLetterISOLanguageName.Equals("ru"))
						{
							if(count != 11)
								msg +=
									Environment.StringResources.GetString(
										"SimilarDocsDialog.ChooseDocDialog_Load.Message2");
							else
								msg +=
									Environment.StringResources.GetString(
										"SimilarDocsDialog.ChooseDocDialog_Load.Message4");
						}
						else
						{
							if(count == 1)
								msg +=
									Environment.StringResources.GetString(
										"SimilarDocsDialog.ChooseDocDialog_Load.Message2");
							else
								msg +=
									Environment.StringResources.GetString(
										"SimilarDocsDialog.ChooseDocDialog_Load.Message4");
						}
						break;

					case 2:
					case 3:
					case 4:
						msg += Environment.StringResources.GetString("SimilarDocsDialog.ChooseDocDialog_Load.Message3");
						break;

					case 5:
					case 6:
					case 7:
					case 8:
					case 9:
						msg += Environment.StringResources.GetString("SimilarDocsDialog.ChooseDocDialog_Load.Message4");
						break;
				}

				msg += " " + Environment.StringResources.GetString("To") + " " + typeStr + " " +
					   Environment.StringResources.GetString("Num");

				if(NumberStr != "")
					msg += NumberStr;
				else
					msg += Environment.StringResources.GetString("SimilarDocsDialog.ChooseDocDialog_Load.Message5");

				msg += " " + Environment.StringResources.GetString("Of") + " ";

				if(DocDate > DateTime.MinValue)
					msg += DocDate.ToString("dd.MM.yyyy");
				else
					msg += Environment.StringResources.GetString("SimilarDocsDialog.ChooseDocDialog_Load.Message6");

				if(list.Columns.Count == 0)
				{
					list.Columns.Add(Environment.StringResources.GetString("ID"), 60, HorizontalAlignment.Left);
					list.Columns.Add(Lib.Win.Document.Environment.StringResources.GetString("DocumentName"), 120,
									 HorizontalAlignment.Left);
					list.Columns.Add(Environment.StringResources.GetString("DocumentNumber"), 80,
									 HorizontalAlignment.Left);
					list.Columns.Add(Environment.StringResources.GetString("Description"), 180, HorizontalAlignment.Left);
					list.Columns.Add(
						Environment.StringResources.GetString("LoopLinkDialog.ChooseDocDialog_Load.Message2"), 130,
						HorizontalAlignment.Center);
				}

				foreach(DataRow dr in table.Rows)
				{
					var id = (int)dr[Environment.DocData.IDField];

					var values = new string[5];

					// код документа
					values[0] = id.ToString();

					// тип документа
					if(string.IsNullOrEmpty(dr[Environment.DocData.NameField].ToString()))
					{
						if(Environment.CurCultureInfo.TwoLetterISOLanguageName.Equals("ru"))
							values[1] = dr[Environment.DocData.DocTypeField].ToString();
						else if(table.Columns.Contains(Environment.DocData.DocTypeEngField))
							values[1] = dr[Environment.DocData.DocTypeEngField].ToString();
						else
							values[1] = dr[Environment.DocData.DocTypeField].ToString();
					}
					else
						values[1] = dr[Environment.DocData.NameField].ToString();

					object obj;
					// номер
					obj = dr[Environment.DocData.NumberField];
					if(!obj.Equals(DBNull.Value))
						values[2] = obj.ToString();

					// описание
					obj = dr[Environment.DocData.DescriptionField];
					if(!obj.Equals(DBNull.Value))
						values[3] = obj.ToString();

					// доступен?
					var available = (int)dr[Environment.DocData.RightsField];
					bool rights = (available == 1);
					if(rights)
						values[4] = Environment.StringResources.GetString("Available");
					else
						values[4] = Environment.StringResources.GetString("NotAvailable");

					var item = new DocListItem(id, rights, values);
					if(!item.Rights)
						item.ForeColor = Color.Gray;

					list.Items.Add(item);
				}
			}
			else
				list.Visible = false;
			labelX.Text = msg;
			UpdateControls();
		}

		private void btnSearchByPerson_Click(object sender, EventArgs e)
		{
			internalSearchType = (byte)(internalSearchType != 0 ? 0 : 2);
			UpdateControls();
			Search();
		}

		private void btSearchByNumber_Click(object sender, EventArgs e)
		{
			internalSearchType = (byte)(internalSearchType != 1 ? 1 : 2);
			UpdateControls();
			Search();
		}

		private void Search()
		{
			int search = -1;
			if(internalSearchType != 2)
				search = internalSearchType;
			if(table != null)
				table.Dispose();

			table = Environment.DocData.GetSimilarDocs(typeID, NumberStr, DocDate, personIDs, docID, search);

			FillLV();
		}
	}
}