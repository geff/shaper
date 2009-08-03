using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shaper.Triangle
{
    public class Grid
    {
        public Triangle[,] Values { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        private Color[] colors = new Color[] { Color.Red, Color.Yellow, Color.Cyan, Color.LightPink, Color.LightGreen };

        public Grid(int width, int height)
        {
            this.Width = width;
            this.Height= height;

            Init();
        }

        private void Init()
        {
            this.Values = new Triangle[this.Width, this.Height];
            Random rnd = new Random();

            for (int i = 0; i < this.Width; i++)
            {
                for (int j = 0; j < this.Height; j++)
                {
                    this.Values[i, j] = new Triangle(i,j, colors[rnd.Next(0, colors.Length)]);

                    //this.Values[i, j].Angle =(float)(rnd.NextDouble() * MathHelper.Pi);

                    this.Values[i, j].Angle = MathHelper.Pi;
                }
            }
        }
    }
}
