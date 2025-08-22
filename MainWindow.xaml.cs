using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace HHG_WPF_Fileversion
    {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
        {
        //adding player field so we can instantiate player object in MainWindow's constructor
        private Player player;

        private ScaleTransform zoomTransform = new ScaleTransform(1, 1);
        private RotateTransform rotateTransform = new RotateTransform(0);
        private TransformGroup transformGroup = new TransformGroup();

        public MainWindow()
            {
            InitializeComponent();

            //Instantiate player object and pass it as a parameter whenever we need to access it outside this (MainWindow) class
            player = new Player();

            player.ReadFromFile(player);
            tbFirstName.Focus();
            }

        private void FadeInImage()
            {
            DoubleAnimation fadeIn = new DoubleAnimation(0, 0.25, TimeSpan.FromSeconds(5));
            image.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            }

        private void ZoomIn()
            {
            //ScaleTransform zoomTransform = new ScaleTransform(1, 1);

            //image.RenderTransformOrigin = new Point(0, 0);

            //image.RenderTransform = zoomTransform;

            DoubleAnimation zoomIn = new DoubleAnimation
                {
                From = 1.0,
                To = 1.5,
                Duration = TimeSpan.FromSeconds(2),
                AutoReverse = false,
                //RepeatBehavior = RepeatBehavior.Forever
                };

            zoomTransform.BeginAnimation(ScaleTransform.ScaleXProperty, zoomIn);
            zoomTransform.BeginAnimation(ScaleTransform.ScaleYProperty, zoomIn);
            }

        private void StartImageSpin()
            {
            // Create a RotateTransform and set it on the image
            //RotateTransform rotateTransform = new RotateTransform();
            //image.RenderTransform = rotateTransform;



            // Set the rotation center to the center of the image
            //image.RenderTransformOrigin = new Point(0.5, 0.5); // Center

            // Create the animation
            DoubleAnimation rotateAnimation = new DoubleAnimation
                {
                From = 0,
                To = 360,
                Duration = TimeSpan.FromSeconds(10),
                RepeatBehavior = RepeatBehavior.Forever
                };


            // Apply the animation to the RotateTransform
            rotateTransform.BeginAnimation(RotateTransform.AngleProperty, rotateAnimation);

            //zoomTransform.BeginAnimation(RotateTransform.AngleProperty, rotateAnimation);
            }

        private void btnOK_Click(object sender, RoutedEventArgs e)
            {
            bool missingInfo = false;
            const string warning = "Please fill out all fields!";

            image.RenderTransformOrigin = new Point(0.5, 0.5);
            transformGroup.Children.Add(zoomTransform);
            transformGroup.Children.Add(rotateTransform);
            image.RenderTransform = transformGroup;

            image.Visibility = Visibility.Hidden;
            player.ClearPlayerData(player);

            tbQuote.Text = player.ReadInput(tbFirstName.Text, tbLastName.Text, tbAge.Text, player);

            //check for textbox values
            if (String.IsNullOrWhiteSpace(tbFirstName.Text) || String.IsNullOrWhiteSpace(tbLastName.Text) || String.IsNullOrWhiteSpace(tbAge.Text))
                {
                missingInfo = true;

                tbQuote.Text = warning;
                image.Visibility = Visibility.Visible;
                //image.Opacity = 0.25;
                image.Source = player.ShowImage(player, missingInfo);

                FadeInImage();
                ZoomIn();

                StartImageSpin();
                }
            else
            if (player.Age == player.dontPanic)
                {
                image.Visibility = Visibility.Visible;
                //image.Opacity = 1.0;

                image.Source = player.ShowImage(player, missingInfo);
                FadeInImage();
                tbQuote.Text = "";
                }
            else
                {
                image.Visibility = Visibility.Hidden;
                //tbQuote.Text = player.ReadInput(tbFirstName.Text, tbLastName.Text, tbAge.Text, player);
                }
            }
        }
    }