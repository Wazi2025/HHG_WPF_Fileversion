using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace HHG_WPF_Fileversion
    {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
        {
        //adding player field so we can instantiate player object in MainWindow's constructor
        private Player player;

        //adding musicManager field
        private MusicManager musicManager;

        //adding gfxManager field
        private GfxManager gfxManager;

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

            //we'll do the same here as with player
            musicManager = new MusicManager();

            //we'll do the same here as with player
            gfxManager = new GfxManager();

            //fetch contents in quotes file
            player.ReadFromFile(player);

            //init screen stuff
            gfxManager.InitScreenStuff(player, gfxManager, mainWindow);

            //init animation stuff
            gfxManager.InitImageControl(image);

            //init music stuff
            musicManager.InitMusicStuff(player);

            //set focus to FirstName textbox
            //note: prolly set this in MainWindow.xaml
            tbFirstName.Focus();

            //set textwrap on
            //don't need this, done in MainWindow.xaml
            //tbQuote.TextWrapping = TextWrapping.Wrap;
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
                image.Source = gfxManager.ShowImage(missingInfo, player);
                player.SetWarning(tbQuote, player);
                gfxManager.FadeInImage(0.50, image);
                gfxManager.ZoomIn(missingInfo, player);

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

                image.Source = gfxManager.ShowImage(true, player);
                gfxManager.FadeInImage(0.50, image);
                gfxManager.ZoomIn(missingInfo, player);

                RestoreButtonPosition();

                RemoveExtraQuotes();
                }
            else
            //show spinning hhg image, clear quotes, restore button
            if (player.Age == player.DontPanic && !String.IsNullOrWhiteSpace(player.FirstName) && !String.IsNullOrWhiteSpace(player.LastName))
                {
                //set song position to it's most HHG's "moment"                
                musicManager.audioFileReader.CurrentTime = TimeSpan.FromMinutes(1.10);

                //player.fade.BeginFadeIn(1000);
                image.Source = gfxManager.ShowImage(missingInfo, player);
                image.Visibility = Visibility.Visible;

                tbQuote.Visibility = Visibility.Hidden;

                gfxManager.FadeInImage(1.0, image);
                gfxManager.ZoomIn(missingInfo, player);
                gfxManager.StartImageSpin();

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

                //randomize position of textblocks
                Canvas.SetLeft(textBlock, random.Next((int)this.Width - 100));
                Canvas.SetTop(textBlock, random.Next((int)this.Height - 100));

                //declare and instantiate a RotateTransform
                RotateTransform angle = new RotateTransform();

                //set angle randomly
                angle.Angle = random.Next(360);

                //set origin/rotation position to center of textBlock
                textBlock.RenderTransformOrigin = new Point(0.5, 0.5);

                //attach angle to textBlocks RenderTransform
                textBlock.RenderTransform = angle;
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
            musicManager.outputDevice.Stop();
            musicManager.outputDevice.Dispose();
            musicManager.audioFileReader.Dispose();

            //just in case the user decides to just close the program
            //RemoveExtraButtons();
            RemoveExtraQuotes();
            }
        }
    }