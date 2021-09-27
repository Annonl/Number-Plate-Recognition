using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Windows.Media.Imaging;

namespace Number_Plate_Recognition.DistortionFix
{
    static class Affine
    {
        public static BitmapImage RotateImage(BitmapImage plate, double angle)
        {
            angle = -angle;
            Image<Bgr, Byte> imagePlate = ConvertImage.ToImage(plate);
            var center = new System.Drawing.PointF(imagePlate.Width / 2, imagePlate.Height / 2);
            RotationMatrix2D matrix2D = new RotationMatrix2D(center, angle, 1);
            var newImage = imagePlate.WarpAffine(matrix2D, Inter.Cubic, Warp.FillOutliers, BorderType.Constant, new Bgr(0,0,0));
            return ConvertImage.ToBitmapImage(newImage);
        }
    }
}
