using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Kesco.App.Win.DocView.Grids.Styles.FormattedStyles;
using Kesco.App.Win.DocView.Grids.Styles.FormattedStyles.DBDocsStyles;
using Kesco.App.Win.DocView.Grids.Styles.FormattedStyles.DBDocumentStyle;
using Kesco.App.Win.DocView.Grids.Styles.FormattedStyles.FaxesStyles;

namespace Kesco.App.Win.DocView.Grids.Styles
{
	public class Style: IDisposable
	{
		#region Variables

		private string friendlyName;
		protected Grid grid;
		protected Lib.Win.Options.Folder optionFolder;

		public bool Reset = false;

		protected MenuItem separator;
		protected MenuItem saveItem;
		protected MenuItem savePartItem;
		protected MenuItem saveSelectedItem;
		protected MenuItem propertiesItem;
		protected MenuItem openInNewWindowItem;
		protected MenuItem refreshItem;

		protected string needBoldField;
		protected string needColorField;
		protected string needUnderlineField;

		protected string idField;
		protected string keyField;

		protected ColParams colParams;

		#endregion

		#region Constructor & Factory Methods

		protected Style(Grid grid)
		{
			this.grid = grid;

			optionFolder = grid.OptionFolder.Folders.GetByNameForced(this.GetType().Name);

			separator = new MenuItem();

			saveItem = new MenuItem();
			savePartItem = new MenuItem();
			saveSelectedItem = new MenuItem();
			propertiesItem = new MenuItem();
			openInNewWindowItem = new MenuItem();
			refreshItem = new MenuItem();

			// разделитель
			separator.Text = "-";

			// сохранить...
			saveItem.Text = Environment.StringResources.GetString("Save") + "...";
			saveItem.Shortcut = Shortcut.F2;
			saveItem.Click += new System.EventHandler(saveItem_Click);

			// сохранить часть...
			savePartItem.Text = Environment.StringResources.GetString("SavePart") + "...";
			savePartItem.Shortcut = Shortcut.CtrlF2;
			savePartItem.Click += new System.EventHandler(savePartItem_Click);

			// сохранить выделенное...
			saveSelectedItem.Text = Environment.StringResources.GetString("SaveSelected") + "...";
			saveSelectedItem.Click += new System.EventHandler(saveSelectedItem_Click);

			// свойства...
			propertiesItem.Text = Environment.StringResources.GetString("Properties") + "...";
			propertiesItem.Shortcut = Shortcut.F6;
			propertiesItem.Click += new System.EventHandler(propertiesItem_Click);

			// открыть в новом окне
			openInNewWindowItem.Text = Environment.StringResources.GetString("OpenNewWindow");
			openInNewWindowItem.Shortcut = Shortcut.CtrlW;
			openInNewWindowItem.Click += new System.EventHandler(openInNewWindowItem_Click);

			// обновить
			refreshItem.Text = Environment.StringResources.GetString("Refresh");
			refreshItem.Click += new System.EventHandler(refreshItem_Click);

			idField = string.Empty;
			keyField = Environment.ImageReader.FullNameField;
		}

		public static void ResetStyles()
		{
			DBDocsStyle.DropInstance();
			FoundStyle.DropInstance();
			FoundQueryStyle.DropInstance();
			WorkFolderStyle.DropInstance();
			SharedWorkFolderStyle.DropInstance();
			FullAccessFolderStyle.DropInstance();
			DiskImagesStyle.DropInstance();
			FaxesInStyle.DropInstance();
			FaxesOutStyle.DropInstance();
			ScanerStyle.DropInstance();
			DBDocumentStyle.DropInstance();
		}

		public static Style CreateDBDocsStyle(DocGrid grid)
		{
			return DBDocsStyle.Instance(grid);
		}

		public static Style CreateFoundStyle(DocGrid grid)
		{
			return FoundStyle.Instance(grid);
		}

