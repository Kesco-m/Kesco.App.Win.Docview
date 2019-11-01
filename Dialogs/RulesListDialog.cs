using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using Kesco.App.Win.DocView.Misc;
using Kesco.Lib.Win.Data.Temp.Objects;

namespace Kesco.App.Win.DocView.Dialogs
{
    public partial class RulesListDialog : Form
    {
        public bool Edited { get; private set; }

        private int sCol;
        private static SortOrder sOrd = SortOrder.Ascending;
        private int stampID = -1;

        private ResourceManager resMngr = new ResourceManager("Kesco.App.Win.DocView.Dialogs.RuleEditDialog", Assembly.GetExecutingAssembly());

        public RulesListDialog(int stampId)
        {
            InitializeComponent();
            stampID = stampId;

            AddRulesToList(Environment.StampChecksData.GetStampRules(stampID, Environment.CurCultureInfo.TwoLetterISOLanguageName));
            Edited = false;

            listViewRules.ListViewItemSorter = new ListViewItemComparer(sOrd);
            listViewRules.Sorting = sOrd;

            listViewRules.ColumnClick += (sender, e) =>
                                             {
                                                 if (sCol != e.Column)
                                                     sOrd = SortOrder.Ascending;
                                                 else
                                                     sOrd = (sOrd == SortOrder.Ascending
                                                                 ? SortOrder.Descending
                                                                 : SortOrder.Ascending);

                                                 listViewRules.ListViewItemSorter = new ListViewItemComparer(sOrd, e.Column);
                                                 sCol = e.Column;
                                                 listViewRules.Sorting = sOrd;
                                             };
        }

        private void AddRulesToList(List<StampRule> lst)
        {
            if (lst == null)
                return;
            foreach (StampRule rule in lst)
            {
                var item = new ListViewItem(new string[3]
                                                {
                                                    rule.UserName,
                                                    (rule.OrganizationID > 0
                                                         ? rule.OrganizationName
                                                         : resMngr.GetString("PersonAllchkBox.Text")),
                                                    (rule.DocTypeID > 0
                                                         ? rule.DocTypeName
                                                         : resMngr.GetString("DocTypeAllchkBox.Text"))
                                                });
                listViewRules.Items.Add(item);
                item.Tag = rule.RuleId;
                item.SubItems[0].Tag = rule.UserId;
                item.SubItems[1].Tag = rule.OrganizationID;
                item.SubItems[2].Tag = rule.DocTypeID;
            }
        }

        private bool AddRuleToDb(int[] di, StampRule r)
        {
            return Environment.StampChecksData.SetStampRules(di, stampID, r);
        }

        private bool UpdRuleInDb(int[] di, StampRule r)
        {
            return Environment.StampChecksData.UpdStampRules(di, r);
        }

        private void listViewRules_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnEdit.Enabled = 1 == listViewRules.SelectedItems.Count;
            btnDel.Enabled = listViewRules.SelectedItems.Count > 0;
        }

