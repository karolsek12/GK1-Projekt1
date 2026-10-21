using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GK1_1.Components.Edges;

namespace GK1_1.Components
{
    public class Shape
    {
        private ObservableCollection<Vertex> _vertices;

        private ObservableCollection<Edge> _edges;

        private ObservableCollection<CircularEdge> _circularEdges;

        private ObservableCollection<BezierEdge> _bezierEdges;

        private ObservableCollection<FakeEdge> _fakeEdges;

        private ObservableCollection<ControlVertex> _controlPoints;

        public Shape(List<Vertex> vertices, List<Edge> edges)
        {
            _vertices = new ObservableCollection<Vertex>(vertices);
            _edges = new ObservableCollection<Edge>(edges);
            _circularEdges = new ObservableCollection<CircularEdge>();
            _bezierEdges = new ObservableCollection<BezierEdge>();
            _fakeEdges = new ObservableCollection<FakeEdge>();
            _controlPoints = new ObservableCollection<ControlVertex>();
        }

        public ObservableCollection<Edge> Edges
        {
            get
            {
                return _edges;
            }
        }

        public ObservableCollection<Vertex> Vertices
        {
            get
            {
                return _vertices;
            }
        }

        public ObservableCollection<ControlVertex> ControlPoints
        {
            get
            {
                return _controlPoints;
            }
        }

        public ObservableCollection<CircularEdge> CircularEdges
        {
            get
            {
                return _circularEdges;
            }
        }

        public ObservableCollection<BezierEdge> BezierEdges
        {
            get
            {
                return _bezierEdges;
            }
        }

        public ObservableCollection<FakeEdge> FakeEdges
        {
            get
            {
                return _fakeEdges;
            }
        }

        private Edge ConnectEdges(Edge e1, Edge e2)
        {
            if (e1.P1 == e2.P1)
            {
                return new Edge(e1.P2, e2.P2);
            }

            if (e1.P2 == e2.P2)
            {
                return new Edge(e1.P1, e2.P1);
            }

            if(e1.P1 == e2.P2)
            {
                return new Edge(e1.P2, e2.P1);
            }

            return new Edge(e1.P1, e2.P2);
        }

        public (List<Point>, List<Point>) EraseVertex(Vertex vertex)
        {
            List<Edge> toConnect = new List<Edge>();

            _vertices.Remove(vertex);

            foreach (Edge edge in _edges)
            {
                if (edge.P1 == vertex || edge.P2 == vertex)
                    toConnect.Add(edge);
            }

            if (toConnect.Count != 2)
                throw new Exception("Impossible, something horrible happened!!!");

            foreach(Edge edge in toConnect)
            {
                edge.VisitRemove(this);

            }
            Edge newest = ConnectEdges(toConnect[0], toConnect[1]);

            _edges.Add(newest);

            List<Point> older = new List<Point>();
            List<Point> newer = new List<Point>();

            foreach(Point p in newest.Pixels)
            {
                newer.Add(p);
            }

            for(int i = 0; i < 2; i++)
            {
                foreach(Point p in toConnect[i].Pixels)
                {
                    older.Add(p);
                }
            }


            return (older,newer);
        }
        
        public (List<Point>, List<Point>) SubdivideEdge(Edge edge)
        {
            int midX = edge.P1.X + edge.P2.X;
            midX /= 2;

            int midY = edge.P1.Y + edge.P2.Y;
            midY /= 2;

            Vertex midPoint = new Vertex(midX, midY);

            Edge e1 = new Edge(edge.P1, midPoint);
            Edge e2 = new Edge(edge.P2, midPoint);

            edge.VisitRemove(this);
            _edges.Add(e1);
            _edges.Add(e2);

            _vertices.Add(midPoint);

            List<Point> older = new List<Point>();
            List<Point> newer = new List<Point>();

            foreach (Point p in e1.Pixels)
            {
                newer.Add(p);
            }

            foreach (Point p in e2.Pixels)
            {
                newer.Add(p);
            }

            foreach (Point p in edge.Pixels)
            {
                older.Add(p);
            }

            return (older, newer);
        }

