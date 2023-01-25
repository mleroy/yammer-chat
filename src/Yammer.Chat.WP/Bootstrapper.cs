using Caliburn.Micro;
using Microsoft.ApplicationInsights.Telemetry.WindowsStore;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Navigation;
using Yammer.Chat.Core;
using Yammer.Chat.Core.API;
using Yammer.Chat.Core.Parsers;
using Yammer.Chat.Core.Repositories;
using Yammer.Chat.Core.Resources;
using Yammer.Chat.Core.Services;
using Yammer.Chat.ViewModels;
using Yammer.Chat.WP.Core;
using Yammer.OAuthSDK.Utils;

namespace Yammer.Chat.WP
{
    public class Bootstrapper : PhoneBootstrapperBase
    {
        PhoneContainer container;

        private IIdentityStore identityStore;
        private IRealtimeManager realtimeManager;
        private IResumeManager resumeManager;
        private ISettings settings;
        private INavigator navigator;
        private IClientConfiguration clientConfiguration;

        private const string DeactivateTimeSettingsKey = "DeactivateTime";
        private const string SessionTypeSettingsKey = "SessionType";

        private bool hasDeactivated;

        public Bootstrapper()
        {
            Start();
        }

        protected override void Configure()
        {
            if (Execute.InDesignMode)
                return;

            var config = new TypeMappingConfiguration
            {
                DefaultSubNamespaceForViewModels = "ViewModels",
                DefaultSubNamespaceForViews = "WP.Views"
            };

            ViewModelLocator.ConfigureTypeMappings(config);
            ViewLocator.ConfigureTypeMappings(config);

            container = new PhoneContainer();

            container.RegisterPhoneServices(RootFrame);

            container.Singleton<IHttpClientProvider, HttpClientProvider>(); // It is recommended to re-use a single instance of HttpClient
            container.Singleton<IHttpService, HttpService>(); // Keep decorators around

            container.Singleton<IRealtimeManager, RealtimeManager>();
            container.Singleton<IResumeManager, ResumeManager>();

            // View models
            container.PerRequest<ShellViewModel>();
            container.PerRequest<OAuthLoginViewModel>();
            //container.PerRequest<LoginViewModel>();
            //container.PerRequest<SsoLoginViewModel>();
            container.PerRequest<SettingsViewModel>();
            container.PerRequest<ThreadsViewModel>();
            container.PerRequest<ThreadViewModel>();
            container.PerRequest<ConversationDetailsViewModel>();
            container.PerRequest<ProfileViewModel>();
            container.PerRequest<EditProfileViewModel>();
            container.PerRequest<ParticipantSelectionViewModel>();

            // Services
            container.PerRequest<IApiService, ApiService>();
            container.PerRequest<IMessagesService, MessagesService>();
            container.PerRequest<IUserService, UserService>();
            container.PerRequest<IFilesService, FilesService>();

            // Repositories
            container.PerRequest<IClientConfiguration, DefaultClientConfiguration>();
            container.PerRequest<ITokenStore, TokenStore>();
            container.PerRequest<IFileRepository, FileRepository>();
            container.Singleton<IIdentityStore, IdentityStore>(); // Singleton to hold on to identity instead of reading it from storage every time
            container.Singleton<IThreadRepository, ThreadRepository>(); // Make a single instance cache, no need for this to be single instance'd
            container.Singleton<IUserRepository, UserRepository>(); // Make a single instance cache, no need for this to be single instance'd
            container.Singleton<IRealtimeRepository, RealtimeRepository>(); // Make a single instance cache, no need for this to be single instance'd

            // Parsers
            container.PerRequest<IThreadParser, ThreadParser>();
            container.PerRequest<IMessageParser, MessageParser>();
            container.PerRequest<IAttachmentParser, AttachmentParser>();
            container.PerRequest<IParticipantParser, ParticipantParser>();
            container.PerRequest<IUserParser, UserParser>();

            // Phone services
            container.Instance<PhoneApplicationFrame>(this.RootFrame);
            container.Instance<IProgressIndicator>(new Yammer.Chat.WP.Core.ProgressIndicator(this.RootFrame));
            container.Singleton<INavigator, Navigator>(); // Persist some state related to application lifecycle
            container.PerRequest<ICryptographer, Cryptographer>();
            container.PerRequest<ISettings, Settings>();
            container.PerRequest<IPhotoChooser, PhotoChooser>();
            container.PerRequest<ITileManager, TileManager>();
            container.PerRequest<IApplicationVersion, ApplicationVersion>();
            container.PerRequest<IEmailer, Emailer>();
            container.PerRequest<IVibrator, Vibrator>();
            container.PerRequest<IOAuthWrapper, OAuthWrapper>();

            this.Inject();

            InitializeLanguage();
            RegisterUriMapper();
        }

        private void Inject()
        {
            this.identityStore = container.GetInstance<IIdentityStore>();
            this.realtimeManager = container.GetInstance<IRealtimeManager>();
            this.resumeManager = container.GetInstance<IResumeManager>();
            this.settings = container.GetInstance<ISettings>();
            this.navigator = container.GetInstance<INavigator>();
            this.clientConfiguration = container.GetInstance<IClientConfiguration>();
        }

        protected override void OnLaunch(object sender, LaunchingEventArgs e)
        {
            base.OnLaunch(sender, e);

            Initialize();
        }

