using Number_Plate_Recognition.Detect;
using System.Collections.Generic;
using System.IO;

namespace Number_Plate_Recognition.Settings
{
    static class CreateLog
    {
        /// <summary>
        /// Создаёт файл log.txt 
        /// </summary>
        /// <param name="colectionPlates"></param>
        public static bool Create(List<State> colectionPlates)
        {
            if (colectionPlates.Count == 0)
                return false;
            Dictionary<DetectWay, LogInformation> a = new Dictionary<DetectWay, LogInformation>();
            foreach (var plate in colectionPlates)
            {
                var detectWay = plate.detectWay;
                if (!a.ContainsKey(detectWay))
                {
                    a[detectWay] = new LogInformation();
                    a[detectWay].Recall = new List<double>();
                    a[detectWay].Precision = new List<double>();
                }
                var current = a[detectWay];
                current.TP += plate.CountRightLP;
                current.FP += plate.CountWrongLP;
                current.FN += plate.CountUnknownLP;
                current.AffineCorrect += plate.CountAffine;
                current.Time += plate.Time;
                current.Precision.Add((double)plate.CountRightLP / (plate.CountRightLP + plate.CountWrongLP));
                current.Recall.Add((double)plate.CountRightLP / (plate.CountRightLP + plate.CountUnknownLP));
                if (current.MaxTime < plate.Time)
                    current.MaxTime = plate.Time;
                if (current.MinTime > plate.Time)
                    current.MinTime = plate.Time;
                current.CountImages++;
            }
            WriteInfoToFile(a);
            return true;
        }
        static private void WriteInfoToFile(Dictionary<DetectWay, LogInformation> log)
        {
            using (StreamWriter writer = new StreamWriter("log.txt"))
            {
                foreach (var item in log.Keys)
                {
                    var current = log[item];
                    writer.WriteLine("Для метода {0}", item.ToString());
                    writer.WriteLine("Время:");
                    writer.WriteLine("Общее время работы {0}", current.Time);
                    writer.WriteLine("Среднее время работы: {0}", current.Time / current.CountImages);
                    writer.WriteLine("Максимальное время работы: {0}", current.MaxTime);
                    writer.WriteLine("Минимальное время работы: {0}", current.MinTime);
                    writer.WriteLine("Количество номеров");
                    writer.WriteLine("Общее количество номеров: {0}", current.TP + current.FN);
                    writer.WriteLine("Количество истинно положительных: {0}", current.TP);
                    writer.WriteLine("Количество ложно положительных: {0}", current.FP);
                    writer.WriteLine("Количество ложно отрицательных: {0}", current.FN);
                    writer.WriteLine("Количество правильного аффинного преобразования: {0}", current.AffineCorrect);
                    writer.WriteLine("Метрики:");
                    var precision = (double)current.TP / (current.TP + current.FP);
                    writer.WriteLine("Precision: {0}", precision);
                    var recall = (double)current.TP / (current.TP + current.FN);
                    writer.WriteLine("Recall: {0}", recall);
                    writer.WriteLine("F1: {0}", 2 * precision * recall / (precision + recall));
                    writer.WriteLine("Аффинное преобразование: {0}", (double)current.AffineCorrect / current.TP);
                    writer.WriteLine("----------------");
                }
            }
        }
    }
    class LogInformation
    {
        /// <summary>
        /// True Positive
        /// </summary>
        public uint TP { get; set; }
        /// <summary>
        /// False Negative
        /// </summary>
        public uint FN { get; set; }
        /// <summary>
        /// False Positive
        /// </summary>
        public uint FP { get; set; }
        /// <summary>
        /// Количество изображений, для которых правильно был устранён угол искажения
        /// </summary>
        public uint AffineCorrect { get; set; }
        /// <summary>
        /// Общее время работы алгоритма
        /// </summary>
        public long Time { get; set; }
        /// <summary>
        /// Максимальное время работы алгоритма
        /// </summary>
        public long MaxTime { get; set; } = 0;
        /// <summary>
        /// Минимальное время работы алгоритма
        /// </summary>
        public long MinTime { get; set; } = long.MaxValue;
        /// <summary>
        /// Количество обработанных изображений
        /// </summary>
        public double CountImages { get; set; }
        public List<double> Recall { get; set; }
        public List<double> Precision { get; set; }
        public List<bool> HasObjectInImage { get; set; }
        public List<double> MyProperty { get; set; }
    }
}