        internal IEnumerable<StampRule> GetRules()
        {
            var rules = new List<StampRule>(listViewRules.Items.Count);
            rules.AddRange(from ListViewItem item in listViewRules.Items
                           select new StampRule
                                      {
                                          RuleId = (int) item.Tag,
                                          UserId = (int) item.SubItems[0].Tag,
                                          UserName = item.Text,
                                          DocTypeID = (int) item.SubItems[2].Tag,
                                          DocTypeName = item.SubItems[2].Text,
                                          OrganizationID = (int) item.SubItems[1].Tag,
                                          OrganizationName = item.SubItems[2].Text
                                      });
            return rules;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            bool rez = false;
            int cI = -1;
            using (var dlg = new RuleEditDialog())
            {
                object idx = -1;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    if (!UserExists(dlg.UserId, dlg.DocTypeId, dlg.OrganizationID, ref idx))
                    {
                        rez = AddRuleToDb(null, new StampRule
                                                    {
                                                        UserId = dlg.UserId,
                                                        UserName = dlg.UserName,
                                                        DocTypeID = dlg.DocTypeId,
                                                        DocTypeName = dlg.DocTypeName,
                                                        OrganizationID = dlg.OrganizationID,
                                                        OrganizationName = dlg.OrganizationName
                                                    });
                    }
                    else
                    {
                        if (idx is int)
                        {
                            MessageBox.Show(
                                Environment.StringResources.GetString("Kesco.App.Win.DocView.Dialogs.Rules.Exists"));
                            cI = listViewRules.Items[(int) idx].Index;
                        }
                        else
                        {
                            if (MessageBox.Show(
                                string.Format(
                                    Environment.StringResources.GetString("Kesco.App.Win.DocView.Dialogs.Rules.Exists1"),
                                    System.Environment.NewLine + System.Environment.NewLine +
                                    string.Join(System.Environment.NewLine,
                                                listViewRules.Items.Cast<ListViewItem>().Where(
                                                    li => (idx as int[]).Contains(li.Index)).
                                                    Select(
                                                        li =>
                                                        "[" + li.Text + "] [" + li.SubItems[1].Text + "] [" +
                                                        li.SubItems[2].Text + "]").ToArray()) +
                                    System.Environment.NewLine),
                                "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                rez =
                                    AddRuleToDb(
                                        listViewRules.Items.Cast<ListViewItem>().Where(
                                            li => (idx as int[]).Contains(li.Index)).Select(li => (int) li.Tag).ToArray(),
                                        new StampRule
                                            {
                                                UserId = dlg.UserId,
                                                UserName = dlg.UserName,
                                                DocTypeID = dlg.DocTypeId,
                                                DocTypeName = dlg.DocTypeName,
                                                OrganizationID = dlg.OrganizationID,
                                                OrganizationName = dlg.OrganizationName
                                            });
                            }
                        }
                    }
                }
                if (rez)
                {
                    List<StampRule> lst = Environment.StampChecksData.GetStampRules(stampID,
                                                                                    Environment.CurCultureInfo.
                                                                                        TwoLetterISOLanguageName);
                    listViewRules.Items.Clear();
                    AddRulesToList(lst);
                    ListViewItem it = listViewRules.Items.Cast<ListViewItem>().FirstOrDefault(ci =>
                                                                                              (int) ci.SubItems[0].Tag ==
                                                                                              dlg.UserId &&
                                                                                              (int) ci.SubItems[2].Tag ==
                                                                                              dlg.DocTypeId &&
                                                                                              (int) ci.SubItems[1].Tag ==
                                                                                              dlg.OrganizationID);
                    if (it != null)
                        it.Selected = true;

                    btnOK.Enabled = true;
                    Edited = true;
                }
                else if (cI >= 0)
                {
                    listViewRules.SelectedItems.Clear();
                    listViewRules.Items[cI].Selected = true;
                }
            }
            listViewRules.Focus();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (listViewRules.SelectedItems.Count == 0)
                return;

            bool rez = false;
            ListViewItem item = listViewRules.SelectedItems[0];
            int cI = -1;
            using (
                var dlg = new RuleEditDialog((int) item.SubItems[0].Tag, item.Text, (int) item.SubItems[2].Tag,
                                             item.SubItems[2].Text, (int) item.SubItems[1].Tag, item.SubItems[1].Text))
            {
                object idx = item.Index;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    if (!UserExists(dlg.UserId, dlg.DocTypeId, dlg.OrganizationID, ref idx))
                    {
                        rez = UpdRuleInDb(null, new StampRule
                                                    {
                                                        RuleId = (int) item.Tag,
                                                        UserId = dlg.UserId,
                                                        UserName = dlg.UserName,
                                                        DocTypeID = dlg.DocTypeId,
                                                        DocTypeName = dlg.DocTypeName,
                                                        OrganizationID = dlg.OrganizationID,
                                                        OrganizationName = dlg.OrganizationName
                                                    });
                    }
                    else
                    {
                        if (idx is int)
                        {
                            MessageBox.Show(
                                Environment.StringResources.GetString("Kesco.App.Win.DocView.Dialogs.Rules.Exists"));
                            cI = listViewRules.Items[(int) idx].Index;
                        }
                        else
                        {
                            if (MessageBox.Show(
                                string.Format(
                                    Environment.StringResources.GetString("Kesco.App.Win.DocView.Dialogs.Rules.Exists1"),
                                    System.Environment.NewLine + System.Environment.NewLine +
                                    string.Join(System.Environment.NewLine,
                                                listViewRules.Items.Cast<ListViewItem>().Where(
                                                    li => (idx as int[]).Contains(li.Index)).
                                                    Select(
                                                        li =>
                                                        "[" + li.Text + "] [" + li.SubItems[1].Text + "] [" +
                                                        li.SubItems[2].Text + "]").ToArray()) +
                                    System.Environment.NewLine),
                                "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                rez =
                                    UpdRuleInDb(
                                        listViewRules.Items.Cast<ListViewItem>().Where(
                                            li => (idx as int[]).Contains(li.Index)).Select(li => (int) li.Tag).ToArray(),
                                        new StampRule
                                            {
                                                RuleId = (int) item.Tag,
                                                UserId = dlg.UserId,
                                                UserName = dlg.UserName,
                                                DocTypeID = dlg.DocTypeId,
                                                DocTypeName = dlg.DocTypeName,
                                                OrganizationID = dlg.OrganizationID,
                                                OrganizationName = dlg.OrganizationName
                                            });
                            }
                        }
                    }
                }
                if (rez)
                {
                    List<StampRule> lst = Environment.StampChecksData.GetStampRules(stampID, Environment.CurCultureInfo.TwoLetterISOLanguageName);
                    listViewRules.Items.Clear();
                    AddRulesToList(lst);
                    ListViewItem it = listViewRules.Items.Cast<ListViewItem>().FirstOrDefault(ci =>
                                                                                              (int) ci.SubItems[0].Tag ==
                                                                                              dlg.UserId &&
                                                                                              (int) ci.SubItems[2].Tag ==
                                                                                              dlg.DocTypeId &&
                                                                                              (int) ci.SubItems[1].Tag ==
                                                                                              dlg.OrganizationID);
                    if (it != null)
                        it.Selected = true;

                    btnOK.Enabled = true;
                    Edited = true;
                }
                else if (cI >= 0)
                {
                    listViewRules.SelectedItems.Clear();
                    listViewRules.Items[cI].Selected = true;
                }
            }
            listViewRules.Focus();
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                Environment.StringResources.GetString("Kesco.App.Win.DocView.Dialogs.Rules.DelRulesConf"),
                Environment.StringResources.GetString("Confirmation"), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) ==
                DialogResult.Yes)
            {
                if (
                    Environment.StampChecksData.SetStampRules(
                        listViewRules.SelectedItems.Cast<ListViewItem>().Select(i => (int) i.Tag).ToArray(), 0, null))
                    foreach (ListViewItem item in listViewRules.SelectedItems)
                        listViewRules.Items.Remove(item);

                btnOK.Enabled = true;
                Edited = true;
            }
            listViewRules.Focus();
        }

        private bool UserExists(int userId, int docTypeId, int personId, ref object ruleIndex)
        {
            var stId = (int) ruleIndex;
            var ret = (from ListViewItem item in listViewRules.Items
                       where item.Index != stId &&
                             (int) item.SubItems[0].Tag == userId &&
                             ((int) item.SubItems[2].Tag == docTypeId || docTypeId == -1) &&
                             ((int) item.SubItems[1].Tag == personId || personId == -1)
                       select item.Index).ToList();

            foreach (ListViewItem item in listViewRules.Items.Cast<ListViewItem>().Where(
                item => item.Index != stId && (int) item.SubItems[0].Tag == userId &&
                        ((int) item.SubItems[2].Tag == docTypeId || (int) item.SubItems[2].Tag == -1) &&
                        ((int) item.SubItems[1].Tag == personId || (int) item.SubItems[1].Tag == -1)))
            {
                ruleIndex = item.Index;
                return true;
            }

            if (ret.Count > 0)
            {
                ruleIndex = ret.ToArray();
                return true;
            }

            return false;
        }

        private void listViewRules_DoubleClick(object sender, EventArgs e)
        {
            btnEdit_Click(null, null);
        }

        private void listViewRules_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete && !(e.Alt || e.Control || e.Shift))
                btnDel_Click(null, null);
		}
    }
}