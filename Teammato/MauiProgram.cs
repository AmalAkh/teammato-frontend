using Firebase.Analytics;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Teammato.Utils;
namespace Teammato;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("fa-solid-900.ttf", "FontAwesomeSolid");
            });
        builder.Services.AddSingleton<IAnalyticsService, AnalyticsService>();
      
        builder.ConfigureLifecycleEvents(events =>
        {
#if ANDROID
            events.AddAndroid(android => android.OnCreate((activity, bundle) =>
            {
                var app = Firebase.FirebaseApp.InitializeApp(activity);
                var crashlytics = Firebase.Crashlytics.FirebaseCrashlytics.Instance;
                crashlytics.SetCrashlyticsCollectionEnabled(new Java.Lang.Boolean(true));
                FirebaseAnalytics.GetInstance(Android.App.Application.Context).SetAnalyticsCollectionEnabled(true);
            }));
#endif
        });
        

#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}