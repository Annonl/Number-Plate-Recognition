using Emgu.CV;
using Emgu.CV.Structure;
using Number_Plate_Recognition.Detect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Media.Imaging;

namespace Number_Plate_Recognition
{
    public class HaarDetect : IDetect
    {
        #region Settings
        static public double ScaleFactor { get; set; } = 1.04;
        static public int MinNeighbords { get; set; } = 12;
        static public Size MinSize { get; set; } = new Size(15, 15);
        public string FileName { get; private set; }
        #endregion

        public long TimeWork { get; private set; }
        static private CascadeClassifier cascadeClassifierForRectangle;
        static private CascadeClassifier cascadeClassifierForSquare;
        private Image<Bgr, byte> imageCar;
        private Rectangle[] plates4Rectangle;
        private Rectangle[] plates4Square;
        static HaarDetect()
        {
            cascadeClassifierForRectangle = new CascadeClassifier("HaarModel\\haarcascade_russian_plate_number.xml");
            cascadeClassifierForSquare = new CascadeClassifier("HaarModel\\cascade4.xml");
        }
        public HaarDetect(string fileName)
        {
            FileName = fileName;
            imageCar = new Image<Bgr, byte>(FileName);
        }
        public void Detect()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            plates4Rectangle = cascadeClassifierForSquare.DetectMultiScale(imageCar, ScaleFactor, MinNeighbords);
            watch.Stop();
            watch.Start();
            plates4Square = cascadeClassifierForRectangle.DetectMultiScale(imageCar, ScaleFactor, MinNeighbords);
            watch.Stop();
            TimeWork = watch.ElapsedMilliseconds;
        }
        /// <summary>
        /// Получение массива отдельных изображений номерных знаков
        /// </summary>
        /// <returns>Возвращает массив рамок номерных знаков</returns>
        public BitmapImage[] GetImagePlates()
        {
            List<Image<Bgr, Byte>> arrayPlates = new List<Image<Bgr, byte>>();
            foreach (var item in plates4Rectangle)
                arrayPlates.Add(imageCar.Copy(item));
            foreach (var item in plates4Square)
                arrayPlates.Add(imageCar.Copy(item));
            List<BitmapImage> imagesPlates = new List<BitmapImage>();
            return arrayPlates.Select(x => ConvertImage.ToBitmapImage(x)).ToArray();
        }
        /// <summary>
        /// Получение изображения с отмеченными рамками номерных знаков
        /// </summary>
        /// <returns>Изображение с прямоугольниками, обозначающие рамки номерного знака</returns>
        public BitmapImage GetImageWithPlates()
        {
            var newImageCar = imageCar.Clone();
            foreach (var rectangle in plates4Rectangle)
                newImageCar.Draw(rectangle, new Bgr(0, 0, 255), 3);
            foreach (var item in plates4Square)
                newImageCar.Draw(item, new Bgr(0, 0, 255), 3);
            return ConvertImage.ToBitmapImage(newImageCar);
        }
    }
}
