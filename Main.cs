using System.IO;
using System.Windows.Media.Imaging;

namespace HHG_WPF_Fileversion;

class Program
{
    private class Player
    {
        //Since we aren't using any custom logic in get/set we'll use C#'s auto-implementation
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }//end of class Player

    //instantiate player object at Program class level 
    //so it's accessible from any method within Program class as long as the methods are static
    private static Player player = new Player();

    //create a new string list which also needs to be static since we are populating it inside the InitializeList method
    static List<string> greetingList = new List<string>();

    static public void ReadFromFile()
    {
        string fileDir = "Data";
        string fileName = "quotes.txt";
        string projectRoot = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;
        string filePath = Path.Combine(projectRoot, fileDir, fileName);

        //Open a streamReader
        using StreamReader streamReader = new StreamReader(filePath);

        //Add each line to the greetinglist as long as streamReader hasn't reached the end of the stream i.e. the file
        while (!streamReader.EndOfStream)
        {
            greetingList.Add(streamReader.ReadLine());
        }
    }

    public static bool IsNull(string? input)
    {
        //not really necessary to create a method just for IsNullOrEmpty but I wanted to test my own understanding
        //of using methods/functions with a return value (and parameter)
        if (string.IsNullOrEmpty(input))
            return true;
        else
            return false;
    }

    public static BitmapImage ShowImage()
    {
        string fileDir = "Data";
        string fileName = "hhg2.png";

        string projectRoot = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;
        string filePath = Path.Combine(projectRoot, fileDir, fileName);

        BitmapImage bitmapImage = new BitmapImage(new Uri(filePath));

        return bitmapImage;        
    }

    public static string ReadInput(string firstName, string lastName, string age)
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
        const int dontPanic = 42;
        
        if (player.Age == dontPanic)
        {
            return "";
        }   
        else
        {
            string temp = $"'Hello, {player.FirstName} {player.LastName} ({player.Age} years). Your quote is:\n\"{greetingList[date.Second]}\"\n\n{dateMessage} {date.DayOfWeek} {date.ToString(dateFormat)}\n{timeMessage} {date.ToString(timeFormat)}\n\n'";
            string temp2="Quote used is located at position {greetingList.IndexOf(greetingList[date.Second])} in a list of {greetingList.Count} items.";
            return temp+temp2;
        }

        //while (IsNull(result))
        //{
        //    Console.WriteLine("What is your last name?");
        //    result = Console.ReadLine();
        //}
        //player.LastName = result;
        //result = null;

        //while (!ageIsANumber)
        //{
        //    Console.WriteLine("What is your age?");
        //    result = Console.ReadLine();

        //    //check if string can be converted to number                
        //    if (int.TryParse(result, out int number))
        //    {
        //        player.Age = number;
        //        ageIsANumber = true;
        //    }
        //    else
        //        Console.WriteLine($"{result} is not a number.");
        //}
    }//end of ReadInput
} //end of class Program
