using System;
using System.Collections.Generic;

namespace UserActivityTracker.FileFormat
{
    public class UserAction
    {
        public UserActionType ActionType { get; set; }

        public object[] ActionParameters { get; set; }

        public override string ToString()
        {
            return (char)ActionType + string.Join(",", ActionParameters);
        }

        public static UserAction FromString(string value)
        {
            UserAction action = new UserAction();

            string valueTrimmed = value.Trim();
            if (valueTrimmed.Length > 0)
            {
                action.ActionType = UserActionType.Unknown;
                if (Enum.TryParse(((int)valueTrimmed[0]).ToString(), out UserActionType actionType))
                {
                    action.ActionType = actionType;
                }

                if (action.ActionType == UserActionType.Unknown)
                {
                    action.ActionParameters = new object[] { valueTrimmed };
                }
                else if (action.ActionType == UserActionType.Message)
                {
                    action.ActionParameters = new object[] { valueTrimmed.Substring(1).Trim('\'') };
                }
                else
                {
                    action.ActionParameters = valueTrimmed.Substring(1).Split(',');
                }
            }

            return action;
        }

        public static IEnumerable<UserAction> FromStringList(string value)
        {
            string currentAction = "";

            foreach (char c in value)
            {
                if (currentAction.Length == 0)
                {
                    if (!char.IsWhiteSpace(c))
                    {
                        currentAction += c;
                    }
                    continue;
                }

                if (currentAction[0] == (char)UserActionType.Message)
                {
                    if (c == '\'' && currentAction.Contains("\'"))
                    {
                        yield return FromString(currentAction + c);
                        currentAction = "";
                        continue;
                    }
                }
                else
                {
                    if (char.IsLetter(c))
                    {
                        yield return FromString(currentAction);
                        currentAction = "";
                    }
                }

                currentAction += c;
            }
        }
    }

    public enum UserActionType
    {
        Unknown = 'x',
        Message = 'i', //Info
        Pause = 'w', //Wait
        MouseMove = 'm', //Move
        MouseDown = 'p', //Press
        MouseUp = 'r', //Release
        MouseWheel = 's', //Scroll
        KeyDown = 'd', //Down
        KeyUp = 'u' //Up
    }
}
