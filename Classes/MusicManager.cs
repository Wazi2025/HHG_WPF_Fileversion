using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.IO;


namespace HHG_WPF_Fileversion
    {
    //we don't have have to change it's type to IDisposable but now we can use the "using" statement 
    public class MusicManager : IDisposable
        {
        //we'll use NAudio since MediaPlayer class has limititations (no native fade-in/fade-out)
        private IWavePlayer outputDevice = new WaveOutEvent();

        //this needs to be public since we adjust the song position in btnOK_Click
        public AudioFileReader AudioReader { get; set; }

        private FadeInOutSampleProvider fade;

        //MusicManager constructor
        public MusicManager(Player player)
            {
            //init audio stuff
            AudioReader = new AudioFileReader(GetSong(player));
            fade = new FadeInOutSampleProvider(AudioReader, true);

            //set fade-in to 2 seconds
            fade.BeginFadeIn(2000);

            //attach to an output device
            outputDevice = new WaveOutEvent();
            outputDevice.Init(fade);

            //start playing
            outputDevice.Play();
            }

        //Now we can keep most fields private and just call this method
        public void Dispose()
            {
            outputDevice.Stop();
            outputDevice.Dispose();
            AudioReader.Dispose();
            }

        public string GetSong(Player player)
            {
            player.fileName = "Journey of the Sorcerer.mp4";
            player.filePath = Path.Combine(player.projectRoot, player.fileDir, player.fileName);

            return player.filePath;
            }
        }//end of MusicManager class
    }
