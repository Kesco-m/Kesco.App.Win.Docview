using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Kesco.Lib.Win;

namespace Kesco.App.Win.DocView.Dialogs
{
	public class ChangesDialog : FreeDialog
	{
		private readonly string _defUrl;
		private readonly string _changesUrl;

		private CheckBox _checkShowNext;
		private Panel _panelBrowser;
		private Button _buttonOk;
		private Label _version;
		private WebBrowser _webBrowser;
		private Button buttonCancel;

		private Container _components;

		public ChangesDialog()
		{
			InitializeComponent();
			_defUrl = Environment.ShowHelpString;
			_changesUrl = Environment.ShowHelpFirstTimeString;
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(_components != null)
				{
					_components.Dispose();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChangesDialog));
			this._checkShowNext = new System.Windows.Forms.CheckBox();
			this._panelBrowser = new System.Windows.Forms.Panel();
			this._webBrowser = new System.Windows.Forms.WebBrowser();
			this._buttonOk = new System.Windows.Forms.Button();
			this._version = new System.Windows.Forms.Label();
			this.buttonCancel = new System.Windows.Forms.Button();
			this._panelBrowser.SuspendLayout();
			this.SuspendLayout();
			// 
			// _checkShowNext
			// 
			resources.ApplyResources(this._checkShowNext, "_checkShowNext");
			this._checkShowNext.Checked = true;
			this._checkShowNext.CheckState = System.Windows.Forms.CheckState.Checked;
			this._checkShowNext.Name = "_checkShowNext";
			this._checkShowNext.CheckedChanged += new System.EventHandler(this.checkShowNext_CheckedChanged);
			// 
			// _panelBrowser
			// 
			this._panelBrowser.Controls.Add(this._webBrowser);
			resources.ApplyResources(this._panelBrowser, "_panelBrowser");
			this._panelBrowser.Name = "_panelBrowser";
			// 
			// _webBrowser
			// 
			this._webBrowser.AllowWebBrowserDrop = false;
			resources.ApplyResources(this._webBrowser, "_webBrowser");
			this._webBrowser.IsWebBrowserContextMenuEnabled = false;
			this._webBrowser.Name = "_webBrowser";
			this._webBrowser.WebBrowserShortcutsEnabled = false;
			// 
			// _buttonOk
			// 
			this._buttonOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this._buttonOk, "_buttonOk");
			this._buttonOk.Name = "_buttonOk";
			this._buttonOk.Click += new System.EventHandler(this.button_Click);
			// 
			// _version
			// 
			resources.ApplyResources(this._version, "_version");
			this._version.Name = "_version";
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Click += new System.EventHandler(this.button_Click);
			// 
			// ChangesDialog
			// 
			this.AcceptButton = this._buttonOk;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this._version);
			this.Controls.Add(this._buttonOk);
			this.Controls.Add(this._panelBrowser);
			this.Controls.Add(this._checkShowNext);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ChangesDialog";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.ChangesDialog_Closing);
			this.Load += new System.EventHandler(this.ChangesDialog_Load);
			this._panelBrowser.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void ChangesDialog_Load(object sender, EventArgs e)
		{
			if(string.IsNullOrEmpty(_defUrl) && string.IsNullOrEmpty(_changesUrl))
				return;
			_checkShowNext.CheckedChanged -= checkShowNext_CheckedChanged;
			_checkShowNext.Checked = Environment.UserSettings.ShowNews;
			_checkShowNext.CheckedChanged += checkShowNext_CheckedChanged;

			_version.Text += Application.ProductVersion + @" FW " + System.Environment.Version.Major.ToString() + (IntPtr.Size == 8 ? " x64" : " x86");
			string url = _defUrl;
			if(Changes)
				url = _changesUrl;
			else
				Text = Environment.StringResources.GetString("Help");

			_webBrowser.DocumentTitleChanged += webBrowser_DocumentTitleChanged;
			_webBrowser.Url = new Uri(url);
		}

		private void webBrowser_DocumentTitleChanged(object sender, EventArgs e)
		{
			Text = _webBrowser.DocumentTitle;
		}

		private void checkShowNext_CheckedChanged(object sender, EventArgs e)
		{
			if(Environment.IsConnected)
			{
				Environment.UserSettings.ShowNews = _checkShowNext.Checked;
				Environment.UserSettings.Save();
			}
		}

		private void ChangesDialog_Closing(object sender, CancelEventArgs e)
		{
			Forms.MainFormDialog.changesDialog = null;
		}

		void button_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		public bool Changes { get; set; }
	}
}