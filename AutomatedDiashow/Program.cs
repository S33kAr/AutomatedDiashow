
namespace AutomatedDiashow
{
    class Program
    {
        static void Main(string[] args)
        {
            PictureManager pictureManager = new PictureManager(Config.Load());
            DiashowForm form = new DiashowForm();
            pictureManager.PictureChanged += form.ShowImage;
            form.NextPicture += () => { pictureManager.SwitchToNextPicture(); };
            form.ToggleRunning += pictureManager.ToggleRunning;

            form.ShowDialog();
        }
    }
}