using System;
using System.Collections.Generic;
using ServiceStack.Text;
using SnapNotes.Models;
using SnapNotes.Repositories.Interfaces;
using SnapNotes.Services.Interfaces;

namespace SnapNotes.Services
{
    // For more information on understanding and extending activation flow see
    // https://github.com/Microsoft/WindowsTemplateStudio/blob/release/docs/UWP/activation.md
    public class NoteService : INoteService
    {

        Lazy<INoteRepository> noteRepository;
        public NoteService()
        {
            this.noteRepository = App.NoteRepository;
        }

        public string CaseNotesToCSV(IEnumerable<CaseNote> casenotes = null)
        {
            return CsvSerializer.SerializeToCsv(casenotes);
        }

        public IEnumerable<CaseNote> FilterByDate(DateTimeOffset start, DateTimeOffset end)
        {
            return noteRepository.Value.ReturnByDateTime(start, end);
        }

        public IEnumerable<CaseNote> FilterByTime(TimeSpan start, TimeSpan end)
        {
            return noteRepository.Value.ReturnByTimeSpan(start, end);
        }

        public IEnumerable<CaseNote> FilterByOverlapping(IEnumerable<CaseNote> caseNotes = null)
        {
            return noteRepository.Value.ReturnOverlapping(caseNotes ?? this.CaseNotes());
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
