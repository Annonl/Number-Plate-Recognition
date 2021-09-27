using Microsoft.WindowsAPICodePack.Dialogs;
using Number_Plate_Recognition.Detect;
using Number_Plate_Recognition.DistortionFix;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Number_Plate_Recognition
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Основное изображение на форме
        /// </summary>
        private ImageSource image;
        private Uri uri;
        private List<string> fileNames;
        /// <summary>
        /// Массив рамок автомобильных номеров на текущем изображении изображении
        /// </summary>
        private BitmapImage[] plates;
        /// <summary>
        /// Массив для представления объектов в таблице
        /// </summary>
        public static ObservableCollection<State> collection;
        /// <summary>
        /// Обект для работы с нахождением рамки автомобилей на изображении
        /// </summary>
        private IDetect detect;
        private ImageSource[] affinePlates;
        private State state;

        public MainWindow()
        {
            InitializeComponent();
            fileNames = new List<string>();
            collection = new ObservableCollection<State>();
            dataImagesCarsGrid.ItemsSource = collection;
            YoloDetect.Init();
        }
        /// <summary>
        /// Обновляет массив рамок на форме для текущего изображения
        /// </summary>
        private void RefreshPlates()
        {
            ImagesPlateListBox.Items.Clear();
            AffinePlateListBox.Items.Clear();
            for (int i = 0; i < plates.Length; i++)
                ImagesPlateListBox.Items.Add(plates[i]);
            foreach (var item in affinePlates)
                AffinePlateListBox.Items.Add(item);
        }

        private void OpenImage_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Photo Files |*.jpeg; *.png; *.jpg; *.bmp"
            };
            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                fileNames.Clear();
                // Open document 
                fileNames.Add(dlg.FileName);
                SetImageInForm(fileNames.Last());
                ImagesPlateListBox.Items.Clear();
                AffinePlateListBox.Items.Clear();
                LogStackPanel.Visibility = Visibility.Hidden;
            }
        }
        private void SetImageInForm(string fileName)
        {
            uri = new Uri(fileName);
            image = new BitmapImage(uri);
            MainImage.Source = image;
        }
        private void AddDataImageToGrid(long timeWorking)
        {
            state = new State(collection.Count + 1, (BitmapImage)image, plates, affinePlates, DateTime.Now, timeWorking, DetectCreate.detectWay);
            state.CountWrongLP = (uint)plates.Length;
            collection.Add(state);
        }
        /// <summary>
        /// Метод, вызывающий окно настроек
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            Setting setting = new Setting();
            setting.ShowDialog();
        }
        /// <summary>
        /// Метод для переключения между строчками в таблице
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var rowIndex = (int)((State)dataImagesCarsGrid.SelectedItems[0]).Number;
            state = collection[rowIndex - 1];
            plates = state.ImagesPlates;
            affinePlates = state.AffineImagesPlates;
            MainImage.Source = state.MainImageCar;
            CountRightLPTextBox.Text = state.CountRightLP.ToString();
            CountWrongLPTextBox.Text = state.CountWrongLP.ToString();
            CountUnknownLPTextBox.Text = state.CountUnknownLP.ToString();
            CountCorrectAffine.Text = state.CountAffine.ToString();
            LogStackPanel.Visibility = Visibility.Visible;
            RefreshPlates();
        }

        /// <summary>
        /// Метод нахождения рамок автомобильного номера
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindPlates_Click(object sender, RoutedEventArgs e)
        {
            if (fileNames.Count == 0)
            {
                System.Windows.MessageBox.Show("Выберите изображение, а затем выберите поиск рамок.", "Ошибка");
                return;
            }
            foreach (var fileName in fileNames)
            {
                detect = DetectCreate.Detect(fileName);
                detect.Detect();
                image = detect.GetImageWithPlates();
                plates = detect.GetImagePlates();
                affinePlates = new ImageSource[plates.Length];
                int count = 0;
                foreach (var item in plates)
                    affinePlates[count++] = RemoveDistortion.GetCorrectImage(item);
                AddDataImageToGrid(detect.TimeWork);
            }
            MainImage.Source = image;
            RefreshPlates();
            LogStackPanel.Visibility = Visibility.Visible;
        }

        private void OpenDirectory_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog fbd = new CommonOpenFileDialog { IsFolderPicker = true };
            if (fbd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                fileNames = Directory.GetFiles(fbd.FileName, "*.jpg").ToList();
                SetImageInForm(fileNames.Last());
                ImagesPlateListBox.Items.Clear();
                AffinePlateListBox.Items.Clear();
                LogStackPanel.Visibility = Visibility.Hidden;
                fbd.Dispose();
            }
        }

        private void SaveLogButton_Click(object sender, RoutedEventArgs e)
        {
            uint currentForUnknown, currentForRight, currentForWrong, currentForAffine;
            if (uint.TryParse(CountUnknownLPTextBox.Text, out currentForUnknown) && uint.TryParse(CountRightLPTextBox.Text, out currentForRight) &&
                uint.TryParse(CountWrongLPTextBox.Text, out currentForWrong) && uint.TryParse(CountCorrectAffine.Text, out currentForAffine))
            {
                state.CountUnknownLP = currentForUnknown;
                state.CountRightLP = currentForRight;
                state.CountWrongLP = currentForWrong;
                state.CountAffine = currentForAffine;
            }
            else
                System.Windows.MessageBox.Show("Ошибка с сохранением данных. Пожалуйста, вводите в отчёт неотрицательные числа", "Ошибка");
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkBox = (sender as System.Windows.Controls.CheckBox).IsChecked;
            if (checkBox.Value)
            {
                state.CountWrongLP--;
                state.CountRightLP++;
            }
            else
            {
                state.CountWrongLP++;
                state.CountRightLP--;
            }
        }

        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            var checkBox = (sender as System.Windows.Controls.CheckBox).IsChecked;
            if ((bool)checkBox)
                state.CountAffine++;
            else
                state.CountAffine--;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            YoloDetect.Dispose();
        }
    }
}
