using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK1_1
{
    public class ViewModel : SuperNotifyChanged
    {
        private bool _library;
        private bool _bresenham;

        public bool Library
        {
            get
            {
                return _library;
            }
            set
            {
                SetProperty(ref _library, value);
            }
        }

        public bool Bresenham
        {
            get
            {
                return _bresenham;
            }
            set
            {
                SetProperty(ref _bresenham, value);
            }
        }

        public ViewModel()
        {
            _library = true;
            _bresenham = false;
        }

    }
}
