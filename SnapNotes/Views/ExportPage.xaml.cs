using LiteDB;
using ServiceStack.Text;
using SnapNotes.Models;
using SnapNotes.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public sealed partial class ExportPage : Page, INotifyPropertyChanged
    {
        FileSavePicker savePicker;
        App app;
        ILiteCollection<CaseNote> casenotes;
        NoteService noteService;

        public ExportPage()
        {
            this.InitializeComponent();
            this.savePicker = new FileSavePicker();
            app = Application.Current as App;
            noteService = app.noteService.Value;
            casenotes = noteService.CaseNotes();
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

        private void setExport()
        {
            this.savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            // Dropdown of file types the user can save the file as
            this.savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".csv" });
            // Default file name if the user does not type one in or select a file to replace
            this.savePicker.SuggestedFileName = "New Document";
        }

        private void Export_All(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ExportNotes(casenotes.FindAll());
        }

        private void Filter_ByDateTime(int? year, int? month, int? day, int? hour, int? minute)
        {
            IEnumerable<CaseNote> notes;
            if (year.HasValue) { notes = casenotes.Find(x => x.StartTime.Year == year || x.EndTime.Year == year); }
            else { notes = casenotes.Find(x => x.StartTime.Year == year || x.EndTime.Year == year); }
            var notes = casenotes.Find(x => x.StartTime.Month == month || x.EndTime.Month == month);
            var notes = casenotes.Find(x => x.StartTime.Day == day || x.EndTime.Day == day);
            var notes = casenotes.Find(x => x.StartTime.Hour == hour || x.EndTime.Hour == hour);
            var notes = casenotes.Find(x => x.StartTime.Minute == minute || x.EndTime.Minute == minute);
        }

        public async void ExportNotes(IEnumerable<CaseNote> notes)
        {
            setExport();
            
            StorageFile file = await savePicker.PickSaveFileAsync();
            string csv = CsvSerializer.SerializeToCsv<CaseNote>(notes);

            if (file != null)
            {
                // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
                CachedFileManager.DeferUpdates(file);
                // write to file
                await FileIO.WriteTextAsync(file, csv);
                // Let Windows know that we're finished changing the file so the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                if (status == FileUpdateStatus.Complete)
                {
                    Console.WriteLine("File saved");
                }
                else
                {
                    Console.WriteLine("File couldn't be saved");
                }
            }
            else
            {
                Console.WriteLine("Operation canceled.");
            }
        }
    }
}

