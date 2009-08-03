using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Shaper.Color
{
    public class GameColor : GameBase
    {
        public GameMain Game { get; set; }
        public SpriteBatch SpriteBatch { get; set; }
        public GraphicsDevice GraphicsDevice { get; set; }
        public ContentManager ContentManager { get; set; }

        private Texture2D pixel;
        private Texture2D circleTx;

        #region Keys
        private Keys keyTop1 = Keys.NumPad7;
        private Keys keyTop2 = Keys.NumPad8;
        private Keys keyTop3 = Keys.NumPad9;

        private Keys keyLeft = Keys.NumPad4;
        private Keys keyCenter = Keys.NumPad5;
        private Keys keyRight = Keys.NumPad6;

        private Keys keyBottom1 = Keys.NumPad1;
        private Keys keyBottom2 = Keys.NumPad2;
        private Keys keyBottom3 = Keys.NumPad3;

        private bool keyStateTop1;
        private bool keyStateTop2;
        private bool keyStateTop3;

        private bool keyStateLeft;
        private bool keyStateCenter;
        private bool keyStateRight;

        private bool keyStateBottom1;
        private bool keyStateBottom2;
        private bool keyStateBottom3;
        #endregion

        public GameColor(GameMain game, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
            this.Game = game;
            this.SpriteBatch = spriteBatch;
            this.GraphicsDevice = graphicsDevice;
            this.ContentManager = contentManager;

            Init();
        }

        #region Initialization
        private void Init()
        {
            circleTx = ContentManager.Load<Texture2D>("Circle");

            //--- Création du pixel
            pixel = new Texture2D(GraphicsDevice, 1, 1, 1, TextureUsage.Linear, SurfaceFormat.Color);
            pixel.SetData<Color>(new Color[] { Color.White });
            //---
        }
        #endregion

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            UpdateKeyBoard(gameTime);

            UpdateMouse(gameTime);

            UpdateCircles(gameTime);
        }

        private void UpdateKeyBoard(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
        }

        private ButtonState mouseLeftButtonState = ButtonState.Released;

        private void UpdateMouse(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();

            //---> clique de la souris
            if (mouseState.LeftButton == ButtonState.Released && mouseLeftButtonState == ButtonState.Pressed)
            {
            }
            //---

            //---
            mouseLeftButtonState = mouseState.LeftButton;
            //---
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(25, 25, 30));

            SpriteBatch.Begin();

            foreach (Circle circle in listCircle)
            {
                Vector2 pos = circle.Position - new Vector2(circle.Size / 2);
                SpriteBatch.Draw(circleTx, new Rectangle((int)pos.X, (int)pos.Y, circle.Size, circle.Size), Color.Lerp(new Color(25, 25, 30), Color.Azure, (float)circle.Life / 100f));
            }

            SpriteBatch.End();
        }

        private void NewGame()
        {
        }
    }
}