        public (List<Point>,List<Point>) UpdateEdges(Point p)
        {
            List<Point> older = new List<Point>();
            List<Point> newer = new List<Point>();

            foreach(Edge e in _edges)
            {
                if (e.CheckContainsPoint(p))
                {
                    (List<Point> o,List<Point> n) = e.GetPoints();

                    foreach(Point pp in o)
                    {
                        older.Add(pp);
                    }

                    foreach(Point pp in n)
                    {
                        newer.Add(pp);
                    }
                }
            }

            return (older, newer);  
        }
        public BoundingBox GetBoundingBox()
        {
            int xMin = _vertices.Min(v => v.X);
            int xMax = _vertices.Max(v => v.X);

            int yMin = _vertices.Min(v => v.Y);
            int yMax = _vertices.Max(v => v.Y);

            return new BoundingBox(new Point(xMin,yMin),new Point(xMax,yMax));
        }

        public void MoveAll(Vector vector)
        {
            foreach(Vertex v in _vertices)
            {
                v.Shift(vector);
            }

            foreach(ControlVertex c in _controlPoints)
            {
                c.Shift(vector);
            }
        }

        public (List<Point>, List<Point>) UpdateAllEdges()
        {
            List<Point> older = new List<Point>();
            List<Point> newer = new List<Point>();

            foreach (Edge e in _edges)
            {
                (List<Point> o, List<Point> n) = e.GetPoints();

                foreach (Point pp in o)
                {
                    older.Add(pp);
                }

                foreach (Point pp in n)
                {
                    newer.Add(pp);
                }
            }

            return (older, newer);
        }

        public (List<Point>, List<Point>) MakeEdgeCircular(Edge edge)
        {
            edge.VisitRemove(this);
            CircularEdge circular = new CircularEdge(edge.P1, edge.P2);

            _circularEdges.Add(circular);
            _edges.Add(circular);

            return (edge.Pixels, new List<Point>());

        }

        public (List<Point> older, List<Point> newer) MakeEdgeBezier(Edge edge)
        {
            edge.VisitRemove(this);
            BezierEdge bezierEdge = new BezierEdge(edge.P1, edge.P2);

            _bezierEdges.Add(bezierEdge);
            _edges.Add(bezierEdge);

            _controlPoints.Add(bezierEdge.V2);
            _controlPoints.Add(bezierEdge.V1);

            foreach(FakeEdge fake in bezierEdge.GetFakeEdges())
            {
                _fakeEdges.Add(fake);
            }

            return (edge.Pixels, new List<Point>());

        }

        public (List<Point> older, List<Point> newer) MakeEdgeStraight(Edge edge)
        {
            edge.VisitRemove(this);
            RemoveStraight(edge);

            Edge straight = new Edge(edge.P1, edge.P2);

            _edges.Add(straight);

            return (new List<Point>(), straight.Pixels);

        }

        public void RemoveStraight(Edge edge)
        {
            _edges.Remove(edge);
        }

        public void RemoveCircular(CircularEdge cEdge)
        {
            _circularEdges.Remove(cEdge);
            _edges.Remove(cEdge);
        }

        public void RemoveBezier(BezierEdge bEdge)
        {
            _bezierEdges.Remove(bEdge);
            _edges.Remove(bEdge);
            ControlVertex v1 = bEdge.V1;
            ControlVertex v2 = bEdge.V2;
            List<FakeEdge> killList = new List<FakeEdge>();
            foreach (FakeEdge fEdge in _fakeEdges)
            {
                if (fEdge.HasVertex(v1) || fEdge.HasVertex(v2))
                    killList.Add(fEdge);
            }
            foreach (FakeEdge fEdge in killList)
            {
                _fakeEdges.Remove(fEdge);
            }
            _controlPoints.Remove(v1);
            _controlPoints.Remove(v2);
        }
    }
}
