using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
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

        private int? _fixedLength;
        private Edge? _perpendicular;
        private int? _perpendicularIndex;


        public Edge? Perpendicular
        {
            get
            {
                return _perpendicular;
            }
            set
            {
                SetProperty(ref _perpendicular, value);
            }
        }
        public int? Index
        {
            get
            {
                return _perpendicularIndex;
            }
            set
            {
                SetProperty(ref _perpendicularIndex, value);
            }
        }

        public int? FixedLength
        {
            get
            {
                return _fixedLength;
            }
            set
            {
                SetProperty(ref _fixedLength, value);
            }
        }

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
            _fixedLength = null;
            _perpendicular = null;
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
            ExactVector t = new ExactVector(tangent);

            if(_fixedLength != null)
            {
                t = t.GetNormalised();
                t.x *= (double)_fixedLength;
                t.y *= (double)_fixedLength;
            }

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

        public virtual void FixConstraints(Edge from, Vertex to)
        {
            Vector tangent = from.GetTangent(to);
            SetTangent(tangent, to);

        }

        public void FixLengthConstraint(Vertex from)
        {
            Vector tangent;
            if (from == P1)
            {
                tangent = GetTangent(P2);
                SetTangent(tangent, P1);
            }
            else
            {
                tangent = GetTangent(P1);
                SetTangent(tangent, P2);
            }


             
        }

        public int GetLength()
        {
            return (int)(new ExactVector(new Vector(new Point(P1), new Point(P2))).Length());
        }

        public void FixLength()
        {
            FixedLength = GetLength();
        }

        public void UnfixLength()
        {
            FixedLength = null;
        }

        public void AlterLength(int value)
        {
            Vector tangent = GetTangent(P1);
            Point origin = new Point(P2);

            ExactVector ev = new ExactVector(tangent);
            ev = ev.GetNormalised();
            ev.x *= value;
            ev.y *= value;

            origin.Shift(new Vector(ev));

            P1.X = origin.X;
            P1.Y = origin.Y;
            
        }

        public void FixPerpendicularConstraint(Edge from)
        {
            Vector tangent = from.GetTangent(from.P1);

            tangent = tangent.GetRotated(Math.PI / 2);

            SetTangent(tangent, P1);
            
        }

        public void CleanupPerpendicular()
        {
            if(Perpendicular == null)
            {
                Index = null;
                return;
            }

            Edge friend = Perpendicular!;
            friend.Index = null;
            friend.Perpendicular = null;
            Index = null;
            Perpendicular = null;
        }
    }
}
