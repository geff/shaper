using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Shaper.Nature
{
    public class Grass : NatureBloc
    {
        public Grass(float posX, float posY)
        {
            this.Speed = 0.05f;
            this.Position = new Vector2(posX, posY);
        }
    }
}
