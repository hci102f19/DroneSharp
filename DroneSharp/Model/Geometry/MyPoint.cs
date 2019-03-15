using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using DroneSharp.Model.DBSCAN;
using Emgu.CV;
using Emgu.CV.Structure;

namespace DroneSharp.Model
{
    public class MyPoint : DatasetItemBase
    {
        public MyPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        public MyPoint()
        {
            
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Threshold { get; set; } = 5;
        public bool IsValid { get; set; } = true;
        public bool IsChecked { get; set; } = false;
        public Cluster Cluster { get; private set; } = null;

        public void SetCluster(Cluster cluster)
        {
            IsChecked = true;
            Cluster = cluster;
            Cluster.Add(this);
        }

        public Point ToPoint()
        {
            return new Point(X,Y);
        }

        public void Render(Mat image)
        {
            if (this.IsValid)
            {
                if (Cluster != null)
                {
                    CvInvoke.Circle(image, ToPoint(), 1, Cluster.Color, -1);
                }
                else
                {
                    CvInvoke.Circle(image, ToPoint(), 5, new MCvScalar(255,255,255), -1);
                    CvInvoke.Circle(image, ToPoint(), 3, new MCvScalar(0, 0, 0), -1);
                }
            }
        }

    }
}
