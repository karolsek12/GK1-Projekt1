using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using GK1_1.Components;
using GK1_1.Components.Edges;

namespace GK1_1
{
    public class MenuCreator
    {
        private static int EdgeMenuSize = 6;
        private bool[] _isEnabled;

        private static int VertexMenuSize = 4;
        private bool[] _isVertexEnabled;
        public MenuCreator() {
            _isEnabled = new bool[EdgeMenuSize];
            _isVertexEnabled = new bool[VertexMenuSize];
        }

        public bool[] GetEdgeMenuEnabled(Edge e)
        {
            //0 subdivide
            //1 straight
            //2 circular
            //3 bezier
            //4 fix Length
            e.VisitMenu(this);
            return _isEnabled;
        }

        public bool[] GetVertexMenuEnabled(Vertex v, Shape s)
        {
            //0 delete
            //1 G0
            //2 G1
            //3 C1

            if (v.Continuity == Continuity.NotSet)
                return [s.Vertices.Count > 3, false, false, false];

            _isVertexEnabled = [s.Vertices.Count > 3, true, true, true];

            //G0
            if (v.Continuity == Continuity.G0)
            {
                _isVertexEnabled[1] = false;
            }

            //G1
            if (v.Continuity == Continuity.G1)
            {
                _isVertexEnabled[2] = false;
            }
            else
            {
                foreach (Edge e in s.CircularEdges)
                {
                    if (e.HasVertex(v))
                    {
                        if (e.P1.Continuity == Continuity.G1 || e.P2.Continuity == Continuity.G1)
                        {
                            _isVertexEnabled[2] = false;
                            break;
                        }
                            
                    }
                }
                if (_isVertexEnabled[2])
                {
                    Continuity c = v.Continuity;
                    v.Continuity = Continuity.G1;
                    if (!s.CheckCanMove(v))
                    {
                        _isVertexEnabled[2] = false;
                    }
                    v.Continuity = c;
                }
               
            }

            //C1
            if(v.Continuity == Continuity.C1)
            {
                _isVertexEnabled[3] = false;
            }
            else
            {
                foreach (Edge e in s.CircularEdges)
                {
                    if (e.HasVertex(v))
                    {
                        _isVertexEnabled[3] = false;
                        break;
                    }
                }

                foreach(Edge e in s.Edges)
                {
                    if (e.HasVertex(v) && e.FixedLength != null)
                    {
                        _isVertexEnabled[3] = false;
                        break;
                    }
                }

            }

            //reset


            return _isVertexEnabled;
        }

        public void AcceptBezier(BezierEdge edge)
        {
            _isEnabled = [false, true, false, false,false,false];
        }

        public void AcceptCircular(CircularEdge edge)
        {
            _isEnabled = [false, true, false, false,false,false];
        }

        public void AcceptStraight(Edge edge)
        {
            _isEnabled = [true, false, true, true,true,true];

            if(edge.FixedLength != null)
            {
                _isEnabled[4] = false;
            }

            if(edge.FixedLength == null)
            {
                _isEnabled[5] = false;

            }

            Continuity[] con = [edge.P1.Continuity, edge.P2.Continuity];

            for(int i = 0; i < 2; i++)
            {
                if (con[i] == Continuity.C1)
                {
                    _isEnabled[4] = false;
                    _isEnabled[2] = false;
                    return;
                }
                   

                if (con[i] == Continuity.G1 && con[(i + 1) % 2] == Continuity.G1)
                {
                    _isEnabled[2] = false;
                }

            }

        }
    }
}
