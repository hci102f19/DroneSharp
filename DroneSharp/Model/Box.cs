﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;

namespace DroneSharp.Model
{
    public class Box
    {
        public Box(int x1, int y1, int x2, int y2)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
        }

        public int X1 { get; set; }
        public int Y1 { get; set; }
        public int X2 { get; set; }
        public int Y2 { get; set; }

        public void Render(Mat image)
        {
            CvInvoke.Rectangle(image, new Rectangle(X1,Y1,X2,Y2), new MCvScalar(0,255,0),3);
        }
    }

}
