using HHG_WPF_Fileversion.Classes;
using System.Windows;

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
            gfxManager.InitScreenStuff(player, gfxManager, mainWindow);

            //init animation stuff
            gfxManager.InitImageControl(image);

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

                gfxManager.RestoreButtonPosition(btnOK);

                gfxManager.RemoveExtraQuotes(tbQuote, MainCanvas);
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

                gfxManager.RestoreButtonPosition(btnOK);

                gfxManager.RemoveExtraQuotes(tbQuote, MainCanvas);
            }
            else
            //show spinning hhg image, clear quotes, restore button
            if (player.Age == player.DontPanic && !String.IsNullOrWhiteSpace(player.FirstName) && !String.IsNullOrWhiteSpace(player.LastName))
            {
                //set song position to it's most HHG's "moment"                
                musicManager.AudioReader.CurrentTime = TimeSpan.FromMinutes(1.10);

                //player.fade.BeginFadeIn(1000);
                image.Source = gfxManager.ShowImage(missingInfo, player);
                image.Visibility = Visibility.Visible;

                tbQuote.Visibility = Visibility.Hidden;

                gfxManager.FadeInImage(1.0, image);
                gfxManager.ZoomIn(missingInfo, player);
                gfxManager.StartImageSpin();

                gfxManager.RestoreButtonPosition(btnOK);

                gfxManager.RemoveExtraQuotes(tbQuote, MainCanvas);
            }
            else
            //show chaos if any fields are empty
            {
                tbQuote.Visibility = Visibility.Hidden;

                gfxManager.RandomizeButton(MainCanvas, btnOK, player.random);
                gfxManager.MultiplyVogonQuote(player, MainCanvas);
            }
        }//end of btnOk_Click


        private void MainGrid_Unloaded(object sender, RoutedEventArgs e)
        {
            //release and free NAudio resources
            //musicManager.outputDevice.Stop();
            //musicManager.outputDevice.Dispose();
            //musicManager.audioFileReader.Dispose();

            //call Dispose method to free resources instead            
            musicManager.Dispose();

            //just in case the user decides to just close the program
            //RemoveExtraButtons();
            gfxManager.RemoveExtraQuotes(tbQuote, MainCanvas);
        }
    }
}