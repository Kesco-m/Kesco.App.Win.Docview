using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kesco.Lib.Win.Data.Temp.Objects;
using Kesco.Lib.Win.Document;

namespace Kesco.App.Win.DocView.Grids
{
	/// <summary>
	/// Список документов в виде DataGrid'а
	/// </summary>
	public class DocGrid : Grid
	{
		private bool isInWork;
		CancellationTokenSource source = new CancellationTokenSource();
		Task<DataTable> task = null;

		public DocGrid()
		{
			MultiSelect = true;
			ShowCellToolTips = true;
			CellToolTipTextNeeded += DocGrid_CellToolTipTextNeeded;
			CellMouseUp += DocGrid_MouseUp;
			ColumnHeaderMouseClick += DocGridHeader_MouseClick;
			SizeChanged += DocGrid_SizeChanged;
		}

		void DocGrid_KeyUp(object sender, KeyEventArgs e)
		{
			if((e.Shift && e.KeyCode == Keys.Insert) || (e.Control && e.KeyCode == Keys.V))
			{
				DataObject dob = Clipboard.GetDataObject() as DataObject;
				if(dob.GetDataPresent("DocViewCopy"))
				{
					object obj = dob.GetData("DocViewCopy");
					int[] ids = ((object[])obj)[0] as int[];
					if(ids.Length > 0 && ids.Length < 4)
					{
						Environment.NewWindow(ids[0], Environment.ZoomString,(Context)((object[])obj)[1]);
					}
				}
				if(dob.GetDataPresent(DataFormats.Text))
				{
					string txt = dob.GetText();
				}
			}
		}

		#region BackGroundLoad

		private DataTable loader_FoundDocs(object obj)
		{

			var empID = obj as Employee;

			if(source.IsCancellationRequested)
			{
				source.Token.ThrowIfCancellationRequested();
				return null;
			}
#if AdvancedLogging
			using(Lib.Log.Logger.DurationMetter("DocGrid loader_FoundDocs empID= " + empID))
#endif
			{
				DataTable dt = Environment.DocData.GetFoundDocs(Environment.CurCultureInfo.Name, empID.ID, Environment.UserSettings.PersonID, source.Token);
				if(source.IsCancellationRequested)
				{
					source.Token.ThrowIfCancellationRequested();
					return null;
				}
				else
					return dt;
			}
		}

		private DataTable loader_QueryDocs(object obj)
		{
			var xml = (string)((object[ ])obj)[0];
			var empID = (int)((object[ ])obj)[1];
			CancellationToken ct = (CancellationToken)((object[ ])obj)[2];
			string sql = Lib.Win.Data.DALC.Documents.Search.Options.GetSQL(xml);

			if(ct.IsCancellationRequested)
			{
				ct.ThrowIfCancellationRequested();
				return null;
			}

#if AdvancedLogging
			using(Lib.Log.Logger.DurationMetter("DocGrid loader_QueryDocs empID= " + empID + " ;xml= " + xml))
#endif
			{
				DataTable dt = Environment.DocData.GetQueryDocs(sql, Environment.CurCultureInfo.TwoLetterISOLanguageName, empID, Environment.UserSettings.PersonID, ct);
				if(!ct.IsCancellationRequested)
					return dt;
				else
				{
					ct.ThrowIfCancellationRequested();
					return null;
				}
			}
		}

		private DataTable loader_WorkFolderDocs(object obj)
		{
			var id = (int)((object[ ])obj)[0];
			var emp = (Employee)((object[ ])obj)[1];
			CancellationToken ct = (CancellationToken)((object[ ])obj)[2];
			if(ct.IsCancellationRequested)
			{
				ct.ThrowIfCancellationRequested();
				return null;
			}

#if AdvancedLogging
			using(Lib.Log.Logger.DurationMetter("DocGrid loader_WorkFolderDocs id= " + id))
#endif
			{
				DataTable dt = Environment.DocData.GetWorkFolderDocs(Environment.CurCultureInfo.Name, Environment.UserSettings.PersonID, emp.ID, id, ct);
				if(ct.IsCancellationRequested)
				{
					ct.ThrowIfCancellationRequested();
					return null;
				}
				else
					return dt;
			}
		}

		//private void loader_SharedWorkFolderDocs(object sender, DoWorkEventArgs e)
		//{
		//    loader.DoWork -= loader_SharedWorkFolderDocs;

