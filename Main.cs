using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;


namespace HHG_WPF_Fileversion
    {
    public class MusicManager
        {
        //we'll use NAudio since MediaPlayer class has limititations
        public IWavePlayer outputDevice;
        public AudioFileReader audioFileReader;
        public FadeInOutSampleProvider fade;

        public void InitMusicStuff(Player player)
            {
            //init audio stuff
            audioFileReader = new AudioFileReader(GetSong(player));
            fade = new FadeInOutSampleProvider(audioFileReader, true);

            //set fade-in to 2 seconds
            fade.BeginFadeIn(2000);

            //attach to an output device
            outputDevice = new WaveOutEvent();
            outputDevice.Init(fade);

            //start playing
            outputDevice.Play();
            }

        public string GetSong(Player player)
            {
            player.fileName = "Journey of the Sorcerer.mp4";
            player.filePath = Path.Combine(player.projectRoot, player.fileDir, player.fileName);

            return player.filePath;
            }
        }//end of MusicManager class

    public class GfxManager
        {
        //declare and initialize a BitmapImage for use with...well, bitmaps AKA images
        private BitmapImage bitmapImage = new BitmapImage();

        //declare and initialize animation stuff
        private ScaleTransform zoomTransform = new ScaleTransform(1, 1);
        private RotateTransform rotateTransform = new RotateTransform(0);
        private TransformGroup transformGroup = new TransformGroup();

        public void InitScreenStuff(Player player, GfxManager gfxManager, Window mainWindow)
            {
            //create new brush and set window's background image
            ImageBrush brush = new ImageBrush();
            brush.Opacity = 0.25;
            brush.ImageSource = gfxManager.ShowImage(player);
            mainWindow.Background = brush;

            mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            mainWindow.WindowState = WindowState.Maximized;
            }

        public void FadeInImage(double maxOpacity, Image image)
            {
            DoubleAnimation fadeIn = new DoubleAnimation(0, maxOpacity, TimeSpan.FromSeconds(5));

            fadeIn.AutoReverse = true;
            fadeIn.RepeatBehavior = RepeatBehavior.Forever;
            image.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            }

        public void ZoomIn(bool missingInfo, Player player)
            {
            //the spinning animation will activate even though it's never called
            //when the spin animation starts it effectively carries over due to the use of TransFormGroup
            //the solution is to set it to null (remove it)
            rotateTransform.BeginAnimation(RotateTransform.AngleProperty, null);

            DoubleAnimation zoomIn = new DoubleAnimation();

            zoomIn.From = 0.0;
            zoomIn.To = 3.5;
            zoomIn.Duration = TimeSpan.FromSeconds(2);
            zoomIn.AutoReverse = true;
            zoomIn.RepeatBehavior = RepeatBehavior.Forever;

            //only bounce if only 42 
            if (missingInfo && player.Age == player.DontPanic)
                {
                BounceEase bounceEase = new BounceEase();
                bounceEase.Bounces = 1;
                bounceEase.Bounciness = 4;
                bounceEase.EasingMode = EasingMode.EaseOut;

                zoomIn.EasingFunction = bounceEase;
                }

            //apply the animation (and bounce if applicable) to zoomTransform
            zoomTransform.BeginAnimation(ScaleTransform.ScaleXProperty, zoomIn);
            zoomTransform.BeginAnimation(ScaleTransform.ScaleYProperty, zoomIn);
            }

        public void StartImageSpin()
            {
            DoubleAnimation rotateAnimation = new DoubleAnimation();

            rotateAnimation.From = 0;
            rotateAnimation.To = 360;
            rotateAnimation.Duration = TimeSpan.FromSeconds(10);
            rotateAnimation.RepeatBehavior = RepeatBehavior.Forever;

            //apply the animation (spin) to rotateTransform
            rotateTransform.BeginAnimation(RotateTransform.AngleProperty, rotateAnimation);
            }

        public void InitImageControl(Image image)
            {
            //only one animation can be active so we have to add both animations to a TransformGroup
            //and then add the TransformGroup to the image RenderTransform
            image.RenderTransformOrigin = new Point(0.5, 0.5);
            transformGroup.Children.Add(zoomTransform);
            transformGroup.Children.Add(rotateTransform);
            image.RenderTransform = transformGroup;
            }

        public BitmapImage ShowImage(Player player)
            {
            player.fileName = "Andromeda-Galaxy-Milky-Way.jpg";
            player.filePath = Path.Combine(player.projectRoot, player.fileDir, player.fileName);

            bitmapImage = new BitmapImage(new Uri(player.filePath));

            return bitmapImage;
            }

        public BitmapImage ShowImage(bool missingInfo, Player player)
            {
            if (missingInfo)
                player.fileName = "hhg2.png";
            else
                player.fileName = "DontPanic.jpg";

            player.filePath = Path.Combine(player.projectRoot, player.fileDir, player.fileName);

            bitmapImage = new BitmapImage(new Uri(player.filePath));

            return bitmapImage;
            }

        public void RandomizeButton(Canvas canvas, Button btnOK, Random random)
            {
            //randomize button placement or technically the Canvas
            //note: SetLeft is a static method, hence we can only access it with it's type
            Canvas.SetLeft(btnOK, random.Next((int)canvas.ActualWidth - 100));
            Canvas.SetTop(btnOK, random.Next((int)canvas.ActualHeight - 100));

            //note: Canvas.ZIndex="1" in MainWindow.xaml makes sure the button is "on top" so it's reachable with mouse too
            //since no other elements use ZIndex we can just use 1
            }
        }//end of GfxManager

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
