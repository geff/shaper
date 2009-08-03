using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Shaper.Bloc
{
    public class GameBlocOnePlayer : GameBase
    {
        public GameMain Game { get; set; }
        public SpriteBatch SpriteBatch { get; set; }
        public GraphicsDevice GraphicsDevice { get; set; }
        public ContentManager ContentManager { get; set; }

        private Texture2D pixel;
        private Texture2D bloc0;
        private Texture2D bloc1;

        private Texture2D poingH;
        private Texture2D poingB;
        private Texture2D poingG;
        private Texture2D poingD;


        private Grid grid;
        private Bloc bloc;
        private Vector2 nextPosition;
        private int durtionPoingAnimation = 200;

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

        AnimPoing[,] poingPos = new AnimPoing[3, 3];


        public GameBlocOnePlayer(GameMain game, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, ContentManager contentManager)
            : base(game, spriteBatch, graphicsDevice, contentManager)
        {
            Init();
        }

        #region Initialization
        private void Init()
        {
            bloc0 = ContentManager.Load<Texture2D>("Bloc0");
            bloc1 = ContentManager.Load<Texture2D>("Bloc1");

            poingH = ContentManager.Load<Texture2D>("Poing10_H");
            poingB = ContentManager.Load<Texture2D>("Poing10_B");
            poingG = ContentManager.Load<Texture2D>("Poing10_G");
            poingD = ContentManager.Load<Texture2D>("Poing10_D");

            NewGame();

            //--- Création du pixel
            pixel = new Texture2D(GraphicsDevice, 1, 1, 1, TextureUsage.Linear, SurfaceFormat.Color);
            pixel.SetData<Color>(new Color[] { Color.White });
            //---
        }
        #endregion

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            this.bloc.Position.Y += (float)(this.bloc.Speed * gameTime.ElapsedGameTime.TotalMilliseconds);

            UpdateKeyBoard(gameTime);

            UpdatePoingAnimation(gameTime);

            CheckColision(gameTime);
        }


        private void UpdatePoingAnimation(GameTime gameTime)
        {
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (poingPos[x, y] != AnimPoing.Empty)
                    {
                        int elapsedTime = (int)gameTime.TotalRealTime.Subtract(poingPos[x, y].StatAnim).TotalMilliseconds;

                        if (elapsedTime >= durtionPoingAnimation)
                        {
                            poingPos[x, y] = AnimPoing.Empty;
                            bloc.Values[x, y] = System.Math.Abs(bloc.Values[x, y] - 1);
                        }
                        else
                        {
                            float pct = (float)elapsedTime / (float)durtionPoingAnimation;

                            poingPos[x, y].PosCalcule = poingPos[x, y].PosFinale * pct;
                        }
                    }
                }
            }
        }

        private void UpdateKeyBoard(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            KeyBoardManager(ref keyStateTop1, keyboardState, keyTop1, 0, 0, gameTime);
            KeyBoardManager(ref keyStateTop2, keyboardState, keyTop2, 1, 0, gameTime);
            KeyBoardManager(ref keyStateTop3, keyboardState, keyTop3, 2, 0, gameTime);

            KeyBoardManager(ref keyStateLeft, keyboardState, keyLeft, 0, 1, gameTime);
            KeyBoardManager(ref keyStateCenter, keyboardState, keyCenter, 1, 1, gameTime);
            KeyBoardManager(ref keyStateRight, keyboardState, keyRight, 2, 1, gameTime);

            KeyBoardManager(ref keyStateBottom1, keyboardState, keyBottom1, 0, 2, gameTime);
            KeyBoardManager(ref keyStateBottom2, keyboardState, keyBottom2, 1, 2, gameTime);
            KeyBoardManager(ref keyStateBottom3, keyboardState, keyBottom3, 2, 2, gameTime);

            if (keyboardState.IsKeyDown(Keys.Add))
                bloc.Speed += 0.01f;
            if (keyboardState.IsKeyDown(Keys.Subtract))
                bloc.Speed -= 0.01f;
        }

        private void KeyBoardManager(ref bool keyState, KeyboardState keyboardState, Keys key, int x, int y, GameTime gameTime)
        {
            if (keyState && keyboardState.IsKeyUp(key))
            {
                poingPos[x, y].StatAnim = gameTime.TotalRealTime;

                if (x == 0)
                    poingPos[x, y].PosFinale = new Vector2(x * bloc0.Width + bloc.Position.X, y * bloc1.Height + bloc.Position.Y);
                if (x == 1)
                {
                    if (y == 0)
                        poingPos[x, y].PosFinale = new Vector2(x * bloc0.Width + bloc.Position.X, y * bloc1.Height + bloc.Position.Y);
                    if (y == 2)
                        poingPos[x, y].PosFinale = new Vector2(x * bloc0.Width + bloc.Position.X, (grid.Height - 1) * bloc0.Height - y * bloc1.Height - bloc.Position.Y);
                }
                if (x == 2)
                    poingPos[x, y].PosFinale = new Vector2((grid.Width - 1) * bloc0.Width - x * bloc0.Width - bloc.Position.X, y * bloc1.Height + bloc.Position.Y);
            }

            keyState = keyboardState.IsKeyDown(key);
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(25, 25, 30));

            SpriteBatch.Begin();

            float defaultX = 400;
            float defaultY = 50;

            SpriteBatch.Draw(pixel, new Rectangle((int)defaultX, (int)defaultY, (int)(bloc0.Width * grid.Width), (int)(bloc0.Height * grid.Height)), Color.Black);


            //--- Lines
            for (int i = 0; i < 4; i++)
            {
                Rectangle recV = new Rectangle((int)(defaultX + bloc.Position.X + i * bloc0.Width - bloc0.Width / 20), (int)defaultY, bloc0.Width / 10, grid.Height * bloc0.Height);
                Rectangle recH = new Rectangle((int)defaultX, (int)(defaultY + bloc.Position.Y + i * bloc0.Height - bloc0.Height / 20), grid.Width * bloc0.Width, bloc0.Height / 10);

                Color color = new Color(40, 50, 40);
                SpriteBatch.Draw(pixel, recV, color);
                SpriteBatch.Draw(pixel, recH, color);
            }
            //---

            //--- Bloc
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (bloc.Values[x, y] > 0)
                    {
                        SpriteBatch.Draw(bloc1, bloc.Position + new Microsoft.Xna.Framework.Vector2(defaultX + x * bloc1.Width, defaultY + y * bloc1.Height), Color.White);
                    }
                }
            }
            //---

            //--- Grid
            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    if (grid.Values[x, y] > 0)
                    {
                        SpriteBatch.Draw(bloc0, new Microsoft.Xna.Framework.Vector2(defaultX + x * bloc0.Width, defaultY + bloc0.Height * (grid.Height - 1) - y * bloc0.Height), Color.White);
                    }
                }
            }
            //---

            //--- Poing
            SpriteBatch.Draw(poingH, new Vector2(defaultX + bloc.Position.X + bloc0.Width * 1.5f, defaultY - poingH.Height + poingPos[1, 0].PosCalcule.Y), Color.White);
            SpriteBatch.Draw(poingB, new Vector2(defaultX + bloc.Position.X + bloc0.Width * 1.5f, defaultY + (grid.Height) * bloc0.Height - poingPos[1, 2].PosCalcule.Y), Color.White);

            for (int i = 0; i < 3; i++)
            {
                SpriteBatch.Draw(poingG, new Vector2(defaultX - poingG.Width + poingPos[0, i].PosCalcule.X, defaultY + bloc0.Height * (i + 0.5f) + bloc.Position.Y), Color.White);
                SpriteBatch.Draw(poingD, new Vector2(defaultX + bloc0.Width * grid.Width - poingPos[2, i].PosCalcule.X, defaultY + bloc0.Height * (i + 0.5f) + bloc.Position.Y), Color.White);
            }
            //---

            SpriteBatch.End();
        }

        private void NewGame()
        {
            grid = new Grid(10, 20);
            bloc = new Bloc(new Vector2(5 * bloc1.Width, 0));

            NewBloc(new GameTime());
        }

        private void NewBloc(GameTime gameTime)
        {
            Random rnd = new Random();
            bloc.Init(new Vector2(rnd.Next(grid.Width - 2) * bloc1.Width, 0), gameTime.ElapsedGameTime);

            //--- Initialisation de l'animation des poings
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    poingPos[x, y] = AnimPoing.Empty;
                }
            }
            //---
        }

        public void CheckColision(GameTime gameTime)
        {
            bool isColision = false;
            bool isGameOver = false;

            int posX = 0;
            int posY = 0;

            if (bloc.Position.Y > (bloc0.Height * grid.Height - 1))
                NewBloc(gameTime);

            for (int x = 0; x < 3 && !isColision; x++)
            {
                for (int y = 0; y < 3 && !isColision; y++)
                {
                    if (bloc.Values[x, y] > 0)
                    {
                        Vector2 position = bloc.Position + new Vector2(x * bloc1.Width, y * bloc1.Height);

                        position /= bloc1.Width;

                        posX = (int)position.X;
                        posY = grid.Height - 1 - (int)System.Math.Round((double)position.Y, MidpointRounding.AwayFromZero);
                        //posY = grid.Height - 1 - (int)position.Y;

                        if (posY < grid.Height && (posY == -1 || grid.Values[posX, posY] > 0))
                        {
                            if (posY != -1)
                                posY += y + 1;
                            else
                                posY = 2;

                            posX -= x;
                            isColision = true;
                        }
                    }
                }
            }

            if (isColision)
            {
                for (int x = 0; x < 3 && !isGameOver; x++)
                {
                    for (int y = 0; y < 3 && !isGameOver; y++)
                    {
                        if (bloc.Values[x, y] > 0)
                        {
                            if (posY - y >= grid.Height)
                                isGameOver = true;
                            else
                                grid.Values[posX + x, posY - y] = 1;
                        }
                    }
                }

                if (isGameOver)
                {
                    NewGame();
                }
                else
                {
                    CheckLine();
                    NewBloc(gameTime);
                }
            }
        }

        private void CheckLine()
        {
            for (int y = 0; y < grid.Height; y++)
            {
                int count = grid.Width;

                for (int x = 0; x < grid.Width; x++)
                {
                    if (grid.Values[x, y] > 0) count--;

                    if (count == 0)
                    {

                        for (int y2 = y; y2 < grid.Height - 1; y2++)
                        {
                            for (int x2 = 0; x2 < grid.Width; x2++)
                            {
                                grid.Values[x2, y2] = grid.Values[x2, y2 + 1];
                            }
                        }

                        y--;
                    }
                }
            }
        }
    }

    public struct AnimPoing
    {
        public TimeSpan StatAnim;
        public Vector2 PosFinale;
        public Vector2 PosCalcule;

        public static AnimPoing Empty
        {
            get
            {
                AnimPoing empty = new AnimPoing();
                empty.PosFinale = Vector2.Zero;
                empty.StatAnim = TimeSpan.Zero;
                return empty;
            }
        }

        public static bool operator ==(AnimPoing a1, AnimPoing a2)
        {
            if (a1 == null && a2 == null)
                return true;
            if (a1 != null && a2 == null || a1 == null && a2 != null)
                return false;
            else
            {
                return (a1.PosFinale == a2.PosFinale && a1.StatAnim == a2.StatAnim);
            }
        }

        public static bool operator !=(AnimPoing a1, AnimPoing a2)
        {
            return !(a1 == a2);
        }
    }
}
