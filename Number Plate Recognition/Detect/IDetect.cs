using System.Windows.Media.Imaging;

namespace Number_Plate_Recognition.Detect
{
    interface IDetect
    {
        long TimeWork { get; }
        BitmapImage[] GetImagePlates();
        BitmapImage GetImageWithPlates();
        void Detect();
    }
}
