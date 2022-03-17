using LiteDB;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

using Windows.UI.Xaml.Controls;

namespace SnapNotes.Views
{
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public MainPage()
        {
            InitializeComponent();
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
            var filePath = Path.Combine(
            Windows.Storage.ApplicationData.Current.LocalFolder.Path,
            "Data");
            using (var db = new LiteDatabase(filePath))
            {
                var _shift = new shift()
                {
                    date = this.date.SelectedDate.ToString(),
                    start = this.startTime.Time.ToString(),
                    end = this.endTime.Time.ToString()
                };

                var col = db.GetCollection<shift>("shifts");
                col.Insert(_shift);

                // Requires Microsoft.Toolkit.Uwp.Notifications NuGet package version 7.0 or greater
                new ToastContentBuilder()
                    .AddArgument("action", "viewConversation")
                    .AddArgument("conversationId", 9813)
                    .AddText("From database:")
                    .AddText(col.Query().Where(x => x.date.Contains(this.date.SelectedDate.ToString())).ToList().ToString())
                    .Show(); // Not seeing the Show() method? Make sure you have version 7.0, and if you're using .NET 5, your TFM must be net5.0-windows10.0.17763.0 or greater
            }
        }
        public class shift
        {
            public string date { get; set; }
            public string start { get; set; }
            public string end { get; set; }
        }
    }
}
