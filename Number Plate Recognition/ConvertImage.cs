using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace Number_Plate_Recognition
{
    static class ConvertImage
    {
        /// <summary>
        /// Преобразует заданное изображение из библиотеки EmguCV в изображение BitmapImage
        /// </summary>
        /// <param name="image">Изображение из библиотеки EmguCV, которое необходимо преобразовать</param>
        /// <returns></returns>
        public static BitmapImage ToBitmapImage(Image<Bgr, Byte> image)
        {
            var bitmap = image.ToBitmap();
            using (MemoryStream inStream = new MemoryStream())
            {
                bitmap.Save(inStream, ImageFormat.Png);
                inStream.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = inStream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }
        /// <summary>
        /// Преобразует заданное изображение типа BitmapImage в изображение из библиотеки EmguCV
        /// </summary>
        /// <param name="image">Изображение типа BitmapImage, которое необходимо преобразовать</param>
        /// <returns></returns>
        public static Image<Bgr, Byte> ToImage(BitmapImage image)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(image));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return bitmap.ToImage<Bgr, Byte>();
            }
        }
        /// <summary>
        /// Преобразует заданное изображение типа BitmapImage в изображение с выделенными контурами
        /// </summary>
        /// <param name="image">Изображение типа BitmapImage, которое необходимо преобразовать</param>
        /// <returns></returns>
        public static Image<Gray, Byte> ToBinaryGray(BitmapImage image)
        {
            Image<Gray, Byte> gray;
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(image));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);
                gray = bitmap.ToImage<Gray, Byte>();
            }
            CvInvoke.GaussianBlur(gray, gray, new Size(3, 3), 0, 1, BorderType.Default);
            CvInvoke.Sobel(gray, gray, DepthType.Default, 0, 1, 3);
            CvInvoke.Canny(gray, gray, 70, 130);
            return gray;
        }
        /// <summary>
        /// Преобразует заданное изображение типа BitmapSource в иззображение типа BitmapImage
        /// </summary>
        /// <param name="image">Изображение типа BitmapSouorce, которое необходимо преобразовать</param>
        /// <returns></returns>
        public static BitmapImage ToBitmapImage(BitmapSource image)
        {
            var bitmapImage = new BitmapImage();
            var bitmapEncoder = new PngBitmapEncoder();
            bitmapEncoder.Frames.Add(BitmapFrame.Create(image));
            using (var stream = new MemoryStream())
            {
                bitmapEncoder.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);

                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }
    }
}
