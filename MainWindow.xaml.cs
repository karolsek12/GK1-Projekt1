using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Metrics;
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
using GK1_1.Components.Edges;
using MyPoint = GK1_1.Components.Point;
using MyVector = GK1_1.Components.Vector;
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

            MenuItem G0Vertex = new MenuItem { Header = "G0" };
            G0Vertex.Click += G0Vertex_Click;
            _rightClickVertexMenu.Items.Add(G0Vertex);

            MenuItem G1Vertex = new MenuItem { Header = "G1" };
            G1Vertex.Click += G1Vertex_Click;
            _rightClickVertexMenu.Items.Add(G1Vertex);

            MenuItem C1Vertex = new MenuItem { Header = "C1" };
            C1Vertex.Click += C1Vertex_Click;
            _rightClickVertexMenu.Items.Add(C1Vertex);



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


            MenuItem fixEdge = new MenuItem { Header = "Fix Length" };
            fixEdge.Click += FixEdge_Click;
            _rightClickEdgeMenu.Items.Add(fixEdge);

            MenuItem unfixEdge = new MenuItem { Header = "Unfix Length" };
            unfixEdge.Click += UnfixEdge_Click;
            _rightClickEdgeMenu.Items.Add(unfixEdge);


            _viewModel = new ViewModel();
            InitializeComponent();
            Loaded += Setup;
            
        }

        private void ResetBitmap()
        {
            List<MyPoint> total = new List<MyPoint>();
            foreach(Shape s in _shapes)
            {
                List<MyPoint> toAdd = s.UpdateAllEdges();

                for(int i = 0; i < toAdd.Count; i++)
                {
                    total.Add(toAdd[i]);
                }
            }

            _bmpHandler.ChangePoints(total);
        }

        private void SetContinuity(Continuity c)
        {
            (Vertex v, Shape s) = _markedVertex!.Value;

            v.Continuity = c;

            s.MoveVertex(v, v.X,v.Y,true);
            ResetBitmap();
        }

        private void C1Vertex_Click(object sender, RoutedEventArgs e)
        {
            SetContinuity(Continuity.C1);
        }

        private void G1Vertex_Click(object sender, RoutedEventArgs e)
        {
            SetContinuity(Continuity.G1);
        }

        private void G0Vertex_Click(object sender, RoutedEventArgs e)
        {
            SetContinuity(Continuity.G0);
        }

        private void UnfixEdge_Click(object sender, RoutedEventArgs e)
        {
            (Edge edge, Shape s) = _markedEdge!.Value;

            s.UnfixLength(edge);
        }

        private void FixEdge_Click(object sender, RoutedEventArgs e)
        {
            (Edge edge, Shape s) = _markedEdge!.Value;
            LengthInput inputDialog = new LengthInput(edge.GetLength());
            if (inputDialog.ShowDialog() == true)
            {
                s.FixLength(edge, inputDialog.Value);

                ResetBitmap();
            }

            
        }

        private void StraightEdge_Click(object sender, RoutedEventArgs e)
        {
            (Edge edge, Shape s) = _markedEdge!.Value;
            s.MakeEdgeStraight(edge);
            ResetBitmap();
        }

        private void CircularEdge_Click(object sender, RoutedEventArgs e)
        {
            (Edge edge, Shape s) = _markedEdge!.Value;
            s.MakeEdgeCircular(edge);
            ResetBitmap();

        }

        private void BezierEdge_Click(object sender, RoutedEventArgs e)
        {
            (Edge edge, Shape s) = _markedEdge!.Value;
            s.MakeEdgeBezier(edge);
            ResetBitmap();
        }

        private void SubdivideEdge_Click(object sender, RoutedEventArgs e)
        {
            (Edge edge, Shape s) = _markedEdge!.Value;
            
            s.SubdivideEdge(edge);
            ResetBitmap();

        }

        private void DelVertex_Click(object sender, RoutedEventArgs e)
        {
            (Vertex v, Shape s) = _markedVertex!.Value;

            s.EraseVertex(v);
            ResetBitmap();

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

                ResetBitmap();
                return;
            }
                
            (Vertex point, Shape shape) = _held.Value;

            shape.MoveVertex(point, p.x, p.y);
            ResetBitmap();
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

            bool[] isEnabledVertex = _menuCreator.GetVertexMenuEnabled(v,shape);

            for (int i = 0; i < isEnabledVertex.Length; i++)
            {
                ((MenuItem)_rightClickVertexMenu.Items[i]).IsEnabled = isEnabledVertex[i];
            }


            _rightClickVertexMenu.PlacementTarget = this;
            _rightClickVertexMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
            _rightClickVertexMenu.IsOpen = true;
        }

    }
}