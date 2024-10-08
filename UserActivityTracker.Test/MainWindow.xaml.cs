﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Diagnostics;

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

            textRandom.Text = $"This is a random number: {new Random().Next()}"; //Generate a random content of the TextBlock to demonstrate the customizable starting configuration.

            recorder = new UserActivityTracker.Recorder(this); //Set the element to be recorded to the window.

            //The ContentRendered event can be used to start the recording automatically.
            //this.ContentRendered += (obj, args) => //Start the recording when the window is ready.
            //{
            //    if (this.Tag != null && this.Tag.ToString() == "Play")
            //    {
            //        return;
            //    }
            //
            //    if (!recorder.IsRecording) //Check whether the recording has started yet.
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
            //
            //    if (recorder.IsRecording) //Check whether the recording has started yet.
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

            if (!recorder.IsRecording) //Check whether the recording has started yet.
            {
                string startingConfig = textRandom.Text; //Use the content of the TextBlock as the starting configuration. This configuration cannot include the character ";" in it.
                recorder.Start(startingConfig); //Start the recording with a customized configuration that can be used upon playing. Returns true if the recording was started successfully.
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
            player.LogOutputUpdated += Player_LogOutputUpdated; //Use the LogOutputUpdated event to receive real-time updates on the log output.

            window.Show(); //Show the new window.
            window.ContentRendered += async (obj, args) => //Play the user actions when the new window is ready.
            {
                if (window.FindName("buttonRecord") is Button button)
                {
                    button.IsEnabled = false; //Disable the record button during the playing.
                }

                if (!player.IsPlaying) //Check whether the playing has started yet.
                {
                    await player.Play(session, (startingConfig) => //Play the recorded user actions from the string representation along with a callback that retrieves the saved starting configuration.
                    {
                        if (window.FindName("textRandom") is TextBlock textBlock)
                        {
                            textBlock.Text = startingConfig; //Use the retrieved configuration on the TextBlock.
                        }
                    });
                    await Task.Delay(500); //Pause for 500 milliseconds before closing the new window.
                    window.Close(); //Close the new window as the playing is done.

                    MessageBox.Show(session, "Recording Data"); //Display the string representation of the recording.
                    if (!string.IsNullOrWhiteSpace(player.LogOutput))
                    {
                        MessageBox.Show(player.LogOutput, "Log Output"); //Display all outputted logs of the recording.
                    }
                }
            };

            buttonPlay.IsEnabled = true;
        }

        private void Player_LogOutputUpdated(object sender, LogOutputEventArgs e)
        {
            Debug.Write(e.Update, "UserActivityTracker.Player"); //Write the received update to the debug window.
        }

        private void SampleButton_Click(object sender, RoutedEventArgs e)
        {
            if (recorder.IsRecording) //Check whether the recording has started yet.
            {
                recorder.LogMessage("Sample Button Clicked!"); //Log a custom string message into the recording.
            }
        }
    }
}
