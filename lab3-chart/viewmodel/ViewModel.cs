using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using lab3_chart.model;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;

namespace lab3_chart.viewmodel
{
    class ViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly ErrorsViewModel errorsVM;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        private bool showStartupMessage;
        public bool ShowStartupMessage
        {
            get => showStartupMessage;
            set
            {
                showStartupMessage = value;
                OnPropertyChanged();
            }
        }

        private double leftBorder = -2;
        public double LeftBorder
        {
            get => leftBorder;
            set
            {
                leftBorder = value;

                errorsVM.ClearErrors(nameof(LeftBorder));
                if (value >= RightBorder)
                {
                    errorsVM.AddError(nameof(LeftBorder), "Нарушение границ");
                }

                OnPropertyChanged(nameof(SeriesCollection));
            }
        }

        private double rightBorder = 2;
        public double RightBorder
        {
            get => rightBorder;
            set
            {
                rightBorder = value;

                errorsVM.ClearErrors(nameof(RightBorder));
                if (LeftBorder >= value)
                {
                    errorsVM.AddError(nameof(RightBorder), "Нарушение границ");
                }

                OnPropertyChanged(nameof(SeriesCollection));
            }
        }

        private double stepLength = 0.1;
        public double StepLength
        {
            get => stepLength;
            set
            {
                stepLength = value;

                errorsVM.ClearErrors(nameof(StepLength));
                if (value <= 0)
                {
                    errorsVM.AddError(nameof(StepLength), "Шаг не может быть меньше или равным нулю");
                }

                OnPropertyChanged(nameof(SeriesCollection));
            }
        }

        private double coefC = 1;
        public double CoefC
        {
            get => coefC;

            set
            {
                coefC = value;

                errorsVM.ClearErrors(nameof(CoefC));
                if (value == 0)
                {
                    errorsVM.AddError(nameof(CoefC), "При коэффициенте 0 функция не имеет точек");
                }

                OnPropertyChanged(nameof(SeriesCollection));
            }
        }

        public ObservableCollection<ISeries> SeriesCollection { get; set; }
        public ObservableCollection<Axis> XAxes { get; set; }
        public ObservableCollection<Axis> YAxes { get; set; }

        private ObservableCollection<ObservablePoint> points;
        public ObservableCollection<ObservablePoint> Points
        {
            get => points;
            set
            {
                points = value;
                OnPropertyChanged(nameof(Points));
            }
        }

        private Model model;

        public ViewModel()
        {
            showStartupMessage = Properties.Settings.Default.ShowStartupMessage;

            if (showStartupMessage)
            {
                ShowAbout();
            }

            SeriesCollection = new ObservableCollection<ISeries>();
            points = new ObservableCollection<ObservablePoint>();
            model = new Model();
            errorsVM = new ErrorsViewModel();
            errorsVM.ErrorsChanged += ErrorsViewModel_ErrorsChanged;

            StartCommand = new RelayCommand(startCommand, CanExecute);
            ShowAboutCommand = new RelayCommand(ShowAbout, () => true);
            FileLoadDataCommand = new RelayCommand(FileLoadData, () => true);
            FileSaveInitialCommand = new RelayCommand(FileSaveInitial, () => true);
            FileSaveResultCommand = new RelayCommand(FileSaveResult, () => SeriesCollection.Count != 0);
            SwitchShowStartupCommand = new RelayCommand(switchShow, () => true);

            XAxes = new ObservableCollection<Axis>
            {
                new Axis {}
            };

            YAxes = new ObservableCollection<Axis>
            {
                new Axis {}
            };
        }

        public ICommand StartCommand { get; }
        public ICommand FileLoadDataCommand { get; }
        public ICommand FileSaveInitialCommand { get; }
        public ICommand FileSaveResultCommand { get; }
        public ICommand ShowAboutCommand { get; }
        public ICommand SwitchShowStartupCommand { get; }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        private void switchShow()
        {
            Properties.Settings.Default.ShowStartupMessage = showStartupMessage;
            Properties.Settings.Default.Save();
        }

