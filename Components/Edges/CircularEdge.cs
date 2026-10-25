using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GK1_1.Components.Edges
{
    public class CircularEdge : Edge
    {

        private double _betweenRadius;
        private double _radius;
        private bool _isLargeArc;
        private SweepDirection _sweepDirection;
        private bool _freeze;
        public double BetweenRadius
        {
            get
            {
                return _betweenRadius;
            }
            set
            {
                SetProperty(ref _betweenRadius, value);
            }
        }

        public double Radius
        {
            get
            {
                return _radius;
            }
            set
            {
                SetProperty(ref _radius,value);
            }
        }

        public bool IsLargeArc
        {
            get
            {
                return _isLargeArc;
            }
            set
            {
                SetProperty(ref _isLargeArc,value);
            }
        }

        public SweepDirection SweepDirection
        {
            get
            {
                return _sweepDirection;
            }
            set
            {
                SetProperty(ref _sweepDirection,value);
            }
        }

        public CircularEdge(Vertex p1, Vertex p2) : base(p1, p2)
        {
            _pixels = new List<Point>();
            _freeze = false;
            P1.PropertyChanged  += RecalculateRadius;
            P2.PropertyChanged += RecalculateRadius;
            CalcRadius();
            
            
        }

        private void CalcRadius()
        {
            if (_freeze)
                return;
            int diff1 = P1.X - P2.X;
            int diff2 = P1.Y - P2.Y;
            BetweenRadius = Math.Sqrt(diff1 * diff1 + diff2 * diff2) / 2.0;
            Radius = BetweenRadius;
        }

        private void RecalculateRadius(object? sender, PropertyChangedEventArgs e)
        {
            CalcRadius();
        }


        public override (List<Point>, List<Point>) GetPoints()
        {
            return (new List<Point>(), new List<Point>());
        }

        public override bool IsAlwaysVisible()
        {
            return true;
        }

        public override void VisitRemove(Shape s)
        {
            s.RemoveCircular(this);
        }

        public override void VisitMenu(MenuCreator mc)
        {
            mc.AcceptCircular(this);
        }

        public override void SetTangent(Vector tangent, Vertex vertex)
        {

            
            ExactPoint half = new ExactPoint(P1.X + P2.X, P1.Y + P2.Y);
            half = 0.5 * half;

            ExactVector ev = new ExactVector(new ExactPoint(new Point(vertex)),half);
            ExactVector et = new ExactVector(tangent);

            double cosb = (ev.x*et.x + ev.y*et.y);
            cosb /= ev.Length();
            cosb /= et.Length();
            double sinb = Math.Sqrt(1 - Math.Pow(cosb,2));

            double singedSinb = ev.x * et.y - ev.y * et.x;

            if(singedSinb >= 0)
            {
                if(vertex == P2)
                {
                    SweepDirection = SweepDirection.Counterclockwise;
                }
                else
                {
                    SweepDirection = SweepDirection.Clockwise;
                }
                
            }
            else
            {
                if (vertex == P2)
                {
                    SweepDirection = SweepDirection.Clockwise;
                }
                else
                {
                    SweepDirection = SweepDirection.Counterclockwise;
                }
                
            }


            IsLargeArc = cosb >= 0;
            Radius = BetweenRadius / sinb;

            //CONSUMATION IS CORRECT
        }

        public override Vector GetTangent(Vertex vertex)
        {
            double cosa = BetweenRadius / Radius;
            double a = Math.Acos(cosa);

            ExactPoint half = new ExactPoint(P1.X + P2.X, P1.Y + P2.Y);
            half = 0.5 * half;
            ExactVector ev = new ExactVector(half,new ExactPoint(new Point(vertex)));
            ExactVector rv;
            rv = ev.GetRotated(Math.PI/2 - a); // TODO: fix diffrent rotation angles

            if (SweepDirection == SweepDirection.Clockwise)
            {
                if (vertex == P1)
                {
                    rv.x *= -1;
                    rv.y *= -1;
                }

            }

            return new Vector(rv);

        }
        public void Freeze()
        {
            _freeze = true;
        }

        public void Unfreeze()
        {
            _freeze = false;
        }
 
    }
}
