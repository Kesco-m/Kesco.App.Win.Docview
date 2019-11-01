using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Kesco.App.Win.DocView.Common;

namespace Kesco.App.Win.DocView.Blocks
{
    public class CurPersonBlock : UserControl
    {
        public CurPersonBlock()
        {
            InitializeComponent();
        }

        #region Event

        public event EventHandler PersonChange;

        private void OnPersonChange()
        {
            if (PersonChange != null)
                PersonChange(this, EventArgs.Empty);
        }

        #endregion

        private ComboBox _comboPerson;

        private PictureBox _pictureBox;
        private Button _buttonOk;
        private readonly Container _components;

        /// <summary>
        ///   Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_components != null)
                {
                    _components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        ///   Required method for Designer support - do not modify the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CurPersonBlock));
			this._comboPerson = new System.Windows.Forms.ComboBox();
			this._pictureBox = new System.Windows.Forms.PictureBox();
			this._buttonOk = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this._pictureBox)).BeginInit();
			this.SuspendLayout();
			// 
            // comboPerson
			// 
			resources.ApplyResources(this._comboPerson, "_comboPerson");
			this._comboPerson.Name = "_comboPerson";
			this._comboPerson.TextChanged += new System.EventHandler(this.comboPerson_TextChanged);
			// 
            // pictureBox
			// 
			resources.ApplyResources(this._pictureBox, "_pictureBox");
			this._pictureBox.Name = "_pictureBox";
			this._pictureBox.TabStop = false;
			// 
            // buttonOK
			// 
			resources.ApplyResources(this._buttonOk, "_buttonOk");
			this._buttonOk.Name = "_buttonOk";
			this._buttonOk.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// CurPersonBlock
			// 
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this._buttonOk);
			this.Controls.Add(this._comboPerson);
			this.Controls.Add(this._pictureBox);
			this.DoubleBuffered = true;
			this.Name = "CurPersonBlock";
			this.Load += new System.EventHandler(this.CurPersonBlock_Load);
            ((System.ComponentModel.ISupportInitialize) (this._pictureBox)).EndInit();
			this.ResumeLayout(false);
        }

        #endregion

        private void CurPersonBlock_Load(object sender, EventArgs e)
        {
            FillList();
        }

        private void FillList()
        {
            if (!Environment.IsConnected)
                return;
            try
            {
                using (var dt = Environment.PersonData.GetCurrentPersons())
                {
                    foreach (var per in from DataRow dr in dt.Rows select new Person(dr))
                        _comboPerson.Items.Add(per);
                    dt.Dispose();
                }

                Lib.Win.Document.Environment.PersonID = Environment.UserSettings.PersonID;
				Environment.CompanyName = null;
                for (var i = 0; i < _comboPerson.Items.Count; i++)
                {
                    var per = _comboPerson.Items[i] as Person;
                    if (per != null && per.ID == Environment.UserSettings.PersonID)
                        _comboPerson.SelectedIndex = i;
                }
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        public void RefreshPerson()
        {
            FillList();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            try
            {
                var per = _comboPerson.SelectedItem as Person;
                if (per != null && per.ID != Environment.UserSettings.PersonID)
                {
                    Environment.UserSettings.PersonID = per.ID;
                    Environment.UserSettings.Save();
					Environment.CompanyName = null;
                }
                OnPersonChange();
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        private void comboPerson_TextChanged(object sender, EventArgs e)
        {
            _comboPerson.SelectionLength = 1;
        }
    }
}