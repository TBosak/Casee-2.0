using System;

namespace SnapNotes.Models
{
    public class CaseNote
    {
        public string Consumer { get; set; }
        public string Documentation { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
    }
}
