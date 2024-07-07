namespace UserActivityTracker.FileFormat
{
    public class Structure
    {
        public int FrameRate { get; set; }

        public double StartingWidth { get; set; }

        public double StartingHeight { get; set; }

        public string Actions { get; set; }
    }
}