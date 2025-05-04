using Microsoft.Maui.Platform;
using Teammato.Controls;
using Teammato.Pages;
using Teammato.Services;

namespace Teammato;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        RestAPIService.Init("http://192.168.100.3:8080/api/");
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
        
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(IView.Background), (handler, view) =>
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
    }
    
    protected override  Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new LoginPage());
       
    }
}