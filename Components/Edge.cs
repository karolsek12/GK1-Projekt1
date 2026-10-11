using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GK1_1.Components
{
    public class Edge : SuperNotifyChanged
    {

        private Vertex _p1;
        private Vertex _p2;

        public Vertex P1
        {
            get
            {
                return _p1;
            }
            set
            {
                SetProperty(ref _p1, value);
            }
        }

        public Vertex P2
        {
            get
            {
                return _p2;
            }
            set
            {
                SetProperty(ref _p2, value);
            }
        }

        public Edge(Vertex p1, Vertex p2)
        {
            _p1 = p1;
            _p2 = p2;
        }

        public bool CheckHit(int x, int y)
        {
            return HitDetection.IsNearLine(P1, P2, x, y);
        }

    }
}
