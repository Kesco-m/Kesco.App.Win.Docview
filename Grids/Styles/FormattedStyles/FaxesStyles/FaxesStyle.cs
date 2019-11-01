using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Kesco.App.Win.DocView.Items;
using Kesco.Lib.Win.Document;

namespace Kesco.App.Win.DocView.Grids.Styles.FormattedStyles.FaxesStyles
{
    public class FaxesStyle : FormattedStyle
    {
        protected MenuItem toSpamItem;
        protected MenuItem toDBDocItem;
        protected MenuItem setPersonItem;
		protected string needMoreColorField;

		protected FaxesStyle(DocGrid grid) : base(grid)
		{
			idField = Environment.FaxData.IDField;
			keyField = Environment.FaxData.FileNameField;

			needColorField = Environment.FaxData.StatusField;
			needMoreColorField = Environment.FaxOutData.SpamField;
			needBoldField = Environment.FaxInData.ReadField;

			toSpamItem = new MenuItem();
			toDBDocItem = new MenuItem();
			setPersonItem = new MenuItem();

			// отправить в спам
			toSpamItem.Text = Environment.StringResources.GetString("Spam");
			toSpamItem.Click += toSpamItem_Click;

			// перейти к документу в архиве
			toDBDocItem.Text = Environment.StringResources.GetString("FaxPropertiButtonControl.Message1") + "...";

			// добавление лица факсу
			setPersonItem.Text = Environment.StringResources.GetString("AddPerson") + "...";
			setPersonItem.Click += setPersonItem_Click;
		}

        public override int GetColumnWidth(string colName)
        {
            if (colName == Environment.FaxData.DateField)
                return 100;

            if (colName == Environment.FaxData.SenderField)
                return 100;

            if (colName == Environment.FaxData.RecipField)
                return 100;

            if (colName == Environment.FaxData.DescriptionField)
                return 220;

            if (colName == Environment.FaxData.SavedField)
                return 80;

			if(colName == Environment.FaxData.ReadField)
				return 20;

            return base.GetColumnWidth(colName);
        }

        public override string GetColumnHeaderName(string colName)
        {
            if (colName == Environment.FaxData.DateField)
                return Environment.StringResources.GetString("Date");

            if (colName == Environment.FaxData.SenderField)
                return Environment.StringResources.GetString("Sender").Replace(":", string.Empty);

            if (colName == Environment.FaxData.RecipField)
                return Environment.StringResources.GetString("Reciever").Replace(":", string.Empty);

            if (colName == Environment.FaxData.DescriptionField)
                return Environment.StringResources.GetString("Description");

            if (colName == Environment.FaxData.SavedField)
                return Environment.StringResources.GetString("Saved");

            return base.GetColumnHeaderName(colName);
        }

        public override bool IsColumnSystem(string column)
        {
            return base.IsColumnSystem(column) || (column != Environment.FaxData.DateField && column != Environment.FaxData.SenderField && column != Environment.FaxData.RecipField && column != Environment.FaxData.DescriptionField);
        }

        public override bool IsColumnBool(string colName)
        {
            if (colName == Environment.FaxData.SavedField)
                return true;

            return false;
        }

        public override bool IsFaxes()
        {
            return true;
        }

        #region Context Menu

        protected virtual ContextMenu AddOwnMenu(ContextMenu menu)
        {
            toDBDocItem.MenuItems.Clear();

            var faxID = (int)grid.GetCurValue(Environment.FaxData.IDField);

            // есть ли у факса изображения, сохраненные в архив?
            List<int> ids = Environment.FaxData.GetFaxDBDocs(faxID);
            if (ids != null)
            {
                foreach (var item in from t in ids where t >= 1 select new IDMenuItem(t) {Text = DBDocString.Format(t)})
                {
                    item.Click += toDBDoc_Click;
                    toDBDocItem.MenuItems.Add(item);
                }
            }
            toDBDocItem.Enabled = (toDBDocItem.MenuItems.Count > 0);
            menu.MenuItems.Add(toDBDocItem);

            return menu;
        }

        private void toSpamItem_Click(object sender, EventArgs e)
        {
            Environment.CmdManager.Commands["FaxToSpam"].Execute();
        }

        private void toDBDoc_Click(object sender, EventArgs e)
        {
            if (!(sender is IDMenuItem))
                return;
            var item = sender as IDMenuItem;
            Forms.MainFormDialog.returnID = item.ID;
            Environment.CmdManager.Commands["Return"].Execute();
        }

        virtual internal void setPersonItem_Click(object sender, EventArgs e)
        {
        }

        virtual internal void createContactDialog_DialogEvent(object source, Kesco.Lib.Win.DialogEventArgs e)
        {
        }
        #endregion
    }
}