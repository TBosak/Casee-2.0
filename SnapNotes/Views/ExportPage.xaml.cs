using LiteDB;
using ServiceStack.Text;
using SnapNotes.Models;
using SnapNotes.Services;
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
            DateTimeOffset? startDate = this.StartDate.Date;
            DateTimeOffset? endDate = this.EndDate.Date;
            TimeSpan? startTime = this.StartTime.Time;
            TimeSpan? endTime = this.EndTime.Time;
            DateTimeOffset start;
            DateTimeOffset end;
            start = startDate.HasValue ?
                    startDate.Value.Add(
                        startTime.HasValue ?
                        startTime.Value :
                        TimeSpan.MinValue )
                    : DateTimeOffset.MinValue.Add(
                        startTime.HasValue ?
                        startTime.Value :
                        TimeSpan.MinValue);
            end = endDate.HasValue ?
                    endDate.Value.Add(
                        endTime.HasValue ?
                        endTime.Value :
                        TimeSpan.MaxValue)
                    : DateTimeOffset.MaxValue.Add(
                        endTime.HasValue ?
                        endTime.Value :
                        TimeSpan.MaxValue);
            var notes = noteService.FilterByDate(start, end);
            if (DoubleBilling.IsChecked ?? false) notes = noteService.FilterByOverlapping(notes);
            //WIP RIGHT HERE
            ExportNotes(notes);
        }



        public async void ExportNotes(IEnumerable<CaseNote> notes)
        {
            setExport();

            StorageFile file = await savePicker.PickSaveFileAsync();
            string csv = noteService.CaseNotesToCSV(notes);

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


        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

