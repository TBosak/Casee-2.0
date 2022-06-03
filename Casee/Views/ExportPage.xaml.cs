using LiteDB;
using Microsoft.Extensions.Caching.Memory;
using Casee.Models;
using Casee.Services.Interfaces;
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

namespace Casee.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExportPage : Page, INotifyPropertyChanged
    {
        FileSavePicker savePicker;
        Lazy<INoteService> noteService;
        Lazy<IMemoryCache> cache;

        public ExportPage()
        {
            InitializeComponent();
            savePicker = new FileSavePicker();
            noteService = App.NoteService;
            cache = App.Cache;
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
            var ufid = Guid.NewGuid().ToString();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.TryAdd("Comma-separated values file", new List<string>() { ".csv" });
            savePicker.FileTypeChoices.TryAdd("Text file", new List<string>() { ".txt" });
            savePicker.FileTypeChoices.TryAdd("MS Excel Binary File format (Excel 03)", new List<string>() { ".xls" });
            savePicker.FileTypeChoices.TryAdd("MS Excel XML-based file format (Excel 07)", new List<string>() { ".xlsx" });
            savePicker.FileTypeChoices.TryAdd("MS Excel macro-enabled spreadsheet", new List<string>() { ".xlsm" });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = $"CaseeExport-{DateTime.Now.ToString("ddd, dd MMM yyy HH':'mm':'ss 'CDT'")}";
        }

        private void Edit(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Frame.Navigate(typeof(EditPage));
        }

        private void Export_All(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            DateTimeOffset? startDate = StartDate.Date;
            DateTimeOffset? endDate = EndDate.Date;
            TimeSpan? startTime = StartTime.Time;
            TimeSpan? endTime = EndTime.Time;
            DateTimeOffset start;
            DateTimeOffset end;
            start = startDate.HasValue ?
                    startDate.Value.Add(
                        startTime.HasValue ?
                        startTime.Value :
                        TimeSpan.MinValue)
                    : DateTimeOffset.MinValue;

            end = endDate.HasValue ?
                    endDate.Value.Add(
                        endTime.HasValue ?
                        endTime.Value :
                        TimeSpan.MaxValue)
                    : DateTimeOffset.MaxValue;

            var notes = noteService.Value.FilterByDate(start, end);

            if (DoubleBilling.IsChecked ?? false)
            {
                notes = noteService.Value.FilterByOverlapping(notes);
            }
            //WIP RIGHT HERE
            cache.Value.Set<IEnumerable<CaseNote>>("Notes", notes);
            ExportNotes(notes);
        }

        public async void ExportNotes(IEnumerable<CaseNote> notes)
        {
            setExport();

            StorageFile file = await savePicker.PickSaveFileAsync();
            string csv = noteService.Value.CaseNotesToCSV(notes);

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

