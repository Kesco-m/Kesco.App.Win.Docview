using System;
using System.Windows.Forms;

namespace Kesco.App.Win.DocView.Grids
{
	/// <summary>
	/// Перегруженный DataGridView
	/// </summary>
	public class Grid : Kesco.Lib.Win.Document.Grid.SelectDataGrid
	{
		public Grid()
		{
			ImageTime = DateTime.MinValue;
			CellPainting += Grid_CellPainting;
		}

		protected override void Grid_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
		{
			e.Column.HeaderCell = new Kesco.Lib.Win.Document.Grid.DocsDataGridColumnHeaderCell(Style.GetColumnHeaderName(e.Column.Name));
			e.Column.SortMode = DataGridViewColumnSortMode.Programmatic;

			FormatColumn(e.Column);
		}

		public void Init(Lib.Win.Options.Folder layout, Forms.MainFormDialog mainForm)
		{
			Init(layout);
			MainForm = mainForm;
		}

		#region Accessors

		public Styles.Style Style { get; set; }

		protected override string KeyField
		{
			get { return Style == null ? string.Empty : Style.KeyField; }
		}

		protected override string IDField
		{
			get { return Style == null ? string.Empty : Style.IdField; }
		}

		public Forms.MainFormDialog MainForm { get; set; }

		public DateTime ImageTime { get; set; }

		#endregion

		#region OnPaint

		private void Grid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
		{
			if(e.RowIndex == -1)
				return;

			if(Style == null)
				return;

			if(Style.IsColumnBool(Columns[e.ColumnIndex].Name))
				return;

			Style.FormatGridCells(e);
		}

		#endregion

		#region Misc

		public override void DisplayCurrentRow()
		{
			if(MainForm != null && !MainForm.Visible)
				return;
			base.DisplayCurrentRow();
		}

		#endregion

		#region Is

		public virtual bool IsSpam()
		{
#if(Debug)
			Debug.Assert(false, "IsSpam: Вызов должен производиться только из объекта DocGrid.");
#endif
			return false;
		}

		public virtual bool IsInWork()
		{
#if(Debug)
            Debug.Assert(false, "IsInWork: Вызов должен производиться только из объекта DocGrid.");
#endif
			return false;
		}

		#endregion

		#region MultiSelect Get and Set

		public bool SetSelectedValues(string colName, object val)
		{
			if(SelectedRows.Count == 0 || !Columns.Contains(colName))
				return false;
			lock(this)
			{
				try
				{
					SuspendLayout();
					int rowIndex = CurrentCell.RowIndex;
					int colIndex = CurrentCell.ColumnIndex;

					foreach(DataGridViewRow row in SelectedRows)
						row.Cells[colName].Value = val;

					CurrentCell = this[colIndex, rowIndex];

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

		public object[] GetSelectedValues(string colName)
		{
			if(!IsFine || !Columns.Contains(colName))
				return null;


			var obj = new object[SelectedRows.Count];
			for(int i = 0; i < SelectedRows.Count; i++)
				obj[i] = SelectedRows[i].Cells[colName].Value;
			return obj.Length > 0 ? obj : null;

		}

		public bool GetBoolValue(string colName)
		{
			return GetBoolValue(CurrentRowIndex, colName);
		}

		public bool GetBoolValue(int rowIndex, string colName)
		{
			try
			{
				object obj = GetValue(rowIndex, colName) ?? false;

				if(obj is bool)
					return (bool)obj;
				if(obj is byte || obj is int)
					return Convert.ToBoolean(obj);
				if(obj is string)
				{
					bool result;
					if(bool.TryParse((string)obj, out result))
						return result;
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}

			return false;
		}

		public bool GetSelectedBoolValuesSummary(string colName)
		{
			if(!IsFine || !Columns.Contains(colName))
				return false;

			bool result = true;
			for(int i = 0; i < SelectedRows.Count; i++)
			{
				if(SelectedRows[i].Cells[colName].Value is bool)
					result = result && (bool)SelectedRows[i].Cells[colName].Value;
				else if(SelectedRows[i].Cells[colName].Value is int)
					result = result && (int)SelectedRows[i].Cells[colName].Value == 1;
			}

			return result;
		}

		#endregion	
	}
}