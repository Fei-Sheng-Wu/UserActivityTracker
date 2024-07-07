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

            recorder = new UserActivityTracker.Recorder(this);
        }

        private void RecordButton_Click(object sender, RoutedEventArgs e)
        {
            buttonRecord.IsEnabled = false;
            buttonPlay.IsEnabled = false;

            if (!recorder.IsRecording)
            {
                recorder.Start();
                buttonRecord.Content = "Save Session";
                buttonRecord.IsEnabled = true;
            }
            else
            {
                recorder.Stop();
                buttonRecord.Content = "Record Session";
                buttonRecord.IsEnabled = true;

                session = recorder.Save();
                buttonPlay.IsEnabled = true;
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            buttonPlay.IsEnabled = false;

            MainWindow window = new MainWindow();
            UserActivityTracker.Player player = new UserActivityTracker.Player(window);

            window.Show();
            window.ContentRendered += async (obj, args) =>
            {
                if (!player.IsPlaying)
                {
                    await player.Play(session);
                    await Task.Delay(500);
                    window.Close();
                }
            };

            buttonPlay.IsEnabled = true;
        }
    }
}
