using System.Collections.Generic;

namespace UserActivityTracker.FileFormat
{
    public class UserAction
    {
        public char ActionType { get; set; }

        public object[] ActionParameters { get; set; }

        public override string ToString()
        {
            return ActionType.ToString() + string.Join(",", ActionParameters);
        }

        public static UserAction FromString(string value)
        {
            UserAction action = new UserAction();

            string valueTrimmed = value.Trim();
            if (valueTrimmed.Length > 0)
            {
                action.ActionType = valueTrimmed[0];
                action.ActionParameters = valueTrimmed.Substring(1).Split(',');
            }

            return action;
        }

        public static IEnumerable<UserAction> FromStringList(string value)
        {
            string currentAction = "";

            foreach (char c in value)
            {
                if (char.IsLetter(c) && currentAction.Length > 0)
                {
                    yield return FromString(currentAction);
                    currentAction = "";
                }

                currentAction += c;
            }
        }
    }
}