		//    var id = (int)((object[ ])e.Argument)[0];
		//    var emp = (Employee)((object[ ])e.Argument)[1];

		//    if(loader.CancellationPending)
		//        return;

		//#if AdvancedLogging
		//	using(Lib.Log.Logger.DurationMetter("DocGrid loader_SharedWorkFolderDocs id= " + id))
		//#endif
		//    {
		//        DataTable dt = Environment.DocData.GetWorkFolderDocs(Environment.CurCultureInfo.Name, Environment.UserSettings.PersonID, emp.ID, id, out loader.Cmd);
		//        if(!loader.CancellationPending)
		//            e.Result = dt;
		//    }
		//}

		//private void loader_FullAccessFolderDocs(object sender, DoWorkEventArgs e)
		//{
		//    loader.DoWork -= loader_FullAccessFolderDocs;

		//    var id = (int)((object[ ])e.Argument)[0];
		//    var emp = (Employee)((object[ ])e.Argument)[1];

		//    if(loader.CancellationPending)
		//        return;

		//#if AdvancedLogging
		//			using (Lib.Log.Logger.DurationMetter("DocGrid loader_FullAccessFolderDocs id= " + id))
		//#endif
		//    {
		//        DataTable dt = Environment.DocData.GetWorkFolderDocs(Environment.CurCultureInfo.Name, Environment.UserSettings.PersonID, emp.ID, id, out loader.Cmd);
		//        if(!loader.CancellationPending)
		//            e.Result = dt;
		//    }
		//}

		private DataTable loader_DiskImages(object obj)
		{

			var path = (string)((object[])obj)[0];
			CancellationToken ct = (CancellationToken)((object[])obj)[1];
			var ir = new ImageRead.ImageReader(path);

			if(ct.IsCancellationRequested)
			{
				ct.ThrowIfCancellationRequested();
				return null;
			}

#if AdvancedLogging
			using(Lib.Log.Logger.DurationMetter("DocGrid loader_DiskImages path= " + path))
#endif
			{
				DataTable dt = ir.GetImages();
				if(!ct.IsCancellationRequested)
					return dt;
				else
				{
					ct.ThrowIfCancellationRequested();
					return null;
				}
			}
		}

		private DataTable loader_Scans(object obj)
		{
			var path = (string)((object[ ])obj)[0];
			CancellationToken ct = (CancellationToken)((object[ ])obj)[1];
			DateTime yesterday = DateTime.Today.Subtract(TimeSpan.FromDays(1));

			if(ct.IsCancellationRequested)
			{
				ct.ThrowIfCancellationRequested();
				return null;
			}

			var sr = new ImageRead.ScanReader(path);

			if(ct.IsCancellationRequested)
			{
				ct.ThrowIfCancellationRequested();
				return null;
			}

#if AdvancedLogging
			using(Lib.Log.Logger.DurationMetter("DocGrid loader_Scans path= " + path))
#endif
			{
				DataTable dt = sr.GetImages(yesterday);
				if(!ct.IsCancellationRequested)
					return dt;
				else
				{
					ct.ThrowIfCancellationRequested();
					return null;
				}
			}
		}

		private DataTable loader_CatalogDocs(object state)
		{
			string path = state.ToString();
			if(source.Token.IsCancellationRequested)
			{
				source.Token.ThrowIfCancellationRequested();
				return null;
			}

#if AdvancedLogging
			using(Lib.Log.Logger.DurationMetter("DocGrid loader_CatalogDocs Path = " + path))
#endif
			{
				DataTable dt = Environment.DocData.GetDocs(path, source.Token);
				if(!source.Token.IsCancellationRequested)
					return dt;
				else
				{
					source.Token.ThrowIfCancellationRequested();
					return null;
				}
			}
		}

		///<summary>
		///загрузка данных в грид
		///</summary>
		///<param name="sender"></param>
		///<param name="e"></param>
		private void loader_Load(Task<DataTable> ta)
		{
			if(task == ta)
				task = null;
			if(ta.Status != TaskStatus.RanToCompletion)
				return;
			DataTable dt = ta.Result as DataTable;
			if(dt == null)
				return;

			if(this.InvokeRequired)
			{
				this.Invoke((MethodInvoker)(() =>
				{
					if(Style.LoadData(dt))
						AutoWidth();
					else
						return;
				}));
			}
			else
			{
				if(Style.LoadData(dt))
					AutoWidth();
				else
					return;

			}

			if(this.InvokeRequired)
				Invoke((MethodInvoker)(ReturnCursorsToDefault));
			else
				ReturnCursorsToDefault();
		}

