using System;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Collections.Generic;
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
        /// Indicates whether the playing has started.
        /// </summary>
        public bool IsPlaying { get; internal set; }

        /// <summary>
        /// Initialize a new <see cref="Player"/> on a specified <see cref="FrameworkElement"/>.
        /// </summary>
        /// <param name="element">The <see cref="FrameworkElement"/> that is set to play the user actions.</param>
        public Player(FrameworkElement element)
        {
            this.Element = element;
            this.IsPlaying = false;
        }

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

            Structure session;
            try
            {
                session = JsonSerializer.Deserialize<Structure>(data);
            }
            catch (JsonException)
            {
                return false;
            }

            this.Element.Focus();

            this.Element.Width = session.StartingWidth;
            this.Element.Height = session.StartingHeight;

            if (startingConfigHandler != null && !string.IsNullOrWhiteSpace(session.StartingConfig))
            {
                startingConfigHandler.Invoke(session.StartingConfig);
            }

            List<UserAction> actions = UserAction.FromListString(session.Actions);

            foreach (UserAction userAction in actions)
            {
                switch (userAction.ActionType)
                {
                    case 'w': //Wait
                        if (userAction.ActionParameters.Length >= 1
                            && int.TryParse(userAction.ActionParameters[0].ToString(), out int wTime))
                        {
                            await Task.Delay(wTime);
                        }
                        break;
                    case 'm': //Move
                        if (userAction.ActionParameters.Length >= 2
                            && double.TryParse(userAction.ActionParameters[0].ToString(), out double mX)
                            && double.TryParse(userAction.ActionParameters[1].ToString(), out double mY))
                        {
                            Point point = this.Element.PointToScreen(new Point(mX, mY));
                            SetCursorPos((int)point.X, (int)point.Y);
                        }
                        break;
                    case 'p': //Press
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
                            }
                        }
                        break;
                    case 'r': //Release
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
                            }
                        }
                        break;
                    case 's': //Scroll
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
                    case 'd': //Down
                        if (userAction.ActionParameters.Length >= 1
                            && int.TryParse(userAction.ActionParameters[0].ToString(), out int dKey))
                        {
                            keybd_event((byte)dKey, 0x45, 0x0001 | 0x0000, IntPtr.Zero);
                        }
                        break;
                    case 'u': //Up
                        if (userAction.ActionParameters.Length >= 1
                            && int.TryParse(userAction.ActionParameters[0].ToString(), out int uKey))
                        {
                            keybd_event((byte)uKey, 0x45, 0x0001 | 0x0002, IntPtr.Zero);
                        }
                        break;
                }

                await Task.Delay(1000 / session.FrameRate);
            }

            return true;
        }
    }
}
