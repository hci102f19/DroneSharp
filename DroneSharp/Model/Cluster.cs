using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using RoadTracer;
namespace DroneSharp.Model
{
    public class Cluster 
    {
        public string FindMiddleOfImg(Mat img)
        {
            RoadTracer.Operator op = new Operator(640,480, 280);
            if (img == null)
            {
                return "null";
            }
            return op.FindMiddleByImg(Image<Bgr, byte>.FromIplImagePtr(img.DataPointer));
        }
    }
}
