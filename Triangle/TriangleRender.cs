using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FarseerGames.GettingStarted.DrawingSystem;
using Shaper.Drawingsystem;
using RoundLineCode;

namespace Shaper.Triangle
{
    public class TriangleRender
    {
        #region Properties
        public GameMain Game { get; set; }
        public SpriteBatch SpriteBatch { get; set; }
        public GraphicsDevice GraphicsDevice { get; set; }
        public ContentManager ContentManager { get; set; }
        public GameTriangleOnePlayer GameTriangleOnePlayer { get; set; }

        private Texture2D pixel;
        //public Texture2D texTriangle;
        //private Texture2D texTriangleActivated;
        private LineBrush lineBrush;

        /// <summary>Matrix used to transform 3D to screen coordinates</summary>
        private Matrix projection;
        /// <summary>Matrix that defines the viewer's location in the scene</summary>
        private Matrix view;
        /// <summary>Matrix for controlling the location of rendered polygons</summary>
        private Matrix world;

        /// <summary>Vertex type that stores a position and color in each vertex</summary>
        private VertexDeclaration positionColorVertexDeclaration;

        /// <summary>Default Vertex- & PixelShader for the graphics card</summary>
        private Effect colorEffect;

        private List<RoundLine> listRoundLines;
        private RoundLineManager roundLineManager;
        Matrix viewMatrix;
        Matrix projMatrix;
        float cameraX = 0;
        float cameraY = 0;
        float cameraZoom = 256;

        #endregion

        public TriangleRender(GameTriangleOnePlayer gameTriangle, GameMain game, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
            this.GameTriangleOnePlayer = gameTriangle;
            this.Game = game;
            this.SpriteBatch = spriteBatch;
            this.GraphicsDevice = graphicsDevice;
            this.ContentManager = contentManager;
        }

        public void Initialize()
        {
            //--- Création des lignes
            roundLineManager = new RoundLineManager();
            listRoundLines = new List<RoundLine>();

            roundLineManager.Init(GraphicsDevice, ContentManager);
            //---

            if (this.Game.Mini)
            {
                //texTriangle = ContentManager.Load<Texture2D>("trianglem");
                //texTriangleActivated = ContentManager.Load<Texture2D>("trianglem_activated");

                GameTriangleOnePlayer.defaultX = 10;
                GameTriangleOnePlayer.defaultY = 33;
                lineBrush = new LineBrush(1, Color.White);
                lineBrush.Load(this.GraphicsDevice);
            }
            else
            {
                //texTriangle = ContentManager.Load<Texture2D>("triangle");
                //texTriangleActivated = ContentManager.Load<Texture2D>("triangle_activated");

                GameTriangleOnePlayer.defaultX = 100;
                GameTriangleOnePlayer.defaultY = 50;
                lineBrush = new LineBrush(1, Color.White);
                lineBrush.Load(this.GraphicsDevice);
            }

            // Create a new perspective projection matrix
            //this.projection = Matrix.CreatePerspectiveFieldOfView(
            //  MathHelper.PiOver4, // field of view
            //  (float)Game.Window.ClientBounds.Width / (float)Game.Window.ClientBounds.Height, // aspect ratio
            //  0.01f, 1000.0f // near and far clipping plane
            //);

            this.world = Matrix.Identity;

            Vector2 screen = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            //this.projection = Matrix.CreateOrthographic(screen.X, screen.Y, 0.001f, 5000f);
            
            //---good
            this.projection = Matrix.CreateOrthographicOffCenter(0f, screen.X, screen.Y, 0f, 0.001f, 5000f);
            this.view = Matrix.CreateLookAt(new Vector3(0f, 0f, 1000.0f), new Vector3(0f, 0f, 0f), new Vector3(0.0f, 1.0f, 0.0f));
            //---

            this.projMatrix = Matrix.CreateOrthographic(screen.X, screen.Y, 0f, 5000f);


            // Declare the vertex format we will use for the rotating triangle's vertices
            this.positionColorVertexDeclaration = new VertexDeclaration(
              this.GraphicsDevice, VertexPositionColor.VertexElements
            );

            // Create a new default effect containing universal
            // vertex and pixel shaders that we can use to draw our triangle
            //this.colorEffect = new BasicEffect(GraphicsDevice, null);
            this.colorEffect = this.ContentManager.Load<Effect>("Content/Shader/ColorFiller");


            //---------
            PresentationParameters pp = GraphicsDevice.PresentationParameters;
            colorBuffer = new ResolveTexture2D(GraphicsDevice,
                                               pp.BackBufferWidth,
                                               pp.BackBufferHeight,
                                               1,
                                               pp.BackBufferFormat);

            r = new RenderTarget2D(GraphicsDevice,
                                               pp.BackBufferWidth,
                                               pp.BackBufferHeight,
                                               1,
                                               pp.BackBufferFormat);



            //monoEffect = new Monochrome(GraphicsDevice);

            Create2DProjectionMatrix();
        }

