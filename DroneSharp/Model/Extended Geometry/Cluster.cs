﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
namespace DroneSharp.Model
{
    public class Cluster 
    {
        public Cluster()
        {
            Random r = new Random();
            Color = new MCvScalar(r.Next(0,255),r.Next(0,255),r.Next(0,255));
        }

        public int Clustersize { get; private set; } = 0;
        public List<MyPoint> Points { get; set; }
        public int Border { get; set; } = 1;
        public MCvScalar Color { get; private set; } 
        public int ClusterDensity { get; private set; }

        public void Add(MyPoint point)
        {
            Points.Add(point);
            Clustersize++;
        }

        public int Min(List<float> vals)
        {
            return (int) Math.Round((double) vals.Min()-(double)Border / 2, 0);
        }

        public int Max(List<float> vals)
        {
            return (int)Math.Round((double)vals.Max() -(double)Border / 2, 0);

        }

        public void Render(Mat image)
        {
            List<MyPoint> copyPoints = new List<MyPoint>();
            Points.CopyTo(copyPoints.ToArray());
            List<float> xVals = new List<float>();
            List<float> yVals = new List<float>();
            foreach (var point in copyPoints)
            {
                xVals.Add(point.X);
                yVals.Add(point.Y);
            }

            var xMin = Min(xVals);
            var xMax = Max(xVals);
            var yMin = Min(yVals);
            var yMax = Max(yVals);

            foreach (var point in Points)
            {
                point.Render(image);
            }
            CvInvoke.Rectangle(image,new Rectangle(xMin,yMin,xMax,yMax),Color,Border);
        }

        public void Density()
        {
            if (Clustersize == 0)
            {
                List<MyPoint> copyPoints = new List<MyPoint>();
                Points.CopyTo(copyPoints.ToArray());
                List<float> xVals = new List<float>();
                List<float> yVals = new List<float>();
                foreach (var point in copyPoints)
                {
                    xVals.Add(point.X);
                    yVals.Add(point.Y);
                }

                var xMin = Min(xVals);
                var xMax = Max(xVals);
                var yMin = Min(yVals);
                var yMax = Max(yVals);

                var box = new Box(xMin,yMin,xMax,yMax);

                if (box.Area() > 0)
                {
                    ClusterDensity = Clustersize * 2 / box.Area();
                }
                else
                {
                    ClusterDensity = 0;
                }
            }
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

        public Box GetCorners()
        {
            List<MyPoint> copyPoints = new List<MyPoint>();
            Points.CopyTo(copyPoints.ToArray());

            List<float> xVals = new List<float>();
            List<float> yVals = new List<float>();
            foreach (var point in copyPoints)
            {
                xVals.Add(point.X);
                yVals.Add(point.Y);
            }
            var minX = Min(xVals);
            var maxX = Max(xVals);
            var minY = Min(yVals);
            var maxY = Max(yVals);
            return new Box(minX, minY, maxX, maxY);
        }



    }
}
