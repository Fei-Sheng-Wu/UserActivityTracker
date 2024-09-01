using System;
using System.Drawing;
using UserActivityTracker.FileFormat;

namespace UserActivityTracker
{
    /// <summary>
    /// Provides the ability to analyze a series of user actions.
    /// </summary>
    public class Analysis
    {
        protected Analysis()
        {
            return;
        }

        /// <summary>
        /// Creates an instance of <see cref="Bitmap"/> that graphs the mouse actions within a a series of user actions.
        /// </summary>
        /// <param name="data">A <see langword="string"/> representation of all user actions on <see cref="Element"/>.</param>
        /// <param name="userInterface">An underlying <see cref="Bitmap"/> image to be placed beneath the graphed mouse actions.</param>
        /// <returns>The created <see cref="Bitmap"/> if the user actions were analyzed successfully; otherwise, <see langword="null"/>.</returns>
        public static Bitmap TrackMouseMovements(string data, Bitmap userInterface = null)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                return null;
            }

            Structure session = Structure.Deserialize(data);

            int maxWidth = (int)session.StartingWidth;
            int maxHeight = (int)session.StartingHeight;
            foreach (UserAction userAction in UserAction.FromStringList(session.Actions))
            {
                if (userAction.ActionType == UserActionType.Resize && userAction.ActionParameters.Length >= 2
                    && double.TryParse(userAction.ActionParameters[0].ToString(), out double width)
                    && double.TryParse(userAction.ActionParameters[1].ToString(), out double height))
                {
                    maxWidth = Math.Max(maxWidth, (int)width);
                    maxHeight = Math.Max(maxHeight, (int)height);
                }
            }

            Bitmap result = new Bitmap(maxWidth, maxHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (Graphics graphics = Graphics.FromImage(result))
            {
                if (userInterface != null)
                {
                    graphics.DrawImage(userInterface, 0, 0);
                }

                graphics.DrawRectangle(new Pen(Color.Purple, 2), 1, 1, maxWidth - 2, maxHeight - 2);
                graphics.DrawRectangle(new Pen(Color.Green, 2), 1, 1, (float)session.StartingWidth - 2, (float)session.StartingHeight - 2);

                float lastX = float.NaN;
                float lastY = float.NaN;
                foreach (UserAction userAction in UserAction.FromStringList(session.Actions))
                {
                    if ((userAction.ActionType == UserActionType.MouseMove
                        || userAction.ActionType == UserActionType.MouseDown
                        || userAction.ActionType == UserActionType.MouseUp)
                        && userAction.ActionParameters.Length >= 2
                        && float.TryParse(userAction.ActionParameters[0].ToString(), out float x)
                        && float.TryParse(userAction.ActionParameters[1].ToString(), out float y))
                    {
                        if (userAction.ActionType == UserActionType.MouseMove)
                        {
                            if (!float.IsNaN(lastX) && !float.IsNaN(lastY))
                            {
                                graphics.DrawLine(new Pen(Color.Yellow, 1), lastX, lastY, x, y);
                            }
                            lastX = x;
                            lastY = y;
                        }
                        else
                        {
                            graphics.DrawEllipse(new Pen(Color.Red, 2), x - 1, y - 1, 2, 2);
                            lastX = float.NaN;
                            lastY = float.NaN;
                        }
                    }
                }
            }

            return result;
        }
    }
}