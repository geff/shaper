using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shaper.Nature
{
    public class Grid
    {
        public NatureBloc[,] Values { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Grid(int width, int height, int blocWidth, int blocHeight)
        {
            this.Width = width;
            this.Height = height;

            Init(blocWidth, blocHeight);
        }

        private void Init(int blocWidth, int blocHeight)
        {
            this.Values = new NatureBloc[this.Width, this.Height];

            for (int i = 0; i < this.Width; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    this.Values[i, j] = new Earth((float)(i * blocWidth), (float)(j * blocHeight));
                }
            }
        }
    }
}