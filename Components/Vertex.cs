using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GK1_1.Components
{
    public enum Continuity
    {
      G0,
      G1,
      C1,
      NotSet,
      ControlPoint
    }

    public class Vertex : SuperNotifyChanged
    {

        private int _x;
        private int _y;
        private Continuity _continuity;
        
        public Continuity Continuity
        {
            get
            {
                return _continuity;
            }
            set
            {
                SetProperty(ref _continuity, value);
            }
        }

        public int X
        {
            get
            {
                return _x;
            }
            set
            {
                SetProperty(ref _x, value);
            }
        }

        public int Y
        {
            get
            {
                return _y;
            }
            set
            {
                SetProperty(ref _y, value);
            }
        }

        public Vertex(int  x, int y)
        {
            _x = x;
            _y = y;
            _continuity = Continuity.NotSet;
        }

        public Vertex(ExactPoint ev) : this((int)ev.x, (int)ev.y)
        {

        }

        public void UpdatePosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void Shift(Vector v)
        {
            X = _x + v.x;
            Y = _y + v.y;
        }
    }
}
