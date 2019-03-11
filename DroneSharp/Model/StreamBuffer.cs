using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emgu.CV;

namespace DroneSharp.Model
{
    public static class StreamBuffer
    {
        public static VideoCapture Stream { get; set; }
        public static Mat CurrentFrame { get; set; } = null;
        public static bool IsRunning { get; set; } = false;
        public static Size FrameSize { get; set; }
        public static int Blur { get; set; }

        public static Mat GetCurrentFrame()
        {
            if (CurrentFrame != null)
            {
                Mat frame = new Mat();
                CurrentFrame.CopyTo(frame);
                return frame;
            }

            return null;
        }

        public static void Run()
        {
            IsRunning = true;
            while (!Stream.IsOpened)
            {
                Console.WriteLine("Waiting for stream to open...");
                Thread.Yield();
            }
            Mat image = Stream.QueryFrame();
            while (image != null && IsRunning)
            {
                Mat resize = new Mat();
                using (image)
                {
                    CvInvoke.Resize(image, resize, FrameSize);
                }
                Mat resizePlusBlur = new Mat();
                using (resize)
                {
                    CvInvoke.GaussianBlur(image, resizePlusBlur, new Size(Blur, Blur), 0);
                }
                using (resizePlusBlur)
                {
                    if (CurrentFrame != null)
                    {
                        lock (CurrentFrame)
                        {
                            resizePlusBlur.CopyTo(CurrentFrame);
                        }
                    }
                    else
                    {
                        CurrentFrame = new Mat();
                    }
                }
                image = Stream.QueryFrame();
            }
            IsRunning = false;
            CurrentFrame = null;
        }

        private static void Kill()
        {
            IsRunning = false;
        }
    }
}