        /// <summary>
        /// Create a simple 2D projection matrix
        /// </summary>
        public void Create2DProjectionMatrix()
        {
            // Projection matrix ignores Z and just squishes X or Y to balance the upcoming viewport stretch
            float projScaleX;
            float projScaleY;
            float width = GraphicsDevice.Viewport.Width;
            float height = GraphicsDevice.Viewport.Height;
            if (width > height)
            {
                // Wide window
                projScaleX = height / width;
                projScaleY = 1.0f;
            }
            else
            {
                // Tall window
                projScaleX = 1.0f;
                projScaleY = width / height;
            }
            projMatrix = Matrix.CreateScale(projScaleX, projScaleY, 0.0f);
            projMatrix.M43 = 0.5f;
        }

        //Monochrome monoEffect;
        ResolveTexture2D colorBuffer;
        RenderTarget2D r;

        public void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(25, 25, 30));

            this.GraphicsDevice.VertexDeclaration = this.positionColorVertexDeclaration;
            this.GraphicsDevice.RenderState.CullMode = CullMode.None;
            GraphicsDevice.RenderState.AlphaBlendEnable = true;
            GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
            GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
            GraphicsDevice.RenderState.BlendFunction = BlendFunction.Add;

            Rectangle recBackGround = new Rectangle((int)GameTriangleOnePlayer.defaultX, (int)GameTriangleOnePlayer.defaultY, GameTriangleOnePlayer.grid.Width * GameTriangleOnePlayer.TriWidth, GameTriangleOnePlayer.grid.Height * GameTriangleOnePlayer.TriHeight);
            Rectangle recBackGround0 = new Rectangle((int)GameTriangleOnePlayer.defaultX-2, (int)GameTriangleOnePlayer.defaultY-2, GameTriangleOnePlayer.grid.Width * GameTriangleOnePlayer.TriWidth+4, GameTriangleOnePlayer.grid.Height * GameTriangleOnePlayer.TriHeight+4);


            this.colorEffect.Parameters["WorldViewProj"].SetValue(world * view * projection);
            this.colorEffect.Begin();


            //GraphicsDevice.SetRenderTarget(1, r2);



