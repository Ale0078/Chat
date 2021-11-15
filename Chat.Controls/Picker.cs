using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Emoji.Wpf;

namespace Chat.Controls
{
    public class Picker : Control
    {
        private bool _isLoaded;

        public static readonly RoutedEvent PickEvent;
        public static readonly DependencyProperty EmojiGroupsProperty;
        public static readonly DependencyProperty PickedEmojiProperty;
        public static readonly DependencyProperty CornerRadiusProperty;

        static Picker()
        {
            PickEvent = EventManager.RegisterRoutedEvent(
                name: nameof(Pick),
                routingStrategy: RoutingStrategy.Bubble,
                handlerType: typeof(RoutedEventHandler),
                ownerType: typeof(Picker));

            EmojiGroupsProperty = DependencyProperty.Register(
                name: nameof(EmojiGroups),
                propertyType: typeof(IList<EmojiData.Group>),
                ownerType: typeof(Picker),
                typeMetadata: new PropertyMetadata(EmojiData.AllGroups));

            PickedEmojiProperty = DependencyProperty.Register(
                name: nameof(PickedEmoji),
                propertyType: typeof(string),
                ownerType: typeof(Picker));

            CornerRadiusProperty = DependencyProperty.Register(
                name: nameof(CornerRadius),
                propertyType: typeof(CornerRadius),
                ownerType: typeof(Picker));
        }

        public Picker()
        {
            EventManager.RegisterClassHandler(
                classType: typeof(Picker),
                routedEvent: Button.ClickEvent,
                handler: new RoutedEventHandler(OnEmojiPicked));
        }

        public event RoutedEventHandler Pick 
        {
            add => AddHandler(PickEvent, value);
            remove => RemoveHandler(PickEvent, value);
        }

        public IList<EmojiData.Group> EmojiGroups 
        {
            get => (IList<EmojiData.Group>)GetValue(EmojiGroupsProperty);
            set => SetValue(EmojiGroupsProperty, value);
        }

        public string PickedEmoji 
        {
            get => (string)GetValue(PickedEmojiProperty);
            set => SetValue(PickedEmojiProperty, value);
        }

        public CornerRadius CornerRadius 
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        protected virtual void OnEmojiPicked(object sender, RoutedEventArgs e) 
        {
            if (e.OriginalSource is Button buttonWithEmoji)
            {
                PickedEmoji = ((EmojiData.Emoji)buttonWithEmoji.Content).Text;

                RaiseEvent(new RoutedEventArgs(Picker.PickEvent));
            }
        }
    }
}
