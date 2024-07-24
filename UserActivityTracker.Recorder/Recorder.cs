using System;
using System.Windows;
using System.Windows.Input;
using UserActivityTracker.FileFormat;

namespace UserActivityTracker
{
    /// <summary>
    /// Provides the ability to record a series of user actions on a specified <see cref="FrameworkElement"/>.
    /// </summary>
    public class Recorder
    {
        /// <summary>
        /// The <see cref="FrameworkElement"/> that is set to be recorded.
        /// </summary>
        public FrameworkElement Element { get; }

        /// <summary>
        /// The number of basic user actions that are recorded per second, including moving the mouse. The default value is 30.
        /// </summary>
        public int FrameRate { get; set; }

        /// <summary>
        /// Indicates whether to record mouse actions from the user. The default value is <see langword="true"/>.
        /// </summary>
        public bool RecordMouseActions { get; set; }

        /// <summary>
        /// Indicates whether to record keyboard actions from the user. The default value is <see langword="true"/>.
        /// </summary>
        public bool RecordKeyboardActions { get; set; }

        /// <summary>
        /// Indicates whether the recording has started yet.
        /// </summary>
        public bool IsRecording { get; internal set; }

        private Structure session;
        private int lastActionTime;

        /// <summary>
        /// Initialize a new instance of the <see cref="Recorder"/> class on a specified <see cref="FrameworkElement"/>.
        /// </summary>
        /// <param name="element">The <see cref="FrameworkElement"/> that is set to be recorded.</param>
        public Recorder(FrameworkElement element)
        {
            this.Element = element;
            this.FrameRate = 30;
            this.RecordMouseActions = true;
            this.RecordKeyboardActions = true;
            this.IsRecording = false;
        }

        /// <summary>
        /// Start to record user actions on <see cref="Element"/> if the recording has not started yet.
        /// </summary>
        /// <param name="startingConfig">An optional <see langword="string"/> value to store customized starting configurations. Cannot include the character ";" in it.</param>
        /// <returns><see langword="true"/> if the recording was started successfully; otherwise, <see langword="false"/>.</returns>
        public bool Start(string startingConfig = "")
        {
            if (this.IsRecording || this.Element == null || startingConfig.Contains(";"))
            {
                return false;
            }
            else
            {
                this.IsRecording = true;
            }

            this.Element.Focus();

            lastActionTime = Environment.TickCount;
            session = new Structure
            {
                FrameRate = this.FrameRate,
                StartingWidth = this.Element.ActualWidth,
                StartingHeight = this.Element.ActualHeight,
                StartingConfig = startingConfig,
                Actions = ""
            };

            this.Element.SizeChanged += AddSizeChanged;
            this.Element.PreviewMouseMove += AddMouseMove;
            this.Element.PreviewMouseDown += AddMouseDown;
            this.Element.PreviewMouseUp += AddMouseUp;
            this.Element.PreviewMouseWheel += AddMouseWheel;
            this.Element.PreviewKeyDown += AddKeyDown;
            this.Element.PreviewKeyUp += AddKeyUp;

            return true;
        }

        /// <summary>
        /// Stop the current recording on <see cref="Element"/> if the recording is ongoing.
        /// </summary>
        /// <returns><see langword="true"/> if the recording was stopped successfully; otherwise, <see langword="false"/>.</returns>
        public bool Stop()
        {
            if (!this.IsRecording || this.Element == null)
            {
                return false;
            }
            else
            {
                this.IsRecording = false;
            }

            this.Element.SizeChanged -= AddSizeChanged;
            this.Element.PreviewMouseMove -= AddMouseMove;
            this.Element.PreviewMouseDown -= AddMouseDown;
            this.Element.PreviewMouseUp -= AddMouseUp;
            this.Element.PreviewMouseWheel -= AddMouseWheel;
            this.Element.PreviewKeyDown -= AddKeyDown;
            this.Element.PreviewKeyUp -= AddKeyUp;

            return true;
        }

        /// <summary>
        /// Save the current recording on <see cref="Element"/> if the recording exists.
        /// </summary>
        /// <returns>A <see langword="string"/> representation of all user actions on <see cref="Element"/> during the recording.</returns>
        public string Save()
        {
            if (session == null)
            {
                return "";
            }

            return Structure.Serialize(session);
        }

