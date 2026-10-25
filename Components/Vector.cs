using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GK1_1.Components
{
    public struct ExactVector
    {
        public double x;
        public double y;

        public ExactVector(double x, double y)
        {
            this.x = x; ;
            this.y = y;
        }

        public ExactVector(ExactPoint to, ExactPoint from)
        {
            x = to.x - from.x;
            y = to.y - from.y;
        }

        public ExactVector(ExactPoint v)
        {
            x = v.x;
            y = v.y;
        }

        public ExactVector(Vector v) 
        {
            x = (double)v.x;
            y = (double)v.y;
        
        }

        public ExactVector(ExactVector v)
        {
            x = v.x;
            y = v.y;

        }

        public double Length()
        {
            return Math.Sqrt(x * x + y * y);
        }

        public ExactVector GetRotated(double alpha)
        {
            double cos = Math.Cos(alpha);
            double sin = Math.Sin(alpha);

            return new ExactVector((int)(cos * x + sin * y), (int)(-sin * x + cos * y));
        }

        public ExactVector GetNormalised()
        {
            double l = Length();
            l = 1 / l;
            ExactVector normalised = new ExactVector(this);
            normalised.x *= l;
            normalised.y *= l;

            return normalised;
        }
    }


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

        public Vector(Point v)
        {
            x = v.x;
            y = v.y;
        }

        public Vector(ExactVector v)
        {
            x = (int)v.x;
            y = (int)v.y;
        }

        public Vector GetRotated(double alpha)
        {
            double cos = Math.Cos(alpha);
            double sin = Math.Sin(alpha);

            return new Vector((int)(cos * x + sin * y), (int)(-sin * x + cos * y));
        }
    }
}
