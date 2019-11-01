using System;
using Kesco.Lib.Win;
using Kesco.Lib.Win.Data.Temp.Objects;
using Kesco.Lib.Win.Document.Blocks;
using EmployeeParser = Kesco.Lib.Win.Document.Blocks.Parsers.EmployeeParser;

namespace Kesco.App.Win.DocView.Dialogs
{
    public partial class RightEditDialog : FreeDialog
    {
        private Employee _employee;

        public int UserId
        {
            get { return _employee != null ? _employee.ID : -1; }
        }

        public string UserName
        {
            get { return _employee != null ? _employee.ShortName : string.Empty; }
        }

        public bool EnableProxies
        {
            get { return checkBoxEnapleProxies.Checked; }
        }

        /// <summary>
        ///   Конструктор формы права на штамп - создание
        /// </summary>
        public RightEditDialog()
        {
            InitializeComponent();
            employeeBlock.Parser = new EmployeeParser(Environment.EmpData, false);
        }

        /// <summary>
        ///   Конструктор формы права на штамп - редактирование
        /// </summary>
        public RightEditDialog(string userName, bool enableProxies)
        {
            InitializeComponent();
            employeeBlock.Enabled = false;
            textBoxEmployee.Text = userName;

            checkBoxEnapleProxies.CheckedChanged -= checkBoxEnapleProxies_CheckedChanged;
            checkBoxEnapleProxies.Checked = enableProxies;
            checkBoxEnapleProxies.CheckedChanged += checkBoxEnapleProxies_CheckedChanged;
        }

        private void employeeBlock_EmployeeSelected(object source, EmployeeBlockEventArgs e)
        {
            if (e.Emps == null || e.Emps.Length <= 0)
                return;
            _employee = e.Emps[0];
            textBoxEmployee.Text = _employee.ShortName;
            btnOK.Enabled = true;
        }

        private void checkBoxEnapleProxies_CheckedChanged(object sender, EventArgs e)
        {
            // проверка на то, что это пользователь нажал, а не из конструктора вызвалось
            if (checkBoxEnapleProxies.Focused)
                btnOK.Enabled = textBoxEmployee.Text.Length > 0;
        }
    }
}