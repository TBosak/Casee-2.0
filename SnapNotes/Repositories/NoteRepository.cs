using LiteDB;
using SnapNotes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapNotes.Repositories
{
    public class NoteRepository
    {
        ILiteCollection<CaseNote> casenotes;

        public NoteRepository(ILiteCollection<CaseNote> casenotes)
        {
            this.casenotes = casenotes as LiteCollection<CaseNote>;
        }

        public IEnumerable<CaseNote> ReturnAll()
        {
            return casenotes.FindAll();
        }

        private IEnumerable<CaseNote> ReturnByDateTime(DateTime? startTime, DateTime? endTime)
        {
            IEnumerable<CaseNote> notes;
            if(startTime == null) { startTime = new DateTime(01 / 01 / 0001); }
            if(endTime == null) { endTime = DateTime.Now; }
            notes = casenotes.Find(x => x.StartTime < endTime && startTime < x.EndTime);
            return notes;
        }

        private IEnumerable<CaseNote> ReturnOverlapping(IEnumerable<CaseNote> notes = null)
        {
            var overlapping = new List<CaseNote>();
            if (notes == null) { notes = ReturnAll(); }
            overlapping = (from first in notes
                           from second in notes
                           where first.StartTime < second.EndTime
                           && second.StartTime < first.EndTime
                           && first != second
                           select first).ToList();
            return overlapping;
        }

        //builds a Datetime reflective of user-selected date + time
        public DateTime CombineDateTime(DateTime dateObj, DateTime timeObj)
        {
            return dateObj - dateObj.TimeOfDay + timeObj.TimeOfDay;
        }
    }
}
