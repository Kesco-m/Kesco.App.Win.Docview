using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Kesco.App.Win.DocView.Settings
{
	public partial class SettingsAdditionDialog : Lib.Win.FreeDialog
	{
		public SettingsAdditionDialog()
		{
			InitializeComponent();
		}

		private void SettingsAdditionDiaolg_Load(object sender, EventArgs e)
		{
			checkBoxSeachReload.Checked =  numericUpDownMinutes.Enabled = Environment.UserSettings.FolderUpdateTime > 0;
			if(numericUpDownMinutes.Enabled)
				numericUpDownMinutes.Value = Environment.UserSettings.FolderUpdateTime;
			else
				numericUpDownMinutes.Value = 15;
		}

		private void SettingsAdditionDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			if(checkBoxSeachReload.Checked && (Environment.UserSettings.FolderUpdateTime != (int)numericUpDownMinutes.Value))
			{
				Environment.UserSettings.FolderUpdateTime = (int)numericUpDownMinutes.Value;
				Environment.UserSettings.Save();
			}
			if(!checkBoxSeachReload.Checked && Environment.UserSettings.FolderUpdateTime > 0)
			{
				Environment.UserSettings.FolderUpdateTime = 0;
				Environment.UserSettings.Save();
			}
		}
	}
}