		public static Style CreateFoundQueryStyle(DocGrid grid)
		{
			return FoundQueryStyle.Instance(grid);
		}

		public static Style CreateWorkFolderStyle(DocGrid grid)
		{
			return WorkFolderStyle.Instance(grid);
		}

		public static Style CreateSharedWorkFolderStyle(DocGrid grid)
		{
			return SharedWorkFolderStyle.Instance(grid);
		}

		public static Style CreateFullAccessFolderStyle(DocGrid grid)
		{
			return FullAccessFolderStyle.Instance(grid);
		}

		public static Style CreateDiskImagesStyle(DocGrid grid)
		{
			return DiskImagesStyle.Instance(grid);
		}

		public static Style CreateFaxesInStyle(DocGrid grid)
		{
			return FaxesInStyle.Instance(grid);
		}

		public static Style CreateFaxesOutStyle(DocGrid grid)
		{
			return FaxesOutStyle.Instance(grid);
		}

		public static Style CreateScanerStyle(DocGrid grid)
		{
			return ScanerStyle.Instance(grid);
		}

		public static Style CreateOuterScanerStyle(DocGrid grid)
		{
			return OuterScanerStyle.Instance(grid);
		}

		public static Style CreateInfoStyle(InfoGrid grid)
		{
			return InfoStyle.Instance(grid);
		}

		public static Style CreateDocumentStyle(DocGrid grid)
		{
			return DBDocumentStyle.Instance(grid);
		}

		#endregion

		#region Accessors

		public string FriendlyName
		{
			get
			{
				return string.IsNullOrEmpty(friendlyName) ? GetType().Name : friendlyName;
			}
			set { friendlyName = value; }
		}

		public Grid Grid
		{
			get { return grid; }
		}

		public string KeyField
		{
			get { return keyField; }
		}

		public string IdField
		{
			get { return idField; }
		}

		public Lib.Win.Options.Folder OptionFolder
		{
			get { return optionFolder; }
		}

		public bool WasInited
		{
			get { return colParams != null && colParams.WasInited; }
		}
		#endregion

		#region Columns

		public virtual int GetColumnWidth(string colName)
		{
			return 100;
		}

		public virtual string GetColumnHeaderName(string colName)
		{
			return colName;
		}

		public virtual bool IsColumnBool(string colName)
		{
			return false;
		}

		public virtual void FormatGridCells(DataGridViewCellPaintingEventArgs e)
		{
		}

		public virtual bool UseLock()
		{
			return false;
		}

		public virtual bool IsColumnVisible(string column)
		{
			if(colParams == null || this.IsColumnSystem(column))
				return false;

			if(column == "Прочитан")
				return false;

			try
			{
				return !colParams.Contains(column) || colParams.GetParam(column).Visible;
			}
			catch
			{
				return false;
			}
		}

		public virtual bool IsColumnSystem(string column)
		{
			if(colParams == null)
				return false;

			try
			{
				return colParams.IsColumnSystem(column);
			}
			catch
			{
				return false;
			}
		}

		#endregion

		#region Context Menu

		public virtual ContextMenu BuildContextMenu()
		{
			return null;
		}

		public virtual ContextMenu BuildHeaderContextMenu(int ix)
		{
			if(!IsShownInSettings())
				return null;

			var contextMenu = new ContextMenu() { Tag = ix };
			foreach(KeyValuePair<int, ColParam> cp in colParams.Where(x => !x.Value.IsSystemField).OrderBy(x => x.Key))
			{
				MenuItem mi = new MenuItem(cp.Value.HeaderName) { Checked = cp.Value.Visible, Tag = cp.Value, Enabled = !cp.Value.IsKeyField };
				mi.Click += new EventHandler(OnHeaderContextMenuClick);
				contextMenu.MenuItems.Add(mi);
			}
			contextMenu.MenuItems.Add("-");
			MenuItem mi1 = new MenuItem(Environment.StringResources.GetString("ColumnsDedales"));
			mi1.Click += new EventHandler(OnHeaderContextMenuDedalesClick);
			contextMenu.MenuItems.Add(mi1);

			return contextMenu;
		}

