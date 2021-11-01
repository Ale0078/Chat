using System.Windows;
using System.Windows.Controls;

using Chat.Client.ViewModel;

namespace Chat.Client.TemplateSelectors
{
    public class MessageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate MessageTemplate { get; set; }
        public DataTemplate GroupMessageTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ChatMessageViewModel)
            {
                return MessageTemplate;
            }

            return GroupMessageTemplate;
        }
    }
}
