using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using LaCuca;

namespace MapGenerator
{
    public class DrunkCaveGenerator
    {
        static readonly int FLOOR = 0;
        static readonly int WALL = 1;

        static Random random = new Random(DateTime.Now.Millisecond);

        #region Cave generation properties

        public static int[,] Map;

        public static int Width { get; private set; }

        public static int Height { get; private set; }

        public static int FloorPercentage { get; private set; }

        public static float TotalCells { get; private set; }

        private static float FloorCount { get; set; }

        public static Point lastCell { get; set; }

        #endregion

        static List<Point> Directions = new List<Point>()
        {
            new Point(0, -1),   // North
            new Point(0, 1),   // South
            new Point(1, 0),   // East
            new Point(-1, 0)   // West
        };

        public DrunkCaveGenerator() { }

        public static int[,] GenerateMap(int width = 30, int height = 30, int floorPercentage = 30)
        {
            Width = width;
            Height = height;
            FloorPercentage = floorPercentage;

            FillMap();

            return TrimMap();
        }

        public static int[,] GenerateMap(int width = 30, int height = 30, int floorPercentage = 30, bool byHand = false)
        {
            Width = width;
            Height = height;
            FloorPercentage = floorPercentage;

            if (byHand)
            {
                UpdateMap(-1);
                return Map;
            }
            else
            {
                FillMap();
                return TrimMap();
            }
        }

        private static void FillMap()
        {
            TotalCells = Width * Height;

            Map = new int[Width, Height];

            Map.Fill(WALL);

            var cell = new Point(random.Next(1, Width), random.Next(1, Height));

            Map[cell.X, cell.Y] = FLOOR;
            FloorCount = 1;

            int newDirection;
            var v = 0.0f;

            do
            {
                newDirection = random.Next(0, 4);

                if (Directions[newDirection].X < 0)
                {
                    if (cell.X + Directions[newDirection].X <= 0)
                    {
                        var newMap = new int[Width + 1, Height];

                        for (int x = 0; x <= Width; x++)
                        {
                            for (int y = 0; y < Height; y++)
                            {
                                if (x == 0)
                                {
                                    newMap[x, y] = WALL;
                                }
                                else
                                {
                                    newMap[x, y] = Map[x - 1, y];
                                }
                            }
                        }

                        Map = newMap;
                        Width = Width + 1;
                        cell.X = cell.X + Directions[newDirection].X + 1;
                    }
                    else
                    {
                        cell.X = cell.X + Directions[newDirection].X;
                    }
                }

                if (Directions[newDirection].X > 0)
                {
                    if (cell.X + Directions[newDirection].X >= Width - 1)
                    {
                        var newMap = new int[Width + 1, Height];

                        for (int x = 0; x <= Width; x++)
                        {
                            for (int y = 0; y < Height; y++)
                            {
                                if (x < Width)
                                {
                                    newMap[x, y] = Map[x, y];
                                }
                                else
                                {
                                    newMap[x, y] = WALL;
                                }
                            }
                        }

                        Map = newMap;
                        Width = Width + 1;
                    }

                    cell.X = cell.X + Directions[newDirection].X;
                }

                if (Directions[newDirection].Y < 0)
                {
                    if (cell.Y + Directions[newDirection].Y <= 0)
                    {
                        var newMap = new int[Width, Height + 1];

                        for (int x = 0; x < Width; x++)
                        {
                            for (int y = 0; y <= Height; y++)
                            {
                                if (y == 0)
                                {
                                    newMap[x, y] = WALL;
                                }
                                else
                                {
                                    newMap[x, y] = Map[x, y - 1];
                                }
                            }
                        }

                        Map = newMap;
                        Height = Height + 1;
                        cell.Y = cell.Y + Directions[newDirection].Y + 1;
                    }
                    else
                    {
                        cell.Y = cell.Y + Directions[newDirection].Y;
                    }
                }

                if (Directions[newDirection].Y > 0)
                {
                    if (cell.Y + Directions[newDirection].Y >= Height - 1)
                    {
                        var newMap = new int[Width, Height + 1];

                        for (int x = 0; x < Width; x++)
                        {
                            for (int y = 0; y <= Height; y++)
                            {
                                if (y < Height)
                                {
                                    newMap[x, y] = Map[x, y];
                                }
                                else
                                {
                                    newMap[x, y] = WALL;
                                }
                            }
                        }

                        Map = newMap;
                        Height = Height + 1;
                    }

                    cell.Y = cell.Y + Directions[newDirection].Y;
                }
                
                if (Map[cell.X, cell.Y] == FLOOR)
                {
                    continue;
                }

                Map[cell.X, cell.Y] = FLOOR;
                FloorCount++;
                v = FloorCount / TotalCells * 100;

            } while (v < FloorPercentage);

            lastCell = cell;
        }
        
