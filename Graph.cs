using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using GK1_1.Components;

namespace GK1_1
{
    public class Graph
    {
        private List<Vertex> _vertices;
        private List<ControlVertex> _fakeVertices;
        private bool[,] _edges;

        public Graph(List<Vertex> vertices, List<ControlVertex> fakeVertices)
        {
            _vertices = vertices;
            _edges = new bool[vertices.Count+fakeVertices.Count, vertices.Count + fakeVertices.Count];
            _fakeVertices = fakeVertices;
        }

        public void AddEdge(Vertex v1, Vertex v2)
        {
            int i1 = GetIndex(v1);
            int i2 = GetIndex(v2);

            _edges[i1, i2] = true;
            _edges[i2, i1] = true;
        }

        public int GetIndex(Vertex v)
        {
            for(int i = 0; i < _vertices.Count; i++)
            {
                if (_vertices[i] == v)
                    return i;
            }

            return GetIndexControl((ControlVertex)v);
        }

        public int GetIndexControl(ControlVertex c)
        {
            for (int i = 0; i < _fakeVertices.Count; i++)
            {
                if (_fakeVertices[i] == c)
                    return _vertices.Count+i;
            }

            return -1;
        }

        public bool CheckCycle(Vertex v)
        {
            bool[] visited = new bool[_vertices.Count+_fakeVertices.Count];

            return CheckCycleRec(visited, GetIndex(v));
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
                if (prev != j && _edges[curr, j])
                {
                    if (CheckCycleRec(visited, j, curr))
                        return true;
                }

            }

            return false;
        }

        public List<Vertex> GetNeighbors(Vertex v)
        {
            int i = GetIndex(v);
            List<Vertex> neigh = new List<Vertex>();
            for(int j = 0; j < _vertices.Count; j++)
            {
                if (_edges[i, j])
                {
                    neigh.Add(_vertices[j]);
                }
            }

            return neigh;
        }
    }
}
