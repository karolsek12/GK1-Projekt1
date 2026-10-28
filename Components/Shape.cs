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

        private Edge? _deletedEdge;

        public ObservableCollection<BezierEdge> _bSplineCurve;

        public Shape(List<Vertex> vertices, List<Edge> edges)
        {
            _vertices = new ObservableCollection<Vertex>(vertices);
            _edges = new ObservableCollection<Edge>(edges);
            _circularEdges = new ObservableCollection<CircularEdge>();
            _bezierEdges = new ObservableCollection<BezierEdge>();
            _fakeEdges = new ObservableCollection<FakeEdge>();
            _controlPoints = new ObservableCollection<ControlVertex>();
            _bSplineCurve = new ObservableCollection<BezierEdge>();
            _deletedEdge = null;
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

        public ObservableCollection<BezierEdge> BSpline
        {
            get
            {
                return _bSplineCurve;
            }
        }

        public Edge? DeletedEdge
        {
            get
            {
                return _deletedEdge;
            }
        }



       public void DeleteEdge(Edge e){
            RemoveContinuity(e);
            e.VisitRemove(this);
            _deletedEdge = e;
       }

        public bool HasDeletedEdge()
        {
            return _deletedEdge != null;
        }

        public void RestoreEdge()
        {
            Edge restored = new Edge(_deletedEdge!.P1, _deletedEdge.P2);

            _deletedEdge = null;

            _edges.Add(restored);
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

            if (e1.P1 == e2.P2)
            {
                return new Edge(e1.P2, e2.P1);
            }

            return new Edge(e1.P1, e2.P2);
        }

        public void EraseVertex(Vertex vertex)
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

            foreach (Edge edge in toConnect)
            {
                edge.VisitRemove(this);

            }
            Edge newest = ConnectEdges(toConnect[0], toConnect[1]);

            _edges.Add(newest);
        }

        public void SubdivideEdge(Edge edge)
        {
            RemoveContinuity(edge);

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

        }

        public BoundingBox GetBoundingBox()
        {
            int xMin = _vertices.Min(v => v.X);
            int xMax = _vertices.Max(v => v.X);

            int yMin = _vertices.Min(v => v.Y);
            int yMax = _vertices.Max(v => v.Y);

            return new BoundingBox(new Point(xMin, yMin), new Point(xMax, yMax));
        }

        public void MoveAll(Vector vector)
        {
            foreach(CircularEdge c in CircularEdges)
            {
                c.Freeze();
            }

            foreach (Vertex v in _vertices)
            {
                v.Shift(vector);
            }

            foreach (ControlVertex c in _controlPoints)
            {
                c.Shift(vector);
            }

            foreach (CircularEdge c in CircularEdges)
            {
                c.Unfreeze();
            }
        }

        public  List<Point> UpdateAllEdges()
        {
            List<Point> newer = new List<Point>();

            foreach (Edge e in _edges)
            {
                (List<Point> o, List<Point> n) = e.GetPoints();


                foreach (Point pp in n)
                {
                    newer.Add(pp);
                }
            }

            return newer;
        }

        public void  MakeEdgeCircular(Edge edge)
        {
            edge.VisitRemove(this);
            CircularEdge circular = new CircularEdge(edge.P1, edge.P2);

            _circularEdges.Add(circular);
            _edges.Add(circular);


            TrySetG0(circular);

        }

        public void MakeEdgeBezier(Edge edge)
        {
            edge.VisitRemove(this);
            BezierEdge bezierEdge = new BezierEdge(edge.P1, edge.P2);

            _bezierEdges.Add(bezierEdge);
            _edges.Add(bezierEdge);

            _controlPoints.Add(bezierEdge.V2);
            _controlPoints.Add(bezierEdge.V1);

            foreach (FakeEdge fake in bezierEdge.GetFakeEdges())
            {
                _fakeEdges.Add(fake);
            }


            TrySetC1(bezierEdge);

        }

        public void MakeEdgeStraight(Edge edge)
        {
            edge.VisitRemove(this);
            _edges.Remove(edge);

            Edge straight = new Edge(edge.P1, edge.P2);

            _edges.Add(straight);

        }

        public void RemoveStraight(Edge edge)
        {
            edge.CleanupPerpendicular();
            _edges.Remove(edge);
        }

        public void RemoveCircular(CircularEdge cEdge)
        {
            _circularEdges.Remove(cEdge);
            _edges.Remove(cEdge);

            RemoveContinuity(cEdge);
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

            RemoveContinuity(bEdge);

        }

        private void RemoveContinuity(Edge edge)
        {
            TryRemoveContinuity(edge);

            if (edge.P1.Continuity != Continuity.NotSet)
                edge.P1.Continuity = Continuity.G0;

            if (edge.P2.Continuity != Continuity.NotSet)
                edge.P2.Continuity = Continuity.G0;
        }

        private void TryRemoveContinuity(Edge bEdge)
        {
            Vertex[] vertices = [bEdge.P1, bEdge.P2];

            for (int i = 0; i < 2; i++)
            {

                foreach (Edge e in _edges)
                {
                    if (e.HasVertex(vertices[i]) && !e.HasVertex(vertices[(i + 1) % 2]))
                    {
                        if(e is not BezierEdge && e is not CircularEdge)
                        {
                            vertices[i].Continuity = Continuity.NotSet;
                        }
                    }
                }


            }
        }

        private void TrySetC1(BezierEdge bEdge)
        {
            Vertex[] vertices = [bEdge.P1, bEdge.P2];
            Vector tangent = new Vector(0, 0);
            for (int i = 0; i < 2; i++){

                foreach (Edge e in _edges)
                {
                    if (e.HasVertex(vertices[i]) && !e.HasVertex(vertices[(i + 1) % 2]))
                    {
                        tangent = e.GetTangent(vertices[i]);
                    }
                }

                bEdge.SetTangent(tangent, vertices[i]);

                if (vertices[i].Continuity == Continuity.NotSet)
                {
                    

                    vertices[i].Continuity = Continuity.C1;
                    foreach (Edge e in Edges)
                    {
                       
                        if (e.HasVertex(vertices[i]) && LocateOtherEdge(vertices[i],e) == e)
                        {
                            vertices[i].Continuity = Continuity.G0;
                            break;
                        }

                        if (e.HasVertex(vertices[i]) && e.FixedLength != null)
                        {
                            vertices[i].Continuity = Continuity.G1;
                            break;
                        }
                    }
                }
                    

            }
        }

        private void TrySetG0(CircularEdge circular)
        {
            Vertex[] vertices = [circular.P1, circular.P2];
            Vector tangent = new Vector(0, 0);
            for (int i = 0; i < 2; i++)
            {



                if (vertices[i].Continuity == Continuity.NotSet || vertices[i].Continuity == Continuity.G0)
                {
                    vertices[i].Continuity = Continuity.G0;
                }
                

            }
            for (int i = 0; i < 2; i++)
            {

                if (vertices[i].Continuity != Continuity.NotSet && vertices[i].Continuity != Continuity.G0)
                {
                    foreach (Edge e in _edges)
                    {
                        if (e.HasVertex(vertices[i]) && !e.HasVertex(vertices[(i + 1) % 2]))
                        {
                            tangent = e.GetTangent(vertices[i]);
                        }
                    }

                    circular.SetTangent(tangent, vertices[i]);
                }

            }

        }

        private Edge FindEdge(Vertex v1, Vertex v2)
        {
            foreach (Edge e in _edges)
            {
                if (e.HasVertex(v1) && e.HasVertex(v2))
                    return e;
            }

            return null!;
        }

        private Edge LocateOtherEdge(Vertex v, Edge wrong)
        {
            foreach (Edge e in Edges)
            {
                if (e.HasVertex(v) && e != wrong)
                    return e;
            }

           return wrong;
        }

        private void UpdateVertex(Container curr,Graph g,Container? prev = null)
        {
            List<(Container,ConnectionType)> neigh = g.GetNeighbors(curr);
            
            foreach((Container c, ConnectionType ct) in neigh)
            {
                if (prev == c)
                    continue;

                if (c.HasVertex())
                {
                    Vertex v = c.Vertex!;

                    if (curr.HasVertex() && v.Continuity == Continuity.ControlPoint)
                    {
                        Vertex vcurr = curr.Vertex!;
                        foreach (BezierEdge be in BezierEdges)
                        {
                            if (be.HasVertex(v))
                            {
                                be.FixConstraints(LocateOtherEdge(vcurr,be), vcurr);
                            }
                        }

                    }
 
                }
                else
                {
                    Edge e = c.Edge!;

                    if(ct != ConnectionType.Moving)
                    {
                        if((ct & ConnectionType.Continuity) == ConnectionType.Continuity)
                        {
                            Vertex vcurr = curr.Vertex!;
                            e.FixConstraints(LocateOtherEdge(vcurr, e), vcurr);
                        }else if((ct & ConnectionType.FixedSize) == ConnectionType.FixedSize)
                        {
                            Vertex vcurr = curr.Vertex!;
                            e.FixLengthConstraint(vcurr);
                        }else if(ct == ConnectionType.Perpendicular)
                        {
                            Edge ecurr = curr.Edge!;
                            e.FixPerpendicularConstraint(ecurr);
                        }

                    }

                }
                UpdateVertex(c, g, curr);
            }

        }

        public Graph CreateConstraintsGraph()
        {
            Graph g = new Graph(Vertices.ToList(),ControlPoints.ToList(),Edges.ToList());

            foreach (Vertex v in Vertices)
            {
                foreach (Edge e in _edges)
                {
                    if (e.HasVertex(v))
                    {
                        g.AddEdge(v, e, ConnectionType.Moving, false);

                    }
                }
            }

            foreach (Vertex v in Vertices)
            {
                if(v.Continuity != Continuity.NotSet && v.Continuity != Continuity.G0)
                {
                    foreach(Edge e in _edges)
                    {
                        if (e.HasVertex(v))
                        {
                            if (!_bezierEdges.Contains(e))
                            {
                                g.AddEdge(v,e, ConnectionType.Continuity);
                                if(e.P1 == v)
                                {
                                    g.AddEdge(e, e.P2, ConnectionType.Continuity,false);
                                }
                                else
                                {
                                    g.AddEdge(e, e.P1, ConnectionType.Continuity, false);
                                }
                            }
                            else
                            {
                                if(v == e.P1)
                                {
                                    g.AddEdge(((BezierEdge)e).V1, v,ConnectionType.Continuity);
                                }
                                else
                                {
                                    g.AddEdge(((BezierEdge)e).V2,v, ConnectionType.Continuity);
                                }
                            }
                            
                        }
                    }
                }
            }

            foreach (Edge e in _edges)
            {
                if (e.FixedLength != null)
                {
                    g.AddEdge(e,e.P1,ConnectionType.FixedSize);
                    g.AddEdge(e,e.P2, ConnectionType.FixedSize);
                }

                if(e.Perpendicular != null)
                {
                    g.AddEdge(e, e.Perpendicular, ConnectionType.Perpendicular);
                    g.AddEdge(e, e.P1, ConnectionType.Perpendicular, false);
                    g.AddEdge(e, e.P2, ConnectionType.Perpendicular, false);
                }
            }


            return g;
        }

        public bool CheckCanMove(Edge edge)
        {
            Graph g = CreateConstraintsGraph();
            return !g.CheckCycle(edge);
        }

        public bool CheckCanMove(Vertex point)
        {
            Graph g = CreateConstraintsGraph();
            return !g.CheckCycle(point);
        }

        public void MoveVertex(Vertex point, int x, int y, bool fakeMove = false)
        {
            Graph g = CreateConstraintsGraph();

            if (g.CheckCycle(point) && !fakeMove)
            {
                MoveAll(new Vector(new Point(x, y), new Point(point)));
                return;
            }

            if(!fakeMove)
                point.UpdatePosition(x, y);



            UpdateVertex(g.GetContainer(point), g);
            RecalcBSpline();
        }

        public void FixLength(Edge edge, int value)
        {
            int prev = edge.GetLength();
            RemoveContinuity(edge);
            edge.AlterLength(value);
            edge.FixLength();
            if (CheckCanMove(edge.P1))
            {
                MoveVertex(edge.P1, 0, 0, true);
            }
            else
            {
                edge.UnfixLength();
                edge.AlterLength(prev);
                edge.FixLength();
            }

            
        }

        public void UnfixLength(Edge edge)
        {
            edge.UnfixLength();
        }

        public Vertex GetOtherVertex(Edge e, Vertex v)
        {
            if(e.P1 == v)
            {
                return e.P2;
            }
            else
            {
                return e.P1;
            }
        }

        public ExactPoint GetTriple(Edge e, Vertex from)
        {
            Vector v = e.GetTangent(from);
            v.x *= -1;
            v.y *= -1;

            ExactVector ev = new ExactVector(v);

            ev.x /= 3;
            ev.y /= 3;

            ExactPoint ep = new ExactPoint(new Point(from));

            ep.Shift(ev);

            return ep;
        }

        public Vertex GetShared(Edge e1, Edge e2)
        {
            if(e1.P1 == e2.P1 || e1.P1 == e2.P2)
            {
                return e1.P1;
            }else 
            {
                return e1.P2;
            }
        }

        public ExactPoint GetMid(Edge e)
        {
            ExactPoint h = new ExactPoint(new Point(e.P1) + new Point(e.P2));
            h = 0.5 * h;

            return h;
        }
        public ExactPoint GetMidTriple(Edge e1, Edge e2)
        {
            ExactPoint h1 = new ExactPoint(new Point(e1.P1) + new Point(e1.P2));
            h1 = 0.5 * h1;
            ExactPoint h2 = GetTriple(e2, GetShared(e1, e2));

            ExactPoint hh = new ExactPoint(h1.x+h2.x,h1.y+h2.y);
            hh = 0.5 * hh;

            return hh;
        }

        public ExactPoint GetMidTripleTriple(Edge e1, Edge e2)
        {
            Vertex v = GetShared(e1, e2);

            ExactPoint ev1 = GetTriple(e1, v);
            ExactPoint ev2 = GetTriple(e2, v);

            ExactPoint h = new ExactPoint(ev1.x + ev2.x, ev1.y+ev2.y);
            h = 0.5 * h;

            return h;
        }

        public void RecalcBSpline()
        {
            //4 krawedzi
            _bSplineCurve.Clear();


            if(_deletedEdge != null)
            {
                if (_edges.Count < 4)
                    return;

                List<Edge> list = new List<Edge>();

                Vertex v = _deletedEdge.P1;
                Edge e = LocateOtherEdge(v, _deletedEdge);
                BezierEdge bedge;

                list.Add(e);
                while (v != _deletedEdge.P2)
                {
                    
                    v = GetOtherVertex(e, v);
                    e = LocateOtherEdge(v, e);
                    list.Add(e);
                }


                bedge = new BezierEdge(_deletedEdge.P1, new Vertex(GetMidTriple(list[1], list[2])));

                bedge.V1 = new ControlVertex(GetOtherVertex(_deletedEdge, _deletedEdge.P1));
                bedge.V2 = new ControlVertex(new Vertex(GetTriple(list[2], GetShared(list[1], list[2]))));

                _bSplineCurve.Add(bedge);

                bedge = new BezierEdge(_bSplineCurve[0].V3, new Vertex(GetMidTripleTriple(list[2], list[3])));
                bedge.V1 = new ControlVertex(new Vertex(GetMid(list[1])));
                bedge.V2 = new ControlVertex(new Vertex(GetTriple(list[3], GetShared(list[2], list[3]))));

                _bSplineCurve.Add(bedge);

                for (int i = 2; i < list.Count-2; i++)
                {
                    bedge = new BezierEdge(_bSplineCurve[i - 1].V3, new Vertex(GetMidTripleTriple(list[i + 1], list[i + 2])));
                    bedge.V1 = new ControlVertex(new Vertex(GetTriple(list[i], GetShared(list[i], list[i + 1]))));
                    bedge.V2 = new ControlVertex(new Vertex(GetTriple(list[i+2], GetShared(list[i+1], list[i + 2]))));
                    _bSplineCurve.Add(bedge);
                }



            }
            else
            {

            }
        }
    }
}
