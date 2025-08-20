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
    public MainWindow()
    {
        InitializeComponent();

        //Instantiate player object and pass it as a parameter whenever we need to access it outside this class
        player = new Player();

        player.ReadFromFile(player);
        tbFirstName.Focus();
    }

    private void btnOK_Click(object sender, RoutedEventArgs e)
    {
        image.Visibility = Visibility.Hidden;
        Program.ClearPlayerData(player);

        Program.ReadInput(tbFirstName.Text, tbLastName.Text, tbAge.Text, player);

        if (player.Age== player.dontPanic)
        {
            image.Visibility = Visibility.Visible;
            image.Source = Program.ShowImage();
        }
        else
        {
            image.Visibility = Visibility.Hidden;
            tbQuote.Text = Program.ReadInput(tbFirstName.Text, tbLastName.Text, tbAge.Text, player);
        }
    }
}
}