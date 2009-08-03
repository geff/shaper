using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Shaper.Triangle
{
    public class Triangle
    {
        public int PosX { get; set; }
        public int PosY { get; set; }

        public Boolean Activated { get; set; }
        public Boolean Selected { get; set; }
        public TimeSpan ActivatedTime { get; set; }

        public float Angle { get; set; }

        public Triangle(int posX, int posY)
        {
            this.PosX = posX;
            this.PosY = posY;
        }
    }
}
