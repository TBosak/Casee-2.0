using SnapNotes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapNotes.Repositories.Interfaces
{
    internal interface INoteRepository
    {
        IEnumerable<CaseNote> ReturnAll();
        IEnumerable<CaseNote> ReturnByDateTime(DateTimeOffset? startTime, DateTimeOffset? endTime);
        IEnumerable<CaseNote> ReturnOverlapping(IEnumerable<CaseNote> notes = null);
    }
}
