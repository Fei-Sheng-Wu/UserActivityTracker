using System.Collections.Generic;

namespace UserActivityTracker.FileFormat
{
    public class Structure
    {
        public int FrameRate { get; set; }

        public double StartingWidth { get; set; }

        public double StartingHeight { get; set; }

        public List<string> Actions { get; set; }
    }
}