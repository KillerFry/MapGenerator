using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapGenerator.BSP
{
    class BSPMapNode
    {
        #region Properties

        public Map Map { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int MapOffsetX { get; private set; }
        public int MapOffsetY { get; private set; }
        public int ParentOffsetX { get; private set; } 
        public int ParentOffsetY { get; private set; }
        public bool IsRoot { get; private set; }
        public bool IsLeaf { get; private set; }
        
        private BSPMapNode Parent { get; set; }
        public BSPMapNode LeftChild { get; private set; }
        public BSPMapNode RightChild { get; private set; }

        #endregion

        public BSPMapNode(BSPMapNode parent, int startX, int startY, int nodeWidth, int nodeHeight, Map map)
        {
            if (parent is null)
            {
                IsRoot = true;
            }

            Map = map;
            Parent = parent;
            Width = nodeWidth;
            Height = nodeHeight;
            ParentOffsetX = (parent is null ? 0 : startX - Parent.MapOffsetX);
            ParentOffsetY = (parent is null ? 0 : startY - Parent.MapOffsetY);
            MapOffsetX = startX;
            MapOffsetY = startY;

            IsLeaf = true;
        }

        public void SplitNode(int splitDirection, int splitX, int splitY)
        {
            IsLeaf = false;

            if (splitDirection == 0)
            {
                LeftChild = new BSPMapNode(this, MapOffsetX, MapOffsetY, splitX, splitY, Map);
                RightChild = new BSPMapNode(this, MapOffsetX, splitY + MapOffsetY, splitX, Height - splitY, Map);
            }
            else
            {
                LeftChild = new BSPMapNode(this, MapOffsetX, MapOffsetY, splitX, splitY, Map);
                RightChild = new BSPMapNode(this, splitX + MapOffsetX, MapOffsetY, Width - splitX, splitY, Map);
            }
            
        }

        public void GenerateMap()
        {
            if (IsLeaf)
            {
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        Map.TheGrid[x + MapOffsetX, y + MapOffsetY] = 0;
                    }
                }
            } else
            {
                LeftChild.GenerateMap();
                RightChild.GenerateMap();
            }
        }
    }
}
