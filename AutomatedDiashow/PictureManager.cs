
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
namespace AutomatedDiashow
{
    class PictureManager
    {
        public delegate void PictureChangedEventHandler(string newPath, Color color);
        public event PictureChangedEventHandler PictureChanged;

        private LockedPictureList Pictures;
        private Object PicturesLock = new Object();
        private Queue<PictureInfo> Commercials;

        private Config Config;

        private bool isRunning;
        private Object runningLock = new Object();

        private int currentPosition;

        private LimitedList<PictureInfo> PictureHistory;

        public PictureManager(Config config)
        {
            this.Config = config;
            lock (PicturesLock)
            {
                this.Pictures = new LockedPictureList(new PictureInfoComparer(), this.LoadExistingPictures(config.ImagesFolder, config.FileExtension));
            }

            this.Commercials = new Queue<PictureInfo>(this.LoadExistingPictures(config.CommercialsFolder, config.FileExtension));

            this.PictureHistory = new LimitedList<PictureInfo>(this.Config.MinimumImageCycle - 1);

            FileSystemWatcher imagesWatcher = new FileSystemWatcher(config.ImagesFolder, config.FileExtension);

            imagesWatcher.IncludeSubdirectories = true;
            imagesWatcher.Created += new FileSystemEventHandler(this.OnImageAdded);
            imagesWatcher.Deleted += new FileSystemEventHandler(this.OnImageRemoved);

            imagesWatcher.EnableRaisingEvents = true;
        }

        private void OnImageAdded(object source, FileSystemEventArgs e)
        {
            lock (PicturesLock)
            {
                string file = e.FullPath;
                Console.WriteLine("ImageAdded: " + file);

                this.Pictures.SortedInsert(
                    new PictureInfo()
                    {
                        FilePath = file.Substring(this.Config.ImagesFolder.Length + 1),
                        TimesShown = 0,
                        CreationDate = File.GetLastWriteTime(file)
                    }
                );
            }
        }

        private void OnImageRemoved(object source, FileSystemEventArgs e)
        {
            lock (PicturesLock)
            {
                Console.WriteLine("ImageRemoved: " + e.FullPath);

                this.Pictures.Remove(i => Path.Combine(this.Config.ImagesFolder, i.FilePath) == e.FullPath);
            }
        }

        private List<PictureInfo> LoadExistingPictures(string path, string searchPattern, string rootFolder = null)
        {
            if (rootFolder == null)
            {
                rootFolder = path;
            }
            List<PictureInfo> pictures = new List<PictureInfo>();

            foreach (string file in Directory.GetFiles(path, searchPattern))
            {
                pictures.Add(
                    new PictureInfo()
                    {
                        FilePath = file.Substring(rootFolder.Length + 1),
                        TimesShown = 0,
                        CreationDate = File.GetLastWriteTime(file)
                    }
                );

                foreach (string dir in Directory.GetDirectories(path))
                {
                    pictures.AddRange(this.LoadExistingPictures(dir, searchPattern, rootFolder));
                }
            }

            return pictures;
        }

        public bool SwitchToNextPicture()
        {
            if (this.PictureHistory.List.Count >= this.Pictures.Count)
            {
                this.PictureHistory = new LimitedList<PictureInfo>(this.Config.MinimumImageCycle - 1);
            }

            PictureInfo next = this.Pictures.GetFirstExcept(this.PictureHistory.List);
            if (next != null)
            {
                this.PictureChanged(Path.Combine(this.Config.ImagesFolder, next.FilePath), Color.Black);
                next.TimesShown++;

                this.PictureHistory.Add(next);

                return true;
            }
            return false;
        }

        public void Run()
        {
            lock (this.runningLock)
            {
                if (!this.isRunning)
                {
                    this.isRunning = true;
                    ThreadPool.QueueUserWorkItem(o => this.WorkerThread());
                }
            }
        }

        public void Stop()
        {
            lock (this.runningLock)
            {
                if (this.isRunning)
                {
                    this.isRunning = false;
                }
            }
        }

        private void WorkerThread()
        {
            while (this.isRunning)
            {
                if (currentPosition < this.Config.ImagesCount)
                {
                    if (this.SwitchToNextPicture())
                    {
                        System.Threading.Thread.Sleep(this.Config.ImageDuration);
                    }
                }
                else if (currentPosition < this.Config.ImagesCount + this.Config.CommercialsCount)
                {
                    if (this.Commercials.Count > 0)
                    {
                        PictureInfo next = this.Commercials.Dequeue();
                        this.Commercials.Enqueue(next);
                        this.PictureChanged(Path.Combine(this.Config.CommercialsFolder, next.FilePath), Color.White);
                        System.Threading.Thread.Sleep(this.Config.CommercialsDuration);
                    }
                }
                else
                {
                    this.currentPosition = -1;
                }

                this.currentPosition++;
            }
        }

        public void ToggleRunning()
        {
            lock (this.runningLock)
            {
                if (this.isRunning)
                {
                    this.Stop();
                }
                else
                {
                    this.Run();
                }
            }
        }
    }
}
