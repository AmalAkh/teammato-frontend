namespace Teammato.Controls;

public class CustomPicker : Picker
{
    public static readonly BindableProperty BorderColorProperty  = 
        BindableProperty.Create("BorderColor", typeof(Color), typeof(CustomEditor));
    public Color BorderColor 
    {
        get { return GetValue(BorderColorProperty) as Color; }
        set { SetValue(BorderColorProperty, value); }
    }
}