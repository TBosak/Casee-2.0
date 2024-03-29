﻿using LiteDB;
using Microsoft.Extensions.Caching.Memory;
using Casee.Models;
using Casee.Services.Interfaces;
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
using System.Collections.ObjectModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Casee.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditPage : Page, INotifyPropertyChanged
    {
        Lazy<INoteService> noteService;
        Lazy<IMemoryCache> cache;
        public ObservableCollection<CaseNote> caseNotes { get; set; }
        public EditPage()
        {
            InitializeComponent();
            noteService = App.NoteService;
            cache = App.Cache;
            caseNotes = cache.Value.Get("Notes") as ObservableCollection<CaseNote>;
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

