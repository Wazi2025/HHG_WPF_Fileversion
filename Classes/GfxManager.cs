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

        private int textBlockAmount = 0;

        public void InitScreenStuff(Player player, Window mainWindow)
            {
            //create new brush and set window's background image
            ImageBrush brush = new ImageBrush();
            brush.Opacity = 0.25;
            brush.ImageSource = ShowImage(player);
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
            player.FilePath = Path.Combine(AppContext.BaseDirectory, player.FileDir, player.FileName);

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

            player.FilePath = Path.Combine(AppContext.BaseDirectory, player.FileDir, player.FileName);

            bitmapImage = new BitmapImage(new Uri(player.FilePath));

            return bitmapImage;
            }

        //public void RandomizeButton(Canvas canvas, Button btnOK, Random random)
        //    {
        //    //randomize button placement or technically the Canvas
        //    //note: SetLeft is a static method, hence we can only access it with it's type
        //    Canvas.SetLeft(btnOK, random.Next((int)canvas.ActualWidth));
        //    Canvas.SetTop(btnOK, random.Next((int)canvas.ActualHeight));

        //    //note: Canvas.ZIndex="1" in MainWindow.xaml makes sure the button is "on top" so it's reachable with mouse too
        //    //since no other elements use ZIndex we can just use 1
        //    }

        //public void RestoreButtonPosition(Button btnOK)
        //    {
        //    //restore button's original position (from MainWindow.xaml)
        //    Canvas.SetLeft(btnOK, 165);
        //    Canvas.SetTop(btnOK, 230);
        //    }

        public void TransformButton(Button btnOK, Random random)
            {
            //test
            double min = 1.0;
            double max = 14.0;
            double value = random.NextDouble() * (max - min) + min;

            //instantiate an animation 
            DoubleAnimation anim = new DoubleAnimation();
            anim.From = 0;
            anim.To = 360;
            anim.Duration = TimeSpan.FromSeconds(value);
            anim.RepeatBehavior = RepeatBehavior.Forever;

            //instantiate an animation
            DoubleAnimation animScaling = new DoubleAnimation();
            animScaling.From = 1;
            animScaling.To = 3;
            animScaling.Duration = TimeSpan.FromSeconds(value);
            animScaling.RepeatBehavior = RepeatBehavior.Forever;
            animScaling.AutoReverse = true;

            //rotate
            RotateTransform rotate = new RotateTransform();
            rotate.BeginAnimation(RotateTransform.AngleProperty, anim);

            btnOK.RenderTransformOrigin = new Point(0.5, 0.5);

            //scale
            ScaleTransform scale = new ScaleTransform();

            //From and To from anim2 refers to scaling here, in this case the size of the button
            scale.BeginAnimation(ScaleTransform.ScaleXProperty, animScaling);
            scale.BeginAnimation(ScaleTransform.ScaleYProperty, animScaling);

            //transformgroup
            //we're using more than one animation on the same element
            TransformGroup group = new TransformGroup();
            group.Children.Add(rotate);
            group.Children.Add(scale);
            btnOK.RenderTransform = group;

            //instantiate a fade effect
            DoubleAnimation fade = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(5));
            fade.RepeatBehavior = RepeatBehavior.Forever;
            fade.AutoReverse = true;

            //add fade effect to element (btnOK in this case)
            btnOK.BeginAnimation(UIElement.OpacityProperty, fade);
            }

        public void RestoreButton(Button btnOK)
            {
            //resett RenderTransform
            btnOK.RenderTransform = null;
            btnOK.BeginAnimation(UIElement.OpacityProperty, null);
            btnOK.Opacity = 1;
            }

        public void TransformTextBox(TextBox tbAge)
            {
            RotateTransform rotate = new RotateTransform();
            DoubleAnimation anim = new DoubleAnimation();

            //tbAge.RenderTransformOrigin = new Point(0.0, 0.0);
            tbAge.RenderTransform = rotate;

            BounceEase bounceEase = new BounceEase();
            bounceEase.Bounces = 3;
            bounceEase.Bounciness = 2;
            bounceEase.EasingMode = EasingMode.EaseOut;

            anim.EasingFunction = bounceEase;

            anim.From = 0;
            anim.To = 90;

            anim.Duration = TimeSpan.FromSeconds(1);

            rotate.BeginAnimation(RotateTransform.AngleProperty, anim);
            }

        public void RestoreTextBox(TextBox tbAge)
            {
            tbAge.RenderTransform = null;
            }

        public void RotateSubGrid(Grid SubGrid, Random random)
            {
            double min = 5.0;
            double max = 20.0;
            double value = random.NextDouble() * (max - min) + min;

            RotateTransform rotate = new RotateTransform();

            //instantiate an animation 
            DoubleAnimation spin = new DoubleAnimation();
            spin.From = 0;
            spin.To = 360;
            spin.Duration = TimeSpan.FromSeconds(value);
            spin.RepeatBehavior = RepeatBehavior.Forever;

            //screen rotation
            SubGrid.RenderTransformOrigin = new Point(0.5, 0.5);
            SubGrid.RenderTransform = rotate;

            rotate.BeginAnimation(RotateTransform.AngleProperty, spin);
            }

        public void RestoreGridPosition(Grid SubGrid)
            {
            SubGrid.RenderTransform = null;
            }

        public void MultiplyVogonQuote(Player player, Canvas MainCanvas)
            {
            if (textBlockAmount < 6)
                {
                //instantiate textblocks
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

                //declare and instantiate a RotateTransform for text/quote rotation
                RotateTransform angle = new RotateTransform();

                //set starting angle randomly
                angle.Angle = player.random.Next(360);

                //set origin/rotation position to center of textBlock
                textBlock.RenderTransformOrigin = new Point(0.5, 0.5);

                //instantiate a fade effect
                DoubleAnimation fade = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(5));
                fade.RepeatBehavior = RepeatBehavior.Forever;
                fade.AutoReverse = true;

                //test
                double min = 4.0;
                double max = 20.0;
                double value = player.random.NextDouble() * (max - min) + min;

                //instantiate an animation 
                DoubleAnimation spin = new DoubleAnimation();

                //set a sort of random spin direction
                if (value > 10)
                    {
                    spin.From = 0;
                    spin.To = 360;
                    }
                else
                    {
                    spin.From = -1;
                    spin.To = -360;
                    }

                //spin.Duration = TimeSpan.FromSeconds(player.random.Next(1, 10));
                spin.Duration = TimeSpan.FromSeconds(value);
                spin.RepeatBehavior = RepeatBehavior.Forever;

                //attach angle to textBlocks RenderTransform
                textBlock.RenderTransform = angle;

                //begin opacity anim
                textBlock.BeginAnimation(UIElement.OpacityProperty, fade);

                //begin spin anim
                angle.BeginAnimation(RotateTransform.AngleProperty, spin);
                }

            //increment textBlockAmount
            textBlockAmount++;
            }


        public void RemoveExtraQuotes(TextBlock tbQuote, Canvas canvas)
            {
            textBlockAmount = 0;

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

        }//end of GfxManager
    }
