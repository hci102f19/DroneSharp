using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneSharp.Model.Geometry
{
    public class Hitbox : Box
    {
        public Hitbox(int x1, int y1, int x2, int y2) : base(x1, y1, x2, y2)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
        }
    }
}
