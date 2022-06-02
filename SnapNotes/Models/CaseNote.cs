using System;

namespace SnapNotes.Models
{
    public class CaseNote
    {
        public int Id { get; set; }
        public string Consumer { get; set; }
        public string Documentation { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public override string ToString()
        {
            string displayTime, start, end;
            start = this.StartTime.ToString("MM / dd / yyyy hh: mm tt");
            if (this.StartTime.Day == this.EndTime.Day)
            {
                end = this.EndTime.ToString("hh: mm tt");
            }
            else { end = this.EndTime.ToString("MM / dd / yyyy hh: mm tt"); }
            displayTime = $"{start} - {end}";
            return $"{displayTime} : {this.Consumer} - {this.Documentation}";
        }
    }
}
