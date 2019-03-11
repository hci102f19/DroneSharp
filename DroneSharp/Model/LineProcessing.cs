using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;

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
                                timeToProcess = sw.ElapsedMilliseconds;
                                if (vector.Size == 0) return null;
                                Mat imageWithLines = new Mat();
                                imageWithLines = DrawLines(vector, localframe);
                                sw.Stop();
                                timeToProcess = sw.ElapsedMilliseconds;
                                Mat frame = new Mat();
                                imageWithLines.CopyTo(frame);
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
            catch (Exception)
            {
                Console.WriteLine("Processimage");
                throw;
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

            Framecount = lines.Size;
            if (lines.Size < 10 && Theta > ThetaModifier)
            {
                Theta -= (int)Math.Round((double) (ThetaModifier * modifier), 0);
                Console.WriteLine("Too much data, decreasing theta to: " + Theta);
            }
            else if(lines.Size > 50)
            {
                Theta += (int) Math.Round((double) (ThetaModifier * modifier), 0);
                Console.WriteLine("Not enough data, increasing theta to: " + Theta);
            }

            return LineMax < lines.Size;
        }

    }
}
