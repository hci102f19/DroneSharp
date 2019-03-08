using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emgu.CV;

namespace DroneSharp.Model
{
    public static class FrameBuffer
    {
        public static VideoCapture Stream { get; set; }
        public static double FPS { get; set; }
        public static Mat CurrentFrame { get; set; } = null;
        public static bool IsRunning { get; set; } = false;
        public static Size FrameSize { get; set; }
        public static double Overflow { get; set; }
        public static int SleepTimer { get; set; }

        public static Mat GetCurrentFrame()
        {
            if (CurrentFrame != null)
            {
                lock (CurrentFrame)
                {
                    return CurrentFrame;
                } 
            }

            return null;
        }

        /// <summary>
        /// Sleep in a specific time based on execution time
        /// 0.002 -> Magic value
        /// </summary>
        /// <param name="exectime">Last execution time</param>
        private static void Sleep(double exectime)
        {
            exectime = exectime + Overflow + 0.002;
            if (exectime >= SleepTimer)
            {
                Overflow = exectime - SleepTimer;
            }
            else
            {
                Overflow = 0;
                Thread.Sleep((int) (SleepTimer - exectime));

            }
        }

        public static void Run()
        {
            IsRunning = true;
            DateTime startTime = DateTime.Now;
            Mat image = Stream.QueryFrame();
            while (image!=null && IsRunning)
            {
                Mat blurredImage = new Mat();
                using (image)
                {
                    CvInvoke.GaussianBlur(image, blurredImage, new Size(3, 3), 0);
                }

                Mat resizePlusBlur = new Mat();
                using (blurredImage)
                {
                    CvInvoke.Resize(blurredImage, resizePlusBlur, new Size(640, 360));
                }

                using (resizePlusBlur)
                {
                    if (CurrentFrame != null)
                    {
                        lock (CurrentFrame)
                        {
                            CurrentFrame = resizePlusBlur;
                        }
                    }
                    else
                    {
                        CurrentFrame = new Mat();
                    }
                }
                image = Stream.QueryFrame();
                var sleeptimer = DateTime.Now - startTime;
                Sleep(sleeptimer.Milliseconds);
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