        private void ShowAbout()
        {
            string messageBoxText = "Задание выполнил студент группы №424 Губкин Максим.\n" +
                "Вариант №4\n\n" +
                "Текст задания: Необходимо написать приложение для " +
                "построения графика функции и вывода таблицы значений функции.\n" +
                "Функция: Лемниската Бернулли.";

            string caption = "Справка";

            MessageBoxImage icon = MessageBoxImage.Information;

            MessageBox.Show(messageBoxText, caption, MessageBoxButton.OK, icon);
        }

        private void startCommand()
        {
            model = new Model(leftBorder, rightBorder, stepLength, coefC);

            SeriesCollection.Clear();

            var branch = new LineSeries<ObservablePoint>
            {
                Values = model.PointCalculation(),
                Name = "Лемниската Бернулли"
            };

            SeriesCollection.Add(branch);

            UpdatePointsTable();
            OnPropertyChanged(nameof(SeriesCollection));
        }

        private void UpdatePointsTable()
        {
            points.Clear();

            // Извлекаем точки из всех серий
            foreach (var series in SeriesCollection)
            {
                if (series is LineSeries<ObservablePoint> lineSeries)
                {
                    foreach (var point in lineSeries.Values)
                    {
                        // Добавляем строку с координатами в таблицу
                        points.Add(new ObservablePoint(point.X, point.Y));
                    }
                }
            }

            OnPropertyChanged(nameof(Points));
        }

        private void FileLoadData()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Текстовые документы (.txt)|*.txt";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                string filename = dialog.FileName;
                GetData(filename);
            }
        }
        private void GetData(string filename)
        {
            StreamReader sr = new StreamReader(filename);

            string? line;
            List<double> values = new List<double>();

            while ((line = sr.ReadLine()) != null)
            {
                string[] numbers = line.Split(' ');
                foreach (string numberString in numbers)
                {
                    if (double.TryParse(numberString, out double number))
                    {
                        values.Add(number);
                    }
                    else
                    {
                        values.Clear();
                        MessageBox.Show("Файл содержит некорректные данные", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        sr.Close();
                        return;
                    }
                }
            }
            if (values.Count != 4)
            {
                values.Clear();
                MessageBox.Show("Файл содержит некорректные данные", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                sr.Close();
                return;
            }
            else
            {
                leftBorder = values[0]; OnPropertyChanged(nameof(LeftBorder));
                rightBorder = values[1]; OnPropertyChanged(nameof(RightBorder));
                stepLength = values[2]; OnPropertyChanged(nameof(StepLength));
                coefC = values[3]; OnPropertyChanged(nameof(CoefC));
            }

            sr.Close();
        }
        private void FileSaveInitial()
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.FileName = "Исходные данные";
            dialog.DefaultExt = ".txt";
            dialog.Filter = "Текстовые документы (.txt)|*.txt";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                string filename = dialog.FileName;
                WriteInitialDataToFile(filename);
            }
        }
        private void WriteInitialDataToFile(string filename)
        {
            StreamWriter sw = new StreamWriter(filename);

            string line = string.Join(" ", leftBorder, rightBorder, stepLength, coefC);

            sw.Write(line);

            sw.Close();
        }
        private void FileSaveResult()
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.FileName = "Результат";
            dialog.DefaultExt = ".txt";
            dialog.Filter = "Текстовые документы (.txt)|*.txt";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                string filename = dialog.FileName;
                WriteResultDataToFile(filename);
            }
        }
        private void WriteResultDataToFile(string filename)
        {
            StreamWriter sw = new StreamWriter(filename);

            foreach(ObservablePoint pt in points)
            {
                sw.WriteLine($"X: {pt.X}\tY: {pt.Y}");
            }

            sw.Close();
        }
        public bool CanExecute() => !HasErrors;

        public bool HasErrors => errorsVM.HasErrors;

        private void ErrorsViewModel_ErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            ErrorsChanged?.Invoke(this, e);
            OnPropertyChanged(nameof(CanExecute));
        }

        public IEnumerable GetErrors(string? propertyName)
        {
            return errorsVM.GetErrors(propertyName);
        }
    }
}
