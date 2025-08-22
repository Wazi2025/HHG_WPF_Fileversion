using System.Windows;
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
            DoubleAnimation fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(2));
            image.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            }

        private void btnOK_Click(object sender, RoutedEventArgs e)
            {
            bool missingInfo = false;
            const string warning = "Please fill out all fields!";

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
                }
            else
            if (player.Age == player.dontPanic)
                {
                image.Visibility = Visibility.Visible;
                image.Opacity = 1.0;

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