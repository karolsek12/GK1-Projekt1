using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK1_1.Components.Edges
{
    public class CircularEdge : Edge
    {
        public CircularEdge(Vertex p1, Vertex p2) : base(p1, p2)
        {
            _pixels = new List<Point>();
        }


        public override (List<Point>, List<Point>) GetPoints()
        {
            return (new List<Point>(), new List<Point>());
        }

        public override bool IsAlwaysVisible()
        {
            return true;
        }

        public override void VisitRemove(Shape s)
        {
            s.RemoveCircular(this);
        }

        public override void VisitMenu(MenuCreator mc)
        {
            mc.AcceptCircular(this);
        }
    }
}