		/// <summary>
		/// перезагрезка данных в гриде
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void loader_Reload(Task<DataTable> ta)
		{
			if(task == ta)
				task = null;
			if(ta.Status != TaskStatus.RanToCompletion)
				return;
			var dt = ta.Result as DataTable;
			if(dt != null && Style != null)
			{
				if(this.InvokeRequired)
				{
					this.Invoke((MethodInvoker)(() =>
					{
						if(IsFine && Rows.Count > 0)
							Style.ReloadData(dt);
						else if(Style.LoadData(dt))
							AutoWidth();
					}));
				}
				else
				{
					if(IsFine && Rows.Count > 0)
						Style.ReloadData(dt);
					else if(Style.LoadData(dt))
						AutoWidth();
				}
			}
			ReturnCursorsToDefault();
		}

		/// <summary>
		/// Возвращает курсоры в основном окне в нормальное состояние
		/// </summary>
		private void ReturnCursorsToDefault()
		{
			if(this.InvokeRequired)
			{
				this.Invoke((MethodInvoker)ReturnCursorsToDefault);
				return;
			}
			if(MainForm != null)
			{
				MainForm.StatusBar_UpdateDocCount();
				MainForm.folders.Cursor = Cursors.Default;
				MainForm.docControl.UseLock = Style.UseLock();
				if(IsFaxesIn())
					MainForm.folders.UpdateFaxInStatus();
				if(IsFaxesOut())
					MainForm.folders.UpdateFaxOutStatus();
			}

			Cursor = Cursors.Default;
		}

		#endregion

		#region CurDocString

		public string MakeCurDocString()
		{
			return Style.MakeCurDocString();
		}

		public string[ ] MakeCurDocsString()
		{
			var s = new string[SelectedRows.Count];

			for(int i = 0; i < SelectedRows.Count; i++)
				s[i] = Style.MakeDocString(SelectedRows[i].Index);

			return s;
		}

		public string MakeCurDBDocString()
		{
			return Format(CurrentRowIndex);
		}

		public string[ ] MakeCurDBDocsStrings()
		{
			var s = new string[SelectedRows.Count];

			for(int i = 0; i < SelectedRows.Count; i++)
				s[i] = Format(SelectedRows[i].Index);

			return s;
		}

		public string MakeCurDBDocsString()
		{
			var sb = new StringBuilder();

			for(int i = 0; i < SelectedRows.Count; i++)
			{
				sb.Append(Format(SelectedRows[i].Index));
				sb.Append("\n");
			}
			return sb.ToString();
		}

		public string MakeCurDBDocsStringForNotRead()
		{
			var sb = new StringBuilder();

			for(int i = 0; i < SelectedRows.Count; i++)
			{
				if(Columns.Contains(Environment.DocData.WorkDocReadField) &&
					(this[Environment.DocData.WorkDocReadField, SelectedRows[i].Index].Value.Equals(0) ||
					this[Environment.DocData.WorkDocReadField, SelectedRows[i].Index].Value.Equals(false)))
				{
					sb.Append(Format(SelectedRows[i].Index));
					sb.Append("\n");
				}
			}
			return sb.ToString();
		}

		private string Format(int rowIndex)
		{
			string s = null;

			if(rowIndex > -1 && rowIndex < Rows.Count && Environment.DocData != null)
			{
				string someValue;

				// id
				s = "[" + (int)this[Environment.DocData.IDField, rowIndex].Value + "]";

				// doc type
				{
					var name = this[Environment.DocData.NameField, rowIndex].Value as string;
					var type = this[Environment.DocData.DocTypeField, rowIndex].Value as string;

					if(!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(type) && name.ToLower().Equals(type.ToLower()))
						s = TextProcessor.StuffSpace(s) + name;
					else if(!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(type) && !name.ToLower().Equals(type.ToLower()))
						s = TextProcessor.StuffSpace(s) + name + " / " + type;
					else if(string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(type))
						s = TextProcessor.StuffSpace(s) + type;
				}
				// number
				someValue = this[Environment.DocData.NumberField, rowIndex].Value as string;
				if(!string.IsNullOrEmpty(someValue))
					s = TextProcessor.StuffSpace(s) + Environment.StringResources.GetString("Num") + someValue;

				// date
				if(this[Environment.DocData.DateField, rowIndex].Value != null && !this[Environment.DocData.DateField, rowIndex].Value.Equals(DBNull.Value) && this[Environment.DocData.DateField, rowIndex].Value is DateTime)
					s = TextProcessor.StuffSpace(s) + Environment.StringResources.GetString("Of") + " " +
						((DateTime)this[Environment.DocData.DateField, rowIndex].Value).ToString("dd.MM.yyyy");

				// descr
				someValue = this[Environment.DocData.DescriptionField, rowIndex].Value as string;
				if(!string.IsNullOrEmpty(someValue))
					s = TextProcessor.StuffSpace(s) + "(" + someValue + ")";
			}
			return s;
		}

