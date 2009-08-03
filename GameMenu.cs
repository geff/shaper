using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Shaper.Bloc;
using Shaper.Nature;
using Shaper.Triangle;
using Shaper.Etang;
using Shaper.Fleur;

namespace Shaper
{
    public class GameMenu : GameBase
    {
        private ClickableImage btnBloc;
        private ClickableImage btnNature;
        private ClickableImage btnSphere;
        private ClickableImage btnTriangle;
        private ClickableImage btnTitle;

        delegate void MenuItemDelegate();
        MenuItemDelegate currentMenuItem = null;

        public GameMenu(GameMain game, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, ContentManager contentManager) : base(game, spriteBatch, graphicsDevice, contentManager)
        {
            this.Game = game;

            Init();
        }

        #region Initialization
        public override void Init()
        {
            this.MenuAnimationCloseEnded += new MenuAnimationCloseEndedHandler(GameMenu_MenuAnimationCloseEnded);

            btnTitle = new ClickableImage(
                ContentManager.Load<Texture2D>(@"Content\Pic\ShaperTitle_On"),
                ContentManager.Load<Texture2D>(@"Content\Pic\ShaperTitle_Off"),
                new Microsoft.Xna.Framework.Vector2(50, GraphicsDevice.Viewport.Height / 4 + 35));

            //btnBloc = new ClickableImage(
            //    ContentManager.Load<Texture2D>(@"Content\Pic\Bloc_On"),
            //    ContentManager.Load<Texture2D>(@"Content\Pic\Bloc_Off"),
            //    new Microsoft.Xna.Framework.Vector2(300, GraphicsDevice.Viewport.Height / 3 + 145));

            //btnBloc.ClickImage += new ClickableImage.ClickImageHandler(btnBloc_ClickImage);

            //btnNature = new ClickableImage(
            //    ContentManager.Load<Texture2D>(@"Content\Pic\Nature_On"),
            //    ContentManager.Load<Texture2D>(@"Content\Pic\Nature_Off"),
            //    new Microsoft.Xna.Framework.Vector2(300, GraphicsDevice.Viewport.Height / 3 + 195));

            //btnNature.ClickImage += new ClickableImage.ClickImageHandler(btnNature_ClickImage);

            //btnSphere = new ClickableImage(
            //    ContentManager.Load<Texture2D>(@"Content\Pic\Sphere_On"),
            //    ContentManager.Load<Texture2D>(@"Content\Pic\Sphere_Off"),
            //    new Microsoft.Xna.Framework.Vector2(300, GraphicsDevice.Viewport.Height / 3 + 245));

            //btnSphere.ClickImage += new ClickableImage.ClickImageHandler(btnSphere_ClickImage);

            //btnTriangle = new ClickableImage(
            //    ContentManager.Load<Texture2D>(@"Content\Pic\Triangle_On"),
            //    ContentManager.Load<Texture2D>(@"Content\Pic\Triangle_Off"),
            //    new Microsoft.Xna.Framework.Vector2(300, GraphicsDevice.Viewport.Height / 3 + 295));

            //btnTriangle.ClickImage += new ClickableImage.ClickImageHandler(btnTriangle_ClickImage);

            this.AddClickableImage(btnTitle);
            //this.AddClickableImage(btnBloc);
            //this.AddClickableImage(btnNature);
            ////this.AddClickableImage(btnSphere);
            //this.AddClickableImage(btnTriangle);

            ClickableText txtBloc = new ClickableText(this, "Font0", "Font1", "Bloc", new Microsoft.Xna.Framework.Vector2(300, GraphicsDevice.Viewport.Height / 4 + 280));
            txtBloc.ClickText += new ClickableText.ClickTextHandler(txtBloc_ClickText);
            ClickableText txtTriangle = new ClickableText(this, "Font0", "Font1", "Triangle", new Microsoft.Xna.Framework.Vector2(500, GraphicsDevice.Viewport.Height / 4 + 130));
            txtTriangle.ClickText += new ClickableText.ClickTextHandler(txtTriangle_ClickText); 
            ClickableText txtNature = new ClickableText(this, "Font0", "Font1", "Nature", new Microsoft.Xna.Framework.Vector2(300, GraphicsDevice.Viewport.Height / 4 + 130));
            txtNature.ClickText += new ClickableText.ClickTextHandler(txtNature_ClickText);
            ClickableText txtFleur = new ClickableText(this, "Font0", "Font1", "Fleur", new Microsoft.Xna.Framework.Vector2(300, GraphicsDevice.Viewport.Height / 4 + 180));
            txtFleur.ClickText += new ClickableText.ClickTextHandler(txtFleur_ClickText);
            ClickableText txtEtang = new ClickableText(this, "Font0", "Font1", "Etang", new Microsoft.Xna.Framework.Vector2(300, GraphicsDevice.Viewport.Height / 4 + 230));
            txtEtang.ClickText += new ClickableText.ClickTextHandler(txtEtang_ClickText);
            
            this.AddClickableText(txtBloc);
            this.AddClickableText(txtNature);
            this.AddClickableText(txtTriangle);
            this.AddClickableText(txtEtang);
            this.AddClickableText(txtFleur);

            base.Init(true);
        }

