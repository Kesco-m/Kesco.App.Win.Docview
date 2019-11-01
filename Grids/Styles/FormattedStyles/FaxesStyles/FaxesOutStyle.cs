using System;
using System.Drawing;
using System.Windows.Forms;
using Kesco.Lib.Win;
using Kesco.Lib.Win.Document;
using Kesco.Lib.Win.Web;

namespace Kesco.App.Win.DocView.Grids.Styles.FormattedStyles.FaxesStyles
{
	public class FaxesOutStyle : FaxesStyle
	{
		private static FaxesOutStyle instance;

		#region Constructor & Instance

		protected FaxesOutStyle(DocGrid grid) : base(grid)
		{
			FriendlyName = Environment.StringResources.GetString("OutcomingFax");
		}

		public static Style Instance(DocGrid grid)
		{
			return instance ?? (instance = new FaxesOutStyle(grid));
		}

		#endregion

		#region Context Menu

		public override ContextMenu BuildContextMenu()
		{
			var contextMenu = new ContextMenu();
			if(grid.IsSingle)
			{

				contextMenu.MenuItems.AddRange(new[] { saveItem, savePartItem, saveSelectedItem, separator.CloneMenu(), toSpamItem, setPersonItem, separator.CloneMenu() });

				try
				{
					setPersonItem.Enabled = (grid.GetCurValue(Environment.FaxInData.RecipField).ToString() == grid.GetCurValue(Environment.FaxInData.RecvAddressField).ToString());
				}
				catch
				{
					setPersonItem.Enabled = false;
				}

				if(grid.MainForm != null)
					saveSelectedItem.Enabled = grid.MainForm.docControl.RectDrawn();

				contextMenu = AddOwnMenu(contextMenu);

				contextMenu.MenuItems.AddRange(new[] { openInNewWindowItem, separator.CloneMenu(), refreshItem, separator.CloneMenu(), propertiesItem });

				toSpamItem.Checked = grid.IsSpam();

				saveItem.Enabled = grid.GetBoolValue(Environment.FaxData.SavedField); // not saved

				if(Environment.CmdManager.Commands.Contains("SavePart"))
					savePartItem.Enabled = saveItem.Enabled && Environment.CmdManager.Commands["SavePart"].Enabled;
			}
			else if(grid.IsMultiple)
				contextMenu.MenuItems.AddRange(new[] { openInNewWindowItem, separator.CloneMenu(), refreshItem });
			else
				contextMenu.MenuItems.AddRange(new[] { refreshItem });

			return contextMenu;
		}

		internal override void setPersonItem_Click(object sender, EventArgs e)
		{
			var faxID = (int)grid.GetCurValue(Environment.FaxData.IDField);
			var ccDialog = new ContactDialog(Environment.CreateContactString, "personContactCategor=3&docview=yes&personContactText=" + grid.GetCurValue(Environment.FaxData.RecipField));
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

		public override bool IsFaxesOut()
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
			s = Environment.StringResources.GetString("OutgoingFax");

			// date
			obj = grid.GetValue(row, Environment.FaxOutData.DateField);
			if(obj is DateTime)
			{
				DateTime date = (DateTime)obj;
				s = TextProcessor.Stuff(s, ", ") + Environment.StringResources.GetString("Sended") + " " + date.ToString("dd.MM.yyyy HH:mm:ss");
			}

			// sender
			obj = grid.GetValue(row, Environment.FaxOutData.SenderField);
			if(obj is string)
			{
				val = (string)obj;
				if(val.Length > 0)
					s = TextProcessor.Stuff(s, ", ") + Environment.StringResources.GetString("Sender") + " " + val;
			}

			// description
			obj = grid.GetValue(row, Environment.FaxOutData.DescriptionField);
			if(obj is string)
			{
				val = (string)obj;
				if(val.Length > 0)
					s = TextProcessor.StuffSpace(s) + "(" + val + ")";
			}

			return s;
		}

		public override string DocStringToSave()
		{
			string s = "";
			object obj;
			string val;

			// doc type
			s = Environment.StringResources.GetString("SendFax");

			// date
			obj = grid.GetCurValue(Environment.FaxOutData.DateField);
			if(obj is DateTime)
			{
				DateTime date = (DateTime)obj;
				s = TextProcessor.StuffSpace(s) + date.ToString("dd.MM.yyyy HH:mm:ss");
			}

			// phone number
			obj = grid.GetCurValue(Environment.FaxOutData.RecvAddressField);
			if(obj is String)
			{
				val = (string)obj;
				if(val.Length > 0)
					s = TextProcessor.StuffSpace(s) + Environment.StringResources.GetString("To") + " " + val;
			}

			// recipient
			obj = grid.GetCurValue(Environment.FaxOutData.RecipField);
			if(obj is string)
			{
				val = (string)obj;
				if(val.Length > 0)
					s = TextProcessor.StuffNewLine(s) + Environment.StringResources.GetString("Reciever") + " " + val;
			}

			// description
			obj = grid.GetCurValue(Environment.FaxOutData.DescriptionField);
			if(obj is string)
			{
				val = (string)obj;
				if(val.Length > 0)
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
				e.CellStyle.ForeColor = Color.Red;

			if(status == -2 || grid.GetBoolValue(e.RowIndex, needMoreColorField))
				e.CellStyle.ForeColor = Color.Gray;

			if(IsColumnBool(grid.Columns[e.ColumnIndex].DataPropertyName))
			{
				e.CellStyle.ForeColor = Color.Gray;
				e.CellStyle.ForeColor = Color.Gray;
				e.CellStyle.Font = new Font(FontFamily.GenericSerif.GetName(409), 5, FontStyle.Regular);
			}

		}

		#endregion

		public override int GetColumnWidth(string colName)
		{
			if(colName == Environment.FaxData.SavedField &&
				Environment.UserSettings.FaxesOutUnsavedOnly)
				return 0;

			return base.GetColumnWidth(colName);
		}

		internal override void createContactDialog_DialogEvent(object source, DialogEventArgs e)
		{
			if(e.Dialog.DialogResult != DialogResult.OK)
				return;
			var dialog = e.Dialog as ContactDialog;
			if(dialog == null)
				return;

			dialog.DialogEvent -= createContactDialog_DialogEvent;
			if(dialog.ContactID <= 0)
				return;

			try
			{
				int conPersonID = Environment.FaxRecipientData.GetDocIntField(Environment.FaxRecipientData.PersonIDField, dialog.ContactID, -1);
				if(conPersonID > -1)
				{
					string personStr = Environment.PersonData.GetFullPerson(conPersonID);
					grid.SetValue(dialog.PersonID, Environment.FaxOutData.RecipField, personStr);
				}
			}
			catch(Exception ex)
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