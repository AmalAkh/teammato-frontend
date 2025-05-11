namespace Teammato.Controls;

internal class CustomEditor : Editor
{
    public static readonly BindableProperty BorderColorProperty  = 
        BindableProperty.Create("BorderColor", typeof(Color), typeof(CustomEditor));
    public Color BorderColor 
    {
        get { return GetValue(BorderColorProperty) as Color; }
        set { SetValue(BorderColorProperty, value); }
    }
}