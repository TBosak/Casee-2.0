using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using ServiceStack.Text;
using SnapNotes.Activation;
using SnapNotes.Core.Helpers;
using SnapNotes.Models;
using SnapNotes.Repositories;
using SnapNotes.Services;

using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SnapNotes.Services
{
    // For more information on understanding and extending activation flow see
    // https://github.com/Microsoft/WindowsTemplateStudio/blob/release/docs/UWP/activation.md
    public partial class NoteService
    {

        NoteRepository noteRepo;
        public NoteService(NoteRepository noteRepo)
        {
            this.noteRepo = noteRepo;
        }

        public string CaseNotesToCSV(IEnumerable<CaseNote> casenotes = null)
        {
            return CsvSerializer.SerializeToCsv<CaseNote>(casenotes);
        }

        public IEnumerable<CaseNote> FilterByDate(DateTimeOffset start, DateTimeOffset end)
        {
            return noteRepo.ReturnByDateTime(start, end);
        }

        public IEnumerable<CaseNote> FilterByOverlapping(IEnumerable<CaseNote> caseNotes = null)
        {
            return noteRepo.ReturnOverlapping(caseNotes ?? this.CaseNotes());
        }

        public IEnumerable<CaseNote> CaseNotes()
        {
            return noteRepo.ReturnAll();
        }

        public Boolean SubmitNote(CaseNote caseNote)
        {
            return noteRepo.SubmitNote(caseNote);
        }

        //builds a Datetime reflective of user-selected date + time
        private DateTimeOffset CombineDateTime(DateTimeOffset dateObj, TimeSpan timeObj)
        {
            return dateObj - dateObj.TimeOfDay + timeObj;
        }
    }
}