		public string DocStringToSave()
		{
			return Style.DocStringToSave();
		}

		#endregion

		#region Is

		public bool IsDBDocs()
		{
			return (Style != null && Style.IsDBDocs());
		}

		public bool IsWorkFolder()
		{
			return (Style != null && Style.IsWorkFolder());
		}

		public bool IsDBDocsFullAccess()
		{
			return (Style != null && Style.IsDBDocsFullAccess());
		}

		public bool IsFaxes()
		{
			return (Style != null && Style.IsFaxes());
		}

		public bool IsFaxesIn()
		{
			return (Style != null && Style.IsFaxesIn());
		}

		public bool IsFaxesOut()
		{
			return (Style != null && Style.IsFaxesOut());
		}

		public bool IsScaner()
		{
			return (Style != null && Style.IsScaner());
		}

		public bool IsDiskImages()
		{
			return (Style != null && Style.IsDiskImages());
		}

		public bool IsFound()
		{
			return (Style != null && Style.IsFound());
		}

		public override bool IsSpam()
		{
			if(!IsFaxes())
				return false;

			bool spam = true;
			for(int i = 0; (i < SelectedRows.Count) && spam; i++)
				spam = spam && GetBoolValue(SelectedRows[i].Index, Environment.FaxData.SpamField);

			return spam;
		}

		public override bool IsInWork()
		{
			return isInWork;
		}

		public void UpdateIsInWorkStatus()
		{
			if(!IsFine)
				return;

		
#if AdvancedLogging
           	try
			{    
				Lib.Log.Logger.EnterMethod(this, "DocGrid UpdateIsInWorkStatus()");
#endif

				if(IsWorkFolder())
					isInWork = true;
				else if(IsDBDocs() && SelectedRows.Count > 0)
				{
					int empID = MainForm != null ? MainForm.curEmpID : Environment.CurEmp.ID;

					if(IsSingle)
						isInWork = Environment.WorkDocData.IsInWork(GetCurID(), empID);
					else if(IsMultiple)
						isInWork = Environment.WorkDocData.IsInWork(string.Join(",", GetCurIDs().Select(id => id.ToString()).ToArray()), empID);
				}
				else
					isInWork = false;
#if AdvancedLogging
			}
			finally
			{
				Lib.Log.Logger.LeaveMethod(this, "DocGrid UpdateIsInWorkStatus()");
			}
#endif
		}

		#endregion

		#region Load

		public void LoadCatalogDocs(string path, bool clean, int docID)
		{
#if AdvancedLogging
			Lib.Log.Logger.Message("DocGrid LoadCatalogDocs path= " + path);
#endif

			CancelOperation();
			if(docID > -1)
				KeyObject = docID;

#if AdvancedLogging
			Lib.Log.Logger.Message("DocGrid LoadCatalogDocs after waitWhileLoaderIsBusy(); path= " + path);
#endif

			//loader.DoWork += loader_CatalogDocs;
			//loader.RunWorkerCompleted += loader_Load;
			//loader.RunWorkerAsync(path);
			task = Task.Factory.StartNew<DataTable>(new Func<object, DataTable>(loader_CatalogDocs), path, source.Token);
			Task t1 = Task.Factory.ContinueWhenAny<DataTable>(new Task<DataTable>[ ] { task }, new Action<Task<DataTable>>(loader_Load));

			Style = Styles.Style.CreateDBDocsStyle(this);
			if(clean)
				Style.ReloadData(null);
		}

