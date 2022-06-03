using Microsoft.Toolkit.Uwp.Notifications;
using Casee.Models;
using Casee.Services.Interfaces;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace Casee.Views
{
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        App app;
        Lazy<INoteService> noteService;
        DateTime serviceStartTime;
        DateTime serviceEndTime;

        public MainPage()
        {
            InitializeComponent();
            noteService = App.NoteService;
            serviceDate.MaxYear = DateTimeOffset.Now.AddYears(5);
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


        private void ClearDateButton_Click(object sender, RoutedEventArgs e)
        {
            serviceStartTime = new DateTime();
            serviceEndTime = new DateTime();
            serviceDate.SelectedDate = null;
            serviceStart.SelectedTime = null;
            serviceEnd.SelectedTime = null;
            consumer.Text = string.Empty;
            documentation.Text = string.Empty;
            serviceTime.Text = string.Empty;
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void submit_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var _note = new CaseNote()
            {
                Consumer = consumer.Text,
                Documentation = documentation.Text,
                StartTime = serviceStartTime,
                EndTime = serviceEndTime
            };

            if (noteService.Value.SubmitNote(_note))
            {
                new ToastContentBuilder()
                .AddText("Submitted note:")
                .AddText(_note.ToString())
                .Show();
            }

        }


        //TIMEPICKING SHENANIGANS - NEEDS 'DRY' REFACTOR
        private void serviceStart_SelectedTimeChanged(TimePicker sender, TimePickerSelectedValueChangedEventArgs args)
        {
            if (serviceStart.SelectedTime != null)
            {
                serviceStartTime = new DateTime(serviceStartTime.Year, serviceStartTime.Month, serviceStartTime.Day,
                                               args.NewTime.Value.Hours, args.NewTime.Value.Minutes, args.NewTime.Value.Seconds);
                if (serviceEndTime < serviceStartTime)
                {
                    serviceEndTime = serviceStartTime;
                    serviceEnd.SelectedTime = serviceStart.SelectedTime;
                }
            }
            serviceTime.Text = $"{serviceStartTime.ToString() ?? "--" } to {serviceEndTime.ToString() ?? "--" }";
            //NEED TO FORCE ENDTIME TO ALWAYS BE LATER THAN STARTTIME
        }

        private void serviceEnd_SelectedTimeChanged(TimePicker sender, TimePickerSelectedValueChangedEventArgs args)
        {
            if (serviceEnd.SelectedTime != null)
            {
                serviceEndTime = new DateTime(serviceStartTime.Year, serviceStartTime.Month, serviceStartTime.Day,
                                               args.NewTime.Value.Hours, args.NewTime.Value.Minutes, args.NewTime.Value.Seconds);
                if (serviceEndTime < serviceStartTime)
                {
                    serviceStartTime = serviceEndTime;
                    serviceStart.SelectedTime = serviceEnd.SelectedTime;
                }
            }
            serviceTime.Text = $"{serviceStartTime.ToString() ?? "--" } to {serviceEndTime.ToString() ?? "--" }";
            //NEED TO FORCE ENDTIME TO ALWAYS BE LATER THAN STARTTIME
        }

        private void serviceDate_SelectedDateChanged(DatePicker sender, DatePickerSelectedValueChangedEventArgs args)
        {
            if (serviceDate.SelectedDate != null)
            {
                serviceStartTime = new DateTime(args.NewDate.Value.Year, args.NewDate.Value.Month, args.NewDate.Value.Day,
                                                serviceStartTime.Hour, serviceStartTime.Minute, serviceStartTime.Second);
                serviceEndTime = new DateTime(args.NewDate.Value.Year, args.NewDate.Value.Month, args.NewDate.Value.Day,
                                                serviceEndTime.Hour, serviceEndTime.Minute, serviceEndTime.Second);
                serviceTime.Text = $"{serviceStartTime.ToString() ?? "--" } to {serviceEndTime.ToString() ?? "--" }";
            }
        }
    }
}
