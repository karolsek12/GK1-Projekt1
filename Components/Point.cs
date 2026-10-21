using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK1_1.Components
{
    
    public struct ExactPoint
    {
        public double x;
        public double y;

        public ExactPoint(double x, double y)
        {
            this.x = x; this.y = y;
        }

        public ExactPoint(Point p)
        {
            x = p.x;
            y = p.y;
        }

        public void Shift(Vector v)
        {
            x += v.x;
            y += v.y;
        }

        public static ExactPoint operator +(ExactPoint a, ExactPoint b)
        {
            return new ExactPoint(a.x + b.x, a.y + b.y);
        }

        public static ExactPoint operator -(ExactPoint a, ExactPoint b)
        {
            return new ExactPoint(a.x - b.x, a.y - b.y);
        }

        public static ExactPoint operator *(double s, ExactPoint a)
        {
            return new ExactPoint(a.x * s, a.y * s);
        }
    }

    public struct Point
    {
        public int x;
        public int y;

        public int X
        {
            get
            {
                return x;
            }
        }

        public int Y
        {
            get
            {
                return y;
            }
        }

        public Point(int  x, int y)
        {
            this.x = x; this.y = y;
        }

        public Point(Vertex v)
        {
            x = v.X;
            y = v.Y;
        }

        public Point(System.Windows.Point p)
        {
            x = (int)p.X;
            y = (int)p.Y;
        }

        public Point(ExactPoint ep)
        {
            x = (int)(ep.x + 0.5);
            y = (int)(ep.y + 0.5);
        }
        public void Shift(Vector v)
        {
            x += v.x;
            y += v.y;
        }

        public static Point operator +(Point a, Point b)
        {
            return new Point(a.x+b.x, a.y+b.y);
        }

        public static Point operator -(Point a, Point b)
        {
            return new Point(a.x - b.x, a.y - b.y);
        }

        public static Point operator *(int s,Point a)
        {
            return new Point(a.x * s, a.y * s);
        }
    }
}
