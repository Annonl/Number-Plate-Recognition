using Number_Plate_Recognition.Detect;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Number_Plate_Recognition
{
    public class State
    {
        /// <summary>
        /// Номер изображения
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// Изображение, содержащее в себе выделенную рамку автомобильного номера
        /// </summary>
        public BitmapImage MainImageCar { get; set; }
        /// <summary>
        /// Изображения рамок до устранения искажений
        /// </summary>
        public BitmapImage[] ImagesPlates { get; set; }
        /// <summary>
        /// изображения рамок после устранения искажений
        /// </summary>
        public ImageSource[] AffineImagesPlates { get; set; }
        /// <summary>
        /// Дата и время окончания работы алгоритма
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Количество выделенных номерных знаков на изображении
        /// </summary>
        public int CountPlates { get => ImagesPlates.Length; }
        /// <summary>
        /// Время работы алгоритма
        /// </summary>
        public long Time { get; set; }
        /// <summary>
        /// Способ, которым находилась рамка автомобильного номера
        /// </summary>
        public DetectWay detectWay { get; set; }
        /// <summary>
        /// Количество правильно выделенных рамок автомобиля
        /// </summary>
        public uint CountRightLP { get; set; }
        /// <summary>
        /// Количество неправильно выделенных рамок автомобиля
        /// </summary>
        public uint CountWrongLP { get; set; }
        /// <summary>
        /// Количество невыделенных рамок автомобиля
        /// </summary>
        public uint CountUnknownLP { get; set; }
        /// <summary>
        /// Количество рамок, для которых правильно было произведено устранение искажений
        /// </summary>
        public uint CountAffine { get; set; }
        public State(int numberOfCars, BitmapImage image, BitmapImage[] plates, ImageSource[] affine, DateTime date, long timeWork, DetectWay way)
        {
            Number = numberOfCars;
            MainImageCar = image;
            ImagesPlates = plates;
            Date = date;
            Time = timeWork;
            detectWay = way;
            AffineImagesPlates = affine;
        }
    }
}