		public void LoadFoundDocs(Employee emp, bool clean, int docID)
		{
#if AdvancedLogging
            Lib.Log.Logger.Message("DocGrid LoadFoundDocs emp");
#endif

			CancelOperation();
			if(docID > 0)
				KeyObject = docID;

#if AdvancedLogging
            Lib.Log.Logger.Message("DocGrid LoadFoundDocs after waitWhileLoaderIsBusy(); emp");
#endif
			task = Task.Factory.StartNew<DataTable>(new Func<object, DataTable>(loader_FoundDocs), emp, source.Token);
			Task t1 = null;


			if(Style != null && Style is Styles.FormattedStyles.DBDocsStyles.FoundStyle)
			{
				t1 = Task.Factory.ContinueWhenAny<DataTable>(new Task<DataTable>[ ] { task }, new Action<Task<DataTable>>(loader_Reload));
			}
			else
			{
				t1 = Task.Factory.ContinueWhenAny<DataTable>(new Task<DataTable>[ ] { task }, new Action<Task<DataTable>>(loader_Load)); //, TaskContinuationOptions.OnlyOnRanToCompletion
				Style = Styles.Style.CreateFoundStyle(this);
			}
			if(clean)
				Style.ReloadData(null);
		}

		public void LoadQueryDocs(string xml, Employee emp, bool clean, int docID)
		{
#if AdvancedLogging
			Lib.Log.Logger.Message("DocGrid LoadQueryDocs xml= " + xml);
#endif

			CancelOperation();
			if(docID > 0)
				KeyObject = docID;

#if AdvancedLogging
			Lib.Log.Logger.Message("DocGrid LoadQueryDocs after waitWhileLoaderIsBusy(); xml= " + xml);
#endif

			task = Task.Factory.StartNew<DataTable>(new Func<object, DataTable>(loader_QueryDocs), new object[ ] { xml, emp.ID, source.Token }, source.Token);
			Task t1 = null;


			if(Style != null && Style is Styles.FormattedStyles.DBDocsStyles.FoundQueryStyle)
			{
				t1 = Task.Factory.ContinueWhenAny<DataTable>(new Task<DataTable>[ ] { task }, new Action<Task<DataTable>>(loader_Reload));
			}
			else
			{
				t1 = Task.Factory.ContinueWhenAny<DataTable>(new Task<DataTable>[ ] { task }, new Action<Task<DataTable>>(loader_Load));
				Style = Styles.Style.CreateFoundQueryStyle(this);
			}
			if(clean)
				Style.ReloadData(null);
		}

		public void LoadWorkFolderDocs(int id, Employee emp, bool clean, int docID)
		{
#if AdvancedLogging
			Lib.Log.Logger.Message("DocGrid LoadWorkFolderDocs id= " + id);
#endif

			CancelOperation();
			if(docID > 0)
				KeyObject = docID;
			bool reload = false;
#if AdvancedLogging
			Lib.Log.Logger.Message("DocGrid LoadWorkFolderDocs after waitWhileLoaderIsBusy(); id= " + id);
#endif
			task = Task.Factory.StartNew<DataTable>(new Func<object, DataTable>(loader_WorkFolderDocs), new object[ ] { id, emp, source.Token }, source.Token);
			Task t1 = null;
			if(Style != null && Style is Styles.FormattedStyles.DBDocsStyles.WorkFolderStyle)
			{
				t1 = Task.Factory.ContinueWhenAny<DataTable>(new Task<DataTable>[ ] { task }, new Action<Task<DataTable>>(loader_Reload));
				reload = true;
			}
			else
			{
				if(docID == 0)
					DropKeys();
				t1 = Task.Factory.ContinueWhenAny<DataTable>(new Task<DataTable>[ ] { task }, new Action<Task<DataTable>>(loader_Load));
				Style = Styles.Style.CreateWorkFolderStyle(this);
			}
			if(clean)
			{
				Style.ReloadData(null);
				if(reload && docID > 0)
					KeyObject = docID;
			}
		}

