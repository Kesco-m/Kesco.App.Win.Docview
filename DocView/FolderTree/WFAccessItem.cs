using System.Windows.Forms;

namespace Kesco.App.Win.DocView.FolderTree
{
    public class WFAccessItem : ListViewItem
    {
        public WFAccessItem(int id, int empID, string text, AccessLevel rights) : base(text)
        {
            ID = id;
            EmpID = empID;
            Rights = rights;
        }

        #region Accessors

        public int ID { get; private set; }

        public int EmpID { get; private set; }

        public AccessLevel Rights { get; set; }

        #endregion

        public override string ToString()
        {
            return Text;
        }
    }
}