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
using MyVector = GK1_1.Components.Vector;
using GK1_1.Components.Edges;

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
        private (MyPoint,Shape)? _movingShape;
        private ViewModel _viewModel;

        private MenuCreator _menuCreator;
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

            _bmpHandler = null!;
            _held = null;
            _movingShape = null;
            _shapes = new ObservableCollection<Shape>();
            _shapes.Add(ShapeInit.Square(new Vertex(50, 50), 50));
            _menuCreator = new MenuCreator();

            _rightClickVertexMenu = new ContextMenu();
            MenuItem delVertex = new MenuItem { Header = "Delete Vertex" };
            delVertex.Click += DelVertex_Click;
            _rightClickVertexMenu.Items.Add(delVertex);

            _rightClickEdgeMenu = new ContextMenu();
            MenuItem subdivideEdge = new MenuItem { Header = "Subdivide Edge" };
            subdivideEdge.Click += SubdivideEdge_Click;
            _rightClickEdgeMenu.Items.Add(subdivideEdge);

            MenuItem straightEdge = new MenuItem { Header = "Straight Edge" };
            straightEdge.Click += StraightEdge_Click;
            _rightClickEdgeMenu.Items.Add(straightEdge);

            MenuItem circularEdge = new MenuItem { Header = "Circular Edge" };
            circularEdge.Click += CircularEdge_Click;
            _rightClickEdgeMenu.Items.Add(circularEdge);

            MenuItem bezierEdge = new MenuItem { Header = "Bezier Edge" };
            bezierEdge.Click += BezierEdge_Click; 
            _rightClickEdgeMenu.Items.Add(bezierEdge);

            _viewModel = new ViewModel();
            InitializeComponent();
            Loaded += Setup;
            
        }



        private void StraightEdge_Click(object sender, RoutedEventArgs e)
        {
            (Edge edge, Shape s) = _markedEdge!.Value;
            (List<MyPoint> older, List<MyPoint> newer) = s.MakeEdgeStraight(edge);
            _bmpHandler.ChangePoints(older, newer);
        }

        private void CircularEdge_Click(object sender, RoutedEventArgs e)
        {
            (Edge edge, Shape s) = _markedEdge!.Value;
            (List<MyPoint> older, List<MyPoint> newer) = s.MakeEdgeCircular(edge);
            _bmpHandler.ChangePoints(older, newer);

        }

        private void BezierEdge_Click(object sender, RoutedEventArgs e)
        {
            (Edge edge, Shape s) = _markedEdge!.Value;
            (List<MyPoint> older, List<MyPoint> newer) = s.MakeEdgeBezier(edge);
            _bmpHandler.ChangePoints(older, newer);
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
            MyPoint p = new MyPoint(args.GetPosition(grid));
            Shape? shape;

            Vertex? v = HitDetection.CheckPointHit(_shapes.ToList(), p.x,p.y,out shape);
            
            if(v != null && shape != null)
            {
                _held = (v, shape);
            }
            else
            {
                _held = null;
                shape = HitDetection.CheckShapeHit(_shapes.ToList(), p.x, p.y);
                if (shape != null)
                    _movingShape = (p,shape);
            }

        }


        public void MovedMouse(object sender, MouseEventArgs args)
        {
            List<MyPoint> older, newer;
            MyPoint p = new MyPoint(args.GetPosition(grid));
            if (_held == null)
            {
                if (_movingShape == null)
                    return;

                (MyPoint mpoint, Shape moving) = _movingShape.Value;

                moving.MoveAll(new MyVector(p,mpoint));
                _movingShape = (p,moving);

                (older, newer) = moving.UpdateAllEdges();
                _bmpHandler.ChangePoints(older, newer);
                return;
            }
                
            (Vertex point, Shape shape) = _held.Value;

            point.UpdatePosition(p.x,p.y);
            ( older, newer) = shape.UpdateEdges(new MyPoint(point));
            _bmpHandler.ChangePoints(older, newer);
        }
        
        public void ClickedLeftUp(object sender, MouseButtonEventArgs args)
        {
            _held = null;
            _movingShape = null;
        }

        public void ClickedRightDown(object sender, MouseButtonEventArgs args)
        {
            MyPoint p = new MyPoint(args.GetPosition(grid));
            Shape? shape;
            MenuItem item;

            Vertex? v = HitDetection.CheckPointHit(_shapes.ToList(), p.x,p.y,out shape);

            if (v == null|| shape == null)
            {
                _markedVertex = null;

                Edge? e = HitDetection.CheckEdgeHit(_shapes.ToList(), p.x, p.y, out shape);

                if(e == null || shape == null)
                {
                    _markedEdge = null;
                    return;
                }

                _markedEdge = (e, shape);

                //0 subdivide
                //1 straight
                //2 circular
                //3 bezier
                bool[] isEnabled = _menuCreator.GetEdgeMenuEnabled(e);

                for(int i = 0; i < isEnabled.Length; i++)
                {
                    ((MenuItem)_rightClickEdgeMenu.Items[i]).IsEnabled = isEnabled[i];
                }

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