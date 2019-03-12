using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;

namespace DroneSharp.Model
{
    public class Centerbox
    {
        public Centerbox(int width, int height, int widthCenter, int heighOffset)
        {
            Width = width;
            Height = height;
            WidthCenter = widthCenter;
            HeighOffset = heighOffset;
            var x1 = width * ((1 - WidthCenter) / 2);
            var y1 = height * (((1 - HeightCenter) / 2) - heighOffset);
            var x2 = width * (1 - ((1 - WidthCenter) / 2));
            var y2 = height * ((1 - ((1 - HeightCenter) / 2)) - HeighOffset);
            Box = new Box(x1,y1,x2,y2);
        }

        public int Width { get; set; }
        public int Height { get; set; }
        public int HeightCenter { get; set; }
        public int WidthCenter { get; set; }
        public int HeighOffset { get; set; }
        private Box Box { get; set; }

        public void Render(Mat image)
        {
            Box.Render(image);
        }

    }
}
