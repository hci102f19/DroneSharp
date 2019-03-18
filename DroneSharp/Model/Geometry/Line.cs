using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;
using DroneSharp.Model.Exceptions;
using Emgu.CV;
using Emgu.CV.Structure;
using Point = Accord.Point;

namespace DroneSharp.Model
{
    public class Line
    {
        public Line(float rho, float theta)
        {

            var a = Math.Cos(theta);
            var b = Math.Sin(theta);

            var x0 = a * rho;
            var y0 = b * rho;

            var pt1 = new MyPoint
            {
                X = (int)Math.Round(x0 + 1000 * (-b)),
                Y = (int)Math.Round(y0 + 1000 * (a))
            };

            var pt2 = new MyPoint
            {
                X = (int)Math.Round(x0 - 1000 * (-b)),
                Y = (int)Math.Round(y0 - 1000 * (a))
            };
            PointStart = pt1;
            PointEnd = pt2;
            ValidateLine();
        }

        public Line(MyPoint start, MyPoint end)
        {
            if (start == end) throw new ArgumentException("Start point of the line cannot be the same as its end point.");
            this.k = (float)(((double)end.Y - (double)start.Y) / ((double)end.X - (double)start.X));
            this.b = float.IsInfinity(this.k) ? start.X : start.Y - this.k * start.X;
        }


        public MyPoint PointStart { get; set; }
        public MyPoint PointEnd { get; set; }
        public int AngleThreshold { get; set; } = 20;
        public float k { get; set; }
        public float b { get; set; }
        public bool IsVertical
        {
            get
            {
                return float.IsInfinity(this.k);
            }
        }

        public void ValidateLine()
        {
            var cAngle =
                Math.Round(Math.Abs(RadiansToDegrees(Math.Atan2(PointEnd.Y - PointStart.Y, PointEnd.X - PointStart.X))),0);
            if (180 - AngleThreshold <= cAngle || cAngle <= AngleThreshold)
            {
                 throw new InvalidLineException("Line not withing angle scope");
            }

            if (90 - AngleThreshold <= cAngle && cAngle <= 90 + AngleThreshold)
            {
                throw new InvalidLineException("Line not withing angle scope");
            }
        }
       
        public void Render(Mat image)
        {
            CvInvoke.Line(image, PointStart.ToPoint(), PointEnd.ToPoint(), new MCvScalar(0, 0, 255), 2);
        }

        private static double RadiansToDegrees(double radians)
        {
            double degrees = (180 / Math.PI) * radians;
            return (degrees);
        }

        public MyPoint GetIntersectionWith(Line secondLine)
        {
            float k = secondLine.k;
            float b = secondLine.b;
            bool isVertical1 = IsVertical;
            bool isVertical2 = secondLine.IsVertical;
            MyPoint nullable = new MyPoint();
            if ((double)this.k == (double)k || isVertical1 & isVertical2)
            {
                if ((double)this.b == (double)b)
                    throw new InvalidOperationException("Identical lines do not have an intersection point.");
            }
            else if (isVertical1)
                nullable = new MyPoint(b, k * b + b);
            else if (isVertical2)
            {
                nullable = new MyPoint(b, k * b + b);
            }
            else
            {
                float x = (float)(((double)b - (double)this.b) / ((double)this.k - (double)k));
                nullable = new MyPoint(x, k * x + b);
            }
            return nullable;
        }
    }
}
