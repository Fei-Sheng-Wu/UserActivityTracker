using System.Windows;
using System.Threading.Tasks;

namespace UserActivityTracker.Test
{
    public partial class MainWindow : Window
    {
        private string session;
        private UserActivityTracker.Recorder recorder;

        public MainWindow()
        {
            InitializeComponent();

            buttonRecord.IsEnabled = true;
            buttonPlay.IsEnabled = false;

            recorder = new UserActivityTracker.Recorder(this); //Set the element to be recorded to the window.

            //The ContentRendered event can be used to start the recording automatically.
            //this.ContentRendered += (obj, args) => //Start the recording when the window is ready.
            //{
            //    if (this.Tag != null && this.Tag.ToString() == "Play")
            //    {
            //        return;
            //    }
            //    buttonRecord.IsEnabled = false;
            //    buttonPlay.IsEnabled = false;

            //    if (!recorder.IsRecording) //Check whether the recording has been started yet.
            //    {
            //        recorder.Start(); //Start the recording. Returns true if the recording was started successfully.
            //        buttonRecord.Content = "Save Session";
            //        buttonRecord.IsEnabled = true;
            //    }
            //};

            //The Closing event can be used to stop the recording automatically.
            //this.Closing += (obj, args) => //Stop the recording when the window is being closed.
            //{
            //    if (this.Tag != null && this.Tag.ToString() == "Play")
            //    {
            //        return;
            //    }

            //    if (recorder.IsRecording) //Check whether the recording has been started yet.
            //    {
            //        recorder.Stop(); //Stop the recording. Returns true if the recording was stopped successfully.
            //        session = recorder.Save(); //Retrieve the string representation of the recording.

            //        MessageBox.Show(session, "Recording Data"); //Display the string representation of the recording.
            //    }
            //};
        }

        private void RecordButton_Click(object sender, RoutedEventArgs e)
        {
            buttonRecord.IsEnabled = false;
            buttonPlay.IsEnabled = false;

            if (!recorder.IsRecording) //Check whether the recording has been started yet.
            {
                recorder.Start(); //Start the recording. Returns true if the recording was started successfully.
                buttonRecord.Content = "Save Session";
                buttonRecord.IsEnabled = true;
            }
            else
            {
                recorder.Stop(); //Stop the recording. Returns true if the recording was stopped successfully.
                buttonRecord.Content = "Record Session";
                buttonRecord.IsEnabled = true;

                session = recorder.Save(); //Retrieve the string representation of the recording.
                buttonPlay.IsEnabled = true;
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            buttonPlay.IsEnabled = false;

            MainWindow window = new MainWindow() { Tag = "Play" }; //Create a new window for the user actions to be played.
            UserActivityTracker.Player player = new UserActivityTracker.Player(window); //Set the element to play the user actions to the new window.

            window.Show(); //Show the new window.
            window.ContentRendered += async (obj, args) => //Play the user actions when the new window is ready.
            {
                if (!player.IsPlaying) //Check whether the playing has been started yet.
                {
                    await player.Play(session); //Play the recorded user actions from the string representation.
                    await Task.Delay(500); //Pause for 500 milliseconds before closing the new window.
                    window.Close(); //Close the new window as the playing is done.

                    MessageBox.Show(session, "Recording Data"); //Display the string representation of the recording.
                }
            };

            buttonPlay.IsEnabled = true;
        }
    }
}