		public void LoadSharedWorkFolderDocs(int id, Employee emp, bool clean)
		{
//#if AdvancedLogging
			//Lib.Log.Logger.Message("DocGrid LoadSharedWorkFolderDocs id= " + id);
//#endif

			//CancelOperation();

//#if AdvancedLogging
			//Lib.Log.Logger.Message("DocGrid LoadSharedWorkFolderDocs after waitWhileLoaderIsBusy(); id= " + id);
//#endif

			//loader.DoWork += loader_SharedWorkFolderDocs;
			//if(Style != null && Style is Styles.FormattedStyles.DBDocsStyles.SharedWorkFolderStyle)
			//{
			//    loader.RunWorkerCompleted += loader_Reload;
			//    loader.RunWorkerAsync(new object[ ] { id, emp });
			//}
			//else
			//{
			//    loader.RunWorkerCompleted += loader_Load;
			//    loader.RunWorkerAsync(new object[ ] { id, emp });
			//    Style = Styles.Style.CreateSharedWorkFolderStyle(this);
			//}
			//if(clean)
			//    Style.ReloadData(null);
		}

		public void LoadFullAccessFolderDocs(int id, Employee emp, bool clean)
		{
//#if AdvancedLogging
//            Lib.Log.Logger.Message("DocGrid LoadFullAccessFolderDocs id= " + id);
//#endif

			//CancelOperation();

//#if AdvancedLogging
//            Lib.Log.Logger.Message("DocGrid LoadFullAccessFolderDocs after waitWhileLoaderIsBusy(); id= " + id);
//#endif

			//loader.DoWork += loader_FullAccessFolderDocs;
			//if(Style != null && Style is Styles.FormattedStyles.DBDocsStyles.FullAccessFolderStyle)
			//{
			//    loader.RunWorkerCompleted += loader_Reload;
			//    loader.RunWorkerAsync(new object[ ] { id, emp });
			//}
			//else
			//{
			//    loader.RunWorkerCompleted += loader_Load;
			//    loader.RunWorkerAsync(new object[ ] { id, emp });
			//    Style = Styles.Style.CreateFullAccessFolderStyle(this);
			//}
			//if(clean)
			//    Style.ReloadData(null);
		}

		public void LoadDiskImages(string path, bool clean)
		{
//#if AdvancedLogging
			//Lib.Log.Logger.Message("DocGrid LoadDiskImages path= " + path);
//#endif

			CancelOperation();

//#if AdvancedLogging
			//Lib.Log.Logger.Message("DocGrid LoadDiskImages after waitWhileLoaderIsBusy(); path= " + path);
//#endif

			task = Task.Factory.StartNew<DataTable>(new Func<object, DataTable>(loader_DiskImages), new object[ ] { path, source.Token }, source.Token);
			Task t1 = null;
			if(Style != null && Style is Styles.FormattedStyles.DiskImagesStyle)
			{
				//this.DataBindingComplete -= SelectDataGrid_DataBindingComplete;
				t1 = Task.Factory.ContinueWhenAny<DataTable>(new Task<DataTable>[ ] { task }, new Action<Task<DataTable>>(loader_Reload));

			}
			else
			{
				t1 = Task.Factory.ContinueWhenAny<DataTable>(new Task<DataTable>[ ] { task }, new Action<Task<DataTable>>(loader_Load));
				Style = Styles.Style.CreateDiskImagesStyle(this);
			}
			if(clean)
				Style.ReloadData(null);
			//loader.DoWork += loader_DiskImages;
			//if(Style != null && Style is Styles.FormattedStyles.DiskImagesStyle)
			//{
			//    loader.RunWorkerCompleted += loader_Reload;
			//    loader.RunWorkerAsync(path);
			//}
			//else
			//{
			//    loader.RunWorkerCompleted += loader_Load;
			//    loader.RunWorkerAsync(path);
			//    Style = Styles.Style.CreateDiskImagesStyle(this);
			//}
			//if(clean)
			//    Style.ReloadData(null);
		}

		public void LoadScans(string path, bool clean)
		{
			CancelOperation();

			task = Task.Factory.StartNew<DataTable>(new Func<object, DataTable>(loader_Scans), new object[ ] { path, source.Token }, source.Token);
			Task t1 = null;
			t1 = Task.Factory.ContinueWhenAny<DataTable>(new Task<DataTable>[ ] { task }, new Action<Task<DataTable>>(loader_Load));

			if(Style != null && Style is Styles.OuterScanerStyle)
				Style = Styles.Style.CreateOuterScanerStyle(this);
			else
				Style = Styles.Style.CreateScanerStyle(this);
			if(clean)
				Style.ReloadData(null);
		}

