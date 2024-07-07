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

            if (value.Length > 0)
            {
                action.ActionType = value[0];
                action.ActionParameters = value.Substring(1).Split(',');
            }

            return action;
        }
    }
}
