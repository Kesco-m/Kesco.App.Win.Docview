using System;
using System.Collections;
using System.IO;

namespace Kesco.App.Win.DocView.ImageRead
{
    public class FileInfoComparer : IComparer
    {
        public int Compare(object lhs, object rhs)
        {
            var l = (FileInfo)rhs;
            var r = (FileInfo)lhs;
            int timeCompare = DateTime.Compare(r.CreationTime, l.CreationTime);
            return timeCompare == 0 ? string.Compare(r.FullName, l.FullName) : timeCompare;
        }
    }
}
