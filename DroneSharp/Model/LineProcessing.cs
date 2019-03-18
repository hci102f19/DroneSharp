using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math.Geometry;
using DroneSharp.Model.Containers;
using DroneSharp.Model.DBSCAN;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using HdbscanSharp.Distance;
using HdbscanSharp.Hdbscanstar;
using HdbscanSharp.Runner;
namespace DroneSharp.Model
{
    public class LineProcessing
    {
        private const double PiCircm = Math.PI / 180;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="threshold1">The value of threshold 1</param>
        /// <param name="threshold2">The value of threshold 2</param>
        public LineProcessing(int threshold1, int threshold2)
        {
            Threshold1 = threshold1;
            Threshold2 = threshold2;
        }

        /// <summary>
        /// Default constructor for LineProcessing
        /// Initializes Threshold values as 50
        /// </summary>
        public LineProcessing()
        {
            Threshold1 = 50;
            Threshold2 = 50;
        }

        private int Threshold1 { get; set; }
        private int Threshold2 { get; set; }
        public int Framecount { get; set; } = -1;
        public int ThetaModifier { get; set; } = 5;
        public int Theta { get; set; } = 150;
        public int LineMax { get; set; } = 100;
        public int LastTime { get; set; } = Environment.TickCount;
        public List<Line> LinesList { get; set; }

        /// <summary>
        /// Processes an image from the framebuffer
        /// </summary>
        /// <param name="timeToProcess">The time it took to process the frame</param>
        /// <returns>The processed image</returns>
        public Mat ProcessImage(out long timeToProcess)
        {
            try
            {
                var localframe = FrameBuffer.GetCurrentFrame();
                Stopwatch sw = new Stopwatch();
                sw.Start();
                if (localframe != null && !localframe.IsEmpty)
                {
                    Mat edges = new Mat();
                    using (localframe)
                    {
                        CvInvoke.Canny(localframe, edges, Threshold1, Threshold2 * 3);
                    

                    var vector = new VectorOfPointF();
                    using (edges)
                    {
                        CvInvoke.HoughLines(edges, vector, 2, PiCircm, Theta);
                        if (edges != null && !edges.IsEmpty)
                        {
                            if (CalculateTheta(vector))
                            {
                                Console.WriteLine("Too many lines");
                                timeToProcess = sw.ElapsedMilliseconds;
                                return null;
                            }

                            using (vector)
                            {
                                LinesList = GetLines(vector);
                                var clustering = Clustering();
                                var clusterColl = new ClusterContainer();
                                foreach (var cluster in clustering)
                                {
                                    Cluster localCluster = new Cluster {Points = cluster.ToList()};
                                    clusterColl.Add(localCluster);
                                }
                                var best = clusterColl.BestClusterAsPoint();
                                var filteredClusters = FilterCluster(best);
                                Mat clusters = new Mat();
                                clusters = DrawClusters(clusterColl, localframe);
                                timeToProcess = sw.ElapsedMilliseconds;
                                Mat frame = new Mat();
                                clusters.CopyTo(frame);
                                return frame;
                            }
                        }
                        else
                        {
                            throw new ArgumentNullException("Edges was returned as null");

                        }
                        }
                        
                    }
                }
                else
                {
                    throw new ArgumentNullException("Image has not been initialized");
                }
            }
            catch (AccessViolationException)
            {
                Console.WriteLine("Processimage");
                timeToProcess = 0;
                return null;
            }
        }

        /// <summary>
        /// Draws the lines on the image send at parameter and returns it
        /// </summary>
        /// <param name="vectorOfPoints">Vector of the points to draw on the image</param>
        /// <param name="image">Image to draw on</param>
        /// <returns></returns>
        private Mat DrawLines(VectorOfPointF vectorOfPoints, Mat image)
        {
            for (int i = 0; i < vectorOfPoints.Size; i++)
            {
                var vrho = vectorOfPoints[i].X;
                var vtheta = vectorOfPoints[i].Y;

                var a = Math.Cos(vtheta);
                var b = Math.Sin(vtheta);

                var x0 = a * vrho;
                var y0 = b * vrho;

                var pt1 = new Point
                {
                    X = (int)Math.Round(x0 + 1000 * (-b)),
                    Y = (int)Math.Round(y0 + 1000 * (a))
                };

                var pt2 = new Point
                {
                    X = (int)Math.Round(x0 - 1000 * (-b)),
                    Y = (int)Math.Round(y0 - 1000 * (a))
                };

                CvInvoke.Line(image, pt1, pt2, new MCvScalar(0, 0, 255), 2);
            }

            return image;
        }

