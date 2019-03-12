using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DroneSharp.Model.Exceptions;
using Emgu.CV;
using Emgu.CV.Structure;

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


        public MyPoint PointStart { get; set; }
        public MyPoint PointEnd { get; set; }
        public int AngleThreshold { get; set; } = 20;

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
    }
}
