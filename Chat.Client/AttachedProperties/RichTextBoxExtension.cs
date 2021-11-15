using System.Windows;
using Emoji.Wpf;

using Chat.Client.ViewModel;

namespace Chat.Client.AttachedProperties
{
    public static class RichTextBoxExtension
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
                ownerType: typeof(RichTextBoxExtension),
                defaultMetadata: new PropertyMetadata(
                    propertyChangedCallback: OnIsPlaceholderAppliedChanged));

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

        private static void OnIsPlaceholderAppliedChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) 
        {
            RichTextBox richTextBox = obj as RichTextBox;

            if (richTextBox is null)
            {
                return;
            }

            ChatViewModel chat = richTextBox.DataContext as ChatViewModel;

            if (!(bool)richTextBox.GetValue(IsPlaceholderSettedProperty))
            {
                richTextBox.SetValue(IsPlaceholderSettedProperty, true);

                if (string.IsNullOrEmpty(chat.User.MessageCreater.Placeholder))
                {
                    chat.User.MessageCreater.Placeholder = GetPlaceholder(richTextBox);
                }
            }
        }

        private static void OnPlaceholderChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) 
        {
            RichTextBox richTextBox = obj as RichTextBox;
            
            if (richTextBox is null || richTextBox.DataContext is null)
            {
                return;
            }

            ChatViewModel chat = richTextBox.DataContext as ChatViewModel;

            chat.User.MessageCreater.Placeholder = GetPlaceholder(richTextBox);
        }
    }
}
