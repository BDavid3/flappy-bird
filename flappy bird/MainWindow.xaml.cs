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
        Random random = new Random();
        private double yAxis = 100;
        private double velocity = 0;
        private double spawnTimer = 0;
        private List<Rectangle> pipes = new List<Rectangle>();
        public MainWindow()
        {
            InitializeComponent();
            CompositionTarget.Rendering += OnRendering;
            SpawnPipe();
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
                velocity = -5;
            }

            // Gravity
            velocity += 0.5;

            // Y Position
            yAxis += velocity;

            // Set
            Canvas.SetTop(Bird, yAxis);

            // Spawn Pipe
            spawnTimer += 1;

            if (spawnTimer >= 100)
            {
                spawnTimer = 0;
                SpawnPipe();
            }
            for (int i = pipes.Count - 1; i >= 0; i--)
            {
                var chosenPipe = pipes[i];
                Canvas.SetLeft(chosenPipe, Canvas.GetLeft(chosenPipe) - 3);
                if (Canvas.GetLeft(chosenPipe) + chosenPipe.Width < 0)
                {
                    MainCanvas.Children.Remove(chosenPipe);
                    pipes.RemoveAt(i);
                }
            }
        }
        public void SpawnPipe()
        {
            var heightTop = random.Next(100, 350);
            var emptySpace = random.Next(75,125);

            var topPipe = new Rectangle
            {
                Width = 50,
                Height = heightTop,
                Fill = Brushes.Green
            };

            var BottomPipe = new Rectangle
            {
                Width = 50,
                Height = MainCanvas.Height - (heightTop + emptySpace),
                Fill = Brushes.Green
            };

            Canvas.SetLeft(topPipe, MainCanvas.Width);
            Canvas.SetTop(topPipe, 0);

            Canvas.SetLeft(BottomPipe, MainCanvas.Width);
            Canvas.SetTop(BottomPipe, heightTop + emptySpace);

            MainCanvas.Children.Add(topPipe);
            MainCanvas.Children.Add(BottomPipe);

            pipes.Add(topPipe);
            pipes.Add(BottomPipe);
        }
    }
}