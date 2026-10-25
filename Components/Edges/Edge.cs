using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GK1_1.Components.Edges
{

    public class Edge : SuperNotifyChanged
    {

        protected Vertex _p1;
        protected Vertex _p2;
        protected List<Point> _pixels;

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


        public Edge(Vertex p1, Vertex p2)
        {
            _p1 = p1;
            _p2 = p2;
            _pixels = Rasterization.Bresenham(new Point(P1), new Point(P2));
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

        public virtual bool IsAlwaysVisible()
        {
            return false;
        }
        
        public bool IsVisible
        {
            get
            {
                return IsAlwaysVisible();
            }
        }

        public virtual bool HasVertex(Vertex v)
        {
            return P1 == v || P2 == v;
        }

        public virtual void VisitRemove(Shape s)
        {
            s.RemoveStraight(this);
        }

        public virtual void VisitMenu(MenuCreator mc)
        {
            mc.AcceptStraight(this);
        }

        public virtual Vector GetTangent(Vertex vertex)
        {
            if(P1 == vertex)
            {
                return new Vector(new Point(P1), new Point(P2));
            }
            else
            {
                return new Vector(new Point(P2), new Point(P1));
            }
        }


        public virtual void SetTangent(Vector tangent, Vertex vertex)
        {
            ExactPoint from = new ExactPoint(new Point(vertex));
            ExactPoint to;
            if(vertex == P1)
            {
                to = new ExactPoint(new Point(P2));
            }
            else
            {
                to = new ExactPoint(new Point(P1));
            }

            ExactVector t = new ExactVector(tangent);

            from.Shift(t);

            if(vertex == P1)
            {
                P2.X = (int)from.x;
                P2.Y = (int)from.y;
            }
            else
            {
                P1.X = (int)from.x;
                P1.Y = (int)from.y;
            }
        }

        public void FixConstraints(Vertex target, Edge prev)
        {

            Vector tangent;

            if (target == P1)
            {
                tangent = prev.GetTangent(P2);
                SetTangent(tangent, P2);
            }
            else
            {
                tangent = prev.GetTangent(P1);
                SetTangent(tangent, P1);
            }

        }
    }
}
