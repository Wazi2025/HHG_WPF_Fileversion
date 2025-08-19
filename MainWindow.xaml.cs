using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HHG_WPF_Fileversion
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Program.ReadFromFile();
            tbFirstName.Focus();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            image.Visibility = Visibility.Hidden;
            Program.ClearPlayerData();

            Program.ReadInput(tbFirstName.Text, tbLastName.Text, tbAge.Text);

            if (Program.player.Age== Program.player.dontPanic)
            {
                image.Visibility = Visibility.Visible;
                image.Source = Program.ShowImage();
            }
            else
            {
                image.Visibility = Visibility.Hidden;
                tbQuote.Text = Program.ReadInput(tbFirstName.Text, tbLastName.Text, tbAge.Text);
            }
        }
    }
}