using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace HHG_WPF_Fileversion.Classes
    {
    public class GfxManager
        {
        //declare and initialize a BitmapImage for use with...well, bitmaps AKA images
        private BitmapImage bitmapImage = new BitmapImage();

        //declare and initialize animation stuff
        private ScaleTransform zoomTransform = new ScaleTransform(1, 1);
        private RotateTransform rotateTransform = new RotateTransform(0);
        private TransformGroup transformGroup = new TransformGroup();


        public void InitScreenStuff(Player player, GfxManager gfxManager, Window mainWindow)
            {
            //create new brush and set window's background image
            ImageBrush brush = new ImageBrush();
            brush.Opacity = 0.25;
            brush.ImageSource = gfxManager.ShowImage(player);
            mainWindow.Background = brush;

            mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            mainWindow.WindowState = WindowState.Maximized;
            }

        public void FadeInImage(double maxOpacity, Image image)
            {
            DoubleAnimation fadeIn = new DoubleAnimation(0, maxOpacity, TimeSpan.FromSeconds(5));

            fadeIn.AutoReverse = true;
            fadeIn.RepeatBehavior = RepeatBehavior.Forever;
            image.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            }

        public void ZoomIn(bool missingInfo, Player player)
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

        public void StartImageSpin()
            {
            DoubleAnimation rotateAnimation = new DoubleAnimation();

            rotateAnimation.From = 0;
            rotateAnimation.To = 360;
            rotateAnimation.Duration = TimeSpan.FromSeconds(10);
            rotateAnimation.RepeatBehavior = RepeatBehavior.Forever;

            //apply the animation (spin) to rotateTransform
            rotateTransform.BeginAnimation(RotateTransform.AngleProperty, rotateAnimation);
            }

        public void InitImageControl(Image image)
            {
            //only one animation can be active so we have to add both animations to a TransformGroup
            //and then add the TransformGroup to the image RenderTransform
            image.RenderTransformOrigin = new Point(0.5, 0.5);
            transformGroup.Children.Add(zoomTransform);
            transformGroup.Children.Add(rotateTransform);
            image.RenderTransform = transformGroup;
            }

        public BitmapImage ShowImage(Player player)
            {
            player.FileName = "Andromeda-Galaxy-Milky-Way.jpg";
            player.FilePath = Path.Combine(player.ProjectRoot, player.FileDir, player.FileName);

            bitmapImage = new BitmapImage(new Uri(player.FilePath));

            //both ShowImage() methods: this might be a safer method according to ChatGPT. Not entirely sure I see the point, though.
            //the file is automatically released when the program ends anyway
            //bitmapImage.BeginInit();
            //bitmapImage.UriSource = new Uri(player.FilePath);
            //bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            //bitmapImage.EndInit();

            return bitmapImage;
            }

        public BitmapImage ShowImage(bool missingInfo, Player player)
            {
            if (missingInfo)
                player.FileName = "hhg2.png";
            else
                player.FileName = "DontPanic.jpg";

            player.FilePath = Path.Combine(player.ProjectRoot, player.FileDir, player.FileName);

            bitmapImage = new BitmapImage(new Uri(player.FilePath));

            return bitmapImage;
            }

        public void RandomizeButton(Canvas canvas, Button btnOK, Random random)
            {
            //randomize button placement or technically the Canvas
            //note: SetLeft is a static method, hence we can only access it with it's type
            Canvas.SetLeft(btnOK, random.Next((int)canvas.ActualWidth));
            Canvas.SetTop(btnOK, random.Next((int)canvas.ActualHeight));

            //note: Canvas.ZIndex="1" in MainWindow.xaml makes sure the button is "on top" so it's reachable with mouse too
            //since no other elements use ZIndex we can just use 1
            }

        public void MultiplyVogonQuote(Player player, Canvas MainCanvas)
            {
            Debug.WriteLine(MainCanvas.IsLoaded); // should be true
            Debug.WriteLine(VisualTreeHelper.GetParent(MainCanvas)); // should not be null

            //instantiate textblocks
            for (int i = 0; i <= player.DontPanic; i++)
                {
                TextBlock textBlock = new TextBlock();

                //brush must be something light since the background is pretty dark
                textBlock.Foreground = Brushes.White;
                textBlock.Name = "Test";

                //Add Vogon warning to new textblocks. This is a bit cludgy but it'll suffice for now
                //Can't use textBlock.Text = tbQuote.Text since that is pure text only
                textBlock.Inlines.Add(new Run(player.warning) { FontStyle = FontStyles.Italic });
                textBlock.Inlines.Add(new Run(player.author) { FontWeight = FontWeights.Bold });

                //add textBlock to MainCanvas
                MainCanvas.Children.Add(textBlock);

                //randomize position of textblocks
                Canvas.SetLeft(textBlock, player.random.Next((int)MainCanvas.ActualWidth));
                Canvas.SetTop(textBlock, player.random.Next((int)MainCanvas.ActualHeight));

                //declare and instantiate a RotateTransform
                RotateTransform angle = new RotateTransform();

                //set angle randomly
                angle.Angle = player.random.Next(360);

                //set origin/rotation position to center of textBlock
                textBlock.RenderTransformOrigin = new Point(0.5, 0.5);

                //attach angle to textBlocks RenderTransform
                textBlock.RenderTransform = angle;
                }
            }

        public void RemoveExtraQuotes(TextBlock tbQuote, Canvas canvas)
            {
            //collect textblocks into a temporary list (to avoid modifying the collection during iteration)
            List<TextBlock> textBlocksToRemove = new List<TextBlock>();

            //iterate through each element in canvas
            foreach (UIElement element in canvas.Children)
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
                canvas.Children.Remove(textBlock);
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

        public void RestoreButtonPosition(Button btnOK)
            {
            //restore button's original position (from MainWindow.xaml)
            Canvas.SetLeft(btnOK, 165);
            Canvas.SetTop(btnOK, 230);
            }

        }//end of GfxManager
    }
