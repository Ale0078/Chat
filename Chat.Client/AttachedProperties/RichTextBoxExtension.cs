using System.Windows;
using Emoji.Wpf;

namespace Chat.Client.AttachedProperties
{
    public static class RichTextBoxExtension//ToDo: Placeholder is broken
    {
        private static readonly DependencyProperty IsPlaceholderSettedProperty;

        public static readonly DependencyProperty IsPlaceholderAppliedProperty;
        public static readonly DependencyProperty PlaceholderProperty;

        static RichTextBoxExtension() 
        {
            IsPlaceholderSettedProperty = DependencyProperty.RegisterAttached(
                name: "IsPlaceholderSetted",
                propertyType: typeof(bool),
                ownerType: typeof(RichTextBoxExtension));

            IsPlaceholderAppliedProperty = DependencyProperty.RegisterAttached(
                name: "IsPlaceholderApplied",
                propertyType: typeof(bool),
                ownerType: typeof(RichTextBoxExtension));

            PlaceholderProperty = DependencyProperty.RegisterAttached(
                name: "Placeholder",
                propertyType: typeof(string),
                ownerType: typeof(RichTextBoxExtension),
                defaultMetadata: new PropertyMetadata(
                    propertyChangedCallback: OnPlaceholderChanged));
        }

        public static bool GetIsPlaceholderApplied(DependencyObject obj) =>
            (bool)obj.GetValue(IsPlaceholderAppliedProperty);

        public static void SetIsPlaceholderApplied(DependencyObject obj, bool value) =>
            obj.SetValue(IsPlaceholderAppliedProperty, value);

        public static string GetPlaceholder(DependencyObject obj) =>
            (string)obj.GetValue(PlaceholderProperty);

        public static void SetPlaceholder(DependencyObject obj, string value) =>
            obj.SetValue(PlaceholderProperty, value);

        private static void OnPlaceholderChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) 
        {
            RichTextBox richTextBox = obj as RichTextBox;

            if (richTextBox is null)
            {
                return;
            }

            if (!(bool)richTextBox.GetValue(IsPlaceholderSettedProperty))
            {
                richTextBox.LostKeyboardFocus += OnLostKeyboardFocus;
                richTextBox.GotKeyboardFocus += OnGotKeyboardFocus;

                SetIsPlaceholderApplied(richTextBox, true);

                richTextBox.Text = GetPlaceholder(richTextBox);

                richTextBox.SetValue(IsPlaceholderSettedProperty, true);
            }

            if (GetIsPlaceholderApplied(richTextBox) && (bool)richTextBox.GetValue(IsPlaceholderSettedProperty))
            {
                richTextBox.Text = GetPlaceholder(richTextBox);
            }
        }

        private static void OnLostKeyboardFocus(object sender, RoutedEventArgs e) 
        {
            RichTextBox richTextBox = sender as RichTextBox;

            if (richTextBox is null)
            {
                return;
            }

            if (string.IsNullOrEmpty(richTextBox.Text))
            {
                SetIsPlaceholderApplied(richTextBox, true);

                richTextBox.Text = GetPlaceholder(richTextBox);
            }
        }

        private static void OnGotKeyboardFocus(object sender, RoutedEventArgs e) 
        {
            RichTextBox richTextBox = sender as RichTextBox;

            if (richTextBox is null)
            {
                return;
            }

            if (GetIsPlaceholderApplied(richTextBox))
            {
                SetIsPlaceholderApplied(richTextBox, false);

                richTextBox.Text = string.Empty;
            }
        }
    }
}
