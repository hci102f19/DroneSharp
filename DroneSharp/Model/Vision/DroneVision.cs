using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneSharp.Model.Vision
{
    public class DroneVision
    {
        public LineProcessing Canny { get; set; }
        public FrameBuffer FrameBuffer { get; set; }

    }
}