		private void OnHeaderContextMenuClick(object sender, EventArgs eventArgs)
		{
			if(sender as MenuItem == null)
				return;

			MenuItem mi = sender as MenuItem;
			ColParam colP = mi.Tag as ColParam;
			if(!colParams.Any(x => x.Value.Visible && x.Value.IsKeyField) && colParams.Count(x => x.Value.Visible) == 1 && colP.Visible)
				return;

			Save();
			colP.Visible = !(sender as MenuItem).Checked;
			if(colP.Visible)
			{
				colParams.Where(x => x.Value.Index >= (int)mi.Parent.Tag + 1).Select(x => x.Value.Index++).ToArray();
				colP.Index = (int)mi.Parent.Tag + 1;
			}

			colParams.Save();
			ReloadData();
		}

		private void OnHeaderContextMenuDedalesClick(object sender, EventArgs eventArgs)
		{
			//Environment.CmdManager.Commands["Columns"].Execute();
			if(IsShownInSettings())
			{
				Save();
				var dialog = new Settings.SettingsColumnsDialog(this, this.Grid.OptionFolder, this);
				dialog.FormClosed += SettingsColumnsDialog_FormClosed;
				dialog.Show();
			}
		}

		private void SettingsColumnsDialog_FormClosed(object sender, FormClosedEventArgs e)
		{
			if(e.CloseReason == CloseReason.UserClosing && ((Settings.SettingsColumnsDialog)sender).DialogResult == DialogResult.Retry)
			{
				Environment.CmdManager.Commands["ResetColumns"].Execute();
			}
		}

		private void saveItem_Click(object sender, System.EventArgs e)
		{
			Environment.CmdManager.Commands["Save"].Execute();
		}

		private void savePartItem_Click(object sender, System.EventArgs e)
		{
			Environment.CmdManager.Commands["SavePart"].Execute();
		}

		private void saveSelectedItem_Click(object sender, System.EventArgs e)
		{
			Environment.CmdManager.Commands["SaveSelected"].Execute();
		}


		public void propertiesItem_Click(object sender, System.EventArgs e)
		{
			Environment.CmdManager.Commands["DocProperties"].Execute();
		}

		public void openInNewWindowItem_Click(object sender, System.EventArgs e)
		{
			Environment.CmdManager.Commands["NewWindow"].Execute();
		}

		public void refreshItem_Click(object sender, System.EventArgs e)
		{
			Environment.CmdManager.Commands["RefreshDocs"].Execute();
		}

		#endregion

		#region Init

