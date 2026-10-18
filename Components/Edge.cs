using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GK1_1.Components
{
    public enum EdgeType
    {
        StraightEdge,
        NotStraightEdge
    }


    public class Edge : SuperNotifyChanged
    {

        protected Vertex _p1;
        protected Vertex _p2;
        protected List<Point> _pixels;
        protected EdgeType _edgeType;

        public Vertex P1
        {
            get
            {
                return _p1;
            }
            set
            {
                SetProperty(ref _p1, value);
            }
        }

        public Vertex P2
        {
            get
            {
                return _p2;
            }
            set
            {
                SetProperty(ref _p2, value);
            }
        }

        public List<Point> Pixels
        {
            get
            {
                return _pixels;
            }
        }

        public EdgeType EdgeType
        {
            get
            {
                return _edgeType;
            }
        }

        public Edge(Vertex p1, Vertex p2)
        {
            _p1 = p1;
            _p2 = p2;
            _pixels = Rasterization.Bresenham(new Point(P1), new Point(P2));
            _edgeType = EdgeType.StraightEdge;
        }

        public virtual bool CheckHit(int x, int y)
        {
            return HitDetection.IsNearLine(new Point(P1), new Point(P2), x, y);
        }

        public virtual bool CheckContainsPoint(Point p)
        {
            if (_p1.X == p.x && _p1.Y == p.y)
                return true;

            if (_p2.X == p.x && _p2.Y == p.y)
                return true;

            return false;
        }



        public virtual (List<Point>,List<Point>) GetPoints()
        {
            List<Point> older = new List<Point>();
            foreach (Point p in _pixels)
            {
                older.Add(p);
            }

            _pixels = Rasterization.Bresenham(new Point(P1), new Point(P2));

            return (older, _pixels);
        }

    }
}
