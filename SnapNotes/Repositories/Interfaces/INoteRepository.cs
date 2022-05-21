using SnapNotes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapNotes.Repositories.Interfaces
{
    public interface INoteRepository
    {
        IEnumerable<CaseNote> ReturnAll();
        IEnumerable<CaseNote> ReturnByDateTime(DateTimeOffset? startTime, DateTimeOffset? endTime);
        IEnumerable<CaseNote> ReturnOverlapping(IEnumerable<CaseNote> notes = null);
        IEnumerable<CaseNote> ReturnByTimeSpan(TimeSpan startTime, TimeSpan endTime, IEnumerable<CaseNote> filtered = null);
        Boolean SubmitNote(CaseNote caseNote);

    }
}
