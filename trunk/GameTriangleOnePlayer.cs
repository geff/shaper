using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FarseerGames.GettingStarted.DrawingSystem;

namespace Shaper.Triangle
{
    public class GameTriangleOnePlayer : GameBase
    {
        public GameMain Game { get; set; }
        public SpriteBatch SpriteBatch { get; set; }
        public GraphicsDevice GraphicsDevice { get; set; }
        public ContentManager ContentManager { get; set; }

        private Texture2D pixel;
        private Texture2D texTriangle;
        private Grid grid;

        public int TriWidth { get { return texTriangle.Width; } }
        public int TriHeight { get { return texTriangle.Height; } }

        float defaultX = 50;
        float defaultY = 10;

        List<Line> listLine = new List<Line>();
        bool triangleActivated = false;
        int countTriangleActivated = 0;
        Triangle firstActivatedTriangle = null;

        int lastScroolWheelValue = 0;
        ButtonState prevLeftButtonState = ButtonState.Released;
        int maxStep = 5;
        LineBrush lineBrush;

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

        public GameTriangleOnePlayer(GameMain game, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, ContentManager contentManager)
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
            texTriangle = ContentManager.Load<Texture2D>("trianglem");

            //--- Création du pixel
            pixel = new Texture2D(GraphicsDevice, 1, 1, 1, TextureUsage.Linear, SurfaceFormat.Color);
            pixel.SetData<Color>(new Color[] { Color.White });
            //---

            lineBrush = new LineBrush(1, Color.Blue);
            lineBrush.Load(this.GraphicsDevice);

            grid = new Grid(8, 16);
            //grid = new Grid(4,4);
        }
        #endregion

        private void NewLines()
        {
            listLine = new List<Line>();
            triangleActivated = false;
            countTriangleActivated = 0;
        }

        private void CalcTriangleActivated()
        {
            triangleActivated = false;
            countTriangleActivated = 0;
            firstActivatedTriangle = null;
            TimeSpan currentTimespan = TimeSpan.MaxValue;

            ActionOnGrid(delegate(Triangle triangle)
            {
                if (triangle.Activated)
                {
                    countTriangleActivated++;

                    if (!triangleActivated)
                        triangleActivated = true;

                    if (triangle.ActivatedTime < currentTimespan)
                    {
                        firstActivatedTriangle = triangle;
                        currentTimespan = triangle.ActivatedTime;
                    }
                }
            });
        }

        private void UpdateClick()
        {
            CalcTriangleActivated();
        }

        delegate void ActionOnGridDelegate(Triangle triangle);

