using lab3_chart.model;
using System.Collections.ObjectModel;
using System.Windows;
using System.Globalization;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Model model = new Model(-1, 1, 0.1, 1);

            List<ObservablePoint> points = new List<ObservablePoint>();
            try
            {
                // ������ ���� ����� �� �����
                var lines = File.ReadAllLines("D:\\learning\\rps\\3\\lab3-chart\\lab3-chart\\test1.txt");

                // ������� ������ ������
                foreach (var line in lines)
                {
                    // ���������� ������ �� �����
                    var parts = line.Split(new[] { 'X', 'Y', ':', '\t' }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length == 2)
                    {
                        // �������������� ����� � �����
                        if (double.TryParse(parts[0], out double x) &&
                            double.TryParse(parts[1], out double y))
                        {
                            // �������� ����� � ���������� � ���������
                            points.Add(new ObservablePoint(x, y));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"������ ��� ������ �����: {ex.Message}");
            }

            ObservableCollection<ISeries> SeriesCollection = new ObservableCollection<ISeries>();

            var branch = new LineSeries<ObservablePoint>
            {
                Values = model.PointCalculation(),
                Name = "���������� ��������"
            };

            SeriesCollection.Add(branch);

            List<ObservablePoint> points1 = new List<ObservablePoint>();

            foreach (var series in SeriesCollection)
            {
                if (series is LineSeries<ObservablePoint> lineSeries)
                {
                    foreach (var point in lineSeries.Values)
                    {
                        points1.Add(new ObservablePoint(point.X, point.Y));
                    }
                }
            }

            Assert.AreNotEqual(points, points1);
        }
    }
}