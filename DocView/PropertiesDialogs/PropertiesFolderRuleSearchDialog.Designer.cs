namespace Kesco.App.Win.DocView.PropertiesDialogs
{
	/// <summary>
	/// Summary description for PropertiesFolderRuleSearchDialog.
	/// </summary>
	public partial class PropertiesFolderRuleSearchDialog : Lib.Win.FreeDialog
	{
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertiesFolderRuleSearchDialog));
			this.label1 = new System.Windows.Forms.Label();
			this.ParamLv = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.SaveRuleButton = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.RuleNameTextBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.InMessRadButton = new System.Windows.Forms.RadioButton();
			this.OutMessRadButton = new System.Windows.Forms.RadioButton();
			this.optionsSettings = new Kesco.Lib.Win.Document.Controls.OptionsSettings();
			this.SuspendLayout();
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// ParamLv
			// 
			resources.ApplyResources(this.ParamLv, "ParamLv");
			this.ParamLv.CheckBoxes = true;
			this.ParamLv.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
			this.ParamLv.FullRowSelect = true;
			this.ParamLv.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.ParamLv.Name = "ParamLv";
			this.ParamLv.UseCompatibleStateImageBehavior = false;
			this.ParamLv.View = System.Windows.Forms.View.Details;
			this.ParamLv.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.ParamLv_ItemCheck);
			// 
			// columnHeader1
			// 
			resources.ApplyResources(this.columnHeader1, "columnHeader1");
			// 
			// SaveRuleButton
			// 
			resources.ApplyResources(this.SaveRuleButton, "SaveRuleButton");
			this.SaveRuleButton.Name = "SaveRuleButton";
			this.SaveRuleButton.Click += new System.EventHandler(this.SaveRuleButton_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// RuleNameTextBox
			// 
			resources.ApplyResources(this.RuleNameTextBox, "RuleNameTextBox");
			this.RuleNameTextBox.Name = "RuleNameTextBox";
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			// 
			// InMessRadButton
			// 
			resources.ApplyResources(this.InMessRadButton, "InMessRadButton");
			this.InMessRadButton.Name = "InMessRadButton";
			this.InMessRadButton.CheckedChanged += new System.EventHandler(this.InMessRadButton_CheckedChanged);
			// 
			// OutMessRadButton
			// 
			resources.ApplyResources(this.OutMessRadButton, "OutMessRadButton");
			this.OutMessRadButton.Name = "OutMessRadButton";
			this.OutMessRadButton.CheckedChanged += new System.EventHandler(this.OutMessRadButton_CheckedChanged);
			// 
			// optionsSettings
			// 
			resources.ApplyResources(this.optionsSettings, "optionsSettings");
			this.optionsSettings.Name = "optionsSettings";
			// 
			// PropertiesFolderRuleSearchDialog
			// 
			this.AcceptButton = this.SaveRuleButton;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.optionsSettings);
			this.Controls.Add(this.ParamLv);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.RuleNameTextBox);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.SaveRuleButton);
			this.Controls.Add(this.InMessRadButton);
			this.Controls.Add(this.OutMessRadButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PropertiesFolderRuleSearchDialog";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.PropertiesFolderRuleSearchDialog_Closing);
			this.Load += new System.EventHandler(this.PropertiesFolderRuleSearchDialog_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.ListView ParamLv;
		private System.Windows.Forms.TextBox RuleNameTextBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button SaveRuleButton;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.RadioButton InMessRadButton;
		private System.Windows.Forms.RadioButton OutMessRadButton;
		private Kesco.Lib.Win.Document.Controls.OptionsSettings optionsSettings;
	}
}