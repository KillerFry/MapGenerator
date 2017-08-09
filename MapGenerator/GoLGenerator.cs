using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MapGenerator
{
    public class GoLGenerator
    {
        static readonly int FLOOR = 0;
        static readonly int WALL = 1;

        static Random random = new Random(12345);

        #region Cave generation properties

        /// <summary>
        /// Two dimensional int array to hold map cells.
        /// </summary>
        public static int[,] Map;

        /// <summary>
        /// Map width in cells.
        /// </summary>
        public static int Width { get; private set; }

        /// <summary>
        /// Map height in cells.
        /// </summary>
        public static int Height { get; private set; }

        /// <summary>
        /// Probability percentage to generate a wall.
        /// </summary>
        public static int WallPercentage { get; private set; }

        /// <summary>
        /// Total iterations/cell visits.
        /// </summary>
        public static int Iterations { get; private set; }

        /// <summary>
        /// Neighbors that a cell must have in order to inverts it's state.
        /// </summary>
        public static int LiveNeighbors { get; private set; }

        #endregion

        #region Cave cleaning properties

        /// <summary>
        /// Remove rooms smaller than this value.
        /// </summary>
        public static int LowerLimit { get; private set; }

        /// <summary>
        /// Remove rooms larger than this value.
        /// </summary>
        public static int UpperLimit { get; private set; }

        /// <summary>
        /// Removes single cells from cave edges: a cell with this number of empty neighbors is removed.
        /// </summary>
        public static int EmptyNeighbors { get; private set; }

        /// <summary>
        /// Number of full cells that have to exist around a cell to make it come alive.
        /// </summary>
        public static int LiveCellNeighbors { get; private set; }

        #endregion

        #region Cave corridor properties

        /// <summary>
        /// Minimum corridor length.
        /// </summary>
        public static int CorridorMin { get; private set; }

        /// <summary>
        /// Maximum corridor length.
        /// </summary>
        public static int CorridorMax { get; private set; }
        /// <summary>
        /// Maximum number of turns in a corridor.
        /// </summary>
        public static int CorridorMaxTurns { get; private set; }

        /// <summary>
        /// When this value is exceeded, stop attempting to connect caves. Prevents algorithm from getting stuck.
        /// </summary>
        public static int CorridorSpace { get; private set; }


        public static int Breakout { get; private set; }

        #endregion

        // Generic list of points which contain 4 directions
        static List<Point> Directions = new List<Point>()
        {
            new Point (0,-1)    //north
            , new Point(0,1)    //south
            , new Point (1,0)   //east
            , new Point (-1,0)  //west
        };

        static List<Point> Directions8Way = new List<Point>()
        {
            new Point (0,-1)    //north
            , new Point(0,1)    //south
            , new Point (1,0)   //east
            , new Point (-1,0)  //west
            , new Point (1,-1)  //northeast
            , new Point(-1,-1)  //northwest
            , new Point (-1,1)  //southwest
            , new Point (1,1)   //southeast
            , new Point(0,0)    //centre
        };

        /// <summary>
        /// Caves within the map are stored here
        /// </summary>
        private static List<List<Point>> Caves;

        private static List<Point> Corridors;

        public GoLGenerator() { }

        public static int[,] GenerateMap(int width = 100, int height = 100, int wallPercentage = 43, int iterations = 45000,
            int liveNeighbors = 3, int emptyNeigbors = 4, int liveCellNeighbors = 4, int lowerLimit = 16, int upperLimit = 500,
            int corridorMin = 2, int corridorMax = 5, int corridorMaxTurns = 10, int corridorSpace = 2, int breakout = 100000)
        {
            Width = width;
            Height = height;
            WallPercentage = wallPercentage;
            Iterations = iterations;
            LiveNeighbors = liveNeighbors;
            EmptyNeighbors = emptyNeigbors;
            LiveCellNeighbors = liveCellNeighbors;
            LowerLimit = lowerLimit;
            UpperLimit = upperLimit;
            CorridorMin = corridorMin;
            CorridorMax = corridorMax;
            CorridorMaxTurns = corridorMaxTurns;
            CorridorSpace = 2;
            Breakout = breakout;

            FillMap();
            CleanMap();
            GetCaves();
            ConnectCaves();

            return Map;
        }


        private static void FillMap()
        {
            Map = new int[Width, Height];

            /* Place random walls in the map, according to the set WallPercentage */
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (random.Next(0, 100) < WallPercentage)
                    {
                        Map[x, y] = WALL;
                    }
                }
            }

            Point cell;

            /* Picking cells at random, we go through them. If there are enough LiveNeighbors (walls), this means
             * it can be sustained and we give life to it (make it a wall).
             * If not, then it can't live by itself and we kill it make it a floor). */
            for (int x = 0; x <= Iterations; x++)
            {
                cell = new Point(random.Next(0, Width), random.Next(0, Height));

                if (GetCellNeighbors8Way(cell).Where(n => GetCellValue(n) == WALL).Count() > LiveNeighbors)
                {
                    Map[cell.X, cell.Y] = WALL;
                }
                else
                {
                    Map[cell.X, cell.Y] = FLOOR;
                }
            }
        }

        public static void CleanMap()
        {
            //  Smooth of the rough cave edges and any single blocks by making several 
            //  passes on the map and removing any cells with 3 or more empty neighbours
            Point cell;

            for (int ctr = 0; ctr < 5; ctr++)
            {
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        cell = new Point(x, y);

                        if (GetCellValue(cell) == WALL && GetCellNeighbors4Way(cell).Where(n => GetCellValue(n) == FLOOR).Count() >= EmptyNeighbors)
                        {
                            Map[x, y] = FLOOR;
                        }
                    }
                }
            }

            //  Fill in any empty cells that have 4 full neighbours
            //  to get rid of any holes in an cave
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    cell = new Point(x, y);

                    if (
                            GetCellValue(cell) == FLOOR
                            && GetCellNeighbors4Way(cell).Where(n => GetCellValue(n) == WALL).Count() >= LiveCellNeighbors
                        )
                        Map[x, y] = WALL;
                }
            }
        }

        /// <summary>
        /// Returns a list of valid neighboring cells of a point using north, south, east, and west.
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private static List<Point> GetCellNeighbors4Way(Point cell)
        {
            return Directions.Select(d => new Point(cell.X + d.X, cell.Y + d.Y))
                .Where(d => CheckPointInMap(d)).ToList();
        }

        /// <summary>
        /// Returns a list of all eight neighbors of a cell.
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private static List<Point> GetCellNeighbors8Way(Point cell)
        {
            return Directions8Way.Select(d => new Point(cell.X + d.X, cell.Y + d.Y))
                    .Where(d => CheckPointInMap(d)).ToList();
        }

        /// <summary>
        /// Check if a point is within the map.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private static bool CheckPointInMap(Point point)
        {
            return point.X >= 0 & point.X < Width & point.Y >= 0 & point.Y < Height;
        }

        private static int GetCellValue(Point cell)
        {
            return Map[cell.X, cell.Y];
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

        private static void GetCaves()
        {
            Caves = new List<List<Point>>();

            List<Point> Cave;
            Point cell;

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    cell = new Point(x, y);

                    if (GetCellValue(cell) == FLOOR && Caves.Count(s => s.Contains(cell)) == 0)
                    {
                        Cave = new List<Point>();

                        LocateCave(cell, Cave);

                        if (Cave.Count() <= LowerLimit | Cave.Count() > UpperLimit)
                        {
                            foreach (Point p in Cave)
                            {
                                Map[p.X, p.Y] = WALL;
                            }
                        }
                        else
                        {
                            Caves.Add(Cave);
                        }
                    }
                }
            }
        }

        private static void LocateCave(Point cell, List<Point> cave)
        {
            foreach (Point p in GetCellNeighbors4Way(cell).Where(n => GetCellValue(n) == FLOOR))
            {
                if (!cave.Contains(p))
                {
                    cave.Add(p);
                    LocateCave(p, cave);
                }
            }
        }

        private static bool ConnectCaves()
        {
            if (Caves.Count == 0)
            {
                return false;
            }

            List<Point> currentCave;
            List<List<Point>> connectedCaves = new List<List<Point>>();
            Point cor_point = new Point();
            Point cor_direction = new Point();
            List<Point> potentialCorridor = new List<Point>();
            int breakoutctr = 0;

            Corridors = new List<Point>();

            currentCave = Caves[random.Next(0, Caves.Count())];
            connectedCaves.Add(currentCave);
            Caves.Remove(currentCave);

            do
            {
                if (Corridors.Count == 0)
                {
                    currentCave = connectedCaves[random.Next(0, connectedCaves.Count)];
                    CaveGetEdge(currentCave, ref cor_point, ref cor_direction);
                }
                else
                {
                    if (random.Next(0, 100) > 50)
                    {
                        currentCave = connectedCaves[random.Next(0, connectedCaves.Count)];
                        CaveGetEdge(currentCave, ref cor_point, ref cor_direction);
                    }
                    else
                    {
                        currentCave = null;
                        CorridorGetEdge(ref cor_point, ref cor_direction);
                    }
                }

                potentialCorridor = CorridorAttempt(cor_point, cor_direction, true);

                if (potentialCorridor != null)
                {
                    for (int ctr = 0; ctr < Caves.Count; ctr++)
                    {
                        if (Caves[ctr].Contains(potentialCorridor.Last()))
                        {
                            if (currentCave == null | currentCave != Caves[ctr])
                            {
                                potentialCorridor.Remove(potentialCorridor.Last());
                                Corridors.AddRange(potentialCorridor);
                                foreach (Point p in potentialCorridor)
                                {
                                    Map[p.X, p.Y] = FLOOR;
                                }

                                connectedCaves.Add(Caves[ctr]);
                                Caves.RemoveAt(ctr);
                                break;
                            }
                        }
                    }
                }


                if (breakoutctr++ > Breakout)
                {
                    return false;
                }
            } while (Caves.Count > 0);

            Caves.AddRange(connectedCaves);
            connectedCaves.Clear();

            return true;
        }

        private static void CorridorGetEdge(ref Point cor_point, ref Point cor_direction)
        {
            List<Point> validdirections = new List<Point>();

            do
            {
                //the modifiers below prevent the first of last point being chosen
                cor_point = Corridors[random.Next(1, Corridors.Count - 1)];

                //attempt to locate all the empy map points around the location
                //using the directions to offset the randomly chosen point
                foreach (Point p in Directions)
                    if (CheckPointInMap(new Point(cor_point.X + p.X, cor_point.Y + p.Y)))
                        if (GetCellValue(new Point(cor_point.X + p.X, cor_point.Y + p.Y)) == WALL)
                            validdirections.Add(p);


            } while (validdirections.Count == 0);

            cor_direction = validdirections[random.Next(0, validdirections.Count)];
            cor_point.X = cor_point.X + cor_direction.X;
            cor_point.Y = cor_point.Y + cor_direction.Y;
        }

        private static void CaveGetEdge(List<Point> currentCave, ref Point cor_point, ref Point cor_direction)
        {
            do
            {
                cor_point = currentCave.ToList()[random.Next(0, currentCave.Count)];

                cor_direction = DirectionGet(cor_direction);

                do
                {
                    cor_point.X = cor_point.X + cor_direction.X;
                    cor_point.Y = cor_point.Y + cor_direction.Y;

                    if (!CheckPointInMap(cor_point))
                    {
                        break;
                    }
                    else if (GetCellValue(cor_point) == WALL)
                    {
                        return;
                    }
                } while (true);
            } while (true);
        }

        private static List<Point> CorridorAttempt(Point startPoint, Point direction, bool preventBacktracking)
        {
            List<Point> lPotentialCorridor = new List<Point>();
            lPotentialCorridor.Add(startPoint);

            int corridorLength;
            Point startDirection = new Point(direction.X, direction.Y);

            int turns = CorridorMaxTurns;

            while (turns >= 0)
            {
                turns--;

                corridorLength = random.Next(CorridorMin, CorridorMax);

                while (corridorLength > 0)
                {
                    corridorLength--;

                    startPoint.X = startPoint.X + direction.X;
                    startPoint.Y = startPoint.Y + direction.Y;

                    if (CheckPointInMap(startPoint) && GetCellValue(startPoint) != WALL)
                    {
                        lPotentialCorridor.Add(startPoint);
                        return lPotentialCorridor;
                    }

                    if (!CheckPointInMap(startPoint))
                    {
                        return null;
                    }
                    else if (!CorridorPointTest(startPoint, direction))
                    {
                        return null;
                    }

                    lPotentialCorridor.Add(startPoint);
                }

                if (turns > 1)
                {
                    if (!preventBacktracking)
                    {
                        direction = DirectionGet(direction);
                    }
                    else
                    {
                        direction = DirectionGet(direction, startDirection);
                    }
                }
            }

            return null;
        }

        private static Point DirectionGet(Point startPoint)
        {
            Point newDirection;

            do
            {
                newDirection = Directions[random.Next(0, Directions.Count)];
            } while (newDirection.X != -startPoint.X & newDirection.Y != -startPoint.Y);

            return newDirection;
        }

        private static Point DirectionGet(Point startPoint, Point excludedPoint)
        {
            Point newDirection;

            do
            {
                newDirection = Directions[random.Next(0, Directions.Count)];
            } while (ReverseDirection(newDirection) == startPoint | ReverseDirection(newDirection) == excludedPoint);

            return newDirection;
        }

        private static Point ReverseDirection(Point newDirection)
        {
            return new Point(-newDirection.X, -newDirection.Y);
        }

        private static bool CorridorPointTest(Point startPoint, Point direction)
        {
            foreach (int r in Enumerable.Range(-CorridorSpace, 2 * CorridorSpace + 1).ToList())
            {
                if (direction.X == 0)
                {
                    if (CheckPointInMap(new Point(startPoint.X + r, startPoint.Y)))
                    {
                        if (GetCellValue(new Point(startPoint.X + r, startPoint.Y)) != WALL)
                        {
                            return false;
                        }
                    }
                }
                else if (direction.Y == 0)
                {
                    if (CheckPointInMap(new Point(startPoint.X, startPoint.Y + r)))
                    {
                        if (GetCellValue(new Point(startPoint.X, startPoint.Y + r)) != WALL)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}
