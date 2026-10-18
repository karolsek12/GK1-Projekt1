using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK1_1.Components
{
    public struct BoundingBox
    {
        public Point leftTop;
        public Point rightBottom;

        public BoundingBox(Point leftTop, Point rightBottom)
        {
            this.leftTop = leftTop;
            this.rightBottom = rightBottom;
        }

        public bool IsInside(Point p)
        {
            return p.x >=leftTop.x && p.x <= rightBottom.x &&
                p.y >=leftTop.y && p.y <= rightBottom.y;
        }

    }
}
