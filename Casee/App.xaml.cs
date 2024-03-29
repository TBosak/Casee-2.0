﻿using LiteDB;
using LiteDB.Engine;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Casee.Models;
using Casee.Repositories;
using Casee.Repositories.Interfaces;
using Casee.Services;
using Casee.Services.Interfaces;
using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace Casee
{
    public partial class App : Application
    {
        private Lazy<ActivationService> _activationService;
        public IServiceProvider Container { get; }

        private ActivationService ActivationService
        {
            get { return _activationService.Value; }
        }


        public App()
        {
            InitializeComponent();
            UnhandledException += OnAppUnhandledException;
            Container = ConfigureServices();
            // Deferred execution until used. Check https://docs.microsoft.com/dotnet/api/system.lazy-1 for further info on Lazy<T> class.
            _activationService = new Lazy<ActivationService>(CreateActivationService);
        }

        public static IServiceProvider Services => ((App)Application.Current).Container;
        public static Lazy<INoteRepository> NoteRepository => Services.GetService<Lazy<INoteRepository>>();
        public static Lazy<INoteService> NoteService => Services.GetService<Lazy<INoteService>>();
        public static Lazy<IMemoryCache> Cache => Services.GetService<Lazy<IMemoryCache>>();
        public static string DataPath => Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Data");

        private static IServiceProvider ConfigureServices()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient(typeof(Lazy<>), typeof(LazyInstance<>));
            serviceCollection.AddSingleton<INoteRepository, NoteRepository>();
            serviceCollection.AddScoped<INoteService, NoteService>();
            serviceCollection.AddMemoryCache();

            return serviceCollection.BuildServiceProvider();
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (!args.PrelaunchActivated)
            {
                await ActivationService.ActivateAsync(args);
                Xamarin.Forms.Forms.Init(args);
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
