using LiteDB;
using MoreLinq;
using SnapNotes.Models;
using SnapNotes.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
#nullable enable
namespace SnapNotes.Repositories
{
    public class NoteRepository : INoteRepository
    {
        ILiteCollection<CaseNote> casenotes;

        public NoteRepository()
        {
            var appPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path,
            "Data");
            var database = new LiteDatabase(appPath);
            casenotes = database.GetCollection<CaseNote>("CaseNotes") as LiteCollection<CaseNote>;
        }

        public IEnumerable<CaseNote> ReturnAll()
        {
            return casenotes.FindAll();
        }

        public IEnumerable<CaseNote> ReturnByDateTime(DateTimeOffset? startTime, DateTimeOffset? endTime)
        {
            IEnumerable<CaseNote> notes;
            notes = casenotes.Find(x => x.StartTime < endTime && startTime < x.EndTime);
            return notes;
        }

        public IEnumerable<CaseNote> ReturnByTimeSpan(TimeSpan startTime, TimeSpan endTime, IEnumerable<CaseNote>? filtered)
        {
            IEnumerable<CaseNote> notes;
            if (filtered == null)
            {
                notes = casenotes.Find(x => x.StartTime.TimeOfDay < endTime && startTime < x.EndTime.TimeOfDay);
            }
            else
            {
                notes = filtered.Where((x) =>
                { return x.StartTime.TimeOfDay < endTime && startTime < x.EndTime.TimeOfDay; }).ToList();
            }
            return notes;
        }

        public IEnumerable<CaseNote> QueryByConsumer(string query, IEnumerable<CaseNote>? filtered)
        {
            Regex rgx = new Regex(query);
            if (filtered == null)
            {
                return casenotes.Query().Where(x =>
                x.Consumer.ToLower().Contains(query.ToLower())
                || query.ToLower().Contains(x.Consumer.ToLower())
                || rgx.IsMatch(x.Consumer))
                         .ToList();
            }
            else
            {
                return filtered.Where(x =>
                x.Consumer.ToLower().Contains(query.ToLower())
                || query.ToLower().Contains(x.Consumer.ToLower())
                || rgx.IsMatch(x.Consumer))
                         .ToList();
            }
        }

        public IEnumerable<CaseNote> QueryByDocumentation(string query, IEnumerable<CaseNote>? filtered)
        {
            IEnumerable<CaseNote> notes;
            Regex rgx = new Regex(query);
            if (filtered == null)
            {
                return casenotes.Query().Where(x =>
                x.Documentation.ToLower().Contains(query.ToLower())
                || query.ToLower().Contains(x.Documentation.ToLower())
                || rgx.IsMatch(x.Documentation))
                         .ToList();
            }
            else
            {
                return filtered.Where(x =>
                x.Documentation.ToLower().Contains(query.ToLower())
                || query.ToLower().Contains(x.Documentation.ToLower())
                || rgx.IsMatch(x.Documentation))
                         .ToList();
            }
        }


        public IEnumerable<CaseNote> ReturnOverlapping(IEnumerable<CaseNote> notes)
        {
            var overlapping = new List<CaseNote>();
            overlapping = (from first in notes
                           from second in notes
                           where first.StartTime < second.EndTime
                           && second.StartTime < first.EndTime
                           && first != second
                           select first).Distinct().ToList();
            return overlapping;
        }

        public Boolean SubmitNote(CaseNote caseNote)
        {
            casenotes.Insert(caseNote);
            return true;
        }
    }
}
