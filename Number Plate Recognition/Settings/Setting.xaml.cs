using Number_Plate_Recognition.Detect;
using Number_Plate_Recognition.Settings;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Number_Plate_Recognition
{
    /// <summary>
    /// Логика взаимодействия для Setting.xaml
    /// </summary>
    public partial class Setting : Window
    {
        private DetectWay way;
        public Setting()
        {
            InitializeComponent();
            MinSizeTextBox.Text = HaarDetect.MinSize.Width.ToString();
            ScaleFactorTextBox.Text = HaarDetect.ScaleFactor.ToString();
            MinNeighbordsTextBox.Text = HaarDetect.MinNeighbords.ToString();
            switch (DetectCreate.detectWay)
            {
                case DetectWay.Yolo:
                    YoloRadioButton.IsChecked = true;
                    break;
                case DetectWay.Haar:
                    HaarRadioButton.IsChecked = true;
                    break;
                default:
                    break;
            }
        }
        #region ChangeWindowsSettings
        private void DefaulSetting_Click(object sender, RoutedEventArgs e)
        {
            HardSettings.Visibility = Visibility.Hidden;
            DefaultSettings.Visibility = Visibility.Visible;
        }

        private void DificultSetting_Click(object sender, RoutedEventArgs e)
        {
            HardSettings.Visibility = Visibility.Visible;
            DefaultSettings.Visibility = Visibility.Hidden;
        }
        #endregion

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HaarDetect.MinNeighbords = int.Parse(MinNeighbordsTextBox.Text);
                HaarDetect.MinSize = new System.Drawing.Size(int.Parse(MinSizeTextBox.Text), int.Parse(MinSizeTextBox.Text));
                HaarDetect.ScaleFactor = double.Parse(ScaleFactorTextBox.Text);
                DetectCreate.detectWay = way;
                MessageBox.Show("Настройки были успешно сохранены");
            }
            catch (Exception)
            {
                MessageBox.Show("Произошла ошибка. Пожалуйста, проверте правильность заполенных настроек","Ошибка");
                throw;
            }
            
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as RadioButton;
            way = (DetectWay)Enum.Parse(typeof(DetectWay), button.Tag.ToString());
        }

        private void LogButton_Click(object sender, RoutedEventArgs e)
        {

            var plates = MainWindow.collection;
            var flag = CreateLog.Create(plates.ToList());
            if (flag)
                MessageBox.Show("Отчёт был успешно записан. Полный отчёт можно найти в папке приложения в файле log.txt", "Отчёт");
            else
                MessageBox.Show("Произошла ошибка. Пожалуйста, повторите позже.", "Ошибка");
        }

        private void AboutProgramButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Распознавание рамки номера автомобиля и устранение искажений.\n Разработчик: Колпащиков Алексей\nСтудент ФИб-2301 ВятГУ");
        }
    }
}
