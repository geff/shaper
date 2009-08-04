using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shaper.Tools;

namespace Shaper.Fleur
{
    public class Circle
    {
        public Vector2 Position { get; set; }
        
        public int Life { get; set; }
        public Color Color { get; set; }

        public Boolean Stop { get; set; }
        private Lerp lerpSize;

        public Circle()
        {
            lerpSize = new Lerp(0f, 1f, false, false, 3000, -1);
        }

        public float Size(GameTime gameTime)
        {
            return lerpSize.Eval(gameTime);
        }

        public float Size(GameTime gameTime, bool stop)
        {
            return lerpSize.Eval(gameTime, stop);
        }
    }
}