		public virtual void Init()
		{
			try
			{
				grid.Columns.Clear();
				foreach(ColParam col in colParams.Values.Where(col => col.Visible))
				{
					if(IsColumnBool(col.Name))
					{
						DataGridViewCheckBoxColumn bColumn = new DataGridViewCheckBoxColumn();
						if(col.DataType == typeof(int))
						{
							bColumn.FalseValue = 0;
							bColumn.TrueValue = 1;
						}

						bColumn.Name = col.Name; // bool
						bColumn.Width = col.Width; // bool
						bColumn.Visible = col.Visible; // bool
						if(grid.FindForm().InvokeRequired)
							grid.BeginInvoke(new Func<DataGridViewColumn, int>(grid.Columns.Add), bColumn);
						else
							grid.Columns.Add(bColumn); // bool
					}
					else
					{
						DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
						if(col.DataType == typeof(DateTime))
							column.DefaultCellStyle.Format = "dd.MM.yyyy HH:mm:ss";
						column.Name = col.Name;
						column.Width = col.Width;
						column.Visible = col.Visible;
						var form = grid.FindForm();
						if(form != null && form.InvokeRequired)
							grid.BeginInvoke(new Func<DataGridViewColumn, int>(grid.Columns.Add), column);
						else
							grid.Columns.Add(column);
					}
				}
				foreach(ColParam col in colParams.Values.Where(col => !col.Visible))
				{
					if(IsColumnBool(col.Name))
					{
						DataGridViewCheckBoxColumn bColumn = new DataGridViewCheckBoxColumn();
						if(col.DataType == typeof(int))
						{
							bColumn.FalseValue = 0;
							bColumn.TrueValue = 1;
						}

						bColumn.Name = col.Name; // bool
						bColumn.Width = col.Width; // bool
						bColumn.Visible = col.Visible; // bool
						if(grid.FindForm().InvokeRequired)
							grid.BeginInvoke(new Func<DataGridViewColumn, int>(grid.Columns.Add), bColumn);
						else
							grid.Columns.Add(bColumn); // bool
					}
					else
					{
						DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
						if(col.DataType == typeof(DateTime))
							column.DefaultCellStyle.Format = "dd.MM.yyyy HH:mm:ss";
						column.Name = col.Name;
						column.Width = col.Width;
						column.Visible = col.Visible;
						if(grid.FindForm().InvokeRequired)
							grid.BeginInvoke(new Func<DataGridViewColumn, int>(grid.Columns.Add), column);
						else
							grid.Columns.Add(column);
					}
				}

				colParams.WasInited = IsShownInSettings();
			}
			//catch(InvalidOperationException)
			//{
			//    // приводит к повторному вызову функции SetCurrentCellAddressCore
			//    // не лечится
			//}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		#endregion

		#region Load Data

		public virtual bool LoadData(DataTable dt)
		{
			try
			{
				Console.WriteLine("{0}: LoadData", DateTime.Now.ToString("HH:mm:ss fff"));
				grid.Enabled = false;
				grid.SuspendLayout();

				if(dt == null || dt.Rows == null || dt.Columns.Count < 1)
				{
					grid.DataSource = null;
					if(grid.Columns != null)
						grid.Columns.Clear();
				}
				else
				{
					colParams = new ColParams(this);
					colParams.Verify(dt);
					Reset = false;
					Init();
					grid.DataSource = dt.DefaultView;
					grid.SetSort(colParams.SortOrder);
					Save();
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
				return false;
			}
			finally
			{
				grid.CurrentCell = null;
				grid.ResumeLayout(true);
				grid.Enabled = true;
				grid.Invalidate();
			}
			return true;
		}

		public virtual bool ReloadData(DataTable dt)
		{
			Console.WriteLine("{0}: ReloadData", DateTime.Now.ToString("HH:mm:ss fff"));
			if(grid == null)
				return false;
			if(dt == null || dt.Rows == null || dt.Rows.Count == 0)
			{
				colParams = null;
				grid.DataSource = null;
				if(grid.Columns != null)
					grid.Columns.Clear();
				return false;
			}

			if(!grid.IsFine || colParams == null)
				return LoadData(dt);

			object keyObj = grid.KeyObject;
			int firstCellColumnIndex = grid.FirstDisplayedCell == null ? 0 : grid.FirstDisplayedCell.ColumnIndex;
			int firstCellRowIndex = grid.FirstDisplayedCell == null ? 0 : grid.FirstDisplayedCell.RowIndex;
			try
			{
				grid.SuspendLayout();
				if(grid.DataSource != null && dt.Rows.Count > 0)
					grid.SetSilent();
				grid.DataSource = dt.Rows.Count == 0 ? null : dt.DefaultView;
				if(grid.IsFine)
				{
					try { grid.GetDataView().Sort = colParams.SortOrder; }
					catch { grid.SetSort(colParams.SortOrder); }
					try
					{
						while(firstCellColumnIndex > grid.ColumnCount - 1 && firstCellColumnIndex > -1) { firstCellColumnIndex--; }
						grid.FirstDisplayedCell = grid[firstCellColumnIndex, firstCellRowIndex];
					}
					catch { }
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
				return false;
			}
			finally
			{
				grid.RemoveSilent();
				if(grid.IsFine)
				{
					if(!grid.SelectRow(keyObj))
					{
						grid.ClearSelection();
						grid.CurrentCell = null;
					}
				}
				else
					grid.CurrentCell = null;
				grid.ResumeLayout();
			}
			return true;
		}

		public virtual bool ReloadData()
		{
			Console.WriteLine("{0}: RefreshData", DateTime.Now.ToString("HH:mm:ss fff"));
			if(grid.DataSource == null)
				return false;

			DataTable table = grid.GetDataView().Table;
			int firstCellColumnIndex = grid.FirstDisplayedCell == null ? 0 : grid.FirstDisplayedCell.ColumnIndex;
			int firstCellRowIndex = grid.FirstDisplayedCell == null ? 0 : grid.FirstDisplayedCell.RowIndex;
			object keyObj = grid.KeyObject;

			try
			{
				grid.DataSource = null;

				optionFolder.Clear();
				optionFolder.Load();

				colParams = new ColParams(this);
				colParams.Verify(table);
				//grid.BeginInvoke((MethodInvoker)(colParams.Save));
				Init();

				grid.DataSource = table.DefaultView;
				grid.SetSort(colParams.SortOrder);

				grid.BeginInvoke((MethodInvoker)(Save));

				return true;
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
			finally
			{
				if(grid.IsFine && grid.SelectRow(keyObj))
				{
					try
					{
						while(firstCellColumnIndex > grid.ColumnCount - 1 && firstCellColumnIndex > -1) { firstCellColumnIndex--; }
						grid.FirstDisplayedCell = grid[firstCellColumnIndex, firstCellRowIndex];
					}
					catch { }
					grid.DisplayCurrentRow();
				}
				else
					grid.CurrentCell = null;
				grid.ResumeLayout();
			}

			return false;
		}

		#endregion

		#region Save

		public void Save()
		{
			try
			{
				if(Reset)
				{
					optionFolder.Clear();
				}
				else if(colParams != null)
				{
					if(grid != null && grid.ColumnCount > 0)
					{
						foreach(ColParam colParam in colParams.Values.Where(colParam => grid.Columns.Contains(colParam.Name)))
						{
							colParam.Width = grid.Columns[colParam.Name].Width;
							colParam.Index = grid.Columns[colParam.Name].Index;
							colParam.Visible = grid.Columns[colParam.Name].Visible;
						}
					}
					colParams.Save();
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		public void SaveSortOrder()
		{
			if(grid != null && grid.ColumnCount > 0)
				colParams.SortOrder = grid.GetSort();
			Save();
		}

		#endregion

		#region Is

		public virtual bool IsShownInSettings()
		{
			return false;
		}

		public virtual bool IsDBDocs()
		{
			return false;
		}

		public virtual bool IsWorkFolder()
		{
			return false;
		}

		public virtual bool IsDBDocsFullAccess()
		{
			return false;
		}

		public virtual bool IsFaxes()
		{
			return false;
		}

		public virtual bool IsFaxesIn()
		{
			return false;
		}

		public virtual bool IsFaxesOut()
		{
			return false;
		}

		public virtual bool IsScaner()
		{
			return false;
		}

		public virtual bool IsDiskImages()
		{
			return false;
		}

		public virtual bool IsInfo()
		{
			return false;
		}

		public virtual bool IsFound()
		{
			return false;
		}

		public virtual bool IsDocument()
		{
			return false;
		}

		#endregion

		#region CurDocString

		public virtual string MakeDocString(int row)
		{
			return string.Empty;
		}

		public virtual string MakeCurDocString()
		{
			return MakeDocString(grid.CurrentRow == null ? -1 : grid.CurrentRowIndex);
		}

		public virtual string DocStringToSave()
		{
			return string.Empty;
		}

		#endregion

		public void Dispose()
		{
			grid = null;
		}
	}
}