using Microsoft.Extensions.Caching.Memory;
using ServiceStack.Text;
using Casee.Models;
using Casee.Repositories.Interfaces;
using Casee.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Casee.Services
{
    // For more information on understanding and extending activation flow see
    // https://github.com/Microsoft/WindowsTemplateStudio/blob/release/docs/UWP/activation.md
    public class NoteService : INoteService
    {

        Lazy<INoteRepository> noteRepository;
        Lazy<IMemoryCache> cache;

        public NoteService()
        {
            noteRepository = App.NoteRepository;
            cache = App.Cache;
        }

        public string CaseNotesToCSV(IEnumerable<CaseNote> casenotes = null)
        {
            return CsvSerializer.SerializeToCsv(casenotes);
        }

        public IEnumerable<CaseNote> FilterByDate(DateTimeOffset start, DateTimeOffset end)
        {
            var results = noteRepository.Value.ReturnByDateTime(start, end).ToList();
            cache.Value.Set("AllNotes", results);
            return results;
        }

        public IEnumerable<CaseNote> FilterByTime(TimeSpan start, TimeSpan end)
        {
            return noteRepository.Value.ReturnByTimeSpan(start, end);
        }

        public IEnumerable<CaseNote> FilterByOverlapping(IEnumerable<CaseNote> caseNotes = null)
        {
            return noteRepository.Value.ReturnOverlapping(caseNotes ?? CaseNotes());
        }


        public IEnumerable<CaseNote> Query(string query, bool byDocs = true, bool byClient = true)
        {
            var notes = new List<CaseNote>();
            if (byDocs) { notes.AddRange(noteRepository.Value.QueryByDocumentation(query, null)); }
            if (byClient) { notes.AddRange(noteRepository.Value.QueryByConsumer(query, null)); }
            return notes;
        }

        public IEnumerable<CaseNote> CaseNotes()
        {
            return noteRepository.Value.ReturnAll();
        }

        public Boolean SubmitNote(CaseNote caseNote)
        {
            return noteRepository.Value.SubmitNote(caseNote);
        }

    }
}
