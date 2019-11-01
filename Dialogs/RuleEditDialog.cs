using System;
using System.Reflection;
using System.Resources;
using Kesco.Lib.Win;
using Kesco.Lib.Win.Document;
using Kesco.Lib.Win.Document.Blocks;

namespace Kesco.App.Win.DocView.Dialogs
{
    public partial class RuleEditDialog : FreeDialog
    {
        private ResourceManager resMngr;

        private int? _userId;
        private int? _docTypeId;
        private int? _organizationID;

        public int UserId
        {
            get { return (_userId == null) ? -1 : _userId.Value; }
        }

        public string UserName { get; private set; }
        public int DocTypeId { get; private set; }
        private string docTypeName = "";

        public string DocTypeName
        {
            get { return (DocTypeId > 0 ? docTypeName : resMngr.GetString("DocTypeAllchkBox.Text")); }
        }

        public int OrganizationID { get; private set; }
        private string organizationName = "";

        public string OrganizationName
        {
            get { return (OrganizationID > 0 ? organizationName : resMngr.GetString("PersonAllchkBox.Text")); }
        }

        /// <summary>
        ///   Конструктор формы права на штамп - создание
        /// </summary>
        public RuleEditDialog()
        {
            InitializeComponent();
			resMngr = new ResourceManager("Kesco.App.Win.DocView.Dialogs.RuleEditDialog", Assembly.GetExecutingAssembly());
            docTypeName = resMngr.GetString("DocTypeAllchkBox.Text");
            organizationName = resMngr.GetString("PersonAllchkBox.Text");

            employeeBlock.ClearAfterSelect = false;
            employeeBlock.ButtonText = Environment.StringResources.GetString("Kesco.App.Win.DocView.Dialogs.Rules.Select");
            employeeBlock.Parser = new Lib.Win.Document.Blocks.Parsers.EmployeeParser(Environment.EmpData, false);
            
            DocTypeAllchkBox.CheckedChanged += DocTypeAllchkBox_CheckedChanged;
            PersonAllchkBox.CheckedChanged += PersonAllchkBox_CheckedChanged;

            employeeBlock.EmployerTextChanged += employeeBlock_EmployerTextChanged;
            employeeBlock.EmployeeSelected += employeeBlock_EmployeeSelected;
            employeeBlock.ButtonTextChanged += employeeBlock_ButtonTextChanged;
            docTypeBlock.DocTypeTextChanged += docTypeBlock_DocTypeTextChanged;
            docTypeBlock.Selected += docTypeBlock_Selected;
            personSearchBlock.PersonTextChanged += personSearchBlock_PersonTextChanged;
            personSearchBlock.FindPerson += personSearchBlock_FindPerson;
        }

        /// <summary>
        ///   Конструктор формы права на штамп - редактирование
        /// </summary>
        public RuleEditDialog(int userId, string userName, int docTypeID, string dDocTypeName, int orgID, string orgName) : this()
        {
            employeeBlock.FullText = userName;
            _userId = userId;
            UserName = userName;

            DocTypeAllchkBox.Checked = docTypeID <= 0;
            _docTypeId = DocTypeId = docTypeID > 0 ? docTypeID : -1;
            docTypeBlock.Text = docTypeID > 0 ? dDocTypeName : "";
            docTypeName = docTypeID > 0 ? dDocTypeName : (docTypeID == 0 ? "" : resMngr.GetString("DocTypeAllchkBox.Text"));

            PersonAllchkBox.Checked = (orgID <= 0);
            _organizationID = OrganizationID = (orgID > 0 ? orgID : -1);
            personSearchBlock.FullText = (orgID > 0 ? orgName : "");
            organizationName = orgID > 0 ? orgName : (orgID == 0 ? "" : resMngr.GetString("PersonAllchkBox.Text"));

            btnOK.Enabled = false;
        }

        private void employeeBlock_EmployeeSelected(object source, EmployeeBlockEventArgs e)
        {
            if (e.Emps != null && e.Emps.Length > 0)
            {
                _userId = e.Emps[0].ID;

                if (Environment.CurCultureInfo.TwoLetterISOLanguageName.Equals("ru"))
                    UserName = e.Emps[0].ShortName;
                else
                    UserName = e.Emps[0].ShortEngName;

                employeeBlock.FullText = UserName;

                btnOK.Enabled = _userId != null && _docTypeId != null && _organizationID != null;
            }
        }

        private void docTypeBlock_Selected(object source, BlockEventArgs e)
        {
            _docTypeId = DocTypeId = e.ID;
            docTypeName = e.Name;

            btnOK.Enabled = _userId != null && _docTypeId != null && _organizationID != null;

            if (btnOK.Enabled)
                btnOK.Focus();
            else
                PersonAllchkBox.Focus();
        }

        private void personSearchBlock_FindPerson(object sender, EventArgs e)
        {
            _organizationID = OrganizationID = personSearchBlock.PersonID;
            organizationName = personSearchBlock.PersonName;

            btnOK.Enabled = _userId != null && _docTypeId != null && _organizationID != null;
        }

        private void employeeBlock_ButtonTextChanged(object sender, EventArgs e)
        {
            employeeBlock.ButtonTextChanged -= employeeBlock_ButtonTextChanged;
            employeeBlock.ButtonText = Environment.StringResources.GetString("Kesco.App.Win.DocView.Dialogs.Rules.Select");
            employeeBlock.ButtonTextChanged += employeeBlock_ButtonTextChanged;
        }

        private void employeeBlock_EmployerTextChanged(object sender, EventArgs e)
        {
            _userId = null;
            UserName = "";
            btnOK.Enabled = false;
        }

        private void docTypeBlock_DocTypeTextChanged(object sender, EventArgs e)
        {
            _docTypeId = null;
            DocTypeId = 0;
            docTypeName = "";
            btnOK.Enabled = false;
        }

        private void personSearchBlock_PersonTextChanged(object sender, EventArgs e)
        {
            _organizationID = null;
            OrganizationID = 0;
            organizationName = "";
            btnOK.Enabled = false;
        }

        private void DocTypeAllchkBox_CheckedChanged(object sender, EventArgs e)
        {
            docTypeBlock.Enabled = !DocTypeAllchkBox.Checked;
            if (DocTypeAllchkBox.Checked)
            {
                _docTypeId = DocTypeId = -1;

                docTypeBlock.DocTypeTextChanged -= docTypeBlock_DocTypeTextChanged;
                docTypeBlock.Text = "";
                docTypeBlock.DocTypeTextChanged += docTypeBlock_DocTypeTextChanged;
            }
            else
            {
                DocTypeId = 0;
                _docTypeId = null;

                docTypeBlock.Focus();
            }

            btnOK.Enabled = _userId != null && _docTypeId != null && _organizationID != null;
        }

        private void PersonAllchkBox_CheckedChanged(object sender, EventArgs e)
        {
            personSearchBlock.Enabled = !PersonAllchkBox.Checked;
            if (PersonAllchkBox.Checked)
            {
                _organizationID = OrganizationID = -1;

                personSearchBlock.PersonTextChanged -= personSearchBlock_PersonTextChanged;
                personSearchBlock.FullText = "";
                personSearchBlock.PersonTextChanged += personSearchBlock_PersonTextChanged;
            }
            else
            {
                OrganizationID = 0;
                _organizationID = null;

                personSearchBlock.Focus();
            }

            btnOK.Enabled = _userId != null && _docTypeId != null && _organizationID != null;
        }
    }
}