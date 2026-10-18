using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GK1_1.Components;
using MyPoint = GK1_1.Components.Point;

namespace GK1_1
{


    public class BitmapHandler
    {
        private WriteableBitmap _bmp;
        private int[,] _map;
        private int _width;
        private int _height;

        const int BLACK = 0;
        const int WHITE = int.MaxValue;

        public WriteableBitmap BMP
        {
            get
            {
                return _bmp;
            }
        }

        public BitmapHandler(int width, int height,List<Shape> shapes) 
        {
            _bmp = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr32, null);
            _map = new int[width, height];
            _width = width;
            _height = height;
            foreach (Shape shape in shapes)
            {
                foreach (Edge e in shape.Edges)
                {
                    IncrementPoints(e.Pixels);
                }
            }
        }

        public void IncrementPoints(List<MyPoint> newest)
        {
            HashSet<MyPoint> set = new HashSet<MyPoint>();

            foreach (MyPoint p in newest)
            {
                _map[p.x, p.y]++;
                set.Add(p);
            }

            ColorPixels(set);
        }

        public void ChangePoints(List<MyPoint> older, List<MyPoint> newer)
        {
            HashSet<MyPoint> set = new HashSet<MyPoint>();
            int x, y;
            foreach (MyPoint p in older)
            {
                x = int.Max(int.Min(p.x,_width - 1), 0);
                y = int.Max(int.Min(p.y, _height - 1), 0);

                _map[x, y]--;
                set.Add(new MyPoint(x,y));
            }

            foreach(MyPoint p in newer)
            {
                x = int.Max(int.Min(p.x, _width-1), 0);
                y = int.Max(int.Min(p.y, _height-1), 0);

                _map[x, y]++;
                set.Add(new MyPoint(x, y));
            }

            ColorPixels(set);
        }

        private void ColorPixels(HashSet<MyPoint> set)
        {
            int left = set.Min(v => v.x);
            int right = set.Max(v => v.x);
            int top = set.Min(v => v.y);
            int bottom = set.Max(v => v.y);

            Int32Rect rect = new Int32Rect(left, top, right - left+1, bottom - top+1);

            try
            {
                _bmp.Lock();

                unsafe
                {
                    foreach (MyPoint p in set)
                    {

                        // Get a pointer to the back buffer.
                        IntPtr pBackBuffer = _bmp.BackBuffer;

                        // Find the address of the pixel to draw.
                        pBackBuffer += p.y * _bmp.BackBufferStride;
                        pBackBuffer += p.x * 4;

                        int color_data = WHITE;
                        if (_map[p.x, p.y] == 0)
                            color_data = BLACK;


                        // Assign the color data to the pixel.
                        *((int*)pBackBuffer) = color_data;
                    }
                }

                // Specify the area of the bitmap that changed.
                _bmp.AddDirtyRect(rect);
            }
            finally
            {
                // Release the back buffer and make it available for display.
                _bmp.Unlock();
            }


        }

    }
}
