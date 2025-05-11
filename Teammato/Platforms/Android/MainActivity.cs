using Android.App;
using Android.Content.PM;
using Android.OS;
using Firebase;
using Firebase.Analytics;
using Teammato.Utils;


namespace Teammato;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode |
                           ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        
    }
    
}
public class AnalyticsService : IAnalyticsService
{
    public void LogEvent(string eventName, Dictionary<string, string> parameters)
    {
        var firebaseAnalytics = Firebase.Analytics.FirebaseAnalytics.GetInstance(Android.App.Application.Context);
        var bundle = new Android.OS.Bundle();
        if (parameters != null)
        {
            foreach (var param in parameters)
            {
                bundle.PutString(param.Key, param.Value);
            }
        }
        firebaseAnalytics.LogEvent(eventName, bundle);
    }
}