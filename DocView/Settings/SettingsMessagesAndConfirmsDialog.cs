using System;
using System.Windows.Forms;

namespace Kesco.App.Win.DocView.Settings
{
	public partial class SettingsMessagesAndConfirmsDialog : Lib.Win.FreeDialog
	{
		private double minTimeout;
		private double maxTimeout;
		private const int minFolderTimeout = 1;
		private const int maxFolderTimeout = 120;
		private double oldTimeout;
		private bool barState;

		public SettingsMessagesAndConfirmsDialog()
		{
			InitializeComponent();
			minTimeout = -1 * 0.001;
			maxTimeout = 10;
			oldTimeout = Environment.UserSettings.ReadTimeout * 0.001;
		}

		private void SettingsMessagesDialog_Load(object sender, EventArgs e)
		{
			Environment.UserSettings.Reload();
			if(oldTimeout > 0)
			{
				checkTimer.Checked = true;
				timeout.Text = oldTimeout.ToString().Replace(",", ".");
			}
			else
			{
				checkTimer.Checked = false;
				timeout.Text = "5";
				timeout.Enabled = false;
			}
			checkNewMessageNotification.Checked = Environment.UserSettings.NotifyMessage;
			checkDelete.Checked = Environment.UserSettings.DeleteConfirm;
			checkBoxPersonMessage.Checked = Environment.PersonMessage;
			checkBoxScan.Checked = Environment.UserSettings.GotoDocument;
			barState = Environment.General.LoadStringOption("CatchScan", false.ToString()).Equals(true.ToString());
			checkBoxBar.Checked = barState;
			checkBoxNext.Checked = Environment.UserSettings.GotoNext;
			checkBoxSignMessage.Checked = Environment.UserSettings.MessageOnEndSign;

			switch(Environment.UserSettings.ReadMessageOnEndWork)
			{
				case 0:
					rbReadOnEndAlways.Checked = true;
					break;
				case 1:
					rbReadOnEndNever.Checked = true;
					break;
				default:
					rbReadOnEndAskMe.Checked = true;
					break;
			}
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			try
			{
				if(checkTimer.Checked)
				{
					double newT = Convert.ToDouble(timeout.Text.Trim().Replace(".", ","));

					if(newT != oldTimeout)
					{
						if(newT < minTimeout || newT > maxTimeout)
						{
							MessageBox.Show(Environment.StringResources.GetString("SettingsMessagesAndConfirmsDialog.buttonOK_Click.Message1") +
								minTimeout.ToString() +
								Environment.StringResources.GetString("SettingsMessagesAndConfirmsDialog.buttonOK_Click.Message2") +
								maxTimeout.ToString());
							return;
						}
						Environment.UserSettings.ReadTimeout = Convert.ToInt32(newT * 1000);
					}
				}
				else
					Environment.UserSettings.ReadTimeout = 0;
			}
			catch
			{
				MessageBox.Show(Environment.StringResources.GetString("SettingsMessagesAndConfirmsDialog.buttonOK_Click.Message3"));
				return;
			}

			bool currentState = checkBoxBar.Checked;
			Environment.General.Option("CatchScan").Value = currentState.ToString();
			Environment.General.Save();

			Environment.UserSettings.GotoDocument = checkBoxScan.Checked;
			Environment.UserSettings.GotoNext = checkBoxNext.Checked;
			Environment.UserSettings.NotifyMessage = checkNewMessageNotification.Checked;
			Environment.UserSettings.DeleteConfirm = checkDelete.Checked;
			Environment.PersonMessage = checkBoxPersonMessage.Checked;
			Environment.UserSettings.MessageOnEndSign = checkBoxSignMessage.Checked;
			Environment.UserSettings.ReadMessageOnEndWork = rbReadOnEndAlways.Checked ? (byte)0 : rbReadOnEndNever.Checked ? (byte)1 : (byte)2;

			if(barState != currentState)
			{
				End(DialogResult.Yes);
				return;
			}
			End(DialogResult.OK);
		}

		private void checkTimer_CheckedChanged(object sender, EventArgs e)
		{
			bool enable = checkTimer.Checked;
			timeout.Enabled = enable;
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			End(DialogResult.Cancel);
		}

		private void checkBoxBar_CheckedChanged(object sender, EventArgs e)
		{
			checkBoxScan.Enabled = checkBoxBar.Checked;
		}

	}
}