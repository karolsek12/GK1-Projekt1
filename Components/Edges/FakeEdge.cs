using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK1_1.Components.Edges
{
    public class FakeEdge : Edge
    {
        public FakeEdge(Vertex p1, Vertex p2) : base(p1, p2)
        {
            _pixels = new List<Point>();
        }
        public override bool IsAlwaysVisible()
        {
            return true;
        }
    }
}
