using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Shaper.Nature
{
    public class NatureBloc
    {
        public Vector2 Position;
        public TimeSpan StartLife { get; set; }
        public float Speed { get; set; }

        public NatureBloc()
        {
            this.Speed = 0.05f;
        }
    }
}
