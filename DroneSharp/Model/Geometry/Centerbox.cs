using System;
using Emgu.CV;
namespace DroneSharp.Model.Geometry
{
    [Obsolete("Centerbox is deprecated, please use BoxContainer instead.")]
    public class Centerbox
    {
        public Centerbox(int width, int height, int widthCenter, int heighOffset)
        {
            Width = width;
            Height = height;
            WidthCenter = widthCenter;
            HeighOffset = heighOffset;
            var x1 = width * ((1 - WidthCenter) / 2);
            var y1 = height * (((1 - HeightCenter) / 2) - heighOffset);
            var x2 = width * (1 - ((1 - WidthCenter) / 2));
            var y2 = height * ((1 - ((1 - HeightCenter) / 2)) - HeighOffset);
            Box = new Box(x1,y1,x2,y2);
            CenterPoint = new MyPoint(x1 +((x2-x1)/2),y1 + ((y2 - y1)/2));
        }


        public int Width { get; set; }
        public int Height { get; set; }
        public int HeightCenter { get; set; }
        public int WidthCenter { get; set; }
        public int HeighOffset { get; set; }
        public MyPoint CenterPoint { get; set; }
        private Box Box { get; set; }

        public void Render(Mat image)
        {
            Box.Render(image);
        }

        public void Flight(MyPoint point, out double xVal, out double yVal)
        {
            var xForce = point.X - CenterPoint.X;
            var yForce = point.Y - CenterPoint.Y;
            if (Math.Abs(xForce) > CenterPoint.X * WidthCenter)
            {
                xVal = (Map(xForce, -point.X, point.X, -100, 100) - xForce);
            }
            else
            {
                xVal = 0;
            }
            if (Math.Abs(yForce) > CenterPoint.Y * HeightCenter)
            {
                if (yForce < 0)
                {
                    yVal = Map(yForce, -CenterPoint.Y, 0, -100, 0);
                }
                else
                {
                    yVal = (Map(yForce, 0, Height-CenterPoint.Y, 0, 100));
                }
            }
            else
            {
                yVal = 0;
            }

            xVal = Math.Round(xVal, 2);
            yVal = Math.Round(yVal, 2) * -1;
        }

        /// <summary>
        /// Accepts a value and a range of the value together with another range
        /// Then it maps the value to the second range
        /// </summary>
        /// <param name="inputVal">Value to map to new range</param>
        /// <param name="inputMin">Input range minimum</param>
        /// <param name="inputMax">Input range maximum</param>
        /// <param name="outputMin">Output range minimum</param>
        /// <param name="outputMax">Output range maximum</param>
        /// <returns></returns>
        private static float Map(float inputVal, float inputMin, float inputMax, float outputMin, float outputMax)
        {
            return (inputVal - inputMin) * (outputMax - outputMin) / (inputMax - inputMin) + outputMin;
        }
    }
}
