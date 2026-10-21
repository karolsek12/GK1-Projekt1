using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GK1_1.Components.Edges;

namespace GK1_1
{
    public class MenuCreator
    {
        private static int EdgeMenuSize = 4;
        private bool[] _isEnabled;
        public MenuCreator() {
            _isEnabled = new bool[EdgeMenuSize];
        }

        public bool[] GetEdgeMenuEnabled(Edge e)
        {
            //0 subdivide
            //1 straight
            //2 circular
            //3 bezier
            e.VisitMenu(this);
            return _isEnabled;
        }

        public void AcceptBezier(BezierEdge edge)
        {
            _isEnabled = [false, true, false, false];
        }

        public void AcceptCircular(CircularEdge edge)
        {
            _isEnabled = [false, true, false, false];
        }

        public void AcceptStraight(Edge edge)
        {
            _isEnabled = [true, false, true, true];
        }
    }
}
