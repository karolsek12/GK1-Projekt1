using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GK1_1.Components;

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

    }
}
