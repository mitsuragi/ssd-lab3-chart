using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using lab3_chart.model;

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

        private double leftBorder;
        public double LeftBorder
        {
            get => leftBorder;
            set
            {
                leftBorder = value;

                errorsVM.ClearErrors(nameof(LeftBorder));
                if (value > RightBorder)
                {
                    errorsVM.AddError(nameof(LeftBorder), "Левая граница не может быть больше правой");
                }

                OnPropertyChanged(nameof(Points));
            }
        }

        private double rightBorder;
        public double RightBorder
        {
            get => rightBorder;
            set
            {
                rightBorder = value;

                errorsVM.ClearErrors(nameof(RightBorder));
                if (LeftBorder > value)
                {
                    errorsVM.AddError(nameof(RightBorder), "Правая граница не может быть меньше левой");
                }

                OnPropertyChanged(nameof(Points));
            }
        }

        private double stepLength = 1;
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

                OnPropertyChanged(nameof(Points));
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

                OnPropertyChanged(nameof(Points));
            }
        }

        private ObservableCollection<Point> points;
        public ObservableCollection<Point> Points
        {
            get => points;
        }

        private Model model;

        public ViewModel()
        {
            points = new ObservableCollection<Point>();
            model = new Model();
            errorsVM = new ErrorsViewModel();
            errorsVM.ErrorsChanged += ErrorsViewModel_ErrorsChanged;

            StartCommand = new RelayCommand(startCommand, CanExecute);
            ShowAboutCommand = new RelayCommand(showAbout, () => true);
        }

        public ICommand StartCommand { get; }
        public ICommand FileLoadDataCommand { get; }
        public ICommand FileSaveInitialCommand { get; }
        public ICommand FileSaveResultCommand { get; }
        public ICommand ExportToExcelCommand { get; }
        public ICommand ShowAboutCommand { get; }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        private void showAbout()
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

            points = model.PointCalculation();

            OnPropertyChanged(nameof(Points));
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
