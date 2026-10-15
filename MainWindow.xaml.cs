using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
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
using MyPoint = GK1_1.Components.Point;
using WinPoint = System.Windows.Point;

namespace GK1_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    
    public partial class MainWindow : Window
    {
        private BitmapHandler _bmpHandler;
        private ObservableCollection<Shape> _shapes;
        private (Vertex,Shape)? _held;
        private (Vertex,Shape)? _markedVertex;
        private (Edge, Shape)? _markedEdge;
        private ViewModel _viewModel;

        private ContextMenu _rightClickVertexMenu;
        private ContextMenu _rightClickEdgeMenu;


        public WriteableBitmap BMP
        {
            get
            {
                return _bmpHandler.BMP;
            }
        }

        public ObservableCollection<Shape> Shapes
        {
            get 
            { 
                return _shapes; 
            }
        }

        public ViewModel ViewModel
        {
            get
            {
                return _viewModel;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            _bmpHandler = null!;
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

            _viewModel = new ViewModel();

            Loaded += Setup;
            
        }

        private void SubdivideEdge_Click(object sender, RoutedEventArgs e)
        {
            (Edge edge, Shape s) = _markedEdge!.Value;
            
            (List<MyPoint> older, List<MyPoint> newer) = s.SubdivideEdge(edge);
            _bmpHandler.ChangePoints(older, newer);

        }

        private void DelVertex_Click(object sender, RoutedEventArgs e)
        {
            (Vertex v, Shape s) = _markedVertex!.Value;

            (List<MyPoint> older, List<MyPoint> newer) = s.EraseVertex(v);
            _bmpHandler.ChangePoints(older, newer);

        }

        public void Setup(object sender,RoutedEventArgs args)
        {
            _bmpHandler = new BitmapHandler((int)grid.Width, (int)grid.Height,_shapes.ToList());
            DataContext = this;
            grid.MouseLeftButtonDown += ClickedLeftDown;
            grid.MouseLeftButtonUp += ClickedLeftUp;
            grid.MouseMove += MovedMouse;
            grid.MouseRightButtonDown += ClickedRightDown;
        }

        public void ClickedLeftDown(object sender, MouseButtonEventArgs args)
        {
            WinPoint p = args.GetPosition(grid);
            Shape? shape;

            Vertex? v = HitDetection.CheckPointHit(_shapes.ToList(), (int)p.X, (int)p.Y,out shape);
            
            if(v != null && shape != null)
            {
                _held = (v, shape);
            }
            else
            {
                _held = null;
            }

        }


        public void MovedMouse(object sender, MouseEventArgs args)
        {
            
            WinPoint p = args.GetPosition(grid);
            if (_held == null)
                return;
            (Vertex point, Shape shape) = _held.Value;

            point.UpdatePosition((int)p.X,(int)p.Y);
            (List<MyPoint> older, List<MyPoint> newer) = shape.UpdateEdges(new MyPoint(point));
            _bmpHandler.ChangePoints(older, newer);
        }
        
        public void ClickedLeftUp(object sender, MouseButtonEventArgs args)
        {
            _held = null;
        }

        public void ClickedRightDown(object sender, MouseButtonEventArgs args)
        {
            WinPoint p = args.GetPosition(grid);
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

    }
}