using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapGenerator;

namespace MapGenerator.BSP
{
    public class BSPGenerator
    {
        static readonly int HORIZONTAL = 0;
        static readonly int VERTICAL = 1;
        
        public static void GenerateMap(int width, int height, Map map)
        {
            BSPMapNode root = new BSPMapNode(null, 0, 0, width, height, map);
            root.SplitNode(HORIZONTAL, map.Width, 64);
            root.RightChild.SplitNode(VERTICAL, 32, root.RightChild.Height);
            root.RightChild.RightChild.SplitNode(HORIZONTAL, root.RightChild.RightChild.Width, 48);

            //root.SplitNode(VERTICAL, 64, map.Height);
            //root.LeftChild.SplitNode(VERTICAL, 16, root.Height);
            //root.LeftChild.RightChild.SplitNode(VERTICAL, 24, root.Height);
            //root.LeftChild.RightChild.LeftChild.SplitNode(VERTICAL, 12, root.Height);

            BSPMapNode node = root;
            if (node.IsLeaf)
            {
                node.GenerateMap();
            }
            else
            {
                node.LeftChild.GenerateMap(creaMapa);
                //node.LeftChild.GenerateMap();
                node.RightChild.GenerateMap();
            }
        }

        private static void creaMapa(BSPMapNode node)
        {
            DrunkCaveGenerator.GenerateMap(node.Width, node.Height, 30);
        }
    }
}
