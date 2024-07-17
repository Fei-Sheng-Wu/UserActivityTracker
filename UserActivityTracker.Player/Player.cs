using System;
using System.Windows;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Diagnostics;
using UserActivityTracker.FileFormat;

namespace UserActivityTracker
{
    public class Player
    {
        /// <summary>
        /// The <see cref="FrameworkElement"/> that is set to play the user actions.
        /// </summary>
        public FrameworkElement Element { get; }

        /// <summary>
        /// The multiple that is applied to the frame rate during the playing. The default value is 1.0.
        /// </summary>
        public double PlaybackSpeed { get; set; }

        /// <summary>
        /// Indicates whether the playing has started yet.
        /// </summary>
        public bool IsPlaying { get; internal set; }

        /// <summary>
        /// The current output of logs that have already been outputted.
        /// </summary>
        public string LogOutput { get; internal set; }

        /// <summary>
        /// Initialize a new <see cref="Player"/> on a specified <see cref="FrameworkElement"/>.
        /// </summary>
        /// <param name="element">The <see cref="FrameworkElement"/> that is set to play the user actions.</param>
        public Player(FrameworkElement element)
        {
            this.Element = element;
            this.PlaybackSpeed = 1.0;
            this.IsPlaying = false;
            this.LogOutput = "";
        }

        [DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        private static extern int SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, IntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, IntPtr dwExtraInfo);

