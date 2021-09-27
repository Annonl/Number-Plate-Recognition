using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace Number_Plate_Recognition.DistortionFix
{
    static class Haaf
    {
        /// <summary>
        /// Обрезает изображение рамки автомобильного номера
        /// </summary>
        /// <param name="plate">Повёрнутое изображение, содержащее рамку автомобильного номера</param>
        /// <returns>Обрезанное изображение</returns>
        static public BitmapImage CropingImage(BitmapImage plate)
        {
            Image<Bgr, Byte> normalPlate = ConvertImage.ToImage(plate);
            var gray = ConvertImage.ToBinaryGray(plate);
            LineSegment2D[] lines = CvInvoke.HoughLinesP(
               gray,
               1, //Distance resolution in pixel-related units
               Math.PI / 180, //Angle resolution measured in radians.
               100, 100, 50); //gap between lines

            int maxLenght = -1;
            int minLenght = int.MaxValue;
            foreach (var line in lines)
            {
                //normalPlate.Draw(line, new Bgr(Color.Red), 1);
                int max = Math.Max(line.P1.Y, line.P2.Y);
                int min = Math.Min(line.P1.Y, line.P2.Y);
                if (maxLenght < max)
                    maxLenght = max;
                if (minLenght > min)
                    minLenght = min;
            }
            if (minLenght > normalPlate.Height / 2 || minLenght < 0)
                minLenght = 0;
            if (maxLenght < normalPlate.Height / 2 || maxLenght > normalPlate.Height)
                maxLenght = normalPlate.Height;
            var rect = new Rectangle(0, minLenght, normalPlate.Width, maxLenght);
            normalPlate.ROI = rect;
            return ConvertImage.ToBitmapImage(normalPlate);
        }

        #region Angle
        /// <summary>
        /// Метод,который по входному изображению рамки автомобильного номера рассчитывает угол, на который необходимо повернуть изображение
        /// </summary>
        /// <param name="plate">Изображение, содержащее рамку автомобильного номера</param>
        /// <returns>Угол поворота</returns>
        static public double GetAngleOfRotation(BitmapImage plate)
        {
            var gray = ConvertImage.ToBinaryGray(plate);
            return GetAngle(gray);
        }
        static private double GetAngle(Image<Gray, byte> gray)
        {
            int D = (int)(Math.Sqrt(gray.Width * gray.Width + gray.Height * gray.Height));
            Image<Gray, int> houghSpace = new Image<Gray, int>(181, ((int)(1.414213562 * D) * 2) + 1);
            int xpoint = 0;
            double maxT = 0;
            double[,] table = CreateTable();
            int halfHeight = houghSpace.Height / 2;
            int intensity = 0;
            for (int xi = 0; xi < gray.Width; xi++)
            {
                for (int yi = 0; yi < gray.Height; yi++)
                {
                    if (gray.Data[yi, xi, 0] == 0) continue;
                    for (int i = 0; i < 181; i++)
                    {
                        int rho = (int)((xi * table[0, i] + yi * table[1, i])) + halfHeight;
                        if ((intensity = houghSpace.Data[rho, i, 0]++) > maxT)
                        {
                            maxT = intensity;
                            xpoint = i;
                        }
                    }
                }
            }
            double thetaHotPoint = ((Math.PI / 180) * -90d) + (Math.PI / 180) * xpoint;
            return (90 - Math.Abs(thetaHotPoint) * (180 / Math.PI)) * (thetaHotPoint < 0 ? -1 : 1);
        }
        static private double[,] CreateTable()
        {
            double[,] table = new double[2, 181]; // 0 - cos, 1 - sin;
            double rad = Math.PI / 180;
            double theta = rad * -90;
            for (int i = 0; i < 181; i++)
            {
                table[0, i] = Math.Cos(theta);
                table[1, i] = Math.Sin(theta);
                theta += rad;
            }
            return table;
        }
        #endregion
    }
}
