using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DroneSharp.Model.Flight;

namespace DroneSharp.Model.Geometry
{
    public class ReverseHitbox : Box
    {
        public ReverseHitbox(int x1, int y1, int x2, int y2) : base(x1, y1, x2, y2)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
        }

        public override bool Hit(MyPoint point, out FlightVector flightVector)
        {
            flightVector = new FlightVector(0,0,0,0,0);
            if (Rectangle.IntersectsWith(new Rectangle(point.ToPoint(), new Size(1, 1))))
            {
                flightVector = null;
                return false;
            }

            if (!(XMin <= point.X) || !(point.X <= XMax))
            {
                flightVector.Yaw = (int)CalcHorizontal(point);
            }

            if (!(YMin <= point.Y) || !(point.Y <= YMax))
            {
                flightVector.Gaz = (int)CalcVertical(point);
            }

            return true;
        }

        public float CalcHorizontal(MyPoint point)
        {
            return Clamp(point.X - Center.X / Center.X * 100, 100, -100) * -1;
        }

        public float CalcVertical(MyPoint point)
        {
            return Clamp(point.Y - Center.Y / Center.Y * 100, 100, -100);

        }

        public float Clamp(float input, int max, int min)
        {
            if (input > max)
            {
                return max;
            }

            if (input < min)
            {
                return min;
            }

            return input;
        }

    }
}
