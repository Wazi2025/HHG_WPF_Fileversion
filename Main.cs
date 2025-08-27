using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;


namespace HHG_WPF_Fileversion
    {
    public class Player
        {
        //Since we aren't using any custom logic in get/set we'll use C#'s auto-implementation
        private string FirstName { get; set; }
        private string LastName { get; set; }
        public int Age { get; set; }

        //Use readonly instead of const so we can use player.dontPanic instead of Player.dontPanic
        //const is implicitly static, hence the need for type (Player) instead of instance (player)
        public readonly int dontPanic = 42;

        //string list to store quotes from file
        private List<string> greetingList;

        private BitmapImage bitmapImage;

        private string FileDir { get; set; }
        private string FileName { get; set; }
        private string FilePath { get; set; }
        private string ProjectRoot { get; set; }

        //Player constructor
        public Player()
            {
            ProjectRoot = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;
            FileName = "Andromeda-Galaxy-Milky-Way.jpg";
            FileDir = "Data";
            FilePath = "";
            FirstName = "";
            LastName = "";

            }

        public void ReadFromFile(Player player)
            {
            //Instantiate list
            greetingList = new List<string>();

            FileName = "quotes.txt";

            FilePath = Path.Combine(ProjectRoot, FileDir, FileName);

            //Open a streamReader
            using StreamReader streamReader = new StreamReader(FilePath);

            //Add each line to the greetinglist as long as streamReader hasn't reached the end of the stream i.e. the file
            while (!streamReader.EndOfStream)
                {
                player.greetingList.Add(streamReader.ReadLine());
                }
            }// end of ReadFromFile method

        public BitmapImage ShowImage()
            {
            FileName = "Andromeda-Galaxy-Milky-Way.jpg";

            FilePath = Path.Combine(ProjectRoot, FileDir, FileName);

            bitmapImage = new BitmapImage(new Uri(FilePath));

            return bitmapImage;
            }

        public BitmapImage ShowImage(bool missingInfo)
            {
            if (missingInfo)
                FileName = "hhg2.png";
            else
                FileName = "dontPanic.jpg";

            FilePath = Path.Combine(ProjectRoot, FileDir, FileName);

            bitmapImage = new BitmapImage(new Uri(FilePath));

            return bitmapImage;
            }

        public void ClearPlayerData(Player player)
            {
            player.FirstName = "";
            player.LastName = "";
            player.Age = 0;
            }

        public bool ReadInput(string firstName, string lastName, string age, Player player, TextBlock tbQuote)
            {
            //ask the user for their firstname, lastname and age and add these values to their respective player properties
            player.FirstName = firstName;
            player.LastName = lastName;

            if (int.TryParse(age, out int result))
                player.Age = result;

            //Show output
            DateTime date = DateTime.Now;
            const string dateFormat = "dd MMMM, yyyy";
            const string timeFormat = "HH:mm:ss";
            const string dateMessage = "The date is:";
            const string timeMessage = "The time is:";

            tbQuote.Inlines.Add(new Run($"{player.FirstName} {player.LastName} ({player.Age} years).") { FontWeight = FontWeights.Bold });

            tbQuote.Inlines.Add(new Run("Your quote is:\n\n"));
            tbQuote.Inlines.Add(new Run($"{player.greetingList[date.Second]}\n\n") { FontStyle = FontStyles.Italic });
            tbQuote.Inlines.Add(new Run($"{dateMessage} {date.DayOfWeek} {date.ToString(dateFormat)}\n{timeMessage} {date.ToString(timeFormat)}\n\n"));

            if (String.IsNullOrWhiteSpace(player.FirstName) || String.IsNullOrWhiteSpace(player.LastName) || player.Age == 0)
                return false;
            else
                return true;
            }//end of ReadInput
        }//end of class Player

    }
