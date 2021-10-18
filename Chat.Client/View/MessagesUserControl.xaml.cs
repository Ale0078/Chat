using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Chat.Client.View
{
    public partial class MessagesUserControl : UserControl
    {
        public static readonly DependencyProperty TypeControlNameScopeToFileMessageProperty;
        public static readonly DependencyProperty TimeLineAnimationToFileMessageProperty;

        public MessagesUserControl()
        {
            InitializeComponent();
        }

        static MessagesUserControl() 
        {
            TypeControlNameScopeToFileMessageProperty = DependencyProperty.Register(
                name: nameof(TypeControlNameScopeToFileMessage),
                propertyType: typeof(Type),
                ownerType: typeof(MessagesUserControl));

            TimeLineAnimationToFileMessageProperty = DependencyProperty.Register(
                name: nameof(TimeLineAnimationToFileMessage),
                propertyType: typeof(Timeline),
                ownerType: typeof(MessagesUserControl));
        }

        public Type TypeControlNameScopeToFileMessage
        {
            get => (Type)GetValue(TypeControlNameScopeToFileMessageProperty);
            set => SetValue(TypeControlNameScopeToFileMessageProperty, value);
        }

        public Timeline TimeLineAnimationToFileMessage
        {
            get => (Timeline)GetValue(TimeLineAnimationToFileMessageProperty);
            set => SetValue(TimeLineAnimationToFileMessageProperty, value);
        }
    }
}
