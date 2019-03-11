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
using Emgu.CV.UI;
using Emgu.CV.Util;
namespace DroneSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            ProcessHighPrio();
            try
            {
                byte[] p = new byte[] { };
                Thread.Sleep(5000);
                var image = new Image<Rgb, byte>(p);
                StartFromFile();
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
                Console.ReadKey();
            }
        }

        private static void StartFromFile()
        {
            int count = 0;
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
                var frame = FrameBuffer.GetCurrentFrame();
                Cluster cl = new Cluster();

                if (frame == null || frame.IsEmpty) continue;
                using (frame)
                {
                    var points = lineProc.ProcessImage(out var timeToProcess);
                    using (points)
                    {
                        if (points == null || points.IsEmpty) continue;
                        CvInvoke.Imshow("omegalul", points);
                        CvInvoke.WaitKey(1);
                        //CvInvoke.Imwrite("bm/output" + count + ".png", frame);
                    }
                }

                count++;
            }
        }

        private static void StartFromStream()
        {
            VideoCapture cam = new VideoCapture(@"C:\Users\bstaf\Documents\GitHub\DroneVision\ParrotStream/bebop.sdp");
            StreamBuffer.Stream = cam;
            StreamBuffer.Blur = 3;
            StreamBuffer.FrameSize = new Size(640,480);
            Thread streamBuffer = new Thread(StreamBuffer.Run);
            streamBuffer.Start();
            LineProcessing lineProc = new LineProcessing();
            while (StreamBuffer.IsRunning)
            {
                var frame = StreamBuffer.GetCurrentFrame();
                if (frame == null || frame.IsEmpty)continue;
                using (frame)
                {
                    var points = lineProc.ProcessImage(out var timeToProcess);
                    using (points)
                    {
                        if (points == null || points.IsEmpty) continue;
                        CvInvoke.Imshow("omegalul", points);
                        CvInvoke.WaitKey(1);
                        //CvInvoke.Imwrite("bm/output" + count + ".png", frame);
                    }
                }
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