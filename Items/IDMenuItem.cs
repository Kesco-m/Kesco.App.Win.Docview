using System.Windows.Forms;

namespace Kesco.App.Win.DocView.Items
{
    public class IDMenuItem : MenuItem
    {
        public IDMenuItem(int id)
        {
            ID = id;
        }

        public int ID { get; private set; }
    }
}