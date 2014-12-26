using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YapayZeka1
{
    class Point
    {
        public int x; //x koor.
        public int y; //y. koor
        public Point root; // bir ustundeki dugum
        public int level; // seviye

        public Point() { }

        public Point(int x, int y,int lvl, Point p)
        {
            this.x = x;
            this.y = y;
            this.root = p;
            this.level = lvl;
        }
    }
}