		public void LoadFaxesIn(int id)
		{
#if AdvancedLogging
			using(Lib.Log.Logger.DurationMetter("DocGrid LoadFaxesIn id= " + id))
#endif
			LoadStyle(Styles.Style.CreateFaxesInStyle(this), Environment.FaxInData.GetFolderFaxes(id, Environment.UserSettings.FaxesInUnsavedOnly));
		}

		public void LoadFaxesOut(int id)
		{
#if AdvancedLogging
			using(Lib.Log.Logger.DurationMetter("DocGrid LoadFaxesOut id= " + id))
#endif
			{
				DataTable dt = Environment.FaxOutData.GetFolderFaxes(id, Environment.UserSettings.FaxesOutUnsavedOnly);
				LoadStyle(Styles.Style.CreateFaxesOutStyle(this), dt);
			}
		}

		public void LoadDocuments(DataTable dt)
		{
			LoadStyle(Styles.Style.CreateDocumentStyle(this), dt);
		}

		private void LoadStyle(Styles.Style newStyle, DataTable dt)
		{
			CancelOperation();

			Style = newStyle;

#if AdvancedLogging
			using(Lib.Log.Logger.DurationMetter("DocGrid LoadStyle"))
#endif
			if(Style.LoadData(dt))
				AutoWidth();

			ReturnCursorsToDefault();
		}

		#endregion

		#region Mouse

		private bool hederClick = false;
		private void DocGrid_MouseUp(object sender, DataGridViewCellMouseEventArgs e)
		{
			if(hederClick)
			{
				hederClick = false;
				return;
			}

			if(!IsFine)
				return;

			if(e.Button != MouseButtons.Right)
				return;
			Console.WriteLine("{0}: try to build ContextMenu", DateTime.Now.ToString("HH:mm:ss fff"));
			ContextMenu contextMenu = Style.BuildContextMenu();
			if(contextMenu != null && contextMenu.MenuItems.Count > 0)
				contextMenu.Show(this, PointToClient(Cursor.Position));
		}


		private void DocGridHeader_MouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			hederClick = true;
			if(!IsFine)
				return;

			if(e.Button != MouseButtons.Right)
				return;

			Console.WriteLine("{0}: try to build HeaderContextMenu", DateTime.Now.ToString("HH:mm:ss fff"));
			ContextMenu contextMenu = Style.BuildHeaderContextMenu(e.ColumnIndex);
			if(contextMenu != null && contextMenu.MenuItems.Count > 0)
				contextMenu.Show(this, PointToClient(Cursor.Position));
		}

		#endregion

		#region DeleteRows

		public void DeleteSelectedRows()
		{
			lock(this)
			{
				SuspendLayout();

				if(SelectedRows.Count == Rows.Count)
					for(int i = Rows.Count - 1; i > -1; i--)
						Rows.RemoveAt(i);
				else
					foreach(DataGridViewRow row in SelectedRows)
						Rows.Remove(row);

				ResumeLayout();
				Invalidate();
			}
		}

		public void DeleteRowConditional(object key)
		{
			lock(this)
			{
				int index = GetIndex(key);
				if(index != -1)
				{
					SuspendLayout();
					Rows.RemoveAt(index);
					ResumeLayout();
					Invalidate();
				}
			}
		}

		public void DeleteRowConditional(string field, object key)
		{
			lock(this)
			{
				int index = GetIndexConditional(field, key);
				if(index != -1)
				{
					SuspendLayout();
					Rows.RemoveAt(index);
					ResumeLayout();
					Invalidate();
				}
			}
		}

		public void DeleteRow(int rowIndex)
		{
			if(rowIndex < 0)
				return;
			if(InvokeRequired)
			{
				BeginInvoke(new Action<int>(DeleteRow), new object[ ] { rowIndex });
				return;
			}
			SuspendLayout();
			Rows.RemoveAt(rowIndex);
			ResumeLayout();
		}

		#endregion

		private void CancelOperation()
		{
			source.Cancel();
			source.Dispose();
			source = new CancellationTokenSource();
		}

		public bool SetValues(int[ ] IDs, string colName, object val)
		{
			if(SelectedRows.Count == 0 || !Columns.Contains(colName))
				return false;
			lock(this)
			{
				try
				{
					SuspendLayout();

					for(int i = 0; i < IDs.Length; i++)
						this[colName, GetIndex(IDs[i])].Value = val;

					return true;
				}
				catch
				{
					return false;
				}
				finally
				{
					ResumeLayout();
					Invalidate();
				}
			}
		}

