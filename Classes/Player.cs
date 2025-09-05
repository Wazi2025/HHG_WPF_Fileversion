using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace HHG_WPF_Fileversion
    {
    public class Player
        {
        //We only need get/set when property is to be accessed/modified outside it's class, i.e. it's public
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public int Age { get; set; } = 0;

        //Use readonly instead of const so we can use player.DontPanic instead of Player.DontPanic
        //const is implicitly static, hence the need for type (Player) instead of instance (player)
        //
        //update: by using only get it's effectively read-only
        public int DontPanic { get; } = 42;

        //declare and initialize string list to store quotes from file
        private List<string> greetingList = new List<string>();

        ////declare and initialize a BitmapImage for use with...well, bitmaps AKA images
        //private BitmapImage bitmapImage = new BitmapImage();

        public string projectRoot { get; set; } = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;
        public string fileDir { get; set; } = "Data";
        public string fileName { get; set; } = "";
        public string filePath { get; set; } = "";

        public readonly string warning = "'Please fill out all fields. Although bypasses are the bedrock of humanity, this is the one and only exception.'";
        public readonly string author = "\n - Prostetnic Vogon Jeltz -";

        //declare and initialize a Random object
        public Random random = new Random();

        //Player constructor
        //public Player()
        //    {
        //projectRoot = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;
        //fileName = "Andromeda-Galaxy-Milky-Way.jpg";
        //fileDir = "Data";
        //filePath = "";
        //FirstName = "";
        //LastName = "";
        //}

        public void ReadFromFile(Player player)
            {
            fileName = "quotes.txt";

            filePath = Path.Combine(projectRoot, fileDir, fileName);

            //Open a streamReader
            using StreamReader streamReader = new StreamReader(filePath);

            //Add each line to the greetingList as long as streamReader hasn't reached the end of the stream i.e. the file
            while (!streamReader.EndOfStream)
                {
                greetingList.Add(streamReader.ReadLine());
                }
            }// end of ReadFromFile method


        public void ClearPlayerData(Player player)
            {
            player.FirstName = "";
            player.LastName = "";
            player.Age = 0;
            }

        public void SetWarning(TextBlock tbQuote, Player player)
            {
            //tbQuote is used by both quotes and Vogon warning, therefore we clear it's text property first
            tbQuote.Text = "";

            tbQuote.Inlines.Add(new Run(player.warning) { FontStyle = FontStyles.Italic });
            tbQuote.Inlines.Add(new Run(player.author) { FontWeight = FontWeights.Bold });
            }

        public void FetchQuote(TextBlock tbQuote, Player player)
            {
            DateTime date = DateTime.Now;
            const string dateFormat = "dd MMMM, yyyy";
            const string timeFormat = "HH:mm:ss";
            const string dateMessage = "The date is:";
            const string timeMessage = "The time is:";

            tbQuote.Text = "";

            //add quote and set text style 
            tbQuote.Inlines.Add(new Run($"{player.FirstName} {player.LastName} ({player.Age} years).") { FontWeight = FontWeights.Bold });

            tbQuote.Inlines.Add(new Run("Your quote is:\n\n"));
            tbQuote.Inlines.Add(new Run($"{player.greetingList[date.Second]}\n\n") { FontStyle = FontStyles.Italic });
            tbQuote.Inlines.Add(new Run($"{dateMessage} {date.DayOfWeek} {date.ToString(dateFormat)}\n{timeMessage} {date.ToString(timeFormat)}\n\n"));

            }

        public void ReadInput(string firstName, string lastName, string age, Player player)
            {
            //ask the user for their FirstName, LastName and Age and add these values to their respective player properties
            FirstName = firstName;
            LastName = lastName;

            if (int.TryParse(age, out int result))
                Age = result;

            }
        }//end of class Player
    }