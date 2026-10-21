using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using GK1_1.Components;
using GK1_1.Components.Edges;

namespace GK1_1
{
    public static class Rasterization
    {
        public static int pAmount = 20;
        public static double d = 1.0 / pAmount;

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

        public static List<Point> BezierMidPoints(Point v0, Point v1, Point v2, Point v3)
        {
            ExactPoint a0 = new ExactPoint(v0);
            ExactPoint a1 = new ExactPoint(3 * (v1 - v0));
            ExactPoint a2 = new ExactPoint(3 * (v2 - 2 * v1 + v0));
            ExactPoint a3 = new ExactPoint(v3 - 3 * v2 + 3 * v1 - v0);
            double t0 = 0;
            double increment = d;
            List<Point> midPoints = new List<Point>();
            ExactPoint p = a0;
            ExactPoint pDelta = Math.Pow(d, 3) * a3 + Math.Pow(d, 2) * a2 + d * a1;
            ExactPoint pDelta2 = 6 * Math.Pow(d, 3) * a3 + 2 * Math.Pow(d, 2) * a2;
            ExactPoint pDelta3 = 6 * Math.Pow(d, 3) * a3;

            midPoints.Add(v0);

            for(int i = 0; i < pAmount; i++)
            {
                p += pDelta;
                pDelta += pDelta2;
                pDelta2 += pDelta3;
                midPoints.Add(new Point(p));
            }


            midPoints.Add(v3);
            return midPoints;
        }
    }
}
