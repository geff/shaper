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
        #region Properties
        public delegate void ActionOnGridDelegate(Triangle triangle);

        private TriangleRender render;

        public Grid grid;
        public int TriWidth { get { return 80; } }
        public int TriHeight { get { return 80; } }
        public float defaultX = 400;
        public float defaultY = 150;

        public List<Line> listLine = new List<Line>();
        public Triangle firstActivatedTriangle = null;
        public int maxStep = 8;
        public TimeSpan latestTimeOver = TimeSpan.Zero;
        public int timeOverDuration = 25000;

        public Line lineFOI1 = null;
        public Line lineFOI2 = null;

        List<Triangle> listIntersectedTriangle = null;

        bool triangleActivated = false;
        int countTriangleActivated = 0;
        bool triangleMoved = false;

        public Triangle currentTriangle = null;
        float angleClick = 0f;
        #endregion

        public GameTriangleOnePlayer(GameMain game, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, ContentManager contentManager)
            : base(game, spriteBatch, graphicsDevice, contentManager)
        {
            this.Game = game;
            this.render = new TriangleRender(this, game, spriteBatch, graphicsDevice, contentManager);

            Init();
        }

        #region Initialization
        private void Init()
        {
            //--- Evènements
            this.MouseLeftButtonClicked += new MouseLeftButtonClickedHandler(GameTriangleOnePlayer_MouseLeftButtonClicked);
            this.MouseRightButtonClicked += new MouseRightButtonClickedHandler(GameTriangleOnePlayer_MouseRightButtonClicked);
            this.KeyPressed += new KeyPressedHandler(GameTriangleOnePlayer_KeyPressed);
            //---

            //---
            this.AddKeys(Keys.Space);
            //---

            render.Initialize();

            grid = new Grid(6, 10);
            ActionOnGrid(delegate(Triangle triangle)
            {
                triangle.CalcPosition(defaultX, defaultY, TriWidth, TriHeight, grid.Width, grid.Height);
                triangle.CalcPoints(TriWidth);
            });

            base.Init();
        }

        void GameTriangleOnePlayer_KeyPressed(Keys key, GameTime gameTime)
        {
            if(key == Keys.Space)
                SetTimeOver(gameTime);
        }

        void GameTriangleOnePlayer_MouseRightButtonClicked(MouseState mouseState, GameTime gameTime)
        {
            bool exit = false;

            Point mousePoint = new Point(mouseState.X, mouseState.Y);

            for (int x = 0; x < grid.Width && !exit; x++)
            {
                for (int y = 0; y < grid.Height && !exit; y++)
                {
                    if (grid.Values[x, y] != null)
                    {
                        Triangle triangle = grid.Values[x, y];

                        Rectangle rec = new Rectangle(
                            (int)(defaultX + ((float)triangle.PosGridX) * (float)TriWidth),
                            (int)(defaultY + (float)TriHeight * (float)(grid.Height - 1 - (float)triangle.PosGridY + 0.0f)), TriWidth, TriHeight);

                        bool selected = rec.Contains(mousePoint);
                        triangle.Selected = rec.Contains(mousePoint);

                        //--- Désactivation du triangle en cours
                        if (currentTriangle != null)
                        {
                            currentTriangle.Activated = false;
                            currentTriangle = null;
                        }
                        //--- Désactivation du triangle cliqué
                        else if (currentTriangle == null && selected && triangle.Activated)
                        {
                            triangle.Activated = false;
                        }
                    }
                }
            }
        }

        void GameTriangleOnePlayer_MouseLeftButtonClicked(MouseState mouseState, GameTime gameTime)
        {
            if (currentTriangle != null)
            {
                currentTriangle = null;
                return;
            }

            bool exit = false;

            Point mousePoint = new Point(mouseState.X, mouseState.Y);

            for (int x = 0; x < grid.Width && !exit; x++)
            {
                for (int y = 0; y < grid.Height && !exit; y++)
                {
                    if (grid.Values[x, y] != null)
                    {
                        Triangle triangle = grid.Values[x, y];

                        Rectangle rec = new Rectangle(
                            (int)(defaultX + ((float)triangle.PosGridX) * (float)TriWidth),
                            (int)(defaultY + (float)TriHeight * (float)(grid.Height - 1 - (float)triangle.PosGridY + 0.0f)), TriWidth, TriHeight);

                        bool selected = rec.Contains(mousePoint);
                        triangle.Selected = rec.Contains(mousePoint);

                        //--- Sélection du nouveau triangle
                        if (currentTriangle == null && selected)
                        {
                            if (firstActivatedTriangle != null && firstActivatedTriangle.Color != triangle.Color)
                                break;

                            triangle.Activated = true;
                            triangle.ActivatedTime = DateTime.Now.TimeOfDay;

                            currentTriangle = triangle;
                            currentTriangle.PrevAngle = currentTriangle.Angle;

                            angleClick = MathShaper.GetAngle(currentTriangle.PosX, currentTriangle.PosY, mouseState.X, mouseState.Y);

                            exit = true;
                        }
                        //---
                    }
                }
            }
        }
        #endregion

        #region Update
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            UpdateTimeOver(gameTime);
            UpdateTriangleMove(gameTime);
            UpdateMouseMove(gameTime);

            base.Update(gameTime);
        }

        private void UpdateMouseMove(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();
            Point mousePoint = new Point(mouseState.X, mouseState.Y);

            CalcTriangleActivated();

            if (currentTriangle != null)
            {
                currentTriangle.Angle = currentTriangle.PrevAngle + MathShaper.GetAngle(currentTriangle.PosX, currentTriangle.PosY, mouseState.X, mouseState.Y) - angleClick;
                currentTriangle.CalcPoints(TriWidth);
            }

            if (firstActivatedTriangle != null)
                BeginCalcLine(new Vector2(firstActivatedTriangle.PosX, defaultY));
            else
                NewLines();

            if (currentTriangle != null)
            {
                CalcFieldOfIntersection(currentTriangle);
            }
        }

        private void UpdateTimeOver(GameTime gameTime)
        {
            if (latestTimeOver == TimeSpan.Zero)
                latestTimeOver = gameTime.ElapsedGameTime;
            else if (gameTime.TotalGameTime.Subtract(latestTimeOver).TotalMilliseconds >= timeOverDuration)
            {
                SetTimeOver(gameTime);
            }
        }

        private void UpdateTriangleMove(GameTime gameTime)
        {
            triangleMoved = false;

            ActionOnGrid(delegate(Triangle triangle)
            {
                if (triangle.Move)
                {
                    if (triangle.PosY < triangle.NextPosY)
                    {
                        triangle.PosY += (triangle.Speed * (float)gameTime.ElapsedRealTime.TotalMilliseconds / 1000f);
                        triangle.CalcPoints(TriWidth);
                    }
                    else
                    {
                        grid.Values[triangle.PosGridX, triangle.PosGridY] = null;
                        grid.Values[triangle.NextPosGridX, triangle.NextPosGridY] = triangle;

                        triangle.PosGridX = triangle.NextPosGridX;
                        triangle.PosGridY = triangle.NextPosGridY;

                        triangle.Move = false;
                    }
                }

                triangleMoved |= triangle.Move;
            });
        }

        private void SetTimeOver(GameTime gameTime)
        {
            latestTimeOver = gameTime.TotalGameTime;
            listLine = new List<Line>();
            currentTriangle = null;

            ActionOnGrid(delegate(Triangle triangle)
            {
                triangle.Activated = false;

                if (triangle.HitLaser)
                    grid.Values[triangle.PosGridX, triangle.PosGridY] = null;
            });

            CalcNextTrianglePos();
        }

        private void CalcNextTrianglePos()
        {
            int[,] tempGrid = new int[grid.Width, grid.Height];

            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    if (grid.Values[x, y] != null)
                    {
                        tempGrid[x, y] = 1;
                    }
                }
            }

            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    if (tempGrid[x, y] == 0)
                    {
                        for (int y2 = y; y2 < grid.Height; y2++)
                        {
                            if (tempGrid[x, y2] != 0)
                            {
                                grid.Values[x, y2].NextPosGridX = x;
                                grid.Values[x, y2].NextPosGridY = y;

                                grid.Values[x, y2].CalcNextPosition(defaultX, defaultY, TriWidth, TriHeight, grid.Width, grid.Height);
                                grid.Values[x, y2].Move = true;

                                tempGrid[x, y] = 1;
                                tempGrid[x, y2] = 0;
                                break;
                            }
                        }

                        //y = grid.Height;
                    }
                }
            }
        }
        #endregion

        #region Intersection
        private void BeginCalcLine(Vector2 origin)
        {
            ActionOnGrid(delegate(Triangle triangle)
            {
                triangle.HitLaser = false;
            });

            CalcNextLine(origin, Vector2.UnitY, maxStep, null);
        }

        private void CalcNextLine(Vector2 origin, Vector2 direction, int step, Triangle parent)
        {
            //--- Uniquement fait par le premier appelant
            if (step == maxStep)
            {
                listLine = new List<Line>();
                listIntersectedTriangle = new List<Triangle>();
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
                                Vector2 intersection = MathShaper.IntersectionTriangle(defaultX, defaultY, (float)TriWidth, (float)TriHeight, (float)grid.Width, (float)grid.Height, origin, direction, triangle, ref newDirection);

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

                //--- Test d'intersection avec les bords
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

                    Vector2 intersection1 = MathShaper.IntersectionSegment(origin, origin + direction * 5000f, point1, point2);
                    Vector2 intersection2 = MathShaper.IntersectionSegment(origin, origin + direction * 5000f, point2, point3);
                    Vector2 intersection3 = MathShaper.IntersectionSegment(origin, origin + direction * 5000f, point3, point4);
                    Vector2 intersection4 = MathShaper.IntersectionSegment(origin, origin + direction * 5000f, point4, point1);

                    float newDist = (float)System.Math.Round((double)Vector2.Distance(origin, intersection1), 2);
                    if (intersection1 != Vector2.Zero && newDist < dist && newDist > 0)
                    {
                        dist = newDist;
                        intersectionPoint = intersection1;
                        intersectionDirection = new Vector2(direction.X, -direction.Y);
                    }

                    newDist = (float)System.Math.Round((double)Vector2.Distance(origin, intersection2), 2);
                    if (intersection2 != Vector2.Zero && newDist < dist && newDist > 0)
                    {
                        dist = newDist;
                        intersectionPoint = intersection2;
                        intersectionDirection = new Vector2(-direction.X, direction.Y);
                    }

                    newDist = (float)System.Math.Round((double)Vector2.Distance(origin, intersection3), 2);
                    if (intersection3 != Vector2.Zero && newDist < dist && newDist > 0)
                    {
                        dist = newDist;
                        intersectionPoint = intersection3;
                        intersectionDirection = new Vector2(direction.X, -direction.Y);
                    }

                    newDist = (float)System.Math.Round((double)Vector2.Distance(origin, intersection4), 2);
                    if (intersection4 != Vector2.Zero && newDist < dist && newDist > 0)
                    {
                        dist = newDist;
                        intersectionPoint = intersection4;
                        intersectionDirection = new Vector2(-direction.X, direction.Y);
                    }
                }
                //---

                listLine.Add(new Line(origin.X, origin.Y, intersectionPoint.X, intersectionPoint.Y));
                listIntersectedTriangle.Add(intersectedTriangle);

                CalcNextLine(intersectionPoint, intersectionDirection, --step, intersectedTriangle);

                if (intersectedTriangle != null)
                {
                    intersectedTriangle.HitLaser = true;
                }

                //ActionOnGrid(delegate(Triangle triangle)
                //{

                //});
            }
        }

        private void CalcFieldOfIntersection(Triangle triangle)
        {
            lineFOI1 = null;
            lineFOI2 = null;

            int index = listIntersectedTriangle.IndexOf(triangle);
            if (index != -1)
            {
                Line line = listLine[index];
                float[] t = new float[2];
                Vector2[] intersection = new Vector2[2] { Vector2.Zero, Vector2.Zero };
                Vector2[] normal = new Vector2[2];
                float triangleRayon = (float)TriWidth * 0.45f;

                MathShaper.IntersectionLineCircle(line.Pos1, line.Pos2 - line.Pos1, new Vector2(triangle.PosX, triangle.PosY), triangleRayon, ref t, ref intersection, ref normal);

                //Vector2 finalPoint = Vector2.Zero;

                //if (intersection[0] != Vector2.Zero && intersection[1] == Vector2.Zero)
                //{
                //    finalPoint = intersection[0];
                //}
                //else if (intersection[0] == Vector2.Zero && intersection[1] != Vector2.Zero)
                //{
                //    finalPoint = intersection[1];
                //}
                if (intersection[0] != Vector2.Zero && intersection[1] != Vector2.Zero)
                {
                    float dist0 = Vector2.Distance(intersection[0], line.Pos1);
                    float dist1 = Vector2.Distance(intersection[1], line.Pos1);

                    if (dist1 < dist0)
                    {
                        Vector2 v = intersection[0];
                        intersection[0] = intersection[1];
                        intersection[1] = v;
                    }

                    float distInter = Vector2.Distance(intersection[0], intersection[1]);

                    float deltaAngle = MathHelper.Pi / 2000f;

                    if ((float)Math.Round(distInter) < triangleRayon * 2)
                    {
                        lineFOI1 = CalcReflexionOnTrianglePoint(triangle, intersection[0], line.Pos1, line.Pos2 - line.Pos1, -deltaAngle);
                        lineFOI2 = CalcReflexionOnTrianglePoint(triangle, intersection[1], line.Pos1, line.Pos2 - line.Pos1, deltaAngle);
                    }
                    else
                    {
                        lineFOI1 = CalcReflexionOnTrianglePoint(triangle, intersection[0], line.Pos1, line.Pos2 - line.Pos1, deltaAngle);
                        lineFOI2 = CalcReflexionOnTrianglePoint(triangle, intersection[0], line.Pos1, line.Pos2 - line.Pos1, -deltaAngle);
                    }
                }
            }
        }

        private Line CalcReflexionOnTrianglePoint(Triangle triangle, Vector2 point, Vector2 origin, Vector2 direction, float deltaAngle)
        {
            Line line = new Line(0f, 0f);
            float angle = MathShaper.GetAngle(triangle.PosX, triangle.PosY, point.X, point.Y);

            angle += MathHelper.PiOver2 + deltaAngle;

            direction.Normalize();
            Vector2 newDirection = Vector2.Zero;
            Vector2 intersection = MathShaper.IntersectionTriangle(defaultX, defaultY, (float)TriWidth, (float)TriHeight, (float)grid.Width, (float)grid.Height,
                                    origin, direction, triangle, angle, ref newDirection);

            line = new Line(triangle.PosX, triangle.PosY, intersection.X + newDirection.X * 150, intersection.Y + newDirection.Y * 150);

            return line;
        }
        #endregion

        #region Private methods
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

        public void ActionOnGrid(ActionOnGridDelegate method)
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
        #endregion

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            render.Draw(gameTime);

            SpriteBatch.Begin();
            base.Draw(gameTime);
            SpriteBatch.End();
        }
    }
}
