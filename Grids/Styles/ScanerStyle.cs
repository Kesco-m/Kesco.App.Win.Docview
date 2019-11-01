using System;
using System.Windows.Forms;
using Kesco.Lib.Win.Document;

namespace Kesco.App.Win.DocView.Grids.Styles
{
	/// <summary>
	/// Стиль для отображения списка сканов
	/// </summary>
	public class ScanerStyle : Style
	{
		private static ScanerStyle instance;

		#region Constructor & Instance

		protected ScanerStyle(DocGrid grid) : base(grid)
		{
            idField = keyField = Environment.ScanReader.FullNameField;
			FriendlyName = Environment.StringResources.GetString("Scaner");
		}

		public static Style Instance(DocGrid grid)
		{
		    return instance ?? (instance = new ScanerStyle(grid));
		}

	    #endregion

        #region Columns

        public override int GetColumnWidth(string colName)
		{
			if (colName == Environment.ScanReader.DateField)
				return 100;
			if (colName == Environment.ScanReader.ChangedDateField)
				return 100;
			if (colName == Environment.ScanReader.DescrField)
				return 220;

			return base.GetColumnWidth(colName);
		}

		public override string GetColumnHeaderName(string colName)
		{
		    if (colName == Environment.ScanReader.DateField)
		        return Environment.StringResources.GetString("LocalDate");

		    if (colName == Environment.ScanReader.ChangedDateField)
		        return Environment.StringResources.GetString("ChangeDate");
		    if (colName == Environment.ScanReader.DescrField)
		        return Environment.StringResources.GetString("Description");

		    return base.GetColumnHeaderName(colName);
		}

	    public override bool IsColumnSystem(string column)
        {
            return base.IsColumnSystem(column) || (column != Environment.ScanReader.DateField && column != Environment.ScanReader.ChangedDateField && column != Environment.ScanReader.DescrField);
        }

		public override ContextMenu BuildContextMenu()
		{
            var contextMenu = new ContextMenu();
            if (grid.IsSingle)
			{

                contextMenu.MenuItems.AddRange(new[] { saveItem, savePartItem, saveSelectedItem, separator.CloneMenu(), openInNewWindowItem, separator.CloneMenu(), refreshItem, separator.CloneMenu(), propertiesItem, separator.CloneMenu()});

				if (grid.MainForm != null)
				{
					savePartItem.Enabled = (grid.MainForm.docControl.PageCount > 1);
					saveSelectedItem.Enabled = grid.MainForm.docControl.RectDrawn();
				}
            }
            else if (grid.IsMultiple)
		        contextMenu.MenuItems.Add(openInNewWindowItem);
            else
                contextMenu.MenuItems.AddRange(new[] { refreshItem });

		        return contextMenu;
		    }

        #endregion

		public override bool IsScaner()
		{
			return true;
        }

        public override bool IsShownInSettings()
        {
            return true;
        }

		#region CurDocString

		public override string MakeDocString(int row)
		{
			string s = "";
			object obj;

		    // doc type
			s = Environment.StringResources.GetString("ScanedDoc").ToLower();

			// date
			obj = grid.GetValue(row, Environment.ScanReader.DateField);
			if (obj is DateTime)
			{
			    DateTime date = (DateTime)obj;
			    s = TextProcessor.StuffSpace(s) + Environment.StringResources.GetString("Of") + date.ToString("dd.MM.yyyy");
			}

		    // description
			obj = grid.GetValue(row, Environment.ScanReader.DescrField);
			if (obj is string)
			{
			    string val = (string)obj;
			    if (val.Length > 0)
					s = TextProcessor.StuffSpace(s) + "(" + val + ")";
			}

		    return s;
		}

		public override string DocStringToSave()
		{
		    // doc type
			string s = Environment.StringResources.GetString("ScanedDoc");

			// date
			object obj = grid.GetCurValue(Environment.ScanReader.DateField);
			if (obj is DateTime)
			{
			    DateTime date = (DateTime)obj;
			    s = TextProcessor.StuffSpace(s) + date.ToString("dd.MM.yyyy");
			}

		    // description
			obj = grid.GetCurValue(Environment.ScanReader.DescrField);
			if (obj is string)
			{
			    string val = (string)obj;
			    if (val.Length > 0)
					s = TextProcessor.StuffNewLine(s) + Environment.StringResources.GetString("Description") + ": " + val;
			}

		    return s;
		}

		#endregion

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