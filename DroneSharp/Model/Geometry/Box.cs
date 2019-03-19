using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using DroneSharp.Model.Flight;
using Emgu.CV;
using Emgu.CV.Structure;
using Rectangle = System.Drawing.Rectangle;
namespace DroneSharp.Model
{
    public class Box
    {
        public Box(int x1, int y1, int x2, int y2)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
            Rectangle = Rectangle.FromLTRB(X1, Y1, X2, Y2);
            XMin = Math.Min(x1, x2);
            YMin = Math.Min(y1, y2);
            XMax = Math.Max(x1, x2);
            YMax = Math.Max(y1, y2);
            Center = new MyPoint(x1 + ((x2 - x1) / 2), y1 + ((y2 - y1) / 2));
        }

        public int X1 { get; set; }
        public int Y1 { get; set; }
        public int X2 { get; set; }
        public int Y2 { get; set; }
        public int XMin { get; set; }
        public int YMin { get; set; }
        public int XMax { get; set; }
        public int YMax { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public MyPoint Center  { get; set; }
        public Color Color { get; set; } = Color.FromArgb(0, 255, 0);
        public Rectangle Rectangle { get; set; }

        public Rectangle Rotate(float angle)
        {
            RotatedRect rct = new RotatedRect(Center.ToPoint(), new SizeF(Width, Height), angle);
            return rct.MinAreaRect();
        }

        public void Render(Mat image, MyPoint point = null)
        {
            CvInvoke.Circle(image,Center.ToPoint(),5,new MCvScalar(Color.R,Color.G,Color.B),-1);

            if (point == null || Rectangle.IntersectsWith(new Rectangle(point.ToPoint(),new Size(1,1))))
            {
                CvInvoke.Rectangle(image, Rectangle, new MCvScalar(0, 255, 0), 3);
            }
            else
            {
                CvInvoke.Rectangle(image, Rectangle, new MCvScalar(0, 0, 255), 3);
            }
        }

        public int Area()
        {
            return Rectangle.Height * Rectangle.Width;
        }

        public virtual bool Hit(MyPoint point, out FlightVector flightVector)
        {
            throw new NotImplementedException();
        }
    }

}
