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
            if (this.ActionParameters == null)
            {
                this.ActionParameters = new object[] { };
            }

            return (char)this.ActionType + string.Join(",", this.ActionParameters);
        }

        public static UserAction FromString(string value)
        {
            UserAction action = new UserAction()
            {
                ActionType = UserActionType.Unknown
            };

            string valueTrimmed = value.Trim();
            if (valueTrimmed.Length > 0)
            {
                if (Enum.TryParse(((int)valueTrimmed[0]).ToString(), out UserActionType actionType))
                {
                    action.ActionType = actionType;
                }

                switch (action.ActionType)
                {
                    case UserActionType.Unknown:
                        action.ActionParameters = new object[] { valueTrimmed };
                        break;
                    case UserActionType.Message:
                        action.ActionParameters = new object[] { valueTrimmed.Substring(1).Trim('\'') };
                        break;
                    default:
                        action.ActionParameters = valueTrimmed.Substring(1).Split(',');
                        break;
                }
            }
            else
            {
                action.ActionParameters = new object[] { };
            }

            return action;
        }

        public static IEnumerable<UserAction> FromStringList(List<string> value)
        {
            if (value.Count != 1)
            {
                foreach (string action in value)
                {
                    yield return FromString(action);
                }
                yield break;
            }

            string stringList = value[0];
            string currentAction = "";
            for (int i = 0; i < stringList.Length; i++)
            {
                if (currentAction.Length == 0)
                {
                    if (!char.IsWhiteSpace(stringList[i]))
                    {
                        currentAction += stringList[i];
                    }
                    continue;
                }

                bool finished;
                switch (currentAction[0])
                {
                    case (char)UserActionType.Message:
                        finished = stringList[i] == '\'' && currentAction.Contains("\'");
                        break;
                    default:
                        finished = i == stringList.Length - 1 || char.IsLetter(stringList[i + 1]);
                        break;
                }

                currentAction += stringList[i];
                if (finished)
                {
                    yield return FromString(currentAction);
                    currentAction = "";
                }
            }
        }
    }

    public enum UserActionType
    {
        Unknown = 'x',
        Message = 'i', //Info
        Pause = 'w', //Wait
        Resize = 'c', //Change
        MouseMove = 'm', //Move
        MouseDown = 'p', //Press
        MouseUp = 'r', //Release
        MouseWheel = 's', //Scroll
        KeyDown = 'd', //Down
        KeyUp = 'u' //Up
    }
}
