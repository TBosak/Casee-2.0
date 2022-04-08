using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using ServiceStack.Text;
using SnapNotes.Activation;
using SnapNotes.Core.Helpers;
using SnapNotes.Models;
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
        public LiteCollection<CaseNote> _casenotes;

        public NoteService(ILiteCollection<CaseNote> casenotes)
        {
            _casenotes = casenotes as LiteCollection<CaseNote>;
        }

        public string CaseNotesToCSV(IEnumerable<CaseNote> casenotes = null)
        {
            if(casenotes == null)
            {
                casenotes = _casenotes.FindAll();
            }

            return CsvSerializer.SerializeToCsv<CaseNote>(casenotes);

        }

        public LiteCollection<CaseNote> CaseNotes()
        {
            return _casenotes;
        }
    }
}
