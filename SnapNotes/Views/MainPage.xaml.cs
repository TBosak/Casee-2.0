using LiteDB;
using Microsoft.Toolkit.Uwp.Notifications;
using ServiceStack;
using ServiceStack.Text;
using SnapNotes.Models;
using SnapNotes.Services;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SnapNotes.Views
{
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        App app;
        NoteService noteService;

        public MainPage()
        {
            InitializeComponent();
            app = Application.Current as App;
            noteService = app.noteService.Value;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void submit_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
                var noteDate = this.dateOfService.Date;
                var _note = new CaseNote()
                {
                    Consumer = this.consumer.Text,
                    Documentation = this.documentation.Text,
                    StartTime = noteDate.Add(this.startTime.Time),
                    EndTime = noteDate.Add(this.endTime.Time)
                };

            if (noteService.SubmitNote(_note))
            {
                new ToastContentBuilder()
                .AddArgument("action", "viewConversation")
                .AddArgument("conversationId", 9813)
                .AddText("From database:")
                .AddText(_note.ToString())
                .Show();
            }
                
        }
    }
}
