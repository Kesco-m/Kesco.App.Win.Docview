using System.Linq;
using System.Windows.Forms;

namespace Kesco.App.Win.DocView.Grids.Styles.FormattedStyles
{
    public class DiskImagesStyle : Style
    {
        private static DiskImagesStyle instance;

        #region Constructor & Instance

        protected DiskImagesStyle(DocGrid grid)
            : base(grid)
        {
            idField = keyField = Environment.ImageReader.FullNameField;
        }

        public static Style Instance(DocGrid grid)
        {
            return instance ?? (instance = new DiskImagesStyle(grid));
        }

        #endregion

        #region Columns

        public override int GetColumnWidth(string colName)
        {
            if (colName == Environment.ImageReader.DateField)
                return 100;

            if (colName == Environment.ImageReader.NameField)
                return 100;

            return base.GetColumnWidth(colName);
        }

        public override string GetColumnHeaderName(string colName)
        {
            if (colName == Environment.ImageReader.DateField)
                return Environment.StringResources.GetString("Date");

            if (colName == Environment.ImageReader.NameField)
                return Environment.StringResources.GetString("Name");

            return base.GetColumnHeaderName(colName);
        }

        public override bool IsColumnSystem(string column)
        {
            return column != Environment.ImageReader.DateField && column != Environment.ImageReader.NameField;
        }

        public override bool IsColumnVisible(string column)
        {
            return !IsColumnSystem(column);
        }

        #endregion

        public override void Init()
        {
            int index = 2;
            foreach (ColParam col in colParams.Values.Where(col => IsColumnSystem(col.Name)))
            {
                index++;
                col.Index = index;
                col.Visible = false;
            }

            colParams.GetParam(Environment.ImageReader.NameField).Index = 1;
            colParams.GetParam(Environment.ImageReader.DateField).Index = 0;
            colParams.GetParam(Environment.ImageReader.NameField).Visible = true;
            colParams.GetParam(Environment.ImageReader.DateField).Visible = true;
            base.Init();
            colParams.WasInited = false;
        }

        public override ContextMenu BuildContextMenu()
        {
            var contextMenu = new ContextMenu();
            if (grid.IsSingle)
            {

                contextMenu.MenuItems.AddRange(new[] { saveItem, savePartItem, saveSelectedItem, separator.CloneMenu(), openInNewWindowItem, separator.CloneMenu(), propertiesItem, separator.CloneMenu(), refreshItem });

                if (grid.MainForm != null)
                {
                    savePartItem.Enabled = (grid.MainForm.docControl.PageCount > 1) && Environment.CmdManager.Commands["SavePart"].Enabled;
                    saveSelectedItem.Enabled = grid.MainForm.docControl.RectDrawn();
                }
            }
            else if (grid.IsMultiple)
                contextMenu.MenuItems.AddRange(new[] { openInNewWindowItem, separator.CloneMenu(), refreshItem });
            else
                contextMenu = null;
            
                return contextMenu;
            }

        public override string MakeDocString(int row)
        {
            return grid.GetValue(row, Environment.ImageReader.NameField) as string ?? string.Empty;
        }

        public override bool IsDiskImages()
        {
            return true;
        }

        public override bool UseLock()
        {
            return true;
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