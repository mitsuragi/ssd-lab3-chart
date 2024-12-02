using LiveChartsCore.Defaults;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls.Ribbon.Primitives;

namespace lab3_chart.model
{
    public class Model
    {
        private List<ObservablePoint> points;
        private readonly double leftBorder;
        private readonly double rightBorder;
        private readonly double stepLength;
        private readonly double coefC;

        public Model(double leftBorder, double rightBorder, double stepLength, double coefC)
        {
            this.points = new List<ObservablePoint>();
            this.leftBorder = leftBorder;
            this.rightBorder = rightBorder;
            this.stepLength = stepLength;
            this.coefC = coefC;
        }

        public Model()
        {
            points = new List<ObservablePoint>();
        }

        public List<ObservablePoint> PointCalculation()
        {
            for (double x = leftBorder; x <= rightBorder; x += stepLength)
            {
                double y = calculatePoint(x);

                if (double.IsNaN(y))
                {
                    continue;
                }

                points.Add(new ObservablePoint(x, y));
            }

            for (double x = rightBorder; x >= leftBorder; x -= stepLength)
            {
                double y = -calculatePoint(x);

                if (double.IsNaN(y))
                {
                    continue;
                }

                if (y == 0) y = Math.Abs(y);

                points.Add(new ObservablePoint(x, y));
            }

            if (points.Count > 0)
            {
                points.Add(new ObservablePoint(points[0].X, points[0].Y));
            }

            return points;
        }

        private double calculatePoint(double x)
        {
            return Math.Sqrt(Math.Sqrt(Math.Pow(coefC, 4) + 4 * Math.Pow(x, 2) * Math.Pow(coefC, 2))
                           - Math.Pow(x, 2) - Math.Pow(coefC, 2));
        }
    }
}
