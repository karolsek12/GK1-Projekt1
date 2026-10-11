using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GK1_1.Components;

namespace GK1_1
{
    public static class HitDetection
    {
        public static int VertexEpsilon = 8;
        public static int EdgeEpsilon = 8;

        public static Vertex? CheckPointHit(List<Shape> shapes,int x,int y, out Shape? hitShape)
        {

            hitShape = null;
            foreach (Shape shape in shapes)
            {
                foreach (Vertex vertex in shape.Vertices)
                {
                    int value = (vertex.X - x) * (vertex.X - x) + (vertex.Y - y) * (vertex.Y - y);

                    if(value <= VertexEpsilon * VertexEpsilon)
                    {
                        hitShape = shape;
                        return vertex;
                    }
                }

            }
            
            return null;
        }

        public static Edge? CheckEdgeHit(List<Shape> shapes, int x, int y, out Shape? hitShape)
        {
            hitShape = null;

            foreach (Shape shape in shapes)
            {
                foreach (Edge edge in shape.Edges)
                {
                    if (edge.CheckHit(x, y))
                    {
                        hitShape = shape;
                        return edge;
                    }
                    
                }

            }

            return null;
        }

        public static bool IsNearLine(Vertex A, Vertex B, int x, int y)
        {
            if (A.X > B.X)
                return IsNearLine(B, A, x,y);

            double xDif = B.X - A.X;
            double yDif = B.Y - A.Y;

            double alpha = Math.Atan2(yDif, xDif);

            double cos = Math.Cos(alpha);
            double sin = Math.Sin(alpha);

            Vertex ab = new Vertex(B.X - A.X, B.Y - A.Y);
            Vertex ax = new Vertex(x - A.X,y-A.Y);

            Vertex rotB = new Vertex((int)(cos * ab.X + sin * ab.Y), (int)(-sin * ab.X + cos * ab.Y));

            Vertex rotX = new Vertex((int)(cos * ax.X + sin * ax.Y), (int)(-sin * ax.X + cos * ax.Y));

            bool checkX = rotX.X >= 0 && rotX.X <= rotB.X;

            bool checkY = rotX.Y >= -EdgeEpsilon && rotX.Y <= EdgeEpsilon;

            return (checkX && checkY);

        }
    }
}
