using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Kesco.App.Win.DocView.Grids.Styles
{
	public class ColParams : System.Collections.Generic.SortedList<int, ColParam>
	{
		#region Variables

		protected const string SortField = "Сортировка";
		protected const string ListField = "НаименованияСтолбцов";
		protected const string StyleWasInited = "СтильИнициализирован";
		protected const string userFriendlyName = "UserFriendlyName";
		protected Style style = null;
		internal protected Lib.Win.Options.Folder optionFolder;
		public string SortOrder = string.Empty;
		protected string friendlyName = string.Empty;
		public bool WasInited = false;

		#endregion

		#region Accessors

		public string FriendlyName
		{
			set { friendlyName = value; }

			get
			{
				if(string.IsNullOrEmpty(friendlyName))
					friendlyName = style == null ? optionFolder.LoadStringOption(userFriendlyName, optionFolder.Name) : style.FriendlyName;
				return friendlyName;
			}
		}

		#endregion

		#region Constructors

		public ColParams(Style style)
		{
			this.style = style;
			optionFolder = style.OptionFolder;
			friendlyName = style.FriendlyName;

			LoadList();
			SortOrder = optionFolder.LoadStringOption(SortField, string.Empty).ToString();
		}

		public ColParams(Lib.Win.Options.Folder optionFolder, bool LoadSystemFields)
		{
			this.optionFolder = optionFolder;
			bool.TryParse(this.optionFolder.LoadStringOption(StyleWasInited, false.ToString()), out WasInited);
			SortOrder = this.optionFolder.LoadStringOption(SortField, string.Empty);

			if(LoadSystemFields)
				LoadList();
			else
				LoadListWithoutSystemFields();
		}

		#endregion

		#region Load and Verify Data

		private void LoadList()
		{
			Clear();
			string[] _s = (this.optionFolder.LoadStringOption(ListField, string.Empty)).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

			if(_s.Length > 0)
			{
				int i = 1;
				int j = -1;
				foreach(string colName in _s)
				{
					if((style != null && style.IsColumnSystem(colName)) || (style == null && IsColumnSystem(colName)))
						AddParam(colName, j);
					else
						AddParam(colName, i);
					i++;
					j--;
				}
			}
		}

		private void LoadListWithoutSystemFields()
		{
			Clear();
			string[] _s = (this.optionFolder.LoadStringOption(ListField, string.Empty)).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

			if(_s.Length <= 0)
				return;

			int i = 1;
			foreach(string colName in _s)
			{
				if((style != null && !style.IsColumnSystem(colName)) || (style == null && !IsColumnSystem(colName)))
					AddParam(colName, i);
				i++;
			}
		}

		public bool NotEmpty(bool LoadWithoutSystemFieldsIfNeeded)
		{
			if(Count == 0)
			{
				if(MessageBox.Show(
					Environment.StringResources.GetString("ColumnsParamsEmptyAlarm.Text"),
					Environment.StringResources.GetString(friendlyName),
					MessageBoxButtons.YesNoCancel) == DialogResult.No)
					return false;

				if(LoadWithoutSystemFieldsIfNeeded)
					LoadListWithoutSystemFields();
				else
					LoadList();

				if(Count == 0)
					MessageBox.Show(Environment.StringResources.GetString("ColumnsParamsEmptyAlarm.Failed"),
					Environment.StringResources.GetString(friendlyName),
					MessageBoxButtons.OK);
			}

			return Count > 0;
		}

		public void Verify(DataTable table)
		{
			foreach(ColParam col in Values.Where(col => !table.Columns.Contains(col.Name)))
			{
				col.Visible = false;
				col.ToRemove = true;
			}

			foreach(DataColumn col in table.Columns)
			{
				ColParam colParam = GetParam(col.ColumnName);
				colParam.DataType = col.DataType;
				if(style == null)
				{
					colParam.IsSystemField = IsColumnSystem(colParam.Name);
					colParam.Visible = !colParam.IsSystemField && colParam.Visible;
				}
				else
				{
					colParam.IsSystemField = style.IsColumnSystem(colParam.Name);
					colParam.IsKeyField = style.KeyField == colParam.Name;
					colParam.Visible = colParam.Visible && !colParam.IsSystemField;
				}
			}

			int index = 0;
			foreach(ColParam col in Values)
			{
				col.Index = index;
				index++;

				if(col.Visible && (col.Width == -1 || col.Width == 0) && style != null)
					col.Width = style.GetColumnWidth(col.Name);
			}
		}

		#endregion

		#region Info

		public bool IsColumnSystem(string column)
		{
			if(Contains(column) && GetParam(column).IsSystemField)
				return true;
			return column.StartsWith("Код") && column != "КодДокумента";
		}

		public int GetMaxIndex()
		{
			lock(SortField)
			{
				return Values.Select(col => col.Index).Concat(new[] { 0 }).Max();
			}
		}

		private int GetMinIndex()
		{
			lock(SortField)
			{
				return Values.Select(col => col.Index).Concat(new[] { 0 }).Min();
			}
		}

		#endregion

		#region AddParams

		private string getOption(string colName, int index)
		{
			string opt = string.Empty;

			if(style == null)
				opt = index + ";100;" + (!IsColumnSystem(colName)) + ";false;" + (!IsColumnSystem(colName)) + ";" + colName;
			else
				opt = index + ";" + style.GetColumnWidth(colName) + ";" + (style.IsColumnVisible(colName)) + ";" + (colName == style.KeyField) + ";" + (style.IsColumnSystem(colName)) + ";" + style.GetColumnHeaderName(colName);

			/// временная вставка для замены имен
			if(this.optionFolder.CheckExistsOption("new" + colName))
			{
				string s = this.optionFolder.LoadStringOption("new" + colName, string.Empty);
				this.optionFolder.DeleteOption("new" + colName);
				this.optionFolder.OptionForced<string>(colName).Value = s;
			}
			return optionFolder.LoadStringOption(colName, opt);
		}

		private void addParam(ColParam newColParam)
		{
			if(newColParam != null)
				try
				{
					Add(newColParam.Index, newColParam);
				}
				catch
				{
					if((style != null && style.IsColumnSystem(newColParam.Name)) || (style == null && IsColumnSystem(newColParam.Name)))
						newColParam.Index = GetMinIndex() - 1;
					else
						newColParam.Index = GetMaxIndex() + 1;
					Add(newColParam.Index, newColParam);
				}
		}

		public void AddParam(string colName, int index)
		{
			addParam(style == null
							  ? new ColParam(colName, getOption(colName, index))
							  : new ColParam(colName, style.GetColumnHeaderName(colName), getOption(colName, index)));
		}

		#endregion

		#region GetParams

		public bool Contains(string name)
		{
			return Values.Any(colParam => colParam.Name == name);
		}

		public ColParam GetParam(string name)
		{
			lock(SortField)
			{
				foreach(ColParam colParam in Values.Where(colParam => colParam.Name == name))
					return colParam;

				int index = GetMaxIndex() + 1;
				AddParam(name, index);
				return GetParam(name);
			}
		}

		#endregion

		public void Save()
		{
			var colNames = new System.Text.StringBuilder();

			if(style != null)
			{
				optionFolder.OptionForced<string>(userFriendlyName).Value = style.FriendlyName;
				if(style.Grid.IsFine)
					optionFolder.OptionForced<string>(SortField).Value = style.Grid.GetSort();
			}
			else
			{
				optionFolder.OptionForced<string>(SortField).Value = SortOrder.TrimEnd(new[] { ',', ' ' }).TrimStart(new[] { ',', ' ' });
				optionFolder.OptionForced<string>(userFriendlyName).Value = FriendlyName;
			}

			foreach(ColParam col in this.Values.Where(col => !col.ToRemove))
			{
				if(col.Visible && col.Width == -1)
					col.Width = style == null ? 100 : style.GetColumnWidth(col.Name);
				colNames.Append(col.Name);
				colNames.Append(";");
				optionFolder.OptionForced<string>(col.Name).Value = col.Index.ToString() + ";" + col.Width.ToString() + ";" + col.Visible.ToString() + ";" + col.IsKeyField.ToString() + ";" + col.IsSystemField.ToString() + ";" + col.HeaderName;
			}

			optionFolder.OptionForced<string>(StyleWasInited).Value = WasInited.ToString();
			optionFolder.OptionForced<string>(ListField).Value = colNames.ToString();

			optionFolder.Save();
		}
	}
}