        /// <summary>
        /// Play the recorded user actions on <see cref="Element"/> if the playing has not started yet.
        /// </summary>
        /// <param name="data">A <see langword="string"/> representation of all user actions on <see cref="Element"/>.</param>
        /// <param name="startingConfigHandler">An optional handler to use the customized starting configurations retrieved as a <see langword="string"/> if the value exists.</param>
        /// <returns><see langword="true"/> if the user actions were played successfully; otherwise, <see langword="false"/>.</returns>
        public async Task<bool> Play(string data, Action<string> startingConfigHandler = null)
        {
            if (this.IsPlaying || this.Element == null || string.IsNullOrWhiteSpace(data))
            {
                return false;
            }
            else
            {
                this.IsPlaying = true;
            }

            Structure session = Structure.Deserialize(data);

            this.Element.Focus();

            this.Element.Width = session.StartingWidth;
            this.Element.Height = session.StartingHeight;

            if (startingConfigHandler != null && !string.IsNullOrWhiteSpace(session.StartingConfig))
            {
                startingConfigHandler.Invoke(session.StartingConfig);
            }

            int timestamp = Environment.TickCount;
            foreach (UserAction userAction in UserAction.FromStringList(session.Actions))
            {
                try
                {
                    IntPtr windowPointer = Process.GetCurrentProcess().MainWindowHandle;
                    SetForegroundWindow(windowPointer);
                    SendMessage(windowPointer, 0x0112, 0xF120, 0);
                }
                catch { }

                switch (userAction.ActionType)
                {
                    case UserActionType.Unknown:
                        if (userAction.ActionParameters.Length >= 1)
                        {
                            UpdateLogOutput("ERROR", "Unknown User Action: " + userAction.ActionParameters[0].ToString());
                        }
                        break;
                    case UserActionType.Message:
                        if (userAction.ActionParameters.Length >= 1)
                        {
                            UpdateLogOutput("MESSAGE", userAction.ActionParameters[0].ToString());
                        }
                        break;
                    case UserActionType.Pause:
                        if (userAction.ActionParameters.Length >= 1
                            && int.TryParse(userAction.ActionParameters[0].ToString(), out int wTime))
                        {
                            await Pause(wTime, this.PlaybackSpeed);
                        }
                        break;
                    case UserActionType.MouseMove:
                        if (userAction.ActionParameters.Length >= 2
                            && double.TryParse(userAction.ActionParameters[0].ToString(), out double mX)
                            && double.TryParse(userAction.ActionParameters[1].ToString(), out double mY))
                        {
                            Point point = this.Element.PointToScreen(new Point(mX, mY));
                            SetCursorPos((int)point.X, (int)point.Y);
                        }
                        break;
                    case UserActionType.MouseDown:
                        if (userAction.ActionParameters.Length >= 3
                            && double.TryParse(userAction.ActionParameters[0].ToString(), out double pX)
                            && double.TryParse(userAction.ActionParameters[1].ToString(), out double pY)
                            && Enum.TryParse(userAction.ActionParameters[2].ToString(), out MouseButton pButton))
                        {
                            Point point = this.Element.PointToScreen(new Point(pX, pY));
                            SetCursorPos((int)point.X, (int)point.Y);
                            switch (pButton)
                            {
                                case MouseButton.Left:
                                    mouse_event(0x0002, 0, 0, 0, IntPtr.Zero);
                                    break;
                                case MouseButton.Middle:
                                    mouse_event(0x0020, 0, 0, 0, IntPtr.Zero);
                                    break;
                                case MouseButton.Right:
                                    mouse_event(0x0008, 0, 0, 0, IntPtr.Zero);
                                    break;
                                case MouseButton.XButton1:
                                    mouse_event(0x0080, 0, 0, 0x0001, IntPtr.Zero);
                                    break;
                                case MouseButton.XButton2:
                                    mouse_event(0x0080, 0, 0, 0x0002, IntPtr.Zero);
                                    break;
                            }
                        }
                        break;
                    case UserActionType.MouseUp:
                        if (userAction.ActionParameters.Length >= 3
                            && double.TryParse(userAction.ActionParameters[0].ToString(), out double rX)
                            && double.TryParse(userAction.ActionParameters[1].ToString(), out double rY)
                            && Enum.TryParse(userAction.ActionParameters[2].ToString(), out MouseButton rButton))
                        {
                            Point point = this.Element.PointToScreen(new Point(rX, rY));
                            SetCursorPos((int)point.X, (int)point.Y);
                            switch (rButton)
                            {
                                case MouseButton.Left:
                                    mouse_event(0x0004, 0, 0, 0, IntPtr.Zero);
                                    break;
                                case MouseButton.Middle:
                                    mouse_event(0x0040, 0, 0, 0, IntPtr.Zero);
                                    break;
                                case MouseButton.Right:
                                    mouse_event(0x0010, 0, 0, 0, IntPtr.Zero);
                                    break;
                                case MouseButton.XButton1:
                                    mouse_event(0x0100, 0, 0, 0x0001, IntPtr.Zero);
                                    break;
                                case MouseButton.XButton2:
                                    mouse_event(0x0100, 0, 0, 0x0002, IntPtr.Zero);
                                    break;
                            }
                        }
                        break;
                    case UserActionType.MouseWheel:
                        if (userAction.ActionParameters.Length >= 3
                            && double.TryParse(userAction.ActionParameters[0].ToString(), out double sX)
                            && double.TryParse(userAction.ActionParameters[1].ToString(), out double sY)
                            && int.TryParse(userAction.ActionParameters[2].ToString(), out int sDelta))
                        {
                            Point point = this.Element.PointToScreen(new Point(sX, sY));
                            SetCursorPos((int)point.X, (int)point.Y);
                            mouse_event(0x0800, 0, 0, sDelta, IntPtr.Zero);
                        }
                        break;
                    case UserActionType.KeyDown:
                        if (userAction.ActionParameters.Length >= 1
                             && int.TryParse(userAction.ActionParameters[0].ToString(), out int dKey))
                        {
                            keybd_event((byte)dKey, 0x45, 0x0001 | 0x0000, IntPtr.Zero);
                        }
                        break;
                    case UserActionType.KeyUp:
                        if (userAction.ActionParameters.Length >= 1
                            && int.TryParse(userAction.ActionParameters[0].ToString(), out int uKey))
                        {
                            keybd_event((byte)uKey, 0x45, 0x0001 | 0x0002, IntPtr.Zero);
                        }
                        break;
                }

                if (userAction.ActionType != UserActionType.Message)
                {
                    await Pause(1000 / session.FrameRate, this.PlaybackSpeed, Environment.TickCount - timestamp);
                }
                timestamp = Environment.TickCount;
            }

            return true;
        }

        private Task Pause(int milliseconds, double playbackSpeed, int actualTime = 0)
        {
            return Task.Delay(Math.Max(0, (int)(milliseconds / playbackSpeed) - actualTime));
        }

        private void UpdateLogOutput(string outputType, string newOutput)
        {
            this.LogOutput += $"[{outputType}] {newOutput}\n";
        }
    }
}
