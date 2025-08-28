using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;


namespace HHG_WPF_Fileversion
    {
    public class Player
        {
        //first/lastName can be private since they won't be accessed outside Player class
        private string firstName = "";
        private string lastName = "";

        //We only need get/set when property is to be accessed/modified outside it's class, i.e. it's public 
        public int Age { get; set; }

        //Use readonly instead of const so we can use player.dontPanic instead of Player.dontPanic
        //const is implicitly static, hence the need for type (Player) instead of instance (player)
        //
        //update: by using only get it's effectively read-only
        public int dontPanic { get; } = 42;

        //declare and initialize string list to store quotes from file
        private List<string> greetingList = new List<string>();

        //declare and initialize a BitmapImage for use with...well, bitmaps AKA images
        private BitmapImage bitmapImage = new BitmapImage();

        private readonly string projectRoot = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;
        private readonly string fileDir = "Data";
        private string fileName = "";
        private string filePath = "";

        //Player constructor
        //public Player()
        //    {
        //projectRoot = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;
        //fileName = "Andromeda-Galaxy-Milky-Way.jpg";
        //fileDir = "Data";
        //filePath = "";
        //firstName = "";
        //lastName = "";
        //}

        public void ReadFromFile(Player player)
            {
            //Instantiate list
            //greetingList = new List<string>();

            fileName = "quotes.txt";

            filePath = Path.Combine(projectRoot, fileDir, fileName);

            //Open a streamReader
            using StreamReader streamReader = new StreamReader(filePath);

            //Add each line to the greetingList as long as streamReader hasn't reached the end of the stream i.e. the file
            while (!streamReader.EndOfStream)
                {
                player.greetingList.Add(streamReader.ReadLine());
                }
            }// end of ReadFromFile method

        public BitmapImage ShowImage()
            {
            fileName = "Andromeda-Galaxy-Milky-Way.jpg";

            filePath = Path.Combine(projectRoot, fileDir, fileName);

            bitmapImage = new BitmapImage(new Uri(filePath));

            return bitmapImage;
            }

        public BitmapImage ShowImage(bool missingInfo)
            {
            if (missingInfo)
                fileName = "hhg2.png";
            else
                fileName = "dontPanic.jpg";

            filePath = Path.Combine(projectRoot, fileDir, fileName);

            bitmapImage = new BitmapImage(new Uri(filePath));

            return bitmapImage;
            }

        public void ClearPlayerData(Player player)
            {
            player.firstName = "";
            player.lastName = "";
            player.Age = 0;
            }

        public bool ReadInput(string firstName, string lastName, string age, Player player, TextBlock tbQuote)
            {
            //ask the user for their firstName, lastName and age and add these values to their respective player properties
            player.firstName = firstName;
            player.lastName = lastName;

            if (int.TryParse(age, out int result))
                player.Age = result;

            DateTime date = DateTime.Now;
            const string dateFormat = "dd MMMM, yyyy";
            const string timeFormat = "HH:mm:ss";
            const string dateMessage = "The date is:";
            const string timeMessage = "The time is:";

            //add quote and set text style 
            tbQuote.Inlines.Add(new Run($"{player.firstName} {player.lastName} ({player.Age} years).") { FontWeight = FontWeights.Bold });

            tbQuote.Inlines.Add(new Run("Your quote is:\n\n"));
            tbQuote.Inlines.Add(new Run($"{player.greetingList[date.Second]}\n\n") { FontStyle = FontStyles.Italic });
            tbQuote.Inlines.Add(new Run($"{dateMessage} {date.DayOfWeek} {date.ToString(dateFormat)}\n{timeMessage} {date.ToString(timeFormat)}\n\n"));

            if (String.IsNullOrWhiteSpace(player.firstName) || String.IsNullOrWhiteSpace(player.lastName) || player.Age == 0)
                return false;
            else
                return true;
            }//end of ReadInput
        }//end of class Player
    }
