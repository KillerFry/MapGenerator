using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LaCuca;

namespace MapGenerator
{
    public class Map
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int[,] TheGrid { get; private set; }

        public Map(int width, int height)
        {
            Width = width;
            Height = height;

            TheGrid = new int[width, height];
            TheGrid.Fill(1);
        }

        public void CreateMap()
        {
            
        }
    }
}
