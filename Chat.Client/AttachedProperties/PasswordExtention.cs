using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Chat.Client.AttachedProperties
{
    public static class PasswordExtention
    {
        private static readonly DependencyProperty PasswordInitializedProperty;

        public static readonly DependencyProperty PasswordProperty;

        static PasswordExtention() 
        {
            PasswordInitializedProperty = DependencyProperty.RegisterAttached(
                name: "PasswordInitialized",
                propertyType: typeof(bool),
                ownerType: typeof(PasswordExtention),
                defaultMetadata: new FrameworkPropertyMetadata(false));

            PasswordProperty = DependencyProperty.RegisterAttached(
                name: "Password",
                propertyType: typeof(string),
                ownerType: typeof(PasswordExtention),
                defaultMetadata: new FrameworkPropertyMetadata(string.Empty, HandelPasswrodChangedCallback) 
                {
                    BindsTwoWayByDefault = true,
                    DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });
        }

        private static void HandelPasswrodChangedCallback(DependencyObject db, DependencyPropertyChangedEventArgs e) 
        {
            PasswordBox password = db as PasswordBox;

            if (password is null)
            {
                return;
            }

            if (!(bool)password.GetValue(PasswordInitializedProperty))
            {
                password.SetValue(PasswordInitializedProperty, true);

                password.PasswordChanged += HandelPasswordChanged;
            }
        }

        private static void HandelPasswordChanged(object sender, RoutedEventArgs e) =>
            SetPassword((PasswordBox)sender, ((PasswordBox)sender).Password);

        public static string GetPassword(DependencyObject obj) =>
            (string)obj.GetValue(PasswordProperty);

        public static void SetPassword(DependencyObject obj, string value) =>
            obj.SetValue(PasswordProperty, value);
    }
}