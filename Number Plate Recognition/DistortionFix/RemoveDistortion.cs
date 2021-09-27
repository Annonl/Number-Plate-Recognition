using System.Windows.Media.Imaging;

namespace Number_Plate_Recognition.DistortionFix
{
    static class RemoveDistortion
    {
        static public BitmapImage GetCorrectImage(BitmapImage image)
        {
            BitmapImage newImage;
            double angle = Haaf.GetAngleOfRotation(image);
            newImage = Affine.RotateImage(image, angle);
            newImage = Haaf.CropingImage(newImage);
            return newImage;
        }
    }
}
