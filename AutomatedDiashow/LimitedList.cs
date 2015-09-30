using System;
using System.Collections.Generic;

namespace AutomatedDiashow
{
    class LimitedList<T>
    {
        public List<T> List;
        private int maxSize;

        public LimitedList(int maxSize)
        {
            if (maxSize < 1)
            {
                throw new ArgumentOutOfRangeException("maxSize has to be greater than 0.");
            }

            this.List = new List<T>();
            this.maxSize = maxSize;
        }

        public void Add(T item)
        {
            if (this.List.Count == this.maxSize)
            {
                this.List.RemoveAt(0);
            }

            this.List.Add(item);
        }
    }
}
