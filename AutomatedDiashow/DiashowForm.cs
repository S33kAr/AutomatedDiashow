using System;
using System.Drawing;
using System.Windows.Forms;

namespace AutomatedDiashow
{
    public partial class DiashowForm : Form
    {
        public delegate void NextPictureEventHandler();
        public event NextPictureEventHandler NextPicture;
        public delegate void ToggleRunningEventHandler();
        public event ToggleRunningEventHandler ToggleRunning;

        private FullScreen fullScreen = new FullScreen();

		public DiashowForm()
        {
            InitializeComponent();
        }

        public void ShowImage(string path)
        {
            Console.WriteLine("Switch to Image: " + path);
			if (pictureBox.Image != null) {
				pictureBox.Image.Dispose ();
			}
            pictureBox.Load(path);
        }

        public void ShowImage(string path, Color color)
        {
            pictureBox.SuspendLayout();
			this.ShowImage(path);
            pictureBox.BackColor = Color.Black;
            pictureBox.ResumeLayout();
        }

        public void ToggleFullScreen()
        {
            if (this.fullScreen.IsFullScreen)
            {
                this.fullScreen.LeaveFullScreen(this);
            }
            else
            {
                this.fullScreen.EnterFullScreen(this);
            }
        }

        private void pictureBox_DoubleClick(object sender, System.EventArgs e)
        {
            this.ToggleFullScreen();
        }

        private void DiashowForm_KeyDown(object sender, KeyEventArgs e)
        {
            Keys k = e.KeyCode;

            switch (k)
            {
                case Keys.Right:
                    if (this.NextPicture != null)
                    {
                        this.NextPicture();
                    }
                    break;
                case Keys.Space:
                    if (this.ToggleRunning != null)
                    {
                        this.ToggleRunning();
                    }
                    break;
            }
        }
    }
}