            foreach (EffectPass pass in this.colorEffect.CurrentTechnique.Passes)
            {
                pass.Begin();

                DrawRectangle(recBackGround0, new Color(10, 10, 15));
                DrawRectangle(recBackGround, new Color(15, 15, 20));

                //--- Grid
                GameTriangleOnePlayer.ActionOnGrid(delegate(Triangle triangle)
                {
                    if (triangle.Activated)
                    {
                        this.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                              PrimitiveType.TriangleList,
                              new VertexPositionColor[] {
                                        new VertexPositionColor(new Vector3(triangle.Points[0], 0f), triangle.Color),
                                        new VertexPositionColor(new Vector3(triangle.Points[1], 0f), triangle.Color),
                                        new VertexPositionColor(new Vector3(triangle.Points[2], 0f), triangle.Color)
                                      },
                              0, // vertexOffset
                              1 // primitiveCount
                            );
                    }
                    else
                    {
                        this.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                              PrimitiveType.LineStrip,
                              new VertexPositionColor[] {
                                        new VertexPositionColor(new Vector3(triangle.Points[0], 0f), triangle.Color),
                                        new VertexPositionColor(new Vector3(triangle.Points[1], 0f), triangle.Color),
                                        new VertexPositionColor(new Vector3(triangle.Points[2], 0f), triangle.Color),
                                        new VertexPositionColor(new Vector3(triangle.Points[0], 0f), triangle.Color)
                                      },
                              0, // vertexOffset
                              3 // primitiveCount
                            );
                    }
                });
                //---

                //--- Angle de réflexion
                if (GameTriangleOnePlayer.lineFOI1 != null && GameTriangleOnePlayer.lineFOI2 != null && GameTriangleOnePlayer.currentTriangle != null)
                {
                    Color colorAngle = new Color(GameTriangleOnePlayer.currentTriangle.Color, 0.5f);
                    Color colorAngle2 = Color.TransparentWhite;

                    //colorAngle = Color.TransparentWhite;

                    this.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                              PrimitiveType.TriangleList,
                              new VertexPositionColor[] {
                        new VertexPositionColor(new Vector3(GameTriangleOnePlayer.currentTriangle.PosX,GameTriangleOnePlayer.currentTriangle.PosY, 0f), colorAngle),
                        new VertexPositionColor(new Vector3(GameTriangleOnePlayer.lineFOI1.Pos2, 0f), colorAngle2),
                        new VertexPositionColor(new Vector3(GameTriangleOnePlayer.lineFOI2.Pos2, 0f), colorAngle2),},
                              0, // vertexOffset
                              1 // primitiveCount
                            );
                }
                //---

                pass.End();
            }

            //--- Ligne de réflexion
            Vector2 screen = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            screen /= 2;

            listRoundLines = new List<RoundLine>();

            for (int i = 0; i < GameTriangleOnePlayer.listLine.Count; i++)
            {
                Color colorLine = Color.Lerp(GameTriangleOnePlayer.firstActivatedTriangle.Color, Color.TransparentWhite, (float)i / (float)(GameTriangleOnePlayer.maxStep - 1));

                listRoundLines.Add(new RoundLine(GameTriangleOnePlayer.listLine[i].Pos1.X - screen.X, screen.Y - GameTriangleOnePlayer.listLine[i].Pos1.Y,
                                                 GameTriangleOnePlayer.listLine[i].Pos2.X - screen.X, screen.Y - GameTriangleOnePlayer.listLine[i].Pos2.Y));
            }

            float time = (float)gameTime.TotalRealTime.TotalSeconds;
            //---

            //--- TimeOver Bar
            Rectangle recBarTimeOverParent = new Rectangle((int)GameTriangleOnePlayer.defaultX, (int)GameTriangleOnePlayer.defaultY - 30, GameTriangleOnePlayer.grid.Width * GameTriangleOnePlayer.TriWidth, 20);
            Rectangle recBarTimeOver = new Rectangle((int)GameTriangleOnePlayer.defaultX + 5, (int)GameTriangleOnePlayer.defaultY - 25, (int)((GameTriangleOnePlayer.grid.Width * GameTriangleOnePlayer.TriWidth - 10) * ((float)gameTime.TotalGameTime.Subtract(GameTriangleOnePlayer.latestTimeOver).TotalMilliseconds / (float)GameTriangleOnePlayer.timeOverDuration)), 10);

            DrawRectangle(recBarTimeOverParent, Color.DarkGray);
            DrawRectangle(recBarTimeOver, Color.White);
            //---


            //---
            if (GameTriangleOnePlayer.firstActivatedTriangle != null)
            {
                cameraZoom = GraphicsDevice.Viewport.Height / 2;
                viewMatrix = Matrix.CreateTranslation(-cameraX, -cameraY, 0) *Matrix.CreateScale(1.0f / cameraZoom, 1.0f / cameraZoom, 1.0f);

                Matrix viewProjMatrix = viewMatrix * projMatrix;

                roundLineManager.NumLinesDrawn = 0;

                float lineRadius = Math.Max(1, GameTriangleOnePlayer.TriWidth / 15);
                roundLineManager.BlurThreshold = roundLineManager.ComputeBlurThreshold(lineRadius, viewProjMatrix, GraphicsDevice.PresentationParameters.BackBufferWidth);

                string curTechniqueName = "Glow";

                roundLineManager.Draw(listRoundLines, lineRadius, GameTriangleOnePlayer.firstActivatedTriangle.Color, viewProjMatrix, time, curTechniqueName);
            }
            //---

            this.colorEffect.End();
        }

        private void DrawRectangle(Rectangle rec, Color color)
        {
            this.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                  PrimitiveType.TriangleFan,
                  new VertexPositionColor[] {
                                        new VertexPositionColor(new Vector3(rec.Left, rec.Top, 0f), color),
                                        new VertexPositionColor(new Vector3(rec.Right, rec.Top, 0f), color),
                                        new VertexPositionColor(new Vector3(rec.Right, rec.Bottom, 0f), color),
                                        new VertexPositionColor(new Vector3(rec.Left, rec.Bottom, 0f), color)
                                      },
                  0, // vertexOffset
                  2 // primitiveCount
                );
        }
    }
}
