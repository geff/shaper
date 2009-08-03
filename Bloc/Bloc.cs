using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Shaper.Bloc
{
    public class Bloc
    {
        public Vector2 Position;
        public TimeSpan StartLife { get; set; }
        public float Speed { get; set; }

        public int[,] Values;

        public Bloc(Vector2 position)
        {
            this.Position = position;
            this.Speed = 0.035f;
            //this.Speed = 0.1f;

            int width = 3;
            int height = 3;
            this.Values = new int[width, height];
        }

        public void Init(Vector2 position, TimeSpan startLife)
        {
            this.Position = position;
            this.StartLife = startLife;

            int width = 3;
            int height = 3;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Values[x, y] = 1;
                }
            }
        }
    }
}
