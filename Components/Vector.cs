using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GK1_1.Components
{
    public struct Vector
    {
        public int x;
        public int y;

        public Vector(int x, int y)
        {
            this.x = x; ;
            this.y = y;
        }

        public Vector(Point to, Point from)
        {
            x = to.x - from.x;
            y = to.y - from.y;
        }


        public Vector GetRotated(double alpha)
        {
            double cos = Math.Cos(alpha);
            double sin = Math.Sin(alpha);

            return new Vector((int)(cos * x + sin * y), (int)(-sin * x + cos * y));
        }
    }
}
