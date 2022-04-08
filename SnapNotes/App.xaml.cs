﻿using System;
using System.IO;
using LiteDB;
using SnapNotes.Models;
using SnapNotes.Services;

using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace SnapNotes
{
    public partial class App : Application
    {
        private Lazy<ActivationService> _activationService;
        public Lazy<NoteService> noteService;
        public string appPath;

        private ActivationService ActivationService
        {
            get { return _activationService.Value; }
        }

        public App()
        {
            InitializeComponent();
            UnhandledException += OnAppUnhandledException;

            // Deferred execution until used. Check https://docs.microsoft.com/dotnet/api/system.lazy-1 for further info on Lazy<T> class.
            _activationService = new Lazy<ActivationService>(CreateActivationService);
            appPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path,
            "Data");
            var database = new LiteDatabase(appPath);
            var casenotes = database.GetCollection<CaseNote>("CaseNotes");
            noteService = new Lazy<NoteService>(CreateNoteService(casenotes));
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (!args.PrelaunchActivated)
            {
                await ActivationService.ActivateAsync(args);
            }
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
        }

        private void OnAppUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            // TODO WTS: Please log and handle the exception as appropriate to your scenario
            // For more info see https://docs.microsoft.com/uwp/api/windows.ui.xaml.application.unhandledexception
        }

        private ActivationService CreateActivationService()
        {
            return new ActivationService(this, typeof(Views.MainPage), new Lazy<UIElement>(CreateShell));
        }

        private NoteService CreateNoteService(ILiteCollection<CaseNote> casenotes)
        {
            return new NoteService(casenotes);
        }

        private UIElement CreateShell()
        {
            return new Views.ShellPage();
        }

        protected override async void OnShareTargetActivated(ShareTargetActivatedEventArgs args)
        {
            await ActivationService.ActivateFromShareTargetAsync(args);
        }
    }
}
