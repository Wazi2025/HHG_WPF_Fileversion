using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
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

        private Button button;


        //MainWindow's constructor
        public MainWindow()
            {
            InitializeComponent();

            //Instantiate player object and pass it as a parameter whenever we need to access it outside this (MainWindow) class
            player = new Player();

            //fetch contents in quotes file
            player.ReadFromFile(player);

            //set window to autosize based on its content (controls like buttons, textboxes etc...)
            //this.SizeToContent = SizeToContent.WidthAndHeight;

            //create new brush and set window's background image
            ImageBrush brush = new ImageBrush();
            brush.Opacity = 0.25;
            brush.ImageSource = player.ShowImage();
            this.Background = brush;

            //init animation stuff
            InitImageControl();

            //set focus to firstName textbox
            tbFirstName.Focus();

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

        private void ZoomIn(bool missingInfo)
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
            if (missingInfo && player.Age == player.DontPanic)
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


            image.Visibility = Visibility.Hidden;

            player.ClearPlayerData(player);



            //check for empty values
            if (!player.ReadInput(tbFirstName.Text, tbLastName.Text, tbAge.Text, player, tbQuote))
                {
                missingInfo = true;

                tbQuote.Text = "";
                tbQuote.Inlines.Add(new Run(player.warning) { FontStyle = FontStyles.Italic });
                tbQuote.Inlines.Add(new Run(player.author) { FontWeight = FontWeights.Bold });

                image.Visibility = Visibility.Visible;
                image.Source = player.ShowImage(missingInfo);

                FadeInImage(0.50);
                ZoomIn(missingInfo);

                //randomize button placement or technically the Canvas since btnOk is it's only member
                Canvas.SetLeft(btnOK, random.Next((int)this.Width - 100));
                Canvas.SetTop(btnOK, random.Next((int)this.Height - 100));


                //for (int i = 0; i < 10; i++)
                //    {
                button = new Button();

                //button.Content = $"Test {i}";
                button.Content = btnOK.Content;
                //button.Name = btnOK.Content + i.ToString();
                button.Name = "Test";
                //add button to MainGrid
                MainCanvas.Children.Add(button);

                Canvas.SetLeft(button, random.Next((int)this.Width - 100));
                Canvas.SetTop(button, random.Next((int)this.Height - 100));

                //  }

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
                ZoomIn(missingInfo);
                StartImageSpin();

                //restore button's original position
                RestoreButtonPosition();
                RemoveExtraButtons();
                }
            else
                {
                tbQuote.Text = "";

                player.ReadInput(tbFirstName.Text, tbLastName.Text, tbAge.Text, player, tbQuote);

                image.Visibility = Visibility.Hidden;

                RestoreButtonPosition();
                RemoveExtraButtons();
                }
            }

        private void RemoveExtraButtons()
            {
            // Collect buttons into a temporary list (to avoid modifying the collection during iteration)
            List<Button> buttonsToRemove = new List<Button>();

            foreach (UIElement element in MainCanvas.Children)
                {
                if (element is Button btn && btn.Name != "btnOK")
                    {
                    buttonsToRemove.Add(btn);
                    }
                }

            // Now remove them
            foreach (Button btn in buttonsToRemove)
                {
                MainCanvas.Children.Remove(btn);
                }
            }

        private void RestoreButtonPosition()
            {
            Canvas.SetLeft(btnOK, 180);
            Canvas.SetTop(btnOK, 230);
            }

        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //    {

        //    }


        private void MainWindow1_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
            {
            //randomize position
            //btnOK.Margin = new Thickness(random.Next(width - 100), random.Next(height), 0, 0);

            //attach button to mouse
            //Point position = Mouse.GetPosition(this);
            //btnOK.Margin = new Thickness(position.X, position.Y, 0, 0);
            }

        //private void btnTest_Click(object sender, RoutedEventArgs e)
        //    {

        //    }

        private void MainWindow1_Unloaded(object sender, RoutedEventArgs e)
            {
            //FadeOutMusic();

            //release and free MediaPlayer resources
            //player.Song.Stop();
            //player.Song.Close();

            //release and free NAudio resources
            player.outputDevice.Stop();
            player.outputDevice.Dispose();
            player.audioFileReader.Dispose();

            }
        }
    }