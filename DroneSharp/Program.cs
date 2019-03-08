using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Superres;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Threading;
using DroneSharp.Model;
using Emgu.CV.Cuda;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
namespace DroneSharp
{
    class Program
    {
        //Time vars
        private static List<double> times = new List<double>();

        static void Main(string[] args)
        {
            ProcessHighPrio();
            int count = 0;
            try
            {
                VideoCapture capture = new VideoCapture(@"C:\Users\bstaf\Downloads/video.v2.mp4");
                FrameBuffer.Stream = capture;
                FrameBuffer.FPS = capture.GetCaptureProperty(CapProp.Fps);
                FrameBuffer.FrameSize = new Size(640, 480);
                Thread framebuffer = new Thread(FrameBuffer.Run);
                framebuffer.Start();
                LineProcessing lineProc = new LineProcessing();
                FrameBuffer.IsRunning = true;
                while (FrameBuffer.IsRunning)
                {
                    var frame = new Mat();
                    frame = FrameBuffer.GetCurrentFrame();
                    if (frame == null) continue;
                    Console.WriteLine("not null");
                    //var points = lineProc.ProcessImage(out var timeToProcess);
                    //if (points == null) continue;
                    //Console.WriteLine(timeToProcess);
                    //CvInvoke.Imwrite("bm/output" + count + ".png", points);
                    //count++;
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
                //Console.WriteLine("Total time: " + times.Sum());
                //Console.WriteLine("Average time: " + times.Average());
                //Console.WriteLine("Min time: " + times.Min());
                //Console.WriteLine("Max time: " + times.Max());
                Console.ReadKey();
            }
        }

        private static void ProcessHighPrio()
        {
            Process p = Process.GetCurrentProcess();
            p.PriorityClass = ProcessPriorityClass.High;
            p.PriorityBoostEnabled = true;
        }
    }
}