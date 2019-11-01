using System.Drawing;

namespace Kesco.App.Win.DocView.Items
{
    public class BurnedListItem : ListItem
    {
        private bool burned;
        private Color oldColor;

        public BurnedListItem(int id, string text, bool burned) : base(id, text)
        {
            Burned = burned;
        }

        public BurnedListItem(int id, string[] values, bool burned) : base(id, values)
        {
            Burned = burned;
        }

        public bool Burned
        {
            get { return burned; }
            set
            {
                burned = value;
                UpdateState();
            }
        }

        public void UpdateState()
        {
            if (burned)
            {
                oldColor = ForeColor;
                ForeColor = Color.Gray;
            }
            else
                ForeColor = oldColor;
        }
    }
}