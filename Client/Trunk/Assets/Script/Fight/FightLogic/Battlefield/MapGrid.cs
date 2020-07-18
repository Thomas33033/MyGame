//using UnityEngine;
//using UnityEditor;
//using System;
//using System.Collections.Generic;

//namespace Fight
//{
//    public class Node
//    {
//        public float x;
//        public float y;

//        /// <summary>
//        ///
//        /// </summary>
//        /// <param name="q">z</param>
//        /// <param name="r">y</param>
//        /// <param name="s">x</param>
//        public MapGrid(int q, int r, int s)
//        {
//            this.q = q;
//            this.r = r;
//            this.s = s;
//            if (q + r + s != 0) throw new ArgumentException("q + r + s must be 0");
//        }

//        public readonly int q;
//        public readonly int r;
//        public readonly int s;

//        public override string ToString()
//        {
//            return "(" + q + "," + r + "," + s + ")";
//        }

//        public int[] ToArray()
//        {
//            return new int[] { q, r, s };
//        }

//        public MapGrid Add(MapGrid b)
//        {
//            return new MapGrid(q + b.q, r + b.r, s + b.s);
//        }

//        public MapGrid Subtract(MapGrid b)
//        {
//            return new MapGrid(q - b.q, r - b.r, s - b.s);
//        }

//        public MapGrid Scale(int k)
//        {
//            return new MapGrid(q * k, r * k, s * k);
//        }

//        public MapGrid RotateLeft()
//        {
//            return new MapGrid(-s, -q, -r);
//        }

//        public MapGrid RotateRight()
//        {
//            return new MapGrid(-r, -s, -q);
//        }

//        static public List<MapGrid> directions = new List<MapGrid> { 
//            new MapGrid(1, 0, -1), 
//            new MapGrid(1, -1, 0), 
//            new MapGrid(0, -1, 1), 
//            new MapGrid(-1, 0, 1), 
//            new MapGrid(-1, 1, 0), 
//            new MapGrid(0, 1, -1) };

//        static public MapGrid Direction(int direction)
//        {
//            return MapGrid.directions[direction];
//        }

//        public MapGrid Neighbor(int direction)
//        {
//            return Add(MapGrid.Direction(direction));
//        }

//        static public List<MapGrid> diagonals = new List<MapGrid> { 
//            new MapGrid(2, -1, -1), 
//            new MapGrid(1, -2, 1), 
//            new MapGrid(-1, -1, 2), 
//            new MapGrid(-2, 1, 1), 
//            new MapGrid(-1, 2, -1), 
//            new MapGrid(1, 1, -2) };

//        public MapGrid DiagonalNeighbor(int direction)
//        {
//            return Add(MapGrid.diagonals[direction]);
//        }

//        public int Length()
//        {
//            return (int)((Math.Abs(q) + Math.Abs(r) + Math.Abs(s)) / 2);
//        }

//        public int Distance(MapGrid b)
//        {
//            return Subtract(b).Length();
//        }

//        public int GetValue(int index)
//        {
//            if (index == 0)
//                return q;
//            else if (index == 1)
//                return r;
//            return s;
//        }
//    }
//}
