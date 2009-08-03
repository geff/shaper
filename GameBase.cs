﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Shaper
{
    public class GameBase
    {
        public GameMain Game { get; set; }
        public SpriteBatch SpriteBatch { get; set; }
        public GraphicsDevice GraphicsDevice { get; set; }
        public ContentManager ContentManager { get; set; }
        protected Boolean ShowMiniMenu { get; set; }

        private Texture2D imgCenter;
        private Texture2D imgMenu;
        private Color colorCenter;
        private int mapTransition = 0;
        private TimeSpan initTime = TimeSpan.Zero;
        protected Texture2D pixel;
        private TimeSpan timeStartMenuOn;
        private TimeSpan timeStartMenuOff;
        protected float alphaMenu = 255f;
        private int posMenu = 0;
        private float timeChange = 500f;

        private List<ClickableImage> listClickableImage { get; set; }
        private List<ClickableText> listClickableText { get; set; }

        private List<KeyManager> listKeys { get; set; }

        private ButtonState mouseLeftButtonState { get; set; }
        private ButtonState mouseRightButtonState { get; set; }
        private ButtonState mouseMiddleButtonState { get; set; }

        public delegate void KeyPressedHandler(Keys key);
        public event KeyPressedHandler KeyPressed;

        public delegate void MouseLeftButtonClickedHandler(MouseState mouseState);
        public event MouseLeftButtonClickedHandler MouseLeftButtonClicked;

        public delegate void MouseRightButtonClickedHandler(MouseState mouseState);
        public event MouseRightButtonClickedHandler MouseRightButtonClicked;

        public delegate void MouseMidddleButtonClickedHandler(MouseState mouseState);
        public event MouseMidddleButtonClickedHandler MouseMidddleButtonClicked;

        public delegate void MenuAnimationCloseEndedHandler();
        public event MenuAnimationCloseEndedHandler MenuAnimationCloseEnded;

        public delegate void MenuAnimationOpenEndedHandler();
        public event MenuAnimationOpenEndedHandler MenuAnimationOpenEnded;

        public GameBase(GameMain game, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
            this.Game = game;
            this.SpriteBatch = spriteBatch;
            this.GraphicsDevice = graphicsDevice;
            this.ContentManager = contentManager;

            this.listKeys = new List<KeyManager>();
            this.listClickableImage = new List<ClickableImage>();
            this.listClickableText = new List<ClickableText>();
        }

        public virtual void Init()
        {
            this.Init(false);
        }

        public virtual void Init(bool showMenu)
        {
            //--- Initialisation des variables
            timeStartMenuOn = TimeSpan.Zero;
            timeStartMenuOff = TimeSpan.Zero;
            initTime = TimeSpan.MinValue;

            if(showMenu)
                posMenu = GraphicsDevice.Viewport.Height / 4;
            //---

            //--- Création du pixel
            pixel = new Texture2D(GraphicsDevice, 1, 1, 1, TextureUsage.Linear, SurfaceFormat.Color);
            pixel.SetData<Color>(new Color[] { Color.White });
            //---

            //--- Image centrale pour la transition entre niveaux
            imgCenter = new Texture2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 1, TextureUsage.Linear, SurfaceFormat.Color);
            colorCenter = new Color(0, 0, 0, 0);
            mapTransition = 0;
            //---

            //----
            //int taille = (GraphicsDevice.Viewport.Width) * (GraphicsDevice.Viewport.Height);
            //Color[] data = new Color[taille];

            //for (int i = 0; i < taille; i++)
            //    data[i] = new Color(0, 0, 0, 100);

            //imgCenter.SetData<Color>(data, 0, data.Length, SetDataOptions.Discard);
            //---

            //--- Création du menu
            imgMenu = new Texture2D(GraphicsDevice, GraphicsDevice.Viewport.Width, 50, 1, TextureUsage.Linear, SurfaceFormat.Color);
            //---

            //---
            Color[] data = new Color[imgMenu.Width * imgMenu.Height];

            for (int i = 0; i < imgMenu.Width * imgMenu.Height; i++)
            {
                if (i / imgMenu.Width >= imgMenu.Height - 3)
                    data[i] = new Color(255, 255, 255);
                else
                    data[i] = new Color(0, 0, 0, 100);
            }

            imgMenu.SetData<Color>(data, 0, data.Length, SetDataOptions.Discard);
            //---
        }

        #region Évènements
        void keyManager_KeyPressed(Keys key)
        {
            if (KeyPressed != null)
                KeyPressed(key);
        }
        #endregion

        protected void StartMenuOn(TimeSpan gameTime)
        {
            timeStartMenuOn = gameTime;
        }

        protected void StartMenuOff(TimeSpan gameTime)
        {
            timeStartMenuOff = gameTime;
        }

        protected void AddKeys(Keys key)
        {
            KeyManager keyManager = new KeyManager(key);
            this.listKeys.Add(keyManager);

            keyManager.KeyPressed += new KeyManager.KeyPressedHandler(keyManager_KeyPressed);
        }

        protected void AddClickableImage(ClickableImage clickableImage)
        {
            this.listClickableImage.Add(clickableImage);
        }

        protected void AddClickableText(ClickableText clickableText)
        {
            this.listClickableText.Add(clickableText);
        }
        
        public virtual void Update(GameTime gameTime)
        {
            if (Game.IsActive)
            {
                KeyboardState keyboardState = Keyboard.GetState();
                MouseState mouseState = Mouse.GetState();

                //---
                for (int i = 0; i < listKeys.Count; i++)
                {
                    listKeys[i].Update(keyboardState);
                }

                if (MouseLeftButtonClicked != null && mouseLeftButtonState == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
                    MouseLeftButtonClicked(mouseState);

                if (MouseRightButtonClicked != null && mouseRightButtonState == ButtonState.Pressed && mouseState.RightButton == ButtonState.Released)
                    MouseRightButtonClicked(mouseState);

                if (MouseMidddleButtonClicked != null && mouseMiddleButtonState == ButtonState.Pressed && mouseState.MiddleButton == ButtonState.Released)
                    MouseMidddleButtonClicked(mouseState);

                mouseLeftButtonState = mouseState.LeftButton;
                mouseRightButtonState = mouseState.RightButton;
                mouseMiddleButtonState = mouseState.MiddleButton;
                //---

                //---
                for (int i = 0; i < listClickableImage.Count; i++)
                {
                    listClickableImage[i].UpdateMouse(mouseState, gameTime);
                }
                //---

                //---
                for (int i = 0; i < listClickableText.Count; i++)
                {
                    listClickableText[i].UpdateMouse(mouseState, gameTime);
                }
                //---

                //---
                UpdateMenuOn(DateTime.Now.TimeOfDay);
                UpdateMenuOff(DateTime.Now.TimeOfDay);
                //---
            }

            //--- Gestion du fond des transitions
            if (initTime == TimeSpan.Zero)
                initTime = gameTime.TotalGameTime;

            if (mapTransition == 1)
            {
                if (colorCenter.A < 250)
                {
                    byte alpha = (byte)((byte)(gameTime.TotalGameTime.Subtract(initTime).TotalMilliseconds / 2));
                    colorCenter = new Color(0, 0, 0, alpha);
                }
                else
                {

                }
            }
            else if (mapTransition == 2)
            {
                if (colorCenter.A > 0)
                {
                    int alpha = 250 - (int)(gameTime.TotalGameTime.Subtract(initTime).TotalMilliseconds / 2);
                    if (alpha < 0) alpha = 0;

                    colorCenter = new Color(0, 0, 0, (byte)alpha);
                }
                else
                {
                    mapTransition = 0;
                }
            }
            //---
        }

        private void UpdateMenuOn(TimeSpan gameTime)
        {
            if (timeStartMenuOn != TimeSpan.Zero)
            {
                int deltaMs = (int)gameTime.Subtract(timeStartMenuOn).TotalMilliseconds;

                float pct = (float)deltaMs / timeChange;

                if (pct < 1f)
                {
                    alphaMenu = (byte)(255 - (int)(pct * 255f));

                    posMenu = (int)((float)(GraphicsDevice.Viewport.Height / 4) * (pct));
                }
                else
                {
                    timeStartMenuOn = TimeSpan.Zero;

                    if (this.MenuAnimationOpenEnded != null)
                        this.MenuAnimationOpenEnded();
                }
            }
        }

        private void UpdateMenuOff(TimeSpan gameTime)
        {
            if (timeStartMenuOff != TimeSpan.Zero)
            {
                int deltaMs = (int)gameTime.Subtract(timeStartMenuOff).TotalMilliseconds;

                float pct = (float)deltaMs / timeChange;

                if (pct < 1f)
                {
                    alphaMenu = (byte)(255 - (int)(pct * 255f));
                    posMenu = (int)((float)(GraphicsDevice.Viewport.Height / 4) * (1f - pct));
                }
                else
                {
                    timeStartMenuOff = TimeSpan.Zero;

                    if (this.MenuAnimationCloseEnded != null)
                        this.MenuAnimationCloseEnded();
                }
            }
        }

        public virtual void Draw(GameTime gameTime)
        {
            if (mapTransition != 0)
            {
                SpriteBatch.Draw(pixel, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), colorCenter);
            }

            if(ShowMiniMenu)
                SpriteBatch.Draw(imgMenu, Vector2.Zero, Color.Black);

            for (int i = 0; i < listClickableImage.Count; i++)
            {
                listClickableImage[i].Draw(SpriteBatch, new Color(255, 255, 255, (byte)this.alphaMenu));
            }

            for (int i = 0; i < listClickableText.Count; i++)
            {
                listClickableText[i].Draw(SpriteBatch, new Color(255, 255, 255, (byte)this.alphaMenu));
            }

            Rectangle recTop = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, posMenu);
            Rectangle recBottom = new Rectangle(0, GraphicsDevice.Viewport.Height - posMenu, GraphicsDevice.Viewport.Width, posMenu);

            SpriteBatch.Draw(pixel, recTop, Color.Black);
            SpriteBatch.Draw(pixel, recBottom, Color.Black);
        }
    }
}
