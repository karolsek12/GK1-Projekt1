using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using GK1_1.Components;
using GK1_1.Components.Edges;

namespace GK1_1
{
    public enum ConnectionType
    {
        None = 0,
        Moving = 1,
        Continuity = 2,
        FixedSize = 4,
        Perpendicular = 8
    }

    public class Container
    {
        private Vertex? _vertex;
        private Edge? _edge;

        public Vertex? Vertex => _vertex;
        
        public Edge? Edge => _edge;


        public bool HasEdge()
        {
            return _edge != null;
        }

        public bool HasVertex()
        {
            return _vertex != null;
        }

        public Container(Edge edge)
        {
            _edge = edge;
            _vertex = null;
        }

        public Container(Vertex vertex)
        {
            _vertex = vertex;
            _edge = null;
        }

        public bool HasComponent(Edge e)
        {
            return _edge == e;
        }

        public bool HasComponent(Vertex v)
        {
            return _vertex == v;
        }
    }
    public class Graph
    {
        private List<Container> _vertices;
        private ConnectionType[,] _edges;

        private int size;

        public Graph(List<Vertex> vertices, List<ControlVertex> fakeVertices, List<Edge> realEdges)
        {
            _vertices = new List<Container>();
            foreach (Vertex v in vertices)
            {
                _vertices.Add(new Container(v));
            }

            foreach (ControlVertex v in fakeVertices)
            {
                _vertices.Add(new Container(v));
            }

            foreach(Edge e in realEdges)
            {
                _vertices.Add(new Container(e));
            }

            size = vertices.Count + fakeVertices.Count + realEdges.Count;

            _edges = new ConnectionType[size,size];

            for(int i = 0; i < size; i++)
            {
                for(int j = 0; j < size; j++)
                {
                    _edges[i, j] = ConnectionType.None;
                }
            }
            
        }

        public void AddEdge(Vertex vfrom, Vertex vto,ConnectionType ct,bool twoWay = true)
        {
            int i1 = GetIndex(vfrom);
            int i2 = GetIndex(vto);

            _edges[i1, i2] |= ct;
            if(twoWay)
                _edges[i2, i1] |= ct;   
        }

        public void AddEdge(Vertex vfrom, Edge eto, ConnectionType ct, bool twoWay = true)
        {
            int i1 = GetIndex(vfrom);
            int i2 = GetIndex(eto);

            _edges[i1, i2] |= ct;
            if (twoWay)
                _edges[i2, i1] |= ct;
        }

        public void AddEdge(Edge efrom, Edge eto, ConnectionType ct, bool twoWay = true)
        {
            int i1 = GetIndex(efrom);
            int i2 = GetIndex(eto);

            _edges[i1, i2] |= ct;
            if (twoWay)
                _edges[i2, i1] |= ct;
        }

        public void AddEdge(Edge efrom, Vertex vto, ConnectionType ct, bool twoWay = true)
        {
            int i1 = GetIndex(efrom);
            int i2 = GetIndex(vto);

            _edges[i1, i2] |= ct;
            if (twoWay)
                _edges[i2, i1] |= ct;
        }

        public int GetIndex(Vertex v)
        {
            for(int i = 0; i < _vertices.Count; i++)
            {
                if (_vertices[i].HasComponent(v))
                    return i;
            }

            return -1;
        }

        public int GetIndex(Edge e)
        {
            for (int i = 0; i < _vertices.Count; i++)
            {
                if (_vertices[i].HasComponent(e))
                    return i;
            }

            return -1;
        }

        public bool CheckCycle(Vertex v)
        {
            bool[] visited = new bool[size];

            return CheckCycleRec(visited, GetIndex(v));
        }

        public bool CheckCycle(Edge e)
        {
            bool[] visited = new bool[size];

            return CheckCycleRec(visited, GetIndex(e));
        }

        private bool CheckCycleRec(bool[] visited, int curr, int prev = -1)
        {
            if (visited[curr])
            {
                return true;
            }
            
            visited[curr] = true;

            for (int j = 0; j < _vertices.Count; j++)
            {

                if (prev != j && _edges[curr, j] != ConnectionType.None)
                {

                    if (CheckCycleRec(visited, j, curr))
                        return true;
                }

            }
            visited[curr] = false;
            return false;
        }

        public List<(Container, ConnectionType)> GetNeighbors(Vertex v)
        {
            int i = GetIndex(v);
            List<(Container, ConnectionType)> neigh = new List<(Container, ConnectionType)>();
            for (int j = 0; j < _vertices.Count; j++)
            {
                if (_edges[i, j] != ConnectionType.None)
                {
                    neigh.Add((_vertices[j], _edges[i, j]));
                }
            }

            return neigh;
        }

        public List<(Container,ConnectionType)> GetNeighbors(Edge e)
        {
            int i = GetIndex(e);
            List<(Container, ConnectionType)> neigh = new List<(Container, ConnectionType)>();
            for (int j = 0; j < _vertices.Count; j++)
            {
                if (_edges[i, j] != ConnectionType.None)
                {
                    neigh.Add((_vertices[j], _edges[i,j]));
                }
            }

            return neigh;
        }

        public List<(Container, ConnectionType)> GetNeighbors(Container c)
        {
            if (c.HasVertex())
            {
                return GetNeighbors(c.Vertex!);
            }
            else
            {
                return GetNeighbors(c.Edge!);
            }
        }

        public Container GetContainer(Vertex v)
        {
            foreach(Container c in _vertices)
            {
                if(c.HasVertex() && c.Vertex == v)
                {
                    return c;
                }
            }
            return null!;
        }
    }
}
