using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

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
            }).ConfigureMauiHandlers(handlers => { /* custom handlers */ }).ConfigureLifecycleEvents(events =>
        {
#if ANDROID
            events.AddAndroid(android =>
            {
                /*android.OnNewIntent((activity, intent) =>
                {
                    if (intent?.Extras != null)
                    {
                        string notificationData = intent.Extras.GetString("notificationData");
                        if (!string.IsNullOrEmpty(notificationData))
                        {
                            // Handle the notification tap
                            Application.Current?.MainPage?.DisplayAlert("Notification Tapped", notificationData, "OK");
                        }
                    }
                });*/
            });
#endif
        });;

#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}