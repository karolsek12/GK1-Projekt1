using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK1_1.Components
{
    public class Shape
    {
        private ObservableCollection<Vertex> _vertices;

        private ObservableCollection<Edge> _edges;

        private ObservableCollection<CircularEdge> _circularEdges;

        public Shape(List<Vertex> vertices, List<Edge> edges)
        {
            _vertices = new ObservableCollection<Vertex>(vertices);
            _edges = new ObservableCollection<Edge>(edges);
            _circularEdges = new ObservableCollection<CircularEdge>();
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

        public ObservableCollection<CircularEdge> CircularEdges
        {
            get
            {
                return _circularEdges;
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
                _edges.Remove(edge);
                if(edge.EdgeType == EdgeType.NotStraightEdge)
                {
                    _circularEdges.Remove((CircularEdge)edge);
                }
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

            _edges.Remove(edge);
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
            _edges.Remove(edge);
            CircularEdge circular = new CircularEdge(edge.P1, edge.P2);

            _edges.Add(circular);

            _circularEdges.Add(circular);

            return (edge.Pixels, new List<Point>());

        }

        internal (List<Point> older, List<Point> newer) MakeEdgeStraight(Edge edge)
        {
            CircularEdge circular = (CircularEdge) edge;
            _circularEdges.Remove(circular);

            _edges.Remove(edge);

            Edge straight = new Edge(edge.P1, edge.P2);

            _edges.Add(straight);

            return (new List<Point>(), straight.Pixels);

        }
    }
}
