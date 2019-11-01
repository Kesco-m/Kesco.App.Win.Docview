using System;
using System.Collections;
using System.Globalization;
using System.Windows.Forms;

namespace Kesco.App.Win.DocView.Misc
{
    internal class ListViewItemComparer : IComparer
    {
        private readonly int _col;
        private readonly SortOrder _sOrd;

        public ListViewItemComparer(SortOrder sOrd)
        {
            _col = 0;
            _sOrd = sOrd;
        }

        public ListViewItemComparer(SortOrder sOrd, int column)
        {
            _col = column;
            _sOrd = sOrd;
        }

        public int Compare(object x, object y)
        {
            return (_sOrd == SortOrder.Ascending ? 1 : -1) *
                   Math.Sign(String.Compare(((ListViewItem)x).SubItems[_col].Text,
                                            ((ListViewItem)y).SubItems[_col].Text, true,
                                            CultureInfo.InvariantCulture));
        }
    }
}
