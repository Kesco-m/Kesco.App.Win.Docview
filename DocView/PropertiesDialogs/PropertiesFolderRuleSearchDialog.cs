using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using Kesco.App.Win.DocView.Forms;
using Kesco.Lib.Win.Data.Business.Corporate;
using Kesco.Lib.Win.Data.Business.Documents;
using Kesco.Lib.Win.Data.Business.Persons;
using Kesco.Lib.Win.Data.DALC.Documents;
using Kesco.Lib.Win.Data.Options;

namespace Kesco.App.Win.DocView.PropertiesDialogs
{
    /// <summary>
    ///   ƒиалог редактировани€ правил сортировки
    /// </summary>
    public partial class PropertiesFolderRuleSearchDialog
    {
        private bool refreshing;
        private XmlDocument state; //объект дл€ сохранени€ промежуточных состо€ний myRule
        private FolderRule myRule;

        private bool changeMode;
        private bool OpenMode;

        private Option currentOption;

        public PropertiesFolderRuleViewDialog ParentViewDialog;

        public PropertiesFolderRuleSearchDialog(int folderID, string folderName, Rule rule)
        {
            refreshing = false;
            state = new XmlDocument();
            state.AppendChild(state.CreateElement("myRule"));
            myRule = new FolderRule(rule.ID);
            if (myRule.IsNew)
            {
                changeMode = false;
                myRule.Folder = new Folder(folderID);
                Text = Environment.StringResources.GetString("Properties.PropertiesFolderRuleSearchDialog.Message1") +
                    " \"" + myRule.Folder.Name + "\"";
            }
            else
            {
                changeMode = true;
                Text = Environment.StringResources.GetString("Properties.PropertiesFolderRuleSearchDialog.Message2") +
                    " \"" + myRule.Folder.Name + "\"";
            }

            OpenMode = true;

            InitializeComponent();
        }

        //LOAD
        private void PropertiesFolderRuleSearchDialog_Load(object sender, EventArgs e)
        {
            optionsSettings.Command += optionsSettings_Command;
            if (ParentViewDialog != null)
                ParentViewDialog.Enabled = false;
            OpenMode = false;
            RefreshAll();
        }

        private void optionsSettings_Command(Label lLab)
        {
            if (refreshing)
                return;
            if (OpenMode)
                return;

            string url = lLab.Tag.ToString();
            Option option = myRule.GetOption(url.TrimStart('#'));
            if (option != null)
                EditOption(option);
        }

        //CLOSING
        private void PropertiesFolderRuleSearchDialog_Closing(object sender, CancelEventArgs e)
        {
            if (ParentViewDialog != null)
                ParentViewDialog.FilterDialogClose(changeMode, DialogResult);
        }

        private void RefreshAll()
        {
            refreshing = true;
            OutMessRadButton.Checked = (myRule.Mode == FolderRuleMode.MessageSent);
            InMessRadButton.Checked = (myRule.Mode == FolderRuleMode.MessageReceived);

            ParamLv.Items.Clear();
            var options = new Option[FolderRule.maxOptionsInGroupCount];
            int n = myRule.GetFolderRuleOptions(options);
            for (int i = 0; i < n; i++)
            {
                try
                {
                    ParamLv.Items.Add(new ListViewItem
                                      {Text = options[i].GetCaption(), Checked = options[i].Enabled, Tag = options[i].Name});
                }
                catch (Exception ex)
                {
                    Lib.Win.Error.ErrorShower.OnShowError(null, ex.Message, "");
                }
            }

            optionsSettings.AddSettings(myRule.GetHtml());
            RuleNameTextBox.Text = myRule.GetShortText();
            SaveRuleButton.Enabled = !myRule.IsEmpty;
            refreshing = false;
        }

        #region USER INPUT EVENT HANDLERS

        private void InMessRadButton_CheckedChanged(object sender, EventArgs e)
        {
            if (refreshing)
                return;
            if (OpenMode)
                return;
            myRule.Mode = FolderRuleMode.MessageReceived;
            RefreshAll();
        }

        private void OutMessRadButton_CheckedChanged(object sender, EventArgs e)
        {
            if (refreshing)
                return;
            if (OpenMode)
                return;
            myRule.Mode = FolderRuleMode.MessageSent;
            RefreshAll();
        }

        private void ParamLv_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (refreshing)
                return;
            if (OpenMode)
                return;
            ListViewItem lvi = ParamLv.Items[e.Index];
            Option opt = myRule.GetOption((string) lvi.Tag);
            opt.Enabled = e.NewValue == CheckState.Checked;
            e.NewValue = e.CurrentValue; //это устран€ет глюки :)
            RefreshAll();
        }

