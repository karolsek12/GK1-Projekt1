using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GK1_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    
    public partial class MainWindow : Window
    {
        public WriteableBitmap bmp;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += Setup;
            
        }

        public void Setup(object sender,RoutedEventArgs args)
        {

            bmp = new WriteableBitmap((int)Width, (int)Height, 96, 96, PixelFormats.Bgr32, null);
            DataContext = bmp;
            MouseLeftButtonDown += Test;
        }
        public void Test(object sender,MouseButtonEventArgs args)
        {

            for(int i = 0; i < 100; i++)
            {
                int column = i;
                int row = i;

                try
                {
                    // Reserve the back buffer for updates.
                    bmp.Lock();

                    unsafe
                    {
                        // Get a pointer to the back buffer.
                        IntPtr pBackBuffer = bmp.BackBuffer;

                        // Find the address of the pixel to draw.
                        pBackBuffer += row * bmp.BackBufferStride;
                        pBackBuffer += 50 * 4;

                        // Compute the pixel's color.
                        int color_data = 255 << 16; // R
                        color_data |= 128 << 8;   // G
                        color_data |= 255 << 0;   // B

                        // Assign the color data to the pixel.
                        *((int*)pBackBuffer) = color_data;
                    }

                    // Specify the area of the bitmap that changed.
                    bmp.AddDirtyRect(new Int32Rect(50, row, 1, 1));
                }
                finally
                {
                    // Release the back buffer and make it available for display.
                    bmp.Unlock();
                }
            }
       

        }

    }
}