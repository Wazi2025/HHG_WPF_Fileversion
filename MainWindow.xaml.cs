using HHG_WPF_Fileversion.Classes;
using System.Windows;
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

        //adding musicManager field
        private MusicManager musicManager;

        //adding gfxManager field
        private GfxManager gfxManager;

        //adding timer field
        private DispatcherTimer timer;
        private TimeSpan elapsedTime;


        //MainWindow's constructor
        public MainWindow()
            {
            InitializeComponent();

            //Instantiate player object and pass it as a parameter whenever we need to access it outside this (MainWindow) class
            player = new Player();

            //we'll do the same here as with player
            musicManager = new MusicManager(player);

            //we'll do the same here as with player
            gfxManager = new GfxManager();

            //fetch contents in quotes file
            player.ReadFromFile();

            //init screen stuff
            gfxManager.InitScreenStuff(player, mainWindow);

            //init animation stuff
            gfxManager.InitImageControl(image);

            // Create and configure timer
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += Timer_Tick;
            timer.Start();

            elapsedTime = TimeSpan.Zero;


            //set focus to FirstName textbox
            //note: prolly set this in MainWindow.xaml
            //tbFirstName.Focus();
            //update: done in MainWindows.xaml

            //set textwrap on
            //don't need this, done in MainWindow.xaml
            //tbQuote.TextWrapping = TextWrapping.Wrap;
            }

        private void Timer_Tick(object sender, EventArgs e)
            {
            elapsedTime = elapsedTime.Add(TimeSpan.FromSeconds(1));

            gfxManager.MultiplyVogonQuote(player, MainCanvas);

            if (elapsedTime == TimeSpan.FromSeconds(15))
                gfxManager.TransformTextBox(tbAge);

            }
        private void btnOK_Click(object sender, RoutedEventArgs e)
            {
            //stop timer
            timer.Stop();

            bool missingInfo = false;

            image.Visibility = Visibility.Hidden;

            player.ClearPlayerData(player);

            //Read player data
            player.ReadInput(tbFirstName.Text, tbLastName.Text, tbAge.Text);

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

                gfxManager.RemoveExtraQuotes(tbQuote, MainCanvas);

                gfxManager.RestoreGridPosition(SubGrid);
                gfxManager.RestoreTextBox(tbAge);
                gfxManager.RestoreButton(btnOK);
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

                gfxManager.RemoveExtraQuotes(tbQuote, MainCanvas);

                gfxManager.RestoreGridPosition(SubGrid);
                gfxManager.RestoreTextBox(tbAge);
                gfxManager.RestoreButton(btnOK);
                }
            else
            //show spinning hhg image, clear quotes, restore button
            if (player.Age == player.DontPanic && !String.IsNullOrWhiteSpace(player.FirstName) && !String.IsNullOrWhiteSpace(player.LastName))
                {
                //set song position to it's most HHG's "moment"
                musicManager.fade.BeginFadeOut(1000);
                musicManager.AudioReader.CurrentTime = TimeSpan.FromMinutes(1.10);
                musicManager.fade.BeginFadeIn(1000);

                image.Source = gfxManager.ShowImage(missingInfo, player);
                image.Visibility = Visibility.Visible;

                tbQuote.Visibility = Visibility.Hidden;

                gfxManager.FadeInImage(1.0, image);
                gfxManager.ZoomIn(missingInfo, player);
                gfxManager.StartImageSpin();

                gfxManager.RemoveExtraQuotes(tbQuote, MainCanvas);

                gfxManager.RestoreGridPosition(SubGrid);
                gfxManager.RestoreTextBox(tbAge);
                gfxManager.RestoreButton(btnOK);
                }
            else
                {
                if (!timer.IsEnabled)
                    timer.Start();

                tbQuote.Visibility = Visibility.Hidden;
                gfxManager.RotateSubGrid(SubGrid, player.random);
                gfxManager.TransformButton(btnOK, player.random);
                }
            }//end of btnOk_Click


        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
            {
            tbQuote.Visibility = Visibility.Hidden;

            //gfxManager.RandomizeButton(MainCanvas, btnOK, player.random);
            //gfxManager.MultiplyVogonQuote(player, MainCanvas);

            gfxManager.RotateSubGrid(SubGrid, player.random);
            //gfxManager.TransformTextBox(tbAge, player.random);
            gfxManager.TransformButton(btnOK, player.random);

            }

        private void mainWindow_Closed(object sender, EventArgs e)
            {
            //release and free NAudio resources
            //call Dispose method to free resources instead            
            musicManager.Dispose();

            //just in case the user decides to just close the program            
            gfxManager.RemoveExtraQuotes(tbQuote, MainCanvas);

            }
        }
    }