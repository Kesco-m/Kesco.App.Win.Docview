using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using Kesco.Lib.Win;
using Kesco.Lib.Win.Data.Temp.Objects;

namespace Kesco.App.Win.DocView.PropertiesDialogs
{
    public class PropertiesSharedWorkFolderDialog : FreeDialog
    {
        private Label labelCreator;
        private Label label2;
        private Button buttonOK;
        private ListView listViewAllowed;

        private int id;

        private Container components;

        public PropertiesSharedWorkFolderDialog(int id, string name)
        {
            InitializeComponent();

            listViewAllowed.Columns.Add(Environment.StringResources.GetString("Employee"), 200, HorizontalAlignment.Left);
            listViewAllowed.Columns.Add(Environment.StringResources.GetString("Access"), 100, HorizontalAlignment.Left);

            this.id = id;
            Text += " \"" + name + "\"";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///   Required method for Designer support - do not modify the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            var resources =
                new System.ComponentModel.ComponentResourceManager(typeof (PropertiesSharedWorkFolderDialog));
            this.labelCreator = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.listViewAllowed = new System.Windows.Forms.ListView();
            this.buttonOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelCreator
            // 
            resources.ApplyResources(this.labelCreator, "labelCreator");
            this.labelCreator.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.labelCreator.Name = "labelCreator";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label2.Name = "label2";
            // 
            // listViewAllowed
            // 
            resources.ApplyResources(this.listViewAllowed, "listViewAllowed");
            this.listViewAllowed.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.listViewAllowed.FullRowSelect = true;
            this.listViewAllowed.MultiSelect = false;
            this.listViewAllowed.Name = "listViewAllowed";
            this.listViewAllowed.UseCompatibleStateImageBehavior = false;
            this.listViewAllowed.View = System.Windows.Forms.View.Details;
            // 
            // buttonOK
            // 
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // PropertiesSharedWorkFolderDialog
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonOK;
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.listViewAllowed);
            this.Controls.Add(this.labelCreator);
            this.Controls.Add(this.label2);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PropertiesSharedWorkFolderDialog";

            this.Load += new System.EventHandler(this.PropertiesSharedWorkFolderDialog_Load);
            this.ResumeLayout(false);
        }

        #endregion

        private void PropertiesSharedWorkFolderDialog_Load(object sender, EventArgs e)
        {
            var empID = Environment.SharedFolderData.GetOwnerID(id);
            var emp = new Employee(empID, Environment.EmpData);
            labelCreator.Text += emp.LongName;

            using (DataTable dt = Environment.SharedFolderData.GetClients(id))
            using (DataTableReader dr = dt.CreateDataReader())
            {
                while (dr.Read())
                {
                    empID = (int) dr[Environment.SharedFolderData.ClientIDField];
                    emp = new Employee(empID, Environment.EmpData);

                    listViewAllowed.Items.Add(new ListViewItem(new string[2]
                                                                   {
                                                                       emp.LongName,
                                                                       Environment.SharedFolderData.Rights(id, empID)
                                                                           ? Environment.StringResources.GetString(
                                                                               "Full")
                                                                           : Environment.StringResources.GetString(
                                                                               "ReadOnly")
                                                                   }
                                                  ));
                }
                dr.Close();
                dr.Dispose();
                dt.Dispose();
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            End(DialogResult.OK);
        }
    }
}