using System;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using UserActivityTracker.FileFormat;

namespace UserActivityTracker
{
    public class Recorder
    {
        /// <summary>
        /// The <see cref="FrameworkElement"/> that is set to be recorded.
        /// </summary>
        public FrameworkElement Element { get; }

        /// <summary>
        /// The number of basic user actions that are recorded per second, including moving the mouse.
        /// </summary>
        public int FrameRate { get; set; }

        /// <summary>
        /// Indicates whether the recording has started.
        /// </summary>
        public bool IsRecording { get; internal set; }

        private int lastActionTime = 0;
        private Structure session = null;

        /// <summary>
        /// Initialize a new <see cref="Recorder"/> on a specified <see cref="FrameworkElement"/>.
        /// </summary>
        /// <param name="element">The <see cref="FrameworkElement"/> that is set to be recorded.</param>
        public Recorder(FrameworkElement element)
        {
            this.Element = element;
            this.FrameRate = 30;
            this.IsRecording = false;
        }

        /// <summary>
        /// Start to record user actions on <see cref="Element"/> if the recording has not started yet.
        /// </summary>
        /// <returns><see langword="true"/> if the recording was started successfully; otherwise, <see langword="false"/>.</returns>
        public bool Start()
        {
            if (this.IsRecording || this.Element == null)
            {
                return false;
            }
            else
            {
                this.IsRecording = true;
            }

            this.Element.Focus();

            session = new Structure
            {
                FrameRate = this.FrameRate,
                StartingWidth = this.Element.ActualWidth,
                StartingHeight = this.Element.ActualHeight,
                Actions = new List<string>()
            };

            lastActionTime = Environment.TickCount;

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
        /// <param name="compress"><see langword="true"/> to compress the recorded data; otherwise, <see langword="false"/>.</param>
        /// <returns>A <see langword="string"/> representation of all user actions on <see cref="Element"/> during the recording.</returns>
        public string Save(bool compress = true)
        {
            if (session == null)
            {
                return "";
            }

            string jsonString = JsonSerializer.Serialize(session);
            if (compress)
            {
                jsonString = Convert.ToBase64String(Encoding.ASCII.GetBytes(jsonString));
            }

            return (compress ? "1," : "0,") + jsonString;
        }

        private int CalculateTimePassed(int timestamp)
        {
            if (timestamp >= lastActionTime)
            {
                return timestamp - lastActionTime;
            }
            else
            {
                return timestamp - int.MinValue + lastActionTime - int.MaxValue;
            }
        }

        private void AddPossiblePause(int timestamp)
        {
            int extra = CalculateTimePassed(timestamp) - 1000 / this.FrameRate;
            if (extra > 0)
            {
                this.session.Actions.Add(new UserAction()
                {
                    ActionType = 'w', //Wait
                    ActionParameters = new object[] { extra }
                }.ToString());
            }

            lastActionTime = timestamp;
        }

        private void AddMouseMove(object sender, MouseEventArgs e)
        {
            if (CalculateTimePassed(e.Timestamp) < 1000 / this.FrameRate)
            {
                return;
            }

            AddPossiblePause(e.Timestamp);

            Point position = e.GetPosition(this.Element);
            session.Actions.Add(new UserAction()
            {
                ActionType = 'm', //Move
                ActionParameters = new object[] { position.X, position.Y }
            }.ToString());
        }

        private void AddMouseDown(object sender, MouseButtonEventArgs e)
        {
            AddPossiblePause(e.Timestamp);

            Point position = e.GetPosition(this.Element);
            session.Actions.Add(new UserAction()
            {
                ActionType = 'p', //Press
                ActionParameters = new object[] { position.X, position.Y, (int)e.ChangedButton }
            }.ToString());
        }

        private void AddMouseUp(object sender, MouseButtonEventArgs e)
        {
            AddPossiblePause(e.Timestamp);

            Point position = e.GetPosition(this.Element);
            session.Actions.Add(new UserAction()
            {
                ActionType = 'r', //Release
                ActionParameters = new object[] { position.X, position.Y, (int)e.ChangedButton }
            }.ToString());
        }

        private void AddMouseWheel(object sender, MouseWheelEventArgs e)
        {
            AddPossiblePause(e.Timestamp);

            Point position = e.GetPosition(this.Element);
            session.Actions.Add(new UserAction()
            {
                ActionType = 's', //Scroll
                ActionParameters = new object[] { position.X, position.Y, e.Delta }
            }.ToString());
        }

        private void AddKeyDown(object sender, KeyEventArgs e)
        {
            AddPossiblePause(e.Timestamp);

            session.Actions.Add(new UserAction()
            {
                ActionType = 'd', //Down
                ActionParameters = new object[] { KeyInterop.VirtualKeyFromKey(e.Key) }
            }.ToString());
        }

        private void AddKeyUp(object sender, KeyEventArgs e)
        {
            AddPossiblePause(e.Timestamp);

            session.Actions.Add(new UserAction()
            {
                ActionType = 'u', //Up
                ActionParameters = new object[] { KeyInterop.VirtualKeyFromKey(e.Key) }
            }.ToString());
        }
    }
}
