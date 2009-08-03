using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Shaper.Etang
{
    public class GameEtang : GameBase
    {
        private Texture2D circleTx;
        private Texture2D fish0Tx;

        private int nbWaterLily;
        private int nbFish;
        private float maxInfluenceDistance;
        private float maxInfluenceFishDistance;
        private float maxFishSpeed;
        private Random rnd;
        private List<Fish> listFish;
        private List<WaterLily> listWaterlily;
        private List<Circle> listCircle = new List<Circle>();

        ClickableImage txtMenu;

        public GameEtang(GameMain game, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, ContentManager contentManager)
            : base(game, spriteBatch, graphicsDevice, contentManager)
        {
            this.Game = game;

            Init();
        }

        #region Initialization
        private void Init()
        {
            this.MouseLeftButtonClicked += new MouseLeftButtonClickedHandler(GameEtang_MouseLeftButtonClicked);
            this.KeyPressed += new KeyPressedHandler(GameEtang_KeyPressed);
            this.MenuAnimationOpenEnded += new MenuAnimationOpenEndedHandler(GameEtang_MenuAnimationOpenEnded);

            //---
            txtMenu = new ClickableImage(ContentManager.Load<Texture2D>(@"Content\Pic\Menu_On"), ContentManager.Load<Texture2D>(@"Content\Pic\Menu_Off"), new Vector2(50, 18));
            txtMenu.ClickImage += new ClickableImage.ClickImageHandler(txtMenu_ClickImage);
            this.AddClickableImage(txtMenu);
            //---

            //--- Chargement des textures
            circleTx = ContentManager.Load<Texture2D>(@"Content\Pic\Circle");
            fish0Tx = ContentManager.Load<Texture2D>(@"Content\Pic\Poisson0");
            //---

            //---
            nbWaterLily = 5;
            nbFish = 20;
            maxInfluenceDistance = 400f;
            maxInfluenceFishDistance = fish0Tx.Width * .2f;
            maxFishSpeed = 0f;
            rnd = new Random(21);
            listFish = new List<Fish>();
            listWaterlily = new List<WaterLily>();
            this.ShowMiniMenu = true;
            //---

            //--- Initialisation des poissons
            float fishBoxWidth = 400f;
            float fishBoxHeight = 400f;

            for (int i = 0; i < nbFish; i++)
            {
                float posX = (float)fish0Tx.Width * 0.3f + ((float)this.GraphicsDevice.Viewport.Width - 2f * (float)fish0Tx.Width * 0.3f) * (float)rnd.NextDouble();
                float posY = (float)fish0Tx.Height * 0.3f + ((float)this.GraphicsDevice.Viewport.Height - 2f * (float)fish0Tx.Height * 0.3f) * (float)rnd.NextDouble();

                Fish fish = new Fish();
                fish.Position = new Vector2(posX, posY);
                fish.Rotation = MathHelper.TwoPi * (float)rnd.NextDouble();
                fish.Orientation = new Vector2((float)Math.Cos(fish.Rotation), (float)Math.Sin(fish.Rotation));
                fish.Orientation.Normalize();
                listFish.Add(fish);

                fish.MaxSpeed = 15f + 15f * (float)rnd.NextDouble();

                fish.CurveLoopX = new Curve();
                fish.CurveLoopY = new Curve();

                float tangent = 0f;

                List<CurveKey> listCurveKeyX = new List<CurveKey>();
                List<CurveKey> listCurveKeyY = new List<CurveKey>();

                float time = 0f;

                for (int j = 0; j < 4; j++)
                {

                    CurveKey keyX = new CurveKey(time, fish.Position.X - fishBoxWidth / 2f + fishBoxWidth * (float)rnd.NextDouble(), tangent, -tangent, CurveContinuity.Smooth);
                    CurveKey keyY = new CurveKey(time, fish.Position.Y - fishBoxHeight / 2f + fishBoxHeight * (float)rnd.NextDouble(), tangent, -tangent, CurveContinuity.Smooth);

                    listCurveKeyX.Add(keyX);
                    listCurveKeyY.Add(keyY);

                    //time += 1000f;// (float)rnd.Next(500, 1000);
                    time += (float)rnd.Next(500, 1000);
                }

                //---
                CurveKey keyX2 = new CurveKey(-1000f, listCurveKeyX.Last().Value, tangent, -tangent, CurveContinuity.Smooth);
                CurveKey keyY2 = new CurveKey(-1000f, listCurveKeyY.Last().Value, tangent, -tangent, CurveContinuity.Smooth);

                fish.CurveLoopX.Keys.Add(keyX2);
                fish.CurveLoopY.Keys.Add(keyY2);
                //---

                for (int j = 0; j < listCurveKeyX.Count; j++)
                {
                    fish.CurveLoopX.Keys.Add(listCurveKeyX[j]);
                    fish.CurveLoopY.Keys.Add(listCurveKeyY[j]);
                }

                //---
                CurveKey keyX3 = new CurveKey((listCurveKeyX.Count) * 1000, listCurveKeyX.First().Value, tangent, -tangent, CurveContinuity.Smooth);
                CurveKey keyY3 = new CurveKey((listCurveKeyX.Count) * 1000, listCurveKeyY.First().Value, tangent, -tangent, CurveContinuity.Smooth);

                fish.CurveLoopX.Keys.Add(keyX3);
                fish.CurveLoopY.Keys.Add(keyY3); ;
                //---

                fish.CurveLoopX.PostLoop = CurveLoopType.Linear;
                fish.CurveLoopX.PreLoop = CurveLoopType.Linear;
                fish.CurveLoopY.PostLoop = CurveLoopType.Linear;
                fish.CurveLoopY.PreLoop = CurveLoopType.Linear;

                fish.CurveLoopX.ComputeTangents(CurveTangent.Smooth);
                fish.CurveLoopY.ComputeTangents(CurveTangent.Smooth);

                //fish.CurveLoopX.Keys.First().TangentIn = fish.CurveLoopX.Keys.Last().TangentIn;
                //fish.CurveLoopX.Keys.Last().TangentOut = fish.CurveLoopX.Keys.First().TangentOut;

                //fish.CurveLoopY.Keys.First().TangentIn = fish.CurveLoopY.Keys.Last().TangentIn;
                //fish.CurveLoopY.Keys.Last().TangentOut = fish.CurveLoopY.Keys.First().TangentOut;
            }
            //---

            base.Init();
        }

        void GameEtang_MenuAnimationOpenEnded()
        {
            this.Game.GameCurrent = new GameMenu(this.Game, this.SpriteBatch, this.GraphicsDevice, this.ContentManager);
        }

        //public void SetTangents(Curve curveX)
        //{
        //    CurveKey prev;
        //    CurveKey current;
        //    CurveKey next;
        //    int prevIndex;
        //    int nextIndex;
        //    for (int i = 0; i < curveX.Keys.Count; i++)
        //    {
        //        prevIndex = i - 1;
        //        if (prevIndex < 0) prevIndex = i;

        //        nextIndex = i + 1;
        //        if (nextIndex == curveX.Keys.Count) nextIndex = i;

        //        prev = curveX.Keys[prevIndex];
        //        next = curveX.Keys[nextIndex];
        //        current = curveX.Keys[i];
        //        SetCurveKeyTangent(ref prev, ref current, ref next);
        //        curveX.Keys[i] = current;
        //        //...
        //    }
        //}
        //static void SetCurveKeyTangent(ref CurveKey prev, ref CurveKey cur, ref CurveKey next)
        //{
        //    float dt = next.Position - prev.Position;
        //    float dv = next.Value - prev.Value;
        //    if (Math.Abs(dv) < float.Epsilon)
        //    {
        //        cur.TangentIn = 0;
        //        cur.TangentOut = 0;
        //    }
        //    else
        //    {
        //        // The in and out tangents should be equal to the slope between the adjacent keys.
        //        cur.TangentIn = dv * (cur.Position - prev.Position) / dt;
        //        cur.TangentOut = dv * (next.Position - cur.Position) / dt;
        //    }
        //}

        private void AddCurveKey(Curve curveX, Curve curveY, float position)
        {
            CurveKey keyX2 = new CurveKey(position, curveX.Keys.First().Value);
            CurveKey keyY2 = new CurveKey(position, curveY.Keys.First().Value);

            //float tangent = 0.1f;

            //keyX2.TangentIn = tangent;
            //keyX2.TangentOut = tangent;
            //keyY2.TangentIn = tangent;
            //keyY2.TangentOut = tangent;

            keyX2.Continuity = CurveContinuity.Smooth;
            keyY2.Continuity = CurveContinuity.Smooth;

            curveX.Keys.Add(keyX2);
            curveY.Keys.Add(keyY2);
        }
        #endregion

        #region Évènements
        void GameEtang_KeyPressed(Keys key)
        {
            throw new NotImplementedException();
        }

        void GameEtang_MouseLeftButtonClicked(MouseState mouseState)
        {
            //--- Création du cercle
            Circle circle = new Circle { Position = new Vector2(mouseState.X, mouseState.Y), Size = 2, Life = 100 };
            listCircle.Add(circle);
            //---
        }

        void txtMenu_ClickImage(ClickableImage image, MouseState mouseState)
        {
            this.StartMenuOn(DateTime.Now.TimeOfDay);
        }
        #endregion

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            UpdateCircles(gameTime);

            UpdateFishMovement(gameTime);

            base.Update(gameTime);
        }

        private void UpdateCircles(GameTime gameTime)
        {
            foreach (Circle circle in listCircle)
            {
                circle.Size += 1;

                if (circle.Size > 800)
                    circle.Life--;
            }

            listCircle.RemoveAll(c => c.Life == 0);
        }

        private void UpdateFishMovement(GameTime gameTime)
        {
            //--- Calcul de l'influence sur les poissons
            foreach (Fish fish in listFish)
            {
                //Vector2 fishOrientation = fish.Orientation;

                List<float> listRatioCircles = new List<float>();
                List<Vector2> listCirclesToFish = new List<Vector2>();

                List<float> listRatioFish = new List<float>();
                List<Vector2> lisFishDirection = new List<Vector2>();

                Vector2 circleFinalDirection = new Vector2();
                float circleFinalRatio = 0f;

                float fishFinalRatio = 0f;
                Vector2 fishFinalDirection = new Vector2();

                List<Vector2> listDirection = new List<Vector2>();

                foreach (Circle circle in listCircle)
                {
                    float distanceFish = Vector2.Distance(circle.Position, fish.Position);
                    float ratioInfluence = 0f;

                    if (distanceFish < maxInfluenceDistance)
                        ratioInfluence = 1f - distanceFish / maxInfluenceDistance;

                    if (ratioInfluence > 0f)
                    {
                        listRatioCircles.Add(ratioInfluence * (float)circle.Life / 100f);
                        Vector2 circleToFish = (fish.Position - circle.Position);
                        circleToFish.Normalize();
                        listCirclesToFish.Add(circleToFish);
                    }
                }
                //---

                //--- Calcul de la somme des influences
                if (listRatioCircles.Count > 0)
                {
                    for (int i = 0; i < listRatioCircles.Count; i++)
                    {
                        circleFinalRatio += listRatioCircles[i] / (float)listRatioCircles.Count;
                        circleFinalDirection += listRatioCircles[i] / (float)listRatioCircles.Count * listCirclesToFish[i];
                    }

                    listDirection.Add(Vector2.Lerp(fish.Orientation, circleFinalDirection, circleFinalRatio));
                }
                //---

                //--- Calcul de l'influence des poissons entre eux
                foreach (Fish fish2 in listFish)
                {
                    if (fish != fish2)
                    {
                        float distanceFish = Vector2.Distance(fish.Position, fish2.Position);
                        float ratioInfluenceFish = 0f;

                        if (distanceFish < maxInfluenceFishDistance)
                        {
                            ratioInfluenceFish = 1f - distanceFish / maxInfluenceFishDistance;

                            if (ratioInfluenceFish > 0f)
                            {
                                listRatioFish.Add(ratioInfluenceFish);
                                Vector2 fishToFish2 = (fish.Position - fish2.Position);
                                fishToFish2.Normalize();
                                lisFishDirection.Add(fishToFish2);
                            }
                        }
                    }
                }
                //---

                if (listRatioFish.Count > 0)
                {
                    for (int i = 0; i < listRatioFish.Count; i++)
                    {
                        fishFinalRatio += listRatioFish[i] / (float)listRatioFish.Count;
                        fishFinalDirection += listRatioFish[i] / (float)listRatioFish.Count * lisFishDirection[i];
                    }

                    listDirection.Add(Vector2.Lerp(fish.Orientation, fishFinalDirection, fishFinalRatio));
                }

                //---
                if (listDirection.Count == 0)
                {
                    float time = fish.CurveLoopX.Keys[5].Position;

                    Vector2 newPosition = new Vector2(fish.CurveLoopX.Evaluate((float)gameTime.TotalGameTime.TotalMilliseconds % time),
                                                      fish.CurveLoopY.Evaluate((float)gameTime.TotalGameTime.TotalMilliseconds % time));

                    Vector2 newPositionDelta = new Vector2(fish.CurveLoopX.Evaluate((float)gameTime.TotalGameTime.TotalMilliseconds % time + 1),
                                                           fish.CurveLoopY.Evaluate((float)gameTime.TotalGameTime.TotalMilliseconds % time + 1));
                    //fish.Position = newPosition;
                    //fish.Orientation = (newPositionDelta - newPosition);
                    Vector2 newDirection = (newPositionDelta - newPosition);
                    newDirection.Normalize();

                    listDirection.Add(newDirection);
                }
                //fish.Orientation.Normalize();
                //---

                if (listDirection.Count > 0)
                {
                    fish.Orientation = Vector2.Zero;
                    foreach (Vector2 direction in listDirection)
                        fish.Orientation += direction;
                    fish.Orientation /= (float)listDirection.Count;

                    fish.Orientation.Normalize();

                    //fish.Position += fish.Orientation * (circleFinalRatio + fishFinalRatio) / 2f * fish.MaxSpeed;
                    float speed = (circleFinalRatio + fishFinalRatio + 1f) / (float)listDirection.Count * fish.MaxSpeed;

                    if (speed < 5f)
                        speed = 15f;

                    fish.Position += fish.Orientation *speed;
                }
            }
        }

        float angle = 0f;
        float dist = 0f;

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(25, 25, 30));

            SpriteBatch.Begin();

            foreach (Fish fish in listFish)
            {
                //fish.Rotation = MathHelper.PiOver2 * 0f - (float)Math.Acos(Vector2.Dot(Vector2.UnitX, fish.Orientation));
                //MathShaper.GetAngle(0f, 0f, fish.Orientation.X, fish.Orientation.Y)
                fish.Rotation = MathHelper.PiOver2 + MathShaper.GetAngle(0f, 0f, fish.Orientation.X, fish.Orientation.Y);//-(float)Math.Acos(Vector2.Dot(Vector2.UnitX, fish.Orientation));


                SpriteBatch.Draw(fish0Tx, new Vector2((int)fish.Position.X, (int)fish.Position.Y), null, new Color(255, 255, 255, (byte)this.alphaMenu), fish.Rotation, new Vector2(fish0Tx.Width / 2f, fish0Tx.Height / 2f), new Vector2(0.3f, 0.3f), SpriteEffects.None, 1f);

                //SpriteBatch.Draw(pixel, fish.Position + 10f * fish.Orientation, null, Color.Red, 0f, Vector2.Zero, 5f, SpriteEffects.None, 1f);
                //SpriteBatch.Draw(pixel, fish.Position, null, Color.Green, 0f, Vector2.Zero, 5f, SpriteEffects.None, 1f);

                //for (int i = 0; i < fish.CurveLoopX.Keys[5].Position; i++)
                //{
                //    SpriteBatch.Draw(pixel, new Vector2(fish.CurveLoopX.Evaluate((float)i), fish.CurveLoopY.Evaluate((float)i)), null, Color.Red, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                //}
            }

            foreach (Circle circle in listCircle)
            {
                Vector2 pos = circle.Position - new Vector2(circle.Size / 2);
                SpriteBatch.Draw(circleTx, new Rectangle((int)pos.X, (int)pos.Y, circle.Size, circle.Size), Color.Lerp(Color.TransparentBlack, Color.Azure, (float)circle.Life / 100f));
            }

            base.Draw(gameTime);

            SpriteBatch.End();
        }

        private void NewGame()
        {
        }
    }
}
