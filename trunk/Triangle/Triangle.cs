using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shaper.Triangle
{
    public class Triangle
    {
        public float PosX { get; set; }
        public float PosY { get; set; }

        public int PosGridX { get; set; }
        public int PosGridY { get; set; }

        public int NextPosX { get; set; }
        public int NextPosY { get; set; }

        public int NextPosGridX { get; set; }
        public int NextPosGridY { get; set; }

        public Boolean Activated { get; set; }
        public Boolean Selected { get; set; }
        public TimeSpan ActivatedTime { get; set; }
        public Color Color { get; set; }
        public float Angle { get; set; }
        public float PrevAngle { get; set; }

        public Boolean HitLaser { get; set; }

        public float Speed { get; set; }

        public Vector2[] Points;

        public bool Move { get; set; }

        public Triangle(int posX, int posY, Color color)
        {
            this.PosGridX = posX;
            this.PosGridY = posY;
            this.Color = color;
            this.Speed = 100f;

            this.Points = new Vector2[3];
        }

        public void CalcPosition(float defaultX, float defaultY, float width, float height, float gridWidth, float gridHeight)
        {
            this.PosX = (int)(defaultX + ((float)PosGridX + 0.5f) * width);
            this.PosY = (int)(defaultY + (gridHeight - 1 - (float)PosGridY + 0.5f) * height);
        }

        public void CalcNextPosition(float defaultX, float defaultY, float width, float height, float gridWidth, float gridHeight)
        {
            this.NextPosX = (int)(defaultX + ((float)NextPosGridX + 0.5f) * width);
            this.NextPosY = (int)(defaultY + (gridHeight - 1 - (float)NextPosGridY + 0.5f) * height);
        }

        public void CalcPoints(float triWidth)
        {
            float w = triWidth * 0.45f;

            Points[0] = new Vector2(PosX + w * (float)System.Math.Cos(Angle - MathHelper.PiOver2),
                                 PosY + w * (float)System.Math.Sin(Angle - MathHelper.PiOver2));

            Points[1] = new Vector2(PosX + w * (float)System.Math.Cos(Angle + MathHelper.Pi / 6f),
                                 PosY + w * (float)System.Math.Sin(Angle + MathHelper.Pi / 6f));

            Points[2] = new Vector2(PosX + w * (float)System.Math.Cos(Angle + MathHelper.Pi * 5f / 6f),
                                 PosY + w * (float)System.Math.Sin(Angle + MathHelper.Pi * 5f / 6f));
        }
    }
}
