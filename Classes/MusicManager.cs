using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.IO;


namespace HHG_WPF_Fileversion
    {
    public class MusicManager
        {
        //we'll use NAudio since MediaPlayer class has limititations
        private IWavePlayer outputDevice;

        //this needs to be public since we adjust the song position in btnOK_Click
        public AudioFileReader audioFileReader;

        private FadeInOutSampleProvider fade;

        public void Dispose()
            {
            outputDevice.Stop();
            outputDevice.Dispose();
            audioFileReader.Dispose();
            }

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
    }
