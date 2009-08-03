using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shaper.Tools;

namespace Shaper.Fleur
{
    public class Fleur
    {
        public Vector2 Position { get; set; }
        public Color Couleur { get; set; }
        public Boolean Visible { get; set; }
        public Boolean Catched { get; set; }

        private Lerp lerpRotation;
        private Lerp lerpSize;
        private Lerp lerpAlpha;

        public Fleur()
        {
            lerpRotation = new Lerp(0, MathHelper.TwoPi, true, false, -1, MathHelper.Pi);
            lerpSize = new Lerp(0.3f, 0.5f, false, true, 800, -1 );
            lerpAlpha = new Lerp(255f, 0f, false, false, 500, -1);
        }

        public float Rotation(GameTime gameTime)
        {
            return lerpRotation.Eval(gameTime);
        }

        public float Size(GameTime gameTime)
        {
            return lerpSize.Eval(gameTime);
        }

        public Byte Alpha(GameTime gameTime)
        {
            if (Catched)
            {
                return (byte)lerpAlpha.Eval(gameTime);
            }
            else
            {
                return 255;
            }
        }
    }
}