        private void Initialize()
        {
            // Using this static pattern for convenience. All classes need logging, and this allows not having to inject logging everywhere.
            Analytics.Default = new ApplicationInsightsWrapper("2deb8b8e-0445-420b-ad47-a4e416864a51");

            this.identityStore.AutoLogin();

            container.GetInstance<IHttpService>().SetDecorator(httpClient =>
            {
                httpClient.DefaultRequestHeaders.Authorization = this.identityStore.IsLoggedIn
                    ? new AuthenticationHeaderValue("Bearer", this.identityStore.Token)
                    : null;
            });

            // Put expensive work on a background thread and not delay showing the UI (does this actually work?)
            Task.Run(() =>
            {
                BackgroundAgentManager.Setup();
            });

            this.realtimeManager.Initialize();

            this.RemoveCurrentDeactivationSettings();

            Telerik.Windows.Controls.InteractionEffectManager.AllowedTypes.Add(typeof(Telerik.Windows.Controls.RadDataBoundListBoxItem));
        }

        protected override void OnActivate(object sender, ActivatedEventArgs e)
        {
            base.OnActivate(sender, e);

            this.navigator.MustClearPageStack = MustClearPageStack();

            // If IsApplicationInstancePreserved is not true, then set the session type to the value
            // saved in isolated storage. This will make sure the session type is correct for an
            // app that is being resumed after being tombstoned.
            if (!e.IsApplicationInstancePreserved)
            {
                RestoreSessionType();
                this.Initialize();
            }
            else
            {
                this.realtimeManager.Setup();

                this.resumeManager.NeedsToRefreshThreads = hasDeactivated;
                this.resumeManager.NeedsToRefreshThread = hasDeactivated;
            }
        }
        
        protected override void OnDeactivate(object sender, DeactivatedEventArgs e)
        {
            base.OnDeactivate(sender, e);

            this.ClearTile();
            this.RecordActivationTime();
            this.SaveCurrentDeactivationSettings();

            this.realtimeManager.Disconnect();

            this.hasDeactivated = true;
        }

        protected override void OnClose(object sender, ClosingEventArgs e)
        {
            base.OnClose(sender, e);

            this.ClearTile();
            this.RecordActivationTime();
            this.RemoveCurrentDeactivationSettings();

            this.realtimeManager.Disconnect();
        }

        public void SaveCurrentDeactivationSettings()
        {
            this.settings.AddOrUpdate(DeactivateTimeSettingsKey, DateTimeOffset.Now);
            this.settings.AddOrUpdate(SessionTypeSettingsKey, this.navigator.SessionType);
        }

        private void RemoveCurrentDeactivationSettings()
        {
            this.settings.Remove(DeactivateTimeSettingsKey, SessionTypeSettingsKey);
        }

        private void RecordActivationTime()
        {
            this.settings.AddOrUpdate("LastActivationTime", DateTime.Now);
        }

        private bool MustClearPageStack()
        {
            DateTimeOffset lastDeactivated;

            this.settings.TryGetValue(DeactivateTimeSettingsKey, out lastDeactivated);

            var currentDuration = DateTimeOffset.Now.Subtract(lastDeactivated);

            return TimeSpan.FromHours(currentDuration.TotalHours) > TimeSpan.FromHours(2);
        }

        public void RestoreSessionType()
        {
            SessionType sessionType;
            this.settings.TryGetValue(SessionTypeSettingsKey, out sessionType);
            this.navigator.SessionType = sessionType;
        }

        private void ClearTile()
        {
            var tileManager = container.GetInstance<ITileManager>();
            tileManager.ClearTile();
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return new[] { 
                GetType().GetTypeInfo().Assembly,
                typeof(ViewModelBase).GetTypeInfo().Assembly
            };
        }

        protected override object GetInstance(Type service, string key)
        {
            var instance = container.GetInstance(service, key);
            if (instance != null)
                return instance;

            throw new InvalidOperationException("Could not locate any instances.");
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }

        private void RegisterUriMapper()
        {
            this.RootFrame.UriMapper = new OAuthResponseUriMapper(this.clientConfiguration.OAuthCallbackUri);
        }

        // Initialize the app's font and flow direction as defined in its localized resource strings.
        //
        // To ensure that the font of your application is aligned with its supported languages and that the
        // FlowDirection for each of those languages follows its traditional direction, ResourceLanguage
        // and ResourceFlowDirection should be initialized in each resx file to match these values with that
        // file's culture. For example:
        //
        // AppResources.es-ES.resx
        //    ResourceLanguage's value should be "es-ES"
        //    ResourceFlowDirection's value should be "LeftToRight"
        //
        // AppResources.ar-SA.resx
        //     ResourceLanguage's value should be "ar-SA"
        //     ResourceFlowDirection's value should be "RightToLeft"
        //
        // For more info on localizing Windows Phone apps see http://go.microsoft.com/fwlink/?LinkId=262072.
        //
        private void InitializeLanguage()
        {
            try
            {
                RootFrame.Language = XmlLanguage.GetLanguage(AppResources.ResourceLanguage);

                FlowDirection flow = (FlowDirection)Enum.Parse(typeof(FlowDirection), AppResources.ResourceFlowDirection);
                RootFrame.FlowDirection = flow;
            }
            catch
            {
                // If an exception is caught here it is most likely due to either
                // ResourceLangauge not being correctly set to a supported language
                // code or ResourceFlowDirection is set to a value other than LeftToRight
                // or RightToLeft.

                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                throw;
            }
        }
    }
}
