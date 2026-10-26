using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK1_1.Components.Edges
{
    public class BezierEdge : Edge
    {
        private ControlVertex _v1;
        private ControlVertex _v2;
        private List<Point> _midPoints;
        private List<Line> _midLines;

        public Vertex V0
        {
            get
            {
                return P1;
            }
            set
            {
                P1 = value;
            }
        }

        public ControlVertex V1
        {
            get
            {
                return _v1;
            }
            set
            {
                SetProperty(ref _v1, value);
            }
        }

        public ControlVertex V2
        {
            get
            {
                return _v2;
            }
            set
            {
                SetProperty(ref _v2, value);
            }
        }

        public Vertex V3
        {
            get
            {
                return P2;
            }
            set
            {
                P2 = value;
            }
        }

        public List<Point> MidPoints
        {
            get
            {
                return _midPoints;
            }
            set
            {
                SetProperty(ref _midPoints, value);
            }
        }

        public List<Line> MidLines
        {
            get
            {
                return _midLines;
            }
            set
            {
                SetProperty(ref _midLines, value);
            }

        }



        public BezierEdge(Vertex v0,Vertex v3) : base(v0, v3)
        {
            _pixels = new List<Point>();
            _midPoints = new List<Point>();
            _midLines = new List<Line>();
            (_v1, _v2) = GetPointsBetween(v0, v3);
            MidPoints = GetMidPoints();
            MidLines = GetMidLines();
            V0.PropertyChanged += RecalculatePoints;
            V1.PropertyChanged += RecalculatePoints;
            V2.PropertyChanged += RecalculatePoints;
            V3.PropertyChanged += RecalculatePoints;

        }

        private void RecalculatePoints(object? sender, PropertyChangedEventArgs e)
        {
            MidPoints = GetMidPoints();
            MidLines = GetMidLines();
        }

        private List<Point> GetMidPoints()
        {
            return Rasterization.BezierMidPoints(new Point(V0), new Point(V1), new Point(V2), new Point(V3));
        }

        private List<Line> GetMidLines()
        {
            List<Line> lines = new List<Line>();
            for (int i = 1; i < _midPoints.Count; i++)
            {
                lines.Add(new Line(_midPoints[i - 1], _midPoints[i]));
            }

            return lines;
        }
        public override (List<Point>, List<Point>) GetPoints()
        {
            return (new List<Point>(), new List<Point>());
        }

        private static (ControlVertex, ControlVertex) GetPointsBetween(Vertex v0, Vertex v3)
        {
            Vector v = new Vector(new Point(v3),new Point(v0));
            v.x /= 2;
            v.y /= 2;
            Point p1 = new Point(v0);
            Vector v1 = v.GetRotated(Math.PI / 2);
            p1.Shift(v1);

            v.x *= -1;
            v.y *= -1;

            Point p2 = new Point(v3);
            Vector v2 = v.GetRotated(-Math.PI / 2);

            p2.Shift(v2);

            ControlVertex c1 = new ControlVertex(p1.x,p1.y);
            ControlVertex c2 = new ControlVertex(p2.x,p2.y);
            return (c1, c2);
        }
        public override bool IsAlwaysVisible()
        {
            return true;
        }

        public List<FakeEdge> GetFakeEdges()
        {
            List<FakeEdge> flist = new List<FakeEdge>();

            flist.Add(new FakeEdge(V0, V1));
            flist.Add(new FakeEdge(V1, V2));
            flist.Add(new FakeEdge(V2, V3));

            return flist;
        }

        public override void VisitRemove(Shape s)
        {
            s.RemoveBezier(this);
        }

        public override void VisitMenu(MenuCreator mc)
        {
            mc.AcceptBezier(this);
        }

        public override void SetTangent(Vector tangent, Vertex vertex)
        {
            ExactPoint p = new ExactPoint(0, 0);
            p.Shift(tangent);
            p.x /= 3.0;
            p.y /= 3.0;

            ExactPoint v;
            if (vertex == V0)
            {
                
                v = new ExactPoint(new Point(V0));
                
                p = p + v;

                V1.X = (int)p.x;
                V1.Y = (int)p.y;
            }
            else
            {
                v = new ExactPoint(new Point(V3));

                p = p + v;  

                V2.X = (int)p.x;
                V2.Y = (int)p.y;
            }

        }

        public override Vector GetTangent(Vertex vertex)
        {
            Vector v = new Vector(0, 0);
            if(vertex == V3)
            {
                v = new Vector(new Point(V3) - new Point(V2));
            }
            else
            {
                v = new Vector(new Point(V0) - new Point(V1));
            }


            v.x *= 3; // TODO disgusting;
            v.y *= 3;

            return v;
        }

        public override bool HasVertex(Vertex v)
        {
            return V0 == v || V1 == v || V2 == v || V3 == v;
        }

        public override void FixConstraints(Vertex target, Edge prev, bool continous)
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
