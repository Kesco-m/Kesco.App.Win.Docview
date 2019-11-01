using System;
using Kesco.Lib.Win;

namespace Kesco.App.Win.DocView.Settings
{
    public partial class SettingsLinkShowDialog : FreeDialog
    {
        public SettingsLinkShowDialog()
        {
            InitializeComponent();
        }

        private void SettingsLinkShowDialog_Load(object sender, EventArgs e)
        {
            comboBoxType.SelectedIndex = Lib.Win.Document.Environment.Settings.LoadIntOption("LinkMode", 1);
            comboBoxDocs.SelectedIndex = Lib.Win.Document.Environment.Settings.LoadIntOption("LinkSort", 2);
            textBoxCount.Text = Lib.Win.Document.Environment.Settings.LoadIntOption("OldLinkCount", 15).ToString();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            int count = 0;
            Lib.Win.Document.Environment.Settings.Option("LinkMode").Value = comboBoxType.SelectedIndex;
            Lib.Win.Document.Environment.Settings.Option("LinkSort").Value = comboBoxDocs.SelectedIndex;
            if (int.TryParse(textBoxCount.Text.Trim(), out count) && count > -1)
                Lib.Win.Document.Environment.Settings.Option("OldLinkCount").Value = count;
            Lib.Win.Document.Environment.Settings.Save();
            Close();
        }
    }
}