using LiteDB;
using Microsoft.Extensions.Caching.Memory;
using SnapNotes.Models;
using SnapNotes.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SnapNotes.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditPage : Page, INotifyPropertyChanged
    {
        Lazy<INoteService> noteService;
        Lazy<IMemoryCache> cache;
        Dictionary<int,string> caseNotes;
        public EditPage()
        {
            InitializeComponent();
            noteService = App.NoteService;
            cache = App.Cache;
            caseNotes = new Dictionary<int, string>();
            var rawNotes = cache.Value.Get("Notes") as IEnumerable<CaseNote>;
            foreach ( var note in rawNotes.Select((value, i) => new { i, value }))
            {
                caseNotes.Add(note.i, note.value.ToString());
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

