using SnapNotes.Models;
using System;
using System.Collections.Generic;

namespace SnapNotes.Repositories.Interfaces
{
    #nullable enable
    public interface INoteRepository
    {
        IEnumerable<CaseNote> ReturnAll();
        IEnumerable<CaseNote> ReturnByDateTime(DateTimeOffset? startTime, DateTimeOffset? endTime);
        IEnumerable<CaseNote> ReturnOverlapping(IEnumerable<CaseNote> notes = null);
        IEnumerable<CaseNote> ReturnByTimeSpan(TimeSpan startTime, TimeSpan endTime, IEnumerable<CaseNote> filtered = null);
        IEnumerable<CaseNote> QueryByConsumer(string query, IEnumerable<CaseNote>? filtered);
        IEnumerable<CaseNote> QueryByDocumentation(string query, IEnumerable<CaseNote>? filtered);
        Boolean SubmitNote(CaseNote caseNote);

    }
    #nullable disable
}
