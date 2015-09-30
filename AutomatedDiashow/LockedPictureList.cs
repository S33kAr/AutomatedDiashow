using System;
using System.Collections.Generic;

namespace AutomatedDiashow
{
    public class LockedPictureList
    {
        private object Lock = new object();

        private List<PictureInfo> list;
        private IComparer<PictureInfo> comparer;

        public LockedPictureList(IComparer<PictureInfo> comparer, List<PictureInfo> list = null)
        {
            this.comparer = comparer;
            this.list = list != null ? list : new List<PictureInfo>();
            this.list.Sort(this.comparer);
            foreach (PictureInfo picture in this.list)
            {
                picture.PictureUpdated += this.PictureUpdated;
            }
        }

        public int Count
        {
            get
            {
                return this.list.Count;
            }
        }

        public void SortedInsert(PictureInfo picture)
        {
            lock (this.Lock)
            {
                int index = this.list.BinarySearch(picture, this.comparer);
                if (index < 0)
                {
                    this.list.Insert(~index, picture);
                    picture.PictureUpdated += this.PictureUpdated;
                }
            }
        }

        public void Remove(Predicate<PictureInfo> match)
        {
            lock (this.Lock)
            {
                List<PictureInfo> toRemove = this.list.FindAll(match);

                foreach (PictureInfo picture in toRemove)
                {
                    this.list.Remove(picture);
                    picture.PictureUpdated -= this.PictureUpdated;
                }
            }
        }

        public PictureInfo GetFirstExcept(List<PictureInfo> except)
        {
            PictureInfo first = null;

            lock (this.Lock)
            {
                if (this.list.Count > 0)
                {
                    first = this.list.Find(p => !except.Contains(p));

                    if (first == null)
                    {
                        first = this.list[0];
                    }
                }
            }

            return first;
        }

        public void PictureUpdated(PictureInfo picture)
        {
            lock (this.Lock)
            {
                this.list.Sort(this.comparer);
            }
        }
    }
}
