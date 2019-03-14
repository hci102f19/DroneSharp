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
            if (0 > point.Y || point.Y > YMax || 0 > point.X || point.X > XMax)
            {
                return;
            }

            if (Points != null)
            {
                Points.Add(point);
                return;
            }

            if (!Deviate(point,Points))
            {
                Points = Points.Skip(HistorySize - 1).ToList();
                Points.Add(point);
                RejectedPoints.Clear();
            }
            else
            {
                RejectedPoints = RejectedPoints.Skip(HistorySize - 1).ToList();
                if (RejectedPoints.Count > HistorySize/2 && !Deviate(point,RejectedPoints))
                {
                    Console.WriteLine("SETTING NEW POINTS LIST!");
                    Points.Clear();
                    foreach (var p in RejectedPoints)
                    {
                        Points.Add(p);
                    }
                    RejectedPoints.Clear();
                }
            }
        }

        public bool Deviate(MyPoint point, List<MyPoint> list)
        {
            var percent = PointToPercent(point);
            foreach (var p in list)
            {
                var lstPercent = PointToPercent(p);
                if (Math.Abs(lstPercent.X-percent.X)>= DeviationMax || Math.Abs(lstPercent.Y - percent.Y) >= DeviationMax)
                {
                    return true;
                }
            }
            return false;
        }

        public MyPoint GetMean()
        {
            var avgPoint = new MyPoint
            {
                X = (int)Math.Round(Points.Average(p => p.X)),
                Y = (int)Math.Round(Points.Average(p => p.Y))
            };
            return avgPoint;
        }

        public MyPoint GetPoint()
        {
            if (Points != null)
            {
                return GetMean();
            }
            else
            {
                return null;
            }
        }
    }
}
