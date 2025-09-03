using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.Windows;
using System.Windows.Controls;
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

        //declare and initialize animation stuff
        private ScaleTransform zoomTransform = new ScaleTransform(1, 1);
        private RotateTransform rotateTransform = new RotateTransform(0);
        private TransformGroup transformGroup = new TransformGroup();

        //declare and initialize a Random object
        private Random random = new Random();

        //declare Button to be used in CreateButtons method
        //private Button button;

        //declare TextBlock to be used in MultiplyVogonQuote
        private TextBlock textBlock;

        //MainWindow's constructor
        public MainWindow()
        {
            InitializeComponent();

            //Instantiate player object and pass it as a parameter whenever we need to access it outside this (MainWindow) class
            player = new Player();

            //fetch contents in quotes file
            player.ReadFromFile(player);

            //init screen stuff
            InitScreenStuff();

            //init animation stuff
            InitImageControl();

            //init music stuff
            InitMusicStuff();

            //set focus to FirstName textbox
            //note: prolly set this in MainWindow.xaml
            tbFirstName.Focus();

            //set textwrap on
            //don't need this, done in MainWindow.xaml
            //tbQuote.TextWrapping = TextWrapping.Wrap;
        }

        private void InitMusicStuff()
        {
            //init audio stuff
            player.audioFileReader = new AudioFileReader(player.GetSong());
            player.fade = new FadeInOutSampleProvider(player.audioFileReader, true);

            //set fade-in to 2 seconds
            player.fade.BeginFadeIn(2000);

            //attach to an output device
            player.outputDevice = new WaveOutEvent();
            player.outputDevice.Init(player.fade);

            //start playing
            player.outputDevice.Play();
        }

        private void InitScreenStuff()
        {
            //create new brush and set window's background image
            ImageBrush brush = new ImageBrush();
            brush.Opacity = 0.25;
            brush.ImageSource = player.ShowImage();
            this.Background = brush;

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.WindowState = WindowState.Maximized;
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

            //only bounce if only 42 
            if (missingInfo && player.Age == player.DontPanic)
            {
                BounceEase bounceEase = new BounceEase();
                bounceEase.Bounces = 1;
                bounceEase.Bounciness = 4;
                bounceEase.EasingMode = EasingMode.EaseOut;

                zoomIn.EasingFunction = bounceEase;
            }

            //apply the animation (and bounce if applicable) to zoomTransform
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

            //apply the animation (spin) to rotateTransform
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

            //Read player data
            player.ReadInput(tbFirstName.Text, tbLastName.Text, tbAge.Text, player);

            //fetch quote
            player.FetchQuote(tbQuote, player);

            //show bouncing logo, clear quotes, restore button
            if (player.Age == player.DontPanic && String.IsNullOrWhiteSpace(player.FirstName) && String.IsNullOrWhiteSpace(player.LastName))
            {
                missingInfo = true;

                image.Visibility = Visibility.Visible;
                tbQuote.Visibility = Visibility.Visible;
                image.Source = player.ShowImage(missingInfo);
                player.SetWarning(tbQuote, player);
                FadeInImage(0.50);
                ZoomIn(missingInfo);

                RestoreButtonPosition();

                RemoveExtraQuotes();
            }
            else
            //show quote and logo, restore button
            if (player.Age != player.DontPanic && !String.IsNullOrWhiteSpace(player.FirstName) && !String.IsNullOrWhiteSpace(player.LastName))
            {
                image.Visibility = Visibility.Visible;
                tbQuote.Visibility = Visibility.Visible;
                player.FetchQuote(tbQuote, player);

                image.Source = player.ShowImage(true);
                FadeInImage(0.50);
                ZoomIn(missingInfo);

                RestoreButtonPosition();

                RemoveExtraQuotes();
            }
            else
            //show spinning hhg image, clear quotes, restore button
            if (player.Age == player.DontPanic && !String.IsNullOrWhiteSpace(player.FirstName) && !String.IsNullOrWhiteSpace(player.LastName))
            {
                //set song position to it's most HHG's "moment"                
                player.audioFileReader.CurrentTime = TimeSpan.FromMinutes(1.10);

                //player.fade.BeginFadeIn(1000);
                image.Source = player.ShowImage(missingInfo);
                image.Visibility = Visibility.Visible;

                tbQuote.Visibility = Visibility.Hidden;

                FadeInImage(1.0);
                ZoomIn(missingInfo);
                StartImageSpin();

                RestoreButtonPosition();

                RemoveExtraQuotes();
            }
            else
            //show chaos if any fields are empty
            {
                tbQuote.Visibility = Visibility.Hidden;
                RandomizeButton();
                MultiplyVogonQuote();
            }
        }//end of btnOk_Click


        private void RandomizeButton()
        {
            //randomize button placement or technically the Canvas
            Canvas.SetLeft(btnOK, random.Next((int)this.Width - 100));
            Canvas.SetTop(btnOK, random.Next((int)this.Height - 100));

            //note: Canvas.ZIndex="1" in MainWindow.xaml makes sure the button is "on top" so it's reachable with mouse too
            //since no other elements use ZIndex we can just use 1
        }
        private void MultiplyVogonQuote()
        {
            //instantiate textblocks
            for (int i = 0; i <= player.DontPanic; i++)
            {
                textBlock = new TextBlock();

                //brush must be something light since the background is pretty dark
                textBlock.Foreground = Brushes.White;
                textBlock.Name = "Test";

                //Add Vogon warning to new textblocks. This is a bit cludgy but it'll suffice for now
                //Can't use textBlock.Text = tbQuote.Text since that is pure text only
                textBlock.Inlines.Add(new Run(player.warning) { FontStyle = FontStyles.Italic });
                textBlock.Inlines.Add(new Run(player.author) { FontWeight = FontWeights.Bold });

                //add textBlock to MainCanvas
                MainCanvas.Children.Add(textBlock);

                //randomize textblocks
                Canvas.SetLeft(textBlock, random.Next((int)this.Width - 100));
                Canvas.SetTop(textBlock, random.Next((int)this.Height - 100));
            }
        }

        private void RemoveExtraQuotes()
        {
            //collect textblocks into a temporary list (to avoid modifying the collection during iteration)
            List<TextBlock> textBlocksToRemove = new List<TextBlock>();

            //iterate through each element in canvas
            foreach (UIElement element in MainCanvas.Children)
            {
                //add to list if element is of type textblock AND it's name is anything but the original textblock
                if (element is TextBlock textBlock && textBlock.Name != tbQuote.Name)
                {
                    textBlocksToRemove.Add(textBlock);
                }
            }

            //now remove them
            foreach (TextBlock textBlock in textBlocksToRemove)
            {
                MainCanvas.Children.Remove(textBlock);
            }
        }

        //private void CreateButtons()
        //    {
        //    for (int i = 0; i < player.DontPanic - 1; i++)
        //        {
        //        button = new Button();

        //        button.Content = btnOK.Content;
        //        button.Name = "Test";

        //        //add button to MainCanvas
        //        MainCanvas.Children.Add(button);

        //        Canvas.SetLeft(button, random.Next((int)this.Width - 100));
        //        Canvas.SetTop(button, random.Next((int)this.Height - 100));
        //        }
        //    }

        //private void RemoveExtraButtons()
        //    {
        //    List<Button> buttonsToRemove = new List<Button>();

        //    foreach (UIElement element in MainCanvas.Children)
        //        {
        //        if (element is Button btn && btn.Name != btnOK.Name)
        //            {
        //            buttonsToRemove.Add(btn);
        //            }
        //        }

        //    foreach (Button btn in buttonsToRemove)
        //        {
        //        MainCanvas.Children.Remove(btn);
        //        }
        //    }

        private void RestoreButtonPosition()
        {
            //restore button's original position (from MainWindow.xaml)
            Canvas.SetLeft(btnOK, 165);
            Canvas.SetTop(btnOK, 230);
        }


        private void MainWindow1_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //randomize position
            //btnOK.Margin = new Thickness(random.Next(width - 100), random.Next(height), 0, 0);

            //attach button to mouse
            //Point position = Mouse.GetPosition(this);
            //btnOK.Margin = new Thickness(position.X, position.Y, 0, 0);
        }


        private void MainWindow1_Unloaded(object sender, RoutedEventArgs e)
        {
            //release and free NAudio resources
            player.outputDevice.Stop();
            player.outputDevice.Dispose();
            player.audioFileReader.Dispose();

            //just in case the user decides to just close the program
            //RemoveExtraButtons();
            RemoveExtraQuotes();
        }
    }
}