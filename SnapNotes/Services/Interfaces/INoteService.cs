using SnapNotes.Models;
using System;
using System.Collections.Generic;

namespace SnapNotes.Services.Interfaces
{
    public interface INoteService
    {
        string CaseNotesToCSV(IEnumerable<CaseNote> casenotes = null);
        IEnumerable<CaseNote> FilterByDate(DateTimeOffset start, DateTimeOffset end);
        IEnumerable<CaseNote> FilterByTime(TimeSpan start, TimeSpan end);
        IEnumerable<CaseNote> FilterByOverlapping(IEnumerable<CaseNote> caseNotes = null);
        IEnumerable<CaseNote> CaseNotes();
        Boolean SubmitNote(CaseNote caseNote);
    }
}
