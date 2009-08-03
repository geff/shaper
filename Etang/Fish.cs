using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Shaper.Etang
{
    public class Fish
    {
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Vector2 Orientation { get; set; }
        public float MaxSpeed { get; set; }
        public Curve CurveLoopX { get; set; }
        public Curve CurveLoopY { get; set; }
    }
}