        public static int[,] UpdateMap(int di)
        {
            TotalCells = Width * Height;

            if (di == -1)
            {
                Map = new int[Width, Height];

                Map.Fill(WALL);

                var cell = new Point(random.Next(1, Width), random.Next(1, Height));

                lastCell = cell;

                Map[cell.X, cell.Y] = FLOOR;
                FloorCount = 1;
            }
            else
            {
                var cell = lastCell;
                
                if (Directions[di].X < 0)
                {
                    if (cell.X + Directions[di].X <= 0)
                    {
                        var newMap = new int[Width + 1, Height];

                        for (int x = 0; x <= Width; x++)
                        {
                            for (int y = 0; y < Height; y++)
                            {
                                if (x == 0)
                                {
                                    newMap[x, y] = WALL;
                                }
                                else
                                {
                                    newMap[x, y] = Map[x - 1, y];
                                }
                            }
                        }

                        Map = newMap;
                        Width = Width + 1;
                        cell.X = cell.X + Directions[di].X + 1;
                    }
                    else
                    {
                        cell.X = cell.X + Directions[di].X;
                    }
                }

                if (Directions[di].X > 0)
                {
                    if (cell.X + Directions[di].X == Width - 1)
                    {
                        var newMap = new int[Width + 1, Height];

                        for (int x = 0; x <= Width; x++)
                        {
                            for (int y = 0; y < Height; y++)
                            {
                                if (x < Width)
                                {
                                    newMap[x, y] = Map[x, y];
                                }
                                else
                                {
                                    newMap[x, y] = WALL;
                                }
                            }
                        }

                        Map = newMap;
                        Width = Width + 1;
                    }

                    cell.X = cell.X + Directions[di].X;
                }

                if (Directions[di].Y < 0)
                {
                    if (cell.Y + Directions[di].Y <= 0)
                    {
                        var newMap = new int[Width, Height + 1];

                        for (int x = 0; x < Width; x++)
                        {
                            for (int y = 0; y <= Height; y++)
                            {
                                if (y == 0)
                                {
                                    newMap[x, y] = WALL;
                                }
                                else
                                {
                                    newMap[x, y] = Map[x, y - 1];
                                }
                            }
                        }

                        Map = newMap;
                        Height = Height + 1;
                        cell.Y = cell.Y + Directions[di].Y + 1;
                    }
                    else
                    {
                        cell.Y = cell.Y + Directions[di].Y;
                    }
                }

                if (Directions[di].Y > 0)
                {
                    if (cell.Y + Directions[di].Y == Height - 1)
                    {
                        var newMap = new int[Width, Height + 1];

                        for (int x = 0; x < Width; x++)
                        {
                            for (int y = 0; y <= Height; y++)
                            {
                                if (y < Height)
                                {
                                    newMap[x, y] = Map[x, y];
                                }
                                else
                                {
                                    newMap[x, y] = WALL;
                                }
                            }
                        }

                        Map = newMap;
                        Height = Height + 1;
                    }

                    cell.Y = cell.Y + Directions[di].Y;
                }

                lastCell = cell;

                if (Map[cell.X, cell.Y] == FLOOR)
                {
                    return Map;
                }

                Map[cell.X, cell.Y] = FLOOR;
                FloorCount++;
            }

            return Map;
        }

        public static int[,] TrimMap()
        {
            var minX = 0;
            var maxX = 0;
            var minY = 0;
            var maxY = 0;

            for (int i = 1; i < Map.GetLength(0); i++)
            {
                for (int j = 1; j < Map.GetLength(1); j++)
                {
                    if (Map[i, j] == FLOOR)
                    {
                        if (minX == 0 || i < minX)
                        {
                            minX = i;
                        }
                        
                        if (maxX == 0 || i > maxX)
                        {
                            maxX = i;
                        }

                        if (minY == 0 || j < minY)
                        {
                            minY = j;
                        }

                        if (maxY == 0 || j > maxY)
                        {
                            maxY = j;
                        }
                    }
                }
            }
            
            int newWidth = (maxX - minX + 1) + 2;
            int newHeight = (maxY - minY + 1) + 2;

            var newMap = new int[newWidth, newHeight];

            for (int x = 0; x < newWidth; x++)
            {
                for (int y = 0; y < newHeight; y++)
                {
                    newMap[x, y] = Map[x + minX - 1, y + minY - 1];
                }
            }

            Map = newMap;
            Width = newWidth;
            Height = newHeight;

            return Map;
        }

        private static bool CheckPointInMap(Point point)
        {
            return point.X >= 0 & point.X < Width & point.Y >= 0 & point.Y < Height;
        }

        public static Point GetRandomMapPosition()
        {
            Point cell;
            Random randomNumber = new Random();
            do
            {
                cell = new Point(randomNumber.Next(0, Width), randomNumber.Next(0, Height));
            }
            while (Map[cell.X, cell.Y] != FLOOR);

            return cell;
        }
    }
}
