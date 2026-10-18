using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK1_1.Components
{
    public class CircularEdge : Edge
    {
        public CircularEdge(Vertex p1, Vertex p2) : base(p1, p2)
        {
            _pixels = new List<Point>();
            _edgeType = EdgeType.NotStraightEdge;
        }


        public override (List<Point>, List<Point>) GetPoints()
        {
            return (new List<Point>(), new List<Point>());
        }
    }
}