        void GameMenu_MenuAnimationCloseEnded()
        {
            currentMenuItem();
        }
        #endregion

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);
        }

        private void ShowBlocOnePlayerGame()
        {
            Game.GameCurrent = new GameBlocOnePlayer(this.Game, this.SpriteBatch, this.GraphicsDevice, this.ContentManager);
        }

        private void ShowNatureOnePlayerGame()
        {
            Game.GameCurrent = new GameNatureOnePlayer(this.Game, this.SpriteBatch, this.GraphicsDevice, this.ContentManager);
        }

        private void ShowTriangleOnePlayerGame()
        {
            Game.GameCurrent = new GameTriangleOnePlayer(this.Game, this.SpriteBatch, this.GraphicsDevice, this.ContentManager);
        }

        private void ShowEtangGame()
        {
            Game.GameCurrent = new GameEtang(this.Game, this.SpriteBatch, this.GraphicsDevice, this.ContentManager);
        }

        private void ShowFleurGame()
        {
            Game.GameCurrent = new GameFleur(this.Game, this.SpriteBatch, this.GraphicsDevice, this.ContentManager);
        }


        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(25, 25, 30));

            SpriteBatch.Begin();

            base.Draw(gameTime);

            SpriteBatch.End();
        }

        #region Evènements
        void txtFleur_ClickText(ClickableText clickableText, Microsoft.Xna.Framework.Input.MouseState mouseState)
        {
            this.StartMenuOff(DateTime.Now.TimeOfDay);
            currentMenuItem = ShowFleurGame;
        }

        void txtEtang_ClickText(ClickableText clickableText, Microsoft.Xna.Framework.Input.MouseState mouseState)
        {
            this.StartMenuOff(DateTime.Now.TimeOfDay);
            currentMenuItem = ShowEtangGame;
        }

        void txtTriangle_ClickText(ClickableText clickableText, Microsoft.Xna.Framework.Input.MouseState mouseState)
        {
            this.StartMenuOff(DateTime.Now.TimeOfDay);
            currentMenuItem = ShowTriangleOnePlayerGame;
        }

        void txtNature_ClickText(ClickableText clickableText, Microsoft.Xna.Framework.Input.MouseState mouseState)
        {
            this.StartMenuOff(DateTime.Now.TimeOfDay);
            currentMenuItem = ShowNatureOnePlayerGame;
        }

        void txtBloc_ClickText(ClickableText clickableText, Microsoft.Xna.Framework.Input.MouseState mouseState)
        {
            this.StartMenuOff(DateTime.Now.TimeOfDay);
            currentMenuItem = ShowBlocOnePlayerGame;
        }
        #endregion
    }
}