        /// <summary>
        /// Add a custom <see langword="string"/> message to the recording that can be outputted to the logs.
        /// </summary>
        /// <param name="message">A custom <see langword="string"/> that will be added to the recording. Cannot include the characters ";" and "'" in it.</param>
        /// <returns><see langword="true"/> if the message was added successfully; otherwise, <see langword="false"/>.</returns>
        public bool LogMessage(string message)
        {
            if (!this.IsRecording || message.Contains(";") || message.Contains("'"))
            {
                return false;
            }

            session.Actions += new UserAction()
            {
                ActionType = UserActionType.Message,
                ActionParameters = new object[] { $"\'{message}\'" }
            }.ToString();

            return true;
        }

        private int CalculateTimePassed()
        {
            int timestamp = Environment.TickCount;

            if (timestamp >= lastActionTime)
            {
                return timestamp - lastActionTime;
            }
            else
            {
                return timestamp - int.MinValue + int.MaxValue - lastActionTime;
            }
        }

        private void AddPossiblePause()
        {
            int timestamp = Environment.TickCount;

            int extra = CalculateTimePassed() - 1000 / this.FrameRate;
            if (extra > 0)
            {
                session.Actions += new UserAction()
                {
                    ActionType = UserActionType.Pause,
                    ActionParameters = new object[] { extra }
                }.ToString();
            }

            lastActionTime = timestamp;
        }

        private void AddSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!this.IsRecording)
            {
                return;
            }

            AddPossiblePause();

            session.Actions += new UserAction()
            {
                ActionType = UserActionType.Resize,
                ActionParameters = new object[] { this.Element.ActualWidth, this.Element.ActualHeight }
            }.ToString();
        }

        private void AddMouseMove(object sender, MouseEventArgs e)
        {
            if (!this.IsRecording || !this.RecordMouseActions || CalculateTimePassed() < 1000 / this.FrameRate)
            {
                return;
            }

            AddPossiblePause();

            Point position = e.GetPosition(this.Element);
            session.Actions += new UserAction()
            {
                ActionType = UserActionType.MouseMove,
                ActionParameters = new object[] { position.X, position.Y }
            }.ToString();
        }

        private void AddMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.IsRecording || !this.RecordMouseActions)
            {
                return;
            }

            AddPossiblePause();

            Point position = e.GetPosition(this.Element);
            session.Actions += new UserAction()
            {
                ActionType = UserActionType.MouseDown,
                ActionParameters = new object[] { position.X, position.Y, (int)e.ChangedButton }
            }.ToString();
        }

        private void AddMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!this.IsRecording || !this.RecordMouseActions)
            {
                return;
            }

            AddPossiblePause();

            Point position = e.GetPosition(this.Element);
            session.Actions += new UserAction()
            {
                ActionType = UserActionType.MouseUp,
                ActionParameters = new object[] { position.X, position.Y, (int)e.ChangedButton }
            }.ToString();
        }

        private void AddMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!this.IsRecording || !this.RecordMouseActions)
            {
                return;
            }

            AddPossiblePause();

            Point position = e.GetPosition(this.Element);
            session.Actions += new UserAction()
            {
                ActionType = UserActionType.MouseWheel,
                ActionParameters = new object[] { position.X, position.Y, e.Delta }
            }.ToString();
        }

        private void AddKeyDown(object sender, KeyEventArgs e)
        {
            if (!this.IsRecording || !this.RecordKeyboardActions)
            {
                return;
            }

            AddPossiblePause();

            session.Actions += new UserAction()
            {
                ActionType = UserActionType.KeyDown,
                ActionParameters = new object[] { KeyInterop.VirtualKeyFromKey(e.Key) }
            }.ToString();
        }

        private void AddKeyUp(object sender, KeyEventArgs e)
        {
            if (!this.IsRecording || !this.RecordKeyboardActions)
            {
                return;
            }

            AddPossiblePause();

            session.Actions += new UserAction()
            {
                ActionType = UserActionType.KeyUp,
                ActionParameters = new object[] { KeyInterop.VirtualKeyFromKey(e.Key) }
            }.ToString();
        }
    }
}
