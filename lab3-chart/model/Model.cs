using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace lab3_chart.model
{
    public class Model
    {
        private ObservableCollection<Point> points;
        private readonly double leftBorder;
        private readonly double rightBorder;
        private readonly double stepLength;
        private readonly double coefC;

        public Model(double leftBorder, double rightBorder, double stepLength, double coefC)
        {
            this.points = new ObservableCollection<Point>();
            this.leftBorder = leftBorder;
            this.rightBorder = rightBorder;
            this.stepLength = stepLength;
            this.coefC = coefC;
        }

        public Model()
        {
            points = new ObservableCollection<Point>();
        }

        public ObservableCollection<Point> PointCalculation()
        {
            for (double x = leftBorder * 1000; x <= rightBorder * 1000; x += stepLength * 1000)
            {
                double y = Math.Sqrt(Math.Sqrt(Math.Pow(coefC, 4) + 4 * Math.Pow(x / 1000, 2) * Math.Pow(coefC, 2))
                           - Math.Pow(x / 1000, 2) - Math.Pow(coefC, 2));

                if (double.IsNaN(y))
                {
                    continue;
                }

                points.Add(new Point(x / 1000, y));
            }

            for (double x = rightBorder * 1000; x >= leftBorder * 1000; x -= stepLength * 1000)
            {
                double y = -Math.Sqrt(Math.Sqrt(Math.Pow(coefC, 4) + 4 * Math.Pow(x / 1000, 2) * Math.Pow(coefC, 2))
                           - Math.Pow(x / 1000, 2) - Math.Pow(coefC, 2));

                if (double.IsNaN(y))
                {
                    continue;
                }

                points.Add(new Point(x / 1000, y));
            }

            return points;
        }
    }
}
