using Microsoft.Extensions.DependencyInjection;
using SnapNotes.Models;
using SnapNotes.Repositories;
using SnapNotes.Repositories.Interfaces;
using SnapNotes.Services;
using SnapNotes.Services.Interfaces;
using System;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace SnapNotes
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

        private static IServiceProvider ConfigureServices()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient(typeof(Lazy<>), typeof(LazyInstance<>));
            serviceCollection.AddSingleton<INoteRepository, NoteRepository>();
            serviceCollection.AddScoped<INoteService, NoteService>();

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
