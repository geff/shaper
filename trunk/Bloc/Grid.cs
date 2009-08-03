using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shaper.Bloc
{
    public class Grid
    {
        public int[,] Values { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Grid(int width, int height)
        {
            this.Width = width;
            this.Height= height;

            Init();
        }

        private void Init()
        {
            this.Values = new int[this.Width, this.Height];

            for (int i = 0; i < this.Width; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    this.Values[i, j] = 1;
                }
            }
        }
    }
}
