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

namespace flappy_bird
{
    public partial class MainWindow : Window
    {
        private double yAxis = 100;
        private double velocity = 0;
        public MainWindow()
        {
            InitializeComponent();
            CompositionTarget.Rendering += OnRendering;
        }

        public void OnRendering(object sender, EventArgs e)
        {
            // Collision
            if (yAxis >= 550)
            {
                yAxis = 550;
                velocity = 0;
            }

            if (yAxis <= 0)
            {
                yAxis = 0;
                velocity = 0;
            }

            // Jump
            if (Keyboard.IsKeyDown(Key.Space))
            {
                velocity = -10;
            }

            // Gravity
            velocity += 0.5;

            // Y Position
            yAxis += velocity;

            // Set
            Canvas.SetTop(Bird, yAxis);
        }
    }
}