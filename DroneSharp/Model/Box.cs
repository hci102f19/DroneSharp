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
        }

        public int X1 { get; set; }
        public int Y1 { get; set; }
        public int X2 { get; set; }
        public int Y2 { get; set; }

        public void Render(Mat image, MyPoint point = null)
        {
            var rect = Rectangle.FromLTRB(X1, Y1, X2, Y2);
            if (point == null)//todo intersection?
            {
                CvInvoke.Rectangle(image, rect, new MCvScalar(0, 255, 0), 3);
            }
            else
            {
                CvInvoke.Rectangle(image, rect, new MCvScalar(0, 0, 255), 3);
            }
        }
    }

}
