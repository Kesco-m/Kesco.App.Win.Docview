using System;
using System.Drawing;
using System.Windows.Forms;
using Kesco.Lib.Win;
using Kesco.Lib.Win.Document;
using Kesco.Lib.Win.Web;

namespace Kesco.App.Win.DocView.Grids.Styles.FormattedStyles.FaxesStyles
{
    public class FaxesInStyle : FaxesStyle
    {
        private static FaxesInStyle instance;

        protected MenuItem faxDescrItem;

        #region Constructor & Instance

        protected FaxesInStyle(DocGrid grid)
            : base(grid)
        {
            // описание факса
            faxDescrItem = new MenuItem
                               {
                                   Text =
                                       Environment.StringResources.GetString("DocGrid.Styles.FaxesInStyle.Message1") +
                                       "..."
                               };

            faxDescrItem.Click += faxDescrItem_Click;

            FriendlyName = Environment.StringResources.GetString("IncomingFaxDoc");
        }

        public static Style Instance(DocGrid grid)
        {
            return instance ?? (instance = new FaxesInStyle(grid));
        }

        #endregion

        #region Context Menu

        public override ContextMenu BuildContextMenu()
        {
            var contextMenu = new ContextMenu();

            if (grid.IsSingle)
            {
                contextMenu.MenuItems.AddRange(new[] { saveItem, savePartItem, saveSelectedItem, separator.CloneMenu(), faxDescrItem, toSpamItem, setPersonItem, separator.CloneMenu() });

                try
                {
                    setPersonItem.Enabled = !(grid.GetCurValue(Environment.FaxInData.CSIDField) == grid.GetCurValue(Environment.FaxInData.SenderField) || grid.GetCurValue(Environment.FaxInData.SenderField) == grid.GetCurValue(Environment.FaxInData.SenderAddressField));
                }
                catch
                {
                    setPersonItem.Enabled = false;
                }

                if (grid.MainForm != null)
                {
                    saveSelectedItem.Enabled = grid.MainForm.docControl.RectDrawn();
                }

                contextMenu = AddOwnMenu(contextMenu);

                contextMenu.MenuItems.AddRange(new[] { openInNewWindowItem, separator.CloneMenu(), refreshItem, separator.CloneMenu(), propertiesItem});

                toSpamItem.Checked = grid.IsSpam();

                saveItem.Enabled = !grid.GetBoolValue(Environment.FaxData.SavedField); // not saved

                savePartItem.Enabled = false;
            }
            else if (grid.IsMultiple)
                contextMenu.MenuItems.AddRange(new[] { openInNewWindowItem, separator.CloneMenu(), refreshItem });
            else
                contextMenu.MenuItems.AddRange(new[] { refreshItem });

            return contextMenu;
        }

        private void faxDescrItem_Click(object sender, EventArgs e)
        {
            Environment.CmdManager.Commands["FaxDescriptionEdit"].Execute();
        }

        internal override void setPersonItem_Click(object sender, EventArgs e)
        {
            var faxID = (int)grid.GetCurValue(Environment.FaxData.IDField);
            var ccDialog = new ContactDialog(Environment.CreateContactString, "personContactCategor=3&docview=yes&personContactText=" + grid.GetCurValue(Environment.FaxData.SenderAddressField));
            ccDialog.DialogEvent += createContactDialog_DialogEvent;
            ccDialog.PersonID = faxID;
            ccDialog.Show();
        }
        #endregion

        #region Is

        public override bool IsShownInSettings()
        {
            return true;
        }

        public override bool IsFaxesIn()
        {
            return true;
        }

        #endregion

        #region CurDocString

        public override string MakeDocString(int row)
        {
            string s = "";
            object obj;
            string val;

            // doc type
            s = Environment.StringResources.GetString("IncomingFax");

            // date
            obj = grid.GetValue(row, Environment.FaxInData.DateField);
            if (obj is DateTime)
            {
                DateTime date = (DateTime)obj;
                s = TextProcessor.Stuff(s, ", ") + Environment.StringResources.GetString("Received") + " " + date.ToString();
            }

            // sender
            obj = grid.GetValue(row, Environment.FaxInData.SenderField);
            if (obj is string)
            {
                val = (string)obj;
                if (val.Length > 0)
                    s = TextProcessor.Stuff(s, ", ") + Environment.StringResources.GetString("Sender") + " " + val;
            }

            // description
            obj = grid.GetValue(row, Environment.FaxInData.DescriptionField);
            if (obj is string)
            {
                val = (string)obj;
                if (val.Length > 0)
                    s = TextProcessor.StuffSpace(s) + "(" + val + ")";
            }

            return s;
        }

        public override string DocStringToSave()
        {
            string s = "";
            object obj;
            DateTime date;
            string val;

            // doc type
            s = Environment.StringResources.GetString("DocGrid.Styles.FaxesInStyle.DocStringToSave.Message1");

            // date
            obj = grid.GetCurValue(Environment.FaxInData.DateField);
            if (obj is DateTime)
            {
                date = (DateTime)obj;
                s = TextProcessor.StuffSpace(s) + date.ToString("dd.MM.yyyy HH:mm:ss");
            }

            // sender
            obj = grid.GetCurValue(Environment.FaxInData.SenderField);
            if (obj is string)
            {
                val = (string)obj;
                if (val.Length > 0)
                    s = TextProcessor.StuffNewLine(s) + Environment.StringResources.GetString("Sender") + " " + val;
            }

            // description
            obj = grid.GetCurValue(Environment.FaxInData.DescriptionField);
            if (obj is string)
            {
                val = (string)obj;
                if (val.Length > 0)
                    s = TextProcessor.StuffNewLine(s) + Environment.StringResources.GetString("Description") + ": " + val;
            }

            return s;
        }

        #endregion

        #region Format

        protected override void FormatColor(DataGridViewCellPaintingEventArgs e)
        {
			short status = 0;

			object obj = grid.GetValue(e.RowIndex, needColorField);
			if(obj is short)
				status = (short)obj;
			if(status != 0)
				e.CellStyle.ForeColor = Color.LightPink;

			if(grid.GetBoolValue(e.RowIndex, needMoreColorField))
				e.CellStyle.ForeColor = Color.Gray;
        }

        #endregion

        public override int GetColumnWidth(string colName)
        {
            if (colName == Environment.FaxData.SavedField &&
                Environment.UserSettings.FaxesInUnsavedOnly)
                return 0;

            return base.GetColumnWidth(colName);
        }

        internal override void createContactDialog_DialogEvent(object source, DialogEventArgs e)
        {
            if (e.Dialog.DialogResult != DialogResult.OK) 
                return;
            var dialog = e.Dialog as ContactDialog;
            if (dialog == null)
                return;
            dialog.DialogEvent -= createContactDialog_DialogEvent;
            if (dialog.ContactID <= 0) 
                return;
            try
            {
                object obj = Environment.FaxRecipientData.GetField(Environment.FaxRecipientData.PersonIDField, dialog.ContactID);
                if (obj is int)
                {
                    var conPersonID = (int)obj;
                }
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }

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