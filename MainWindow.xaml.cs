using System.Windows;
using System.Windows.Documents;
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
            this.SizeToContent = SizeToContent.WidthAndHeight;

            //Instantiate player object and pass it as a parameter whenever we need to access it outside this (MainWindow) class
            player = new Player();

            player.ReadFromFile(player);
            tbFirstName.Focus();
            original = btnOK.Margin;
            tbQuote.TextWrapping = TextWrapping.Wrap;
            }

        private void FadeInImage(double maxOpacity)
            {
            DoubleAnimation fadeIn = new DoubleAnimation(0, maxOpacity, TimeSpan.FromSeconds(5));
            image.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            }

        private void ZoomIn()
            {
            DoubleAnimation zoomIn = new DoubleAnimation
                {
                From = 1.0,
                To = 1.5,
                Duration = TimeSpan.FromSeconds(2),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
                };

            zoomTransform.BeginAnimation(ScaleTransform.ScaleXProperty, zoomIn);
            zoomTransform.BeginAnimation(ScaleTransform.ScaleYProperty, zoomIn);
            }

        private void StartImageSpin()
            {
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
                image.Visibility = Visibility.Visible;
                image.Source = player.ShowImage(missingInfo);
                tbQuote.Text = "";

                FadeInImage(1.0);
                ZoomIn();
                StartImageSpin();

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

            width = this.Width;
            height = this.Height;
            }

        private void MainWindow1_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
            {
            //randomize position
            //btnOK.Margin = new Thickness(random.Next(width - 100), random.Next(height), 0, 0);

            //attach button to mouse
            //Point position = Mouse.GetPosition(this);
            //btnOK.Margin = new Thickness(position.X, position.Y, 0, 0);
            }
        }
    }