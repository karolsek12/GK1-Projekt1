using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using GK1_1.Components;

namespace GK1_1
{
    public static class Rasterization
    {
        public static List<Point> Bresenham(Point A, Point B)
        {

            if (A.x > B.x)// guarantee that B will be after A
                return Bresenham(B, A);

            if (A.y == B.y)
            {
                List<Point> straight = new List<Point>();

                for (int i = A.x;i<= B.x; i++)
                {
                    straight.Add(new Point(i, A.y));
                }
                return straight;

            }

            if (A.y > B.y) // guarantee that m>0
                return FlipY(Bresenham(FlipY(A), FlipY(B)));

            if(B.y -  A.y > B.x-A.x) // guaranteee that m<1
                return FlipXY(Bresenham(FlipXY(A), FlipXY(B)));

            List<Point> list = new List<Point>();

            int dx = B.x - A.x;
            int dy = B.y - A.y;
            int d = 2*dy - dx;
            int incrE = 2 * dy;
            int incrNE = 2 * (dy - dx);
            int x = A.x;
            int y = A.y;
            list.Add(new Point(x, y));

            while (x < B.x)
            {
                if (d < 0)
                {
                    d += incrE;
                    x++;
                }
                else
                {
                    d += incrNE;
                    x++;
                    y++;
                }
                list.Add(new Point(x, y));
            }

            return list;
        }


        public static List<Point> FlipXY(List<Point> list)
        {
            List<Point> flipped = new List<Point>();

            foreach (Point p in list)
            {
                flipped.Add(FlipXY(p));
            }

            return flipped;
        }

        public static Point FlipXY(Point A)
        {
            return new Point(A.y, A.x);
        }

        public static List<Point> FlipY(List<Point> list)
        {
            List<Point> flipped = new List<Point>();

            foreach (Point p in list)
            {
                flipped.Add(FlipY(p));
            }

            return flipped;
        }

        public static Point FlipY(Point A)
        {
            return new Point(A.x, -A.y);
        }

    }
}
