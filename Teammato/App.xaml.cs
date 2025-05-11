using System.Text.Json;
using Firebase;
using Firebase.Analytics;
using Microsoft.Maui.Platform;
using Teammato.Controls;
using Teammato.Pages;
using Teammato.Services;
using OneSignalSDK.DotNet;
using OneSignalSDK.DotNet.Core;
using OneSignalSDK.DotNet.Core.Debug;
using Teammato.Abstractions;
using Teammato.ViewModels;
using Firebase.Database;
using Teammato.Utils;


namespace Teammato;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();


        RestAPIService.Init("http://147.175.160.66:8080/");


        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(IView.Background), (handler, view) =>
        {
            if (view is CustomEntry entry)
            {
#if ANDROID
                var bg = new Android.Graphics.Drawables.GradientDrawable();
                bg.SetStroke(2, entry.BorderColor.ToPlatform());
                bg.SetColor(entry.BackgroundColor.ToPlatform());
                bg.SetCornerRadius(32);
                bg.SetPadding(20, 10, 10, 10);

                handler.PlatformView.Background = bg;
#elif IOS
                handler.PlatformView.Layer.BorderColor = entry.BorderColor.ToCGColor();
                handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.RoundedRect;
                handler.PlatformView.Layer.BackgroundColor = entry.BackgroundColor.ToCGColor();
                handler.PlatformView.Layer.CornerRadius = 12; 
                handler.PlatformView.Layer.BorderWidth = 0.5f;
                
                handler.PlatformView.Layer.MasksToBounds = true;
                handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
                
                var leftPadding = new UIKit.UIView(new CoreGraphics.CGRect(0, 0, 12, 0));
                handler.PlatformView.LeftView = leftPadding;
                handler.PlatformView.LeftViewMode = UIKit.UITextFieldViewMode.Always;
#endif
            }
        });
        
        Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping(nameof(IView.Background), (handler, view) =>
        {
            if (view is CustomEditor editor)
            {
#if ANDROID
                var bg = new Android.Graphics.Drawables.GradientDrawable();
                bg.SetStroke(2, editor.BorderColor.ToPlatform());
                bg.SetColor(editor.BackgroundColor.ToPlatform());
                bg.SetCornerRadius(32);
                bg.SetPadding(20, 10, 10, 10);

                handler.PlatformView.Background = bg;
#elif IOS
                handler.PlatformView.Layer.BorderColor = editor.BorderColor.ToCGColor();
                handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.RoundedRect;
                handler.PlatformView.Layer.BackgroundColor = editor.BackgroundColor.ToCGColor();
                handler.PlatformView.Layer.CornerRadius = 12; 
                handler.PlatformView.Layer.BorderWidth = 0.5f;
                
                handler.PlatformView.Layer.MasksToBounds = true;
                handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
                
                var leftPadding = new UIKit.UIView(new CoreGraphics.CGRect(0, 0, 12, 0));
                handler.PlatformView.LeftView = leftPadding;
                handler.PlatformView.LeftViewMode = UIKit.UITextFieldViewMode.Always;
#endif                
            }
        });
        
        Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping(nameof(IView.Background), (handler, view) =>
        {
            if (view is CustomPicker picker)
            {
#if ANDROID
                var bg = new Android.Graphics.Drawables.GradientDrawable();
                bg.SetStroke(2, picker.BorderColor.ToPlatform());
                bg.SetColor(picker.BackgroundColor.ToPlatform());
                bg.SetCornerRadius(32);
                bg.SetPadding(20, 10, 10, 10);

                handler.PlatformView.Background = bg;
#elif IOS
                handler.PlatformView.Layer.BorderColor = editor.BorderColor.ToCGColor();
                handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.RoundedRect;
                handler.PlatformView.Layer.BackgroundColor = editor.BackgroundColor.ToCGColor();
                handler.PlatformView.Layer.CornerRadius = 12; 
                handler.PlatformView.Layer.BorderWidth = 0.5f;
                
                handler.PlatformView.Layer.MasksToBounds = true;
                handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
                
                var leftPadding = new UIKit.UIView(new CoreGraphics.CGRect(0, 0, 12, 0));
                handler.PlatformView.LeftView = leftPadding;
                handler.PlatformView.LeftViewMode = UIKit.UITextFieldViewMode.Always;
#endif                
            }
        });
        Akavache.Registrations.Start("Teammato");
        OneSignal.Initialize("3f107787-d140-4cee-a820-f7904d3911c6");
        
        OneSignal.Notifications.RequestPermissionAsync(true);


        LocalProfileViewModel = new ProfileViewModel();
        SettingsViewModel = new SettingsViewModel();
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

        
        var analyticsService = new AnalyticsService();
        // Log a custom event when the app starts
        analyticsService?.LogEvent(FirebaseAnalytics.Event.AppOpen, new Dictionary<string, string>
        {
            { "timestamp", DateTime.UtcNow.ToString() }
        });
        
        
        
        

    }
    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        
        if (e.ExceptionObject is Exception ex)
        {
            HandleException(ex, "AppDomain Unhandled Exception");
        }
    }
    private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        e.SetObserved(); // Prevents the application from crashing
        HandleException(e.Exception, "TaskScheduler Unobserved Exception");
    }

    private void HandleException(Exception ex, string exceptionType)
    {
    //    Firebase.Crashlytics.FirebaseCrashlytics.Instance.Log($"Unhandled Exception: {ex.Message}");
      //  Firebase.Crashlytics.FirebaseCrashlytics.Instance.Log($"Stack Trace: {ex.StackTrace}");
       
    }
    private FirebaseClient _firebaseClient;
    
    public static ProfileViewModel LocalProfileViewModel { get; set; }
    public static SettingsViewModel SettingsViewModel { get; set; }
    
    protected override  Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new LoginPage());
       
    }
}