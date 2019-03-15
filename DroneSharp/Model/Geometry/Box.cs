using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
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
            xMin = Math.Min(x1, x2);
            yMin = Math.Min(y1, y2);
            xMax = Math.Max(x1, x2);
            yMax = Math.Max(y1, y2);


        }

        public int X1 { get; set; }
        public int Y1 { get; set; }
        public int X2 { get; set; }
        public int Y2 { get; set; }
        public int xMin { get; set; }
        public int yMin { get; set; }
        public int xMax { get; set; }
        public int yMax { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public MyPoint Center  { get; set; }
        public Color color { get; set; } = Color.FromArgb(0, 255, 0);
        public Rectangle Rectangle { get; set; }

        public void Render(Mat image, MyPoint point = null)
        {
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
    }

}