        private List<Line> GetLines(VectorOfPointF vectorOfPoints)
        {
            List<Line> lines = new List<Line>();
            for (int i = 0; i < vectorOfPoints.Size; i++)
            {
                var vrho = vectorOfPoints[i].X;
                var vtheta = vectorOfPoints[i].Y;

                var a = Math.Cos(vtheta);
                var b = Math.Sin(vtheta);

                var x0 = a * vrho;
                var y0 = b * vrho;

                var pt1 = new MyPoint
                {
                    X = (int)Math.Round(x0 + 1000 * (-b)),
                    Y = (int)Math.Round(y0 + 1000 * (a))
                };

                var pt2 = new MyPoint
                {
                    X = (int)Math.Round(x0 - 1000 * (-b)),
                    Y = (int)Math.Round(y0 - 1000 * (a))
                };
                Line line = new Line(pt1,pt2);
                lines.Add(line);
            }
            return lines;
        }
        /// <summary>
        /// Modifies the theta value according to the number of lines in an image
        /// </summary>
        /// <param name="lines">A vector of lines in image</param>
        /// <returns>A boolean value indicating if number of lines is larger than a specified line max</returns>
        private bool CalculateTheta(VectorOfPointF lines)
        {
            var modifier = 0;
            if (Framecount != -1 && lines.Size != 0)
            {
                modifier = Framecount / lines.Size;
                if (modifier <= 0)
                {
                    modifier = 1;
                }
            }
            else
            {
                modifier = 1;
            }

            var timestamp = Environment.TickCount;
            if (timestamp - LastTime > 1/100)
            {
                Console.WriteLine("Too slow, increasing theta to: " + Theta);
                Theta += (int)Math.Round((double)(ThetaModifier * 0.5), 0);
            }
            else if (lines.Size < 10 && Theta > ThetaModifier)
            {
                Theta -= (int)Math.Round((double) (ThetaModifier * modifier), 0);
                Console.WriteLine("Too much data, decreasing theta to: " + Theta);
            }
            else if(lines.Size > 50)
            {
                Theta += (int) Math.Round((double) (ThetaModifier * modifier), 0);
                Console.WriteLine("Not enough data, increasing theta to: " + Theta);
            }
            Framecount = lines.Size;
            LastTime = timestamp;
            return LineMax < lines.Size;
        }

        public HashSet<MyPoint[]> Clustering()
        {
            var intersections = GetIntersections();
            var dbscan = new DbscanAlgorithm<MyPoint>((x, y) =>
                Math.Sqrt(((x.X - y.X) * (x.X - y.X)) + ((x.Y - y.Y) * (x.Y - y.Y))));
            var minSamples = (Math.Round(intersections.Count * 0.05, 0));
            dbscan.ComputeClusterDbscan(intersections.ToArray(), 20,(int) minSamples,out var clusterRes);
            return clusterRes;
        }

        private List<MyPoint> GetIntersections()
        {
            List<MyPoint> intersections = new List<MyPoint>();
            foreach (var line in LinesList)
            {
                foreach (var innerLine in LinesList)
                {
                    if (line.Equals(innerLine)) continue;
                    
                    var pointIntersect = line.GetIntersectionWith(innerLine);
                    if (pointIntersect != null)
                    {
                        intersections.Add(pointIntersect);
                    }
                }
            }

            return intersections;
        }

        public Mat DrawClusters(ClusterContainer clusterColl, Mat image)
        {
            var bestClusterPoint = clusterColl.BestClusterAsPoint();

            foreach (var cluster in clusterColl.Clusters)
            {
                var randomColor = GetRandomColor();
                var color = new MCvScalar(randomColor.R,randomColor.G,randomColor.B);
                foreach (var point in cluster.Points)
                {
                    CvInvoke.Circle(image, point.ToPoint(),1, color);
                }
            }

            if (bestClusterPoint != null)
                CvInvoke.Circle(image, bestClusterPoint.ToPoint(), 5, new MCvScalar(0, 255, 0), 5);
            return image;
        }

        private Color GetRandomColor()
        {
            Random rnd = new Random();
            Byte[] b = new Byte[3];
            rnd.NextBytes(b);
            Color color = Color.FromArgb(b[0], b[1], b[2]);
            return color;
        }

        public Sfiltering FilterCluster(MyPoint clusters)
        {
            Sfiltering filter = new Sfiltering(6, 0.1f);
            filter.Add(clusters);
            return filter;
        }

    }
}
