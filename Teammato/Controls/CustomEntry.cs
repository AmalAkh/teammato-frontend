namespace Teammato.Controls;

internal class CustomEntry : Entry
{
    public Color BorderColor 
    {
        get
        {
            return GetValue(BorderColorProperty) as Color;
        }
        set
        {
            SetValue(BorderColorProperty, value);
        }

    }
    
    public static readonly BindableProperty BorderColorProperty  = 
        BindableProperty.Create("BorderColor", typeof(Color), typeof(CustomEntry));
}