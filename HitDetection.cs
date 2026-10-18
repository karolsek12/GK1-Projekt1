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

        public static bool IsNearLine(Point A, Point B, int x, int y)
        {
            Point clicked = new Point(x, y);

            if (A.x > B.x)
                return IsNearLine(B, A, x,y);

            double xDif = B.x - A.x;
            double yDif = B.y - A.y;

            double alpha = Math.Atan2(yDif, xDif);

            Vector ab = new Vector(B,A);
            Vector ax = new Vector(clicked, A);

            Vector rotB = ab.GetRotated(alpha);

            Vector rotX = ax.GetRotated(alpha);

            bool checkX = rotX.x >= 0 && rotX.x <= rotB.x;

            bool checkY = rotX.y >= -EdgeEpsilon && rotX.y <= EdgeEpsilon;

            return (checkX && checkY);

        }

        public static Shape? CheckShapeHit(List<Shape> shapes,int x,int y)
        {
            Point p = new Point(x, y);
            foreach(Shape shape in shapes)
            {
                BoundingBox bb = shape.GetBoundingBox();

                if(bb.IsInside(p))
                    return shape;
            }

            return null;
        }
    }
}