		#region Misc

		public override DataObject GetClipboardContent()
		{
			if(IsDBDocs())
			//    return base.GetClipboardContent();
			//else
			{
				DataObject dob = new DataObject();
				int[] ids = GetCurIDs();
				if(ids.Length > 0)
				{
					dob.SetText(string.Join(",", ids.Select(x => x.ToString()).ToArray()));
					dob.SetData("DocViewCopy", new object[] { ids, Program.MainFormDialog.curContext });
				}
				return dob;
			}
			else return null;
		}

		private void DocGrid_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
		{
			e.ToolTipText = string.Empty;
			if(!IsFine || Cursor != Cursors.Default)
				return;

			e.ToolTipText = Style.MakeDocString(e.RowIndex).Replace((char)160 + " ", "\r\n");
		}

		public void AutoWidth()
		{
			if(!IsFine || Rows.Count == 0)
				return;
			try
			{
				const int minWidth = 70;
				const int boolMinWidth = 30;

				int targetWidth = ClientSize.Width - 4;

				if(VerticalScrollBar.Visible)
					targetWidth -= SystemInformation.VerticalScrollBarWidth;

				int runningWidthUsed = 0;

				int lastVisible = -1;
				try { lastVisible = Columns.GetLastColumn(DataGridViewElementStates.Visible, DataGridViewElementStates.None).Index; }
				catch { return; }
				if(lastVisible > -1)
				{
					for(int i = 0; i < lastVisible; i++)
						if(Columns[i].Visible)
							runningWidthUsed += Columns[i].Width;

					int plannedWidth = targetWidth - runningWidthUsed;
					if(Style.IsColumnBool(Columns[lastVisible].DataPropertyName))
					{
						if(plannedWidth < boolMinWidth)
							plannedWidth = boolMinWidth;
					}
					else
						if(plannedWidth < minWidth)
							plannedWidth = minWidth;

					Columns[lastVisible].Width = plannedWidth;
				}
				RestoreSelect();
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		#endregion

		#region Select

		public override bool SelectConditional(string field, object key, bool soft)
		{
#if AdvancedLogging
			try
			{
				Lib.Log.Logger.EnterMethod(this, "DocGrid SelectConditional(string field, object key, bool soft)");
#endif
			return base.SelectConditional(field, key, soft);
#if AdvancedLogging
			}
			finally
			{
				Lib.Log.Logger.LeaveMethod(this, "DocGrid SelectConditional(string field, object key, bool soft)");

			}
#endif
		}

		private void CheckTask()
		{
			if(task != null)
			{
				task.Wait();
				Application.DoEvents();
			}
		}

		public override bool SelectRow(object key)
		{
#if AdvancedLogging
			try
			{
				Lib.Log.Logger.EnterMethod(this, "DocGrid SelectRow(object key)");
#endif
				return base.SelectRow(key);
#if AdvancedLogging
 			}
			finally
			{
				Lib.Log.Logger.LeaveMethod(this, "DocGrid SelectRow(object key)");

			}
#endif
		}

		public override bool SelectRow(int row)
		{
			try
			{
#if AdvancedLogging
                Lib.Log.Logger.EnterMethod(this, "SelectRow(int row)");
#endif
				return base.SelectRow(row);
			}
			finally
			{
#if AdvancedLogging
                Lib.Log.Logger.LeaveMethod(this, "SelectRow(int row)");
#endif
			}
		}

		public bool SelectByID(int id)
		{
			if(id <= 0)
				return false;

			try
			{
#if AdvancedLogging
                Lib.Log.Logger.EnterMethod(this, "SelectByID(int id)");
#endif
				return SelectConditional(IDField, id, true);
			}
			finally
			{
#if AdvancedLogging
                Lib.Log.Logger.LeaveMethod(this, "SelectByID(int id)");
#endif
			}
		}

		#endregion

		private void DocGrid_SizeChanged(object sender, EventArgs e)
		{
			Invalidate();
		}

		protected override void OnColumnWidthChanged(DataGridViewColumnEventArgs e)
		{
			base.OnColumnWidthChanged(e);
			Style.Save();
		}

		protected override void  MakeSortOrder(bool save = false)
		{
 			base.MakeSortOrder(save);
			if(save)
				Style.SaveSortOrder();
		}
	}
}