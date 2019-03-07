using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Superres;
using System.Drawing;
using System.Numerics;
using Emgu.CV.Cuda;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
namespace DroneSharp
{
    class Program
    {
        private const double theta = Math.PI / 180;

        static void Main(string[] args)
        {
            Process p = Process.GetCurrentProcess();
            p.PriorityClass = ProcessPriorityClass.High;
            p.PriorityBoostEnabled = true;
            Console.WriteLine("Thread info:");
            Console.WriteLine(p.BasePriority);
            Console.WriteLine(p.ProcessorAffinity);
            Console.WriteLine(p.Threads.Count);
            int count = 0;
            const double rho = 1;
            const int threshold = 100;
            const int srn = 20;
            List<double> times = new List<double>();
            Stopwatch sw = new Stopwatch();
            try
            {
                VideoCapture _capture = new VideoCapture(@"C:\Users\bstaf\Downloads/video.v2.mp4");
                sw.Start();
                Mat image = _capture.QueryFrame();
                while (image != null)
                {
                    Mat resizedImage = new Mat();
                    using (image)
                    {
                        CvInvoke.Resize(image, resizedImage, new Size(640, 360));
                    }
                    Mat edges = new Mat();
                    using (edges)
                    {
                        CvInvoke.Canny(resizedImage, edges, 100, 200);
                        if (!edges.IsEmpty)
                        {
                            using (resizedImage)
                            {
                                using (var vector = new VectorOfPointF())
                                {
                                    CvInvoke.HoughLines(edges, vector, rho, theta, threshold, srn);
                                    for (int i = 0; i < vector.Size; i++)
                                    {
                                        var vrho = vector[i].X;
                                        var vtheta = vector[i].Y;

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

                                        CvInvoke.Line(resizedImage, pt1, pt2, new MCvScalar(0, 0, 255), 2);
                                    }
                                }
                                CvInvoke.Imwrite("bm/output" + count + ".png", resizedImage);
                                sw.Stop();
                                times.Add(sw.ElapsedMilliseconds / (double)1000);
                                sw.Restart();
                            }
                        } 
                    }
                    count++;
                    image = _capture.QueryFrame();
                }
            }
            catch (NullReferenceException excpt)
            {
                Console.WriteLine(excpt.Message);
            }
            catch (Emgu.CV.Util.CvException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                sw.Stop();
                Console.WriteLine("Total time: " + times.Sum());
                Console.WriteLine("Average time: " + times.Average());
                Console.WriteLine("Min time: " + times.Min());
                Console.WriteLine("Max time: " + times.Max());
                Console.ReadKey();
            }
        }
    }
}