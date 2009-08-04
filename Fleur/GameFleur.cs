using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Shaper.Fleur
{
    public class GameFleur : GameBase
    {
        private Texture2D circleTx;
        private Texture2D fleur0Tx;

        private Random rnd;
        private int nbClick = 0;
        private List<Circle> listCircle = new List<Circle>();
        private List<Fleur> listFleur = new List<Fleur>();

        private List<Color> listColor = new List<Color>();
        private List<Color> listColorFlower = new List<Color>();

        ClickableImage txtMenu;

        public GameFleur(GameMain game, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, ContentManager contentManager)
            : base(game, spriteBatch, graphicsDevice, contentManager)
        {
            this.Game = game;

            Init();
        }

        #region Initialization
        private void Init()
        {
            this.MouseLeftButtonClicked += new MouseLeftButtonClickedHandler(GameFleur_MouseLeftButtonClicked);
            this.KeyPressed += new KeyPressedHandler(GameFleur_KeyPressed);
            this.MenuAnimationOpenEnded += new MenuAnimationOpenEndedHandler(GameFleur_MenuAnimationOpenEnded);

            //---
            txtMenu = new ClickableImage(ContentManager.Load<Texture2D>(@"Content\Pic\Menu_On"), ContentManager.Load<Texture2D>(@"Content\Pic\Menu_Off"), new Vector2(50, 18));
            txtMenu.ClickImage += new ClickableImage.ClickImageHandler(txtMenu_ClickImage);
            this.AddClickableImage(txtMenu);
            //---

            //--- Chargement des textures
            circleTx = ContentManager.Load<Texture2D>(@"Content\Pic\Bulle");
            fleur0Tx = ContentManager.Load<Texture2D>(@"Content\Pic\Fleur1");
            //---

            //---
            rnd = new Random();
            listFleur = new List<Fleur>();
            this.ShowMiniMenu = true;
            //---

            //---
            listColor.Add(Color.Magenta);
            listColor.Add(Color.Yellow);
            listColor.Add(Color.Cyan);
            //---

            listColorFlower.Add(listColor[0]);
            listColorFlower.Add(Color.Lerp(listColor[0], listColor[1], 0.5f));
            listColorFlower.Add(listColor[1]);
            listColorFlower.Add(Color.Lerp(listColor[1], listColor[2], 0.5f));
            listColorFlower.Add(listColor[2]);

            //--- Initialisation des fleurs
            int nbFleur = 10;

            for (int i = 0; i < nbFleur; i++)
            {
                float posX = (float)fleur0Tx.Width + ((float)this.GraphicsDevice.Viewport.Width - 2f * (float)fleur0Tx.Width) * (float)rnd.NextDouble();
                float posY = (float)fleur0Tx.Height + ((float)this.GraphicsDevice.Viewport.Height - 2f * (float)fleur0Tx.Height) * (float)rnd.NextDouble();

                Fleur fleur = new Fleur();
                fleur.Position = new Vector2(posX, posY);
                fleur.Couleur = listColorFlower[rnd.Next(0, listColorFlower.Count)];
                fleur.Visible = true;

                listFleur.Add(fleur);
            }
            //---

            base.Init();
        }

        void GameFleur_MenuAnimationOpenEnded(GameTime gameTime)
        {
            this.Game.GameCurrent = new GameMenu(this.Game, this.SpriteBatch, this.GraphicsDevice, this.ContentManager);
        }
        #endregion

        #region Évènements
        void GameFleur_KeyPressed(Keys key, GameTime gameTime)
        {
        }

        void GameFleur_MouseLeftButtonClicked(MouseState mouseState, GameTime gameTime)
        {
            nbClick++;

            //--- Création du cercle
            Circle circle = new Circle { Position = new Vector2(mouseState.X, mouseState.Y), Life = 100, Color = listColor[nbClick % listColor.Count] };
            listCircle.Add(circle);
            //---
        }

        void txtMenu_ClickImage(ClickableImage image, MouseState mouseState, GameTime gameTime)
        {
            this.StartMenuOn(gameTime);
        }
        #endregion

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            UpdateCircles(gameTime);

            CheckFlowerCircle(gameTime);

            base.Update(gameTime);
        }

        private void UpdateCircles(GameTime gameTime)
        {
            foreach (Circle circle in listCircle)
            {
                if (circle.Size(gameTime) >= 1f)
                {
                    circle.Life = 0;
                }
            }

            listCircle.RemoveAll(c => c.Life == 0);
        }

        private void CheckFlowerCircle(GameTime gameTime)
        {
            foreach (Fleur fleur in listFleur)
            {
                if (fleur.Visible && !fleur.Catched)
                {
                    List<Color> listColorOnFlower = new List<Color>();
                    float r = 0f;
                    float g = 0f;
                    float b = 0f;
                    int nbColorOnFlower = 0;

                    foreach (Circle circle in listCircle)
                    {
                        float distance = Vector2.Distance(circle.Position, fleur.Position);

                        if (distance <= circle.Size(gameTime) * (float)circleTx.Width / 2f)
                        {
                            nbColorOnFlower++;

                            r += (float)circle.Color.R;
                            g += (float)circle.Color.G;
                            b += (float)circle.Color.B;
                        }
                    }

                    if (nbColorOnFlower > 0)
                    {
                        r /= nbColorOnFlower;
                        g /= nbColorOnFlower;
                        b /= nbColorOnFlower;

                        Color colorOnFlower = new Color((byte)r, (byte)g, (byte)b);

                        fleur.Catched = (fleur.Couleur == colorOnFlower);
                    }
                }

                if (fleur.Catched && fleur.Alpha(gameTime) == 0)
                {
                    fleur.Visible = false;
                }
            }
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(255, 255, 255));

            SpriteBatch.Begin();

            foreach (Circle circle in listCircle)
            {
                SpriteBatch.Draw(circleTx, circle.Position, null, circle.Color, 0f, new Vector2(circleTx.Width, circleTx.Height) / 2, circle.Size(gameTime), SpriteEffects.None, 0f);
            }

            foreach (Fleur fleur in listFleur)
            {
                if (fleur.Visible)
                    SpriteBatch.Draw(fleur0Tx, fleur.Position, null, new Microsoft.Xna.Framework.Graphics.Color(fleur.Couleur, fleur.Alpha(gameTime)), fleur.Rotation(gameTime), new Vector2(fleur0Tx.Width / 2, fleur0Tx.Height / 2), fleur.Size(gameTime), SpriteEffects.None, 1f);
            }

            base.Draw(gameTime);

            SpriteBatch.End();
        }
    }
}
