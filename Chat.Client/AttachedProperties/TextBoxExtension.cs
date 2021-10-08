using System.Windows;
using System.Windows.Controls;

namespace Chat.Client.AttachedProperties
{
    public static class TextBoxExtension
    {
        public static readonly DependencyProperty BorderCornerRadiusProperty;

        static TextBoxExtension() 
        {
            BorderCornerRadiusProperty = DependencyProperty.RegisterAttached(
                name: "BorderCornerRadius",
                propertyType: typeof(CornerRadius),
                ownerType: typeof(TextBoxExtension));
        }

        public static CornerRadius GetBorderCornerRadius(TextBox textBox) =>
            (CornerRadius)textBox.GetValue(BorderCornerRadiusProperty);

        public static void SetBorderCornerRadius(TextBox textBox, CornerRadius borderCorderRadius) =>
            textBox.SetValue(BorderCornerRadiusProperty, borderCorderRadius);
    }
}
