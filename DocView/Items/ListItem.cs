using System.Windows.Forms;

namespace Kesco.App.Win.DocView.Items
{
    public class ListItem : ListViewItem
    {
        public ListItem(int id, string text) : base(text)
        {
            ID = id;
        }

        public ListItem(int id, string[] values) : base(values)
        {
            ID = id;
        }

        public int ID { get; private set; }
        
        public override string ToString()
        {
            return Text;
        }
    }
}