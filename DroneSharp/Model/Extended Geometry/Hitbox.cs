using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DroneSharp.Model.Flight;

namespace DroneSharp.Model.Geometry
{
    public class Hitbox : Box
    {
        public int Force { get; set; } = 100;
        public int Rotation { get; set; }

        public Hitbox(int x1, int y1, int x2, int y2) : base(x1, y1, x2, y2)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;

        }

        public override bool Hit(MyPoint point, out FlightVector flightVector)
        {
            if (Rectangle.IntersectsWith(new Rectangle(point.ToPoint(), new Size(1, 1))))
            {
                flightVector = new FlightVector(1, Force, 0, 0, 0);
                return true;
            }

            flightVector = null;
            return false;
        }
    }
}