        private void SaveRuleButton_Click(object sender, EventArgs e)
        {
            if (refreshing)
                return;
            if (OpenMode)
                return;

            myRule.Name = RuleNameTextBox.Text;

            bool closeOnException = false;
            try
            {
                //1. провер€ем наличие папки
                if (myRule.Folder == null ||
                    (new Folder(myRule.Folder.ID)).IsUnavailable)
                {
                    closeOnException = true;
                    throw new Exception(Environment.StringResources.GetString("FolderNotExist"));
                }
                //2. провер€ем наличие правила с теми-же параметрами RuleExist
                if (myRule.Exists())
                    throw new Exception(
                        Environment.StringResources.GetString(
                            "Properties.PropertiesFolderRuleSearchDialog.SaveRuleButton_Click.Message2"));
                //3. провер€ем задана ли хоть одна опции€ правила
                if (myRule.IsEmpty)
                    throw new Exception(
                        Environment.StringResources.GetString(
                            "Properties.PropertiesFolderRuleSearchDialog.SaveRuleButton_Click.Message3"));
                //4. провер€ем им€ правила
                if (myRule.Name.Equals(""))
                    throw new Exception(
                        Environment.StringResources.GetString(
                            "Properties.PropertiesFolderRuleSearchDialog.SaveRuleButton_Click.Message4"));
                //на пустую строку
                if (myRule.NameExists())
                    throw new Exception(
                        Environment.StringResources.GetString(
                            "Properties.PropertiesFolderRuleSearchDialog.SaveRuleButton_Click.Message5"));
                //сохран€ем
                myRule.SaveRule();
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                Kesco.Lib.Win.Error.ErrorShower.OnShowError(null, ex.Message, Environment.StringResources.GetString("InputError"));
                if (closeOnException) 
                    Close();
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (refreshing)
                return;
            if (OpenMode)
                return;

            myRule = null;
            Close();
        }

        #endregion

        private void EditOption(Option option)
        {
            switch (option.Name)
            {
                case "MessageSender":
                case "MessageReceiver":
                case "DocumentSigner":
                case "NoDocumentSigner":
                    EditEmployeeOption((EmployeeOption) option);
                    break;

                case "LinkedDocuments":
                case "NoLinkedDocuments":
                    EditDocumentOption((DocumentOption) option);
                    break;

                case "LinkedPerson":
                case "NoLinkedPerson":
                    EditPersonOption((PersonOption) option);
                    break;

                case "DocumentType":
                    EditDocumentTypeOption((DocumentTypeOption) option);
                    break;

                case "MessageText":
                case "NotMessageText":
                    EditSimpleTextOption(option);
                    break;
            }
        }

        #region EmployeeOption

        public void EditEmployeeOption(EmployeeOption option)
        {
            currentOption = option;
            var frm = new Kesco.Lib.Win.Document.Select.SelectEmployeeDialog(option.GetCaption() + ":");
            frm.DialogEvent += EditEmployeeOption_DialogEvent;
            Enabled = false;
            ShowSubForm(frm);
        }

        private void EditEmployeeOption_DialogEvent(object source, Kesco.Lib.Win.DialogEventArgs e)
        {
            var frm = e.Dialog as Kesco.Lib.Win.Document.Select.SelectEmployeeDialog;
            if (frm.DialogResult == DialogResult.OK)
            {
                ((EmployeeOption) currentOption).Employee =
                    new Employee(frm.Employee.ID);
                RefreshAll();
            }
            Enabled = true;
            Focus();
        }

        #endregion

        #region Document

        public void EditDocumentOption(DocumentOption option)
        {
            currentOption = option;

            var frm = new XmlSearchForm();
            frm.DialogEvent += EditDocumentOption_DialogEvent;

            Enabled = false;
            ShowSubForm(frm);
        }


        private void EditDocumentOption_DialogEvent(object source, Kesco.Lib.Win.DialogEventArgs e)
        {
            var dialog = e.Dialog as XmlSearchForm;

            if (dialog != null && dialog.DialogResult == DialogResult.OK)
            {
                if (Environment.DocData.FoundDocsCount() > 0) // есть найденные документы
                {
                    var uniDialog =
                        new Kesco.Lib.Win.Document.Select.SelectDocUniversalDialog(
                            Environment.DocData.GetFoundDocsIDQuery(Environment.CurEmp.ID), dialog.DocID, null, false);
                    uniDialog.DialogEvent += uniDialog_DialogEvent;
                    ShowSubForm(uniDialog);
                }
                else
                {
                    if (MessageBox.Show(Environment.StringResources.GetString("Search.NotFound.Message1")
                                        + System.Environment.NewLine + System.Environment.NewLine +
                                        Environment.StringResources.GetString("Search.NotFound.Message2"),
                                        Environment.StringResources.GetString("Search.NotFound.Title"),
                                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question,
                                        MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    {
                        EditDocumentOption((DocumentOption) currentOption);
                    }
                    else
                    {
                        Enabled = true;
                        Focus();
                    }
                }
            }
            else
            {
                Enabled = true;
                Focus();
            }
        }

        private void uniDialog_DialogEvent(object source, Kesco.Lib.Win.DialogEventArgs e)
        {
            Kesco.Lib.Win.Document.Select.SelectDocUniversalDialog dialog = e.Dialog as Kesco.Lib.Win.Document.Select.SelectDocUniversalDialog;
            if (dialog != null && dialog.DialogResult == DialogResult.OK)
            {
                ((DocumentOption) currentOption).Document =
                    new Document(dialog.DocID);
                RefreshAll();
            }
            Enabled = true;
            Focus();
        }

        #endregion

        #region DocumentType

        private void EditDocumentTypeOption(DocumentTypeOption option)
        {
            currentOption = option;

            var frm = new Kesco.Lib.Win.Document.Select.SelectTypeDialog(option.Type == null ? 0 : option.Type.ID, false, true)
                    {SubTypesChecked = (option.Filter & 1) > 0, SimilarChecked = (option.Filter & 2) > 0};

			frm.DialogEvent += EditDocumentTypeOption_DialogEvent;

            Enabled = false;
            ShowSubForm(frm);
        }

        private void EditDocumentTypeOption_DialogEvent(object source, Kesco.Lib.Win.DialogEventArgs e)
        {
            var dialog = e.Dialog as Kesco.Lib.Win.Document.Select.SelectTypeDialog;
            if (dialog != null && dialog.DialogResult == DialogResult.OK)
            {
                byte filter = 0;
                if (dialog.SubTypesChecked)
                    filter++;
                if (dialog.SimilarChecked)
                    filter += 2;
                var opt =
                    (DocumentTypeOption) currentOption;
                opt.Type = dialog.TypeID == 0 ? null : new DocumentType(dialog.TypeID);
                opt.Filter = filter;
                RefreshAll();
            }

            Enabled = true;
            Focus();
        }

        #endregion

        #region Person

        private void EditPersonOption(PersonOption option)
        {
            currentOption = option;

            var frm = new Kesco.Lib.Win.Web.PersonDialog(Environment.PersonSearchString, Forms.MainFormDialog.personParamStr);
            frm.DialogEvent += EditPersonOption_DialogEvent;

            Enabled = false;
            ShowSubForm(frm);
        }

        private void EditPersonOption_DialogEvent(object source, Kesco.Lib.Win.DialogEventArgs e)
        {
            Kesco.Lib.Win.Web.PersonDialog frm = e.Dialog as Kesco.Lib.Win.Web.PersonDialog;

			if(frm != null && frm.DialogResult == DialogResult.OK && frm.Persons != null && frm.Persons.Count == 1)
            {
				Kesco.Lib.Win.Web.PersonInfo personInfo = (Kesco.Lib.Win.Web.PersonInfo)frm.Persons[0];
                ((PersonOption) currentOption).Person = new Person(personInfo.ID);
                RefreshAll();
            }

            Enabled = true;
            Focus();
        }

        #endregion

        #region SimpleText

        private void EditSimpleTextOption(Option option)
        {
            if (state != null && state.DocumentElement != null)
            {
                state.DocumentElement.RemoveAll();
                myRule.SaveToXmlElement(state.DocumentElement); //сохран€ем состо€ние на случай отмены
            }
            var frm = new Kesco.Lib.Win.Document.OptionsDialogs.SimpleText((Kesco.Lib.Win.Data.Options.SimpleTextOption) option);
            frm.DialogEvent += EditSimpleTextOption_DialogEvent;

            Enabled = false;
            ShowSubForm(frm);
        }

        private void EditSimpleTextOption_DialogEvent(object source, Kesco.Lib.Win.DialogEventArgs e)
        {
			var frm = (Kesco.Lib.Win.Document.OptionsDialogs.Base)e.Dialog;

            if (frm.DialogResult == DialogResult.OK)
                RefreshAll();
            else
                myRule.LoadFromXmlElement(state.DocumentElement);
            Enabled = true;
            Focus();
        }

        #endregion
    }
}