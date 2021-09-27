using Alturos.Yolo;
using Alturos.Yolo.Model;
using Number_Plate_Recognition.Detect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Number_Plate_Recognition
{
    class YoloDetect : IDetect
    {
        public long TimeWork { get; private set; }
        static YoloWrapper yoloClasificator;
        List<YoloItem> plates;
        BitmapImage image;
        public string FileName { get; private set; }
        static public void Init()
        {
            GpuConfig gpuConfig = new GpuConfig();
            gpuConfig.GpuIndex = 0;
            yoloClasificator = new YoloWrapper("YoloModel\\yolov3_testing.cfg", "YoloModel\\yolov3_training_last.weights", "YoloModel\\coco.names", gpuConfig);
        }
        public YoloDetect(string fileName)
        {
            FileName = fileName;
            image = new BitmapImage(new Uri(fileName));
        }
        public void Detect()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            plates = yoloClasificator.Detect(FileName).ToList<YoloItem>();
            watch.Stop();
            TimeWork = watch.ElapsedMilliseconds;
        }
        public BitmapImage GetImageWithPlates()
        {
            DrawingVisual dVisual = new DrawingVisual();
            using (DrawingContext dc = dVisual.RenderOpen())
            {
                dc.DrawImage(image, new Rect(0, 0, image.PixelWidth, image.PixelHeight));
                foreach (var plate in plates)
                    dc.DrawRectangle(null, new Pen(Brushes.Red, 3), new Rect(plate.X, plate.Y, plate.Width, plate.Height));
            }
            RenderTargetBitmap targetBitmap = new RenderTargetBitmap(image.PixelWidth, image.PixelHeight, 96, 96, PixelFormats.Default);
            targetBitmap.Render(dVisual);
            return ConvertImage.ToBitmapImage(targetBitmap);
        }

        public BitmapImage[] GetImagePlates()
        {
            List<BitmapImage> platesBitMapImage = new List<BitmapImage>();
            int width, height;
            foreach (var plate in plates)
            {
                width = plate.Width + plate.X > image.PixelWidth ? image.PixelWidth - plate.X : plate.Width;
                height = plate.Height + plate.Y > image.PixelHeight ? image.PixelHeight - plate.Y : plate.Height;
                platesBitMapImage.Add(ConvertImage.ToBitmapImage(new CroppedBitmap(image, new Int32Rect(plate.X, plate.Y, width, height))));
            }
            return platesBitMapImage.ToArray();
        }
        static public void Dispose()
        {
            yoloClasificator.Dispose();
        }
    }
}
