using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using GK1_1.Components;

namespace GK1_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    
    public partial class MainWindow : Window
    {
        private WriteableBitmap _bmp;
        private ObservableCollection<Shape> _shapes;
        private Vertex? _held;
        private (Vertex,Shape)? _markedVertex;
        private (Edge, Shape)? _markedEdge;

        private ContextMenu _rightClickVertexMenu;
        private ContextMenu _rightClickEdgeMenu;

        public WriteableBitmap BMP
        {
            get
            {
                return _bmp;
            }
        }

        public ObservableCollection<Shape> Shapes
        {
            get 
            { 
                return _shapes; 
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            _bmp = null!;
            _held = null;
            _shapes = new ObservableCollection<Shape>();
            _shapes.Add(ShapeInit.Square(new Vertex(50, 50), 50));

            _rightClickVertexMenu = new ContextMenu();
            MenuItem delVertex = new MenuItem { Header = "Delete Vertex" };
            delVertex.Click += DelVertex_Click;
            _rightClickVertexMenu.Items.Add(delVertex);

            _rightClickEdgeMenu = new ContextMenu();
            MenuItem subdivideEdge = new MenuItem { Header = "Subdivide Edge" };
            subdivideEdge.Click += SubdivideEdge_Click;
            _rightClickEdgeMenu.Items.Add(subdivideEdge);


            Loaded += Setup;
            
        }

        private void SubdivideEdge_Click(object sender, RoutedEventArgs e)
        {
            (Edge edge, Shape s) = _markedEdge!.Value;

            s.SubdivideEdge(edge);
        }

        private void DelVertex_Click(object sender, RoutedEventArgs e)
        {
            (Vertex v, Shape s) = _markedVertex!.Value;

            s.EraseVertex(v);
        }

        public void Setup(object sender,RoutedEventArgs args)
        {

            _bmp = new WriteableBitmap((int)grid.Width, (int)grid.Height, 96, 96, PixelFormats.Bgr32, null);
            DataContext = this;
            grid.MouseLeftButtonDown += ClickedLeftDown;
            grid.MouseLeftButtonUp += ClickedLeftUp;
            grid.MouseMove += MovedMouse;
            grid.MouseRightButtonDown += ClickedRightDown;
        }

        public void ClickedLeftDown(object sender, MouseButtonEventArgs args)
        {
            Point p = args.GetPosition(grid);
            Shape? shape;

            _held = HitDetection.CheckPointHit(_shapes.ToList(), (int)p.X, (int)p.Y,out shape);
            
        }

        public void MovedMouse(object sender, MouseEventArgs args)
        {
            if (_held == null)
                return;
            Point p = args.GetPosition(grid);

            _held.X = (int)p.X;
            _held.Y = (int)p.Y;
        }
        
        public void ClickedLeftUp(object sender, MouseButtonEventArgs args)
        {
            _held = null;
        }

        public void ClickedRightDown(object sender, MouseButtonEventArgs args)
        {
            Point p = args.GetPosition(grid);
            Shape? shape;
            MenuItem item;

            Vertex? v = HitDetection.CheckPointHit(_shapes.ToList(), (int)p.X, (int)p.Y,out shape);

            if (v == null|| shape == null)
            {
                _markedVertex = null;

                Edge? e = HitDetection.CheckEdgeHit(_shapes.ToList(), (int)p.X, (int)p.Y, out shape);

                if(e == null || shape == null)
                {
                    _markedEdge = null;
                    return;
                }

                _markedEdge = (e, shape);

                _rightClickEdgeMenu.PlacementTarget = this;
                _rightClickEdgeMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
                _rightClickEdgeMenu.IsOpen = true;

                return;
            }
                

            _markedVertex = (v, shape);
            item = (MenuItem)_rightClickVertexMenu.Items[0];
            item.IsEnabled = shape.Vertices.Count > 3;

            _rightClickVertexMenu.PlacementTarget = this;
            _rightClickVertexMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
            _rightClickVertexMenu.IsOpen = true;
        }


        /*public void Test(object sender,MouseButtonEventArgs args)
        {

            for(int i = 0; i < 100; i++)
            {
                int column = i;
                int row = i;

                try
                {
                    // Reserve the back buffer for updates.
                    _bmp.Lock();

                    unsafe
                    {
                        // Get a pointer to the back buffer.
                        IntPtr pBackBuffer = _bmp.BackBuffer;

                        // Find the address of the pixel to draw.
                        pBackBuffer += row * _bmp.BackBufferStride;
                        pBackBuffer += 50 * 4;

                        // Compute the pixel's color.
                        int color_data = 255 << 16; // R
                        color_data |= 128 << 8;   // G
                        color_data |= 255 << 0;   // B

                        // Assign the color data to the pixel.
                        *((int*)pBackBuffer) = color_data;
                    }

                    // Specify the area of the bitmap that changed.
                    _bmp.AddDirtyRect(new Int32Rect(50, row, 1, 1));
                }
                finally
                {
                    // Release the back buffer and make it available for display.
                    _bmp.Unlock();
                }
            }
       

        }*/

    }
}