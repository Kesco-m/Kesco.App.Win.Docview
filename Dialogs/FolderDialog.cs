using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Kesco.App.Win.DocView.Dialogs
{
    public class FolderDialog : FolderNameEditor
    {
        public string Path { get; private set; }

        public DialogResult ShowDialog()
        {
            var folderBrowser = new FolderBrowser {Description = Environment.StringResources.GetString("FolderDialog")};
			folderBrowser.Style = FolderBrowserStyles.ShowTextBox;
            var result = folderBrowser.ShowDialog();
            Path = folderBrowser.DirectoryPath;

            return result;
        }
    }
}