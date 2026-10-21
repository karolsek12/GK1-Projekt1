using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK1_1.Components
{
    public struct Line
    {
        public Point p1;
        public Point p2;

        public Point P1
        {
            get
            {
                return p1;
            }
        }

        public Point P2
        {
            get
            {
                return p2;
            }
        }

        public Line(Point p1, Point p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }


    }
}
