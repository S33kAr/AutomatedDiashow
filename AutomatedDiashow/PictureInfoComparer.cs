using System;
using System.Collections.Generic;

namespace AutomatedDiashow
{
    public class PictureInfoComparer : IComparer<PictureInfo>
    {
        public int Compare(PictureInfo a, PictureInfo b)
        {
            int val = a.TimesShown - b.TimesShown;
            if (val != 0)
            {
                return val;
            }
            else if (DateTime.Compare(a.CreationDate, b.CreationDate) != 0)
            {
                return DateTime.Compare(a.CreationDate, b.CreationDate);
            }
            else
            {
                return 1;
            }
        }
    }
}
