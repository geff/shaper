using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Shaper.Triangle
{
    public class Line
    {
        public Vector2 Pos1 { get; set; }
        public Vector2 Pos2 { get; set; }

        public Line(float posX1, float posY1, float posX2, float posY2)
        {
            this.Pos1 = new Vector2(posX1, posY1);
            this.Pos2 = new Vector2(posX2, posY2);
        }

        public Line(float posX1, float posY1)
        {
            this.Pos1 = new Vector2(posX1, posY1);
            this.Pos2 = Vector2.Zero;
        }
    }
}
