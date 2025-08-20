using System.IO;
using System.Windows.Media.Imaging;


namespace HHG_WPF_Fileversion
{
    public class Player
    {
        //Since we aren't using any custom logic in get/set we'll use C#'s auto-implementation
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }

        public int dontPanic = 42;

        //string list to store quotes from file
        public List<string> greetingList;

        public void ReadFromFile(Player player)
        {
            //Instantiate list
            greetingList = new List<string>();

            string fileDir = "Data";
            string fileName = "quotes.txt";
            string projectRoot = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;
            string filePath = Path.Combine(projectRoot, fileDir, fileName);

            //Open a streamReader
            using StreamReader streamReader = new StreamReader(filePath);

            //Add each line to the greetinglist as long as streamReader hasn't reached the end of the stream i.e. the file
            while (!streamReader.EndOfStream)
            {
                player.greetingList.Add(streamReader.ReadLine());
            }
        }
    }//end of class Player
    class Program
{
   
    public static BitmapImage ShowImage()
    {
        string fileDir = "Data";
        string fileName = "hhg2.png";

        string projectRoot = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;
        string filePath = Path.Combine(projectRoot, fileDir, fileName);

        BitmapImage bitmapImage = new BitmapImage(new Uri(filePath));

        return bitmapImage;        
    }

    public static void ClearPlayerData(Player player)
    {
        player.FirstName = "";
        player.LastName = "";
        player.Age = 0;
    }

    public static string ReadInput(string firstName, string lastName, string age, Player player)
    {
        //ask the user for their firstname, lastname and age and add these values to their respective player properties
        
        player.FirstName = firstName;
        player.LastName = lastName;

        if(int.TryParse(age, out int result))
            player.Age = result;

        //Show output
        DateTime date = DateTime.Now;
        const string dateFormat = "dd MMMM, yyyy";
        const string timeFormat = "HH:mm:ss";
        const string dateMessage = "The date is:";
        const string timeMessage = "The time is:";
                        
        string temp = $"'Hello, {player.FirstName} {player.LastName} ({player.Age} years). Your quote is:'\n {player.greetingList[date.Second]}\n\n{dateMessage} {date.DayOfWeek} {date.ToString(dateFormat)}\n{timeMessage} {date.ToString(timeFormat)}\n\n";
        string temp2= $"'Quote used is located at position {player.greetingList.IndexOf(player.greetingList[date.Second])} in a list of {player.greetingList.Count} items.'";
        return temp+temp2;
        
    }//end of ReadInput
} //end of class Program
}
