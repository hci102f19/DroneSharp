using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DroneSharp.Model.Flight;
using DroneSharp.Model.Geometry;
using Emgu.CV;

namespace DroneSharp.Model.Containers
{
    public class BoxContainer
    {
        public BoxContainer(Hitbox leftHitbox, Hitbox rightHitbox, MyPoint xyPoint, ReverseHitbox center)
        {
            LeftHitbox = leftHitbox ?? throw new ArgumentNullException(nameof(leftHitbox));
            RightHitbox = rightHitbox ?? throw new ArgumentNullException(nameof(rightHitbox));
            XyPoint = xyPoint ?? throw new ArgumentNullException(nameof(xyPoint));
            Center = center ?? throw new ArgumentNullException(nameof(center));
            Boxes = new Box[]{LeftHitbox,RightHitbox,Center};
        }

        public float HitboxHeight { get; set; } = 0.7f;
        public float HitBoxWidth { get; set; } = 0.17f;
        public float HorizontalOffset { get; set; } = 0.18f;
        public float VerticalOffset { get; set; } = -0.10f;
        public float HitBoxRotation { get; set; } = 30;
        public float CenterWidth { get; set; } = 0.1f;
        public float CenterHeight { get; set; } = 0.2f;
        public float CenterHeightOffset { get; set; } = 0.25f;
        public Hitbox LeftHitbox { get; set; }
        public Hitbox RightHitbox { get; set; }
        public MyPoint XyPoint { get; set; }
        public ReverseHitbox Center { get; set; }
        public Box[] Boxes { get; set; }

        public FlightVector Hit(MyPoint point)
        {
            FlightVector vector = null;
            foreach (var box in Boxes)
            {
                if (box.Hit(point, out vector)) return vector;
            }
            return vector;
        }

        public void Render(Mat image)
        {
            foreach (var box in Boxes)
            {
                box.Render(image);
            }
        }
    }
}
