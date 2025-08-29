using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace HHG_WPF_Fileversion
    {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
        {
        //adding player field so we can instantiate player object in MainWindow's constructor
        private Player player;

        //declare and initialize animation stuff
        private ScaleTransform zoomTransform = new ScaleTransform(1, 1);
        private RotateTransform rotateTransform = new RotateTransform(0);
        private TransformGroup transformGroup = new TransformGroup();

        //declare and initialize a Random object
        private Random random = new Random();

        private Thickness original;

        //MainWindow's constructor
        public MainWindow()
            {
            InitializeComponent();

            //Instantiate player object and pass it as a parameter whenever we need to access it outside this (MainWindow) class
            player = new Player();

            //fetch contents in quotes file
            player.ReadFromFile(player);

            //set window to autosize based on its content (controls like buttons, textboxes etc...)
            this.SizeToContent = SizeToContent.WidthAndHeight;

            //create new brush and set window's background image
            ImageBrush brush = new ImageBrush();
            brush.Opacity = 0.25;
            brush.ImageSource = player.ShowImage();
            this.Background = brush;

            //init animation stuff
            InitImageControl();

            //set focus to firstName textbox
            tbFirstName.Focus();

            //keep track of original btnOK's margin (position)
            original = btnOK.Margin;

            //set textwrap on
            tbQuote.TextWrapping = TextWrapping.Wrap;

            //sound test using MediaPlayer            
            //player.Song.Open(new Uri(player.GetSong()));
            //player.Song.Play();

            //play music
            FadeInMusic();
            }

        private void FadeInMusic()
            {
            player.audioFileReader = new AudioFileReader(player.GetSong());
            player.volumeProvider = new VolumeSampleProvider(player.audioFileReader);
            player.volumeProvider.Volume = 0f; // start silent

            player.outputDevice = new WaveOutEvent(); // or use WaveOut for more control
            player.outputDevice.Init(player.volumeProvider);
            player.outputDevice.Play();

            FadeInVolume(player.volumeProvider, durationSeconds: 3);
            }

        private void FadeInVolume(VolumeSampleProvider volumeProvider, double durationSeconds = 2.0)
            {
            var timer = new DispatcherTimer
                {
                Interval = TimeSpan.FromMilliseconds(50)
                };

            int totalSteps = (int)(durationSeconds * 1000 / timer.Interval.TotalMilliseconds);
            int currentStep = 0;

            timer.Tick += (s, e) =>
            {
                currentStep++;
                float volume = (float)currentStep / totalSteps;
                volumeProvider.Volume = Math.Min(volume, 1f);

                if (currentStep >= totalSteps)
                    {
                    timer.Stop();
                    }
            };

            timer.Start();
            }

        private void FadeOutMusic()
            {
            player.audioFileReader = new AudioFileReader(player.GetSong());
            player.volumeProvider = new VolumeSampleProvider(player.audioFileReader);
            player.volumeProvider.Volume = 1.0f; // start at max volume

            player.outputDevice = new WaveOutEvent();
            player.outputDevice.Init(player.volumeProvider);
            player.outputDevice.Play();

            FadeInVolume(player.volumeProvider, durationSeconds: 3);
            }

        private void FadeInImage(double maxOpacity)
            {
            DoubleAnimation fadeIn = new DoubleAnimation(0, maxOpacity, TimeSpan.FromSeconds(5));

            fadeIn.AutoReverse = true;
            fadeIn.RepeatBehavior = RepeatBehavior.Forever;
            image.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            }

        private void ZoomIn()
            {
            //the spinning animation will activate even though it's never called
            //when the spin animation starts it effectively carries over due to the use of TransFormGroup
            //the solution is to set it to null (remove it)
            rotateTransform.BeginAnimation(RotateTransform.AngleProperty, null);

            DoubleAnimation zoomIn = new DoubleAnimation();

            zoomIn.From = 0.0;
            zoomIn.To = 3.5;
            zoomIn.Duration = TimeSpan.FromSeconds(2);
            zoomIn.AutoReverse = true;
            zoomIn.RepeatBehavior = RepeatBehavior.Forever;

            //only bounce if not 42
            if (player.Age != player.DontPanic)
                {
                zoomIn.EasingFunction = new BounceEase
                    {
                    Bounces = 1,
                    Bounciness = 4,
                    EasingMode = EasingMode.EaseOut
                    };
                }

            //apply the animation (the zoom) to zoomTransform
            zoomTransform.BeginAnimation(ScaleTransform.ScaleXProperty, zoomIn);
            zoomTransform.BeginAnimation(ScaleTransform.ScaleYProperty, zoomIn);

            //test
            Debug.WriteLine($"Zooming in. Age: {player.Age}, Easing: {(player.Age != player.DontPanic ? "Bounce" : "Linear")}");
            }

        private void StartImageSpin()
            {
            DoubleAnimation rotateAnimation = new DoubleAnimation();

            rotateAnimation.From = 0;
            rotateAnimation.To = 360;
            rotateAnimation.Duration = TimeSpan.FromSeconds(10);
            rotateAnimation.RepeatBehavior = RepeatBehavior.Forever;

            //apply the animation to rotateTransform
            rotateTransform.BeginAnimation(RotateTransform.AngleProperty, rotateAnimation);
            }

        private void InitImageControl()
            {
            //only one animation can be active so we have to add both animations to a TransformGroup
            //and then add the TransformGroup to the image RenderTransform
            image.RenderTransformOrigin = new Point(0.5, 0.5);
            transformGroup.Children.Add(zoomTransform);
            transformGroup.Children.Add(rotateTransform);
            image.RenderTransform = transformGroup;
            }

        private void btnOK_Click(object sender, RoutedEventArgs e)
            {
            bool missingInfo = false;
            const string warning = "'Please fill out all fields. Although bypasses are the bedrock of humanity, this is the one and only exception.'";
            const string author = "\n - Prostetnic Vogon Jeltz -";

            image.Visibility = Visibility.Hidden;

            player.ClearPlayerData(player);

            //check for empty values
            if (!player.ReadInput(tbFirstName.Text, tbLastName.Text, tbAge.Text, player, tbQuote))
                {
                missingInfo = true;

                tbQuote.Text = "";
                tbQuote.Inlines.Add(new Run(warning) { FontStyle = FontStyles.Italic });
                tbQuote.Inlines.Add(new Run(author) { FontWeight = FontWeights.Bold });

                image.Visibility = Visibility.Visible;
                image.Source = player.ShowImage(missingInfo);

                FadeInImage(0.50);
                ZoomIn();

                //randomize button placement
                btnOK.Margin = new Thickness(random.Next((int)this.Width - 100), random.Next((int)this.Height - 100), 0, 0);
                }
            else
            if (player.Age == player.DontPanic)
                {
                //set song position to it's most HHG's "moment"                
                player.audioFileReader.CurrentTime = TimeSpan.FromMinutes(1.10);

                //player.Song.Position = new TimeSpan(0, 1, 10);
                //FadeInMusic();

                tbQuote.Text = "";

                image.Visibility = Visibility.Visible;
                image.Source = player.ShowImage(missingInfo);

                FadeInImage(1.0);
                ZoomIn();
                StartImageSpin();

                //restore button's original position
                btnOK.Margin = original;
                }
            else
                {
                tbQuote.Text = "";

                player.ReadInput(tbFirstName.Text, tbLastName.Text, tbAge.Text, player, tbQuote);

                image.Visibility = Visibility.Hidden;

                btnOK.Margin = original;
                }
            }

        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //    {

        //    }

        public void ThreeDTest()
            {
            //Note: this is straight from ChatGPT, I have very little idea what is actually going on here

            // Set up the viewport
            var viewport = new Viewport3D();

            // Camera
            var camera = new PerspectiveCamera
                {
                Position = new Point3D(0, 0, 3),
                LookDirection = new Vector3D(0, 0, -1),
                UpDirection = new Vector3D(0, 1, 0),
                FieldOfView = 60
                };
            viewport.Camera = camera;

            // Light
            var light = new DirectionalLight
                {
                Color = Colors.White,
                Direction = new Vector3D(-0.5, -0.5, -1)
                };

            viewport.Children.Add(new ModelVisual3D { Content = light });

            // 3D Geometry (rectangle made of 2 triangles)
            var mesh = new MeshGeometry3D
                {
                Positions = new Point3DCollection
                {
                    new Point3D(-1, -1, 0),
                    new Point3D(1, -1, 0),
                    new Point3D(1, 1, 0),
                    new Point3D(-1, 1, 0)
                },
                TriangleIndices = new Int32Collection { 0, 1, 2, 0, 2, 3 },
                TextureCoordinates = new PointCollection
                {
                    new Point(0, 1),
                    new Point(1, 1),
                    new Point(1, 0),
                    new Point(0, 0)
                }
                };

            // Image Brush
            var imageBrush = new ImageBrush();

            string fileDir = "Data";
            string fileName = "dontPanic.jpg";

            string projectRoot = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;

            string filePath = Path.Combine(projectRoot, fileDir, fileName);

            imageBrush.ImageSource = new BitmapImage(new Uri(filePath));


            // Shadow/depth simulation behind image
            // 🎨 Front Material (Image)
            var frontMaterial = new DiffuseMaterial(imageBrush);

            // 🎨 Back Plane (simulated edge color)
            var backMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.BurlyWood));

            // 🌁 Main image model
            var frontModel = new GeometryModel3D
                {
                Geometry = mesh,
                Material = frontMaterial,
                BackMaterial = frontMaterial
                };

            // 💡 Simulated side color: just a dark rectangle placed slightly behind
            var shadowModel = new GeometryModel3D
                {
                Geometry = mesh,
                Material = backMaterial,
                BackMaterial = backMaterial,
                Transform = new TranslateTransform3D(0, 0, -0.05) // push it slightly back
                };

            // Combine both into the scene
            var modelGroup = new Model3DGroup();
            modelGroup.Children.Add(shadowModel); // behind
            modelGroup.Children.Add(frontModel);  // in front

            var modelVisual = new ModelVisual3D { Content = modelGroup };
            viewport.Children.Add(modelVisual);


            // rotation setup
            var rotationAxis = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0); // rotate 30 degrees on X axis
            var rotateTransform = new RotateTransform3D(rotationAxis);
            modelGroup.Transform = rotateTransform;

            //animate rotation
            var animation = new DoubleAnimation();
            animation.From = 0;
            animation.To = 360;
            animation.Duration = TimeSpan.FromSeconds(5);
            animation.RepeatBehavior = RepeatBehavior.Forever;

            //animate
            rotationAxis.BeginAnimation(AxisAngleRotation3D.AngleProperty, animation);

            // Add viewport to new window
            Window testWindow = new Window();
            testWindow.Width = 800;
            testWindow.Height = 600;

            testWindow.Content = viewport;

            //animate
            rotationAxis.BeginAnimation(AxisAngleRotation3D.AngleProperty, animation);

            testWindow.Show();
            }
        private void MainWindow1_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
            {
            //randomize position
            //btnOK.Margin = new Thickness(random.Next(width - 100), random.Next(height), 0, 0);

            //attach button to mouse
            //Point position = Mouse.GetPosition(this);
            //btnOK.Margin = new Thickness(position.X, position.Y, 0, 0);
            }

        private void btnTest_Click(object sender, RoutedEventArgs e)
            {
            ThreeDTest();
            }

        private void MainWindow1_Unloaded(object sender, RoutedEventArgs e)
            {
            //FadeOutMusic();

            //release and free MediaPlayer resources
            //player.Song.Stop();
            //player.Song.Close();

            //release and free Naudio resources
            player.outputDevice.Stop();
            player.outputDevice.Dispose();
            player.audioFileReader.Dispose();
            }
        }
    }