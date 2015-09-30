
using System;
namespace AutomatedDiashow
{
    public class PictureInfo
    {
        public delegate void PictureUpdatedEventHandler(PictureInfo picture);
        public event PictureUpdatedEventHandler PictureUpdated;

        public string FilePath { get; set; }
        private int timesShown;
        public int TimesShown
        {
            get
            {
                return this.timesShown;
            }
            set
            {
                this.timesShown = value;
                if (value != 0 && this.PictureUpdated != null)
                {
                    this.PictureUpdated(this);
                }
            }
        }
        public DateTime CreationDate { get; set; }
    }
}
