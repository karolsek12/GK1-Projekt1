using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GK1_1.Components;
using GK1_1.Components.Edges;

namespace GK1_1
{
    public static class ShapeInit
    {
        public static Shape Square(Vertex LeftTop,int length)
        {
            
            Vertex RightTop = new Vertex(LeftTop.X+length,LeftTop.Y);
            Vertex RightBottom = new Vertex(RightTop.X, RightTop.Y + length);
            Vertex LeftBottom = new Vertex(RightTop.X-length, RightBottom.Y);

            List<Vertex> Vertices = [LeftBottom, RightBottom, RightTop,LeftTop];
            List<Edge> Edges = [
                new Edge(LeftTop,RightTop),
                new Edge(RightTop,RightBottom),
                new Edge(RightBottom,LeftBottom),
                new Edge(LeftBottom,LeftTop)

            ];

            return new Shape(Vertices, Edges);
        }

        public static void PerpendicularSquare(Shape s)
        {
            s.Edges[1].Index = 0;
            s.Edges[1].Perpendicular = s.Edges[3];
            s.Edges[3].Perpendicular = s.Edges[1];
            s.Edges[3].Index = 0;

            s.MoveVertex(s.Vertices[0], 0, 0, true);
        }

        public static void BezierFixedSquare(Shape s)
        {
            s.MakeEdgeBezier(s.Edges[0]);
            s.FixLength(s.Edges[1], 50);

            s.MoveVertex(s.Vertices[0], 0, 0, true);
        }

    }
}
