using System.Windows;
using System.Windows.Controls;

using Chat.Client.ViewModel;

namespace Chat.Client.TemplateSelectors
{
    public class ListOfUsersSelector : DataTemplateSelector
    {
        public DataTemplate UserTemplate { get; set; }
        public DataTemplate GroupTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container) => item is ChatMemberViewModel
            ? UserTemplate
            : GroupTemplate;
    }
}
