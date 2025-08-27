using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace HHG_WPF_Fileversion
    {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
        {
        //adding player field so we can instantiate player object in MainWindow's constructor
        private Player player;

        //only one Animation can be active so we have to add both animations to a TransformGroup
        //and then add the TransformGroup to the image RenderTransform in the InitImageControls method
        private ScaleTransform zoomTransform = new ScaleTransform(1, 1);
        private RotateTransform rotateTransform = new RotateTransform(0);
        private TransformGroup transformGroup = new TransformGroup();

        private Random random = new Random();
        private double width;
        private double height;

        private Thickness original;

        public MainWindow()
            {
            InitializeComponent();

            //Instantiate player object and pass it as a parameter whenever we need to access it outside this (MainWindow) class
            player = new Player();

            //fetch contents in quotes file
            player.ReadFromFile(player);

            this.SizeToContent = SizeToContent.WidthAndHeight;

            //create new brush and set window's background image
            ImageBrush brush = new ImageBrush();
            brush.Opacity = 0.25;
            brush.ImageSource = player.ShowImage();
            this.Background = brush;

            //Init misc controls
            tbFirstName.Focus();
            original = btnOK.Margin;
            tbQuote.TextWrapping = TextWrapping.Wrap;
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
            DoubleAnimation zoomIn = new DoubleAnimation();

            zoomIn.From = 1.0;
            zoomIn.To = 1.5;
            zoomIn.Duration = TimeSpan.FromSeconds(2);
            zoomIn.AutoReverse = true;
            zoomIn.RepeatBehavior = RepeatBehavior.Forever;

            //apply the animation to zoomTransform
            zoomTransform.BeginAnimation(ScaleTransform.ScaleXProperty, zoomIn);
            zoomTransform.BeginAnimation(ScaleTransform.ScaleYProperty, zoomIn);
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

            //check for empty  values
            if (!player.ReadInput(tbFirstName.Text, tbLastName.Text, tbAge.Text, player, tbQuote))
                {
                missingInfo = true;

                tbQuote.Text = "";
                tbQuote.Inlines.Add(new Run(warning) { FontStyle = FontStyles.Italic });
                tbQuote.Inlines.Add(new Run(author) { FontWeight = FontWeights.Bold });

                image.Visibility = Visibility.Visible;
                image.Source = player.ShowImage(missingInfo);

                FadeInImage(0.25);
                ZoomIn();

                //randomize button placement
                btnOK.Margin = new Thickness(random.Next((int)width - 100), random.Next((int)height - 100), 0, 0);
                }
            else
            if (player.Age == player.dontPanic)
                {
                tbQuote.Text = "";

                image.Visibility = Visibility.Visible;
                image.Source = player.ShowImage(missingInfo);

                FadeInImage(1.0);
                ZoomIn();
                StartImageSpin();

                //restore button original position
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
            {
            InitImageControl();

            //get window height/width
            width = this.Width;
            height = this.Height;
            }

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
        }
    }