        private void ActionOnGrid(ActionOnGridDelegate method)
        {
            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    if (grid.Values[x, y] != null)
                    {
                        method(grid.Values[x, y]);
                    }
                }
            }
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            UpdateKeyBoard(gameTime);
            UpdateMouse(gameTime);
        }

        private void UpdateKeyBoard(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
        }

        //Dictionary<Triangle, Vector2> listDistTriangle = new Dictionary<Triangle, Vector2>();

        private void CalcNextLine(Vector2 origin, Vector2 direction, int step, Triangle parent)
        {
            //--- Uniquement fait par le premier appelant
            if (step == maxStep)
            {
                listLine = new List<Line>();
            }
            //---

            List<Vector2> listIntersections = new List<Vector2>();
            List<Vector2> listDirections = new List<Vector2>();
            List<Triangle> listTriangle = new List<Triangle>();

            bool intersected = false;
            float dist = float.MaxValue;
            Vector2 intersectionPoint = Vector2.Zero;
            Vector2 intersectionDirection = Vector2.Zero;
            Triangle intersectedTriangle = null;

            if (step > 0)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    for (int y = 0; y < grid.Height; y++)
                    {
                        if (grid.Values[x, y] != null && grid.Values[x, y] != parent)
                        {
                            Triangle triangle = grid.Values[x, y];
                            //---
                            if (!intersected && triangle.Activated)
                            {
                                Vector2 newDirection = Vector2.Zero;
                                Vector2 intersection = IntersectionTriangle(origin, direction, triangle, ref newDirection);

                                if (intersection != Vector2.Zero)
                                {
                                    intersected = true;
                                    listIntersections.Add(intersection);
                                    listDirections.Add(newDirection);
                                    listTriangle.Add(triangle);
                                }
                            }
                            //---
                        }
                    }
                }

                //--- Intersection avec le triangle le plus proche
                if (intersected)
                {
                    dist = float.MaxValue;
                    for (int i = 0; i < listIntersections.Count; i++)
                    {
                        float curdist = Vector2.Distance(origin, listIntersections[i]);

                        if (curdist < dist)
                        {
                            dist = curdist;
                            intersectionPoint = listIntersections[i];
                            intersectionDirection = listDirections[i];// new Vector2(0.5f, 0.5f);
                            intersectedTriangle = listTriangle[i];
                        }
                    }
                }
                //---

                //--- Test d'intersetion avec les bords
                if (!intersected)
                {
                    Vector2 point1 = Vector2.Zero;
                    Vector2 point2 = Vector2.Zero;
                    Vector2 point3 = Vector2.Zero;
                    Vector2 point4 = Vector2.Zero;

                    point1 = new Vector2(defaultX, defaultY);
                    point2 = new Vector2(defaultX + grid.Width * TriWidth, defaultY);
                    point3 = new Vector2(defaultX + grid.Width * TriWidth, defaultY + grid.Height * TriHeight);
                    point4 = new Vector2(defaultX, defaultY + grid.Height * TriHeight);

                    Vector2 intersection1 = ProcessIntersection(origin, origin + direction * 5000f, point1, point2);
                    Vector2 intersection2 = ProcessIntersection(origin, origin + direction * 5000f, point2, point3);
                    Vector2 intersection3 = ProcessIntersection(origin, origin + direction * 5000f, point3, point4);
                    Vector2 intersection4 = ProcessIntersection(origin, origin + direction * 5000f, point4, point1);

                    float newDist = Vector2.Distance(origin, intersection1);
                    if (intersection1 != Vector2.Zero && newDist < dist && newDist>0)
                    {
                        dist = newDist;
                        intersectionPoint = intersection1;
                        intersectionDirection = new Vector2(direction.X, -direction.Y);
                    }
                    
                    newDist = Vector2.Distance(origin, intersection2);
                    if (intersection2 != Vector2.Zero && Vector2.Distance(origin, intersection2) < dist && newDist>0)
                    {
                        dist = newDist;
                        intersectionPoint = intersection2;
                        intersectionDirection = new Vector2(-direction.X, direction.Y);
                    }

                    newDist = Vector2.Distance(origin, intersection3);
                    if (intersection3 != Vector2.Zero && Vector2.Distance(origin, intersection3) < dist && newDist > 0)
                    {
                        dist = newDist;
                        intersectionPoint = intersection3;
                        intersectionDirection = new Vector2(direction.X, -direction.Y);
                    }

                    newDist = Vector2.Distance(origin, intersection4);
                    if (intersection4 != Vector2.Zero && Vector2.Distance(origin, intersection4) < dist && newDist > 0)
                    {
                        dist = newDist;
                        intersectionPoint = intersection4;
                        intersectionDirection = new Vector2(-direction.X, direction.Y);
                    }
                }
                //---

                listLine.Add(new Line(origin.X, origin.Y, intersectionPoint.X, intersectionPoint.Y));
                CalcNextLine(intersectionPoint, intersectionDirection, --step, intersectedTriangle);

                //ActionOnGrid(delegate(Triangle triangle)
                //{
                    
                //});
            }
        }

        //public static class Vector2Extensions
        //{
        //    public static Vector2 CrossRight(this Vector2 v)
        //    {
        //        return new Vector2(v.Y, -v.X);
        //    }
        //    public static Vector2 CrossLeft(this Vector2 v)
        //    {
        //        return new Vector2(-v.Y, v.X);
        //    }
        //}

        private Vector2 IntersectionTriangle(Vector2 origin, Vector2 direction, Triangle triangle, ref Vector2 newDirection)
        {
            float dist = float.MaxValue;
            Vector2 intersectionPoint = Vector2.Zero;

            Vector2 point1 = Vector2.Zero;
            Vector2 point2 = Vector2.Zero;
            Vector2 point3 = Vector2.Zero;

            Vector2 trianglePos = new Vector2(
            defaultX + ((float)triangle.PosX + 0.5f) * (float)TriWidth,
            defaultY + (float)TriHeight * (float)(grid.Height - 1 - (float)triangle.PosY + 0.5f));


            float w = ((float)TriWidth * 0.5f);

            point1 = new Vector2(trianglePos.X + w * (float)Math.Cos(triangle.Angle - MathHelper.PiOver2),
                                 trianglePos.Y + w * (float)Math.Sin(triangle.Angle - MathHelper.PiOver2));

            point2 = new Vector2(trianglePos.X + w * (float)Math.Cos(triangle.Angle + MathHelper.TwoPi / 6f),
                                 trianglePos.Y + w * (float)Math.Sin(triangle.Angle - MathHelper.TwoPi / 6f));

            point3 = new Vector2(trianglePos.X + w * (float)Math.Cos(triangle.Angle - MathHelper.TwoPi * 2f / 3f),
                                 trianglePos.Y + w * (float)Math.Sin(triangle.Angle - MathHelper.TwoPi * 2f / 3f));

            Vector2 intersection1 = ProcessIntersection(origin, origin + direction * 5000f, point1, point2);
            Vector2 intersection2 = ProcessIntersection(origin, origin + direction * 5000f, point2, point3);
            Vector2 intersection3 = ProcessIntersection(origin, origin + direction * 5000f, point3, point1);
            
            if (intersection1 != Vector2.Zero && Vector2.Distance(origin, intersection1) < dist)
            {
                dist = Vector2.Distance(origin, intersection1);
                intersectionPoint = intersection1;
                newDirection = Vector2.Reflect(direction, new Vector2(point2.Y - point1.Y, -(point2.X - point1.X)));
                newDirection.Normalize();
            }
            if (intersection2 != Vector2.Zero && Vector2.Distance(origin, intersection2) < dist)
            {
                dist = Vector2.Distance(origin, intersection2);
                intersectionPoint = intersection2;
                newDirection = Vector2.Reflect(direction, new Vector2(point3.Y - point2.Y, -(point3.X - point2.X)));
                newDirection.Normalize();
            }
            if (intersection3 != Vector2.Zero && Vector2.Distance(origin, intersection3) < dist)
            {
                dist = Vector2.Distance(origin, intersection3);
                intersectionPoint = intersection3;
                newDirection = Vector2.Reflect(direction, new Vector2(point1.Y - point3.Y, -(point1.X - point3.X)));
                newDirection.Normalize();
            }

            return intersectionPoint;
        }

        private Vector2 ProcessIntersection(Vector2 point1, Vector2 point2, Vector2 point3, Vector2 point4)
        {
            bool intersection = false;
            bool coincident = false;

            Vector2 intersectionPoint = Vector2.Zero;

            float ua = (point4.X - point3.X) * (point1.Y - point3.Y) - (point4.Y - point3.Y) * (point1.X - point3.X);
            float ub = (point2.X - point1.X) * (point1.Y - point3.Y) - (point2.Y - point1.Y) * (point1.X - point3.X);
            float denominator = (point4.Y - point3.Y) * (point2.X - point1.X) - (point4.X - point3.X) * (point2.Y - point1.Y);

            intersection = coincident = false;

            if (Math.Abs(denominator) <= 0.00001f)
            {
                if (Math.Abs(ua) <= 0.00001f && Math.Abs(ub) <= 0.00001f)
                {
                    intersection = coincident = true;
                    intersectionPoint = (point1 + point2) / 2;
                }
            }
            else
            {
                ua /= denominator;
                ub /= denominator;

                if (ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1)
                {
                    intersection = true;
                    intersectionPoint.X = point1.X + ua * (point2.X - point1.X);
                    intersectionPoint.Y = point1.Y + ua * (point2.Y - point1.Y);
                }
            }

            return intersectionPoint;
        }

        private void UpdateMouse(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();
            Point mousePoint = new Point(mouseState.X, mouseState.Y);

            int deltaScroolWheelValue = mouseState.ScrollWheelValue - lastScroolWheelValue;

            ActionOnGrid(delegate(Triangle triangle)
            {
                Rectangle rec = new Rectangle(
                    (int)(defaultX + ((float)triangle.PosX) * (float)TriWidth),
                    (int)(defaultY + (float)TriHeight * (float)(grid.Height - 1 - (float)triangle.PosY + 0.5f)), TriWidth, TriHeight);

                triangle.Selected = rec.Contains(mousePoint);

                //--- Click gauche
                if (triangle.Selected && prevLeftButtonState == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
                {
                    triangle.Activated = !triangle.Activated;
                    triangle.ActivatedTime = DateTime.Now.TimeOfDay;
                }
                //---

                //--- Scroll
                if (triangle.Selected && deltaScroolWheelValue != 0)
                {
                    triangle.Angle -= (float)deltaScroolWheelValue / 800f;
                }
                //---
            });


            if (deltaScroolWheelValue != 0 || (prevLeftButtonState == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released))
            {
                CalcTriangleActivated();

                if (countTriangleActivated >= 1)
                {
                    listLine.Add(new Line(defaultX + ((float)firstActivatedTriangle.PosX + 0.5f) * (float)TriWidth, defaultY));
                    //listDistTriangle = new Dictionary<Triangle, Vector2>();
                    CalcNextLine(new Vector2(defaultX + ((float)firstActivatedTriangle.PosX + 0.5f) * (float)TriWidth, defaultY), new Vector2(0, 1), maxStep, null);
                }
                else if (countTriangleActivated == 0)
                {
                    NewLines();
                }
            }

            prevLeftButtonState = mouseState.LeftButton;
            lastScroolWheelValue = mouseState.ScrollWheelValue;
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(25, 25, 30));

            SpriteBatch.Begin();

            //--- Grid
            ActionOnGrid(delegate(Triangle triangle)
            {
                if (triangle.Selected)
                    SpriteBatch.Draw(pixel, new Rectangle((int)(defaultX + ((float)triangle.PosX + 0.0f) * TriWidth), (int)(defaultY + TriHeight * (grid.Height - 1) - ((float)triangle.PosY + 0.0f) * TriHeight), TriWidth, TriHeight), Color.Gray);

                if (triangle.Activated)
                    SpriteBatch.Draw(pixel, new Rectangle((int)(defaultX + ((float)triangle.PosX + 0.1f) * (float)(TriWidth)), (int)(defaultY + (float)(TriHeight) * ((float)grid.Height - 1f - (float)triangle.PosY + 0.1f)), (int)((float)TriWidth * 0.8f), (int)((float)TriHeight * 0.8f)), Color.Green);

                SpriteBatch.Draw(texTriangle, new Vector2(
                    defaultX + ((float)triangle.PosX + 0.5f) * (float)TriWidth,
                    defaultY + (float)TriHeight * (float)(grid.Height - 1 - (float)triangle.PosY + 0.5f)), null, Color.White, triangle.Angle, new Vector2(TriWidth / 2, TriHeight / 2), 1f, SpriteEffects.None, 0f);
            });
            //---

            //---
            //foreach (Line line in listLine)
            for(int i = 0; i < listLine.Count; i++)
            {
                lineBrush.Color = Color.Lerp(Color.Blue, Color.Black, (float)i / (float)(maxStep-1));
                lineBrush.Draw(SpriteBatch, listLine[i].Pos1, listLine[i].Pos2);
            }
            //---

            SpriteBatch.End();
        }
    }
}
