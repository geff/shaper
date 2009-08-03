using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Shaper.Drawingsystem
{
    public class QuadRenderer
    {
        static VertexDeclaration vertexDecl = null;
        static VertexPositionTexture[] verts = null;
        static short[] ib = null;
        static Game game;

        public static void Initialize(Game g)
        {
            game = g;

            IGraphicsDeviceService graphicsService = (IGraphicsDeviceService)
                game.Services.GetService(typeof(IGraphicsDeviceService));

            vertexDecl = new VertexDeclaration(graphicsService.GraphicsDevice,
                VertexPositionTexture.VertexElements);

            verts = new VertexPositionTexture[]
            {
                new VertexPositionTexture(
                    new Vector3(0,0,0),
                    new Vector2(1,1)),
                new VertexPositionTexture(
                    new Vector3(0,0,0),
                    new Vector2(0,1)),
                new VertexPositionTexture(
                    new Vector3(0,0,0),
                    new Vector2(0,0)),
                new VertexPositionTexture(
                    new Vector3(0,0,0),
                    new Vector2(1,0))
            };

            ib = new short[] { 0, 1, 2, 2, 3, 0 };
        }

        public static void Render(Vector2 v1, Vector2 v2)
        {
            IGraphicsDeviceService graphicsService = (IGraphicsDeviceService)
                game.Services.GetService(typeof(IGraphicsDeviceService));

            GraphicsDevice device = graphicsService.GraphicsDevice;
            device.VertexDeclaration = vertexDecl;

            verts[0].Position.X = v2.X;
            verts[0].Position.Y = v1.Y;

            verts[1].Position.X = v1.X;
            verts[1].Position.Y = v1.Y;

            verts[2].Position.X = v1.X;
            verts[2].Position.Y = v2.Y;

            verts[3].Position.X = v2.X;
            verts[3].Position.Y = v2.Y;

            device.DrawUserIndexedPrimitives<VertexPositionTexture>
                (PrimitiveType.TriangleList, verts, 0, 4, ib, 0, 2);
        }
    }
}
