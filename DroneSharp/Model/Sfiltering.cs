using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneSharp.Model
{
    public class Sfiltering
    {
        public Sfiltering(int historySize, float deviationMax, int xMax = 680, int yMax = 480)
        {
            Points = new List<MyPoint>();
            RejectedPoints =new List<MyPoint>();
            HistorySize = historySize;
            XMax = xMax;
            YMax = yMax;
            DeviationMax = deviationMax;
        }

        public List<MyPoint> Points { get; set; }
        public List<MyPoint> RejectedPoints { get; set; }
        public int HistorySize { get; set; }
        public int XMax { get; set; }
        public int YMax { get; set; }
        public float DeviationMax { get; set; }

        public MyPoint PointToPercent(MyPoint point)
        {
            return new MyPoint(point.X / XMax, point.Y / YMax);
        }

        public void Add(MyPoint point)
        {

        }
    }
}
