using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Kesco.App.Win.DocView.Misc;
using Kesco.Lib.Win.Data.Temp.Objects;

namespace Kesco.App.Win.DocView.Dialogs
{
    public partial class RightsListDialog : Form
    {
        private int stampID = -1;
        private bool edited;
        private int sCol;
        private static SortOrder sOrd = SortOrder.Ascending;

        /// <summary>
        ///   Конструктор.
        /// </summary>
        /// <param name="stampID"> код штампа для редактирования прав </param>
        public RightsListDialog(int stampID)
        {
            this.stampID = stampID;
            InitializeComponent();

            // заполняем список прав
            foreach (var right in Environment.StampRightsData.GetStampRights(stampID))
                AddRightToList(right.UserId, right.UserName, right.EnableProxies);
            edited = false;

            listViewRights.ListViewItemSorter = new RightsListViewItemComparer(sOrd);
            listViewRights.Sorting = sOrd;

            listViewRights.ColumnClick += (sender, e) =>
                                              {
                                                  if (sCol != e.Column)
                                                      sOrd = SortOrder.Ascending;
                                                  else
                                                      sOrd = (sOrd == SortOrder.Ascending
                                                                  ? SortOrder.Descending
                                                                  : SortOrder.Ascending);

                                                  listViewRights.ListViewItemSorter = new RightsListViewItemComparer(sOrd, e.Column);
                                                  sCol = e.Column;
                                                  listViewRights.Sorting = sOrd;
                                              };
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var dlg = new RightEditDialog())
                if (dlg.ShowDialog() == DialogResult.OK && !UserExists(dlg.UserId))
                {
                    edited = true;
                    AddRightToList(dlg.UserId, dlg.UserName, dlg.EnableProxies);
                    btnOK.Enabled = true;
                }
        }

        private bool UserExists(int userId)
        {
            return listViewRights.Items.Cast<ListViewItem>().Any(item => (int) item.Tag == userId);
        }

        /// <summary>
        ///   Добавление права в контрол.
        /// </summary>
        /// <param name="userId"> ID сотрудника </param>
        /// <param name="userName"> ФИО сотрудника </param>
        /// <param name="enableProxies"> давать права замещающим </param>
        private void AddRightToList(int userId, string userName, bool enableProxies)
        {
            ListViewItem item = listViewRights.Items.Add(userName);
            item.Tag = userId;

            ListViewItem.ListViewSubItem subItem = item.SubItems.Add(enableProxies ? Environment.StringResources.GetString("Yes") : Environment.StringResources.GetString("No"));
            subItem.Tag = enableProxies;
            edited = true;
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listViewRights.SelectedItems)
            {
                listViewRights.Items.Remove(item);
                edited = true;
                btnOK.Enabled = true;
            }
        }

        private void listViewRights_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnEdit.Enabled = 1 == listViewRights.SelectedItems.Count;
            btnDel.Enabled = listViewRights.SelectedItems.Count > 0;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            ListViewItem item = listViewRights.SelectedItems[0];
            using (var dlg = new RightEditDialog(item.Text, (bool) item.SubItems[1].Tag))
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    item.SubItems[1].Tag = dlg.EnableProxies;
                    item.SubItems[1].Text = dlg.EnableProxies ? Environment.StringResources.GetString("Yes") : Environment.StringResources.GetString("No");
                    btnOK.Enabled = true;
                    edited = true;
                }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (edited)
            {
                var rights = new List<StampRight>(listViewRights.Items.Count);
                rights.AddRange(from ListViewItem item in listViewRights.Items
                                select new StampRight
                                           {
                                               UserId = (int) item.Tag,
                                               UserName = item.Text,
                                               EnableProxies = (bool) item.SubItems[1].Tag
                                           });
                if (Environment.StampRightsData.SetStampRights(stampID, rights))
                    Close();
            }
            else
                Close();
        }
    }
}