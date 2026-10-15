using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK1_1.Components
{
    
    public struct Point
    {
        public int x;
        public int y;

        public Point(int  x, int y)
        {
            this.x = x; this.y = y;
        }

        public Point(Vertex v)
        {
            x = v.X;
            y = v.Y;
        }
    }
}
