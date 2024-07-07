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

        public static UserAction FromSingleString(string value)
        {
            UserAction action = new UserAction();

            if (value.Length > 0)
            {
                action.ActionType = value[0];
                action.ActionParameters = value.Substring(1).Split(',');
            }

            return action;
        }

        public static List<UserAction> FromListString(string value)
        {
            List<UserAction> actions = new List<UserAction>();

            string currentAction = "";
            foreach (char c in value)
            {
                if (char.IsLetter(c) && currentAction.Length > 0)
                {
                    actions.Add(FromSingleString(currentAction));
                    currentAction = "";
                }

                currentAction += c;
            }

            return actions;
        }
    }
}
